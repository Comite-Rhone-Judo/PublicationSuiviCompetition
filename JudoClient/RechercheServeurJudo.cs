using JudoClient.Communication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;
using Tools.Struct;
using Tools.TCP_Tools.Client;

namespace JudoClient
{
    /// <summary>
    /// Classe permettant la recherche de server
    /// </summary>
    public class RechercheServeurJudo
    {
        /// <summary>
        /// Fonction déléguée de server trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serverEndPoint"></param>
        /// <param name="machine"></param>
        /// <param name="user"></param>
        /// <param name="competition"></param>
        public delegate void ServerTrouveHandler(object sender, System.Net.IPEndPoint serverEndPoint,
            string machine, string user, XElement competition);
        public event ServerTrouveHandler onServerTrouve;

        /// <summary>
        /// Fonction délégué de fin de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pings"></param>
        /// <param name="connecte"></param>
        public delegate void TermineHandler(object sender, int pings, int connecte);
        public event TermineHandler onTermine;

        private ObservableCollection<MachineStruct> _listeMachines = new ObservableCollection<MachineStruct>();

        private int _nbMachines = 0;
        private bool _recherche_en_cours = false;
        public int test_recherche = 0;

        public RechercheServeurJudo()
        {
            //port = _port;
        }

        private List<string> GetListeMachine(string ip1)
        {
            List<string> machines = new List<string>();
            //listeServeurs.Clear();
            if (!String.IsNullOrWhiteSpace(ip1))
            {
                machines.Add(ip1);
                return machines;
            }

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;

                if (uniCast.Count > 0)
                {
                    foreach (UnicastIPAddressInformation uni in uniCast)
                    {
                        if (uni.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && uni.IsDnsEligible)
                        {
                            UInt32 mask = ParseIp(uni.IPv4Mask.ToString());
                            UInt32 ip = ParseIp(uni.Address.ToString());
                            UInt32 first = (ip & mask) + 1;
                            UInt32 last = ((ip & mask) + ~mask);

                            for (UInt32 host = first; host < last; host++)
                            {
                                machines.Add(ToIpString(host));
                            }
                        }
                    }
                }
            }

            return machines;
        }


        public void DemarreRechecherche(string ipAdressText, System.ComponentModel.BackgroundWorker bWorker = null)
        {
            if (_recherche_en_cours)
            {
                return;
            }
            else
            {
                _recherche_en_cours = true;
                test_recherche++;
            }

            List<string> machines = GetListeMachine(ipAdressText);
            _nbMachines = machines.Count * (NetworkTools.PortServerMax - NetworkTools.PortServerMin + 1);

            using (TimedLock.Lock((_listeMachines as ICollection).SyncRoot))
            {
                _listeMachines.CollectionChanged -= new NotifyCollectionChangedEventHandler(liste_Changes);
                _listeMachines.Clear();
            }

            int index = 0;
            foreach (string adresse in machines)
            {
                if (bWorker != null && bWorker.CancellationPending)
                {
                    // si on s'execute dans une tache qui doit s'arreter on quitte directement
                    return;
                }

                if (index++ % 10 == 0)
                {
                    Thread.Sleep(100);
                }

                using (TimedLock.Lock((_listeMachines as ICollection).SyncRoot))
                {
                    for (int port = NetworkTools.PortServerMin; port <= NetworkTools.PortServerMax; port++)
                    {
                        _listeMachines.Add(new MachineStruct { adresse = adresse + ":" + port, response = ServerResponseEnum.Aucun });
                    }
                }
                //LogTools.Log("PING -> " + adresse);

                Ping ping = new Ping();
                ping.PingCompleted += ping_PingCompleted;

                try
                {
                    ping.SendAsync(adresse, 1000, adresse);
                }
                catch (Exception ex)
                {
                    ExceptionHelper.ShowException(ex);
                    for (int port = NetworkTools.PortServerMin; port <= NetworkTools.PortServerMax; port++)
                    {
                        AdresseTerminee(adresse, port, ServerResponseEnum.PingFAIL);
                    }
                }

                _listeMachines.CollectionChanged += new NotifyCollectionChangedEventHandler(liste_Changes);
            }
        }


