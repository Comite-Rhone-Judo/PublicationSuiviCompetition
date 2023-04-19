using AppPublication.Tools;
using JudoClient;
using JudoClient.Communication;
using System;
using System.ComponentModel;
using System.Windows.Threading;
using System.Xml.Linq;
using Tools.Outils;

namespace AppPublication.Controles
{
    public class GestionConnection : NotificationBase
    {
        private const Int32 TEST_CONNECT = 15;

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

                NotifyPropertyChanged("Client");
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
        }

        private void setClient()
        {
            if (_client == null)
            {
                return;
            }

            _client.OnEndConnection += GestionEvent.client_OnEndConnection;

            _client.TraitementConnexion.OnAcceptConnectionCOM += GestionEvent.clientjudo_OnAcceptConnectionCOM;
            
            _client.TraitementStructure.OnListeStructures += GestionEvent.client_OnListeStructures;
            _client.TraitementStructure.OnUpdateStructures += GestionEvent.client_OnUpdateStructures;

            _client.TraitementCategories.OnListeCategories += GestionEvent.client_OnListeCategories;
            _client.TraitementCategories.OnUpdateCategories += GestionEvent.client_OnUpdateCategories;

            _client.TraitementLogos.OnListeLogos += GestionEvent.client_OnListeLogos;
            _client.TraitementLogos.OnUpdateLogos += GestionEvent.client_OnUpdateLogos;

            _client.TraitementOrganisation.OnListeOrganisation += GestionEvent.client_OnListeOrganisation;
            _client.TraitementOrganisation.OnUpdateOrganisation += GestionEvent.client_OnUpdateOrganisation;

            _client.TraitementParticipants.OnListeEquipes += GestionEvent.client_OnListeEquipes;
            _client.TraitementParticipants.OnUpdateEquipes += GestionEvent.client_OnUpdateEquipes;

            _client.TraitementParticipants.OnListeJudokas += GestionEvent.client_OnListeJudokas;
            _client.TraitementParticipants.OnUpdateJudokas += GestionEvent.client_OnUpdateJudokas;

            _client.TraitementDeroulement.OnListePhases += GestionEvent.client_OnListePhases;
            _client.TraitementDeroulement.OnUpdatePhases += GestionEvent.client_OnUpdatePhases;

            _client.TraitementDeroulement.OnListeCombats += GestionEvent.client_OnListeCombats;
            _client.TraitementDeroulement.OnUpdateCombats += GestionEvent.client_OnUpdateCombats;
            _client.TraitementDeroulement.OnUpdateTapisCombats += GestionEvent.client_OnUpdateCombats2;

            _client.TraitementArbitrage.OnListeArbitrage += GestionEvent.client_OnListeArbitrage;
            _client.TraitementArbitrage.OnUpdateArbitrage += GestionEvent.client_OnUpdateArbitrage;

            _client.TraitementConnexion.OnAcceptConnectionTest += clientjudo_OnDemandeConnectionTest;

            DialogControleur.Instance.IsBusy = true;
            DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesClub;

            _client.DemandConnectionCOM();
        }

        private void clientjudo_OnDemandeConnectionTest(object sender, XElement doc)
        {
            _isconnected = true;
            _reference = DateTime.Now;
        }

        private void dispatcherTimer0_Tick(object sender, EventArgs e)
        {
            if (_client == null)
            {
                return;
            }

            DateTime now = DateTime.Now;

            if (_isconnected && (now - _reference).TotalSeconds > TEST_CONNECT)
            {
                this.TesteConnection();
            }
            else if (!_isconnected && (now - _reference).TotalSeconds > TEST_CONNECT)
            {
                this.Client.Client.Stop();
                this.Client = null;
            }
        }
    }
}
