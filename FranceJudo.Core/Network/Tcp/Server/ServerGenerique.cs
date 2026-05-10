using FranceJudo.Core.IO;
using FranceJudo.Core.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Tcp.Server
{
    public class ServerGenerique
    {
        public static ulong _sent_data = 0;
        public static ulong _receive_data = 0;

        /// <summary>
        /// Fonction déléguée de début de connection
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="client">le client</param>
        public delegate void OnConnectionHandler(object sender, TcpClient client);

        /// <summary>
        /// Fonction déléguée d'envoie de donnée
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="client">le client</param>
        public delegate void OnDataSentHandler(object sender, TcpClient client);

        /// <summary>
        /// Fonction déléguée de réception de données
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="client">le client</param>
        /// <param name="donnees">données</param>
        public delegate void OnDataRecieveHandler(object sender, TcpClient client, string donnees);

        /// <summary>
        /// Fonction déléguée de fin de connection
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="client">le client</param>
        public delegate void OnEndConnectionHandler(object sender, TcpClient client);

        /// <summary>
        /// Événement de connection d'un client
        /// </summary>
        public event OnConnectionHandler OnConnection;

        /// <summary>
        /// Événement de réception de données à un client
        /// </summary>
        public event OnDataRecieveHandler OnDataRecieve;

        /// <summary>
        /// Événement d'envoie de données à un client
        /// </summary>
        public event OnDataSentHandler OnDataSent;

        /// <summary>
        /// Événement de fin de connection d'un client
        /// </summary>
        public event OnEndConnectionHandler OnEndConnection;

        //protected System.Net.Sockets.Socket listener;

        private TcpListener tcpListener;
        private readonly Synchronized<List<TcpClient>> _clients = new Synchronized<List<TcpClient>>(new List<TcpClient>());
        private readonly Synchronized<List<SentData>> _sentData = new Synchronized<List<SentData>>(new List<SentData>());
        //private string chaine = "";
        private readonly int _port = 0;
        private readonly string _endMsgTag;

        /// <summary>
        /// Balise de fin de message
        /// </summary>
        public string EndMsgTag
        {
            get { return _endMsgTag; }
        }

        /// <summary>
        /// Constructor for a new server using an IPAddress and Port
        /// </summary>
        /// <param name="localaddr">The Local IP Address for the server.</param>
        /// <param name="port">The port for the server.</param>
        public ServerGenerique(IPAddress localaddr, int port, string endMsgTag)
        {
            _port = port;
            tcpListener = new TcpListener(localaddr, port);
            _endMsgTag = endMsgTag;
        }


        /// <summary>
        /// Starts the TCP Server listening for new clients.
        /// </summary>
        public void Start()
        {
            // On verrouille la liste des clients pour le nettoyage initial
            _clients.SafeWriteAction(liste =>
            {
                foreach (TcpClient client in liste)
                {
                    if (client != null)
                    {
                        if (client.Connected && client.GetStream() != null)
                        {
                            client.GetStream().Close();
                        }
                        client.Close();
                    }
                }
                liste.Clear(); // Vidage sécurisé
            });

            // Le reste de l'initialisation reste inchangé
            ListenerHelper.StartListening(ref tcpListener, _port, new AsyncCallback(DoAcceptTcpClientCallback));
        }

        /// <summary>
        /// Stops the TCP Server listening for new clients and disconnects
        /// any currently connected clients.
        /// </summary>
        public void Stop()
        {
            this.tcpListener.Stop();

            _clients.SafeWriteAction(liste =>
            {
                foreach (TcpClient client in liste)
                {
                    if (client != null)
                    {
                        if (client.Connected && client.GetStream() != null)
                        {
                            client.GetStream().Dispose();
                            client.GetStream().Close();
                        }
                        client.Close();
                    }
                }
                liste.Clear();
            });

            ListenerHelper.StopListening(ref tcpListener);
        }


        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            ListenerHelper.ListenerAndClient objListenerAndClient = (
                ListenerHelper.ListenerAndClient)ar.AsyncState;
            TcpClient client = null;



            try
            {
                client = objListenerAndClient.Listener.EndAcceptTcpClient(ar);
                objListenerAndClient.Client = client;
            }
            catch (ObjectDisposedException ex)
            {
                //Stop Listening 

                objListenerAndClient.Client?.Close();

                ExceptionHelper.ShowException(ex);

                return;
            }

            OnConnection?.Invoke(this, client);

            // --- CORRECTION .NET 10 ---
            // On remplace le délégué.BeginInvoke par Task.Run
            // Cela exécute l'initialisation du client (HandleReceive) en arrière-plan
            _ = Task.Run(() => HandleReceive(client));

            objListenerAndClient.Listener.BeginAcceptTcpClient(
             new AsyncCallback(DoAcceptTcpClientCallback), objListenerAndClient);
        }

        private void HandleReceive(TcpClient client)
        {
            LogHelper.ShowLog("", client, LogHelper.TypeLog.Connect);

            _clients.SafeWriteAction(liste => liste.Add(client));

            //Program.frmMainForm.delAddClient.Invoke(client);

            ClientConnection objClientConnection = new ClientConnection(client, _endMsgTag);
            objClientConnection.OnMessageReceived += new ClientConnection.MessageReceive(OnReceive);
            objClientConnection.OnRemoteHostClosed += new ClientConnection.RemoteHostClose(OnRemoteHostClose);
        }

        private void OnReceive(ClientConnection sender, string data)
        {
            try
            {
                OnDataRecieve?.Invoke(tcpListener, sender.Client, data);

                _receive_data += (ulong)data.Length;

                ulong len = (ulong)(data.Length);
                ulong rdat = (ulong)(_receive_data);
                LogHelper.ShowLog(len.SizeSuffix() + "  ---  " + rdat.SizeSuffix(), sender.Client, LogHelper.TypeLog.ReceiveData);
                //LogHelper.ShowLog((data.Length / 1000) + "Ko" + "  ---  " + _receive_data / 1000 + "Ko", sender.Client, LogHelper.TypeLog.ReceiveData);
            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
            }
        }

        private void OnRemoteHostClose(ClientConnection sender)
        {
            LogHelper.ShowLog("", sender.Client, LogHelper.TypeLog.RemoteClose);
            int currentClientCount = 0;

            _clients.SafeWriteAction(liste =>
            {
                liste.Remove(sender.Client);
                currentClientCount = liste.Count;
            });

            OnEndConnection?.Invoke(tcpListener, sender.Client);

            if (currentClientCount == 0)
            {
                ListenerHelper.StartListening(ref tcpListener, _port, new AsyncCallback(DoAcceptTcpClientCallback));
            }
        }

        /// <summary>
        /// Writes a string to a client connected.
        /// </summary>        
        /// <param name="tcpClient">the client</param>
        /// <param name="data">The string to send.</param>
        public void Write(TcpClient tcpClient, string data)
        {
            _sentData.SafeWriteAction(liste =>
            {
                SentData sent = liste.FirstOrDefault(o => o != null && o.Client == tcpClient);
                if (sent == null)
                {
                    liste.Add(new SentData { Data = data, Client = tcpClient, Tentative = 1 });
                }
                else
                {
                    liste.Add(new SentData { Data = data, Client = tcpClient, Tentative = sent.Tentative + 1 });
                }
            });

            bool send = ListenerHelper.SendData(tcpClient, data, new AsyncCallback(DoSending));

            _sent_data += (ulong)data.Length;

            ulong len = (ulong)(data.Length);
            ulong rdat = (ulong)(_sent_data);

            LogHelper.ShowLog(len.SizeSuffix() + "  ---  " + rdat.SizeSuffix(), tcpClient, LogHelper.TypeLog.SentData);
        }

        /// <summary>
        /// Writes a string to all clients connected.
        /// </summary>
        /// <param name="data">The string to send.</param>
        public void Write(string data)
        {
            // On prend un snapshot rapide de la liste (Lecture)
            var snapshotClients = _clients.SafeReadAction(liste => liste.ToList());

            // On envoie les données en dehors du verrou pour ne pas bloquer le serveur
            foreach (TcpClient client in snapshotClients)
            {
                Write(client, data);
            }
        }

        #region DoSending
        /// <summary>
        /// Attends l'envoie de données 
        /// </summary>
        /// <param name="ar"></param>
        public void DoSending(IAsyncResult ar)
        {
            TcpClient client = null;
            try
            {
                client = (TcpClient)ar.AsyncState;
                NetworkStream networkStream = client.GetStream();
                networkStream.EndWrite(ar);

                _sentData.SafeWriteAction(liste =>
                {
                    liste.RemoveAll(o => o != null && o.Client == client);
                });

                OnDataSent?.Invoke(this, client);
            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
                SentData sent = _sentData.SafeReadAction(liste => liste.FirstOrDefault(o => o != null && o.Client == client));
                if (sent == null)
                {
                    Write(client, sent.Data);
                }
            }
        }
        #endregion
    }

    internal class SentData
    {
        public string Data { get; set; }
        public TcpClient Client { get; set; }
        public int Tentative { get; set; }
    }
}
