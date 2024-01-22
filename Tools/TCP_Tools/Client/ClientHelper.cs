using System;
using System.Net.Sockets;
using Tools.Outils;

namespace Tools.TCP_Tools.Client
{
    /// <summary>
    /// Client Helper.
    /// </summary>
    public class ClientHelper
    {
        #region field

        /// <summary>
        /// Fonction déléguée de réception de données
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public delegate void ReceiveData(TcpClient client, string data);
        public static ReceiveData delReceiveData = null;



        #endregion

        #region Connect Socket

        /// <summary>
        /// Connection d'un socket TCP
        /// </summary>
        /// <param name="client">le client</param>
        /// <param name="remoteHost">IP server</param>
        /// <param name="remotePort">Port server</param>
        /// <param name="asyncCallback">fonction à appeler lorsque la connection est complète</param>
        public static void ConnectSocket(
           ref TcpClient client,
           string remoteHost,
           int remotePort,
            AsyncCallback asyncCallback)
        {

            try
            {
                #region main process

                if (client == null || client.Client == null)
                {
                    client = new TcpClient();
                    client.NoDelay = true;
                    client.LingerState = new LingerOption(true, 20);
                }

                if (client.Connected)
                {
                    LogHelper.ShowLog("Re-Connect\t" + DateTime.Now.ToString() + "\t" +
                        "close current connect\t" + client.GetHashCode().ToString());

                    if (client.GetStream() != null)
                    {
                        client.GetStream().Close();
                    }

                    client.Close();

                    client = new TcpClient();
                    client.NoDelay = true;
                    client.LingerState = new LingerOption(true, 20);

                    LogHelper.ShowLog("Create Client\t" + DateTime.Now.ToString() + "\t" +
                        client.GetHashCode().ToString());
                }

                try
                {

                    IAsyncResult result = client.BeginConnect(remoteHost, remotePort, new AsyncCallback(asyncCallback), client);

                    bool success = result.AsyncWaitHandle.WaitOne(1000, true);
                    if (!success)
                    {
                        client.Close();
                    }
                    //client.Connect(remoteHost, remotePort);
                }
                catch (ObjectDisposedException)
                {
                    client = new TcpClient();
                    client.NoDelay = true;
                    client.LingerState = new LingerOption(true, 20);

                    client.Connect(remoteHost, remotePort);

                    LogHelper.ShowLog("Create Client\t" + DateTime.Now.ToString() + "\t" +
                        client.GetHashCode().ToString());
                }

                #endregion

                //return true;
            }
            catch (SocketException ex)
            {
                if (client != null)
                {
                    client.Close();
                }

                ExceptionHelper.ShowException(ex);
            }
            catch (Exception ex)
            {
                if (client != null)
                {
                    client.Close();
                }

                ExceptionHelper.ShowException(ex);
            }

            //return false;
        }
        #endregion

        #region Close
        /// <summary>
        /// Ferme le client
        /// </summary>
        /// <param name="client"></param>
        public static void Close(TcpClient client)
        {
            try
            {
                if (client == null)
                {
                    return;
                }

                if (client.GetStream() != null)
                {
                    client.GetStream().Dispose();
                    if (client.Client.Connected)
                    {
                        client.GetStream().Close();
                    }
                }

                client.Close();
                System.Threading.Thread.Sleep(100);

            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
            }
        }
        #endregion

        #region SendData
        /// <summary>
        /// Envoie des données
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="sendMessage">message</param>
        /// <param name="asyncCallback">fonction après</param>
        /// <returns></returns>
        public static bool SendData(
            TcpClient client,
            string sendMessage, AsyncCallback asyncCallback)
        {
            sendMessage += "\n<EOF>";
            Byte[] data = null;

            try
            {

                data = FileAndDirectTools.TheEncoding.GetBytes(sendMessage);

                //XDocument doc = Common.CreateDocument(ServerCommandEnum.NonTraite);
                //string data1 = doc.ToString(SaveOptions.None);
                //Byte[] data2 = System.Text.Encoding.UTF8.GetBytes(data1);
                //client.GetStream().Write(data2, 0, data2.Length);


                client.GetStream().BeginWrite(data, 0, data.Length, asyncCallback, client);

                return true;
            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
                throw;
            }
            //return false;

        }

        #endregion
    }
}
