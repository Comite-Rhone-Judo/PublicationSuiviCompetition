using AppPublication.Tools.Enum;
using JudoClient;
using JudoClient.Communication;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System;
using System.Web.UI;
using System.Windows.Threading;
using System.Xml.Linq;
using Tools.Outils;

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
        public bool IsConnected => _isconnected;

        /// <summary>
        /// Le client de connexion Judo
        /// </summary>
        public ClientJudo Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                _isconnected = true;

                NotifyPropertyChanged();
                SetupClient();
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
            lock (_lock)
            {
                _isconnected = false;
                _reference = DateTime.Now;
                _nRetry++;
            }

            // On fait la demande de connection test en dehors du lock pour éviter un deadlock
            _client?.DemandConnectionTest();
        }

        /// <summary>
        /// Supprime le client et libere les ressources
        /// </summary>
        public void DisposeClient()
        {
            if(this.Client != null)
            {
                this.Client.NetworkClient.Stop();
                this.Client = null;
                _isconnected = false;

                // Notify subscribers of disconnection
                ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs());
            }
        }
        #endregion

        #region MTHODES PRIVEES
        /// <summary>
        /// Initialise a configure le client en connexion avec les evenements
        /// </summary>
        private void SetupClient()
        {
            if (_client == null)
            {
                return;
            }

            // Only subscribe to connection test (internal to GestionConnection)
            _client.TraitementConnexion.OnAcceptConnectionTest += clientjudo_OnDemandeConnectionTest;
            _client.OnReceivedDataErrorOccured += client_OnReceivedDataErrorOccured;
            _client.OnReceivedDataSuccessOccured += client_OnReceivedDataSuccessOccured;


            // Raise event so GestionEvent can subscribe to client events
            ClientReady?.Invoke(this, new ClientReadyEventArgs(_client));

            RaiseConnectionStatusChanged(true, BusyStatusEnum.InitDonneesStructures);

            // Start connection handshake
            _client.DemandConnectionCOM();
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
            LogTools.Logger.Debug("Signalement d'une erreur de donnee recue", data);
        }

        /// <summary>
        /// Evenement leve lors d'une reception de donnee avec succes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void client_OnReceivedDataSuccessOccured(object sender, string data)
        {
            // TODO Ajouter le traitement de la qualite de la connexion
            LogTools.Logger.Debug("Donnees recues avec succes signalee");
        }

        /// <summary>
        /// Timer de reponse a la demande de connection test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="doc"></param>
        private void clientjudo_OnDemandeConnectionTest(object sender, XElement doc)
        {
            _isconnected = true;
            _reference = DateTime.Now;
            _nRetry = 0;
        }

        /// <summary>
        /// Timer de gestion de la connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // TODO Voir pour integrer la gestion de la qualite de la connexion ici

            if (_client == null)
            {
                return;
            }

            DateTime now = DateTime.Now;
            double elapseSec = (now - _reference).TotalSeconds;


            if (_isconnected)
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
                    if (_nRetry > MAX_RETRY)
                    {
                        // on a deja essaye plusieurs fois, on est vraiment deconnecte
                        this.Client.NetworkClient.Stop();
                        this.Client = null;
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