        void ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string adresse = e.UserState.ToString();
            bool EnvoieConnection = false;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                //LogTools.Log("PING SUCCESS -> " + adresse);
                Ping ping = (Ping)sender;
                ping.SendAsyncCancel();


                for (int port = NetworkTools.PortServerMin; port <= NetworkTools.PortServerMax; port++)
                {

                    try
                    {
                        //LogTools.Log("DEMANDE CONNEXION -> " + adresse +  ":"+port);

                        ClientJudo clientjudo = new ClientJudo(adresse, port);
                        if (clientjudo.IsConnected)
                        {
                            EnvoieConnection = true;
                            clientjudo.TraitementConnexion.OnAcceptConnectionTest += clientjudo_OnDemandeConnection;
                            clientjudo.DemandConnectionTest();
                        }
                        else
                        {
                            AdresseTerminee(adresse, port, ServerResponseEnum.PingOK);
                            //LogTools.Log("DEMANDE REFUSEE -> " + adresse + ":" + port);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.ShowException(ex);
                        AdresseTerminee(adresse, port, ServerResponseEnum.PingOK);
                    }


                    Thread.Sleep(100);
                }

            }

            if (!EnvoieConnection)
            {
                for (int port = NetworkTools.PortServerMin; port <= NetworkTools.PortServerMax; port++)
                {
                    //LogTools.Log("PING FAIL -> " + adresse);
                    AdresseTerminee(adresse, port, ServerResponseEnum.PingFAIL);
                }
            }
        }

        void clientjudo_OnDemandeConnection(object sender, XElement xvaleur)
        {
            ClientJudo clientjudo = (ClientJudo)sender;
            //if (doc.Element(ConstantXML.ServerJudo) != null)
            //{


            if (onServerTrouve != null)
            {
                //XElement xvaleur = doc.Element(ConstantXML.ServerJudo);

                string machine = xvaleur.Attribute(ConstantXML.Machine).Value;
                string user = xvaleur.Attribute(ConstantXML.User).Value;

                //LogTools.Log("DEMANDE ACCEPTER -> " + machine + ":" + user);

                XElement xcompetition = xvaleur.Element(ConstantXML.Competition);

                onServerTrouve(this, new System.Net.IPEndPoint(
                    System.Net.IPAddress.Parse(clientjudo.NetworkClient.IP), clientjudo.NetworkClient.Port), machine, user, xcompetition);

                clientjudo.NetworkClient.Stop();
            }
            //}
            AdresseTerminee(clientjudo.NetworkClient.IP, clientjudo.NetworkClient.Port, ServerResponseEnum.ConnectionOK);
        }

        void clientjudo_OnConnection(object sender)
        {
            ClientGenerique clientjudo = (ClientGenerique)sender;
        }

        void AdresseTerminee(string adresse, int port, ServerResponseEnum value)
        {
            try
            {
                using (TimedLock.Lock((_listeMachines as ICollection).SyncRoot, TimeSpan.FromSeconds(30)))
                {
                    MachineStruct machine = _listeMachines.FirstOrDefault(o => o.adresse == adresse + ":" + port);
                    int index = _listeMachines.IndexOf(machine);
                    _listeMachines[index] = new MachineStruct { adresse = machine.adresse, response = value };
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
            }
        }

        private void liste_Changes(object sender, NotifyCollectionChangedEventArgs e)
        {
            int nb = _listeMachines.Count(o => o.response != ServerResponseEnum.Aucun);

            if (onTermine != null && nb == _nbMachines && _recherche_en_cours)
            {
                _recherche_en_cours = false;
                _listeMachines.CollectionChanged -= new NotifyCollectionChangedEventHandler(liste_Changes);
                onTermine(this, _listeMachines.Count, _listeMachines.Count(o => o.response == ServerResponseEnum.ConnectionOK));
            }
        }

        public string ToIpString(UInt32 value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = (value & bitmask) >> ((3 - i) * 8);
                bitmask >>= 8;
                parts[i] = masked.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return String.Join(".", parts);
        }

        public UInt32 ParseIp(string ipAddress)
        {
            var splitted = ipAddress.Split('.');
            UInt32 ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + UInt32.Parse(splitted[i]);
            }
            return ip;
        }


    }
}
