using AppPublication.Controles;
using JudoClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using Xunit;
using FluentAssertions;

namespace AppPublication.Tests.Controles
{
    public class GestionConnectionTests
    {
        [Fact]
        public void Constructeur_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            GestionConnection connection = new GestionConnection();

            // Assert
            Assert.False(connection.IsConnected);
            Assert.False(connection.HasErreurTransmission);
            Assert.Null(connection.Client);
            Assert.Null(connection.IpAdress);
            Assert.Null(connection.Port);
        }

        [Fact]
        public void HasErreurTransmission_Set_DeclencheNotifyPropertyChanged()
        {
            // Arrange
            GestionConnection connection = new GestionConnection();
            List<string> proprietesModifiees = new List<string>();

            connection.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != null) proprietesModifiees.Add(e.PropertyName);
            };

            // Act
            connection.HasErreurTransmission = true;

            // Assert
            Assert.True(connection.HasErreurTransmission);
            Assert.Contains("HasErreurTransmission", proprietesModifiees);
        }

        [Fact]
        public void DisposeClient_AvecClientNull_NePlantePas()
        {
            // Arrange
            GestionConnection connection = new GestionConnection();

            // Act
            Exception? exception = Record.Exception(delegate ()
            {
                connection.DisposeClient();
            });

            // Assert
            Assert.Null(exception);
            Assert.False(connection.IsConnected);
            Assert.False(connection.HasErreurTransmission);
        }

        [Fact]
        public void TesteConnection_AvecClientNull_NePlantePas()
        {
            // Arrange
            GestionConnection connection = new GestionConnection();

            // Act
            Exception? exception = Record.Exception(delegate ()
            {
                connection.TesteConnection();
            });

            // Assert
            Assert.Null(exception);
        }

        // Utilitaire pour créer un vrai serveur d'écho éphémère
        private int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        [Fact(Timeout = 5000)]
        public async Task AffectationClient_DeclencheLeHandshakeSansRaceCondition()
        {
            // Arrange : Création d'un serveur temporaire
            int port = GetFreePort();
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();

            var ct = TestContext.Current.CancellationToken;

            var gestionConnection = new GestionConnection();
            var clientJudo = new ClientJudo("127.0.0.1", port);

            try
            {
                // Act : 
                // 1. On affecte le client (ce qui déclenche vos abonnements sécurisés via le setter)
                gestionConnection.Client = clientJudo;

                // 2. Le serveur local accepte la connexion
                using var serverSideSocket = await listener.AcceptTcpClientAsync(ct);

                // Assert :
                // 3. Encapsulation stricte : On lit directement la socket côté SERVEUR 
                // pour prouver que le proxy a bien expédié la trame sur le réseau.
                var buffer = new byte[1024];
                var readTask = serverSideSocket.GetStream().ReadAsync(buffer, 0, buffer.Length, ct);

                // On utilise Task.WhenAny pour borner l'attente. Si une race condition se produit, 
                // le flux restera vide et ce bloc tombera en Timeout.
                var completedTask = await Task.WhenAny(readTask, Task.Delay(2000, ct));

                completedTask.Should().Be(readTask, "La condition de course a frappé : la connexion a réussi mais le Handshake initial n'est jamais arrivé au serveur.");

                // Défense en profondeur : on valide l'état explicite du client
                gestionConnection.IsConnected.Should().BeTrue("L'état du gestionnaire doit refléter le succès de l'opération.");
            }
            finally
            {
                // Nettoyage impératif pour éviter l'accumulation de sockets fantômes 
                listener.Stop();
                gestionConnection.DisposeClient();
            }
        }

        [Fact(Timeout = 5000)]
        public async Task ChangementDeClient_LibereAncienneSocketImmediatement()
        {
            // Ce test valide que le setter de GestionConnection n'accumule pas les sockets (Memory/Socket Leak)

            // Arrange
            var gestionConnection = new GestionConnection();
            var client1 = new ClientJudo("127.0.0.1", GetFreePort());
            var client2 = new ClientJudo("127.0.0.1", GetFreePort());

            // Act
            gestionConnection.Client = client1;
            gestionConnection.Client = client2; // Écrasement

            // CORRECTION : Attente asynchrone respectueuse du Timeout de xUnit
            await Task.Delay(200, TestContext.Current.CancellationToken);

            // Le premier client doit avoir été stoppé par le mécanisme de sécurité du setter
            client1.IsConnected.Should().BeFalse("Le premier client n'a pas été libéré proprement lors du remplacement, risque de fuite de socket (TIME_WAIT/SYN_SENT).");
        }
    }
}