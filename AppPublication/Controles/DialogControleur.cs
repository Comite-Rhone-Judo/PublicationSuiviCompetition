using AppPublication.Data;
using AppPublication.Models.Statistiques;
using AppPublication.Tools.Enum;
using AppPublication.ViewModels.Configuration;
using AppPublication.Views.Configuration;
using FranceJudo.Core.Environment;
using FranceJudo.Core.Foundation;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Network;
using FranceJudo.Core.Reflection;
using FranceJudo.Core.Security;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.UI.Wpf.Dialogs;
using FranceJudo.UI.Wpf.Foundation;
using KernelImpl;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

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
        private readonly JudoData _serverData;
        private bool _startSiteDistantEnCours = false;
        private bool _nettoyageEnCours = false;
        #endregion

        #region CONSTRUCTEUR

        // Constructeur privé : inaccessible depuis l'extérieur
        private DialogControleur(JudoData data)
        {
            _serverData = data ?? throw new ArgumentNullException(nameof(data));


            string dataPath = AppEnvironment.GetDataDirectory();
            string appPath = AppEnvironment.GetAppDirectory();

            AppDirectoryManager.Initialize(dataPath, appPath);
            InitControleur();
            AppInformation = AppInformation.Instance;
        }

        #endregion

        #region PROPRIETES

        /// <summary>
        /// Indique si un démarrage de site distant est en cours
        /// </summary>
        public bool StartSiteDistantEnCours
        {
            get { return _startSiteDistantEnCours; }
            set { _startSiteDistantEnCours = value; NotifyPropertyChanged(); } // Invalide nativement les commandes
        }


        /// <summary>
        /// Indique si un nettoyage du site FTP est en cours
        /// </summary>
        public bool NettoyageEnCours
        {
            get { return _nettoyageEnCours; }
            set { _nettoyageEnCours = value; NotifyPropertyChanged(); } // Invalide nativement les commandes
        }

        private ICompetition _competition = null;
        /// <summary>
        /// On expose la competition courante pour liaison avec l'IHM (et la notification de changement)
        /// </summary>
        public ICompetition Competition
        {
            get
            {
                return _competition;
            }

            private set
            {
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
            private set
            {
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


        private SitePublicationCoordinator _siteCoord = null;
        /// <summary>
        /// Le gestionnaire des site de publication
        /// </summary>
        public SitePublicationCoordinator SiteCoordinator
        {
            get { return _siteCoord; }
            private set { _siteCoord = value; }
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

        private bool _tracesDebugOn = false;
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

            SiteCoordinator.IdCompetition = (ServerData.Organisation.Competition != null) ? ServerData.Organisation.Competition.remoteId : string.Empty;
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

                    _siteCoord = new SitePublicationCoordinator(evtMgr, _stats);
                }
                catch (Exception ex)
                {
                    LogTools.Error(ex);
                }
            }));
        }

        #endregion

        #region COMMANDES


        private ICommand _cmdAfficherTestFtp = null;

        /// <summary>
        /// Commande permettant d'afficher la fenetre de test de connexion FTP pour le site selectionne
        /// </summary>
        public ICommand CmdAfficherTestFtp
        {
            get
            {
                _cmdAfficherTestFtp ??= new RelayCommand(
                        o =>
                        {
                            // Extrait le mode de passe des controles passes en parametres (1er = FranceJudo, 2nd = Advanced)
                            ExtractPasswordFromParameters(o);

                            // Lecture directe de la propriété courante du ViewModel
                            MiniSite siteToTest = SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne;

                            if (siteToTest != null)
                            {
                                var testViewModel = new TestFtpViewModel(siteToTest);
                                var testWindow = new TestFtpWindow(testViewModel)
                                {
                                    Owner = App.Current.MainWindow
                                };
                                testWindow.ShowDialog();
                            }
                        },
                        o =>
                        {
                            // Actif uniquement si on a un site sélectionné
                            return SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null
                                        && SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsFTPConfigPropertiesValid
                                        && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif;
                        });
                return _cmdAfficherTestFtp;
            }
        }

        private ICommand _cmdAcquitterErreurCommunication = null;

        /// <summary>
        /// Commande permettant d'acquitter une erreur de communication
        /// </summary>
        public ICommand CmdAcquitterErreurCommunication
        {
            get
            {
                _cmdAcquitterErreurCommunication ??= new RelayCommand(
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
                _cmdCopyUrlLocal ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif)
                                {
                                    Clipboard.SetText(SiteCoordinator.GestionnaireSitePublique.URLLocalPublication);
                                }
                            },
                            o =>
                            {


                                return SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif;
                            });
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
                _cmdCopyUrlDistant ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null && SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif)
                                {
                                    Clipboard.SetText(SiteCoordinator.GestionnaireSitePublique.URLDistantPublication);
                                }
                            },
                            o =>
                            {
                                return (SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null) && SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif;
                            });
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
                _cmdCopyUrlEcransAppel ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif)
                                {
                                    Clipboard.SetText(SiteCoordinator.GestionnaireSiteInterne.URLLocalPublication);
                                }
                            },
                            o =>
                            {
                                return (SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null) && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif;
                            });
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
                _cmdDemarrerSiteLocal ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && !SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    SiteCoordinator.GestionnaireSitePublique.SiteLocal.StartSite();

                                    // Force la mise a jour de l'URL
                                    SiteCoordinator.GestionnaireSitePublique.ForceRefreshUrls();
                                }
                            },
                            o =>
                            {
                                return SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSitePublique.IdCompetition) && !SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsChanged;
                            });
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
                _cmdArreterSiteLocal ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    SiteCoordinator.GestionnaireSitePublique.SiteLocal.StopSite();
                                }
                            },
                            o =>
                            {
                                return SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif;
                            });
                return _cmdArreterSiteLocal;
            }
        }

        private ICommand _cmdDemarrerSiteInterne = null;
        /// <summary>
        /// Command de demarrage du site des ecrans d'appel
        /// </summary>
        public ICommand CmdDemarrerSiteInterne
        {
            get
            {
                _cmdDemarrerSiteInterne ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null && !SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    SiteCoordinator.GestionnaireSiteInterne.SiteLocal.StartSite();

                                    // Force la mise a jour de l'URL
                                    SiteCoordinator.GestionnaireSiteInterne.ForceRefreshUrls();
                                }
                            },
                            o =>
                            {
                                return SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null && !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSiteInterne.IdCompetition) && !SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsChanged;
                            });
                return _cmdDemarrerSiteInterne;
            }
        }

        private ICommand _cmdArreterSiteInterne = null;
        /// <summary>
        /// Commande d'arret du site des ecrans d'appel
        /// </summary>
        public ICommand CmdArreterSiteInterne
        {
            get
            {
                _cmdArreterSiteInterne ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    SiteCoordinator.GestionnaireSiteInterne.SiteLocal.StopSite();
                                }
                            },
                            o =>
                            {
                                return SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif;
                            });
                return _cmdArreterSiteInterne;
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
                _cmdDemarrerSiteDistant ??= new RelayCommand(
                            async o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif)
                                {
                                    // 2. On verrouille le bouton et on force WPF à mettre à jour l'IHM
                                    StartSiteDistantEnCours = true;

                                    try
                                    {
                                        // 3. LECTURE DES DONNÉES UI (Doit obligatoirement rester ici, hors du Task.Run)
                                        ExtractPasswordFromParameters(o);

                                        // 4. TRAITEMENT LONG EN ARRIÈRE-PLAN
                                        // Demarre le site distant selectione sans figer l'interface
                                        await Task.Run(() =>
                                        {
                                            SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.StartSite();
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        // Gérer l'erreur (Log + notification utilisateur)
                                        LogTools.Logger.Error(ex, "Erreur lors du démarrage du site distant.");
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            AlertWindow win = new AlertWindow("Erreur", "Impossible de démarrer le site distant. Vérifiez les paramètres de connexion.")
                                            {
                                                Owner = App.Current.MainWindow
                                            };
                                            win.ShowDialog();
                                        });
                                    }
                                    finally
                                    {
                                        // 5. On déverrouille le bouton quoi qu'il arrive (même en cas d'erreur de StartSite)
                                        StartSiteDistantEnCours = false;
                                    }
                                }
                            },
                            o =>
                            {
                                // 6. Si on est en cours de démarrage, le bouton est inactif (CanExecute = false)
                                if (StartSiteDistantEnCours)
                                    return false;

                                // Votre logique d'origine
                                return SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif && !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSitePublique.IdCompetition);
                            });
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
                _cmdArreterSiteDistant ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null && SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif)
                                {
                                    // Arrete le site concernee
                                    SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.StopSite();
                                }
                            },
                            o =>
                            {
                                return (SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null) && SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif;
                            });
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
                _cmdNettoyerSiteDistant ??= new RelayCommand(
                            async o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif)
                                {
                                    DialogParameters param = new DialogParameters
                                    {
                                        OkButtonContent = "Oui",
                                        CancelButtonContent = "Non",
                                        Content = $"Etes-vous sûr de vouloir supprimer le contenu de '{SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.RepertoireSiteFTPDistant}' sur le site distant ?",
                                        Header = "Nettoyer site distant"
                                    };

                                    ConfirmWindow win = new ConfirmWindow(param);
                                    win.ShowDialog();

                                    if (win.DialogResult.HasValue && (bool)win.DialogResult)
                                    {
                                        // Nettoyer le site distant
                                        try
                                        {
                                            // Monte le flag de nettoyage en cours
                                            NettoyageEnCours = true;

                                            // 2. Lance l'arrêt en arrière-plan et libère le thread UI pendant l'attente
                                            await Task.Run(() =>
                                            {
                                                SiteCoordinator.GestionnaireSitePublique.StartNettoyage();
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            LogTools.Logger.Error(ex, "Erreur interceptée lors du nettoyage distant.");
                                        }
                                        finally
                                        {
                                            // Nettoyage terminé
                                            NettoyageEnCours = false;
                                        }
                                    }
                                }
                            },
                            o =>
                            {
                                // Bloque le bouton si le nettoyage est en cours
                                if (NettoyageEnCours) { return false; }
                                return SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsActif && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsCleaning;
                            });
                return _cmdNettoyerSiteDistant;
            }
        }

        #region Site Publication
        private ICommand _cmdDemarrerGeneration = null;
        /// <summary>
        /// Comamnde de demarrage de la generation du site
        /// </summary>
        public ICommand CmdDemarrerGeneration
        {
            get
            {
                _cmdDemarrerGeneration ??= new RelayCommand(
                            o =>
                            {
                                SiteCoordinator.GestionnaireSitePublique.StartGeneration();
                            },
                            o =>
                            {
                                return !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSitePublique.IdCompetition) && !SiteCoordinator.GestionnaireSitePublique.IsGenerationActive;
                            });
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
                _cmdArreterGeneration ??= new RelayCommand(
                            async o =>
                            {
                                try
                                {
                                    // 1. Active le statut d'attente (sur le thread UI)
                                    BusyStatus = BusyStatusEnum.AttenteFinGeneration;
                                    IsBusy = true;

                                    // 2. Lance l'arrêt en arrière-plan et libère le thread UI pendant l'attente
                                    await Task.Run(() =>
                                    {
                                        SiteCoordinator.GestionnaireSitePublique.StopGeneration();
                                    });
                                }
                                catch (Exception ex)
                                {
                                    LogTools.Logger.Error(ex, "Erreur lors de CmdArreterGeneration");
                                }
                                finally
                                {
                                    // 3. On remet l'état d'occupation à None
                                    // Ceci s'exécute GARANTIE et AUTOMATIQUEMENT de retour sur le thread UI
                                    BusyStatus = BusyStatusEnum.None;
                                    IsBusy = false;
                                }
                            },
                            o =>
                            {
                                return SiteCoordinator.GestionnaireSitePublique.IsGenerationActive;
                            });
                return _cmdArreterGeneration;
            }
        }
        #endregion

        #region Site Interne
        private ICommand _cmdDemarrerGenerationInterne = null;
        /// <summary>
        /// Comamnde de demarrage de la generation du site Interne
        /// </summary>
        public ICommand CmdDemarrerGenerationInterne
        {
            get
            {
                _cmdDemarrerGenerationInterne ??= new RelayCommand(
                            o =>
                            {
                                SiteCoordinator.GestionnaireSiteInterne.StartGeneration();
                            },
                            o =>
                            {
                                return !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSiteInterne.IdCompetition) && !SiteCoordinator.GestionnaireSiteInterne.IsGenerationActive;
                            });
                return _cmdDemarrerGenerationInterne;
            }
        }


        private ICommand _cmdArreterGenerationInterne = null;
        /// <summary>
        /// Commande d'arret de la generation du site Interne
        /// </summary>
        public ICommand CmdArreterGenerationInterne
        {
            get
            {
                _cmdArreterGenerationInterne ??= new RelayCommand(
                            async o =>
                            {
                                try
                                {
                                    // 1. Active le statut d'attente (sur le thread UI)
                                    BusyStatus = BusyStatusEnum.AttenteFinGeneration;
                                    IsBusy = true;

                                    // 2. Lance l'arrêt en arrière-plan et libère le thread UI pendant l'attente
                                    await Task.Run(() =>
                                    {
                                        SiteCoordinator.GestionnaireSiteInterne.StopGeneration();
                                    });
                                }
                                catch (Exception ex)
                                {
                                    LogTools.Logger.Error(ex, "Erreur lors de CmdArreterGeneration");
                                }
                                finally
                                {
                                    // 3. On remet l'état d'occupation à None
                                    // Ceci s'exécute GARANTIE et AUTOMATIQUEMENT de retour sur le thread UI
                                    BusyStatus = BusyStatusEnum.None;
                                    IsBusy = false;
                                }
                            },
                            o =>
                            {
                                return SiteCoordinator.GestionnaireSiteInterne.IsGenerationActive;
                            });
                return _cmdArreterGenerationInterne;
            }
        }
        #endregion

        private ICommand _cmdAfficherSiteLocal = null;
        /// <summary>
        /// Commande d'affichage du site en local
        /// </summary>
        public ICommand CmdAfficherSiteLocal
        {
            get
            {
                _cmdAfficherSiteLocal ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsLocal && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif)
                                {
                                    string url = SiteCoordinator.GestionnaireSitePublique.URLLocalPublication;
                                    this.OpenUrlInDefaultBrowser(url);
                                }
                            },
                            o =>
                            {
                                return SiteCoordinator.GestionnaireSitePublique.SiteLocal != null && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsActif && SiteCoordinator.GestionnaireSitePublique.SiteLocal.IsLocal && !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSitePublique.IdCompetition);
                            });
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
                _cmdAfficherSiteDistant ??= new RelayCommand(
                            o =>
                            {
                                if ((SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null) && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsLocal)
                                {
                                    string url = SiteCoordinator.GestionnaireSitePublique.URLDistantPublication;

                                    this.OpenUrlInDefaultBrowser(url);
                                }
                            },
                            o =>
                            {
                                // on ne peut pas ouvrir l'URL si on n'est pas connecte a une competition
                                return (SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne != null) && !SiteCoordinator.GestionnaireSitePublique.SiteDistantSelectionne.IsLocal && !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSitePublique.IdCompetition);
                            });
                return _cmdAfficherSiteDistant;
            }
        }

        private ICommand _cmdAfficherSiteInterne = null;
        /// <summary>
        /// Commande d'affichage du site interne
        /// </summary>
        public ICommand CmdAfficherSiteInterne
        {
            get
            {
                _cmdAfficherSiteInterne ??= new RelayCommand(
                            o =>
                            {
                                if (SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsLocal && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif)
                                {
                                    string url = SiteCoordinator.GestionnaireSiteInterne.URLLocalPublication;
                                    this.OpenUrlInDefaultBrowser(url);
                                }
                            },
                            o =>
                            {
                                // on ne peut pas ouvrir l'URL si on n'est pas connecte a une competition
                                return SiteCoordinator.GestionnaireSiteInterne.SiteLocal != null && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsActif && SiteCoordinator.GestionnaireSiteInterne.SiteLocal.IsLocal && !String.IsNullOrEmpty(SiteCoordinator.GestionnaireSiteInterne.IdCompetition);
                            });
                return _cmdAfficherSiteInterne;
            }
        }



        private ICommand _cmdAfficherInformations = null;
        public ICommand CmdAfficherInformations
        {
            get
            {
                _cmdAfficherInformations ??= new RelayCommand(
                            o =>
                            {
                                if (_infoWindow == null)
                                {
                                    _infoWindow = new AppPublication.Views.Infos.InformationsView();
                                    _infoWindow?.IsTopmost = true;
                                    _infoWindow.Closed += (sender, args) => _infoWindow = null;
                                    _infoWindow.Show();
                                }
                                else
                                {
                                    _infoWindow?.IsTopmost = true;
                                    _infoWindow.Show();
                                }
                            },
                            o =>
                            {
                                return true;
                            });
                return _cmdAfficherInformations;
            }
        }

        private ICommand _cmdAfficherManuel = null;
        public ICommand CmdAfficherManuel
        {
            get
            {
                _cmdAfficherManuel ??= new RelayCommand(
                            o =>
                            {
                                if (_manuelViewer == null)
                                {
                                    // Genere un dictionnaire de ressources pour l'assembly courant
                                    AssemblyResourceDictionary appDict = new AssemblyResourceDictionary(typeof(DialogControleur).Assembly);

                                    Stream manuelStream = appDict.GetStream("AppPublication.Documentation.ManuelUtilisateur.pdf");
                                    if (manuelStream != null)
                                    {
                                        byte[] bytes = manuelStream.ReadAllBytes();
                                        // Fenetre de visualisation du manuel utilisateur (sans impression)
                                        _manuelViewer = new PdfViewer(bytes, "Manuel utilisateur", false, true);
                                        _manuelViewer.Closed += (sender, args) => _manuelViewer = null;
                                        _manuelViewer.Show();
                                        _manuelViewer.BringToFront();
                                    }
                                }
                                else
                                {
                                    _manuelViewer.BringToFront();
                                }
                            },
                            o =>
                            {
                                return true;
                            });
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
                _cmdAfficherStatistiques ??= new RelayCommand(
                            o =>
                            {
                                if (_statWindow == null)
                                {
                                    _statWindow = new AppPublication.Views.Infos.StatistiquesView(GestionStatistiques);
                                    _statWindow.Closed += (sender, args) => _statWindow = null;
                                    _statWindow.Show();
                                    _statWindow.BringToFront();
                                }
                                else
                                {
                                    if (_statWindow.WindowState == WindowState.Minimized)
                                        _statWindow.WindowState = WindowState.Normal;

                                    _statWindow.BringToFront();
                                }
                            },
                            o =>
                            {
                                return true;
                            });
                return _cmdAfficherStatistiques;
            }
        }

        private ICommand _cmdAfficherConfigurationGenerale = null;
        /// <summary>
        /// Commande d'affichage de la configuration
        /// </summary>
        public ICommand CmdAfficherConfigurationGenerale
        {
            get
            {
                _cmdAfficherConfigurationGenerale ??= new RelayCommand(
                            o =>
                            {
                                var cfgWindowGenerale = new AppPublication.Views.Configuration.ConfigurationGeneraleView(SiteCoordinator);
                                cfgWindowGenerale?.ShowDialog();
                            },
                            o =>
                            {
                                return !SiteCoordinator.IsGenerationActiveOne;
                            });
                return _cmdAfficherConfigurationGenerale;
            }
        }

        private ICommand _cmdAfficherConfigurationSite = null;
        /// <summary>
        /// Commande d'affichage de la configuration
        /// </summary>
        public ICommand CmdAfficherConfigurationSite
        {
            get
            {
                _cmdAfficherConfigurationSite ??= new RelayCommand(
                            o =>
                            {
                                var cfgWindowSite = new AppPublication.Views.Configuration.ConfigurationPublicationSiteView(SiteCoordinator);
                                cfgWindowSite?.ShowDialog();
                            },
                            o =>
                            {
                                return !SiteCoordinator.GestionnaireSitePublique.IsGenerationActive;
                            });
                return _cmdAfficherConfigurationSite;
            }
        }

        private ICommand _cmdAfficherConfigurationSiteInterne = null;
        /// <summary>
        /// Commande d'affichage de la configuration
        /// </summary>
        public ICommand CmdAfficherConfigurationSiteInterne
        {
            get
            {
                _cmdAfficherConfigurationSiteInterne ??= new RelayCommand(
                            o =>
                            {
                                var cfgWindowSiteInterne = new AppPublication.Views.Configuration.ConfigurationPublicationSiteInterneView(SiteCoordinator);
                                cfgWindowSiteInterne?.ShowDialog();
                            },
                            o =>
                            {
                                return !SiteCoordinator.GestionnaireSiteInterne.IsGenerationActive;
                            });
                return _cmdAfficherConfigurationSiteInterne;
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
                _cmdGenererTracesIncident ??= new RelayCommand(
                            o =>
                            {
                                string msg = string.Empty;

                                try
                                {
                                    // Par defaut, on va generer le fichier sur le bureau
                                    string destDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                                    string destZip = string.Format("LogAppPublication_{0:yyyyMMdd-HHmmss}.zip", DateTime.Now);

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
                IsBusy = e.IsBusy;
                if (e.IsBusy)
                {
                    BusyStatus = e.Status;
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

            if (e.CategorieDonnee == CategorieDonneesEnum.Organisation)
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

        #region METHODES PRIVEES

        /// <summary>
        /// Ouvre une URL dans le navigateur par defaut
        /// </summary>
        /// <param name="url"></param>
        private void OpenUrlInDefaultBrowser(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true // <-- Indispensable pour ouvrir une URL dans le navigateur par defaut
                });
            }
        }

        /// <summary>
        /// Extrait le mode de passe des controles passes en parametres (1er = FranceJudo, 2nd = Advanced)
        /// </summary>
        /// <param name="o"></param>
        private void ExtractPasswordFromParameters(object o)
        {
            if (o != null && o.GetType() == typeof(Tuple<object, object>))
            {
                Tuple<object, object> tuple = (Tuple<object, object>)o;
                if (tuple.Item1 != null && tuple.Item1.GetType() == typeof(RadPasswordBox))
                {
                    SiteCoordinator.GestionnaireSitePublique.SiteFranceJudo.PasswordSiteFTPDistant = Encryption.ToInsecureString(((RadPasswordBox)tuple.Item1).SecurePassword);
                }
                if (tuple.Item2 != null && tuple.Item2.GetType() == typeof(RadPasswordBox))
                {
                    SiteCoordinator.GestionnaireSitePublique.SiteDistant.PasswordSiteFTPDistant = Encryption.ToInsecureString(((RadPasswordBox)tuple.Item2).SecurePassword);
                }
            }
        }

        #endregion
    }
}
