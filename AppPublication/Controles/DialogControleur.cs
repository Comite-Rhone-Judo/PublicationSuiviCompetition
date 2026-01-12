using AppPublication.Tools;
using AppPublication.Tools.Enum;
using KernelImpl;
using KernelImpl.Noyau.Organisation;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Tools.Logging;
using Tools.Security;
using Tools.Windows;
using Tools.Framework;
using Tools.Files;
using Tools.Outils;
using AppPublication.Tools.Streams;
using Tools.Threading;


namespace AppPublication.Controles
{
    /// <summary>
    /// Cette classe joue le role de coordinateur et de View-Model. Il regroupe les commandes et les objets metier
    /// </summary>
    public class DialogControleur : NotificationBase
    {
        #region MEMBRES
        private static DialogControleur _instance = null; // Instance unique du singleton
        private AppPublication.Views.Infos.StatistiquesView _statWindow = null;
        private AppPublication.Views.Infos.InformationsView _infoWindow = null;
        private PdfViewer _manuelViewer = null;
        private AppPublication.Views.Configuration.ConfigurationPublicationView _cfgWindow = null;
        private readonly JudoData _serverData;
        #endregion

        #region CONSTRUCTEUR

        // Constructeur privé : inaccessible depuis l'extérieur
        private DialogControleur(JudoData data)
        {
            _serverData = data ?? throw new ArgumentNullException(nameof(data));

            FileAndDirectTools.InitDataDirectories();
            InitControleur();
            AppInformation = AppInformation.Instance;
        }

        #endregion

        #region PROPRIETES

        private Competition _competition = null;
        /// <summary>
        /// On expose la competition courante pour liaison avec l'IHM (et la notification de changement)
        /// </summary>
        public Competition Competition
        {
            get
            {
                return _competition;
            }

            private set { 
                _competition = value;
                NotifyPropertyChanged();
            }
        }


        private AppInformation _appInformation = null;

        public AppInformation AppInformation
        {
            get
            {
                return _appInformation;
            }
            private set { 
                _appInformation = value;
                NotifyPropertyChanged();
            }
        }



