using AppPublication.Tools.Enum;
using JudoClient;
using JudoClient.Communication;
using System;
using System.Windows.Threading;
using System.Xml.Linq;
using Tools.Outils;

namespace AppPublication.Controles
{
    public class GestionConnection : NotificationBase, IClientProvider
    {
        private const Int32 TEST_CONNECT = 15;
        private const Int32 MAX_RETRY = 5;

        public event EventHandler<ClientReadyEventArgs> ClientReady;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public string IpAdress
        {
            get;
            set;
        }

        public string Port
        {
            get;
            set;
        }

        public bool IsConnected => _isconnected;

        private ClientJudo _client = null;
        private bool _isconnected = false;
        private DispatcherTimer _timer = null;
        private DateTime _reference = DateTime.Now;
        private int _nRetry = 0;

        public GestionConnection()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(dispatcherTimer0_Tick);
            _timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
        }

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

        /// <summary>
        /// Le test de connection revient à faire une demande de connection à laquelle on doit recevoir un accusé.
        /// Si ce n'est pas le cas, la connection a été perdu.
        /// </summary>
        public void TesteConnection()
        {
            _isconnected = false;
            _reference = DateTime.Now;
            _client.DemandConnectionTest();
            _nRetry++;
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

        private void clientjudo_OnDemandeConnectionTest(object sender, XElement doc)
        {
            _isconnected = true;
            _reference = DateTime.Now;
            _nRetry = 0;
        }

        private void dispatcherTimer0_Tick(object sender, EventArgs e)
        {
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
