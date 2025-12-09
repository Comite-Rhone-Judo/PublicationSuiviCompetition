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
using Tools.Enum;
using Tools.Outils;

namespace AppPublication.Controles
{
    public enum ClientJudoStatusEnum
    {
        Idle = 0,               // En attente de recevoir les informations apres initialisation - update uniquement
        Initializing = 1,       // En cours d'initialisation
        Disconnected = 2        // Deconnecte
    }

    public class GestionEvent
    {
        // TODO Revoir la gestion car certains envois de donnees ne se font pas par snapshot (ex: update des combats)
        // TODO Analyse le comportement de Project_TAS/AppCommissaire/Controles/GestionEvent.cs par rapport aux mise à jour et/ou update complet

        #region CONSTANTES
        private const int kDefaultTimeoutMs = 15000;
        #endregion

        #region MEMBRES
        private static GestionEvent _instance = null;   // Singleton
        private object _lock = new object();            // Pour les verrous
        private ClientJudoStatusEnum _status = ClientJudoStatusEnum.Idle;   // Le statut du client
        SingleShotTimer _timerReponse = null;     // Timer d'attente d'une reponse la reponse

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
        public int Timeout { get; set; } = kDefaultTimeoutMs;
        #endregion

        #region EVENT

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
            LogTools.Logger.Debug("Reception donnees: '{0}'", element.ToString(SaveOptions.DisableFormatting));

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
            DialogControleur.Instance.ServerData.Structures.lecture_clubs(element);
            DialogControleur.Instance.ServerData.Structures.lecture_comites(element);
            DialogControleur.Instance.ServerData.Structures.lecture_secteurs(element);
            DialogControleur.Instance.ServerData.Structures.lecture_ligues(element);
            DialogControleur.Instance.ServerData.Structures.lecture_pays(element);
        }

        /// <summary>
        /// Reception des donnees suite a une demande de structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void client_OnListeStructures(object sender, XElement element)
        {
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
            UpdateRequestDispatcher(LectureDonneesStructures, element);
        }

        /// <summary>
        /// Lecture des donnees de categories contenu dans  Element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesCategories(XElement element)
        {
            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Categories.lecture_cateages(element);
            DC.ServerData.Categories.lecture_catepoids(element);
            DC.ServerData.Categories.lecture_ceintures(element);
        }

