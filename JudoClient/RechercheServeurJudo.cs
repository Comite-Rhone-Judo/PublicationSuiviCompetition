using FranceJudo.Core.Logging;
using FranceJudo.Core.Network.Tcp.Client;
using FranceJudo.Core.Threading;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;
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
        public event ServerTrouveHandler OnServerTrouve;

        /// <summary>
        /// Fonction délégué de fin de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pings"></param>
        /// <param name="connecte"></param>
        public delegate void TermineHandler(object sender, int pings, int connecte);
        public event TermineHandler OnTermine;

        private readonly Synchronized<List<MachineStruct>> _listeMachines = new Synchronized<List<MachineStruct>>(new List<MachineStruct>());

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
                        bool isDnsEligible = !OperatingSystem.IsWindows() || uni.IsDnsEligible;

                        if (uni.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && isDnsEligible)
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
            _nbMachines = machines.Count * (ConstantNetwork.PortServerMax - ConstantNetwork.PortServerMin + 1);


            _listeMachines.SafeWriteAction(liste => liste.Clear());

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

                // 2. Ajout sécurisé par lots
                _listeMachines.SafeWriteAction(liste =>
                {
                    for (int port = ConstantNetwork.PortServerMin; port <= ConstantNetwork.PortServerMax; port++)
                    {
                        liste.Add(new MachineStruct { Adresse = adresse + ":" + port, Response = ServerResponseEnum.Aucun });
                    }
                });

                Ping ping = new Ping();
                ping.PingCompleted += Ping_PingCompleted;

                try
                {
                    ping.SendAsync(adresse, 1000, adresse);
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur lors de la tentative de ping sur {0}", adresse);
                    for (int port = ConstantNetwork.PortServerMin; port <= ConstantNetwork.PortServerMax; port++)
                    {
                        AdresseTerminee(adresse, port, ServerResponseEnum.PingFAIL);
                    }
                }
            }
        }


        void Ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string adresse = e.UserState.ToString();
            bool EnvoieConnection = false;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                //LogTools.Log("PING SUCCESS -> " + adresse);
                Ping ping = (Ping)sender;
                ping.SendAsyncCancel();


                for (int port = ConstantNetwork.PortServerMin; port <= ConstantNetwork.PortServerMax; port++)
                {

                    try
                    {
                        //LogTools.Log("DEMANDE CONNEXION -> " + adresse +  ":"+port);

                        ClientJudo clientjudo = new ClientJudo(adresse, port);
                        if (clientjudo.IsConnected)
                        {
                            EnvoieConnection = true;
                            clientjudo.TraitementConnexion.OnAcceptConnectionTest += Clientjudo_OnDemandeConnection;
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
                        LogTools.Logger.Error(ex, "Erreur lors de la tentative de connexion sur {0}:{1}", adresse, port);
                        AdresseTerminee(adresse, port, ServerResponseEnum.PingOK);
                    }


                    Thread.Sleep(100);
                }

            }

            if (!EnvoieConnection)
            {
                for (int port = ConstantNetwork.PortServerMin; port <= ConstantNetwork.PortServerMax; port++)
                {
                    //LogTools.Log("PING FAIL -> " + adresse);
                    AdresseTerminee(adresse, port, ServerResponseEnum.PingFAIL);
                }
            }
        }

        void Clientjudo_OnDemandeConnection(object sender, XElement xvaleur)
        {
            ClientJudo clientjudo = (ClientJudo)sender;
            //if (doc.Element(ConstantXML.ServerJudo) != null)
            //{


            if (OnServerTrouve != null)
            {
                //XElement xvaleur = doc.Element(ConstantXML.ServerJudo);

                string machine = xvaleur.Attribute(ConstantXML.Machine).Value;
                string user = xvaleur.Attribute(ConstantXML.User).Value;

                //LogTools.Log("DEMANDE ACCEPTER -> " + machine + ":" + user);

                XElement xcompetition = xvaleur.Element(ConstantXML.Competition);

                OnServerTrouve(this, new System.Net.IPEndPoint(
                    System.Net.IPAddress.Parse(clientjudo.NetworkClient.IP), clientjudo.NetworkClient.Port), machine, user, xcompetition);

                clientjudo.NetworkClient.Stop();
            }
            //}
            AdresseTerminee(clientjudo.NetworkClient.IP, clientjudo.NetworkClient.Port, ServerResponseEnum.ConnectionOK);
        }

        void AdresseTerminee(string adresse, int port, ServerResponseEnum value)
        {
            try
            {
                _listeMachines.SafeWriteAction(liste =>
                {
                    // FindIndex est beaucoup plus performant qu'un FirstOrDefault suivi d'un IndexOf
                    int index = liste.FindIndex(o => o.Adresse == adresse + ":" + port);
                    if (index >= 0)
                    {
                        liste[index] = new MachineStruct { Adresse = liste[index].Adresse, Response = value };
                    }
                });

                // On appelle la vérification explicitement, EN DEHORS du verrou.
                CheckSiTermine();
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de la tentative de mise à jour de l'état de la machine {0}:{1}", adresse, port);
            }
        }

        private void CheckSiTermine()
        {
            // Optimisation : si la recherche est déjà déclarée finie, on sort tout de suite
            if (!_recherche_en_cours) return;

            // On utilise le Read pour RETOURNER un objet contenant nos statistiques.
            var stats = _listeMachines.SafeReadAction(liste => new
            {
                Total = liste.Count,
                Repondus = liste.Count(o => o.Response != ServerResponseEnum.Aucun),
                Connectes = liste.Count(o => o.Response == ServerResponseEnum.ConnectionOK)
            });

            if (OnTermine != null && stats.Repondus == _nbMachines && _recherche_en_cours)
            {
                _recherche_en_cours = false; // Agit comme un verrou logique
                OnTermine(this, stats.Total, stats.Connectes);
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
