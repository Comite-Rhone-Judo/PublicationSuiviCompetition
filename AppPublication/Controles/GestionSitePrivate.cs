using AppPublication.Config.EcransAppel;
using AppPublication.Config.Publication;
using AppPublication.Export;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Generation;
using AppPublication.Models.EcransAppel;
using AppPublication.Publication;
using AppPublication.Statistiques;
using AppPublication.Tools.Files;
using AppPublication.Tools.FranceJudo;
using AppPublication.ViewModels.Configuration;
using AppPublication.Views.Configuration;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using AppPublication.ExtensionNoyau;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;
using Tools.Windows;
using Tools.Framework;


namespace AppPublication.Controles
{
    /// <summary>
    /// Classe de gestion du Site private auto-généré tout au long de la compétition. Il assure la generation du site Private et contient les objets de publication
    /// locaux et distants.
    /// </summary>
    public class GestionSitePrivate : NotificationBase
    {
        #region CONSTANTES
        private const string kSitePrivateInstanceName = "private";
        private const string kSiteRepertoire = "private-site";
        #endregion

        #region MEMBRES
        private CancellationTokenSource _tokenSource;   // Token pour la gestion de la thread de lecture
        private Task _taskGeneration = null;            // La tache de generation
        private GestionStatistiques _statMgr = null;
        private ExportSitePrivateStructure _structureRepertoiresPrivateSite;        // La structure de repertoire d'export du site prive
        private ExportPrivateSiteUrls _structureSitePrivate;                 // la structure d'export du site local

        private long _generationCounter = 0;                        // Nombre de generation realisees depuis le demarrage
        private int _workCounter = 0;                               // Compteur de travail en cours pour la generation du site
        private int _nbGeneration = 0;                              // Nombre de generation en cours pour le site distant   
        private List<int> _allTaskProgress = new List<int>();       // Progression de chacune des taches (clef = Id)

        // TODO : A revoir pour le site private, on a un seul ecran de configuration ...
        private ConfigurationEcransAppelView _cfgEcransAppelView = null; // La fenetre de configuration des ecrans d'appel

        private IJudoDataManager _judoDataManager;                  // Le gestionnaire de données interne

        /// <summary>
        /// Structure interne pour gerer les parametres de generation du site
        /// </summary>
        public class GenereSiteStruct
        {
            public TypeExportSiteEnum type { get; set; }
            public Phase phase { get; set; }
            public int? tapis { get; set; }
        }
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="dataManager">Le gestionnaire de données</param>
        /// <param name="statMgr">le gestionnaire de statitiques</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GestionSitePrivate(IJudoDataManager dataManager, GestionStatistiques statMgr)
        {
            // Impossible d'etre null
            if (dataManager == null) throw new ArgumentNullException();
            try
            {
                // Initialise les objets de gestion des sites Web. Ils chargent automatiquement leur configuration
                _sitePrivate = new MiniSiteConfigurable(true, kSitePrivateInstanceName, true, false);
                _statMgr = (statMgr != null) ? statMgr : new GestionStatistiques();
                _judoDataManager = dataManager;

                // TODO a supprimer car c'est partagee
                // Initialise la liste des logos
                InitFichiersLogo();

                // Initialise la configuration via le cache de fichier
                InitCacheConfig();
            }
            catch (Exception ex)
            {
                LogTools.Logger.Fatal(ex, "Impossible d'initialiser le gestionnaire de Site Private interne. Impossible de continuer");
                AlertWindow win = new AlertWindow("Erreur fatale", "Impossible de démarrer un composant interne, l'application doit s'arrêter. Veuillez contacter le support.");
                if (win != null)
                {
                    win.ShowDialog();
                }

                // Emergency shutdown
                App.Current.Shutdown();
            }
        }

        #endregion

        #region PROPRIETES

        private ExtendedJudoData _extendedJudoData;
        /// <summary>
        /// Le bloc de donnees etendue
        /// </summary>
        public ExtendedJudoData ExtendedJudoData
        {
            get
            {
                if (_extendedJudoData == null)
                {
                    _extendedJudoData = new ExtendedJudoData();
                }
                return _extendedJudoData;
            }
        }

