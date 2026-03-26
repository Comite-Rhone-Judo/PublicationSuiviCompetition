using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Tcp
{
    public static class TcpClientExtension
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetAddressClient(this TcpClient client)
        {
            try
            {
                // return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                // Ajoute le remote port pour pouvoir distinguer deux clients lancés sur le meme poste
                string ipAddr = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                string port = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
                return string.Format("{0}_{1}", ipAddr, port);
            }
            catch
            {
                return client.Client.RemoteEndPoint.ToString();
            }
        }
    }
}
