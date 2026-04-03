using AppPublication.Export;
using AppPublication.Models.EcransAppel;
using AppPublication.Publication;
using FranceJudo.Core.IO;
using FranceJudo.Core.Export;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using FranceJudo.Metier.Noyau;
using System;
using System.Collections.Generic;


namespace AppPublication.Generation
{
    public class GenerateurSiteInterne : IGenerateurSite, IConfigurableGenerateur<ConfigurationExportSiteInterne>
    {
        #region MEMBRES
        // Les gestionnaires
        readonly private IJudoDataManager _judoDataManager;                  // Le gestionnaire de données interne
        private IJudoData _snapshot;                                // Le snapshot des données 

        EcranCollectionManager _ecransAppel;                        // La configuration des ecrans d'appel (pour les combats)

        // La structure du site
        private SiteInterneUrlGenerator _siteInterneUrlGenerator;      // La structure de repertoire d'export du site
        private ExportSharedContextInterne _currentContext;             // Le contexte de generation partage (donnees statiques communes a toutes les taches)
        private readonly ConfigurationExportSiteInterne _cfgExport;


        // Suivi des taches de generation
        private EtapeGenerateurSiteEnum _etapeCourante = EtapeGenerateurSiteEnum.None;
        private readonly ParallelTaskBatcher<OperationProgress, FileWithChecksum> _taskBatcher;          // Le gestionnaire de taches paralleles
        List<FileWithChecksum> _checksumGenere = new List<FileWithChecksum>();                     // Les fichiers generes lors de la derniere generation  
        #endregion

        #region PROPERTIES PUBLIQUES

        /// <summary>
        /// La configuration de l'export (version ReadOnly)
        /// </summary>
        public IReadOnlyConfigurationExportSiteInterne ConfigurationGeneration
        {
            get
            {
                return _cfgExport;
            }
        }

        // Le gestionnaire est fortement typé, mais n'est exposé que via l'interface IConfigurableGenerateur
        public ThreadSafeConfigManager<ConfigurationExportSiteInterne> ExportConfigurationManager { get; }

        /// <summary>
        /// La structure de repertoire utilisee pour l'export du site
        /// </summary>
        public SiteInterneUrlGenerator StructureSiteGenerator
        {
            get { return _siteInterneUrlGenerator; }
            set { _siteInterneUrlGenerator = value; }
        }



        private EcranCollectionManager EcransAppel
        {
            get
            {
                return _ecransAppel;
            }
            set
            {
                _ecransAppel = value;
            }
        }

        #endregion

        #region CONSTRUCTEURS

        public GenerateurSiteInterne(IJudoDataManager dataManager, EcranCollectionManager ecransAppel, IProgress<OperationProgress> progressHandler)
        {
            _judoDataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            _ecransAppel = ecransAppel;
            _cfgExport = new ConfigurationExportSiteInterne();     // Init par defaut
            ExportConfigurationManager = new ThreadSafeConfigManager<ConfigurationExportSiteInterne>(_cfgExport);

            try
            {
                // Initialise le gestionnaire de taches paralleles
                _taskBatcher = new ParallelTaskBatcher<OperationProgress, FileWithChecksum>(progressHandler, (f) => { return new OperationProgress(_etapeCourante, f); });
            }
            catch (Exception ex)
            {
                LogTools.Logger.Fatal(ex, "Impossible d'initialiser le generateur de Site Interne. Impossible de continuer");
                throw new NotSupportedException("Impossible d'initialiser le generateur de Site Interne. Impossible de continuer", ex);
            }
        }
        #endregion

        #region IMPLEMENTATION IGenerateurSite

        /// <summary>
        /// Effectue le nettoyage initial du site interne.
        /// </summary>
        /// <returns></returns>
        public ResultatOperation CleanupInitial()
        {
            _etapeCourante = EtapeGenerateurSiteEnum.CleanupInitial;
            try
            {
                // Efface le contenu local
                ClearRepertoireCompetition();
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors du nettoyage initial du site");
                return new ResultatOperation(EtapeGenerateurSiteEnum.CleanupInitial, false, true, -1);
            }

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.CleanupInitial, true, true, -1);
        }

        /// <summary>
        /// Effectue le démarrage du site interne.
        /// </summary>
        /// <returns></returns>
        public ResultatOperation Demarrage()
        {
            return new ResultatOperation(EtapeGenerateurSiteEnum.Demarrage, true, true, -1);
        }

