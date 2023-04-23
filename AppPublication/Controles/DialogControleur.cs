using KernelImpl;
using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Tools.Outils;
using Tools.Windows;
using AppPublication.Tools.Enum;
using Telerik.Windows.Controls;

namespace AppPublication.Controles
{
    /// <summary>
    /// Cette classe joue le role de coordinateur et de View-Model. Il regroupe les commandes et les objets metier
    /// </summary>
    public class DialogControleur : NotificationBase
    {
        #region MEMBRES
        private static DialogControleur _currentControleur = null;      // instance singletion
        #endregion

        #region CONSTRUCTEUR

        private DialogControleur()
        {
            FileAndDirectTools.InitDataDirectories();
            LogTools.HeaderType = "PC PUBLICATION";

            InitControleur();
        }

        #endregion

        #region PROPRIETES

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
                NotifyPropertyChanged("BusyStatus");
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
                NotifyPropertyChanged("IsBusy");
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
                    LogTools.Log(ex);
                }
            }));
        }

        #endregion

        #region COMMANDES
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
                                if (Instance.GestionSite.MiniSiteLocal != null && !Instance.GestionSite.MiniSiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.MiniSiteLocal.StartSite();
                                }
                            },
                            o =>
                            {
                                return true;
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
                                if (Instance.GestionSite.MiniSiteLocal != null && Instance.GestionSite.MiniSiteLocal.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.MiniSiteLocal.StopSite();
                                }
                            },
                            o =>
                            {
                                return Instance.GestionSite.MiniSiteLocal.IsActif;
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
                                if (Instance.GestionSite.MiniSiteDistant != null && !Instance.GestionSite.MiniSiteDistant.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.MiniSiteDistant.StartSite();
                                }
                            },
                            o =>
                            {
                                return true;
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
                                if (Instance.GestionSite.MiniSiteDistant != null && Instance.GestionSite.MiniSiteDistant.IsActif)
                                {
                                    // Demarre le site en local
                                    Instance.GestionSite.MiniSiteDistant.StopSite();
                                }
                            },
                            o =>
                            {
                                return Instance.GestionSite.MiniSiteDistant.IsActif;
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
                                if (Instance.GestionSite.MiniSiteDistant != null && !Instance.GestionSite.MiniSiteDistant.IsActif)
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
                                        Instance.GestionSite.MiniSiteDistant.NettoyerSite();
                                    }
                                }
                            },
                            o =>
                            {
                                return !Instance.GestionSite.MiniSiteDistant.IsActif;
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
                                return true;
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
                                (new AppPublication.IHM.Commissaire.Statistiques(GestionStatistiques)).Show();
                            },
                            o =>
                            {
                                return true;
                            });
                }
                return _cmdAfficherStatistiques;
            }
        }

        
        #endregion
    }
}
