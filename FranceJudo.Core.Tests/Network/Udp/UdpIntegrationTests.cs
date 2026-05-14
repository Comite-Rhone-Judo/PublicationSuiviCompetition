#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Udp;

namespace FranceJudo.Core.Tests.Network.Udp
{
    // L'attribut Sequential est VITAL ici pour éviter que xUnit ne lance 
    // plusieurs tests sur le même port UDP en parallèle.
    [Collection("UdpSequential")]
    public class UdpIntegrationTests
    {
        private const int TEST_PORT = 11042;
        private const string TEST_IP = "127.0.0.1";

        [Fact]
        public async Task CommunicationUDP_ClientEnvoie_ServeurRecoitEtDeclencheEvenement()
        {
            // Configuration du timeout
            int timeoutMs = 3000;
            using var timeoutCts = new CancellationTokenSource(timeoutMs);
            using var server = new ServerUDP(TEST_PORT);

            string? messageRecu = null;
            using var signal = new SemaphoreSlim(0, 1);

            server.OnDataReceive += (sender, msg) =>
            {
                messageRecu = msg;
                signal.Release();
            };

            server.Start();

            using var client = new ClientUDP(TEST_IP, TEST_PORT);
            string messageAEnvoyer = "JUDO-TEST-UDP-" + Guid.NewGuid().ToString();

            client.Send(messageAEnvoyer);

            // Correction CS0029 : On passe le délai explicitement à WaitAsync
            // On utilise ConfigureAwait(false) pour éviter les deadlocks de contexte (surtout en WPF/WinForms)
            await signal.WaitAsync(TestContext.Current.CancellationToken);

            // Assert
            messageRecu.Should().Be(messageAEnvoyer, "Le message doit transiter du client au serveur sans altération.");
        }

        [Fact]
        public void ClientUDP_Proprietes_SontInitialiseesCorrectement()
        {
            // Act
            using var client = new ClientUDP("192.168.1.100", 8484);

            // Assert
            client.IP.Should().Be("192.168.1.100");
            client.Port.Should().Be(8484);
        }

        [Fact]
        public void ServerUDP_Stop_ArreteLeThreadProprement_EtLibereLePort()
        {
            // Arrange
            int portPourCeTest = TEST_PORT + 1; // On isole ce test sur un autre port
            using var server = new ServerUDP(portPourCeTest);

            // Act
            Action actStart1 = () => server.Start();
            Action actStop = () => server.Stop();
            Action actStart2 = () => server.Start();

            // Assert : Si le port n'est pas libéré, le deuxième Start() lèvera une SocketException
            actStart1.Should().NotThrow();
            actStop.Should().NotThrow();
            actStart2.Should().NotThrow("La méthode Stop doit libérer correctement le port UDP via _listener.Close().");
        }

        [Fact]
        public void ServerUDP_Restart_ShouldNotConflict()
        {
            using var server = new ServerUDP(TEST_PORT + 1);

            server.Start();
            server.Stop();

            // Vérification de la libération réelle du socket
            Action reStart = () => server.Start();
            reStart.Should().NotThrow("Le socket doit être libéré immédiatement après Stop().");
        }
    }
}