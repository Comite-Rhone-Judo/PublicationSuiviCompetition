using AppPublication.Controles;
using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Export;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Export;
using Tools.Logging;
using Tools.Threading;
using Tools.Files;
using Tools.Net;


namespace AppPublication.Generation
{
    public class GenerateurSiteInterne : IGenerateurSite
    {
        #region MEMBRES
        // Les gestionnaires
        private IJudoDataManager _judoDataManager;                  // Le gestionnaire de données interne
        private IJudoData _snapshot;                                // Le snapshot des données 
        private IProgress<OperationProgress> _progressHandler;      // Utilise pour le suivi de progression

        // La structure du site
        private ExportSiteInterneStructure _structureRepertoiresSiteInterne;      // La structure de repertoire d'export du site

        // Suivi des taches de generation
        private EtapeGenerateurSiteEnum _etapeCourante = EtapeGenerateurSiteEnum.None;
        private readonly ParallelTaskBatcher<OperationProgress, FileWithChecksum> _taskBatcher;          // Le gestionnaire de taches paralleles
        List<FileWithChecksum> _checksumGenere = new List<FileWithChecksum>();                     // Les fichiers generes lors de la derniere generation  
        #endregion

        #region PROPERTIES PUBLIQUES

        /// <summary>
        /// La structure de repertoire utilisee pour l'export du site
        /// </summary>
        public ExportSiteInterneStructure StructureRepertoire
        {
            get { return _structureRepertoiresSiteInterne; }
            set { _structureRepertoiresSiteInterne = value; }
        }

        private ConfigurationExportSiteInterne _cfgExport = new ConfigurationExportSiteInterne();     // Init par defaut
        /// <summary>
        /// La configuration de l'export (version simple)
        /// </summary>
        public ConfigurationExportSiteInterne ConfigurationGeneration
        {
            get {
                if (_cfgExport == null)
                {
                    _cfgExport = new ConfigurationExportSiteInterne();
                }
                return _cfgExport;
            }
            private set { _cfgExport = value; }
        }

        #endregion

        #region CONSTRUCTEURS

        public GenerateurSiteInterne(IJudoDataManager dataManager, IProgress<OperationProgress> progressHandler)
        {
            _judoDataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            _progressHandler = progressHandler;

            try
            {
                // Initialise le gestionnaire de taches paralleles
                _taskBatcher = new ParallelTaskBatcher<OperationProgress, FileWithChecksum>(progressHandler, (f) => { return new OperationProgress(_etapeCourante, f);});
            }
            catch (Exception ex)
            {
                LogTools.Logger.Fatal(ex, "Impossible d'initialiser le generateur de Site Interne. Impossible de continuer");
                throw new NotSupportedException("Impossible d'initialiser le generateur de Site Interne. Impossible de continuer", ex);
            }
        }
        #endregion

        #region IMPLEMENTATION IGenerateurSite

        public ResultatOperation CleanupInitial()
        {
            _etapeCourante = EtapeGenerateurSiteEnum.CleanupInitial;
            try
            {
                // Efface le contenu local
                ClearRepertoireCompetition();
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors du nettoyage initial du site");
                return new ResultatOperation(EtapeGenerateurSiteEnum.CleanupInitial, false, true, -1);
            }

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.CleanupInitial, true, true, -1);
        }

        public ResultatOperation Demarrage()
        {
            return new ResultatOperation(EtapeGenerateurSiteEnum.Demarrage, true, true, -1);
        }

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

                // Initialise les donnees partagees de generation (ces donnees sont statiques et communes a toutes les taches)
                // TODO A Revoir
                // ExportSiteInterne.InitSharedData(_snapshot, _extendedJudoData, ConfigurationGeneration, true);
            }
            else
            {
                // Le controle d'integrite a echoue
                LogTools.Logger.Warn("Impossible de valider l'integrite des donnees combats (Timeout ou deconnexion).");
            }

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.PrepareGeneration, dataConsistent, true, -1);
        }

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
            if (_snapshot.Organisation.Competitions.Count > 0)
            {
                try
                {
                    // Ok, a partir d'ici on peut lancer les tasks dans le batcher
                    ExportSiteInterne exporter = new ExportSiteInterne();     // L'exporteur

                    // TODO C'est ici que l'on va mettre les fonction de traitement
                    /*
                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereWebSiteIndex(_snapshot, _cfgExport, _structureRepertoiresSite, p);
                    });
                    */


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
                LogTools.Logger.Debug("Aucune competition presente dans le snapshot, generation avortee");
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
            if (_structureRepertoiresSiteInterne != null)
            {
                // Efface le contenu du repertoire de la competition
                if (!FileAndDirectTools.DeleteDirectory(_structureRepertoiresSiteInterne.RepertoireCompetition(), true))
                {
                    LogTools.Logger.Error("Erreur lors de l'effacement du contenu de  '{0}'", _structureRepertoiresSiteInterne.RepertoireCompetition());
                }
            }
        }
        #endregion
    }
}
