using AppPublication.Controles;
using AppPublication.Tools.Enum;
using System;
using Xunit;

namespace AppPublication.Tests.Controles
{
    public class IClientProviderTests
    {
        [Fact]
        public void ClientDisconnectedEventArgs_InitialiseLHeureActuelle()
        {
            // Arrange
            DateTime avant = DateTime.Now;

            // Act
            ClientDisconnectedEventArgs arguments = new ClientDisconnectedEventArgs();

            // Assert
            Assert.True(arguments.DisconnectionTime >= avant);
            Assert.True(arguments.DisconnectionTime <= DateTime.Now);
        }

        [Fact]
        public void ConnectionStatusEventArgs_AssigneLesValeursCorrectement()
        {
            // Arrange
            bool expectedBusy = true;
            BusyStatusEnum expectedStatus = BusyStatusEnum.InitDonneesStructures;

            // Act
            ConnectionStatusEventArgs arguments = new ConnectionStatusEventArgs(expectedBusy, expectedStatus);

            // Assert
            Assert.Equal(expectedBusy, arguments.IsBusy);
            Assert.Equal(expectedStatus, arguments.Status);
        }

        [Fact]
        public void ClientReadyEventArgs_AssigneLeClient()
        {
            // Arrange
            // ClientJudo etant probablement une classe de communication externe,
            // on passe null pour tester l'assignation de la coquille de l'évènement.
            JudoClient.ClientJudo? clientVirtuel = null;

            // Act
            ClientReadyEventArgs arguments = new ClientReadyEventArgs(clientVirtuel);

            // Assert
            Assert.Null(arguments.Client);
        }
    }
}