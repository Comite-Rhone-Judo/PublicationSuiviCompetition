#nullable enable
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Tcp;

namespace FranceJudo.Core.Tests.Network.Tcp
{
    public class TcpClientExtensionTests
    {
        [Fact]
        public void GetAddressClient_ClientNul_RetourneUnknown()
        {
            // Arrange
            TcpClient? client = null;

            // Act
            string result = client.GetAddressClient();

            // Assert
            result.Should().Be("Unknown_0", "La méthode d'extension doit résister à une instance nulle de TcpClient.");
        }

        [Fact]
        public void GetAddressClient_ClientNonConnecte_NePlantePas()
        {
            // Arrange
            using var client = new TcpClient(); // Nouveau client, aucun RemoteEndPoint

            // Act
            string result = client.GetAddressClient();

            // Assert
            result.Should().Be("Unknown_0", "Un client non connecté n'a pas de RemoteEndPoint, la méthode doit retourner un fallback au lieu de crasher sur null.ToString().");
        }

        [Fact]
        public async Task GetAddressClient_ClientConnecte_RetourneIpEtPortFormatte()
        {
            // Arrange : Création d'un mini-serveur éphémère (port 0 = port dynamique libre)
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int serverPort = ((IPEndPoint)listener.LocalEndpoint).Port;

            using var client = new TcpClient();

            try
            {
                // Act : On connecte le client au serveur
                await client.ConnectAsync(IPAddress.Loopback, serverPort, TestContext.Current.CancellationToken);

                string result = client.GetAddressClient();

                // Assert
                // On vérifie que la conversion a bien nettoyé le ::ffff: si présent
                result.Should().MatchRegex(@"^127\.0\.0\.1_\d+$", "L'adresse IP doit être formatée en IPv4 pur, suivie d'un underscore et du port.");

                // Le port distant assigné par l'OS au client est aléatoire, on vérifie juste le format IP_PORT
                var parts = result.Split('_');
                parts.Should().HaveCount(2, "Le format attendu est '{ipAddr}_{port}'.");
                int.TryParse(parts[1], out _).Should().BeTrue("La deuxième partie après l'underscore doit être un numéro de port valide.");
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}