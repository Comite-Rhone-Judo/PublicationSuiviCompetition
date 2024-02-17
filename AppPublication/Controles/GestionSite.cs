using AppPublication.Export;
using AppPublication.Tools;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;

namespace AppPublication.Controles
{
    /// <summary>
    /// Classe de gestion du Site auto-généré tout au long de la compétition. Il assure la generation du site et contient les objets de publication
    /// locaux et distants.
    /// </summary>
    public class GestionSite : NotificationBase
    {
        #region MEMBRES
        private CancellationTokenSource _tokenSource;   // Token pour la gestion de la thread de lecture
        private Task _taskGeneration = null;            // La tache de generation
        private Task _taskNettoyage = null;            // La tache de nettoyage
        private GestionStatistiques _statMgr = null;

        /// <summary>
        /// Structure interne pour gerer les parametres de generation du site
        /// </summary>
        public class GenereSiteStruct
        {
            public SiteEnum type { get; set; }
            public Phase phase { get; set; }
            public int? tapis { get; set; }
        }
        #endregion

        #region CONSTRUCTEURS
        public GestionSite(GestionStatistiques statMgr)
        {
            try
            {
                // Initialise les objets de gestion des sites Web
                _siteLocal = new MiniSite(true);
                _siteDistant = new MiniSite(false);
                _statMgr = (statMgr != null) ? statMgr : new GestionStatistiques();

                // Initialise la liste des logos
                InitFichiersLogo();

                // Initialise la configuration via le cache de fichier
                InitCacheConfig();
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
        }

        #endregion

        #region PROPRIETES
        
        ObservableCollection<FilteredFileInfo> _fichiersLogo = new ObservableCollection<FilteredFileInfo>();   
        public ObservableCollection<FilteredFileInfo> FichiersLogo
        {
            get {
                return _fichiersLogo;
            }
            private set {
                _fichiersLogo = value;
                NotifyPropertyChanged("FichiersLogo");
            }
        }

        FilteredFileInfo _selectedLogo = null;
        public FilteredFileInfo SelectedLogo
        {
            get
            {
                return _selectedLogo;
            }
            set
            {
                _selectedLogo = value;
                AppSettings.SaveSettings("SelectedLogo", _selectedLogo.Name);
                NotifyPropertyChanged("SelectedLogo");
            }
        }

        StatExecution _statGeneration;
        /// <summary>
        /// Statistique de derniere generation - lecture seule
        /// </summary>
        public StatExecution DerniereGeneration
        {
            get
            {
                return _statGeneration;
            }
            private set
            {
                _statGeneration = value;
                NotifyPropertyChanged("DerniereGeneration");
            }
        }

        StatExecution _statSyncDistant;
        /// <summary>
        /// Statistiques de derniere synchronisation - lecture seule
        /// </summary>
        public StatExecution DerniereSynchronisation
        {
            get
            {
                return _statSyncDistant;
            }
            private set
            {
                _statSyncDistant = value;
                NotifyPropertyChanged("DerniereSynchronisation");
            }
        }

        bool _siteGenere = false;
        /// <summary>
        /// Indique si le site a ete bien genere (true) - lecture seule
        /// </summary>
        public bool SiteGenere
        {
            get
            {
                return _siteGenere;
            }
            private set
            {
                _siteGenere = value;
                NotifyPropertyChanged("SiteGenere");
            }
        }

        bool _siteSynchronise = false;
        /// <summary>
        /// Indique si le site a bien ete synchronnise - lecture seule
        /// </summary>
        public bool SiteSynchronise
        {
            get
            {
                return _siteSynchronise;
            }
            private set
            {
                _siteSynchronise = value;
                NotifyPropertyChanged("SiteSynchronise");
            }
        }

        private MiniSite _siteLocal = null;
        /// <summary>
        /// Le site de publication local
        /// </summary>
        public MiniSite MiniSiteLocal
        {
            get
            {
                return _siteLocal;
            }
        }

        /// <summary>
        /// Propriete passerelle pour selectionner l'interface de publication du site local
        /// Permet de tenir a jour le QR code de l'URL de publication
        /// </summary>
        public IPAddress InterfaceLocalPublication
        {
            get
            {
                return MiniSiteLocal.InterfaceLocalPublication;
            }
            set
            {
                // Verifie que la valeur selectionnee est bien dans la liste des interfaces
                try
                {
                    MiniSiteLocal.InterfaceLocalPublication = value;
                    AppSettings.SaveSettings("InterfaceLocalPublication", MiniSiteLocal.InterfaceLocalPublication.ToString());
                    NotifyPropertyChanged("InterfaceLocalPublication");
                    URLLocalPublication = CalculURLSiteLocal();
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        private MiniSite _siteDistant = null;
        /// <summary>
        /// Le site de publication distant
        /// </summary>
        public MiniSite MiniSiteDistant
        {
            get
            {
                return _siteDistant;
            }
        }

        bool _generationActive = false;
        /// <summary>
        /// Etat de la generation du site
        /// </summary>
        public bool IsGenerationActive
        {
            get
            {
                return _generationActive;
            }
            private set
            {
                _generationActive = value;
                NotifyPropertyChanged("IsGenerationActive");
            }
        }

        bool _isolerCompetition = false;
        /// <summary>
        /// Isole les competitions avec leur ID lors de l'upload sur le site distant
        /// </summary>
        public bool IsolerCompetition
        {
            get
            {
                return _isolerCompetition;
            }
            set
            {
                _isolerCompetition = value;
                NotifyPropertyChanged("IsolerCompetition");
                AppSettings.SaveSettings("IsolerCompetition", _isolerCompetition.ToString());
                URLDistantPublication = CalculURLSiteDistant();
                MiniSiteDistant.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();
            }
        }

        private int _nbProchainsCombats = 6;
        /// <summary>
        /// Nb de prochains combats a publier pour la chambre d'appel
        /// </summary>
        public int NbProchainsCombats
        {
            get
            {
                return _nbProchainsCombats;
            }
            set
            {
                _nbProchainsCombats = value;
                NotifyPropertyChanged("NbProchainsCombats");
                AppSettings.SaveSettings("NbProchainsCombats", _nbProchainsCombats.ToString());
            }
        }


        int _delaiGenerationSec = 30;
        /// <summary>
        /// Delai entre 2 generations du site
        /// </summary>
        public int DelaiGenerationSec
        {
            get
            {
                return _delaiGenerationSec;
            }
            set
            {
                _delaiGenerationSec = value;
                AppSettings.SaveSettings("DelaiGenerationSec", _delaiGenerationSec.ToString());
                NotifyPropertyChanged("DelaiGenerationSec");
            }
        }

        int _delaiActualisationClientSec = 30;
        /// <summary>
        /// Delai entre 2 generations du site
        /// </summary>
        public int DelaiActualisationClientSec
        {
            get
            {
                return _delaiActualisationClientSec;
            }
            set
            {
                _delaiActualisationClientSec = value;
                AppSettings.SaveSettings("DelaiActualisationClientSec", _delaiActualisationClientSec.ToString());
                NotifyPropertyChanged("DelaiActualisationClientSec");
            }
        }

        string _msgProchainsCombats = string.Empty;
        /// <summary>
        /// Message optionnel pour les prochains coùbats
        /// </summary>
        public string MsgProchainsCombats
        {
            get
            {
                return _msgProchainsCombats;
            }
            set
            {
                _msgProchainsCombats = value;
                AppSettings.SaveSettings("MsgProchainsCombats", _msgProchainsCombats);
                NotifyPropertyChanged("MsgProchainsCombats");
            }
        }


        private string _urlDistant;
        /// <summary>
        /// URL racine du site distant de publication
        /// </summary>
        public string URLDistant
        {
            get
            {
                return _urlDistant;
            }
            set
            {
                _urlDistant = value;
                AppSettings.SaveSettings("URLDistant", _urlDistant);
                NotifyPropertyChanged("URLDistant");
                URLDistantPublication = CalculURLSiteDistant();
            }
        }

        private string _urlDistantPublication;
        /// <summary>
        /// URL Complete sur le site distant de publication
        /// </summary>
        public string URLDistantPublication
        {
            get
            {
                return _urlDistantPublication;
            }
            private set
            {
                _urlDistantPublication = value;
                NotifyPropertyChanged("URLDistantPublication");
            }
        }

        private string _urlLocalPublication;
        /// <summary>
        /// URL sur le site local
        /// </summary>
        public string URLLocalPublication
        {
            get
            {
                return _urlLocalPublication;
            }
            private set
            {
                _urlLocalPublication = value;
                NotifyPropertyChanged("URLLocalPublication");
            }
        }

        private string _ftpRepertoireRacineDistant;
        /// <summary>
        /// Repertoire racine cible sur le site distant
        /// </summary>
        public string RepertoireRacineSiteFTPDistant
        {
            get
            {
                return _ftpRepertoireRacineDistant;
            }
            set
            {
                _ftpRepertoireRacineDistant = value;
                AppSettings.SaveSettings("RepertoireRacineSiteFTPDistant", _ftpRepertoireRacineDistant);
                NotifyPropertyChanged("RepertoireRacineSiteFTPDistant");
                MiniSiteDistant.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();
            }
        }

        private string _idCompetition;
        /// <summary>
        /// ID de la competition en cours
        /// </summary>
        public string IdCompetition
        {
            get
            {
                return _idCompetition;
            }
            set
            {
                _idCompetition = value;
                NotifyPropertyChanged("IdCompetition");
                MiniSiteDistant.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();
                URLDistantPublication = CalculURLSiteDistant();
                URLLocalPublication = CalculURLSiteLocal();

                // On en peut publier que en individuelle
                CanPublierAffectation = DialogControleur.Instance.ServerData.competition.IsIndividuelle();
            }
        }

        private bool _canPublierAffectation = true;
        /// <summary>
        /// Indique si on peut publier l'affectation des tapis ou nnon
        /// </summary>
        public bool CanPublierAffectation
        {
            get
            {
                return _canPublierAffectation;
            }
            private set
            {
                _canPublierAffectation = value;
                NotifyPropertyChanged("CanPublierAffectation");
            }
        }

        private bool _publierProchainsCombats = false;
        /// <summary>
        /// Indique si on doit publier la liste des prochains combats ou non
        /// </summary>
        public bool PublierProchainsCombats
        {
            get
            {
                return _publierProchainsCombats;
            }
            set
            {
                _publierProchainsCombats = value;
                AppSettings.SaveSettings("PublierProchainsCombats", _publierProchainsCombats.ToString());
                NotifyPropertyChanged("PublierProchainsCombats");
            }
        }

        

        private bool _publierAffectationTapis = false;
        /// <summary>
        /// Indique si on doit publier la liste des prochains combats ou non
        /// </summary>
        public bool PublierAffectationTapis
        {
            get
            {
                return _publierAffectationTapis;
            }
            set
            {
                _publierAffectationTapis = value;
                AppSettings.SaveSettings("PublierAffectationTapis", _publierAffectationTapis.ToString());
                NotifyPropertyChanged("_publierAffectationTapis");
            }
        }

        private StatusGenerationSite _status;
        /// <summary>
        /// Le statut de generation du site
        /// </summary>
        public StatusGenerationSite Status
        {
            get
            {
                if (null == _status)
                {
                    _status = new StatusGenerationSite();
                }
                return _status;
            }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
                IsGenerationActive = !(_status.State == StateGenerationEnum.Stopped);
            }
        }

        /// <summary>
        /// Nom du fichierd de cache utiliser pour le controle des checksums
        /// </summary>
        public string ChecksumFileName
        {
            get
            {
                return Path.Combine(ConstantFile.ExportSite_dir, ExportTools.getFileName(ExportEnum.Site_Checksum) + ConstantFile.ExtensionXML);
            }
        }

        #endregion

        #region METHODES

        private void InitFichiersLogo()
        {
            // Recupere le repertoire des images du site
            DirectoryInfo di = new DirectoryInfo(ConstantFile.ExportStyle_dir);

            IEnumerable<FilteredFileInfo> files = di.EnumerateFiles("*logo-*.png", SearchOption.TopDirectoryOnly).Select(o => new FilteredFileInfo(o)).OrderBy(o => o.Name);

            // Liste les fichiers logos
            FichiersLogo = new ObservableCollection<FilteredFileInfo>(files);
        }

        /// <summary>
        /// Initialise les donnees a partir du cache de fichier AppConfig
        /// </summary>
        private void InitCacheConfig()
        {
            string valCache = string.Empty;

            try
            {
                valCache = AppSettings.ReadSettings("URLDistant");
                URLDistant = (valCache == null) ? String.Empty : valCache;

                valCache = AppSettings.ReadSettings("IsolerCompetition");
                IsolerCompetition = (valCache == null) ? false : bool.Parse(valCache);

                valCache = AppSettings.ReadSettings("RepertoireRacineSiteFTPDistant");
                RepertoireRacineSiteFTPDistant = (valCache == null) ? String.Empty : valCache;

                valCache = AppSettings.ReadSettings("PublierProchainsCombats");
                PublierProchainsCombats = (valCache == null) ? false : bool.Parse(valCache);

                valCache = AppSettings.ReadSettings("NbProchainsCombats");
                NbProchainsCombats = (valCache == null) ? 6 : int.Parse(valCache);

                valCache = AppSettings.ReadSettings("PublierAffectationTapis");
                PublierAffectationTapis = (valCache == null) ? true : bool.Parse(valCache);

                valCache = AppSettings.ReadSettings("DelaiGenerationSec");
                DelaiGenerationSec = (valCache == null) ? 30 : int.Parse(valCache);

                valCache = AppSettings.ReadSettings("DelaiActualisationClientSec");
                DelaiActualisationClientSec = (valCache == null) ? 30 : int.Parse(valCache);

                valCache = AppSettings.ReadSettings("MsgProchainsCombats");
                MsgProchainsCombats = (valCache == null) ? string.Empty : valCache;

                // Recherche le logo dans la liste
                if(FichiersLogo.Count >= 1)
                {
                    valCache = AppSettings.ReadSettings("SelectedLogo");
                    if(valCache != null)
                    {
                        foreach(FilteredFileInfo fl in FichiersLogo)
                        {
                            if(fl.Name == valCache)
                            {
                                SelectedLogo = fl;
                                break;
                            }
                        }
                    }
                }

                // Si la liste contient au moins un element
                if (MiniSiteLocal.InterfacesLocal.Count >= 1)
                { 
                    // Cherche si une interface existe dans la configuration du fichier
                    valCache = AppSettings.ReadSettings("InterfaceLocalPublication");
                    IPAddress ipToUse = null;
                    bool useCache = false;

                    if (valCache != null)
                    {
                        try
                        {
                            // Lit l'adresse dans le fichier et verifie qu'elle est dans la liste
                            ipToUse = IPAddress.Parse(valCache);
                            useCache = MiniSiteLocal.InterfacesLocal.Contains(ipToUse);
                        }
                        catch (Exception ex)
                        {
                            // Soit l'IP configuree est incorrecte, soit elle n'est pas dans la liste
                            useCache = false;
                            LogTools.Log(ex);
                        }
                    }

                    // on prend la 1ere interface de la liste si elle n'est pas dans la 
                    if (!useCache)
                    {
                        ipToUse = MiniSiteLocal.InterfacesLocal.First();
                    }

                    // Assigne la valeur (en dernier pour eviter les bindings successifs)
                    InterfaceLocalPublication = ipToUse;
                }
            }
            catch (Exception ex)
            {
                LogTools.Trace(ex);
            }
        }

        /// <summary>
        /// Calcul l'URL sur le site distant en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLSiteDistant()
        {
            string output = "Indefinie";

            if (!String.IsNullOrEmpty(URLDistant))
            {
                if (IsolerCompetition)
                {
                    if (!String.IsNullOrEmpty(IdCompetition))
                    {
                        output = ExportTools.GetURLSiteDistant(URLDistant, IdCompetition);
                    }
                }
                else
                {
                    output = ExportTools.GetURLSiteDistant(URLDistant, "courante");
                }
            }
            return output;
        }

        /// <summary>
        /// Calcul l'URL sur le site local en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLSiteLocal()
        {
            string output = "Indefinie";

            if (!String.IsNullOrEmpty(IdCompetition) && MiniSiteLocal.ServerHTTP != null && MiniSiteLocal.ServerHTTP.ListeningIpAddress != null && MiniSiteLocal.ServerHTTP.Port > 0)
            {
                output = ExportTools.GetURLSiteLocal(MiniSiteLocal.ServerHTTP.ListeningIpAddress.ToString(),
                                                        MiniSiteLocal.ServerHTTP.Port,
                                                        IdCompetition);
            }

            return output;
        }

        /// <summary>
        /// Calcul le repertoire sur le site distant en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculRepertoireSiteDistant()
        {
            string output = string.Empty;
            if (!String.IsNullOrEmpty(RepertoireRacineSiteFTPDistant))
            {
                if (IsolerCompetition)
                {
                    if (!String.IsNullOrEmpty(IdCompetition))
                    {
                        output = Path.Combine(RepertoireRacineSiteFTPDistant, IdCompetition);
                    }
                }
                else
                {
                    output = Path.Combine(RepertoireRacineSiteFTPDistant, "courante");
                }
            }
            return output;
        }

        /// <summary>
        /// Demarre le thread de generation du site
        /// </summary>
        public void StartGeneration()
        {
            // Status = new StatusGenerationSite(StateGenerationEnum.Idle);
            Status = StatusGenerationSite.Instance(StateGenerationEnum.Idle);
            DateTime wakeUpTime = DateTime.Now;
            int delaiScrutationMs = 1000;

            // Reset le token d'arret
            if (_tokenSource != null)
            {
                _tokenSource = null;
            }
            _tokenSource = new CancellationTokenSource();

            if (_taskGeneration == null || _taskGeneration.IsCompleted)
            {
                try
                {
                    _taskGeneration = Task.Factory.StartNew(() =>
                    {
                        while (!_tokenSource.Token.IsCancellationRequested)
                        {
                            if (DateTime.Now >= wakeUpTime)
                            {
                                // Pour controler la duree total par rapport au timer
                                Stopwatch watcherTotal = new Stopwatch();
                                watcherTotal.Start();

                                // Pousse les commandes de generation dans le thread de travail
                                // Status = new StatusGenerationSite(StateGenerationEnum.Generating, "Generation du site ...");
                                Status = StatusGenerationSite.Instance(StateGenerationEnum.Generating);

                                StatExecution statGeneration = new StatExecution();
                                Stopwatch watcherGen = new Stopwatch();
                                watcherGen.Start();

                                // Charge le fichier de cache de checksum
                                List<FileWithChecksum> checksumCache = LoadChecksumFichiersGeneres();
                                List<FileWithChecksum> checksumGenere = GenereAll();
                                SiteGenere = (checksumGenere.Count > 0);
                                watcherGen.Stop();
                                statGeneration.DelaiExecutionMs = watcherGen.ElapsedMilliseconds;
                                // Status = new StatusGenerationSite(StateGenerationEnum.Idle, "En attente ...");
                                Status = StatusGenerationSite.Instance(StateGenerationEnum.Idle);

                                _statMgr.EnregistrerGeneration(watcherGen.ElapsedMilliseconds / 1000F);

                                if (SiteGenere)
                                {
                                    // Met a jour la date de generation puisque le site a ete traite
                                    DerniereGeneration = statGeneration;

                                    // Si le site distant est actif, transfere la mise a jour
                                    if (MiniSiteDistant.IsActif)
                                    {
                                        string localRoot = Path.Combine(ConstantFile.ExportSite_dir, DialogControleur.Instance.ServerData.competition.remoteId);

                                        // Le site distant sur lequel charger les fichiers selon si on isole ou pas
                                        StatExecution statSync = new StatExecution();
                                        Stopwatch watcherSync = new Stopwatch();
                                        watcherSync.Start();

                                        // Calcul les fichiers a prendre en compte
                                        List<FileInfo> filesToSync = null;
                                        if (checksumCache != null && checksumCache.Count > 0)
                                        {
                                            // Extrait les fichiers generes qui sont differents du cache
                                            List<FileWithChecksum> chkToSync = checksumGenere.Except(checksumCache, new FileWithChecksumComparer()).ToList();
                                            filesToSync = chkToSync.Select(o => o.File).ToList();

                                            // For Tests only
                                            /*
                                            if (filesToSync.Count <= 0)
                                            {
                                                Random rnd1 = new Random();
                                                Random rnd2 = new Random();

                                                int nbFile = rnd1.Next(checksumGenere.Count);

                                                for(int i = 0; i < nbFile; i++)
                                                {
                                                    filesToSync.Add(checksumGenere[rnd2.Next(checksumGenere.Count)].File);
                                                }
                                            }
                                            */
                                        }

                                        // Synchronise le site FTP
                                        UploadStatus uploadOut = MiniSiteDistant.UploadSite(localRoot, filesToSync);
                                        SiteSynchronise = uploadOut.IsSuccess;

                                        watcherSync.Stop();
                                        statSync.DelaiExecutionMs = watcherSync.ElapsedMilliseconds;

                                        _statMgr.EnregistrerSynchronisation(watcherSync.ElapsedMilliseconds / 1000F, uploadOut);

                                        if (SiteSynchronise)
                                        {
                                            // Enregistre les checksums en cache maintenant qu'on sait que l'etat distant est synchrone
                                            SaveChecksumFichiersGeneres(checksumGenere);
                                            DerniereSynchronisation = statSync;
                                        }
                                    }
                                }
                                else
                                {
                                    _statMgr.EnregistrerErreurGeneration();
                                }

                                watcherTotal.Stop();

                                // Si le transfert a duree plus que le temps d'attente, on attend au plus 5 sec
                                // Sinon, on attend la difference restantes
                                int delaiThread = (int)Math.Max(DelaiGenerationSec * 1000 - watcherTotal.ElapsedMilliseconds, 5000);

                                // Met le thread en attente pour la prochaine generation
                                Status.NextGenerationSec = (int)Math.Round(delaiThread / 1000.0);

                                _statMgr.EnregsitrerDelaiGeneration(delaiThread / 1000F);

                                // prochaine heure de generation
                                wakeUpTime = DateTime.Now.AddMilliseconds(delaiThread);
                                DerniereGeneration.DateProchaineGeneration = wakeUpTime;
                            }
                            Thread.Sleep(delaiScrutationMs);
                        }
                    }, _tokenSource.Token);
                }
                catch (Exception ex)
                {
                    // On RAZ l'etat du lecteur
                    throw new Exception("Erreur lors du lancement de la generation du site", ex);
                }
            }
            else
            {
                throw new Exception("Une tache de génération est déjà en cours d'exécution");
            }
        }

        /// <summary>
        /// Demarre un thread pour traiter le nettoyage du site
        /// </summary>
        public void StartNettoyage()
        {
            if (_taskNettoyage == null || _taskNettoyage.IsCompleted)
            {
                try
                {
                    _taskNettoyage = Task.Factory.StartNew(() =>
                    {
                        // Nettoyer le site distant
                        MiniSiteDistant.NettoyerSite();
                    });
                }
                catch (Exception ex)
                {
                    // On RAZ l'etat du lecteur
                    throw new Exception("Erreur lors du lancement du nettoyage du site", ex);
                }
            }
            else
            {
                throw new Exception("Une tache de nettoyage est déjà en cours d'exécution");
            }
        }

        /// <summary>
        /// Arrete le thread de generation du site
        /// </summary>
        public void StopGeneration()
        {
            if (_tokenSource != null)
            {
                // Arrete le thread de generation
                _tokenSource.Cancel();
                _taskGeneration.Wait();
            }

            // Etat de la generation
            // Status = new StatusGenerationSite(StateGenerationEnum.Stopped);
            Status = StatusGenerationSite.Instance(StateGenerationEnum.Stopped);
        }

        /// <summary>
        /// Declenche l'exportation
        /// </summary>
        /// <param name="genere">Type d'exportation</param>
        private List<FileWithChecksum> Exporter(GenereSiteStruct genere)
        {
            List<FileWithChecksum> urls = new List<FileWithChecksum>();
            ConfigurationExportSite cfg = new ConfigurationExportSite(PublierProchainsCombats, PublierAffectationTapis && CanPublierAffectation, DelaiActualisationClientSec, NbProchainsCombats, MsgProchainsCombats, (SelectedLogo != null) ? SelectedLogo.Name : string.Empty);

            try
            {
                JudoData DC = DialogControleur.Instance.ServerData;

                switch (genere.type)
                {
                    case SiteEnum.AllTapis:
                        urls = ExportSite.GenereWebSiteAllTapis(DC, cfg);
                        break;
                    case SiteEnum.Classement:
                        urls = ExportSite.GenereWebSiteClassement(DC, genere.phase.GetVueEpreuve(DC), cfg);
                        break;
                    case SiteEnum.Index:
                        urls = ExportSite.GenereWebSiteIndex(cfg);
                        break;
                    case SiteEnum.Menu:
                        urls = ExportSite.GenereWebSiteMenu(DC, cfg);
                        break;
                    case SiteEnum.Phase:
                        urls = ExportSite.GenereWebSitePhase(DC, genere.phase, cfg);
                        break;
                    case SiteEnum.AffectationTapis:
                        urls = ExportSite.GenereWebSiteAffectation(DC, cfg);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogTools.Trace(ex);
            }

            return urls;
        }

        /// <summary>
        /// Ajoute une tache de fond de generation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="phase"></param>
        /// <param name="tapis"></param>
        /// <returns></returns>
        public Task<List<FileWithChecksum>> AddWork(SiteEnum type, Phase phase, int? tapis)
        {
            Task<List<FileWithChecksum>> output = null;

            if (IsGenerationActive)
            {
                GenereSiteStruct export = new GenereSiteStruct
                {
                    type = type,
                    phase = phase,
                    tapis = tapis
                };

                output = OutilsTools.Factory.StartNew(() =>
                {
                    return Exporter(export);
                });
            }

            return output;
        }

        /// <summary>
        /// Genere la totalite du site
        /// </summary>
        /// <returns></returns>
        public List<FileWithChecksum> GenereAll()
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            if (IsGenerationActive)
            {
                JudoData DC = DialogControleur.Instance.ServerData;
                if (DC.Organisation.Competitions.Count > 0)
                {
                    List<Task<List<FileWithChecksum>>> listTaskGeneration = new List<Task<List<FileWithChecksum>>>();

                    listTaskGeneration.Add(AddWork(SiteEnum.Index, null, null));
                    listTaskGeneration.Add(AddWork(SiteEnum.Menu, null, null));
                    if (PublierAffectationTapis && CanPublierAffectation)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.AffectationTapis, null, null));
                    }

                    // On ne genere pas les informations de prochains combat si ce n'est pas necessaire
                    if (PublierProchainsCombats)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.AllTapis, null, null));
                    }

