#nullable enable
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Tcp.Client;

namespace FranceJudo.Core.Tests.Network.Tcp.Client
{
    public class ClientGeneriqueTests
    {
        [Fact]
        public void Constructeur_InitialiseLesProprietesCorrectement()
        {
            // Arrange & Act
            var client = new ClientGenerique("192.168.1.50", 9000, "</MsgJudo>");

            // Assert
            client.IP.Should().Be("192.168.1.50", "L'IP doit être assignée par le constructeur.");
            client.Port.Should().Be(9000, "Le port doit être assigné par le constructeur.");
            client.EndMsgFlag.Should().Be("</MsgJudo>", "Le tag de fin de message doit être assigné par le constructeur.");
            client.IsConnected.Should().BeFalse("Un client fraîchement instancié ne doit pas être connecté.");
        }

        [Fact(Timeout = 5000)]
        public async Task Connect_ServeurActif_DeclencheEvenementOnConnection()
        {
            // Arrange
            var ct = TestContext.Current.CancellationToken;

            // On crée un serveur local éphémère sur un port disponible dynamiquement
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = new ClientGenerique("127.0.0.1", port, "</EOF>");
            var tcs = new TaskCompletionSource<bool>();

            client.OnConnection += (sender) =>
            {
                tcs.TrySetResult(true);
            };

            try
            {
                // Act : Utilisation de la méthode sans paramètre
                client.Connect();

                // Assert : On attend le déclenchement du callback interne DoConnecting
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(2000, ct));
                completedTask.Should().Be(tcs.Task, "L'événement OnConnection doit être déclenché après une connexion réussie.");
                client.IsConnected.Should().BeTrue();
            }
            finally
            {
                client.Stop(); //
                listener.Stop();
            }
        }

        [Fact(Timeout = 5000)]
        public async Task Write_ConnexionEtablie_DeclencheEvenementOnDataSent()
        {
            // Arrange
            var ct = TestContext.Current.CancellationToken;
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = new ClientGenerique("127.0.0.1", port, "</EOF>");
            var tcsConnection = new TaskCompletionSource<bool>();
            var tcsSent = new TaskCompletionSource<bool>();

            client.OnConnection += (s) => tcsConnection.TrySetResult(true);
            client.OnDataSent += (s) => tcsSent.TrySetResult(true); //

            try
            {
                // Connexion préalable
                client.Connect();
                await tcsConnection.Task;

                // Côté serveur, on accepte le client pour ouvrir le flux
                using var serverSideClient = await listener.AcceptTcpClientAsync(ct);

                // Act : Utilisation de la méthode Write
                client.Write("<Judo>Test</Judo>");

                // Assert : On attend le callback DoSending
                var completedTask = await Task.WhenAny(tcsSent.Task, Task.Delay(2000, ct));
                completedTask.Should().Be(tcsSent.Task, "L'événement OnDataSent doit être levé après un envoi réussi.");
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        [Fact(Timeout = 3000)]
        public async Task Connect_ServeurInexistant_NePlantePas()
        {
            // Arrange
            var client = new ClientGenerique("127.0.0.1", 55555, "</EOF>");
            bool eventFired = false;

            client.OnConnection += (s) => eventFired = true;

            // Act : Port fermé
            Action act = () => client.Connect();

            // Assert
            act.Should().NotThrow("La méthode Connect encapsule l'appel dans un try/catch (via ClientHelper).");

            var ct = TestContext.Current.CancellationToken;
            try { await Task.Delay(500, ct); } catch (OperationCanceledException) { }

            eventFired.Should().BeFalse("L'événement OnConnection ne doit pas être déclenché si la connexion échoue.");
            client.IsConnected.Should().BeFalse();
        }
    }
}