using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tools.UDP_Tools
{
    public class ServerUDP
    {

        private int listenPort = 11000;
        // public delegate void OnDataReceiveHandler(object sender, string donnees);
        // public event OnDataReceiveHandler OnDataReceive;

        public ServerUDP(int port)
        {
            listenPort = port;
            Thread thdUDPServer = new Thread(new ThreadStart(StartListener));
            thdUDPServer.Start();
        }

        /// <summary>
        /// Lance le server UDP
        /// </summary>
        private void StartListener()
        {
            bool done = false;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                        groupEP.ToString(),
                        Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
