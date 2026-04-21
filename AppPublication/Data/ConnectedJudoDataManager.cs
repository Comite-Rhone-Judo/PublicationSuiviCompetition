using AppPublication.Controles;
using AppPublication.Statistiques;
using AppPublication.Tools.Enum;
using FranceJudo.Core.Diagnostic;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.XML;
using JudoClient;
using JudoClient.Communication;
using KernelImpl;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace AppPublication.Data
{
    #region CLASSES ANNEXES
    public enum ClientJudoStatusEnum
    {
        Idle = 0,               // En attente de recevoir les informations apres initialisation - update uniquement
        Initializing = 1,       // En cours d'initialisation
        Disconnected = 2        // Deconnecte
    }

    // EventArgs pour transporter l'info
    public class BusyStatusEventArgs : EventArgs
    {
        public bool IsBusy { get; }
        public BusyStatusEnum Status { get; }

        public BusyStatusEventArgs(bool isBusy, BusyStatusEnum status)
        {
            IsBusy = isBusy;
            Status = status;
        }
    }

    // EventArgs pour transporter l'info d'une mise a jour de donnees
    public class DataUpdateEventArgs : EventArgs
    {
        public CategorieDonneesEnum CategorieDonnee { get; }

        public DataUpdateEventArgs(CategorieDonneesEnum cat)
        {
            CategorieDonnee = cat;
        }
    }


    #endregion

    public class ConnectedJudoDataManager : IJudoDataManager
    {
        #region CONSTANTES
        private const int kDefaultTimeoutMs = 15000;
        private const int kTimeoutAttenteReponseMs = 5000;
        #endregion

        #region MEMBRES
        private static ConnectedJudoDataManager _instance = null;   // Singleton
        private readonly object _lock = new object();            // Pour les verrous
        private ClientJudoStatusEnum _status = ClientJudoStatusEnum.Idle;   // Le statut du client
        readonly SingleShotTimer _timerReponse = null;     // Timer d'attente d'une reponse la reponse

        private readonly object _lockDirty = new object();  // Objet de verrouillage pour garantir l'accès exclusif
        private bool _isCombatsCacheDirty = false; // Le flag d'état
        private readonly object _repairLock = new object(); // Verrou dédié pour sérialiser les demandes de réparation (évite les requêtes multiples simultanées)
        private bool _concurrentRequestReceived = false;  // Indicateur qu'une modification (UpdateCombat ou Tapis) a été rejetée ou reçue pendant l'attente

        private JudoData _dataManager = null;
        private StatMgrDonnees _statManager = null;
        private IClientProvider _clientProvider = null;

        readonly private ConcurrentDictionary<ServerCommandEnum, EchangeMarkup> _balisesEchanges;

        #endregion

        #region EVENT HANDLER
        // Déclaration de l'événement d'un chagement de statut d'occupation
        public event EventHandler<BusyStatusEventArgs> BusyStatusChanged;

        public event EventHandler<DataUpdateEventArgs> DataUpdated;
        #endregion

        #region CONSTRUCTEUR

        private ConnectedJudoDataManager(JudoData dataMgr, StatMgrDonnees statMgr, IClientProvider provider)
        {
            InternalDataManager = dataMgr;
            StatistiquesManager = statMgr;
            ClientProvider = provider;

            _lock = new object();
            _status = ClientJudoStatusEnum.Disconnected;
            _timerReponse = new SingleShotTimer();
            _timerReponse.Elapsed += OnResponseTimeout;

            // Intialisation balises d'echange
            _balisesEchanges = new ConcurrentDictionary<ServerCommandEnum, EchangeMarkup>();
        }

        public static ConnectedJudoDataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("L'instance de GestionEvent n'a pas été initialisée.");
                }

                return _instance;
            }
        }

        public static ConnectedJudoDataManager CreateInstance(JudoData dataMgr, StatMgrDonnees statMgr, IClientProvider provider)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("L'instance de GestionEvent a déjà été initialisée.");
            }
            _instance = new ConnectedJudoDataManager(dataMgr, statMgr, provider);
            return _instance;
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Indique si les données de combats sont potentiellement incohérentes (suite à un update partiel).
        /// </summary>
        public bool IsCombatsCacheDirty
        {
            get
            {
                using (TimedLock.Lock(_lockDirty))
                {
                    return _isCombatsCacheDirty;
                }
            }
            set
            {
                using (TimedLock.Lock(_lockDirty))
                {
                    _isCombatsCacheDirty = value;
                }
            }
        }

        public int Timeout { get; set; } = kDefaultTimeoutMs;

        /// <summary>
        /// Retourne le client provider enregistre (ou une exception si non enregistre)
        /// </summary>
        public IClientProvider ClientProvider
        {
            get
            {
                if (_clientProvider == null)
                {
                    throw new InvalidOperationException("Le fournisseur de client n'a pas ete enregistre.");
                }
                return _clientProvider;
            }

            private set
            {
                _clientProvider = value ?? throw new ArgumentNullException(nameof(value), "Le fournisseur de client ne peut pas être null.");
                _clientProvider.ClientReady += OnClientReady;
                _clientProvider.ClientDisconnected += OnClientDisconnected;
            }
        }

        /// <summary>
        /// Propriété pour accéder au gestionnaire de données Judo interne
        /// </summary>
        private JudoData InternalDataManager
        {
            get
            {
                // Idéalement injecté, mais ici on garde la compatibilité avec l'existant
                if (_dataManager == null)
                {
                    throw new InvalidOperationException("Le gestionnaire de données n'a pas été initialisé.");
                }
                return _dataManager;
            }

            set
            {
                _dataManager = value ?? throw new ArgumentNullException(nameof(value), "Le gestionnaire de données ne peut pas être null.");
            }
        }

        public StatMgrDonnees StatistiquesManager
        {
            get
            {
                // Idéalement injecté, mais ici on garde la compatibilité avec l'existant
                if (_statManager == null)
                {
                    throw new InvalidOperationException("Le gestionnaire de statistiques n'a pas été initialisé.");
                }
                return _statManager;
            }

            private set
            {
                _statManager = value ?? throw new ArgumentNullException(nameof(value), "Le gestionnaire de statistiques ne peut pas être null.");
            }
        }


        #endregion

        #region EVENT

        // Signal de synchronisation : L'état initial est non-signifié (false)
        // AutoResetEvent passera automatiquement à false après qu'un thread l'ait passé
        private readonly AutoResetEvent _combatsDonneesRecuesSignal = new AutoResetEvent(false);

        /// <summary>
        /// Callback appele si le timer de reponse arrive a expiration
        /// </summary>
        /// <param name="state"></param>
        public void OnResponseTimeout(object state)
        {
            LogTools.Logger.Debug("Expiration du timer de reception de message");
            using (TimedLock.Lock(_lock))
            {
                // Demande l'arret du client si on est bien en phase d'init. Dans le cas contraire, on ignore le timer car cet evenement 
                // ne doit pas arriver si on est deja connecté ou pas encore
                if (_status == ClientJudoStatusEnum.Initializing)
                {
                    StopOnError(true, false);      // Demande l'affichage du message mais n'arrete pas le timer car on est deja dans le callback
                }
            }
        }


        /// <summary>
        /// Appelé quand un client est prêt et configuré
        /// </summary>
        private void OnClientReady(object sender, ClientReadyEventArgs e)
        {
            LogTools.Logger.Debug("Client pret, abonnement aux evenements du serveur");

            var client = e.Client;

            // NOW subscribe to all ClientJudo events (moved from GestionConnection.setClient)
            client.OnEndConnection += Client_OnEndConnection;

            client.TraitementConnexion.OnAcceptConnectionCOM += Client_OnAcceptConnectionCOM;

            client.TraitementStructure.OnListeStructures += Client_OnListeStructures;
            client.TraitementStructure.OnUpdateStructures += Client_OnUpdateStructures;

            client.TraitementCategories.OnListeCategories += Client_OnListeCategories;
            client.TraitementCategories.OnUpdateCategories += Client_OnUpdateCategories;

            client.TraitementLogos.OnListeLogos += Client_OnListeLogos;
            client.TraitementLogos.OnUpdateLogos += Client_OnUpdateLogos;

            client.TraitementOrganisation.OnListeOrganisation += Client_OnListeOrganisation;
            client.TraitementOrganisation.OnUpdateOrganisation += Client_OnUpdateOrganisation;

            client.TraitementParticipants.OnListeEquipes += Client_OnListeEquipes;
            client.TraitementParticipants.OnUpdateEquipes += Client_OnUpdateEquipes;

            client.TraitementParticipants.OnListeJudokas += Client_OnListeJudokas;
            client.TraitementParticipants.OnUpdateJudokas += Client_OnUpdateJudokas;

            client.TraitementDeroulement.OnListePhases += Client_OnListePhases;
            client.TraitementDeroulement.OnUpdatePhases += Client_OnUpdatePhases;

            client.TraitementDeroulement.OnListeCombats += Client_OnListeCombats;
            client.TraitementDeroulement.OnUpdateCombats += Client_OnUpdateCombats;
            client.TraitementDeroulement.OnUpdateTapisCombats += Client_OnUpdateTapisCombats;
            client.TraitementDeroulement.OnUpdateRencontreReceived += Client_onUpdateRencontres;

            client.TraitementArbitrage.OnListeArbitrage += Client_OnListeArbitrage;
            client.TraitementArbitrage.OnUpdateArbitrage += Client_OnUpdateArbitrage;
        }

        /// <summary>
        /// Appelé quand le client se déconnecte
        /// </summary>
        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            LogTools.Logger.Debug("Client deconnecte a {0}", e.DisconnectionTime);

            using (TimedLock.Lock(_lock))
            {
                _status = ClientJudoStatusEnum.Disconnected;
            }

            SetBusyStatus(Tools.Enum.BusyStatusEnum.None);
        }

        #endregion

        #region EVENT SERVER

        // La gestion des evenements se borne a actualiser les donnees de la competition en memoire
        // La generation du site est fait de maniere asynchrone par rapport a la reception des donnees
        /// <summary>
        /// Arret de la connexion
        /// </summary>
        /// <param name="sender"></param>
        public void Client_OnEndConnection(object sender)
        {
            LogTools.Logger.Debug("Fin de connexion");

            try
            {
                if (_clientProvider.Client == (ClientJudo)sender)
                {
                    StopClient(true);   // Il faut arreter le timer suite a la deconnexion

                    // Enregistre la deconnexion dans les stats
                    _statManager?.EnregistrerDeconnexion();
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de la gestion de la deconnexion");
            }
        }

        /// <summary>
        /// Reponse d'acception de la demande de connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void Client_OnAcceptConnectionCOM(object sender, XElement element)
        {
            LogTools.Logger.Debug("clientjudo_OnAcceptConnectionCOM");
            LogTools.DebugLogData("clientjudo_OnAcceptConnectionCOM - Reception donnees: '{0}'", element);

            using (TimedLock.Lock(_lock))
            {
                try
                {
                    // Indique que l'on va demarrer un echange pour initialiser les donnees
                    _status = ClientJudoStatusEnum.Initializing;

                    SetBusyStatus(Tools.Enum.BusyStatusEnum.DemandeDonneesStructures);

                    // Demande les structures au serveur
                    EnregistrerDebutEchange(ServerCommandEnum.DemandStructures);
                    _clientProvider.Client.DemandeStructures();

                    // Demarre le timer d'attente de la reponse a la demande
                    _timerReponse.Start(Timeout);

                    // Enregistre la connexion dans les stats
                    _statManager?.EnregistrerConnexion();
                }
                catch (Exception ex)
                {
                    // Trace l'erreur et arrete le client
                    LogTools.Logger.Error(ex, "Erreur lors de la demande d'initialisation");
                    StopOnError(true);
                }
            }
        }


        /// <summary>
        /// LEcture des donnees de structures contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesStructures(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Structures.ChargerClubs(element);
            judoDataInstance.Structures.ChargerComites(element);
            judoDataInstance.Structures.ChargerSecteurs(element);
            judoDataInstance.Structures.ChargerLigues(element);
            judoDataInstance.Structures.ChargerPays(element);
        }

        /// <summary>
        /// Reception des donnees suite a une demande de structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void Client_OnListeStructures(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeStructures");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesStructures,
                                                    elem =>
                                                    {
                                                        EnregistrerFinEchange(ServerCommandEnum.DemandStructures);
                                                        double delai = ActionWatcher.Execute(() => { LectureDonneesStructures(elem); });
                                                        NotifyDataUpdated(CategorieDonneesEnum.Structures);
                                                        // Enregistre la reception du snapshot complet dans les stats
                                                        _statManager?.EnregistrerSnapshotCompletRecu(delai);
                                                    },
                                                    BusyStatusEnum.DemandeDonneesCategories,
                                                    () =>
                                                    {
                                                        EnregistrerDebutEchange(ServerCommandEnum.DemandCategories);
                                                        _clientProvider.Client.DemandeCategories();
                                                    },
                                                    element);
        }

        /// <summary>
        /// Reception de mise a jour de donnees de structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void Client_OnUpdateStructures(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateStructures");

            UpdateRequestDispatcher(elem =>
                                        {
                                            double delai = ActionWatcher.Execute(() => { LectureDonneesStructures(elem); });
                                            _statManager?.EnregistrerSnapshotCompletRecu(delai);
                                        }
            , element);
            NotifyDataUpdated(CategorieDonneesEnum.Structures);
        }

        /// <summary>
        /// Lecture des donnees de categories contenu dans  Element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesCategories(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Categories.ChargeCategorieAges(element);
            judoDataInstance.Categories.ChargeCategoriePoids(element);
            judoDataInstance.Categories.ChargeCeintures(element);
        }

        /// <summary>
        /// Demande de liste des categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void Client_OnListeCategories(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeCategories");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesCategories,
                                         elem =>
                                        {
                                            EnregistrerFinEchange(ServerCommandEnum.DemandCategories);
                                            double delai = ActionWatcher.Execute(() => { LectureDonneesCategories(elem); });
                                            NotifyDataUpdated(CategorieDonneesEnum.Categories);
                                            // Enregistre la reception du snapshot complet dans les stats
                                            _statManager?.EnregistrerSnapshotCompletRecu(delai);
                                        },
                                        BusyStatusEnum.DemandeDonneesLogos,
                                        () =>
                                        {
                                            EnregistrerDebutEchange(ServerCommandEnum.DemandLogos);
                                            _clientProvider.Client.DemandeLogos();
                                        },
                                        element);
        }

        public void Client_OnUpdateCategories(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateCategories");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesCategories(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            }, element);

            NotifyDataUpdated(CategorieDonneesEnum.Categories);
        }

        /// <summary>
        /// Lecture des donnees Logoes contenues dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesLogos(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Logos.LectureLogos(element);
        }

        public void Client_OnListeLogos(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeLogos");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesLogos,
                            elem =>
                            {
                                EnregistrerFinEchange(ServerCommandEnum.DemandLogos);
                                double delai = ActionWatcher.Execute(() => { LectureDonneesLogos(elem); });
                                NotifyDataUpdated(CategorieDonneesEnum.Logos);
                                // Enregistre la reception du snapshot complet dans les stats
                                _statManager?.EnregistrerSnapshotCompletRecu(delai);
                            },
                            BusyStatusEnum.DemandeDonneesOrganisation,
                            () =>
                            {
                                EnregistrerDebutEchange(ServerCommandEnum.DemandOrganisation);
                                _clientProvider.Client.DemandeOrganisation();
                            },
                            element);
        }

        public void Client_OnUpdateLogos(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateLogos");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesLogos(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            }
            , element);
            NotifyDataUpdated(CategorieDonneesEnum.Logos);
        }

        /// <summary>
        /// Lecture des donnees d'organisation contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesOrganisations(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Organisation.ChargeCompetitions(element, judoDataInstance);
            judoDataInstance.Organisation.ChargeEpreuvesEquipe(element, judoDataInstance);
            judoDataInstance.Organisation.ChargeEpreuves(element, judoDataInstance);
        }

        public void Client_OnListeOrganisation(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeOrganisation");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesOrganisation,
                            elem =>
                            {
                                EnregistrerFinEchange(ServerCommandEnum.DemandOrganisation);
                                double delai = ActionWatcher.Execute(() => { LectureDonneesOrganisations(elem); });
                                NotifyDataUpdated(CategorieDonneesEnum.Organisation);
                                // Enregistre la reception du snapshot complet dans les stats
                                _statManager?.EnregistrerSnapshotCompletRecu(delai);
                            },
                            BusyStatusEnum.DemandeDonneesJudokas,
                           () =>
                           {
                               var judoDataInstance = InternalDataManager;
                               if (judoDataInstance.Organisation.Competition.IsEquipe())
                               {
                                   EnregistrerDebutEchange(ServerCommandEnum.DemandEquipes);
                                   _clientProvider.Client.DemandeEquipes();
                               }
                               else
                               {
                                   EnregistrerDebutEchange(ServerCommandEnum.DemandJudokas);
                                   _clientProvider.Client.DemandeJudokas();
                               }
                           },
                            element);
        }

        public void Client_OnUpdateOrganisation(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateOrganisation");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesOrganisations(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            }, element);
            NotifyDataUpdated(CategorieDonneesEnum.Organisation);
        }

        /// <summary>
        /// Lecture des donnees des equipes contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesEquipes(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Participants.ChargeEpreuvesJudokas(element);
            judoDataInstance.Participants.ChargeEquipes(element);
            judoDataInstance.Participants.ChargeJudokas(element, judoDataInstance);
        }

        public void Client_OnListeEquipes(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeEquipes");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesJudokas,
                            elem =>
                            {
                                EnregistrerFinEchange(ServerCommandEnum.DemandEquipes);
                                double delai = ActionWatcher.Execute(() => { LectureDonneesEquipes(elem); });
                                NotifyDataUpdated(CategorieDonneesEnum.Participants);
                                // Enregistre la reception du snapshot complet dans les stats
                                _statManager?.EnregistrerSnapshotCompletRecu(delai);
                            },
                            BusyStatusEnum.DemandeDonneesPhases,
                            () =>
                            {
                                EnregistrerDebutEchange(ServerCommandEnum.DemandPhases);
                                _clientProvider.Client.DemandePhases();
                            },
                            element);
        }


        public void Client_OnUpdateEquipes(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateEquipes");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesEquipes(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            },
            element);
            NotifyDataUpdated(CategorieDonneesEnum.Participants);
        }

        /// <summary>
        /// lecture des donnees des judokas contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesJudokas(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Participants.ChargeEpreuvesJudokas(element);
            judoDataInstance.Participants.ChargeJudokas(element, judoDataInstance);
        }

        public void Client_OnListeJudokas(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeJudokas");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesJudokas,
                            elem =>
                            {
                                EnregistrerFinEchange(ServerCommandEnum.DemandJudokas);
                                double delai = ActionWatcher.Execute(() => { LectureDonneesJudokas(elem); });
                                NotifyDataUpdated(CategorieDonneesEnum.Participants);
                                // Enregistre la reception du snapshot complet dans les stats
                                _statManager?.EnregistrerSnapshotCompletRecu(delai);
                            },
                            BusyStatusEnum.DemandeDonneesPhases,
                            () =>
                            {
                                EnregistrerDebutEchange(ServerCommandEnum.DemandPhases);
                                _clientProvider.Client.DemandePhases();
                            },
                            element);

        }

        public void Client_OnUpdateJudokas(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateJudokas");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesJudokas(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            }, element);
            NotifyDataUpdated(CategorieDonneesEnum.Participants);
        }

        /// <summary>
        /// Lecture des donnees des phases contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesPhases(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            // DC.ServerData.Deroulement.clear_deroulement();
            judoDataInstance.Deroulement.ChargePhases(element);
            judoDataInstance.Deroulement.ChargeParticipants(element);
            judoDataInstance.Deroulement.ChargeDecoupages(element);
            judoDataInstance.Deroulement.ChargePoules(element);
            judoDataInstance.Deroulement.ChargeGroupes(element, judoDataInstance);
        }

        public void Client_OnListePhases(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListePhases");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesPhases,
                            elem =>
                            {
                                EnregistrerFinEchange(ServerCommandEnum.DemandPhases);
                                double delai = ActionWatcher.Execute(() => { LectureDonneesPhases(elem); });
                                NotifyDataUpdated(CategorieDonneesEnum.Deroulement);
                                // Enregistre la reception du snapshot complet dans les stats
                                _statManager?.EnregistrerSnapshotCompletRecu(delai);
                            },
                            BusyStatusEnum.DemandeDonneesCombats,
                            () =>
                            {
                                EnregistrerDebutEchange(ServerCommandEnum.DemandCombats);
                                _clientProvider.Client.DemandeCombats();
                            },
                            element);
        }

        public void Client_OnUpdatePhases(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdatePhases");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesPhases(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            }, element);
            NotifyDataUpdated(CategorieDonneesEnum.Deroulement);
        }

        /// <summary>
        /// Lecture des donnees des combats contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesCombats(XElement element, bool isFull)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Deroulement.ChargeRencontres(element, isFull);
            judoDataInstance.Deroulement.ChargeFeuilles(element, isFull);
            judoDataInstance.Deroulement.ChargeCombats(element, judoDataInstance, isFull);
        }


        private void LectureDonneesCombatsFull(XElement element)
        {
            LectureDonneesCombats(element, true);
        }

        private void LectureDonneesCombatsDiff(XElement element)
        {
            LectureDonneesCombats(element, false);
        }

        public void Client_OnListeCombats(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeCombats");

            // Détection sécurisée de l'état Idle
            bool isIdleContext = false;
            using (TimedLock.Lock(_lock))
            {
                isIdleContext = (_status == ClientJudoStatusEnum.Idle);
            }

            if (isIdleContext)
            {
                // Etape 1 : On intègre les données (Opération lourde)
                UpdateRequestDispatcher(elem =>
                {
                    EnregistrerFinEchange(ServerCommandEnum.DemandCombats);
                    double delai = ActionWatcher.Execute(() => { LectureDonneesCombatsFull(elem); });
                    _statManager?.EnregistrerSnapshotCompletRecu(delai);
                },
                element);
                NotifyDataUpdated(CategorieDonneesEnum.Deroulement);

                // Etape 2 : Validation conditionnelle du cache
                using (TimedLock.Lock(_lockDirty))
                {
                    if (_concurrentRequestReceived)
                    {
                        // Une mise à jour a été rejetée PENDANT que nous traitions ce snapshot.
                        // Ce snapshot est valide structurellement, mais il lui manque des données récentes.
                        // ON LAISSE IsCombatsCacheDirty = true.
                        LogTools.Logger.Debug("Snapshot integre, mais des modifications concurrentes ont ete detectees. Le cache reste 'Dirty'.");
                    }
                    else
                    {
                        // Tout est calme, le snapshot est l'image exacte du serveur.
                        IsCombatsCacheDirty = false;
                        LogTools.Logger.Debug("Snapshot integre avec succes. Cache valide.");
                    }
                }

                // Etape 3 : On libère le thread qui attend dans EnsureDataConstistency
                _combatsDonneesRecuesSignal.Set();
            }
            else
            {
                // Logique d'initialisation standard (au démarrage de l'app)
                InitializationRequestDispatcher(BusyStatusEnum.InitDonneesCombats,
                                elem =>
                                {
                                    EnregistrerFinEchange(ServerCommandEnum.DemandCombats);
                                    double delai = ActionWatcher.Execute(() => { LectureDonneesCombatsFull(elem); });
                                    NotifyDataUpdated(CategorieDonneesEnum.Deroulement);
                                    // Enregistre la reception du snapshot complet dans les stats
                                    _statManager?.EnregistrerSnapshotCompletRecu(delai);
                                },
                                BusyStatusEnum.DemandeDonneesArbitres,
                                () =>
                                {
                                    EnregistrerDebutEchange(ServerCommandEnum.DemandArbitrage);
                                    _clientProvider.Client.DemandeArbitrage();
                                },
                                element);
            }
        }

        public void Client_OnUpdateTapisCombats(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateTapisCombats, invalidation du cache");

            using (TimedLock.Lock(_lockDirty))
            {
                // 1. On invalide le cache
                IsCombatsCacheDirty = true;

                // 2. On signale qu'une modification majeure a eu lieu.
                // Si un snapshot était en cours de réception, il est désormais potentiellement obsolète.
                _concurrentRequestReceived = true;
            }

            // ON NE TRAITE PAS les données pour éviter la corruption du cache
            // Enregistre la reception du snapshot complet dans les stats
            _statManager?.EnregistrerSnapshotInvalideRecu();

        }

        public void Client_OnUpdateCombats(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateCombats");

            using (TimedLock.Lock(_lockDirty))
            {
                if (IsCombatsCacheDirty)
                {
                    // SITUATION CRITIQUE : Le cache est invalide, on ne peut pas appliquer le différentiel.
                    // On rejette la donnée, MAIS on lève le drapeau.
                    // Cela forcera EnsureDataConstistency à redemander un snapshot complet qui contiendra cette donnée.
                    _concurrentRequestReceived = true;
                    _statManager.EnregistrerSnapshotIgnore();
                    LogTools.Logger.Warn("UpdateCombat ignore (Cache Dirty). Flag d'interference leve pour rechargement futur.");
                    return;
                }
            }

            // Si le cache est propre, on applique normalement
            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesCombatsDiff(elem); });
                _statManager?.EnregistrerSnapshotDifferentielRecu(delai);
            }, element);
            NotifyDataUpdated(CategorieDonneesEnum.Deroulement);
        }

        /// <summary>
        /// Lecture des donnees des rencontres contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesRencontres(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Deroulement.ChargeRencontres(element, true);
        }

        public void Client_onUpdateRencontres(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_onUpdateRencontres");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesRencontres(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            }, element);
            NotifyDataUpdated(CategorieDonneesEnum.Deroulement);
        }

        /// <summary>
        /// lecture des donnees d'arbitrage contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesArbitrage(XElement element)
        {
            var judoDataInstance = InternalDataManager;
            judoDataInstance.Arbitrage.ChargeArbitres(element);
            judoDataInstance.Arbitrage.ChargeCommissaires(element);
            judoDataInstance.Arbitrage.ChargeDelegues(element);
        }

        public void Client_OnListeArbitrage(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnListeArbitrage");

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesArbitres,
                                                   elem =>
                                                   {
                                                       EnregistrerFinEchange(ServerCommandEnum.DemandArbitrage);
                                                       double delai = ActionWatcher.Execute(() => { LectureDonneesArbitrage(elem); });
                                                       NotifyDataUpdated(CategorieDonneesEnum.Arbitrage);
                                                       // Enregistre la reception du snapshot complet dans les stats
                                                       _statManager?.EnregistrerSnapshotCompletRecu(delai);
                                                   },
                                                   BusyStatusEnum.None,
                                                   null,
                                                   element);
        }

        public void Client_OnUpdateArbitrage(object sender, XElement element)
        {
            LogTools.Logger.Debug("client_OnUpdateArbitrage");

            UpdateRequestDispatcher(elem =>
            {
                double delai = ActionWatcher.Execute(() => { LectureDonneesArbitrage(elem); });
                _statManager?.EnregistrerSnapshotCompletRecu(delai);
            }, element);
            NotifyDataUpdated(CategorieDonneesEnum.Arbitrage);
        }

        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// Nettoie les abonnements (à appeler lors de la fermeture de l'application)
        /// </summary>
        public void Cleanup()
        {
            if (_clientProvider != null)
            {
                _clientProvider.ClientReady -= OnClientReady;
                _clientProvider.ClientDisconnected -= OnClientDisconnected;
                _clientProvider = null;
            }

            // Unsubscribe from client events if still connected
            var client = _clientProvider?.Client;
            if (client != null)
            {
                UnsubscribeFromClientEvents(client);
            }
        }
        #endregion

        #region METHODES INTERNES

        /// <summary>
        /// Enregistre le debut d'un echange avec le serveur
        /// </summary>
        /// <param name="categorie"></param>
        protected void EnregistrerDebutEchange(ServerCommandEnum categorie)
        {
            try
            {
                EchangeMarkup tag = _balisesEchanges.GetOrAdd(categorie, new EchangeMarkup());
                tag.DemandeEmise();
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex, "Erreur lors de l'enregistrement du debut d'echange pour la categorie {0}", categorie.ToString());
            }
        }

        /// <summary>
        /// Enregistre la fin d'un echange avec le serveur
        /// </summary>
        /// <param name="categorie"></param>
        protected void EnregistrerFinEchange(ServerCommandEnum categorie)
        {
            try
            {
                if (_balisesEchanges.TryGetValue(categorie, out EchangeMarkup cTag))
                {
                    double? delai = cTag.ReponseRecue();
                    if (delai.HasValue)
                    {
                        _statManager.EnregistrerDelaiEchange(delai.Value);
                    }
                }
                else
                {
                    LogTools.Logger.Debug("Reponse recu pour un echange inconnu {0}", categorie.ToString());
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex, "Erreur lors de l'enregistrement de la fin d'echange pour la categorie {0}", categorie.ToString());
            }
        }

        private void StopOnError(bool withMessage = false, bool stopTimer = false)
        {
            // Arrete le client
            StopClient(stopTimer);

            // Affiche un message d'erreur a l'utilisateur
            if (withMessage)
            {
                LogTools.Alert("Une erreur est survenue lors du chargement initiale des données. Les données peuvent être incorrecte. Veuillez essayer de vous reconnecter au serveur");
            }
        }

        /// <summary>
        /// Arrete le client JudoClient
        /// </summary>
        private void StopClient(bool stopTimer = false)
        {
            // ÉTAPE 1 : Unsubscribe from client events (avoid memory leaks)
            var client = _clientProvider?.Client;
            if (client != null)
            {
                UnsubscribeFromClientEvents(client);
            }

            // ÉTAPE 2 : Demander à GestionConnection de fermer proprement le client
            // (GestionConnection gère le lifecycle complet)
            _clientProvider?.DisposeClient(); // This will trigger ClientDisconnected event

            using (TimedLock.Lock(_lock))
            {
                _status = ClientJudoStatusEnum.Disconnected;
                if (stopTimer)
                {
                    _timerReponse?.Stop();                      // Si appelez depuis le callback, on est dans un deadlock
                }
            }

            SetBusyStatus(Tools.Enum.BusyStatusEnum.None);
        }

        /// <summary>
        /// Désabonne tous les événements du client (évite les fuites mémoire)
        /// </summary>
        private void UnsubscribeFromClientEvents(ClientJudo client)
        {
            try
            {
                client.OnEndConnection -= Client_OnEndConnection;
                client.TraitementConnexion.OnAcceptConnectionCOM -= Client_OnAcceptConnectionCOM;
                client.TraitementStructure.OnListeStructures -= Client_OnListeStructures;
                client.TraitementStructure.OnUpdateStructures -= Client_OnUpdateStructures;
                client.TraitementCategories.OnListeCategories -= Client_OnListeCategories;
                client.TraitementCategories.OnUpdateCategories -= Client_OnUpdateCategories;
                client.TraitementLogos.OnListeLogos -= Client_OnListeLogos;
                client.TraitementLogos.OnUpdateLogos -= Client_OnUpdateLogos;
                client.TraitementOrganisation.OnListeOrganisation -= Client_OnListeOrganisation;
                client.TraitementOrganisation.OnUpdateOrganisation -= Client_OnUpdateOrganisation;
                client.TraitementParticipants.OnListeEquipes -= Client_OnListeEquipes;
                client.TraitementParticipants.OnUpdateEquipes -= Client_OnUpdateEquipes;
                client.TraitementParticipants.OnListeJudokas -= Client_OnListeJudokas;
                client.TraitementParticipants.OnUpdateJudokas -= Client_OnUpdateJudokas;
                client.TraitementDeroulement.OnListePhases -= Client_OnListePhases;
                client.TraitementDeroulement.OnUpdatePhases -= Client_OnUpdatePhases;
                client.TraitementDeroulement.OnListeCombats -= Client_OnListeCombats;
                client.TraitementDeroulement.OnUpdateCombats -= Client_OnUpdateCombats;
                client.TraitementDeroulement.OnUpdateTapisCombats -= Client_OnUpdateTapisCombats;
                client.TraitementDeroulement.OnUpdateRencontreReceived -= Client_onUpdateRencontres;
                client.TraitementArbitrage.OnListeArbitrage -= Client_OnListeArbitrage;
                client.TraitementArbitrage.OnUpdateArbitrage -= Client_OnUpdateArbitrage;
            }
            catch (Exception ex)
            {
                // Log but don't throw - we're in cleanup mode
                LogTools.Logger.Warn(ex, "Erreur lors du desabonnement des evenements client");
            }
        }

        /// <summary>
        /// Positionne le status d'occupation du gestionnaire de connexion
        /// </summary>
        /// <param name="status"></param>
        private void SetBusyStatus(Tools.Enum.BusyStatusEnum status)
        {
            bool isBusy = status != BusyStatusEnum.None;

            // On déclenche l'événement. Le ?.Invoke permet de ne rien faire si personne n'écoute.
            BusyStatusChanged?.Invoke(this, new BusyStatusEventArgs(isBusy, status));
        }

        /// <summary>
        /// NOtifie d'une mise a jour de donnees
        /// </summary>
        /// <param name="datCat"></param>
        private void NotifyDataUpdated(CategorieDonneesEnum datCat)
        {
            DataUpdated?.Invoke(this, new DataUpdateEventArgs(datCat));
        }

        /// <summary>
        /// Dispatcher general pour traiter les demandes d'initialisation
        /// </summary>
        /// <param name="currentStatus">Etat a la reception de la donnees</param>
        /// <param name="nextStatus">Etat suivante</param>
        /// <param name="dataAction">Action pour traiter les donnees</param>
        /// <param name="nextAction">Action pour la demande suivante</param>
        /// <param name="element">Les donnees recues</param>
        private void InitializationRequestDispatcher(BusyStatusEnum currentStatus, Action<XElement> dataAction, BusyStatusEnum nextStatus, Action nextAction, XElement element)
        {
            LogTools.Logger.Debug("Traitement Request initialisation");
            LogTools.DebugLogData("Traitement Request initialisation: '{0}'", element);

            using (TimedLock.Lock(_lock))
            {
                // Arrete le timer de reponse
                _timerReponse.Stop();

                // Verifie si on est bien entrain d'initialiser les donnees
                if (_status != ClientJudoStatusEnum.Initializing)
                {
                    return;
                }

                try
                {
                    // Positionne le status courant pour le binding (thread safe)
                    SetBusyStatus(currentStatus);

                    // Met a jour l'heure des donnees
                    XDocument doc = new XDocument();
                    doc.Add(element);
                    doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

                    // Appelle  le callback pour traiter les donnees via le contexte securise
                    if (InternalDataManager != null)
                    {
                        InternalDataManager.RunSafeDataUpdate(() =>
                        {
                            dataAction?.Invoke(element);
                        });
                    }
                    else
                    {
                        // Fallback si pas de manager (ne devrait pas arriver)
                        dataAction?.Invoke(element);
                    }

                    // Effectue la demande suivante si necessaire
                    if (nextStatus != BusyStatusEnum.None && null != nextAction)
                    {
                        SetBusyStatus(nextStatus);

                        // appel le callback suivant
                        nextAction?.Invoke();

                        // Demarre le timer d'attente de la reponse a la demande
                        _timerReponse.Start(Timeout);
                    }
                    else
                    {
                        // Fin de l'initialisation
                        SetBusyStatus(Tools.Enum.BusyStatusEnum.None);
                        _status = ClientJudoStatusEnum.Idle;
                    }
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur lors de la lecture des donnees recues");
                    StopOnError(true);      // Arrete le gestionnaire d'evenement sur une erreur
                }
            }
        }

        private void UpdateRequestDispatcher(Action<XElement> action, XElement element)
        {
            LogTools.Logger.Debug("Traitement request update");
            LogTools.DebugLogData("Traitement request update: '{0}'", element);

            // Verifie l'etat du gestionnaire (on ne peut pas recevoir ces donnees pendant une initialisation)
            using (TimedLock.Lock(_lock))
            {
                if (_status != ClientJudoStatusEnum.Idle)
                {
                    return;
                }
            }

            try
            {
                XDocument doc = new XDocument();
                doc.Add(element);
                doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

                // Appel la methode de traitement via le callback securise
                if (InternalDataManager != null)
                {
                    InternalDataManager.RunSafeDataUpdate(() =>
                    {
                        action?.Invoke(element);
                    });
                }
                else
                {
                    // Fallback si pas de manager (ne devrait pas arriver)
                    action?.Invoke(element);
                }

            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de la mise a jour des donnees");
            }
        }

        #endregion

        #region IMPLEMENTATION IJudoDataManager


        public IJudoData Data
        {
            get
            {
                if (InternalDataManager != null)
                {
                    return InternalDataManager.Data;
                }
                return null;
            }
        }

        /// <summary>
        /// Retourne un snapshot complet des données Judo
        /// </summary>
        /// <returns></returns>
        public IJudoData Snapshot
        {
            get
            {
                if (InternalDataManager != null)
                {
                    return InternalDataManager.Snapshot;
                }
                return null;
            }
        }

        /// <summary>
        /// Vérifie l'intégrité du cache des combats. Si le cache est marqué "Sale" (Dirty),
        /// cette méthode déclenche une mise à jour complète et bloque l'exécution jusqu'à réception
        /// des données ou expiration du délai.
        /// </summary>
        /// <returns>True si les données sont garanties intègres, False si la récupération a échoué (Timeout ou déconnexion).</returns>
        public bool EnsureDataConsistency()
        {
            // Verrou de réparation : Un seul thread peut piloter la réparation à la fois.
            using (TimedLock.Lock(_repairLock))
            {
                // Lecture thread-safe de la propriété (qui utilise _lockDirty en interne)
                if (!IsCombatsCacheDirty) return true;

                LogTools.Logger.Debug("Cache Dirty detecte. Demarrage de la procedure de reparation...");

                var client = _clientProvider.Client;
                if (client != null)
                {
                    // 1. Reset du signal d'attente
                    _combatsDonneesRecuesSignal.Reset();

                    // 2. Reset du détecteur d'interférence (On commence une nouvelle tentative propre)
                    using (TimedLock.Lock(_lockDirty))
                    {
                        _concurrentRequestReceived = false;
                    }

                    // Enregistrement de la demande dans les stats
                    _statManager?.EnregistrerDemandeSnapshot();

                    // 3. Envoi de la demande (Asynchrone)
                    client.DemandeCombats();

                    // 4. Attente bloquante de la réponse
                    bool received = _combatsDonneesRecuesSignal.WaitOne(kTimeoutAttenteReponseMs);

                    if (!received)
                    {
                        LogTools.Logger.Error("Timeout en attendant le snapshot complet.");
                        return false;
                    }

                    // 5. Vérification finale
                    // Le snapshot est arrivé et a été traité par client_OnListeCombats.
                    // Si une interférence a eu lieu pendant le transfert, IsCombatsCacheDirty sera encore à true.
                    bool isClean;
                    using (TimedLock.Lock(_lockDirty)) { isClean = !IsCombatsCacheDirty; }

                    if (!isClean)
                    {
                        LogTools.Logger.Debug("Le snapshot a ete reçu mais des donnees concurrentes ont empeche la validation du cache.");
                        // On retourne TRUE car on a chargé un snapshot valide (le "moins pire" disponible).
                        // La génération se fera, et au prochain cycle, EnsureDataConstistency verra que c'est toujours Dirty et recommencera.
                        return true;
                    }

                    return true; // Cache propre et validé
                }

                return false; // Pas de client connecté
            }
        }

        /// <summary>
        /// Execute une mise a jour des donnees de maniere securisee
        /// </summary>
        /// <param name="actionMiseAJour"></param>
        public void RunSafeDataUpdate(Action actionMiseAJour)
        {
            InternalDataManager?.RunSafeDataUpdate(actionMiseAJour);
        }

        #endregion
    }
}