        private EcranCollectionManager _ecransAppel = new EcranCollectionManager();
        public EcranCollectionManager EcransAppel
        {
            get
            {
                return _ecransAppel;
            }
            set
            {
                if (_ecransAppel != value)
                {
                    _ecransAppel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // TODO A revoir car c'est partage
        private string _repertoireRacine;
        public string RepertoireRacine
        {
            get
            {
                return _repertoireRacine;
            }
            set
            {
                if (value != _repertoireRacine)
                {
                    PublicationConfigSection.Instance.RepertoireRacine = (_repertoireRacine = value);
                    NotifyPropertyChanged();

                    // Met a jour la constante d'export
                    string tmp = OutilsTools.GetExportDir(_repertoireRacine);

                    // Initialise les structures d'export
                    _structureRepertoiresSite = new ExportSiteStructure(Path.Combine(tmp, kSiteRepertoire), IdCompetition);
                    _structureRepertoiresPrivateSite = new ExportSitePrivateStructure(Path.Combine(tmp, kPrivateSiteRepertoire));

                    _structureSiteDistant = new ExportSiteUrls(_structureRepertoiresSite);
                    _structureSiteLocal = new ExportSiteUrls(_structureRepertoiresSite);


                    // Met a jour les repertoires de l'application
                    InitExportSiteStructure();

                    // Initialise la racine du serveur Web local
                    SiteLocal.ServerHTTP.LocalRootPath = tmp;
                }
            }
        }

        // TODO A revoir car c'est partage
        ObservableCollection<FilteredFileInfo> _fichiersLogo = new ObservableCollection<FilteredFileInfo>();
        public ObservableCollection<FilteredFileInfo> FichiersLogo
        {
            get {
                return _fichiersLogo;
            }
            private set {
                if (_fichiersLogo != value)
                {
                    _fichiersLogo = value;
                    NotifyPropertyChanged();
                }
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
                if (_selectedLogo != value)
                {
                    _selectedLogo = value;
                    PublicationConfigSection.Instance.Logo  = _selectedLogo.Name;
                    NotifyPropertyChanged();
                }
            }
        }

        // TODO A voir pour ajouter les stats de generation privees
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        private MiniSite _sitePrivate = null;
        /// <summary>
        /// Le site de publication local des ecrans d'appel
        /// </summary>
        public MiniSite SitePrivate
        {
            get
            {
                return _sitePrivate;
            }
        }

        /// <summary>
        /// Propriete passerelle pour selectionner l'interface de publication du site ecrans
        /// Permet de tenir a jour le QR code de l'URL de publication
        /// </summary>
        public IPAddress InterfacePrivateSite
        {
            get
            {
                return SitePrivate.InterfaceLocalPublication;
            }
            set
            {
                // Verifie que la valeur selectionnee est bien dans la liste des interfaces
                try
                {
                    SitePrivate.InterfaceLocalPublication = value;
                    NotifyPropertyChanged();
                    URLEcransAppelPublication = CalculURLPrivateSite();
                }
                catch (ArgumentOutOfRangeException) { }
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
                NotifyPropertyChanged();
            }
        }

        

        // TODO proprietes a garder mais peut etre a renommer
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
                if (_nbProchainsCombats != value)
                {
                    PublicationConfigSection.Instance.NbProchainsCombats = (_nbProchainsCombats = value);
                    NotifyPropertyChanged();
                }
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
                if (_delaiGenerationSec != value)
                {
                    PublicationConfigSection.Instance.DelaiGenerationSec = (_delaiGenerationSec = value);
                    NotifyPropertyChanged();
                }
            }
        }

        bool _effacerAuDemarrage = true;
        /// <summary>
        /// Indique si on doit faire un RAZ du contenu du répertoire au demarrage de la generation
        /// </summary>
        public bool EffacerAuDemarrage
        {
            get
            {
                return _effacerAuDemarrage;
            }
            set
            {
                if (_effacerAuDemarrage != value)
                {
                    PublicationConfigSection.Instance.EffacerAuDemarrage = (_effacerAuDemarrage = value);
                    NotifyPropertyChanged();
                }
            }
        }


        // TODO Sans doute a garder mais a renommer
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
                if (_delaiActualisationClientSec != value)
                {
                    PublicationConfigSection.Instance.DelaiActualisationClientSec = (_delaiActualisationClientSec = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private string _urlEcransAppelPublication;
        /// <summary>
        /// URL pour le site local des ecrans d'appel
        /// </summary>
        public string URLEcransAppelPublication
        {
            get
            {
                return _urlEcransAppelPublication;
            }
            private set
            {
                _urlEcransAppelPublication = value;
                NotifyPropertyChanged();
            }
        }

        // TODO A voir si on garde car l'ID de la competition n'est pas utilisee pour le site Private
        private string _idCompetition = string.Empty;
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
                NotifyPropertyChanged();

                // Met a jour la structure d'export
                if (_structureRepertoiresPrivateSite != null)
                {
                    _structureRepertoiresPrivateSite.IdCompetition = value;
                }

                // TODO A revoir si c'est bien cela
                URLEcransAppelPublication = CalculURLPrivateSite();

                // Note: ici on devrait dans l'absolu utiliser le snapshot mais le traitement est rapide et a peu de chance de changer
                var DC = _judoDataManager as IJudoData;
                // Le nombre de tapis peut avoir changer selon la compétition
                NbTapis = DC.Organisation.Competition.nbTapis;
            }
        }

        private int _nbTapis = 6;
        public int NbTapis
        {
            get
            {
                return _nbTapis;
            }
            set
            {
                if (_nbTapis != value)
                {
                    _nbTapis = value;
                    NotifyPropertyChanged();
                }
            }
        }

        
        // TODO a voir si on garde comme une option
        private bool _afficherPositionCombat;
        public bool AfficherPositionCombat
        {
            get
            {
                return _afficherPositionCombat;
            }
            set
            {
                if( _afficherPositionCombat != value)
                {
                    PublicationConfigSection.Instance.AfficherPositionCombat =(_afficherPositionCombat = value);
                    NotifyPropertyChanged();
                }
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
                NotifyPropertyChanged();
                IsGenerationActive = !(_status.State == StateGenerationEnum.Stopped);
            }
        }

        #endregion

        #region COMMANDES

        private ICommand _cmdAfficherConfigurationEcransAppel = null;
        /// <summary>
        /// Commande d'affichage de la configuration
        /// </summary>
        public ICommand CmdAfficherConfigurationEcransAppel
        {
            get
            {
                if (_cmdAfficherConfigurationEcransAppel == null)
                {
                    _cmdAfficherConfigurationEcransAppel = new RelayCommand(
                            o =>
                            {
                                if (_cfgEcransAppelView == null)
                                {
                                    // Crée la ViewModel de configuration. Comme on le refait a chaque fois, on est sur d'avoir les dernieres valeurs
                                    // notamment par rapport aux nombres de tapis de la competition s'il a été modifié
                                    ConfigurationEcransViewModel vm = new ConfigurationEcransViewModel(this.EcransAppel, this.NbTapis);
                                    if (vm != null)
                                    {
                                        _cfgEcransAppelView = new ConfigurationEcransAppelView(vm);
                                    }
                                }
                                if (_cfgEcransAppelView != null)
                                {
                                    _cfgEcransAppelView.ShowDialog();
                                    _cfgEcransAppelView = null;
                                }
                            },
                            o =>
                            {
                                return !this.IsGenerationActive;
                            });
                }
                return _cmdAfficherConfigurationEcransAppel;
            }
        }

        // TODO A revoir car c'est partage

        private ICommand _cmdAjouterLogo;

        /// <summary>
        /// Commande permettant d'ajouter un logo dans la liste
        /// </summary>
        public ICommand CmdAjouterLogo
        {
            get
            {
                if (_cmdAjouterLogo == null)
                {
                    _cmdAjouterLogo = new RelayCommand(
                            o =>
                            {
                                bool allFileOk = true;

                                OpenFileDialog op = new OpenFileDialog();
                                op.Title = "Sélectionner une image";
                                op.Filter = "Portable Network Graphic (*.png)|*.png";
                                op.Multiselect = true;
                                op.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                                op.RestoreDirectory = true;
                                if (op.ShowDialog() == DialogResult.OK)
                                {
                                    foreach (string imgFile in op.FileNames)
                                    {
                                        try
                                        {
                                            if (imgFile.ToLower().Contains("logo"))
                                            {
                                                int w, h;

                                                using (var stream = new FileStream(imgFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                                                {
                                                    var bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                                                    w = bitmapFrame.PixelWidth;
                                                    h = bitmapFrame.PixelHeight;

                                                    // Verifie la taille de l'image
                                                    if (w <= 200 && h <= 200)
                                                    {
                                                        FilteredFileInfo newItem = new FilteredFileInfo(new FileInfo(imgFile));

                                                        // Copy le fichier dans le répertoire de travail de l'application
                                                        File.Copy(newItem.FullName, Path.Combine(ConstantFile.ExportStyle_dir, newItem.Name));

                                                        // Actualise la liste des logos
                                                        FichiersLogo.Add(newItem);
                                                    }
                                                    else
                                                    {
                                                        LogTools.Logger.Debug("Fichier '{0}' ignore - taille {1}x{2} incorrecte", imgFile, w, h);
                                                        allFileOk = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LogTools.Logger.Debug("Fichier '{0}' ignore - Nom ne contient pas 'logo'", imgFile);
                                                allFileOk = false;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LogTools.Logger.Debug("Fichier '{0}' ignore - Exception lors de la lecture du format", imgFile, ex);
                                            allFileOk = false;
                                        }
                                    }

                                    if (!allFileOk)
                                    {
                                        AlertWindow win = new AlertWindow("Infomation", "Certains fichiers n'ont pas put être chargé. Veuillez vérifier les noms, formats et dimensions");
                                        if (win != null)
                                        {
                                            win.ShowDialog();
                                        }
                                    }
                                }

                            },
                            o =>
                            {
                                // Meme si le site est demarre on peut ajouter un logo, il n'est pas pris automatiquement enc compte
                                return true;
                            });
                }
                return _cmdAjouterLogo;
            }
        }

        // TODO A revoir car c'est partage

        private ICommand _cmdGetRepertoireRacine;
        /// <summary>
        /// Commande pour gérer la selection du repertoire Racine
        /// </summary>
        public ICommand CmdGetRepertoireRacine
        {
            get
            {
                if (_cmdGetRepertoireRacine == null)
                {
                    _cmdGetRepertoireRacine = new RelayCommand(
                            o =>
                            {
                                string output = string.Empty;

                                FolderBrowserDialog dlg = new FolderBrowserDialog();
                                dlg.Description = "Sélectionner le répertoire à utiliser pour les exports";
                                dlg.ShowNewFolderButton = true;
                                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    output = dlg.SelectedPath;
                                }
                                RepertoireRacine = output;
                            },
                            o =>
                            {
                                // On ne peut modifier le repertoire racine que si tous les processus sont arretes
                                return (SiteDistantSelectionne != null) ? !SiteDistantSelectionne.IsActif && !SiteLocal.IsActif && !IsGenerationActive : true;
                            });
                }
                return _cmdGetRepertoireRacine;
            }
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Assure l'initialisation de la structure du site
        /// </summary>
        private void InitExportSiteStructure()
        {
            // TODO Ajouter ici la strucuture specifique au Ecrans d'appel

            if (_structureRepertoiresPrivateSite != null)
            {
                FileAndDirectTools.CreateDirectorie(_structureRepertoiresPrivateSite.RepertoireRacine);
            }
        }

        // TODO a revoir car c'est partage
        /// <summary>
        /// Initialise la liste des fichiers de logos
        /// </summary>
        private void InitFichiersLogo()
        {
            // Recupere le repertoire des images du site
            IEnumerable<FilteredFileInfo> files = ExportTools.EnumerateCustomLogoFiles().Select(o => new FilteredFileInfo(o)).OrderBy(o => o.Name);

            // Liste les fichiers logos
            FichiersLogo = new ObservableCollection<FilteredFileInfo>(files);
        }

        // TODO A revoir car c'est partage
        /// <summary>
        /// Initialise les donnees a partir du cache de fichier AppConfig
        /// </summary>
        private void InitCacheConfig()
        {
            try
            {
                // La lecture de la config ne permet pas d'initialiser la structure du site correctement

                // On lit le repertoire racine en 1er afin de pouvoir initialiser la structure du site
                RepertoireRacine = PublicationConfigSection.Instance.RepertoireRacine;

                // Les autres parametres peuvent suivre
                NbProchainsCombats = PublicationConfigSection.Instance.NbProchainsCombats;
                DelaiGenerationSec = PublicationConfigSection.Instance.DelaiGenerationSec;
                EffacerAuDemarrage = PublicationConfigSection.Instance.EffacerAuDemarrage;
                DelaiActualisationClientSec = PublicationConfigSection.Instance.DelaiActualisationClientSec;
                AfficherPositionCombat = PublicationConfigSection.Instance.AfficherPositionCombat;

                // TODO A revoir car c'est partage
                // Recherche le logo dans la liste
                SelectedLogo = PublicationConfigSection.Instance.GetLogo(FichiersLogo.ToList(), o => o.Name);

                URLEcransAppelPublication = CalculURLPrivateSite();

                // ici on initialise les ecrans d'appel
                InitEcransAppel();
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        /// <summary>
        /// Initialise les ecrans d'appel depuis les données en configuration
        /// </summary>
        private void InitEcransAppel()
        {
            try
            {
                // Chargement des Ecrans depuis la Config vers le Modèle Runtime
                _ecransAppel = new EcranCollectionManager();

                if (EcransAppelConfigSection.Instance != null && EcransAppelConfigSection.Instance.Ecrans != null)
                {
                    foreach (EcransAppelConfigElement cfg in EcransAppelConfigSection.Instance.Ecrans)
                    {
                        // Parsing des IDs de tapis "1;2;3" -> List<int>
                        List<int> tapisIds = new List<int>();
                        if (!string.IsNullOrEmpty(cfg.TapisIds))
                        {
                            tapisIds = cfg.TapisIds.Split(';')
                                          .Select(s => int.TryParse(s, out int i) ? i : 0)
                                          .Where(i => i > 0)
                                          .ToList();
                        }

                        // On crée le modèle à partir de la config
                        IPAddress ip = IPAddress.None;
                        bool ipValid = IPAddress.TryParse(cfg.AdresseIp, out ip);
                        var model = new EcranAppelModel
                        {
                            Id = cfg.Id,
                            Description = cfg.Description,
                            Hostname = cfg.Hostname,
                            AdresseIP = ipValid ? ip : IPAddress.None,
                            TapisIds = tapisIds
                        };

                        // Ajuster le compteur statique pour éviter les doublons d'ID futurs
                        if (model.Id > EcranAppelModel.LastId)
                            EcranAppelModel.LastId = model.Id;

                        _ecransAppel.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        /// <summary>
        /// Calcul l'URL sur le site ecrans en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLPrivateSite()
        {
            string output = "Indefinie";

            try
            {
                if (!String.IsNullOrEmpty(IdCompetition) && SitePrivate.ServerHTTP != null && SitePrivate.ServerHTTP.ListeningIpAddress != null && SitePrivate.ServerHTTP.Port > 0)
                {
                    string urlBase = string.Format("http://{0}:{1}/", SitePrivate.ServerHTTP.ListeningIpAddress.ToString(), SitePrivate.ServerHTTP.Port);

                    // TODO Ajouter la structure des ecrans d'appel
                    // output = (new Uri(new Uri(urlBase), _structureSiteLocal.UrlPathIndex)).ToString();
                }
            }
            catch (Exception ex)
            {
                output = string.Empty;
                LogTools.Logger.Error(ex, "Impossible de calculer l'URL du site des ecrans d'appel");
            }
            return output;
        }


        /// <summary>
        /// Demarre le thread de generation du site
        /// </summary>
        public void StartGeneration()
        {
            // TODO Ajouter la génération des écrans d'appel

            // Status = new StatusGenerationSite(StateGenerationEnum.Idle);
            Status = StatusGenerationSite.Instance(StateGenerationEnum.Idle);

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
                    // Nettoie si necessaire le repertoire avant de lancer la tache
                    if (EffacerAuDemarrage)
                    {
                        Status = StatusGenerationSite.Instance(StateGenerationEnum.Cleaning);

                        // Efface le contenu local
                        ClearRepertoireCompetition();
                    }

                    // Lance la tache de generation
                    _taskGeneration = Task.Factory.StartNew(GenerationRun, _tokenSource.Token);
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur lors du lancement de la generation du site");
                    throw new Exception("Erreur lors du lancement de la generation du site", ex);
                }
            }
            else
            {
                LogTools.Logger.Error("Une tache de generation est deja en cours d'execution");
                throw new Exception("Une tache de generation est deja en cours d'execution");
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

        private void GenerationRun()
        {
            DateTime wakeUpTime = DateTime.Now;
            int delaiScrutationMs = 1000;

            while (!_tokenSource.Token.IsCancellationRequested)
            {
                if (DateTime.Now >= wakeUpTime)
                {
                    // Pour controler la duree total par rapport au timer
                    Stopwatch watcherTotal = new Stopwatch();
                    watcherTotal.Start();

                    try
                    {
                         // Pousse les commandes de generation dans le thread de travail
                        Status = StatusGenerationSite.Instance(StateGenerationEnum.Generating);
                        SiteGenere = false; // Reset du flag de succès pour ce cycle

                        // Commence par garantir que les données des caches sont consistantes
                        bool dataConsistent = false;
                        try
                        {
                            // Appel bloquant (avec timeout) vers GestionEvent
                            dataConsistent = ConnectedJudoDataManager.Instance.EnsureDataConstistency();
                        }
                        catch (Exception ex)
                        {
                            LogTools.Logger.Error(ex, "Exception lors du controle de la consistance donnees recues.");
                        }

                        if (dataConsistent)
                        {
                            // Recupere le snapshot des données (thread safe)
                            IJudoData snapshot = _judoDataManager.GetSnapshot();

                            // Met a jour les données de l'extension
                            ExtendedJudoData.SyncAll(snapshot);

                            StatExecution statGeneration = new StatExecution();
                            Stopwatch watcherGen = new Stopwatch();
                            watcherGen.Start();

                            try
                            {
                                // Charge le fichier de cache de checksum
                                List<FileWithChecksum> checksumCache = LoadChecksumFichiersGeneres();
                                List<FileWithChecksum> checksumGenere = GenereAll(snapshot);
                                SiteGenere = (checksumGenere.Count > 0);
                                watcherGen.Stop();
                                statGeneration.DelaiExecutionMs = watcherGen.ElapsedMilliseconds;
                                // Status = new StatusGenerationSite(StateGenerationEnum.Idle, "En attente ...");
                                Status = StatusGenerationSite.Instance(StateGenerationEnum.Idle);

                                _statMgr.EnregistrerGeneration(watcherGen.ElapsedMilliseconds / 1000F);

                                // On ne traite le transfert que si le site a bien ete generee
                                if (SiteGenere)
                                {
                                    // Met a jour la date de generation puisque le site a ete traite
                                    DerniereGeneration = statGeneration;

                                    // Si le site distant est actif, transfere la mise a jour
                                    if (SiteDistantSelectionne != null && SiteDistantSelectionne.IsActif)
                                    {
                                        try
                                        {
                                            string localRoot = _structureRepertoiresSite.RepertoireCompetition();

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

                                                // For Debug only
                                                if (filesToSync.Count <= 0)
                                                {
                                                    LogTools.Logger.Debug("Fichiers a synchroniser: {0}", string.Join(",", filesToSync.Select(f => f.Name)));
                                                }
                                            }

                                            // Synchronise le site FTP
                                            UploadStatus uploadOut = SiteDistantSelectionne.UploadSite(localRoot, filesToSync);
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
                                        catch (Exception ex)
                                        {
                                            LogTools.Logger.Error(ex, "Une erreur est survenue pendant la tentative de synchronisation");
                                            SiteSynchronise = false;
                                        }
                                    }
                                }
                                else
                                {
                                    LogTools.Logger.Debug("Site non genere, pas de synchronisation distante");
                                }
                            }
                            catch (Exception ex)
                            {
                                LogTools.Logger.Error(ex, "Une erreur est survenue durant la sequence de generation du site");
                                SiteGenere = false;
                            }
                        }
                        else
                        {
                            // Le controle d'integrite a echoue
                            LogTools.Logger.Warn("Impossible de valider l'integrite des donnees combats (Timeout ou deconnexion).");
                        }
                    }
                    finally
                    {
                        // Met toujours, via le finally, le sstatus a Idle
                        Status = StatusGenerationSite.Instance(StateGenerationEnum.Idle);

                        // Controle final si tout s'est bien passe
                        if (!SiteGenere)
                        {
                            _statMgr.EnregistrerErreurGeneration();
                        }

                        watcherTotal.Stop();

                        // Si le transfert a duree plus que le temps d'attente, on attend au plus 5 sec
                        // Sinon, on attend la difference restantes
                        int delaiThread = (int)Math.Max(DelaiGenerationSec * 1000 - watcherTotal.ElapsedMilliseconds, 5000);

                        // Met le thread en attente pour la prochaine generation
                        Status.NextGenerationSec = (int)Math.Round(delaiThread / 1000.0);

                        _statMgr.EnregistrerDelaiGeneration(delaiThread / 1000F);

                        // prochaine heure de generation
                        wakeUpTime = DateTime.Now.AddMilliseconds(delaiThread);

                        StatExecution tmp = DerniereGeneration;
                        tmp.DateProchaineGeneration = wakeUpTime;
                        DerniereGeneration = tmp;
                    }
                }

                // Endort le thread pour le delai de scrutation
                Thread.Sleep(delaiScrutationMs);
            }
        }

        /// <summary>
        /// Declenche l'exportation
        /// </summary>
        /// <param name="genere">Type d'exportation</param>
        private List<FileWithChecksum> Exporter(IJudoData dataContext, GenereSiteStruct genere, ConfigurationExportSite cfg, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<FileWithChecksum> urls = new List<FileWithChecksum>();
            ExportSite exporter = new ExportSite();

            try
            {
                ExportSiteStructure structRep = (ExportSiteStructure) _structureRepertoiresSite.Clone();  // Clone la structure de repertoires pour ne pas l'altérer dans le contexte multi-thread

                switch (genere.type)
                {
                    case TypeExportSiteEnum.AllTapis:
                        urls = exporter.GenereWebSiteAllTapis(dataContext, cfg, structRep, progress, workId);
                        break;
                    case TypeExportSiteEnum.Classement:
                        urls = exporter.GenereWebSiteClassement(dataContext, genere.phase.GetVueEpreuve(dataContext), cfg, structRep, progress, workId);
                        break;
                    case TypeExportSiteEnum.Index:
                        urls = exporter.GenereWebSiteIndex(dataContext, cfg, structRep, progress, workId);
                        break;
                    case TypeExportSiteEnum.Menu:
                        urls = exporter.GenereWebSiteMenu(dataContext, ExtendedJudoData, cfg, structRep, progress, workId);
                        break;
                    case TypeExportSiteEnum.Phase:
                        urls = exporter.GenereWebSitePhase(dataContext, genere.phase, cfg, structRep, progress, workId);
                        break;
                    case TypeExportSiteEnum.AffectationTapis:
                        urls = exporter.GenereWebSiteAffectation(dataContext, cfg, structRep, progress, workId);
                        break;
                    case TypeExportSiteEnum.Engagements:
                        urls = exporter.GenereWebSiteEngagements(dataContext, ExtendedJudoData, genere.groupeEngages, cfg, structRep, progress, workId);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur rencontree lors de l'export {0}", genere.type);
            }

            return urls;
        }

        /// <summary>
        /// Ajoute une tache de fond de generation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="phase"></param>
        /// <param name="tapis"></param>
        /// <param name="groupeP">Identifiant du groupe de participant</param>
        /// <returns></returns>
        public Task<List<FileWithChecksum>> AddWork(IJudoData dataContext, TypeExportSiteEnum type, Phase phase, int? tapis, ConfigurationExportSite cfg, List<GroupeEngagements> groupeP = null)
        {
            Task<List<FileWithChecksum>> output = null;

            if (IsGenerationActive)
            {
                GenereSiteStruct export = new GenereSiteStruct
                {
                    type = type,
                    phase = phase,
                    tapis = tapis,
                    groupeEngages = groupeP
                };

                int workId = _workCounter;    // New work ID
                _workCounter++;
                _allTaskProgress.Add(0);
                output = OutilsTools.Factory.StartNew(() =>
                {
                    Progress<GenerationProgressInfo> progress = new Progress<GenerationProgressInfo>(onReportProgress);
                    return Exporter(dataContext, export, cfg, progress, workId);
                });
            }

            return output;
        }


        /// <summary>
        /// Genere la totalite du site
        /// </summary>
        /// <returns></returns>
        public List<FileWithChecksum> GenereAll(IJudoData dataContext)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            ConfigurationExportSite cfg = new ConfigurationExportSite(PublierProchainsCombats, PublierAffectationTapis && CanPublierAffectation, PublierEngagements && CanPublierEngagements, EngagementsAbsents, EngagementsTousCombats, ScoreEngagesGagnantPerdant, AfficherPositionCombat, DelaiActualisationClientSec, NbProchainsCombats, MsgProchainsCombats, (SelectedLogo != null) ? SelectedLogo.Name : string.Empty, PouleEnColonnes, PouleToujoursEnColonnes, TailleMaxPouleColonnes, UseIntituleCommun, IntituleCommun);

            if (IsGenerationActive)
            {
                if (_generationCounter < long.MaxValue) { _generationCounter++; }                
                LogTools.Logger.Debug("Lancement de la {0}eme generation du site", _generationCounter);

                if (dataContext.Organisation.Competitions.Count > 0)
                {
                    List<Task<List<FileWithChecksum>>> listTaskGeneration = new List<Task<List<FileWithChecksum>>>();

                    // Initialise les donnees partagees de generation (ces donnees sont statiques et communes a toutes les taches)
                    ExportSite.InitSharedData(dataContext, ExtendedJudoData, cfg);

                    _allTaskProgress.Clear();
                    _workCounter = 0;
                    _nbGeneration = 0;

                    listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.Index, null, null, cfg, null));
                    listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.Menu, null, null, cfg, null));
                    if (PublierAffectationTapis && CanPublierAffectation)
                    {
                        listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.AffectationTapis, null, null, cfg, null));
                    }

                    // On ne genere pas les informations de prochains combat si ce n'est pas necessaire
                    if (PublierProchainsCombats)
                    {
                        listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.AllTapis, null, null, cfg, null));
                    }

                    
                    if(PublierEngagements && CanPublierEngagements)
                    {
                        foreach (Competition comp in dataContext.Organisation.Competitions)
                        {
                            // Recupere les groupes en fonction du type de groupement
                            List<EchelonEnum> typesGrp = ExtendedJudoData.Engagement.TypesGroupes[comp.id]; 

                            // On genere les engagements pour chaque type de groupe
                            foreach (EchelonEnum typeGrp in typesGrp)
                            {
                                List<GroupeEngagements> groupesP = ExtendedJudoData.Engagement.GroupesEngages.Where(g => g.Competition == comp.id && g.Type == (int)typeGrp).ToList();

                                // Ce code est plus efficace qye celui qui cree une tache par groupe
                                // sans doute car le lancement de nombreuses Task est couteux mais il provoque une latence a la fin de la generation
                                listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.Engagements, null, null, cfg, groupesP));

                                // foreach (GroupeEngagements g in groupesP)
                                // {
                                //   _nbTaskGeneration++;
                                //  listTaskGeneration.Add(AddWork(SiteEnum.Engagements, null, null, cfg, new List<GroupeEngagements>(1) { g }));
                                // }
                            }
                        }
                    }                    

                    foreach (Phase phase in dataContext.Deroulement.Phases)
                    {
                        listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.Phase, phase, null, cfg, null));
                        listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.Classement, phase, null, cfg, null));
                    }

                    // TODO il faut mettre un drapeau conditionnel pour activer ou non cette generation
                    // Ajoute la generation des ecrans d'appel
                    listTaskGeneration.Add(AddWork(dataContext, TypeExportSiteEnum.Phase, phase, null, cfg, null));

                    try
                    {
                        // Elimine les elements null
                        listTaskGeneration.RemoveAll(item => item == null);
                        if (listTaskGeneration.Count > 0)
                        {
                            Task<List<FileWithChecksum>> taskAttente = WaitGenereAll(listTaskGeneration);
                            // Attend la fin de la generation pour rendre la main
                            taskAttente.Wait();
                            output = taskAttente.Result;

                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Error(ex);
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
            // int totalTask = _workCounter;
            int nTask = 0;
            List<FileWithChecksum> fichiersGeneres = new List<FileWithChecksum>();
            while (listTaskGeneration.Any())
            {
                Task<List<FileWithChecksum>> finishedTask = await Task.WhenAny(listTaskGeneration.ToArray());
                listTaskGeneration.Remove(finishedTask);
                nTask++;
                // Status.Progress = (int)Math.Round(100.0 * nTask / totalTask);
                fichiersGeneres = fichiersGeneres.Concat(await finishedTask).ToList();
            }

            return fichiersGeneres;
        }


        /// <summary>
        /// Callback pour rapporter la progression de la generation du site
        /// </summary>
        /// <param name="progressInfo"></param>
        private void onReportProgress(GenerationProgressInfo progressInfo)
        {
            try
            {
                if(progressInfo.IsProgress)
                {
                    // progress est le pourcentage de retour d'une seule tache
                    _allTaskProgress[progressInfo.Id] = progressInfo.Progress;

                    // Calcul le pourcentage total de progression
                    int total = 0;
                    foreach (int p in _allTaskProgress.ToList())
                    {
                        total += p;
                    }

                    Status.Progress = (int)Math.Round(100.0 * total / _nbGeneration);
                }
                else if (progressInfo.IsInit)
                {
                    // Ajoute au nombre total de generation prevue
                    _nbGeneration += progressInfo.NbGeneration;
                }
                else
                {
                    throw new ArgumentException("Notification de progression incoherente");
                }
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex);
            }
        }

        /// <summary>
        /// Vide le contenu du repertoire de la competition
        /// </summary>
        private void ClearRepertoireCompetition()
        {
            if(_structureRepertoiresSite != null)
            {
                // Efface le contenu du repertoire de la competition
                if(!FileAndDirectTools.DeleteDirectory(_structureRepertoiresSite.RepertoireCompetition(), true))
                {
                    LogTools.Logger.Error("Erreur lors de l'effacement du contenu de  '{0}'", _structureRepertoiresSite.RepertoireCompetition());
                }

                // Charge le contenu du fichier de checksum
                List<FileWithChecksum> cache = LoadChecksumFichiersGeneres();

                // Elimine tous les fichiers commençant par le répertoire de la competition (ils ont été supprimés)
                cache.RemoveAll(f => f.File.FullName.StartsWith(_structureRepertoiresSite.RepertoireCompetition()));
                SaveChecksumFichiersGeneres(cache);
            }
        }
        #endregion
    }
}
