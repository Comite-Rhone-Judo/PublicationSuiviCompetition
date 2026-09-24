#nullable enable
using System.Net;
using System.Net.Sockets;

namespace FranceJudo.Core.Network.Tcp
{
    public static class TcpClientExtension
    {
        public static string GetAddressClient(this TcpClient? client)
        {
            var remoteEndPoint = client?.Client?.RemoteEndPoint;
            if (remoteEndPoint == null)
            {
                return "Unknown_0";
            }

            if (remoteEndPoint is IPEndPoint ipEndPoint)
            {
                var ip = ipEndPoint.Address;

                // CORRECTION : Si l'adresse est un IPv4 encapsulé dans un IPv6 (ex: ::ffff:192.168.1.10)
                // On la convertit en IPv4 pur pour la lisibilité des logs.
                if (ip.IsIPv4MappedToIPv6)
                {
                    ip = ip.MapToIPv4();
                }

                return $"{ip}_{ipEndPoint.Port}";
            }

            return remoteEndPoint.ToString() ?? "Unknown_0";
        }
    }
}