        /// <summary>
        /// Prépare la génération du site interne en vérifiant la consistance des données.
        /// </summary>
        /// <returns></returns>
        public ResultatOperation PrepareGeneration()
        {
            _etapeCourante = EtapeGenerateurSiteEnum.PrepareGeneration;

            // Commence par garantir que les données des caches sont consistantes
            bool dataConsistent = false;
            try
            {
                // Appel bloquant (avec timeout) vers GestionEvent
                dataConsistent = _judoDataManager.EnsureDataConsistency();
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors du controle de la consistance donnees recues.");
            }

            if (dataConsistent)
            {
                // Recupere le snapshot des données (thread safe)
                _snapshot = _judoDataManager.Snapshot;

                ConfigurationExportSiteInterne snapshotConfig = ExportConfigurationManager.Snapshot;   // Récupère une copie de la configuration (thread safe)

                // Initialise les donnees partagees de generation (ces donnees sont statiques et communes a toutes les taches)
                _currentContext = ExportSharedContextInterne.Create(_snapshot, snapshotConfig);
            }
            else
            {
                // Le controle d'integrite a echoue
                LogTools.Logger.Warn("Impossible de valider l'integrite des donnees combats (Timeout ou deconnexion).");
            }

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.PrepareGeneration, dataConsistent, true, -1);
        }

        /// <summary>
        /// Effectue la génération du site interne.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ResultatOperation ExecuteGeneration()
        {
            _etapeCourante = EtapeGenerateurSiteEnum.ExecuteGeneration;
            // La liste de sortie
            List<FileWithChecksum> output = new List<FileWithChecksum>();   // La liste de sortie

            // Si un taskbatcher en toujours en cours, ce n'est pas normal. plutot un exception que Silent car ce cas ne devrait pas arriver
            if (_taskBatcher.HasPendingWork)
            {
                LogTools.Logger.Debug("Batch precedent toujours en cours, exception levee");
                throw new InvalidOperationException("Batch precedent toujours en cours");
            }

            // Si pas de donnees, pas la peine de continuer
            if (_snapshot.Organisation.Competitions.Count > 0 && _ecransAppel != null)
            {
                try
                {
                    // Ok, a partir d'ici on peut lancer les tasks dans le batcher
                    ExportSiteInterne<ExportSharedContextInterne> exporter = new ExportSiteInterne<ExportSharedContextInterne>(_currentContext);     // L'exporteur

                    // La racine du site
                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereWebSiteIndex(_snapshot, _currentContext, _siteInterneUrlGenerator, p);
                    });

                    foreach (var ecran in _ecransAppel.Ecrans)
                    {
                        _taskBatcher.AddWork(p =>
                        {
                            return exporter.GenereEcranAppel(_snapshot, _currentContext, _siteInterneUrlGenerator, ecran, p);
                        });
                    }

                    // et on ajoute le traitement par default
                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereEcranAppel(_snapshot, _currentContext, _siteInterneUrlGenerator, _ecransAppel.Default, p);
                    });

                    // Attend la fin de tous les batchs
                    output = _taskBatcher.WaitAllAndGetResults();
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur lors de la generation");
                }
            }
            else
            {
                LogTools.Logger.Debug("Aucune competition presente dans le snapshot ou aucun ecrans d'appel configures, generation avortee");
            }

            _checksumGenere = output;

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteGeneration, _checksumGenere.Count > 0, true, _checksumGenere.Count);
        }

        public ResultatOperation ExecuteSynchronisation()
        {
            // Rien a faire dans ce generteur
            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteSynchronisation, true, true);
        }

        #endregion

        #region METHODES INTERNES
        /// <summary>
        /// Vide le contenu du repertoire de la competition
        /// </summary>
        private void ClearRepertoireCompetition()
        {
            if (_siteInterneUrlGenerator != null)
            {
                // On délègue totalement le nettoyage (disque + cache) à la structure physique
                if (!_siteInterneUrlGenerator.PhysicalStructure.EffacerRepertoireCompetition())
                {
                    LogTools.Logger.Error("Erreur lors de l'effacement du contenu de '{0}'", _siteInterneUrlGenerator.PhysicalStructure.RepertoireCompetition);
                }
            }
        }
        #endregion
    }
}
