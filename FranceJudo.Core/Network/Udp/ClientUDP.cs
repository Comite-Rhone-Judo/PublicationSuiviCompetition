#nullable enable
using System;
using System.Net.Sockets;
using System.Text;
using FranceJudo.Core.Logging;
// Note : J'utilise Encoding.UTF8 au lieu de ton FileSystemHelper.TheEncoding pour être standard.

namespace FranceJudo.Core.Network.Udp
{
    public class ClientUDP : IDisposable
    {
        public string IP { get; }
        public int Port { get; }

        private UdpClient? _udpClient;

        public ClientUDP(string hostNameOrAddress, int port)
        {
            IP = hostNameOrAddress;
            Port = port;
            InitClient();
        }

        private void InitClient()
        {
            _udpClient?.Dispose();
            _udpClient = new UdpClient();
            try
            {
                _udpClient.Connect(IP, Port);
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex, $"[UDP CLIENT] Impossible de configurer la connexion vers {IP}:{Port}");
                // On laisse l'exception remonter, le métier doit savoir s'il y a un souci DNS/IP
                throw;
            }
        }

        public void Send(string message)
        {
            if (_udpClient == null) throw new ObjectDisposedException(nameof(ClientUDP));

            try
            {
                LogTools.Logger?.Info($"[UDP CLIENT] Envoi : {message}");
                byte[] senddata = Encoding.UTF8.GetBytes(message);
                _udpClient.Send(senddata, senddata.Length);
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Warn(ex, "[UDP CLIENT] Échec de l'envoi. Tentative de reconnexion...");

                // Mécanisme de résilience : on recrée et on retente UNE fois.
                InitClient();
                byte[] senddata = Encoding.UTF8.GetBytes(message);
                _udpClient.Send(senddata, senddata.Length); // Si ça recrashe ici, ça remonte à l'appelant (C'EST VOULU)
            }
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
            _udpClient = null;
        }
    }
}