using System;
using System.Net.Sockets;
using Tools.Logging;
using Tools.Files;

namespace Tools.UDP_Tools
{
    /// <summary>
    /// Outils de client UDP (Outils vidéo Fédération)
    /// </summary>
    public class ClientUDP
    {
        int _port = 8484;
        string _ip = "127.0.0.1";
        UdpClient _udpClient = null;

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
        /// Contructeur
        /// </summary>
        /// <param name="hostNameOrAddress"></param>
        /// <param name="port"></param>
        public ClientUDP(string hostNameOrAddress, int port)
        {
            _ip = hostNameOrAddress;
            _port = port;

            //if(!string.IsNullOrWhiteSpace(hostNameOrAddress))
            //{
            _udpClient = new UdpClient();
            try
            {
                _udpClient.Connect(_ip, _port);
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }

            //}           
        }

        /// <summary>
        /// Envoie de données
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            try
            {
                // LogTools.Trace("[UDP]    " + message, LogTools.Level.INFO);
                LogTools.Info("[UDP]    " + message);

                //UdpClient udpClient = new UdpClient();
                //udpClient.Connect(_ip, _port);

                Byte[] senddata = FileAndDirectTools.TheEncoding.GetBytes(message);
                _udpClient.Send(senddata, senddata.Length);

                //_udpClient.Close();
            }
            catch
            {
                try
                {
                    _udpClient = new UdpClient();
                    _udpClient.Connect(_ip, _port);

                    Byte[] senddata = FileAndDirectTools.TheEncoding.GetBytes(message);
                    _udpClient.Send(senddata, senddata.Length);
                }
                catch (Exception ex2)
                {
                    LogTools.Error(ex2);
                }
            }
        }

    }
}
