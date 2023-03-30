using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Tools.Outils;

namespace Tools.TCP_Tools.Server
{
    /// <summary>
    /// Listener Helper.
    /// </summary>
    public class ListenerHelper
    {
        #region field

        /// <summary>
        /// Fonction délégué de reception de données
        /// </summary>
        /// <param name="client"></param>
        public delegate void ReceiveData(TcpClient client);
        public static ReceiveData delReceive = null;

        #endregion

        #region method

        #region CreateTcpListener
        /// <summary>
        /// Création du Listener (server)
        /// </summary>
        /// <param name="port">port sur lequel le server écoute</param>
        /// <returns></returns>
        public static TcpListener CreateTcpListener(int port)
        {
            return new TcpListener(System.Net.IPAddress.Any, port);
        }

        #endregion

        #region StartListen

        /// <summary>
        /// Start Listening
        /// </summary>
        public static void StartListening(ref TcpListener tcpListener, int port, AsyncCallback asyncCallback)
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
            else
            {
                tcpListener = CreateTcpListener(port);
            }

            ListenerAndClient objListenerAndClient = new ListenerAndClient();

            objListenerAndClient.Listener = tcpListener;

            try
            {
                tcpListener.Start();
            }
            catch (SocketException ex)
            {
                ExceptionHelper.ShowException(ex);
                //return;
            }

            tcpListener.BeginAcceptTcpClient(
                asyncCallback, objListenerAndClient);

        }
        #endregion

        #region StopListen

        /// <summary>
        /// Stop Listen
        /// </summary>
        public static void StopListening(ref TcpListener tcpListener)
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
        }

        #endregion

        #region SendData

        /// <summary>
        /// Envoie de données
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="sendMessage">données</param>
        /// <param name="asyncCallback">fonction à appliquer</param>
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
                client.GetStream().BeginWrite(data, 0, data.Length, asyncCallback, client);

                //using (TimedLock.Lock(client.GetStream())
                //{
                //    networkStream = client.GetStream();
                //    networkStream.WriteTimeout = 500;
                //    networkStream.Write(data, 0, data.Length);
                //}

                return true;
            }
            catch (Exception ex)
            {
                ExceptionHelper.ShowException(ex);
                //throw ex;
            }

            return false;
        }

        #endregion

        #endregion

        #region struct

        /// <summary>
        /// Structure d'association server-client
        /// </summary>
        public struct ListenerAndClient
        {
            /// <summary>
            /// le server
            /// </summary>
            public TcpListener Listener;

            /// <summary>
            /// Le client
            /// </summary>
            public TcpClient Client;
        }

        #endregion
    }
}
