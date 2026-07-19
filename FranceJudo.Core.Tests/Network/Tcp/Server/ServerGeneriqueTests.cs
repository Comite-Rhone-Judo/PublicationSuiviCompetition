#nullable enable
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Tcp.Server;

namespace FranceJudo.Core.Tests.Network.Tcp.Server
{
    public class ServerGeneriqueTests
    {
        public ServerGeneriqueTests()
        {
            // Initialisation de l'encoding pour pallier l'ordre d'exécution aléatoire des tests xUnit
            FranceJudo.Core.IO.FileSystemHelper.TheEncoding ??= Encoding.UTF8;
        }

        private int GetFreePort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        [Fact]
        public void Constructeur_InitialiseLesProprietes()
        {
            var server = new ServerGenerique(IPAddress.Loopback, 9050, "</serverjudo>");
            server.EndMsgTag.Should().Be("</serverjudo>", "La balise métier (EndMsgTag) doit être isolée du délimiteur réseau.");
        }

        [Fact(Timeout = 10000)]
        public async Task Start_AccepteClient_EtDeclencheEvenement()
        {
            int port = GetFreePort();
            var server = new ServerGenerique(IPAddress.Loopback, port, "</serverjudo>");
            var tcsConnection = new TaskCompletionSource<TcpClient>();

            server.OnConnection += (s, client) => tcsConnection.TrySetResult(client);

            try
            {
                server.Start();

                using var testClient = new TcpClient();
                // Utilisation de CancellationToken.None pour satisfaire les analyseurs stricts
                await testClient.ConnectAsync("127.0.0.1", port, TestContext.Current.CancellationToken);

                var completed = await Task.WhenAny(tcsConnection.Task, Task.Delay(3000, TestContext.Current.CancellationToken));
                completed.Should().Be(tcsConnection.Task, "Le serveur n'a pas détecté la connexion (Timeout de 3s atteint).");
            }
            finally
            {
                server.Stop();
            }
        }

        [Fact(Timeout = 10000)]
        public async Task ReceiveData_TrameValide_RespecteLeProtocoleMetier()
        {
            int port = GetFreePort();
            string metierTag = "</serverjudo>";
            var server = new ServerGenerique(IPAddress.Loopback, port, metierTag);
            var tcsReceive = new TaskCompletionSource<string>();

            server.OnDataRecieve += (s, c, data) => tcsReceive.TrySetResult(data);

            try
            {
                server.Start();

                using var testClient = new TcpClient();
                await testClient.ConnectAsync("127.0.0.1", port, TestContext.Current.CancellationToken);

                // Temporisation pour s'assurer que le serveur a bien branché ses événements et appelé StartRead()
                await Task.Delay(100, TestContext.Current.CancellationToken);

                var stream = testClient.GetStream();

                string rawPayload = $"<serverjudo>DATA_JUDO{metierTag}\n<EOF>";
                byte[] bytes = Encoding.UTF8.GetBytes(rawPayload);

                // Utilisation stricte de la surcharge (byte[], offset, count, CancellationToken)
                await stream.WriteAsync(bytes, CancellationToken.None);
                await stream.FlushAsync(CancellationToken.None);

                var delayTask = Task.Delay(3000, TestContext.Current.CancellationToken);
                var completed = await Task.WhenAny(tcsReceive.Task, delayTask);

                completed.Should().Be(tcsReceive.Task, "L'événement OnDataRecieve n'a pas été déclenché (Crash serveur potentiel ou erreur de parsing).");

                string receivedData = await tcsReceive.Task;
                receivedData.Should().Be($"<serverjudo>DATA_JUDO{metierTag}");
            }
            finally
            {
                server.Stop();
            }
        }

        [Fact(Timeout = 10000)]
        public async Task Write_EnvoieDonnees_AjouteUniquementLeDelimiterReseau()
        {
            int port = GetFreePort();
            var server = new ServerGenerique(IPAddress.Loopback, port, "</serverjudo>");
            var tcsConnection = new TaskCompletionSource<TcpClient>();

            server.OnConnection += (s, c) => tcsConnection.TrySetResult(c);

            try
            {
                server.Start();

                using var testClient = new TcpClient();
                await testClient.ConnectAsync("127.0.0.1", port, TestContext.Current.CancellationToken);

                var completedConnection = await Task.WhenAny(tcsConnection.Task, Task.Delay(3000, TestContext.Current.CancellationToken));
                completedConnection.Should().Be(tcsConnection.Task, "La connexion n'a pas pu être établie.");
                var serverSideClient = await tcsConnection.Task;

                // Act
                server.Write(serverSideClient, "<msg>test</msg>");

                // Assert
                byte[] buffer = new byte[1024];

                // Utilisation stricte de la surcharge (byte[], offset, count, CancellationToken)
                int bytesRead = await testClient.GetStream().ReadAsync(buffer, CancellationToken.None);
                string receivedByClient = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                receivedByClient.Should().Be("<msg>test</msg>\n<EOF>");
            }
            finally
            {
                server.Stop();
            }
        }

        [Fact(Timeout = 10000)]
        public async Task Integration_CycleComplet_AppPublication()
        {
            int port = GetFreePort();
            string xmlTag = "</serverjudo>";

            var server = new ServerGenerique(IPAddress.Loopback, port, xmlTag);
            var client = new FranceJudo.Core.Network.Tcp.Client.ClientGenerique("127.0.0.1", port, xmlTag);

            var tcsServerReceived = new TaskCompletionSource<string>();
            server.OnDataRecieve += (s, c, data) => tcsServerReceived.TrySetResult(data);

            try
            {
                server.Start();
                client.Connect();

                await Task.Delay(300, TestContext.Current.CancellationToken);

                string payload = $"<serverjudo>ACTION:PUBLISH_RESULTS{xmlTag}";
                client.Write(payload);

                var completed = await Task.WhenAny(tcsServerReceived.Task, Task.Delay(3000, TestContext.Current.CancellationToken));
                completed.Should().Be(tcsServerReceived.Task, "Le flux complet Client > Serveur a échoué.");

                string dataRecue = await tcsServerReceived.Task;
                dataRecue.Should().EndWith(xmlTag);
                dataRecue.Should().Contain("PUBLISH_RESULTS");
            }
            finally
            {
                try { client.Stop(); } catch { }
                try { server.Stop(); } catch { }
            }
        }
    }
}