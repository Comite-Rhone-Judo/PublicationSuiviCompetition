using AppPublication.Export;
using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Publication;
using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Network;
using FranceJudo.Core.Threading;
using FranceJudo.Core.Utils;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppPublication.Generation
{
    public class GenerateurSite : IGenerateurSite, IConfigurableGenerateur<ConfigurationExportSite>
    {
        #region MEMBRES
        // Les gestionnaires
        readonly private IJudoDataManager _judoDataManager;                                         // Le gestionnaire de données interne
        private IJudoData _snapshot;                                                                // Le snapshot des données 
        private ExtendedJudoData _extendedJudoData;
        private MiniSite _site = null;                                                              // Le site a utilise pour le upload a distance
        private ExportSharedContext _currentContext = null;                                         // Le contexte de generation courant (a passer aux taches de generation)
        private readonly ConfigurationExportSite _cfgExport;

        // La structure du site
        private SiteUrlGenerator _siteUrlGenerator;                                                 // La structure de repertoire d'export du site

        // Suivi des taches de generation
        private EtapeGenerateurSiteEnum _etapeCourante = EtapeGenerateurSiteEnum.None;
        private readonly ParallelTaskBatcher<OperationProgress, FileWithChecksum> _taskBatcher;     // Le gestionnaire de taches paralleles
        List<FileWithChecksum> _checksumCache = new List<FileWithChecksum>();                       // Les fichiers en cache pour le controle des checksums
        List<FileWithChecksum> _checksumGenere = new List<FileWithChecksum>();                      // Les fichiers generes lors de la derniere generation  

        private readonly int _nbCoeurs = Environment.ProcessorCount;                                 // Constantes de découpage pour le batching (a ajuster en fonction du cout de generation des phases et des engagements)
        #endregion

        #region PROPERTIES PUBLIQUES

        /// <summary>
        /// La configuration de l'export (version ReadOnly)
        /// </summary>
        public IReadOnlyConfigurationExportSite ConfigurationGeneration
        {
            get
            {
                return _cfgExport;
            }
        }


        public ThreadSafeConfigManager<ConfigurationExportSite> ExportConfigurationManager { get; }

        /// <summary>
        /// La structure de repertoire utilisee pour l'export du site
        /// </summary>
        public SiteUrlGenerator StructureSiteGenerator
        {
            get { return _siteUrlGenerator; }
            set { _siteUrlGenerator = value; }
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
            _extendedJudoData = null;
            SiteProvider = siteDistant;
            _cfgExport = new ConfigurationExportSite();
            ExportConfigurationManager = new ThreadSafeConfigManager<ConfigurationExportSite>(_cfgExport);

            try
            {
                // Initialise le gestionnaire de taches paralleles
                _taskBatcher = new ParallelTaskBatcher<OperationProgress, FileWithChecksum>(progressHandler, (f) => { return new OperationProgress(_etapeCourante, f); });
            }
            catch (Exception ex)
            {
                LogTools.Logger.Fatal(ex, "Impossible d'initialiser le generateur de Site interne. Impossible de continuer");
                throw new NotSupportedException("Impossible d'initialiser le generateur de Site interne. Impossible de continuer", ex);
            }
        }
        #endregion

        #region IMPLEMENTATION IGenerateurSite

        /// <summary>
        /// Effectue le nettoyage initial du site, en supprimant les fichiers locaux et distants si nécessaire.
        /// </summary>
        /// <returns></returns>
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
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors du nettoyage initial du site");
                return new ResultatOperation(EtapeGenerateurSiteEnum.CleanupInitial, false, true, -1);
            }

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.CleanupInitial, true, true, -1);
        }

        /// <summary>
        /// Démarre le générateur de site.
        /// </summary>
        /// <returns></returns>
        public ResultatOperation Demarrage()
        {
            return new ResultatOperation(EtapeGenerateurSiteEnum.Demarrage, true, true, -1);
        }

        /// <summary>
        /// Prépare la génération du site en vérifiant la consistance des données et en initialisant le contexte partagé.
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

                // Met a jour les données de l'extension (ces données sont calculées en differe)
                _extendedJudoData =  new ExtendedJudoData(_snapshot);

                // Clone la configuration
                ConfigurationExportSite snapshotConfig;

                snapshotConfig = ExportConfigurationManager.Snapshot;

                // Initialise les donnees partagees de generation (ces donnees sont statiques et communes a toutes les taches)
                _currentContext = ExportSharedContext.Create(_snapshot, _extendedJudoData, snapshotConfig);

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

        /// <summary>
        /// Exécute la génération du site en utilisant les données et le contexte partagé.
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
                LogTools.Logger.Debug("Batch precedent toujours en cours, exception levee");
                throw new InvalidOperationException("Batch precedent toujours en cours");
            }

            // Si pas de donnees, pas la peine de continuer
            if (_snapshot.Organisation.Competitions.Count > 0)
            {
                try
                {
                    // Ok, a partir d'ici on peut lancer les tasks dans le batcher
                    ExportSite<ExportSharedContext> exporter = new ExportSite<ExportSharedContext>(_currentContext);     // L'exporteur

                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereWebSiteIndex(_currentContext, _siteUrlGenerator, p);
                    });

                    _taskBatcher.AddWork(p =>
                    {
                        return exporter.GenereWebSiteMenu(_currentContext, _siteUrlGenerator, p);
                    });

                    if (_cfgExport.PublierAffectationTapis)
                    {
                        _taskBatcher.AddWork(p =>
                        {
                            return exporter.GenereWebSiteAffectation(_currentContext, _siteUrlGenerator, p);
                        });
                    }

                    // On ne genere pas les informations de prochains combat si ce n'est pas necessaire
                    if (_cfgExport.PublierProchainsCombats)
                    {
                        _taskBatcher.AddWork(p =>
                        {
                            return exporter.GenereWebSiteAllTapis(_currentContext, _siteUrlGenerator, p);
                        });
                    }

                    if (_cfgExport.PublierEngagements)
                    {                      
                        foreach (ICompetition comp in _snapshot.Organisation.Competitions)
                        {
                            // Recupere les groupes en fonction du type de groupement
                            List<EchelonEnum> typesGrp = _extendedJudoData.Engagement.TypesGroupes[comp.id];

                            // On genere les engagements pour chaque type de groupe
                            foreach (EchelonEnum typeGrp in typesGrp)
                            {
                                List<GroupeEngagements> groupesP = _extendedJudoData.Engagement.GroupesEngages.Where(g => g.Competition == comp.id && g.Type == (int)typeGrp).ToList();

                                int nbChunkEng = 0;
                                int tailleChunkEngagement = Math.Max(20, groupesP.Count / (_nbCoeurs * 2)); ; // Ajuste la taille du chunk en fonction du nombre de groupes et du nombre de coeurs, avec un minimum de 1
                                LogTools.Logger.Debug($"Taille de chunk pour Engagement Competition {comp.nom}, groupe {typeGrp} : {tailleChunkEngagement} sur {_nbCoeurs} coeurs");
                                
                                // On fait un decoupe de la liste en paquet de n groupes pour limiter le nombre de taches (et donc le cout de lancement des taches) tout en gardant une bonne granularite pour le progress
                                foreach (var paquet in groupesP.Chunk(tailleChunkEngagement))
                                {
                                    LogTools.Logger.Debug($"Batching chunk Engagement Competition {comp.nom}, groupe {typeGrp}: #{nbChunkEng++} (size = {paquet.Length}");
                                    
                                    // Ce code est plus efficace qye celui qui cree une tache par groupe
                                    // car le lancement de trop nombreuses Task est couteux
                                    // Le paquet étant gros, on passe l'initialEstimate
                                    _taskBatcher.AddWork(p =>
                                    {
                                        return exporter.GenereWebSiteEngagements(paquet, _currentContext, _siteUrlGenerator, p);
                                    }, paquet.Length);
                                }
                            }
                        }
                    }

                    // On découpe les phases en paquets de 10
                    int tailleChunkPhase = Math.Max(5, _snapshot.Deroulement.Phases.Count / _nbCoeurs); ; // Ajuste la taille du chunk en fonction du nombre de groupes et du nombre de coeurs, avec un minimum
                    var chunksPhases = _snapshot.Deroulement.Phases.Chunk(tailleChunkPhase);
                    int nbChunkPhase = 0;
                    LogTools.Logger.Debug($"Taille de chunk pour Phases : {tailleChunkPhase} sur {_nbCoeurs} coeurs");

                    foreach (var paquet in chunksPhases)
                    {
                        // TRÈS IMPORTANT : Chaque phase génère 2 éléments (Phase + Classement)
                        // Donc l'estimation initiale pour ce paquet est : taille du paquet * 2
                        int estimationsPourCePaquet = paquet.Length * 2;
                        LogTools.Logger.Debug($"Batching chunk Phase #{nbChunkPhase++} (size = {paquet.Length}");

                        _taskBatcher.AddWork(p =>
                        {
                            List<FileWithChecksum> resultatsThread = new List<FileWithChecksum>();

                            // Le thread traite son lot de 10 phases de manière séquentielle
                            foreach (IPhase phase in paquet)
                            {
                                // 1. Génération de la phase
                                var fichiersPhase = exporter.GenereWebSitePhase(phase, _currentContext, _siteUrlGenerator, p);
                                if (fichiersPhase != null) resultatsThread.AddRange(fichiersPhase);

                                // 2. Génération du classement lié à cette phase
                                var fichiersClassement = exporter.GenereWebSiteClassement(phase.GetVueEpreuve(_snapshot), _currentContext, _siteUrlGenerator, p);
                                if (fichiersClassement != null) resultatsThread.AddRange(fichiersClassement);
                            }

                            return resultatsThread;

                        }, estimationsPourCePaquet); // Le fameux initialEstimate qui garantit la fluidité !
                    }

                    // Attend la fin de tous les batchs
                    output = await _taskBatcher.WaitAllAndGetResultsAsync();
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
            _currentContext = null;     // Pour s'assurer que l'on libere les resources a la fin de la generation

            _etapeCourante = EtapeGenerateurSiteEnum.None;
            return new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteGeneration, _checksumGenere.Count > 0, true, _checksumGenere.Count);
        }

        /// <summary>
        /// Execute la synchronisation du site distant avec les fichiers generes localement.
        /// </summary>
        /// <returns></returns>
        public async Task<ResultatOperation> ExecuteSynchronisation()
        {
            _etapeCourante = EtapeGenerateurSiteEnum.ExecuteSynchronisation;
            UploadStatus uploadOut = new UploadStatus();

            // Si le site distant est actif, transfere la mise a jour
            if (_site != null && !_site.IsLocal && _site.IsActif)
            {
                try
                {
                    string localRoot = _siteUrlGenerator.PhysicalStructure.RepertoireCompetition;

                    uploadOut = await Task.Run(() =>
                    {
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

                        // Synchronise le site FTP. On déporte l'appel FTP synchrone sur le ThreadPool pour ne pas bloquer le Scheduler
                        return _site.UploadSite(localRoot, filesToSync);
                    });

                    if (uploadOut.IsSuccess)
                    {
                        // Enregistre les checksums en cache maintenant qu'on sait que l'etat distant est synchrone
                        await Task.Run(() => SaveChecksumFichiersGeneres());
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
                return new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteSynchronisation, false);
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
                List<XElement> rootElem = doc.Descendants(FileWithChecksum.checksums).ToList();

                if (rootElem.Count >= 1)
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
                string output;
                // Normalement on ne devrait pas avoir de probleme d'exception ici avec la structure de repertoire
                try
                {
                    output = Path.Combine(_siteUrlGenerator.PhysicalStructure.RepertoireRacine, AppDirectoryManager.ChecksumFile);
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
            if (_siteUrlGenerator != null)
            {
                // On délègue totalement le nettoyage (disque + cache) à la structure physique
                if (!_siteUrlGenerator.PhysicalStructure.EffacerRepertoireCompetition())
                {
                    LogTools.Logger.Error("Erreur lors de l'effacement du contenu de '{0}'", _siteUrlGenerator.PhysicalStructure.RepertoireCompetition);
                }

                // Charge le contenu du fichier de checksum
                LoadChecksumFichiersGeneres();

                // Elimine tous les fichiers commençant par le répertoire de la competition (ils ont été supprimés)
                _checksumCache.RemoveAll(f => f.File.FullName.StartsWith(_siteUrlGenerator.PhysicalStructure.RepertoireCompetition));
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

            if (doc != null && !File.Exists(ChecksumFileName) || !FileSystemHelper.IsFileLocked(ChecksumFileName))
            {
                FileSystemHelper.NeedAccessFile(ChecksumFileName);
                try
                {
                    using (FileStream fs = new FileStream(ChecksumFileName, FileMode.Create))
                    {
                        doc.Save(fs);
                    }
                }
                catch (Exception ex)
                {
                    // Si le fichier est verrouille c'est bien une erreur car on a besoin de mettre a jour le cache de checksum pour la prochaine generation,
                    // mais on ne peut pas faire grand chose de plus que logger l'erreur
                    LogTools.Error(ex);
                }
                finally
                {
                    FileSystemHelper.ReleaseFile(ChecksumFileName);
                }
            }
        }

        #endregion
    }
}