                    foreach (Phase phase in DC.Deroulement.Phases)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.Phase, phase, null));
                        listTaskGeneration.Add(AddWork(SiteEnum.Classement, phase, null));
                    }

                    try
                    {
                        // Elimine les elements null
                        listTaskGeneration.RemoveAll(item => item == null);
                        if (listTaskGeneration.Count > 0)
                        {
                            Task<List<FileWithChecksum>> t = WaitGenereAll(listTaskGeneration);
                            // Attend la fin de la generation pour rendre la main
                            t.Wait();
                            output = t.Result;

                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Trace(ex);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Se met en attente de la fin de tous les travaux de generation. Met a jour l'indicateur de progression en fonction de la fin des taches
        /// </summary>
        /// <param name="listTaskGeneration"></param>
        /// <returns></returns>
        private async Task<List<FileWithChecksum>> WaitGenereAll(List<Task<List<FileWithChecksum>>> listTaskGeneration)
        {
            int totalTask = listTaskGeneration.Count();
            int nTask = 0;
            List<FileWithChecksum> fichiersGeneres = new List<FileWithChecksum>();
            while (listTaskGeneration.Any())
            {
                Task<List<FileWithChecksum>> finishedTask = await Task.WhenAny(listTaskGeneration.ToArray());
                listTaskGeneration.Remove(finishedTask);
                nTask++;
                Status.Progress = (int)Math.Round(100.0 * nTask / totalTask);
                fichiersGeneres = fichiersGeneres.Concat(await finishedTask).ToList();
            }

            return fichiersGeneres;
        }

        /// <summary>
        /// Sauvegarde une liste de fichiers generes dans le cache de checksum (ecrase le precedent)
        /// </summary>
        /// <param name="fichiersGeneres"></param>
        private void SaveChecksumFichiersGeneres(List<FileWithChecksum> fichiersGeneres)
        {
            // Enregistre les checksums des fichiers generes
            XDocument doc = ExportXML.ExportChecksumFichiers(fichiersGeneres);

            if (!File.Exists(ChecksumFileName) || !FileAndDirectTools.IsFileLocked(ChecksumFileName))
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
                    LogTools.Trace(ex);
                }
                finally
                {
                    FileAndDirectTools.ReleaseFile(ChecksumFileName);
                }
            }
        }


        /// <summary>
        /// Charge le fichier de cache de checksum
        /// </summary>
        /// <param name=""></param>
        /// <returns>Liste vide si le fichier n'existe pas</returns>
        private List<FileWithChecksum> LoadChecksumFichiersGeneres()
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
                LogTools.Trace(ex);
            }

            return output;
        }

        #endregion
    }
}
