using AppPublication.Tools.Enum;
using JudoClient;
using JudoClient.Communication;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System;
using System.Web.UI;
using System.Windows.Threading;
using System.Xml.Linq;
using Tools.Framework;
using Tools.Logging;

namespace AppPublication.Controles
{
    public class GestionConnection : NotificationBase, IClientProvider
    {
        #region CONSTANTES
        private const Int32 TEST_CONNECT = 15;
        private const Int32 MAX_RETRY = 5;
        #endregion

        #region EVENT HANDLERS
        public event EventHandler<ClientReadyEventArgs> ClientReady;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;
        #endregion

        #region MEMBERS
        private ClientJudo _client = null;
        private bool _isconnected = false;
        private DispatcherTimer _timer = null;
        private DateTime _reference = DateTime.Now;
        private int _nRetry = 0;
        private object _lock = new object();
        private volatile bool _isDisposing = false;
        private bool _hasErreurTransmission = false;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Adresse IP du serveur Judo
        /// </summary>
        public string IpAdress
        {
            get;
            set;
        }

        /// <summary>
        /// Port de connexion utilise par le client Judo
        /// </summary>
        public string Port
        {
            get;
            set;
        }

        /// <summary>
        /// Etat de connexion au serveur Judo
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock (_lock)
                {
                    return _isconnected;
                }
            }
        }

        /// <summary>
        /// Indique si une erreur de transmission a ete detectee
        /// </summary>
        public bool HasErreurTransmission
        {
            get
            {
                lock (_lock)
                {
                    return _hasErreurTransmission;
                }
            }
            set
            {
                bool changed = false;
                lock (_lock)
                {
                    if (_hasErreurTransmission != value)
                    {
                        _hasErreurTransmission = value;
                        changed = true;
                    }
                }

                if (changed)
                {
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Le client de connexion Judo
        /// </summary>
        public ClientJudo Client
        {
            get
            {
                lock (_lock)
                {
                    return _client;
                }
            }
            set
            {
                bool shouldSetup = false;
                ClientJudo clientToSetup = null;

                lock (_lock)
                {
                    _client = value;
                    _isconnected = value != null;

                    if (value != null)
                    {
                        shouldSetup = true;
                        clientToSetup = value;
                        _reference = DateTime.Now;
                        _nRetry = 0;
                    }
                }

                NotifyPropertyChanged();

                // Réinitialiser le flag d'erreur via le setter
                HasErreurTransmission = false;

                if (shouldSetup)
                {
                    SetupClient(clientToSetup);
                }
            }
        }


        #endregion

        #region CONSTRUCTEURS
        public GestionConnection()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(dispatcherTimer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
        }
        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// Le test de connection revient à faire une demande de connection à laquelle on doit recevoir un accusé.
        /// Si ce n'est pas le cas, la connection a été perdu.
        /// </summary>
        public void TesteConnection()
        {
            ClientJudo clientToTest;

            lock (_lock)
            {
                if (_isDisposing || _client == null)
                {
                    return;
                }

                _isconnected = false;
                _reference = DateTime.Now;
                _nRetry++;
                clientToTest = _client;
            }

            // On fait la demande de connection test en dehors du lock pour éviter un deadlock
            try
            {
                clientToTest?.DemandConnectionTest();
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error("Erreur lors du test de connexion", ex);
            }
        }

        /// <summary>
        /// Supprime le client et libere les ressources
        /// </summary>
        public void DisposeClient()
        {
            ClientJudo clientToDispose = null;

            lock (_lock)
            {
                if (_isDisposing || _client == null)
                {
                    return;
                }

                _isDisposing = true;
                clientToDispose = _client;
                Client = null;  // Ici il faut passer par la propriété pour bien notifier le changement
                _isconnected = false;
            }

            // Arrêt du timer
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
            }

            // Dispose en dehors du lock pour éviter les deadlocks
            if (clientToDispose != null)
            {
                try
                {
                    clientToDispose.NetworkClient.Stop();
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error("Erreur lors de la fermeture du client", ex);
                }

                // Notify subscribers of disconnection
                ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs());
            }

            // Réinitialiser le flag d'erreur via le setter
            HasErreurTransmission = false;

            lock (_lock)
            {
                _isDisposing = false;
            }
        }
        #endregion

        #region MTHODES PRIVEES
        /// <summary>
        /// Initialise a configure le client en connexion avec les evenements
        /// </summary>
        private void SetupClient(ClientJudo client)
        {
            if (client == null)
            {
                return;
            }

            // Vérifier que le client est toujours valide
            lock (_lock)
            {
                if (_isDisposing || _client != client)
                {
                    // Le client a été changé ou dispose entre temps
                    return;
                }
            }

            // Only subscribe to connection test (internal to GestionConnection)
            client.TraitementConnexion.OnAcceptConnectionTest += clientjudo_OnDemandeConnectionTest;
            client.OnReceivedDataErrorOccured += client_OnReceivedDataErrorOccured;
            client.OnReceivedDataSuccessOccured += client_OnReceivedDataSuccessOccured;


            // Raise event so GestionEvent can subscribe to client events
            ClientReady?.Invoke(this, new ClientReadyEventArgs(client));

            RaiseConnectionStatusChanged(true, BusyStatusEnum.InitDonneesStructures);

            // Start connection handshake
            client.DemandConnectionCOM();

            // Démarrer le timer de surveillance de la connexion
            if (_timer != null && !_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Notifie les abonnés d'un changement d'état de connexion
        /// </summary>
        private void RaiseConnectionStatusChanged(bool isBusy, BusyStatusEnum status)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusEventArgs(isBusy, status));
        }
        #endregion

        /// <summary>
        /// Evenement leve lors d'une erreur de donnee recue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void client_OnReceivedDataErrorOccured(object sender, string data)
        {
            // Lever le drapeau d'erreur de transmission
            HasErreurTransmission = true;

            LogTools.Logger.Debug("Signalement d'une erreur de donnee recue", data);
        }

        /// <summary>
        /// Evenement leve lors d'une reception de donnee avec succes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void client_OnReceivedDataSuccessOccured(object sender, string data)
        {
            LogTools.Logger.Debug("Donnees recues avec succes signalee");
        }

        /// <summary>
        /// Timer de reponse a la demande de connection test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="doc"></param>
        private void clientjudo_OnDemandeConnectionTest(object sender, XElement doc)
        {
            lock (_lock)
            {
                if (!_isDisposing)
                {
                    _isconnected = true;
                    _reference = DateTime.Now;
                    _nRetry = 0;
                }
            }
        }

        /// <summary>
        /// Timer de gestion de la connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ClientJudo currentClient;
            bool isConnected;
            DateTime reference;
            int nRetry;

            lock (_lock)
            {
                if (_isDisposing || _client == null)
                {
                    return;
                }

                currentClient = _client;
                isConnected = _isconnected;
                reference = _reference;
                nRetry = _nRetry;
            }

            DateTime now = DateTime.Now;
            double elapseSec = (now - reference).TotalSeconds;


            if (isConnected)
            {
                // Verifie si le delai avant de tester la connexion est echue ou non
                if (elapseSec > TEST_CONNECT)
                {
                    TesteConnection();
                }
            }
            else
            {
                // Le client n'est plus connecte, peut etre la reponse n'est pas encore recu
                if (elapseSec > TEST_CONNECT)
                {
                    // Essaye de refaire une demande de connexion
                    if (nRetry > MAX_RETRY)
                    {
                        // on a deja essaye plusieurs fois, on est vraiment deconnecte
                        DisposeClient();
                    }
                    else
                    {
                        // On va essayer une nouvelle fois
                        TesteConnection();
                    }
                }
            }
        }
    }
}
