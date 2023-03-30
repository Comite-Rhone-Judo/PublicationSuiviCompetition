using KernelImpl;
using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Tools.Outils;
using AppPublication.Tools.Enum;

namespace AppPublication.Controles
{
    /// <summary>
    /// Cette classe joue le role de coordinateur et de View-Model. Il regroupe les commandes et les objets metier
    /// </summary>
    public class DialogControleur : NotificationBase
    {
        #region MEMBRES

        private static DialogControleur _currentControleur = null;
        //private static ICommissaireWindow windows_main;

        #endregion

        #region CONSTRUCTEUR

        public DialogControleur()
        {
            FileAndDirectTools.InitDataDirectories();
            LogTools.HeaderType = "PC COM/PRINT";

            InitControleur();
        }

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

        #endregion

        #region PROPRIETES

        private BusyStatusEnum _busyStatus = BusyStatusEnum.InitDonneesNone;
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
        public GestionConnection Connection
        {
            get {
                return _connection;
            }
        }
        

        private GestionSite _site = null;
        public GestionSite GestionSite
        {
            get { return _site; }
            set { _site = value; }
        }

        private JudoData _serverData;
        public JudoData ServerData
        {
            get {
                if (_serverData == null)
                {
                    _serverData = KernelManager.Manager.manager.m_JudoData;
                }
                return _serverData;
            }
        }

        bool _isBusy;
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

        private void InitControleur()
        {
            Application.Current.ExecOnUiThread(new Action(() =>
            {
                try
                {
                    _connection = new GestionConnection();
                    _site = new GestionSite();
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
                                    // TODO A voir si la notion de demarrer ou non n'est pas gerer plutot dans Gestion site directment
                                    // TODO C'est lui qui va gere le thread de generation

                                    Instance.GestionSite.MiniSiteLocal.StartSite();                                }
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
                                    // TODO A voir si la notion de demarrer ou non n'est pas gerer plutot dans Gestion site directment
                                    // TODO C'est lui qui va gere le thread de generation
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
                                    // TODO A voir si la notion de demarrer ou non n'est pas gerer plutot dans Gestion site directment
                                    // TODO C'est lui qui va gere le thread de generation
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
                                    // TODO A voir si la notion de demarrer ou non n'est pas gerer plutot dans Gestion site directment
                                    // TODO C'est lui qui va gere le thread de generation
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

        private ICommand _cmdDemarrerGeneration = null;
        public ICommand CmdDemarrerGeneration
        {
            get
            {
                if (_cmdDemarrerGeneration == null)
                {
                    _cmdDemarrerGeneration = new RelayCommand(
                            o =>
                            {
                                // TODO A implementer
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
        public ICommand CmdArreterGeneration
        {
            get
            {
                if (_cmdArreterGeneration == null)
                {
                    _cmdArreterGeneration = new RelayCommand(
                            o =>
                            {
                                // TODO A implementer
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
        #endregion
    }
}
