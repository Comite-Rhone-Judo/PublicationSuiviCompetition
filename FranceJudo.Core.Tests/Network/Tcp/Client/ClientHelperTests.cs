#nullable enable
using System;
using System.Net.Sockets;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Tcp.Client;

namespace FranceJudo.Core.Tests.Network.Tcp.Client
{
    public class ClientHelperTests
    {
        [Fact]
        public void ConnectSocket_CreationDuClientSiNull_EtGestionDeLerreurDeConnexion()
        {
            // Arrange
            TcpClient? client = null;

            // Act
            // On tente une connexion sur une IP impossible à router pour déclencher le catch
            Action act = () => ClientHelper.ConnectSocket(ref client, "999.999.999.999", 8080, null);

            // Assert
            act.Should().NotThrow("La méthode possède un try/catch global qui trace l'erreur sans la relancer.");

            client.Should().NotBeNull("Le TcpClient a bien été instancié par la méthode avant l'erreur réseau.");

            // NOTE DE L'ARCHITECTE : On ne vérifie pas client.NoDelay ici.
            // Ton bloc catch exécute 'client.Close()', ce qui détruit le socket interne. 
            // Tenter de lire ses propriétés lèverait une NullReferenceException en .NET 10.0.
        }

        [Fact]
        public void SendData_ClientNonConnecte_LeveUneExceptionApresLog()
        {
            // Arrange
            using var client = new TcpClient(); // Client créé mais jamais connecté

            // Act
            // L'appel à BeginWrite sur un NetworkStream non connecté lève une InvalidOperationException
            Action act = () => ClientHelper.SendData(client, "Message", null);

            // Assert
            act.Should().Throw<InvalidOperationException>("Contrairement à Connect, SendData utilise un 'throw;' dans son catch pour relancer l'erreur vers l'appelant supérieur.");
        }
    }
}