        /// <summary>
        /// Acces a l'instance du singleton - Lecture seule
        /// </summary>
        public static DialogControleur Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("DialogControleur non initialise ! Appelez DialogControleur.CreateInstance()");
                return _instance;
            }
        }

        private BusyStatusEnum _busyStatus = BusyStatusEnum.None;
        /// <summary>
        /// L'etat d'occupation de l'application (pendant le chargement des données)
        /// </summary>
        public BusyStatusEnum BusyStatus
        {
            get
            {
                return _busyStatus;
            }
            set
            {
                _busyStatus = value;
                NotifyPropertyChanged();
            }
        }


        private GestionConnection _connection = null;
        /// <summary>
        /// Le gestionnaire de la connexion au serveur - Lecture seule
        /// </summary>
        public GestionConnection Connection
        {
            get
            {
                return _connection;
            }
        }


        private GestionSite _site = null;
        /// <summary>
        /// Le gestionnaire des site de publication
        /// </summary>
        public GestionSite GestionSite
        {
            get { return _site; }
            private set { _site = value; }
        }

        private GestionStatistiques _stats = null;
        /// <summary>
        /// Le gestionnaire des site de publication
        /// </summary>
        public GestionStatistiques GestionStatistiques
        {
            get { return _stats; }
            private set { _stats = value; }
        }

        /// <summary>
        /// Le bloc de donnees recupere du serveur
        /// </summary>
        public JudoData ServerData
        {
            get
            {
                return _serverData;
            }
        }

        private bool _isBusy;
        /// <summary>
        /// Indique si l'application est occupee (chargement de données)
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private bool _tracesDebugOn  =false;
        /// <summary>
        /// Indique si les traces avancees sont activees
        /// </summary>
        public bool TracesDebugOn
        {
            get
            {
                return _tracesDebugOn;
            }
            set
            {
                _tracesDebugOn = value;
                NotifyPropertyChanged();

                LogTools.ConfigureDebugLevel(_tracesDebugOn);
            }
        }

        private bool _canTraceDebug;
        public bool CanManageTracesDebug
        {
            get
            {
                return _canTraceDebug;
            }
            set
            {
                _canTraceDebug = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Seule méthode autorisée pour créer l'instance unique.
        /// </summary>
        public static DialogControleur CreateInstance(JudoData data)
        {
            if (_instance != null)
                throw new InvalidOperationException("Violation du Singleton : DialogControleur deja instancie.");

            _instance = new DialogControleur(data);
            return _instance;
        }

        /// <summary>
        /// Actualise l'ID de competition (necessaire pour faire le lien avec la reception des donnees)
        /// </summary>
        public void UpdateCompetition()
        {
            Competition = ServerData.Organisation.Competition;  // Met a jour la competition courante pour l'IHM
            GestionSite.IdCompetition = (ServerData.Organisation.Competition != null) ? ServerData.Organisation.Competition.remoteId : string.Empty;
        }

        /// <summary>
        /// Initialisation du controleur
        /// </summary>
        private void InitControleur()
        {
            Application.Current.ExecOnUiThread(new Action(() =>
            {
                try
                {
                    // Commence par le gestionnaire de statistiques
                    _stats = new GestionStatistiques();

                    // Initialise le gestionnaire de connexion
                    _connection = new GestionConnection();
                    // et on s'abonne aux evenements pour pouvoir mettre a jour l'IHM
                    _connection.ClientReady += OnClientReady;
                    _connection.ClientDisconnected += OnClientDisconnected;

                    // Initialise le gestionnaire d'evenements
                    var evtMgr = ConnectedJudoDataManager.CreateInstance(this.ServerData, _stats.Donnees, _connection);
                    // et on s'abonne aux evenements pour pouvoir mettre a jour l'IHM
                    evtMgr.BusyStatusChanged += OnBusyStatusChanged;
                    evtMgr.DataUpdated += OnDataUpdated;

                    // Le gestionnaire de site de publication. On passe EvtMgr comme gestionnaire de donnees car il gere la reception des donnees
                    // et fait l'interface avec le noyau interne de donnees
                    _site = new GestionSite(evtMgr, _stats);
                }
                catch (Exception ex)
                {
                    LogTools.Error(ex);
                }
            }));
        }

        #endregion

        #region COMMANDES

        private ICommand _cmdAcquitterErreurCommunication = null;

        /// <summary>
        /// Commande permettant d'acquitter une erreur de communication
        /// </summary>
        public ICommand CmdAcquitterErreurCommunication
        {
            get
            {
                if (_cmdAcquitterErreurCommunication == null)
                {
                    _cmdAcquitterErreurCommunication = new RelayCommand(
                            o =>
                            {
                                if (Connection != null && Connection.HasErreurTransmission)
                                {
                                    LogTools.Logger.Info("Erreur de transmission acquittee par l'utilisateur.");
                                    Connection.HasErreurTransmission = false;
                                }
                            },
                            o =>
                            {
                                return true;
                            });
                }
                return _cmdAcquitterErreurCommunication;
            }
        }


        private ICommand _cmdCopyUrlLocal = null;
        /// <summary>
        /// Commande de copy de l'URL local dans la presse papier
        /// </summary>
        public ICommand CmdCopyUrlLocal
        {
            get
            {
                if (_cmdCopyUrlLocal == null)
                {
                    _cmdCopyUrlLocal = new RelayCommand(
                            o =>
                            {
                                if (GestionSite.SiteLocal.IsActif)
                                {
                                    Clipboard.SetText(GestionSite.URLLocalPublication);
                                }
                            },
                            o =>
                            {
                                return GestionSite.SiteLocal.IsActif;
                            });
                }
                return _cmdCopyUrlLocal;
            }
        }

        private ICommand _cmdCopyUrlDistant = null;
        /// <summary>
        /// Commande de copy de l'URL local dans la presse papier
        /// </summary>
        public ICommand CmdCopyUrlDistant
        {
            get
            {
                if (_cmdCopyUrlDistant == null)
                {
                    _cmdCopyUrlDistant = new RelayCommand(
                            o =>
                            {
                                if (GestionSite.SiteDistantSelectionne != null && GestionSite.SiteDistantSelectionne.IsActif)
                                {
                                    Clipboard.SetText(GestionSite.URLDistantPublication);
                                }
                            },
                            o =>
                            {
                                return (GestionSite.SiteDistantSelectionne != null) ? GestionSite.SiteDistantSelectionne.IsActif : false;
                            });
                }
                return _cmdCopyUrlDistant;
            }
        }

        private ICommand _cmdCopyUrlEcransAppel = null;
        /// <summary>
        /// Commande de copy de l'URL des ecrans d'appel dans la presse papier
        /// </summary>
        public ICommand CmdCopyUrlEcransAppel
        {
            get
            {
                if (_cmdCopyUrlEcransAppel == null)
                {
                    _cmdCopyUrlEcransAppel = new RelayCommand(
                            o =>
                            {
                                if (GestionSite.SitePrivate != null && GestionSite.SitePrivate.IsActif)
                                {
                                    Clipboard.SetText(GestionSite.URLEcransAppelPublication);
                                }
                            },
                            o =>
                            {
                                return (GestionSite.SitePrivate != null) ? GestionSite.SitePrivate.IsActif : false;
                            });
                }
                return _cmdCopyUrlEcransAppel;
            }
        }

        private ICommand _cmdDemarrerSiteLocal = null;
        /// <summary>
        /// Command de demarrage du site local
        /// </summary>
        public ICommand CmdDemarrerSiteLocal
        {
            get
            {
                if (_cmdDemarrerSiteLocal == null)
                {
                    _cmdDemarrerSiteLocal = new RelayCommand(
                            o =>
                            {
                                if (Instance.GestionSite.SiteLocal != null && !Instance.GestionSite.SiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.SiteLocal.StartSite();

                                    // Force la mise a jour de l'URL
                                    Instance.GestionSite.IdCompetition = Instance.GestionSite.IdCompetition;
                                }
                            },
                            o =>
                            {
                                return !String.IsNullOrEmpty(Instance.GestionSite.IdCompetition) && !Instance.GestionSite.SiteLocal.IsActif && Instance.GestionSite.SiteLocal.IsChanged;
                            });
                }
                return _cmdDemarrerSiteLocal;
            }
        }

        private ICommand _cmdArreterSiteLocal = null;
        /// <summary>
        /// Commande d'arret du site local
        /// </summary>
        public ICommand CmdArreterSiteLocal
        {
            get
            {
                if (_cmdArreterSiteLocal == null)
                {
                    _cmdArreterSiteLocal = new RelayCommand(
                            o =>
                            {
                                if (Instance.GestionSite.SiteLocal != null && Instance.GestionSite.SiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.SiteLocal.StopSite();
                                }
                            },
                            o =>
                            {
                                return Instance.GestionSite.SiteLocal.IsActif;
                            });
                }
                return _cmdArreterSiteLocal;
            }
        }

        private ICommand _cmdDemarrerSitePrivate = null;
        /// <summary>
        /// Command de demarrage du site des ecrans d'appel
        /// </summary>
        public ICommand CmdDemarrerSitePrivate
        {
            get
            {
                if (_cmdDemarrerSitePrivate == null)
                {
                    _cmdDemarrerSitePrivate = new RelayCommand(
                            o =>
                            {
                                if (Instance.GestionSite.SitePrivate != null && !Instance.GestionSite.SitePrivate.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.SitePrivate.StartSite();

                                    // Force la mise a jour de l'URL
                                    Instance.GestionSite.IdCompetition = Instance.GestionSite.IdCompetition;
                                }
                            },
                            o =>
                            {
                                return !String.IsNullOrEmpty(Instance.GestionSite.IdCompetition) && !Instance.GestionSite.SitePrivate.IsActif && Instance.GestionSite.SitePrivate.IsChanged;
                            });
                }
                return _cmdDemarrerSitePrivate;
            }
        }

        private ICommand _cmdArreterPrivateSite = null;
        /// <summary>
        /// Commande d'arret du site des ecrans d'appel
        /// </summary>
        public ICommand CmdArreterPrivateSite
        {
            get
            {
                if (_cmdArreterPrivateSite == null)
                {
                    _cmdArreterPrivateSite = new RelayCommand(
                            o =>
                            {
                                if (Instance.GestionSite.SitePrivate != null && Instance.GestionSite.SitePrivate.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.SitePrivate.StopSite();
                                }
                            },
                            o =>
                            {
                                return Instance.GestionSite.SitePrivate.IsActif;
                            });
                }
                return _cmdArreterPrivateSite;
            }
        }

        private ICommand _cmdDemarrerSiteDistant = null;
        /// <summary>
        /// Commande de demarrage du site distant
        /// </summary>
        public ICommand CmdDemarrerSiteDistant
        {
            get
            {
                if (_cmdDemarrerSiteDistant == null)
                {
                    _cmdDemarrerSiteDistant = new RelayCommand(
                            o =>
                            {
                                if (Instance.GestionSite.SiteDistantSelectionne != null && !Instance.GestionSite.SiteDistantSelectionne.IsActif)
                                {
                                    // Extrait le mode de passe des controles passes en parametres (1er = FranceJudo, 2nd = Advanced)
                                    if (o.GetType() == typeof(Tuple<object, object>))
                                    {
                                        Tuple<object, object> tuple = (Tuple<object, object>)o;

                                        if(tuple.Item1 != null && tuple.Item1.GetType() == typeof(RadPasswordBox))
                                        {
                                            Instance.GestionSite.SiteFranceJudo.PasswordSiteFTPDistant = Encryption.ToInsecureString(((RadPasswordBox)tuple.Item1).SecurePassword);
                                        }
                                        if (tuple.Item2 != null && tuple.Item2.GetType() == typeof(RadPasswordBox))
                                        {
                                            Instance.GestionSite.SiteDistant.PasswordSiteFTPDistant = Encryption.ToInsecureString(((RadPasswordBox)tuple.Item2).SecurePassword);
                                        }
                                    }

                                    // Demarre le site distant selectione
                                    Instance.GestionSite.SiteDistantSelectionne.StartSite();
                                }
                            },
                            o =>
                            {
                                return (Instance.GestionSite.SiteDistantSelectionne == null) ? false : !Instance.GestionSite.SiteDistantSelectionne.IsActif && !String.IsNullOrEmpty(Instance.GestionSite.IdCompetition);
                            });
                }
                return _cmdDemarrerSiteDistant;
            }
        }

        private ICommand _cmdArreterSiteDistant = null;
        /// <summary>
        /// Commande d'arret du site distant
        /// </summary>
        public ICommand CmdArreterSiteDistant
        {
            get
            {
                if (_cmdArreterSiteDistant == null)
                {
                    _cmdArreterSiteDistant = new RelayCommand(
                            o =>
                            {
                                if (Instance.GestionSite.SiteDistantSelectionne != null && Instance.GestionSite.SiteDistantSelectionne.IsActif)
                                {
                                    // Arrete le site concernee
                                    Instance.GestionSite.SiteDistantSelectionne.StopSite();
                                }
                            },
                            o =>
                            {
                                return (Instance.GestionSite.SiteDistantSelectionne != null) ?  Instance.GestionSite.SiteDistantSelectionne.IsActif : false;
                            });
                }
                return _cmdArreterSiteDistant;
            }
        }

        private ICommand _cmdNettoyerSiteDistant = null;
        /// <summary>
        /// Commande de nettoyage du site distant
        /// </summary>
        public ICommand CmdNettoyerSiteDistant
        {
            get
            {
                if (_cmdNettoyerSiteDistant == null)
                {
                    _cmdNettoyerSiteDistant = new RelayCommand(
                            o =>
                            {
                                if ( Instance.GestionSite.SiteDistantSelectionne != null && !Instance.GestionSite.SiteDistantSelectionne.IsActif)
                                {
                                    DialogParameters param = new DialogParameters();
                                    param.OkButtonContent = "Oui";
                                    param.CancelButtonContent = "Non";
                                    param.Content = $"Etes-vous sûr de vouloir supprimer le contenu de '{Instance.GestionSite.SiteDistantSelectionne.RepertoireSiteFTPDistant}' sur le site distant ?";
                                    param.Header = "Nettoyer site distant";

                                    ConfirmWindow win = new ConfirmWindow(param);
                                    win.ShowDialog();

                                    if (win.DialogResult.HasValue && (bool)win.DialogResult)
                                    {
                                        // Nettoyer le site distant
                                        // Instance.GestionSite.SiteDistant.NettoyerSite();
                                        GestionSite.StartNettoyage();
                                    }
                                }
                            },
                            o =>
                            {
                                return (Instance.GestionSite.SiteDistantSelectionne == null) ? false : !Instance.GestionSite.SiteDistantSelectionne.IsActif && !Instance.GestionSite.SiteDistantSelectionne.IsCleaning;
                            });
                }
                return _cmdNettoyerSiteDistant;
            }
        }

        private ICommand _cmdDemarrerGeneration = null;
        /// <summary>
        /// Comamnde de demarrage de la generation du site
        /// </summary>
        public ICommand CmdDemarrerGeneration
        {
            get
            {
                if (_cmdDemarrerGeneration == null)
                {
                    _cmdDemarrerGeneration = new RelayCommand(
                            o =>
                            {
                                Instance.GestionSite.StartGeneration();
                            },
                            o =>
                            {
                                return !String.IsNullOrEmpty(Instance.GestionSite.IdCompetition) && !Instance.GestionSite.IsGenerationActive;
                            });
                }
                return _cmdDemarrerGeneration;
            }
        }

        private ICommand _cmdArreterGeneration = null;
        /// <summary>
        /// Commande d'arret de la generation du site
        /// </summary>
        public ICommand CmdArreterGeneration
        {
            get
            {
                if (_cmdArreterGeneration == null)
                {
                    _cmdArreterGeneration = new RelayCommand(
                            o =>
                            {
                                // Active le statut d'attente
                                DialogControleur.Instance.BusyStatus = BusyStatusEnum.AttenteFinGeneration;
                                DialogControleur.Instance.IsBusy = true;

                                // Lance l'arret dans une tache pour liberer le thread courant (UI)
                                Task<int> stopTask = Task.Factory.StartNew( () =>
                                {
                                    Instance.GestionSite.StopGeneration();
                                    return 1;
                                });

                                // Indique de remettre le status en place a la fin de l'arret pour liberer l'IHM
                                stopTask.ContinueWith(
                                 (task) =>
                                 {
                                     // On remet l'etat d'occupation a None
                                     System.Windows.Application.Current.ExecOnUiThread(new Action(() =>
                                     {
                                         DialogControleur.Instance.BusyStatus = BusyStatusEnum.None;
                                         DialogControleur.Instance.IsBusy = false;
                                     }));
                                 });

                                },
                            o =>
                            {
                                return Instance.GestionSite.IsGenerationActive;
                            });
                }
                return _cmdArreterGeneration;
            }
        }

        private ICommand _cmdAfficherSiteLocal = null;
        /// <summary>
        /// Commande d'affichage du site en local
        /// </summary>
        public ICommand CmdAfficherSiteLocal
        {
            get
            {
                if (_cmdAfficherSiteLocal == null)
                {
                    _cmdAfficherSiteLocal = new RelayCommand(
                            o =>
                            {
                                if (GestionSite.SiteLocal.IsLocal && GestionSite.SiteLocal.IsActif)
                                {
                                    string url = GestionSite.URLLocalPublication;

                                    if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                                    {
                                        System.Diagnostics.Process.Start(url);
                                    }
                                }
                            },
                            o =>
                            {
                                return  GestionSite.SiteLocal.IsActif && GestionSite.SiteLocal.IsLocal;
                            });
                }
                return _cmdAfficherSiteLocal;
            }
        }

        private ICommand _cmdAfficherSiteDistant = null;
        /// <summary>
        /// Commande d'affichage du site en local
        /// </summary>
        public ICommand CmdAfficherSiteDistant
        {
            get
            {
                if (_cmdAfficherSiteDistant == null)
                {
                    _cmdAfficherSiteDistant = new RelayCommand(
                            o =>
                            {
                                if ( (GestionSite.SiteDistantSelectionne != null) && !GestionSite.SiteDistantSelectionne.IsLocal)
                                {
                                    string url = GestionSite.URLDistantPublication;

                                    if(Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                                    {
                                        System.Diagnostics.Process.Start(url);
                                    }
                                }
                            },
                            o =>
                            {
                                // on ne peut pas ouvrir l'URL si on n'est pas connecte a une competition
                                return (GestionSite.SiteDistantSelectionne != null) ? !GestionSite.SiteDistantSelectionne.IsLocal && !String.IsNullOrEmpty(Instance.GestionSite.IdCompetition) : false;
                            });
                }
                return _cmdAfficherSiteDistant;
            }
        }

        private ICommand _cmdAfficherInformations = null;
        public ICommand CmdAfficherInformations
        {
            get
            {
                if (_cmdAfficherInformations == null)
                {
                    _cmdAfficherInformations = new RelayCommand(
                            o =>
                            {
                                if (_infoWindow == null)
                                {
                                    _infoWindow = new AppPublication.Views.Infos.InformationsView();
                                }
                                if (_infoWindow != null)
                                {
                                    _infoWindow.IsTopmost = true;
                                    _infoWindow.Show();
                                }
                            },
                            o =>
                            {
                                return true;
                            });
                }
                return _cmdAfficherInformations;
            }
        }

        private ICommand _cmdAfficherManuel = null;
        public ICommand CmdAfficherManuel
        {
            get
            {
                if (_cmdAfficherManuel == null)
                {
                    _cmdAfficherManuel = new RelayCommand(
                            o =>
                            {
                                if (_manuelViewer == null)
                                {
                                    System.IO.Stream manuelStream = ResourcesTools.GetAssembyResource("AppPublication.Documentation.ManuelUtilisateur.pdf", true);
                                    if(manuelStream != null)
                                    {
                                        byte[] bytes = manuelStream.ReadAllBytes();
                                        // Fenetre de visualisation du manuel utilisateur (sans impression)
                                        _manuelViewer = new PdfViewer(bytes, "Manuel utilisateur", false, true);
                                    }
                                }
                                if (_manuelViewer != null)
                                {
                                    _manuelViewer.Show();
                                    _manuelViewer.BringToFront();
                                }
                            },
                            o =>
                            {
                                return true;
                            });
                }
                return _cmdAfficherManuel;
            }
        }

        private ICommand _cmdAfficherStatistiques = null;
        /// <summary>
        /// Commande d'arret de la generation du site
        /// </summary>
        public ICommand CmdAfficherStatistiques
        {
            get
            {
                if (_cmdAfficherStatistiques == null)
                {
                    _cmdAfficherStatistiques = new RelayCommand(
                            o =>
                            {
                                if (_statWindow == null)
                                {
                                    _statWindow = new AppPublication.Views.Infos.StatistiquesView(GestionStatistiques);
                                }

                                if (_statWindow != null)
                                {
                                    _statWindow.Show();
                                    _statWindow.BringToFront();
                                }
                            },
                            o =>
                            {
                                return true;
                            });
                }
                return _cmdAfficherStatistiques;
            }
        }

        private ICommand _cmdAfficherConfiguration = null;
        /// <summary>
        /// Commande d'affichage de la configuration
        /// </summary>
        public ICommand CmdAfficherConfiguration
        {
            get
            {
                if (_cmdAfficherConfiguration == null)
                {
                    _cmdAfficherConfiguration = new RelayCommand(
                            o =>
                            {
                                if (_cfgWindow == null)
                                {
                                    _cfgWindow = new AppPublication.Views.Configuration.ConfigurationPublicationView(GestionSite);
                                }
                                if (_cfgWindow != null)
                                {
                                    _cfgWindow.ShowDialog();
                                    _cfgWindow = null;
                                }
                            },
                            o =>
                            {
                                return !Instance.GestionSite.IsGenerationActive;
                            });
                }
                return _cmdAfficherConfiguration;
            }
        }

        private ICommand _cmdGenererTracesIncident = null;
        /// <summary>
        /// Commande d'affichage de la configuration
        /// </summary>
        public ICommand CmdGenererTracesIncident
        {
            get
            {
                if (_cmdGenererTracesIncident == null)
                {
                    _cmdGenererTracesIncident = new RelayCommand(
                            o =>
                            {
                                string msg = string.Empty;

                                try
                                {
                                    // Par defaut, on va generer le fichier sur le bureau
                                    string destDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                                    string destZip = string.Format("LogAppPublication_{0:yyyyMMdd-HHmmss}.zip",DateTime.Now);

                                    string fulldestZip = Path.Combine(destDir, destZip);

                                    LogTools.PackageLog(fulldestZip);

                                    msg = string.Format("Les traces de l'application sont disponibles sur le bureau dans l'archive '{0}'. Vous pouvez joindre ce fichier au rapport d'incident.", destZip);
                                }
                                catch (Exception ex)
                                {
                                    LogTools.Logger.Error(ex, "Impossible de creer l'archive de trace de l'application '{0}'", o);
                                    msg = string.Format("Impossibles de créer l'archive des traces de l'application. Consultez le fichier de trace ou contacter le support technique.");
                                }
                                finally
                                {
                                    AlertWindow win = new AlertWindow("Infomation", msg);
                                    if (win != null)
                                    {
                                        // On doit la mettre TopMost car la fenêtre appelante l'est deja et pourrait la masquer.
                                        win.IsTopmost = true;
                                        win.ShowDialog();
                                    }
                                }
                            },
                            o =>
                            {
                                return CanManageTracesDebug;
                            });
                }
                return _cmdGenererTracesIncident;
            }
        }

        #endregion

        #region EVENT HANDLER

        /// <summary>
        /// Gestion de l'evenement de changement de statut d'occupation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBusyStatusChanged(object sender, BusyStatusEventArgs e)
        {
            System.Windows.Application.Current.ExecOnUiThread(new Action(() =>
            {
                DialogControleur.Instance.IsBusy = e.IsBusy;
                if (e.IsBusy)
                {
                    DialogControleur.Instance.BusyStatus = e.Status;
                }
            }
            ));
        }

        /// <summary>
        /// Evenement de mise a jour des donnees
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataUpdated(object sender, DataUpdateEventArgs e)
        {
            LogTools.Logger.Debug("Donnees mises a jour pour la categorie: {0}", e.CategorieDonnee.ToString());

            if (e.CategorieDonnee == KernelImpl.Enum.CategorieDonneesEnum.Organisation)
            {
                this.UpdateCompetition();
            }
        }

        /// <summary>
        /// Traitement de l'evenement de disponibilite du client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientReady(object sender, ClientReadyEventArgs e)
        {
            LogTools.Logger.Info("Client connecte et pret: {0}", e.Client.NetworkClient.IP);
        }

        /// <summary>
        /// Evenement de deconnexion du client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            LogTools.Logger.Info("Client deconnecte a {0}", e.DisconnectionTime);

            Application.Current.ExecOnUiThread(() =>
            {
                this.IsBusy = false;
                this.BusyStatus = BusyStatusEnum.None;
            });
        }
        #endregion
    }
}
