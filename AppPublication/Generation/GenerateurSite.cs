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
    public class GenerateurSite : IGenerateurSite
    {
        #region MEMBRES
        // Les gestionnaires
        private IJudoDataManager _judoDataManager;                  // Le gestionnaire de données interne
        private IJudoData _snapshot;                                // Le snapshot des données 
        private ExtendedJudoData _extendedJudoData;
        private MiniSite _site = null;                              // Le site a utilise pour le upload a distance
        private IProgress<OperationProgress> _progressHandler;      // Utilise pour le suivi de progression

        // La structure du site
        private ExportSiteStructure _structureRepertoiresSite;      // La structure de repertoire d'export du site

        // Suivi des taches de generation
        private EtapeGenerateurSiteEnum _etapeCourante = EtapeGenerateurSiteEnum.None;
        private readonly ParallelTaskBatcher<OperationProgress, FileWithChecksum> _taskBatcher;          // Le gestionnaire de taches paralleles
        List<FileWithChecksum> _checksumCache = new List<FileWithChecksum>();                      // Les fichiers en cache pour le controle des checksums
        List<FileWithChecksum> _checksumGenere = new List<FileWithChecksum>();                     // Les fichiers generes lors de la derniere generation  
        #endregion

        #region PROPERTIES PUBLIQUES

        /// <summary>
        /// La structure de repertoire utilisee pour l'export du site
        /// </summary>
        public ExportSiteStructure StructureRepertoire
        {
            get { return _structureRepertoiresSite; }
            set { _structureRepertoiresSite = value; }
        }

        private ConfigurationExportSite _cfgExport = new ConfigurationExportSite();     // Init par defaut
        /// <summary>
        /// La configuration de l'export (version simple)
        /// </summary>
        public ConfigurationExportSite ConfigurationGeneration
        {
            get {
                if (_cfgExport == null)
                {
                    _cfgExport = new ConfigurationExportSite();
                }
                return _cfgExport;
            }
            private set { _cfgExport = value; }
        }

        /// <summary>
        /// Le gestion de site distant pour faire un transfert FTP
        /// </summary>
        public MiniSite SiteProvider
        {
            get
            {
                return _site;
            }
            set
            {
                if (_site != value)
                {
                    _site = value;
                }
            }
        }
        #endregion

        #region CONSTRUCTEURS

        public GenerateurSite(IJudoDataManager dataManager, MiniSite siteDistant, IProgress<OperationProgress> progressHandler)
        {
            _judoDataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            _extendedJudoData = new ExtendedJudoData() ?? throw new NullReferenceException(nameof(_extendedJudoData));
            _progressHandler = progressHandler;
            _site = siteDistant;

            try
            {
                // Initialise le gestionnaire de taches paralleles
                _taskBatcher = new ParallelTaskBatcher<OperationProgress, FileWithChecksum>(progressHandler, (f) => { return new OperationProgress(_etapeCourante, f);});
            }
            catch (Exception ex)
            {
                LogTools.Logger.Fatal(ex, "Impossible d'initialiser le generateur de Site interne. Impossible de continuer");
                throw new NotSupportedException("Impossible d'initialiser le generateur de Site interne. Impossible de continuer", ex);
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

                // Efface egalement le fichier a distance s'il est actif
                if (_site != null && _site.IsActif)
                {
                    _site.NettoyerSite();
                }
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

                // Met a jour les données de l'extension
                _extendedJudoData.SyncAll(_snapshot);

                // Initialise les donnees partagees de generation (ces donnees sont statiques et communes a toutes les taches)
                ExportSite.InitSharedData(_snapshot, _extendedJudoData, ConfigurationGeneration, true);

                // Charge le contenu du fichier de checksum
                LoadChecksumFichiersGeneres();
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
                    ExportSite exporter = new ExportSite();     // L'exporteur

                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereWebSiteIndex(_snapshot, _cfgExport, _structureRepertoiresSite, p);
                    });

                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereWebSiteMenu(_snapshot, _extendedJudoData, _cfgExport, _structureRepertoiresSite, p);
                    });

                    if (_cfgExport.PublierAffectationTapis)
                    {
                        _taskBatcher.AddWork(p =>
                        {
                            return exporter.GenereWebSiteAffectation(_snapshot, _cfgExport, _structureRepertoiresSite, p);
                        });
                    }

                    // On ne genere pas les informations de prochains combat si ce n'est pas necessaire
                    if (_cfgExport.PublierProchainsCombats)
                    {
                        _taskBatcher.AddWork(p =>
                        {
                            return exporter.GenereWebSiteAllTapis(_snapshot, _cfgExport, _structureRepertoiresSite, p);
                        });
                    }

                    if (_cfgExport.PublierEngagements)
                    {
                        foreach (Competition comp in _snapshot.Organisation.Competitions)
                        {
                            // Recupere les groupes en fonction du type de groupement
                            List<EchelonEnum> typesGrp = _extendedJudoData.Engagement.TypesGroupes[comp.id];

                            // On genere les engagements pour chaque type de groupe
                            foreach (EchelonEnum typeGrp in typesGrp)
                            {
                                List<GroupeEngagements> groupesP = _extendedJudoData.Engagement.GroupesEngages.Where(g => g.Competition == comp.id && g.Type == (int)typeGrp).ToList();

                                // Ce code est plus efficace qye celui qui cree une tache par groupe
                                // sans doute car le lancement de nombreuses Task est couteux mais il provoque une latence a la fin de la generation
                                _taskBatcher.AddWork(p =>
                                {
                                    return exporter.GenereWebSiteEngagements(_snapshot, _extendedJudoData, groupesP, _cfgExport, _structureRepertoiresSite, p);
                                });

                                // foreach (GroupeEngagements g in groupesP)
                                // {
                                //   _nbTaskGeneration++;
                                //  listTaskGeneration.Add(AddWork(SiteEnum.Engagements, null, null, cfg, new List<GroupeEngagements>(1) { g }));
                                // }
                            }
                        }
                    }

                    foreach (Phase phase in _snapshot.Deroulement.Phases)
                    {
                        _taskBatcher.AddWork(p =>
                        {
                            return exporter.GenereWebSitePhase(_snapshot, phase, _cfgExport, _structureRepertoiresSite, p);
                        });
                        _taskBatcher.AddWork(p =>
                        {
                            return exporter.GenereWebSiteClassement(_snapshot, phase.GetVueEpreuve(_snapshot), _cfgExport, _structureRepertoiresSite, p);
                        });
                    }

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
            _etapeCourante = EtapeGenerateurSiteEnum.ExecuteSynchronisation;
            UploadStatus uploadOut = new UploadStatus();

            // Si le site distant est actif, transfere la mise a jour
            if (_site != null && !_site.IsLocal && _site.IsActif)
            {
                try
                {
                    string localRoot = _structureRepertoiresSite.RepertoireCompetition();

                    // Calcul les fichiers a prendre en compte
                    List<FileInfo> filesToSync = null;
                    if (_checksumCache != null && _checksumCache.Count > 0)
                    {
                        // Extrait les fichiers generes qui sont differents du cache
                        List<FileWithChecksum> chkToSync = _checksumGenere.Except(_checksumCache, new FileWithChecksumComparer()).ToList();
                        filesToSync = chkToSync.Select(o => o.File).ToList();

                        // For Debug only
                        if (filesToSync.Count <= 0)
                        {
                            LogTools.Logger.Debug("Fichiers a synchroniser: {0}", string.Join(",", filesToSync.Select(f => f.Name)));
                        }
                    }

                    // Synchronise le site FTP.
                    uploadOut = _site.UploadSite(localRoot, filesToSync);
                    if (uploadOut.IsSuccess)
                    {
                        // Enregistre les checksums en cache maintenant qu'on sait que l'etat distant est synchrone
                        SaveChecksumFichiersGeneres();
                    }
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Une erreur est survenue pendant la tentative de synchronisation");
                }
            }
            else
            {
                LogTools.Logger.Debug("Site distant inactif, pas de upload FTP");
            }

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteSynchronisation, uploadOut.IsSuccess, uploadOut.IsComplet, uploadOut.nbUpload);
        }

        #endregion

        #region METHODES INTERNES

        /// <summary>
        /// Charge le fichier de cache de checksum
        /// </summary>
        /// <param name=""></param>
        /// <returns>Liste vide si le fichier n'existe pas</returns>
        private void LoadChecksumFichiersGeneres()
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            try
            {
                // Charge le fichier
                XDocument doc = XDocument.Load(ChecksumFileName);

                // Recherche la racine
                List<XElement> rootElem = doc.Descendants(ConstantXML.checksums).ToList();

                if (rootElem.Count() >= 1)
                {
                    output = ExportXML.ImportChecksumFichiers(rootElem.First());
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }

            _checksumCache = output;
        }

        /// <summary>
        /// Nom du fichier de cache utiliser pour le controle des checksums
        /// </summary>
        private string ChecksumFileName
        {
            get
            {
                string output = string.Empty;
                // Normalement on ne devrait pas avoir de probleme d'exception ici avec la structure de repertoire
                try
                {
                    output = Path.Combine(_structureRepertoiresSite.RepertoireRacine, ExportTools.getFileName(ExportEnum.Site_Checksum) + ConstantFile.ExtensionXML);
                }
                catch (Exception ex)
                {
                    output = string.Empty;
                    LogTools.Logger.Error(ex, "Impossible de calculer le nom du fichier Checksum");
                }

                return output;
            }
        }

        /// <summary>
        /// Vide le contenu du repertoire de la competition
        /// </summary>
        private void ClearRepertoireCompetition()
        {
            if (_structureRepertoiresSite != null)
            {
                // Efface le contenu du repertoire de la competition
                if (!FileAndDirectTools.DeleteDirectory(_structureRepertoiresSite.RepertoireCompetition(), true))
                {
                    LogTools.Logger.Error("Erreur lors de l'effacement du contenu de  '{0}'", _structureRepertoiresSite.RepertoireCompetition());
                }

                // Charge le contenu du fichier de checksum
                LoadChecksumFichiersGeneres();

                // Elimine tous les fichiers commençant par le répertoire de la competition (ils ont été supprimés)
                _checksumCache.RemoveAll(f => f.File.FullName.StartsWith(_structureRepertoiresSite.RepertoireCompetition()));
                SaveChecksumFichiersGeneres();
            }
        }

        /// <summary>
        /// Sauvegarde une liste de fichiers generes dans le cache de checksum (ecrase le precedent)
        /// </summary>
        /// <param name="fichiersGeneres"></param>
        private void SaveChecksumFichiersGeneres()
        {
            // Enregistre les checksums des fichiers generes
            XDocument doc = ExportXML.ExportChecksumFichiers(_checksumGenere);

            if (doc != null && !File.Exists(ChecksumFileName) || !FileAndDirectTools.IsFileLocked(ChecksumFileName))
            {
                FileAndDirectTools.NeedAccessFile(ChecksumFileName);
                try
                {
                    using (FileStream fs = new FileStream(ChecksumFileName, FileMode.Create))
                    {
                        doc.Save(fs);
                    }
                }
                catch (Exception ex)
                {
                    LogTools.Error(ex);
                }
                finally
                {
                    FileAndDirectTools.ReleaseFile(ChecksumFileName);
                }
            }
        }   

        #endregion
    }
}
