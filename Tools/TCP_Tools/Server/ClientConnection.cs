using System;
using System.IO;
using System.Net.Sockets;
using Tools.Enum;
using Tools.Files;

namespace Tools.TCP_Tools.Server
{
    /// <summary>
    /// Classe de connection de client TCP
    /// </summary>
    public class ClientConnection
    {
        /// <summary>
        /// Fonction délégué de reception de données
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="Data">Données reçues </param>
        public delegate void MessageReceive(ClientConnection sender, string Data);

        /// <summary>
        /// Fonction délégué de fermeture de connection
        /// </summary>
        /// <param name="sender"></param>
        public delegate void RemoteHostClose(ClientConnection sender);


        #region field

        const int READ_BUFFER_SIZE = 10240;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];

        /// <summary>
        /// événement de réception de données
        /// </summary>
        public event MessageReceive OnMessageReceived;

        /// <summary>
        /// événement de fermeture de connection client
        /// </summary>
        public event RemoteHostClose OnRemoteHostClosed;

        private TcpClient _Client;
        private string chaine = "";

        /// <summary>
        /// Le client
        /// </summary>
        public TcpClient Client
        {
            get
            {
                return _Client;
            }
        }

        #endregion

        #region method

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="client">client</param>
        public ClientConnection(TcpClient client)
        {
            try
            {
                _Client = client;

                client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                    new AsyncCallback(StreamReceiver), client);
            }
            catch (NullReferenceException ex)
            {
                ExceptionHelper.ShowException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                client.Close();
                ExceptionHelper.ShowException(ex);
            }
            catch (IOException ex)
            {
                client.Close();
                ExceptionHelper.ShowException(ex);
            }
        }

        /// <summary>
        /// StreamReceiver
        /// </summary>
        /// <param name="ar">object</param>
        private void StreamReceiver(IAsyncResult ar)
        {
            TcpClient client = null;
            int intBytesRead = 0;
            string strReceiveData = string.Empty;
            NetworkStream objNetworkStream = null;

            try
            {
                client = (TcpClient)ar.AsyncState;

                if (!client.Connected)
                {
                    OnRemoteHostClosed(this);
                    client.Close();
                    LogHelper.ShowLog("", client, LogHelper.TypeLog.Close);

                    return;
                }

                try
                {
                    objNetworkStream = client.GetStream();
                }
                catch (ObjectDisposedException ex)
                {
                    OnRemoteHostClosed(this);
                    client.Close();
                    ExceptionHelper.ShowException(ex);
                    return;
                }
                catch (InvalidOperationException ex)
                {
                    OnRemoteHostClosed(this);
                    client.Close();
                    ExceptionHelper.ShowException(ex);
                    return;
                }

                try
                {
                    lock (objNetworkStream)
                    {
                        intBytesRead = objNetworkStream.EndRead(ar);
                    }
                }
                catch (IOException ex)
                {
                    OnRemoteHostClosed(this);
                    client.Close();
                    ExceptionHelper.ShowException(ex);
                    return;

                }

                if (intBytesRead > 0)
                {
                    try
                    {
                        strReceiveData = FileAndDirectTools.TheEncoding.GetString(readBuffer, 0, intBytesRead);

                        //string data = Encoding.UTF8.GetString(client.Buffer, 0, read);
                        chaine += strReceiveData;

                        if (chaine.IndexOf("\n<EOF>") != -1 && OnMessageReceived != null)
                        {
                            string tmp = "";
                            foreach (string data in chaine.Split(new string[] { "\n<EOF>" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (data.EndsWith("</" + ConstantXML.ServerJudo + ">"))
                                {
                                    OnMessageReceived(this, data);
                                }
                                else
                                {
                                    tmp = data;
                                }
                            }
                            chaine = tmp;
                        }
                    }
                    catch (IOException ex)
                    {
                        ExceptionHelper.ShowException(ex);
                    }
                }
                else
                {
                    OnRemoteHostClosed(this);

                    if (objNetworkStream != null)
                    {
                        objNetworkStream.Close();
                    }

                    client.Close();
                    return;

                }

                if (!client.Connected)
                {
                    OnRemoteHostClosed(this);
                    client.Close();
                    LogHelper.ShowLog("", client, LogHelper.TypeLog.ClientClose);
                    //LogHelper.ShowLog("client close\t" + DateTime.Now.ToString() + "\t" + client.GetHashCode().ToString());
                    return;
                }

                try
                {
                    lock (objNetworkStream)
                    {
                        objNetworkStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                            new AsyncCallback(StreamReceiver), client);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    OnRemoteHostClosed(this);
                    client.Close();
                    ExceptionHelper.ShowException(ex);
                }
                catch (IOException ex)
                {
                    OnRemoteHostClosed(this);
                    client.Close();
                    ExceptionHelper.ShowException(ex);
                }

            }
            catch (Exception ex)
            {
                OnRemoteHostClosed(this);
                client.Close();
                ExceptionHelper.ShowException(ex);
            }
        }

        #endregion
    }
}
