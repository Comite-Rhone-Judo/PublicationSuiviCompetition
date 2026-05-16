#nullable enable
using System;
using Xunit;
using FluentAssertions;
using Moq;
using JudoClient;
using FranceJudo.Core.Network.Tcp.Client;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;

namespace JudoClient.Tests.Communication
{
    /// <summary>
    /// Classe de base pour tester l'émission de trames XML (Extensions de ClientJudo).
    /// </summary>
    public abstract class SenderTestBase
    {
        /// <summary>
        /// Intercepte l'appel réseau, valide la présence de la commande, et permet des validations supplémentaires.
        /// </summary>
        protected void VerifyCommandSent(Action<ClientJudo> actionToTest, ServerCommandEnum expectedCommand, Action<string>? extraValidation = null)
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            string? payloadSent = null;

            mockNetworkClient.Setup(c => c.IsConnected).Returns(true);

            // On capture la trame XML passée à la méthode Write !
            mockNetworkClient.Setup(c => c.Write(It.IsAny<string>()))
                             .Callback<string>(data => payloadSent = data);

            var client = new ClientJudo(mockNetworkClient.Object);

            // Act
            actionToTest(client);

            // Assert de base
            mockNetworkClient.Verify(c => c.Write(It.IsAny<string>()), Times.Once, "La méthode Write du réseau doit être appelée exactement une fois.");

            payloadSent.Should().NotBeNullOrEmpty("Le payload XML envoyé ne doit pas être vide.");
            payloadSent.Should().Contain($"<{ConstantXML.Command}>{(int)expectedCommand}</{ConstantXML.Command}>", $"L'enveloppe XML doit contenir la commande {(int)expectedCommand}.");

            // Assert spécifique (optionnel)
            extraValidation?.Invoke(payloadSent!);
        }
    }
}