        /// <summary>
        /// Demande de liste des categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        public void client_OnListeCategories(object sender, XElement element)
        {
            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesCategories,
                                        LectureDonneesCategories,
                                        BusyStatusEnum.DemandeDonneesLogos,
                                        DialogControleur.Instance.Connection.Client.DemandeLogos,
                                        element);
        }

        public void client_OnUpdateCategories(object sender, XElement element)
        {
            UpdateRequestDispatcher(LectureDonneesCategories, element);
        }

        /// <summary>
        /// Lecture des donnees Logoes contenues dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesLogos(XElement element)
        {
            DialogControleur.Instance.ServerData.Logos.lecture_logos(element);
        }

        public void client_OnListeLogos(object sender, XElement element)
        {
            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesLogos,
                            LectureDonneesLogos,
                            BusyStatusEnum.DemandeDonneesOrganisation,
                            DialogControleur.Instance.Connection.Client.DemandeOrganisation,
                            element);
        }

        public void client_OnUpdateLogos(object sender, XElement element)
        {
            UpdateRequestDispatcher(LectureDonneesLogos, element);
        }

        /// <summary>
        /// Lecture des donnees d'organisation contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesOrganisations(XElement element)
        {
            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Organisation.lecture_competitions(element, DC.ServerData);
            DC.ServerData.Organisation.lecture_epreuves_equipe(element, DC.ServerData);
            DC.ServerData.Organisation.lecture_epreuves(element, DC.ServerData);
        }

        public void client_OnListeOrganisation(object sender, XElement element)
        {
            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesOrganisation,
                            LectureDonneesOrganisations,
                            BusyStatusEnum.DemandeDonneesJudokas,
                           () =>
                           {
                               DialogControleur DC = DialogControleur.Instance;
                               if (DC.ServerData.competition.IsEquipe())
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
            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Participants.lecture_epreuves_judokas(element, DC.ServerData);
            DC.ServerData.Participants.lecture_equipes(element);
            DC.ServerData.Participants.lecture_judokas(element, DC.ServerData);
        }

        public void client_OnListeEquipes(object sender, XElement element)
        {

            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesJudokas,
                            LectureDonneesEquipes,
                            BusyStatusEnum.DemandeDonneesPhases,
                            DialogControleur.Instance.Connection.Client.DemandePhases,
                            element);
        }


        public void client_OnUpdateEquipes(object sender, XElement element)
        {
            UpdateRequestDispatcher(LectureDonneesEquipes, element);
        }

        /// <summary>
        /// lecture des donnees des judokas contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesJudokas(XElement element)
        {
            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Participants.lecture_epreuves_judokas(element, DC.ServerData);
            DC.ServerData.Participants.lecture_judokas(element, DC.ServerData);
        }

        public void client_OnListeJudokas(object sender, XElement element)
        {
            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesJudokas,
                            LectureDonneesJudokas,
                            BusyStatusEnum.DemandeDonneesPhases,
                            DialogControleur.Instance.Connection.Client.DemandePhases,
                            element);

        }

        public void client_OnUpdateJudokas(object sender, XElement element)
        {
            UpdateRequestDispatcher(LectureDonneesJudokas, element);
        }

        /// <summary>
        /// Lecture des donnees des phases contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesPhases(XElement element)
        {
            DialogControleur DC = DialogControleur.Instance;
            // DC.ServerData.Deroulement.clear_deroulement();
            DC.ServerData.Deroulement.lecture_phases(element);
            DC.ServerData.Deroulement.lecture_participants(element);
            DC.ServerData.Deroulement.lecture_decoupages(element);
            DC.ServerData.Deroulement.lecture_poules(element);
            DC.ServerData.Deroulement.lecture_groupes(element, DC.ServerData);
        }

        public void client_OnListePhases(object sender, XElement element)
        {
            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesPhases,
                            LectureDonneesPhases,
                            BusyStatusEnum.DemandeDonneesCombats,
                            DialogControleur.Instance.Connection.Client.DemandeCombats,
                            element);
        }

        public void client_OnUpdatePhases(object sender, XElement element)
        {
            UpdateRequestDispatcher(LectureDonneesPhases, element);
        }

        /// <summary>
        /// Lecture des donnees des combats contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesCombats(XElement element)
        {
            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Deroulement.lecture_rencontres(element);
            DC.ServerData.Deroulement.lecture_feuilles(element);
            DC.ServerData.Deroulement.lecture_combats(element, DC.ServerData);
        }

        public void client_OnListeCombats(object sender, XElement element)
        {
            InitializationRequestDispatcher(BusyStatusEnum.InitDonneesCombats,
                            LectureDonneesCombats,
                            BusyStatusEnum.DemandeDonneesArbitres,
                            DialogControleur.Instance.Connection.Client.DemandeArbitrage,
                            element);
        }

        public void client_OnUpdateCombats2(object sender, XElement element)
        {
            // TODO Voir pour différencier les updates partiels des combats via OnUpdateCombats et les updates complets via OnUpdateTapisCombats
            client_OnUpdateCombats(sender, element);
        }

        public void client_OnUpdateCombats(object sender, XElement element)
        {
            // TODO Voir pour différencier les updates partiels des combats via OnUpdateCombats et les updates complets via OnUpdateTapisCombats
            UpdateRequestDispatcher(LectureDonneesCombats, element);
        }

        /// <summary>
        /// Lecture des donnees des rencontres contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesRencontres(XElement element)
        {
            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Deroulement.lecture_rencontres(element);
        }

        public void client_onUpdateRencontres(object sender, XElement element)
        {
            UpdateRequestDispatcher(LectureDonneesRencontres, element);
        }

        /// <summary>
        /// lecture des donnees d'arbitrage contenu dans element
        /// </summary>
        /// <param name="element"></param>
        private void LectureDonneesArbitrage(XElement element)
        {
            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Arbitrage.lecture_arbitres(element);
            DC.ServerData.Arbitrage.lecture_commissaires(element);
            DC.ServerData.Arbitrage.lecture_delegues(element);
        }

        public void client_OnListeArbitrage(object sender, XElement element)
        {
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
            System.Windows.Application.Current.ExecOnUiThread(new Action(() =>
            {
                bool isb = false;
                switch (status)
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

                DialogControleur.Instance.IsBusy = isb;
                if (isb)
                {
                    DialogControleur.Instance.BusyStatus = status;
                }
            }));
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
                LogTools.Logger.Debug("Reception donnees: '{0}'", element.ToString(SaveOptions.DisableFormatting));

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

                    // Appelle  le callback pour traiter les donnees
                    dataAction?.Invoke(element);

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
            LogTools.Logger.Debug("Reception donnees: '{0}'", element.ToString(SaveOptions.DisableFormatting));

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

                action?.Invoke(element);

            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de la mise a jour des donnees");
            }
        }

        #endregion
    }
}

