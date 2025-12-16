using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Tools.Outils;

namespace Tools.TCP_Tools.Server
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
        private List<TcpClient> clients = new List<TcpClient>();
        private List<SentData> sentData = new List<SentData>();
        //private string chaine = "";
        private int _port = 0;



        /// <summary>
        /// Constructor for a new server using an IPAddress and Port
        /// </summary>
        /// <param name="localaddr">The Local IP Address for the server.</param>
        /// <param name="port">The port for the server.</param>
        public ServerGenerique(IPAddress localaddr, int port)
        {
            _port = port;
            tcpListener = new TcpListener(localaddr, port);
        }


        /// <summary>
        /// Starts the TCP Server listening for new clients.
        /// </summary>
        public void Start()
        {
            foreach (TcpClient client in clients)
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

            clients.Clear();
            //lstClientInfo.Clear();
            //bindingSource1.Clear();

            ListenerHelper.StartListening(ref tcpListener, _port, new AsyncCallback(DoAcceptTcpClientCallback));
        }

        /// <summary>
        /// Stops the TCP Server listening for new clients and disconnects
        /// any currently connected clients.
        /// </summary>
        public void Stop()
        {
            this.tcpListener.Stop();
            using (TimedLock.Lock((this.clients as ICollection).SyncRoot))
            {
                foreach (TcpClient client in clients)
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
            }

            clients.Clear();
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

                if (objListenerAndClient.Client != null)
                {
                    objListenerAndClient.Client.Close();
                }

                ExceptionHelper.ShowException(ex);

                return;
            }

            if (OnConnection != null)
            {
                OnConnection(this, client);
            }

            new ListenerHelper.ReceiveData(HandleReceive).BeginInvoke(client,
                new AsyncCallback(ReceiveCallback), objListenerAndClient.Listener);

            objListenerAndClient.Listener.BeginAcceptTcpClient(
             new AsyncCallback(DoAcceptTcpClientCallback), objListenerAndClient);
        }

        private void HandleReceive(TcpClient client)
        {
            LogHelper.ShowLog("", client, LogHelper.TypeLog.Connect);

            clients.Add(client);

            //Program.frmMainForm.delAddClient.Invoke(client);

            ClientConnection objClientConnection = new ClientConnection(client);
            objClientConnection.OnMessageReceived += new ClientConnection.MessageReceive(OnReceive);
            objClientConnection.OnRemoteHostClosed += new ClientConnection.RemoteHostClose(OnRemoteHostClose);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            //do nothing
            //TcpListener listener = (TcpListener)ar.AsyncState;
        }

        private void OnReceive(ClientConnection sender, string data)
        {
            try
            {
                if (OnDataRecieve != null)
                {
                    OnDataRecieve(tcpListener, sender.Client, data);
                    //this.chaine = "";
                }

                _receive_data += (ulong)data.Length;
                LogHelper.ShowLog(OutilsTools.SizeSuffix((ulong)(data.Length)) + "  ---  " + OutilsTools.SizeSuffix((ulong)(_receive_data)), sender.Client, LogHelper.TypeLog.ReceiveData);
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

            clients.Remove(sender.Client);

            if (OnEndConnection != null)
            {
                OnEndConnection(tcpListener, sender.Client);
            }

            if (clients.Count == 0)
            {
                ListenerHelper.StartListening(ref tcpListener, _port, new AsyncCallback(DoAcceptTcpClientCallback));
            }

            //Program.frmMainForm.delRemoveClient.Invoke(sender.Client);
        }

        /// <summary>
        /// Writes a string to a client connected.
        /// </summary>        
        /// <param name="tcpClient">the client</param>
        /// <param name="data">The string to send.</param>
        public void Write(TcpClient tcpClient, string data)
        {



            using (TimedLock.Lock((sentData as ICollection).SyncRoot))
            {
                SentData sent = sentData.FirstOrDefault(o => o != null && o.client == tcpClient);
                if (sent == null)
                {
                    sentData.Add(new SentData { data = data, client = tcpClient, tentative = 1 });
                }
                else
                {
                    sentData.Add(new SentData { data = data, client = tcpClient, tentative = sent.tentative + 1 });
                }
            }

            //if (tcpClient.Connected)
            //{

            bool send = ListenerHelper.SendData(tcpClient, data, new AsyncCallback(DoSending));

            _sent_data += (ulong)data.Length;
            LogHelper.ShowLog(OutilsTools.SizeSuffix((ulong)(data.Length)) + "  ---  " + OutilsTools.SizeSuffix((ulong)(_sent_data)), tcpClient, LogHelper.TypeLog.SentData);
            //}
        }

        /// <summary>
        /// Writes a string to all clients connected.
        /// </summary>
        /// <param name="data">The string to send.</param>
        public void Write(string data)
        {
            foreach (TcpClient client in this.clients)
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

                using (TimedLock.Lock((sentData as ICollection).SyncRoot))
                {
                    SentData sent = sentData.FirstOrDefault(o => o != null && o.client == client);
                    if (sent != null)
                    {
                        sentData.Remove(sent);
                    }
                }

                if (OnDataSent != null)
                {
                    OnDataSent(this, client);
                }

                //(new JudoClient.ClientHelper.ReceiveData(HandleReceive)).BeginInvoke(client, strReceiveData,
                //        new AsyncCallback(ReceiveCallback), client);
            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
                SentData sent = sentData.FirstOrDefault(o => o != null && o.client == client);
                if (sent == null)
                {
                    Write(client, sent.data);
                }
            }
        }
        #endregion
    }

    internal class SentData
    {
        public string data { get; set; }
        public TcpClient client { get; set; }
        public int tentative { get; set; }
    }

    ///// <summary>
    ///// Client Connection
    ///// </summary>
    //internal class ClientConnection
    //{
    //    public delegate void MessageReceive(ClientConnection sender, string Data);
    //    public delegate void RemoteHostClose(ClientConnection sender);


    //    #region field

    //    const int READ_BUFFER_SIZE = 10240;
    //    private byte[] readBuffer = new byte[READ_BUFFER_SIZE];

    //    public event MessageReceive OnMessageReceived;
    //    public event RemoteHostClose OnRemoteHostClosed;

    //    private TcpClient _Client;
    //    private string chaine = "";

    //    public TcpClient Client
    //    {
    //        get
    //        {
    //            return _Client;
    //        }
    //    }

    //    #endregion

    //    #region method

    //    public ClientConnection(TcpClient client)
    //    {
    //        try
    //        {
    //            _Client = client;

    //            client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
    //                new AsyncCallback(StreamReceiver), client);
    //        }
    //        catch (NullReferenceException ex)
    //        {
    //            ExceptionHelper.ShowException(ex);
    //        }
    //        catch (ObjectDisposedException ex)
    //        {
    //            client.Close();
    //            ExceptionHelper.ShowException(ex);
    //        }
    //        catch (IOException ex)
    //        {
    //            client.Close();
    //            ExceptionHelper.ShowException(ex);
    //        }
    //    }

    //    private void StreamReceiver(IAsyncResult ar)
    //    {
    //        TcpClient client = null;
    //        int intBytesRead = 0;
    //        string strReceiveData = string.Empty;
    //        NetworkStream objNetworkStream = null;

    //        try
    //        {
    //            client = (TcpClient)ar.AsyncState;

    //            if (!client.Connected)
    //            {
    //                client.Close();
    //                LogHelper.ShowLog("", client, LogHelper.TypeLog.Close);

    //                return;
    //            }

    //            try
    //            {
    //                objNetworkStream = client.GetStream();
    //            }
    //            catch (ObjectDisposedException ex)
    //            {
    //                client.Close();
    //                ExceptionHelper.ShowException(ex);
    //                return;
    //            }
    //            catch (InvalidOperationException ex)
    //            {
    //                client.Close();
    //                ExceptionHelper.ShowException(ex);
    //                return;
    //            }

    //            try
    //            {
    //                using (TimedLock.Lock(objNetworkStream)
    //                {
    //                    intBytesRead = objNetworkStream.EndRead(ar);
    //                }
    //            }
    //            catch (IOException ex)
    //            {
    //                if (ex.InnerException is SocketException)
    //                {
    //                    client.Close();
    //                    ExceptionHelper.ShowException(ex);

    //                    return;
    //                }
    //                else
    //                {
    //                    client.Close();
    //                    ExceptionHelper.ShowException(ex);
    //                    return;
    //                }
    //            }

    //            if (intBytesRead > 0)
    //            {
    //                strReceiveData = Encoding.UTF8.GetString(readBuffer, 0, intBytesRead);

    //                //string data = Encoding.UTF8.GetString(client.Buffer, 0, read);
    //                chaine += strReceiveData;

    //                if (chaine.IndexOf("\n<EOF>") != -1 && OnMessageReceived != null)
    //                {
    //                    OnMessageReceived(this, TraiteChaineJudo(chaine));

    //                    string[] s = chaine.Split(new string[] { "\n<EOF>" }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (s.Length > 1)
    //                    {
    //                        chaine = s.ElementAt(1);
    //                    }
    //                    else
    //                    {
    //                        chaine = "";
    //                    }

    //                    //OnMessageReceived(this, TraiteChaineJudo(chaine));
    //                    //chaine = "";
    //                }

    //                //OnMessageReceived(this, strReceiveData);
    //            }
    //            else
    //            {
    //                OnRemoteHostClosed(this);

    //                if (objNetworkStream != null)
    //                {
    //                    objNetworkStream.Close();
    //                }

    //                client.Close();
    //                return;

    //            }

    //            if (!client.Connected)
    //            {
    //                client.Close();
    //                LogHelper.ShowLog("", client, LogHelper.TypeLog.ClientClose);
    //                //LogHelper.ShowLog("client close\t" + DateTime.Now.ToString() + "\t" + client.GetHashCode().ToString());
    //                return;
    //            }

    //            try
    //            {
    //                using (TimedLock.Lock(objNetworkStream)
    //                {
    //                    objNetworkStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
    //                        new AsyncCallback(StreamReceiver), client);
    //                }
    //            }
    //            catch (ObjectDisposedException ex)
    //            {
    //                client.Close();
    //                ExceptionHelper.ShowException(ex);
    //            }
    //            catch (IOException ex)
    //            {
    //                client.Close();
    //                ExceptionHelper.ShowException(ex);
    //            }

    //        }
    //        catch (Exception ex)
    //        {
    //            client.Close();
    //            ExceptionHelper.ShowException(ex);
    //        }
    //    }

    //    string TraiteChaineJudo(string s)
    //    {
    //        string chaine = s.Split(new string[] { "\n<EOF>" }, StringSplitOptions.RemoveEmptyEntries).First();
    //        //string chaine = s.Replace("\n<EOF>", "");
    //        return chaine;
    //    }

    //    #endregion
    //}
}
