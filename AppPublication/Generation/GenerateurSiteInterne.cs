using AppPublication.Export;
using AppPublication.Models.EcransAppel;
using AppPublication.Publication;
using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using FranceJudo.Metier.Noyau;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AppPublication.Generation
{
    public class GenerateurSiteInterne : IGenerateurSite, IConfigurableGenerateur<ConfigurationExportSiteInterne>
    {

        #region MEMBRES
        // Les gestionnaires
        readonly private IJudoDataManager _judoDataManager;                  // Le gestionnaire de données interne
        private IJudoData _snapshot;                                // Le snapshot des données 
        private EcranCollectionSnapshot _ecransAppelSnapshot;

        EcranCollectionManager _ecransAppel;                        // La configuration des ecrans d'appel (pour les combats)

        // La structure du site
        private SiteInterneUrlGenerator _siteInterneUrlGenerator;      // La structure de repertoire d'export du site
        private ExportSharedContextInterne _currentContext;             // Le contexte de generation partage (donnees statiques communes a toutes les taches)
        private readonly ConfigurationExportSiteInterne _cfgExport;


        // Suivi des taches de generation
        private EtapeGenerateurSiteEnum _etapeCourante = EtapeGenerateurSiteEnum.None;
        private readonly ParallelTaskBatcher<OperationProgress, FileWithChecksum> _taskBatcher;          // Le gestionnaire de taches paralleles
        List<FileWithChecksum> _checksumGenere = new List<FileWithChecksum>();                     // Les fichiers generes lors de la derniere generation  
        private readonly int _nbCoeurs = Environment.ProcessorCount;                                 // Constantes de découpage pour le batching (a ajuster en fonction du cout de generation des phases et des engagements)
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
                LogTools.Logger?.Fatal(ex, "Impossible d'initialiser le generateur de Site Interne. Impossible de continuer");
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
                LogTools.Logger?.Error(ex, "Erreur lors du nettoyage initial du site");
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
                LogTools.Logger?.Error(ex, "Exception lors du controle de la consistance donnees recues.");
            }

            if (dataConsistent)
            {
                // Recupere le snapshot des données (thread safe)
                _snapshot = _judoDataManager.Snapshot;

                ConfigurationExportSiteInterne snapshotConfig = ExportConfigurationManager.Snapshot;   // Récupère une copie de la configuration (thread safe)

                // Initialise les donnees partagees de generation (ces donnees sont statiques et communes a toutes les taches)
                _currentContext = ExportSharedContextInterne.Create(_snapshot, snapshotConfig);

                _ecransAppelSnapshot = _ecransAppel?.Snapshot;   // Récupère une copie de la configuration des écrans d'appel (thread safe)
            }
            else
            {
                // Le controle d'integrite a echoue
                LogTools.Logger?.Warn("Impossible de valider l'integrite des donnees combats (Timeout ou deconnexion).");
            }

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.PrepareGeneration, dataConsistent, true, -1);
        }

        /// <summary>
        /// Effectue la génération du site interne.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ResultatOperation> ExecuteGeneration()
        {
            _etapeCourante = EtapeGenerateurSiteEnum.ExecuteGeneration;
            // La liste de sortie
            List<FileWithChecksum> output = new List<FileWithChecksum>();   // La liste de sortie

            // Si un taskbatcher en toujours en cours, ce n'est pas normal. plutot un exception que Silent car ce cas ne devrait pas arriver
            if (_taskBatcher.HasPendingWork)
            {
                LogTools.Logger?.Debug("Batch precedent toujours en cours, exception levee");
                throw new InvalidOperationException("Batch precedent toujours en cours");
            }

            // Si pas de donnees, pas la peine de continuer
            if (_snapshot.Organisation.Competitions.Count > 0 && _ecransAppelSnapshot != null)
            {
                try
                {
                    // Ok, a partir d'ici on peut lancer les tasks dans le batcher
                    ExportSiteInterne<ExportSharedContextInterne> exporter = new ExportSiteInterne<ExportSharedContextInterne>(_currentContext);     // L'exporteur

                    // La racine du site
                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereWebSiteIndex(_currentContext, _siteInterneUrlGenerator, p);
                    });

                    // --- OPTIMISATION : FUSION ET CHUNKING DES ÉCRANS ---

                    // 1. On regroupe tous les écrans (configurés + défaut) dans une seule liste matérialisée
                    List<EcranAppelModel> tousLesEcrans = new List<EcranAppelModel>(_ecransAppelSnapshot.Ecrans);
                    if (_ecransAppelSnapshot.Default != null)
                    {
                        tousLesEcrans.Add(_ecransAppelSnapshot.Default);
                    }

                    int nbChunk = 0;
                    int tailleChunk = Math.Max(5, tousLesEcrans.Count / _nbCoeurs); ; // Ajuste la taille du chunk en fonction du nombre de groupes et du nombre de coeurs, avec un minimum
                    LogTools.Logger?.Debug($"Taille de chunk pour Ecran Appel : {tailleChunk} sur {_nbCoeurs} coeurs");

                    // 2. Découpage par lots
                    foreach (var paquet in tousLesEcrans.Chunk(tailleChunk))
                    {
                        LogTools.Logger?.Debug($"Batching chunk Ecran Appel #{nbChunk++} (size = {paquet.Length}");
                        // 3. On envoie le lot complet à la méthode "au pluriel" et passe la taille du paquet pour le reporting de progression
                        _taskBatcher.AddWork(p =>
                        {
                            // Le thread utilisera son propre XPathDocument local pour traiter ces 10 écrans
                            return exporter.GenereEcransAppel(_currentContext, _siteInterneUrlGenerator, paquet, p);
                        }, paquet.Length);
                    }

                    // Attend la fin de tous les batchs
                    output = await _taskBatcher.WaitAllAndGetResultsAsync();
                }
                catch (Exception ex)
                {
                    LogTools.Logger?.Error(ex, "Erreur lors de la generation");
                }
            }
            else
            {
                LogTools.Logger?.Debug("Aucune competition presente dans le snapshot ou aucun ecrans d'appel configures, generation avortee");
            }

            _checksumGenere = output;

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteGeneration, _checksumGenere.Count > 0, true, _checksumGenere.Count);
        }

        public Task<ResultatOperation> ExecuteSynchronisation()
        {
            // Rien a faire dans ce generteur
            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return Task.FromResult(new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteSynchronisation, false));
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
                    LogTools.Logger?.Error("Erreur lors de l'effacement du contenu de '{0}'", _siteInterneUrlGenerator.PhysicalStructure.RepertoireCompetition);
                }
            }
        }
        #endregion
    }
}
