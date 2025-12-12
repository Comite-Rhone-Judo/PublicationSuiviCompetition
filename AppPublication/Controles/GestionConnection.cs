using JudoClient;
using JudoClient.Communication;
using System;
using System.Windows.Threading;
using System.Xml.Linq;
using Tools.Outils;

namespace AppPublication.Controles
{
    public class GestionConnection : NotificationBase
    {
        private const Int32 TEST_CONNECT = 15;
        private const Int32 MAX_RETRY = 5;

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

        private ClientJudo _client = null;
        private bool _isconnected = false;
        private DispatcherTimer _timer = null;
        private DateTime _reference = DateTime.Now;
        private int _nRetry = 0;
        private GestionEvent _eventManager = null;

        public GestionConnection(GestionEvent eventMgr)
        {
            _eventManager = eventMgr ?? throw new ArgumentNullException(nameof(eventMgr));
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
                setClient();
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

        private void setClient()
        {
            if (_client == null)
            {
                return;
            }

            _client.OnEndConnection += _eventManager.client_OnEndConnection;

            _client.TraitementConnexion.OnAcceptConnectionCOM += _eventManager.clientjudo_OnAcceptConnectionCOM;

            _client.TraitementStructure.OnListeStructures += _eventManager.client_OnListeStructures;
            _client.TraitementStructure.OnUpdateStructures += _eventManager.client_OnUpdateStructures;

            _client.TraitementCategories.OnListeCategories += _eventManager.client_OnListeCategories;
            _client.TraitementCategories.OnUpdateCategories += _eventManager.client_OnUpdateCategories;

            _client.TraitementLogos.OnListeLogos += _eventManager.client_OnListeLogos;
            _client.TraitementLogos.OnUpdateLogos += _eventManager.client_OnUpdateLogos;

            _client.TraitementOrganisation.OnListeOrganisation += _eventManager.client_OnListeOrganisation;
            _client.TraitementOrganisation.OnUpdateOrganisation +=  _eventManager.client_OnUpdateOrganisation;

            _client.TraitementParticipants.OnListeEquipes += _eventManager.client_OnListeEquipes;
            _client.TraitementParticipants.OnUpdateEquipes += _eventManager .client_OnUpdateEquipes;

            _client.TraitementParticipants.OnListeJudokas += _eventManager.client_OnListeJudokas;
            _client.TraitementParticipants.OnUpdateJudokas += _eventManager.client_OnUpdateJudokas;

            _client.TraitementDeroulement.OnListePhases += _eventManager.client_OnListePhases;
            _client.TraitementDeroulement.OnUpdatePhases += _eventManager.client_OnUpdatePhases;

            _client.TraitementDeroulement.OnListeCombats += _eventManager.client_OnListeCombats;
            _client.TraitementDeroulement.OnUpdateCombats += _eventManager.client_OnUpdateCombats;
            _client.TraitementDeroulement.OnUpdateTapisCombats += _eventManager.client_OnUpdateTapisCombats;
            _client.TraitementDeroulement.OnUpdateRencontreReceived += _eventManager.client_onUpdateRencontres;

            _client.TraitementArbitrage.OnListeArbitrage += _eventManager.client_OnListeArbitrage;
            _client.TraitementArbitrage.OnUpdateArbitrage += _eventManager.client_OnUpdateArbitrage;

            _client.TraitementConnexion.OnAcceptConnectionTest += clientjudo_OnDemandeConnectionTest;

            // TODO il faudrait virer cela ...
            DialogControleur.Instance.IsBusy = true;
            DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesStructures;

            _client.DemandConnectionCOM();
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

            /*
            if (_isconnected && (now - _reference).TotalSeconds > TEST_CONNECT)
            {
                this.TesteConnection();
            }
            else if (!_isconnected && (now - _reference).TotalSeconds > TEST_CONNECT && _nRetry > MAX_RETRY)
            {
                this.Client.Client.Stop();
                this.Client = null;
            }
            */
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
                        this.Client.Client.Stop();
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
