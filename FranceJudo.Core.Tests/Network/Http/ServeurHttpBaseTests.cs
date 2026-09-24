#nullable enable
using System;
using System.Net;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Http;

namespace FranceJudo.Core.Tests.Network.Http
{
    public class ServeurHttpBaseTests
    {
        // Bouchon pour tester la classe de base (qui agit comme une classe abstraite logicielle)
        private class StubServeurHttp : ServeurHttpBase
        {
            // On expose la méthode protégée uniquement pour vérifier son comportement de rejet
            public void ExposeFindAvailablePort(int min, int max)
            {
                base.FindAvailablePort(min, max);
            }

            // Mock des méthodes manquantes de l'interface IServeurHttp pour que le compilo soit content
            public new void Start() { }
            public new  void Stop() { }
            public new void AddModule(object module) { }
        }

        [Fact]
        public void Constructeur_InitialiseLesProprietesParDefaut()
        {
            // Act
            var server = new StubServeurHttp();

            // Assert
            server.Port.Should().Be(80, "Le port par défaut défini dans la classe de base est 80.");

            // CORRECTION : On valide que le constructeur a bien fait son travail d'auto-découverte réseau.
            server.ListeningIpAddress.Should().NotBeNull("Le serveur HTTP doit automatiquement récupérer l'adresse IP de la machine locale lors de son instanciation.");
        }

        [Fact]
        public void Proprietes_SetEtGet_FonctionnentCorrectement()
        {
            // Arrange
            var server = new StubServeurHttp();
            var ip = IPAddress.Parse("192.168.1.10");

            // Act
            server.ListeningIpAddress = ip;
            server.PortMin = 8080;
            server.PortMax = 8090;

            // Assert
            server.ListeningIpAddress.Should().Be(ip);
            server.PortMin.Should().Be(8080);
            server.PortMax.Should().Be(8090);
        }

        [Fact]
        public void FindAvailablePort_SiTousLesPortsEchouent_LeveArgumentOutOfRangeException()
        {
            // Note : Ce test est très délicat car il tente réellement de lier des ports.
            // On lui donne une plage absurde (ex: ports négatifs ou inversés) 
            // pour forcer la boucle à échouer immédiatement sans bloquer le réseau.
            var server = new StubServeurHttp();

            // Act
            // min > max force la boucle "while (port <= portMax)" à être ignorée
            Action act = () => server.ExposeFindAvailablePort(8000, 7000);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>("Si aucun port n'est libre, la méthode doit crasher explicitement.");
        }
    }
}