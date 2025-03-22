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

        public GestionConnection()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(dispatcherTimer0_Tick);
            _timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            //_timer.Start();
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

            _client.OnEndConnection += GestionEvent.Instance.client_OnEndConnection;

            _client.TraitementConnexion.OnAcceptConnectionCOM += GestionEvent.Instance.clientjudo_OnAcceptConnectionCOM;

            _client.TraitementStructure.OnListeStructures += GestionEvent.Instance.client_OnListeStructures;
            _client.TraitementStructure.OnUpdateStructures += GestionEvent.Instance.client_OnUpdateStructures;

            _client.TraitementCategories.OnListeCategories += GestionEvent.Instance.client_OnListeCategories;
            _client.TraitementCategories.OnUpdateCategories += GestionEvent.Instance.client_OnUpdateCategories;

            _client.TraitementLogos.OnListeLogos += GestionEvent.Instance.client_OnListeLogos;
            _client.TraitementLogos.OnUpdateLogos += GestionEvent.Instance.client_OnUpdateLogos;

            _client.TraitementOrganisation.OnListeOrganisation += GestionEvent.Instance.client_OnListeOrganisation;
            _client.TraitementOrganisation.OnUpdateOrganisation += GestionEvent.Instance.client_OnUpdateOrganisation;

            _client.TraitementParticipants.OnListeEquipes += GestionEvent.Instance.client_OnListeEquipes;
            _client.TraitementParticipants.OnUpdateEquipes += GestionEvent.Instance.client_OnUpdateEquipes;

            _client.TraitementParticipants.OnListeJudokas += GestionEvent.Instance.client_OnListeJudokas;
            _client.TraitementParticipants.OnUpdateJudokas += GestionEvent.Instance.client_OnUpdateJudokas;

            _client.TraitementDeroulement.OnListePhases += GestionEvent.Instance.client_OnListePhases;
            _client.TraitementDeroulement.OnUpdatePhases += GestionEvent.Instance.client_OnUpdatePhases;

            _client.TraitementDeroulement.OnListeCombats += GestionEvent.Instance.client_OnListeCombats;
            _client.TraitementDeroulement.OnUpdateCombats += GestionEvent.Instance.client_OnUpdateCombats;
            _client.TraitementDeroulement.OnUpdateTapisCombats += GestionEvent.Instance.client_OnUpdateCombats2;
            _client.TraitementDeroulement.OnUpdateRencontreReceived += GestionEvent.Instance.client_onUpdateRencontres;

            _client.TraitementArbitrage.OnListeArbitrage += GestionEvent.Instance.client_OnListeArbitrage;
            _client.TraitementArbitrage.OnUpdateArbitrage += GestionEvent.Instance.client_OnUpdateArbitrage;

            _client.TraitementConnexion.OnAcceptConnectionTest += clientjudo_OnDemandeConnectionTest;

            DialogControleur.Instance.IsBusy = true;
            DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesClub;

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
