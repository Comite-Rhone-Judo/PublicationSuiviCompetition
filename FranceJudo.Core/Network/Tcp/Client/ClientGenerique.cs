using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Tcp.Client
{
    // TODO Prévoir un refactoring avec les nouveaux modèles await/async et les améliorations de .NET 10, notamment pour la gestion des threads et des tâches asynchrones.

    /// <summary>
    /// ClientGenerique : classe générique de communication TCP (client)
    /// </summary>
    public class ClientGenerique
    {
        public delegate void OnConnectionHandler(object sender);
        public delegate void OnDataRecieveHandler(object sender, string donnees);
        public delegate void OnDataSentHandler(object sender);
        public delegate void OnEndConnectionHandler(object sender);

        /// <summary>
        /// Evenement de connection
        /// </summary>
        /// <param name="sender"></param>

        public event OnConnectionHandler OnConnection;

        /// <summary>
        /// Evenement de réception de données
        /// </summary>
        /// <param name="sender"></param>

        public event OnDataRecieveHandler OnDataRecieve;

        /// <summary>
        /// Evenement d'envoie de données
        /// </summary>
        /// <param name="sender"></param>
        public event OnDataSentHandler OnDataSent;

        /// <summary>
        /// Evenement de fin de connection
        /// </summary>
        /// <param name="sender"></param>
        public event OnEndConnectionHandler OnEndConnection;
        //public event OnErrorHandler OnError;

        #region MEMBRES
        private const int READ_BUFFER_SIZE = 10240;
        private readonly byte[] readBuffer = new byte[READ_BUFFER_SIZE];
        private string chaine = "";
        private readonly string _endMsgTag = string.Empty;
        private TcpClient objClient = null;
        private readonly int _port = 8484;
        private readonly string _ip = "127.0.0.1";
        // Un cadenas dédié, invisible de l'extérieur, pour protéger les accès au flux TCP
        private readonly object _networkLock = new object();
        #endregion

        /// <summary>
        /// IP du server
        /// </summary>
        public string IP
        {
            get
            {
                return _ip;
            }
        }

        /// <summary>
        /// Port de com du server
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
        }

        /// <summary>
        /// Flag marquant la fin d'un message
        /// </summary>
        public string EndMsgFlag
        {
            get { return _endMsgTag; }
        }

        /// <summary>
        /// EndPoint du client
        /// </summary>
        public System.Net.IPEndPoint EndPoint
        {
            get
            {
                return (System.Net.IPEndPoint)objClient.Client.RemoteEndPoint;
            }
        }

        /// <summary>
        /// Client est connecté au server
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    return objClient.Client != null && objClient.Client.Connected;
                }
                catch
                {
                    LogTools.Logger?.Debug("ClientGenerique IsConnected - Exception sur la verification de la connection");
                    return false;
                }
            }
        }


        /// <summary>
        /// Construct a new client where the address or host name of
        /// the server is known.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address of the server</param>
        /// <param name="port">The port of the server</param>
        public ClientGenerique(string hostNameOrAddress, int port, string endMsgTag)
        {
            _ip = hostNameOrAddress;
            _port = port;
            _endMsgTag = endMsgTag;
        }

        /// <summary>
        /// Connection à un server
        /// </summary>
        public void Connect()
        {
            ClientHelper.ConnectSocket(ref objClient, _ip, _port, new AsyncCallback(DoConnecting));
        }

        /// <summary>
        /// Met fin à la connection au server
        /// </summary>
        public void Stop()
        {
            ClientHelper.Close(objClient);
        }

        /// <summary>
        /// Ecrit un message au server
        /// </summary>
        /// <param name="data"></param>
        public void Write(string data)
        {
            ClientHelper.SendData(objClient, data, new AsyncCallback(DoSending));
        }


        #region DoConnecting
        private void DoConnecting(IAsyncResult ar)
        {
            try
            {
                TcpClient client = (TcpClient)ar.AsyncState;

                if (client.Client != null && client.Client.Connected)
                {
                    client.EndConnect(ar);

                    OnConnection?.Invoke(this);

                    client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoReading), client);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
            }
        }
        #endregion

        #region DoReading
        private void DoReading(IAsyncResult ar)
        {
            int BytesRead = 0;
            string strReceiveData = string.Empty;
            TcpClient client = null;

            try
            {
                client = (TcpClient)ar.AsyncState;

                if (client.Connected)
                {
                    using (TimedLock.Lock(_networkLock))
                    {
                        BytesRead = client.GetStream().EndRead(ar);
                    }
                }

                if (BytesRead > 0)
                {
                    try
                    {
                        strReceiveData = FileSystemHelper.TheEncoding.GetString(readBuffer, 0, BytesRead);

                        chaine += strReceiveData;

                        if (chaine.IndexOf("\n<EOF>") != -1 && OnDataRecieve != null)
                        {
                            string tmp = "";
                            foreach (string data in chaine.Split(new string[] { "\n<EOF>" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                // if (data.EndsWith("</" + ConstantXML.ServerJudo + ">"))
                                if (data.EndsWith(EndMsgFlag))
                                {
                                    OnDataRecieve(this, data);
                                }
                                else
                                {
                                    tmp = data;
                                }
                            }

                            chaine = tmp;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.ShowException(ex);
                    }
                    finally
                    {
                        // SOLUTION .NET 10 : Remplacement du BeginInvok_ par Task.Run
                        // On exécute le logging (HandleReceive) sur le ThreadPool.
                        // C'est asynchrone, performant et indépendant de WPF.
                        _ = Task.Run(() => HandleReceive(client, strReceiveData));

                        // Relance immédiate de la lecture pour ne pas saturer le buffer TCP
                        if (client != null && client.Connected)
                        {
                            using (TimedLock.Lock(_networkLock))
                            {
                                client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoReading), client);
                            }
                        }
                    }
                }
                else
                {
                    OnEndConnection?.Invoke(this);

                    client.Close();

                    LogHelper.ShowLog("Connect Closed\t" + DateTime.Now.ToString() + "\t" +
                        client.GetHashCode().ToString());

                    return;
                }
            }
            catch (IOException ex)
            {
                OnEndConnection?.Invoke(this);

                client.Close();
                ExceptionHelper.ShowException(ex);
            }
            catch (Exception ex)
            {
                client.Close();
                ExceptionHelper.ShowException(ex);
            }
        }
        #endregion

        #region DoSending
        private void DoSending(IAsyncResult ar)
        {
            TcpClient client = null;
            try
            {
                client = (TcpClient)ar.AsyncState;
                NetworkStream networkStream = client.GetStream();
                networkStream.EndWrite(ar);

                OnDataSent?.Invoke(this);
            }
            catch (Exception ex)
            {
                client.Close();
                ExceptionHelper.ShowException(ex);
            }
        }
        #endregion

        #region HandleReceive
        private static void HandleReceive(TcpClient client, string data)
        {
            LogHelper.ShowLog("Receive\t\t" + DateTime.Now.ToString() + "\t" +
                client.GetHashCode().ToString() + "\t" + data);
        }
        #endregion


        private string TraiteChaineJudo(string s)
        {
            string chaine = s.Split(new string[] { "\n<EOF>" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            //string chaine = s.Replace("\n<EOF>", "");
            return chaine;
        }
    }
}
