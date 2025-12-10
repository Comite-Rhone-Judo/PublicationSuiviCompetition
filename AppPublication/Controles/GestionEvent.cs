using AppPublication.Tools;
using AppPublication.Tools.Enum;
using JudoClient;
using JudoClient.Communication;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using Telerik.Windows.Controls;
using Tools.Enum;
using Tools.Outils;
using KernelImpl;

namespace AppPublication.Controles
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
#endregion

    public class GestionEvent
    {
        #region CONSTANTES
        private const int kDefaultTimeoutMs = 15000;
        #endregion

        #region MEMBRES
        private static GestionEvent _instance = null;   // Singleton
        private object _lock = new object();            // Pour les verrous
        private ClientJudoStatusEnum _status = ClientJudoStatusEnum.Idle;   // Le statut du client
        SingleShotTimer _timerReponse = null;     // Timer d'attente d'une reponse la reponse

        private readonly object _lockDirty = new object();  // Objet de verrouillage pour garantir l'accès exclusif
        private bool _isCombatsCacheDirty = false; // Le flag d'état

        private IJudoDataManager _dataManager = null;

        #endregion

        #region EVENT HANDLER
        // Déclaration de l'événement
        public event EventHandler<BusyStatusEventArgs> BusyStatusChanged;
        #endregion

        #region CONSTRUCTEUR

        private GestionEvent()
        {
            _lock = new object();
            _status = ClientJudoStatusEnum.Disconnected;
            _timerReponse = new SingleShotTimer();
            _timerReponse.Elapsed += OnResponseTimeout;
        }

        public static GestionEvent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestionEvent();
                }
                return _instance;
            }
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Indique si les données de combats sont potentiellement incohérentes (suite à un update partiel).
        /// </summary>
        public bool IsCombatsCacheDirty
        {
            get { lock (_lockDirty) return _isCombatsCacheDirty; }
            set { lock (_lockDirty) _isCombatsCacheDirty = value; }
        }

        public int Timeout { get; set; } = kDefaultTimeoutMs;

        private IJudoDataManager DataManager
        {
            get
            {
                // Idéalement injecté, mais ici on garde la compatibilité avec l'existant
                if (_dataManager == null)
                {
                    _dataManager = DialogControleur.Instance.ServerData as IJudoDataManager;
                }
                return _dataManager;
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
            lock (_lock)
            {
                // Demande l'arret du client si on est bien en phase d'init. Dans le cas contraire, on ignore le timer car cet evenement 
                // ne doit pas arriver si on est deja connecté ou pas encore
                if (_status == ClientJudoStatusEnum.Initializing)
                {
                    StopOnError(true, false);      // Demande l'affichage du message mais n'arrete pas le timer car on est deja dans le callback
                }
            }
        }

        #endregion

        #region EVENT SERVER

        // La gestion des evenements se borne a actualiser les donnees de la competition en memoire
        // La generation du site est fait de maniere asynchrone par rapport a la reception des donnees
        /// <summary>
        /// Arret de la connexion
        /// </summary>
        /// <param name="sender"></param>
        public void client_OnEndConnection(object sender)
        {
            if (DialogControleur.Instance.Connection.Client == (ClientJudo)sender)
            {
                StopClient(true);   // Il faut arreter le timer suite a la deconnexion
            }
        }

        /// <summary>
        /// Reponse d'acception de la demande de connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void clientjudo_OnAcceptConnectionCOM(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("Reception donnees: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            lock (_lock)
            {
                try
                {
                    // Indique que l'on va demarrer un echange pour initialiser les donnees
                    _status = ClientJudoStatusEnum.Initializing;

                    SetBusyStatus(Tools.Enum.BusyStatusEnum.DemandeDonneesStructures);

                    // Demande les structures au serveur
                    DialogControleur.Instance.Connection.Client.DemandeStructures();

                    // Demarre le timer d'attente de la reponse a la demande
                    _timerReponse.Start(Timeout);
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
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Structures.lecture_clubs(element);
            judoDataInstance.Structures.lecture_comites(element);
            judoDataInstance.Structures.lecture_secteurs(element);
            judoDataInstance.Structures.lecture_ligues(element);
            judoDataInstance.Structures.lecture_pays(element);
        }

        /// <summary>
        /// Reception des donnees suite a une demande de structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void client_OnListeStructures(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeStructures: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesStructures,
                                                    LectureDonneesStructures,
                                                    BusyStatusEnum.DemandeDonneesCategories,
                                                    DialogControleur.Instance.Connection.Client.DemandeCategories,
                                                    element);
        }

        /// <summary>
        /// Reception de mise a jour de donnees de structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void client_OnUpdateStructures(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateStructures: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher(LectureDonneesStructures, element);
        }

        /// <summary>
        /// Lecture des donnees de categories contenu dans  Element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesCategories(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Categories.lecture_cateages(element);
            judoDataInstance.Categories.lecture_catepoids(element);
            judoDataInstance.Categories.lecture_ceintures(element);
        }

        /// <summary>
        /// Demande de liste des categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void client_OnListeCategories(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeCategories: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesCategories,
                                        LectureDonneesCategories,
                                        BusyStatusEnum.DemandeDonneesLogos,
                                        DialogControleur.Instance.Connection.Client.DemandeLogos,
                                        element);
        }

        public void client_OnUpdateCategories(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateCategories: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher(LectureDonneesCategories, element);
        }

        /// <summary>
        /// Lecture des donnees Logoes contenues dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesLogos(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Logos.lecture_logos(element);
        }

        public void client_OnListeLogos(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeLogos: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesLogos,
                            LectureDonneesLogos,
                            BusyStatusEnum.DemandeDonneesOrganisation,
                            DialogControleur.Instance.Connection.Client.DemandeOrganisation,
                            element);
        }

        public void client_OnUpdateLogos(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateLogos: '{0}'", element.ToString(SaveOptions.DisableFormatting));
            
            UpdateRequestDispatcher(LectureDonneesLogos, element);
        }

        /// <summary>
        /// Lecture des donnees d'organisation contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesOrganisations(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Organisation.lecture_competitions(element, judoDataInstance);
            judoDataInstance.Organisation.lecture_epreuves_equipe(element, judoDataInstance);
            judoDataInstance.Organisation.lecture_epreuves(element, judoDataInstance);
        }

        public void client_OnListeOrganisation(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeOrganisation: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesOrganisation,
                            LectureDonneesOrganisations,
                            BusyStatusEnum.DemandeDonneesJudokas,
                           () =>
                           {
                               var judoDataInstance = DataManager as JudoData;
                               if (judoDataInstance.Competition.IsEquipe())
                               {
                                   DialogControleur.Instance.Connection.Client.DemandeEquipes();
                               }
                               else
                               {
                                   DialogControleur.Instance.Connection.Client.DemandeJudokas();
                               }
                           },
                            element);
        }

        public void client_OnUpdateOrganisation(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateOrganisation: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher( (XElement elem) =>
            {
                LectureDonneesOrganisations(elem);
                // Signale la mise a jour de la competition
                DialogControleur.Instance.UpdateCompetition();
            },
            element);
        }

        /// <summary>
        /// Lecture des donnees des equipes contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesEquipes(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Participants.lecture_epreuves_judokas(element, judoDataInstance);
            judoDataInstance.Participants.lecture_equipes(element);
            judoDataInstance.Participants.lecture_judokas(element, judoDataInstance);
        }

        public void client_OnListeEquipes(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeEquipes: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesJudokas,
                            LectureDonneesEquipes,
                            BusyStatusEnum.DemandeDonneesPhases,
                            DialogControleur.Instance.Connection.Client.DemandePhases,
                            element);
        }


        public void client_OnUpdateEquipes(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateEquipes: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher(LectureDonneesEquipes, element);
        }

        /// <summary>
        /// lecture des donnees des judokas contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesJudokas(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Participants.lecture_epreuves_judokas(element, judoDataInstance);
            judoDataInstance.Participants.lecture_judokas(element, judoDataInstance);
        }

        public void client_OnListeJudokas(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeJudokas: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesJudokas,
                            LectureDonneesJudokas,
                            BusyStatusEnum.DemandeDonneesPhases,
                            DialogControleur.Instance.Connection.Client.DemandePhases,
                            element);

        }

        public void client_OnUpdateJudokas(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateJudokas: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher(LectureDonneesJudokas, element);
        }

        /// <summary>
        /// Lecture des donnees des phases contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesPhases(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            // DC.ServerData.Deroulement.clear_deroulement();
            judoDataInstance.Deroulement.lecture_phases(element);
            judoDataInstance.Deroulement.lecture_participants(element);
            judoDataInstance.Deroulement.lecture_decoupages(element);
            judoDataInstance.Deroulement.lecture_poules(element);
            judoDataInstance.Deroulement.lecture_groupes(element, judoDataInstance);
        }

        public void client_OnListePhases(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListePhases: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesPhases,
                            LectureDonneesPhases,
                            BusyStatusEnum.DemandeDonneesCombats,
                            DialogControleur.Instance.Connection.Client.DemandeCombats,
                            element);
        }

        public void client_OnUpdatePhases(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdatePhases: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher(LectureDonneesPhases, element);
        }

        /// <summary>
        /// Lecture des donnees des combats contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesCombats(XElement element, bool isFull)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Deroulement.lecture_rencontres(element, isFull);
            judoDataInstance.Deroulement.lecture_feuilles(element, isFull);
            judoDataInstance.Deroulement.lecture_combats(element, judoDataInstance, isFull);
        }


        private void LectureDonneesCombatsFull(XElement element)
        {
            LectureDonneesCombats(element, true);
        }

        private void LectureDonneesCombatsDiff(XElement element)
        {
            LectureDonneesCombats(element, false);
        }

        public void client_OnListeCombats(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeCombats: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            bool isIdle = false;
            lock(_lock)
            {
                switch (_status)
                {
                    case ClientJudoStatusEnum.Idle:
                        {
                            isIdle = true;
                            break;
                        }
                    case ClientJudoStatusEnum.Initializing:
                        {
                            isIdle = false;
                            break;
                        }
                    case ClientJudoStatusEnum.Disconnected:
                    default:
                        {
                            LogTools.Logger.Warn("Reception d'un snapshot de combats en dehors des contextes autorises");
                            return;
                        }
                }
            }

            if (isIdle)
            {
                // Cas d'usage suite a une demande d'avoir le snapshot complet des combats
                UpdateRequestDispatcher(LectureDonneesCombatsFull, element);

                // On signale que les données sont propres et reçues
                IsCombatsCacheDirty = false;
                _combatsDonneesRecuesSignal.Set();
            }
            else
            {
                // Cas d'usage pendant l'initialisation
                InitializationRequestDispatcher(BusyStatusEnum.InitDonneesCombats,
                                LectureDonneesCombatsFull,
                                BusyStatusEnum.DemandeDonneesArbitres,
                                DialogControleur.Instance.Connection.Client.DemandeArbitrage,
                                element);
            }
        }

        /// <summary>
        /// Vérifie l'intégrité du cache des combats. Si le cache est marqué "Sale" (Dirty),
        /// cette méthode déclenche une mise à jour complète et bloque l'exécution jusqu'à réception
        /// des données ou expiration du délai.
        /// </summary>
        /// <returns>True si les données sont garanties intègres, False si la récupération a échoué (Timeout ou déconnexion).</returns>
        public bool EnsureDataConstistency()
        {
            // 1. Si les données sont déjà propres, on passe tout de suite
            if (!IsCombatsCacheDirty)
            {
                return true;
            }

            // 2. Si les données sont sales, on tente une réparation
            var client = DialogControleur.Instance.Connection.Client;
            if (client != null)
            {
                // Reset du signal avant la demande
                _combatsDonneesRecuesSignal.Reset();

                // On demande le snapshot complet (Type C1 sûr)
                LogTools.Logger.Debug("Incohérence détectée (Dirty Cache), demande de snapshot complet..."); 
                client.DemandeCombats();

                // On attend que client_OnListeCombats reçoive la réponse et débloque le signal
                bool success = _combatsDonneesRecuesSignal.WaitOne(kDefaultTimeoutMs);

                if (!success)
                {
                    LogTools.Logger.Warn("Timeout lors de la sécurisation des données combats. Les donnees n'ont pas ete actualisees");
                }

                return success;
            }

            return false; // Pas de client connecté pour réparer
        }

        public void client_OnUpdateTapisCombats(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateTapisCombats: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            // MODIFICATION CRITIQUE : 
            // On intercepte UpdateTapisCombats qui est ambigu (C1 vs C2).
            // On marque le cache comme sale pour forcer un rechargement complet avant la prochaine génération.
            IsCombatsCacheDirty = true;

            // ON NE TRAITE PAS les données pour éviter la corruption du cache
        }

        public void client_OnUpdateCombats(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateCombats: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            // Si le cache est sale, on ignore strictement les mises à jour partielles
            // car elles pourraient corrompre davantage la mémoire ou crasher.
            if (IsCombatsCacheDirty)
            {
                LogTools.Logger.Warn("Update partiel ignoré car le cache est invalide (Dirty).");
                return;
            }

            UpdateRequestDispatcher(LectureDonneesCombatsDiff, element);
        }

        /// <summary>
        /// Lecture des donnees des rencontres contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesRencontres(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Deroulement.lecture_rencontres(element, true);
        }

        public void client_onUpdateRencontres(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_onUpdateRencontres: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher(LectureDonneesRencontres, element);
        }

        /// <summary>
        /// lecture des donnees d'arbitrage contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesArbitrage(XElement element)
        {
            var judoDataInstance = DataManager as JudoData;
            judoDataInstance.Arbitrage.lecture_arbitres(element);
            judoDataInstance.Arbitrage.lecture_commissaires(element);
            judoDataInstance.Arbitrage.lecture_delegues(element);
        }

        public void client_OnListeArbitrage(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnListeArbitrage: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesArbitres,
                                                   (XElement elem) =>
                                                   {
                                                       LectureDonneesArbitrage(elem);
                                                       // Signale la mise a jour de la competition a la fin du processus
                                                       DialogControleur.Instance.UpdateCompetition();
                                                   },
                                                   BusyStatusEnum.None,
                                                   null,
                                                   element);
        }

        public void client_OnUpdateArbitrage(object sender, XElement element)
        {
            LogTools.DataLogger.Debug("client_OnUpdateArbitrage: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            UpdateRequestDispatcher(LectureDonneesArbitrage, element);
        }

        #endregion

        #region METHODES INTERNES

        private void StopOnError(bool withMessage = false, bool stopTimer = false)
        {
            // Arrete le client
            StopClient(stopTimer);

            // Affiche un message d'erreur a l'utilisateur
            if (withMessage)
            {
                LogTools.Alert("Une erreur est survenue lors du chargement initiale des données. Les données peuvent être incorrecte. Veuillez essayer de vous reconnecter au serveur", "Initialisation");
            }
        }

        /// <summary>
        /// Arrete le client JudoClient
        /// </summary>
        private void StopClient(bool stopTimer = false)
        {
            DialogControleur.Instance.Connection.Client.Client.Stop();
            DialogControleur.Instance.Connection.Client = null;


            lock (_lock)
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
        /// Positionne le status d'occupation du gestionnaire de connexion
        /// </summary>
        /// <param name="status"></param>
        private void SetBusyStatus(Tools.Enum.BusyStatusEnum status)
        {
            /*
            bool isb = false;
            switch (e.Status)
            {
                case Tools.Enum.BusyStatusEnum.InitDonneesStructures:
                case Tools.Enum.BusyStatusEnum.InitDonneesCategories:
                case Tools.Enum.BusyStatusEnum.InitDonneesLogos:
                case Tools.Enum.BusyStatusEnum.InitDonneesJudokas:
                case Tools.Enum.BusyStatusEnum.InitDonneesOrganisation:
                case Tools.Enum.BusyStatusEnum.InitDonneesPhases:
                case Tools.Enum.BusyStatusEnum.InitDonneesCombats:
                case Tools.Enum.BusyStatusEnum.InitDonneesArbitres:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesStructures:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesCategories:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesLogos:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesJudokas:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesOrganisation:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesPhases:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesCombats:
                case Tools.Enum.BusyStatusEnum.DemandeDonneesArbitres:
                    {
                        isb = true;
                        break;
                    }
                case Tools.Enum.BusyStatusEnum.None:
                default:
                    {
                        isb = false;
                        break;
                    }
            }
            */

            bool isBusy = status != BusyStatusEnum.None;

            // On déclenche l'événement. Le ?.Invoke permet de ne rien faire si personne n'écoute.
            BusyStatusChanged?.Invoke(this, new BusyStatusEventArgs(isBusy, status));
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
            lock (_lock)
            {
                LogTools.DataLogger.Debug("Reception donnees: '{0}'", element.ToString(SaveOptions.DisableFormatting));

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
                    if (DataManager != null)
                    {
                        DataManager.RunSafeDataUpdate(() =>
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
            LogTools.DataLogger.Debug("Reception donnees: '{0}'", element.ToString(SaveOptions.DisableFormatting));

            // Verifie l'etat du gestionnaire (on ne peut pas recevoir ces donnees pendant une initialisation)
            lock (_lock)
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
                if (DataManager != null)
                {
                    DataManager.RunSafeDataUpdate(() =>
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
    }
}

