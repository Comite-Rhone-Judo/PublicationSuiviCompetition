using AppPublication.Tools;
using AppPublication.Tools.Enum;
using KernelImpl;
using System;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;
using Tools.Windows;

namespace AppPublication.Controles
{
    /// <summary>
    /// Cette classe joue le role de coordinateur et de View-Model. Il regroupe les commandes et les objets metier
    /// </summary>
    public class DialogControleur : NotificationBase
    {
        #region MEMBRES
        private static DialogControleur _currentControleur = null;      // instance singletion
        private AppPublication.IHM.Commissaire.StatistiquesView _statWindow = null;
        private AppPublication.IHM.Commissaire.InformationsView _infoWindow = null;
        private PdfViewer _manuelViewer = null;
        private AppPublication.IHM.Commissaire.ConfigurationPublication _cfgWindow = null;
        #endregion

        #region CONSTRUCTEUR

        private DialogControleur()
        {
            FileAndDirectTools.InitDataDirectories();

            InitControleur();

            // Initialise les informations de l'application
            AppInformation = AppInformation.Instance;
        }

        #endregion

        #region PROPRIETES

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
                if (null == _currentControleur)
                {
                    _currentControleur = new DialogControleur();
                }
                return _currentControleur;
            }
            private set
            {
                _currentControleur = value;
            }
        }

        private BusyStatusEnum _busyStatus = BusyStatusEnum.InitDonneesNone;
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


        private GestionConnection _connection = new GestionConnection();
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

        private JudoData _serverData;
        /// <summary>
        /// Le bloc de donnees recupere du serveur
        /// </summary>
        public JudoData ServerData
        {
            get
            {
                if (_serverData == null)
                {
                    _serverData = KernelManager.Manager.manager.m_JudoData;
                }
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
        /// Actualise l'ID de competition (necessaire pour faire le lien avec la reception des donnees)
        /// </summary>
        public void UpdateCompetition()
        {
            GestionSite.IdCompetition = (ServerData.competition != null) ? ServerData.competition.remoteId : string.Empty;
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
                    _connection = new GestionConnection();
                    _stats = new GestionStatistiques();
                    _site = new GestionSite(_stats);

                }
                catch (Exception ex)
                {
                    LogTools.Error(ex);
                }
            }));
        }

        #endregion

        #region COMMANDES



        

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
                                    param.Content = "Etes-vous sûr de vouloir supprimer le contenu du site distant ?";
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
                                return (Instance.GestionSite.SiteDistantSelectionne == null) ? false : !Instance.GestionSite.SiteDistant.IsActif && !Instance.GestionSite.SiteDistant.IsCleaning;
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
                                Instance.GestionSite.StopGeneration();
                            },
                            o =>
                            {
                                return Instance.GestionSite.IsGenerationActive;
                            });
                }
                return _cmdArreterGeneration;
            }
        }

        /*private void ButSite_Click_1(object sender, RoutedEventArgs e)
        {
            DialogControleur DC = DialogControleur.Instance;
            try
            {
                string url = "";
                if (DialogControleur.Instance.GestionSite.SiteLocal.IsLocal)
                {
                    url = ExportTools.GetURLSiteLocal(
                         DialogControleur.Instance.GestionSite.SiteLocal.ServerHTTP.ListeningIpAddress.ToString(),
                         DialogControleur.Instance.GestionSite.SiteLocal.ServerHTTP.Port,
                         DialogControleur.Instance.ServerData.competition.remoteId);
                }
                else if (!DialogControleur.Instance.GestionSite.SiteLocal.IsLocal && DialogControleur.Instance.GestionSite.SiteLocal.SiteFTPDistant == NetworkTools.FTP_EJUDO_SUIVI_URL)
                {
                    url = ExportTools.GetURLSiteFTP(DialogControleur.Instance.ServerData.competition.remoteId);
                }

                System.Diagnostics.Process.Start(url);
            }
            catch { }
        }*/

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
                                    _infoWindow = new AppPublication.IHM.Commissaire.InformationsView();
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
                                    _statWindow = new AppPublication.IHM.Commissaire.StatistiquesView(GestionStatistiques);
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
                                    _cfgWindow = new AppPublication.IHM.Commissaire.ConfigurationPublication(GestionSite);
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
                                    LogTools.Logger.Error("Impossible de creer l'archive de trace de l'application '{0}'", o, ex);
                                    msg = string.Format("Impossibles de créer l'archive des traces de l'application. Consultez le fichier de trace ou contacter le support technique.");
                                }
                                finally
                                {
                                    AlertWindow win = new AlertWindow("Infomation", msg);
                                    if (win != null)
                                    {
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
    }
}
