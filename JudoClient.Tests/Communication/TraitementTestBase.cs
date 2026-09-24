#nullable enable
using System;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using JudoClient;
using FranceJudo.Core.Network.Tcp.Client;

namespace JudoClient.Tests.Communication
{
    /// <summary>
    /// Classe de base fournissant l'injection du mock réseau et un Helper pour valider 
    /// les événements de relais (Pass-through) en une seule ligne.
    /// </summary>
    public abstract class TraitementTestBase
    {
        protected ClientJudo GetTestClient()
        {
            var mockNetworkClient = new Mock<IClientGenerique>();
            mockNetworkClient.Setup(c => c.IsConnected).Returns(true);
            return new ClientJudo(mockNetworkClient.Object);
        }

        protected void VerifyPassThroughEvent<TDelegate>(
            Action<TDelegate> subscribe,
            Action<XElement> triggerMethod,
            object expectedSender) where TDelegate : Delegate
        {
            // Arrange
            var elementEnvoye = new XElement("TestData", "Valeur");
            bool eventTriggered = false;

            // Astuce pour contourner la rigidité des delegates personnalisés sans reflection complexe
            Delegate handler = null!;
            if (typeof(TDelegate).Name.Contains("Handler"))
            {
                // On crée une méthode anonyme correspondant à la signature (object, XElement)
                Action<object, XElement> action = (sender, recu) =>
                {
                    eventTriggered = true;
                    sender.Should().BeSameAs(expectedSender, "Le sender de l'événement est incorrect.");
                    recu.Should().BeSameAs(elementEnvoye, "Le XElement n'a pas été transmis correctement.");
                };
                handler = Delegate.CreateDelegate(typeof(TDelegate), action.Target, action.Method);
            }

            subscribe((TDelegate)(object)handler);

            // Act
            triggerMethod(elementEnvoye);

            // Assert
            eventTriggered.Should().BeTrue("L'événement n'a pas été déclenché par la méthode.");
        }
    }
}