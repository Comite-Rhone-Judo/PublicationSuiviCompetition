#nullable enable
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            var client = new ClientGenerique("192.168.1.50", 9000, "</msgJudo>");

            // Assert
            client.IP.Should().Be("192.168.1.50");
            client.Port.Should().Be(9000);
            client.EndMsgFlag.Should().Be("</msgJudo>");
            client.IsConnected.Should().BeFalse();
        }

        [Fact(Timeout = 5000)]
        public async Task Connect_ServeurActif_DeclencheEvenementOnConnection()
        {
            // Arrange
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = new ClientGenerique("127.0.0.1", port, "</EOF>");
            var tcs = new TaskCompletionSource<bool>();

            client.OnConnection += (sender) => tcs.TrySetResult(true);

            try
            {
                // Act
                client.Connect();

                // Assert
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(-1, cts.Token));
                completedTask.Should().Be(tcs.Task, "L'événement OnConnection doit être déclenché.");
                client.IsConnected.Should().BeTrue();
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        [Fact(Timeout = 5000)]
        public async Task Connect_ServeurInexistant_NePlantePasEtGereLeTimeout()
        {
            // Arrange
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var client = new ClientGenerique("127.0.0.1", 55555, "</EOF>");
            bool eventFired = false;

            client.OnConnection += (s) => eventFired = true;

            // Act
            Action act = () => client.Connect();

            // Assert
            act.Should().NotThrow("La méthode Connect est fire-and-forget et capture ses exceptions en interne.");

            await Task.Delay(1500, cts.Token);

            eventFired.Should().BeFalse("L'événement OnConnection ne doit pas se déclencher.");
            client.IsConnected.Should().BeFalse();
        }

        [Fact(Timeout = 5000)]
        public async Task Stop_DeconnecteLeClient_EtDeclencheOnEndConnection()
        {
            // Arrange
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = new ClientGenerique("127.0.0.1", port, "</EOF>");
            var tcsConnection = new TaskCompletionSource<bool>();
            var tcsEndConnection = new TaskCompletionSource<bool>();

            client.OnConnection += (s) => tcsConnection.TrySetResult(true);
            client.OnEndConnection += (s) => tcsEndConnection.TrySetResult(true);

            try
            {
                client.Connect();
                await tcsConnection.Task;
                client.IsConnected.Should().BeTrue();

                // Act
                client.Stop();

                // Assert
                var completedTask = await Task.WhenAny(tcsEndConnection.Task, Task.Delay(-1, cts.Token));
                completedTask.Should().Be(tcsEndConnection.Task, "L'arrêt doit déclencher OnEndConnection.");
                client.IsConnected.Should().BeFalse();
            }
            finally
            {
                listener.Stop();
            }
        }

        [Fact(Timeout = 5000)]
        public async Task Write_ConnexionEtablie_EnvoieLesDonneesAvecLeMarqueurDeFin()
        {
            // Arrange
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = new ClientGenerique("127.0.0.1", port, "</EOF>");
            var tcsConnection = new TaskCompletionSource<bool>();
            var tcsSent = new TaskCompletionSource<bool>();

            client.OnConnection += (s) => tcsConnection.TrySetResult(true);
            client.OnDataSent += (s) => tcsSent.TrySetResult(true);

            try
            {
                client.Connect();
                await tcsConnection.Task;

                using var serverSideClient = await listener.AcceptTcpClientAsync(cts.Token);
                var stream = serverSideClient.GetStream();

                // Act
                client.Write("<Judo>Test</Judo>");

                // Assert
                await tcsSent.Task;

                byte[] buffer = new byte[1024];

                // Correction : Utilisation de Memory<byte> pour la lecture dans les tests
                int bytesRead = await stream.ReadAsync(buffer.AsMemory(), cts.Token);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                receivedData.Should().Be("<Judo>Test</Judo>\n<EOF>", "La méthode Write doit automatiquement ajouter la balise de fin de flux.");
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        [Fact(Timeout = 5000)]
        public async Task ReadLoop_DonneesCompletes_DeclencheOnDataRecieve()
        {
            // Arrange
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var endTag = "</Msg>";
            var client = new ClientGenerique("127.0.0.1", port, endTag);
            var tcsConnection = new TaskCompletionSource<bool>();
            var tcsDataReceived = new TaskCompletionSource<string>();

            client.OnConnection += (s) => tcsConnection.TrySetResult(true);
            client.OnDataRecieve += (s, data) => tcsDataReceived.TrySetResult(data);

            try
            {
                client.Connect();
                await tcsConnection.Task;

                using var serverSideClient = await listener.AcceptTcpClientAsync(cts.Token);
                var stream = serverSideClient.GetStream();

                // Act
                byte[] dataToSend = Encoding.UTF8.GetBytes($"<Msg>Coucou</Msg>\n<EOF>");

                // Correction : Utilisation de ReadOnlyMemory<byte> pour l'écriture dans les tests
                await stream.WriteAsync(dataToSend.AsMemory(), cts.Token);
                await stream.FlushAsync(cts.Token);

                // Assert
                var completedTask = await Task.WhenAny(tcsDataReceived.Task, Task.Delay(-1, cts.Token));
                completedTask.Should().Be(tcsDataReceived.Task, "Les données envoyées par le serveur doivent être interceptées.");

                // Correction : Suppression du '.Result' bloquant, utilisation d'un 'await' direct
                string receivedMessage = await tcsDataReceived.Task;
                receivedMessage.Should().Be("<Msg>Coucou</Msg>");
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        [Fact(Timeout = 5000)]
        public async Task ReadLoop_DonneesFragmentees_ReconstruitLeMessageEtDeclencheOnDataRecieve()
        {
            // Arrange
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = new ClientGenerique("127.0.0.1", port, "</Data>");
            var tcsConnection = new TaskCompletionSource<bool>();
            var tcsDataReceived = new TaskCompletionSource<string>();

            client.OnConnection += (s) => tcsConnection.TrySetResult(true);
            client.OnDataRecieve += (s, data) => tcsDataReceived.TrySetResult(data);

            try
            {
                client.Connect();
                await tcsConnection.Task;

                using var serverSideClient = await listener.AcceptTcpClientAsync(cts.Token);
                var stream = serverSideClient.GetStream();

                // Act
                // Correction : Utilisation de ReadOnlyMemory<byte> pour l'écriture fragmentée
                await stream.WriteAsync(Encoding.UTF8.GetBytes("<Data>De").AsMemory(), cts.Token);
                await Task.Delay(50, cts.Token);
                await stream.WriteAsync(Encoding.UTF8.GetBytes("but...F").AsMemory(), cts.Token);
                await Task.Delay(50, cts.Token);
                await stream.WriteAsync(Encoding.UTF8.GetBytes("in</Data>\n<EOF>").AsMemory(), cts.Token);
                await stream.FlushAsync(cts.Token);

                // Assert
                var completedTask = await Task.WhenAny(tcsDataReceived.Task, Task.Delay(-1, cts.Token));
                completedTask.Should().Be(tcsDataReceived.Task, "Le client doit recoller les fragments TCP avant de déclencher l'événement.");

                // Correction : Suppression du '.Result' bloquant
                string receivedMessage = await tcsDataReceived.Task;
                receivedMessage.Should().Be("<Data>Debut...Fin</Data>");
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }
    }
}