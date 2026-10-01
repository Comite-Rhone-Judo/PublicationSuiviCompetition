#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using JudoClient;

namespace JudoClient.Tests
{
    public class ScannerServeurJudoTests
    {
        private static readonly string[] ips = new[] { "192.168.1.10", "192.168.1.11" };
        private static readonly string[] ipsArray = new[] { "192.0.2.1" };

        [Fact]
        public async Task ScanAsync_AvecTokenAnnule_SArreteImmediatement()
        {
            // Arrange
            var scanner = new ScannerServeurJudo();
            scanner.SetExplicitIps(ips);

            // On crée un jeton d'annulation et on le déclenche instantanément
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var mockProgress = new Mock<IProgress<int>>();

            // Act
            int serveursTrouves = await scanner.ScanAsync(mockProgress.Object, cts.Token);

            // Assert
            serveursTrouves.Should().Be(0, "Le scan aurait dû intercepter le CancellationToken et s'arrêter avant de trouver quoi que ce soit.");
        }

        [Fact]
        public async Task SetNetworkRange_CalculCorrectDesAdresses_EtRapporteProgression()
        {
            // Arrange
            var scanner = new ScannerServeurJudo
            {
                BypassPing = true, // On saute le ping pour attaquer directement le TCP
                TcpTimeoutMs = 1   // Échec TCP immédiat pour que le test soit instantané
            };

            // Masque /30 (255.255.255.252) -> Génère exactement 2 adresses IP hôtes (ici .1 et .2)
            scanner.SetNetworkRange("192.168.1.1", "255.255.255.252");

            var progressValues = new List<int>();
            var progress = new Progress<int>(v => progressValues.Add(v));

            // Act
            int serveursTrouves = await scanner.ScanAsync(progress, CancellationToken.None);

            // Assert
            serveursTrouves.Should().Be(0, "Il n'y a pas de vrai serveur sur ces adresses.");

            // Si la progression a atteint 100%, cela prouve que :
            // 1. SetNetworkRange a bien calculé les IP
            // 2. La boucle for sur les ports s'est bien exécutée pour chaque IP
            // 3. Le SemaphoreSlim a fait son travail sans bloquer
            progressValues.Should().NotBeEmpty("Le scanner doit utiliser l'interface IProgress pour rapporter son avancée.");
            progressValues.Should().Contain(100, "Le scanner doit toujours terminer et publier un statut à 100%.");
        }

        [Fact]
        public async Task ScanAsync_PingEchoue_FiltreLIPEtAvanceLaProgression()
        {
            // Arrange
            var scanner = new ScannerServeurJudo
            {
                BypassPing = false,
                PingTimeoutMs = 1 // Timeout microscopique pour forcer l'échec du ping
            };

            // 192.0.2.x (TEST-NET-1) est une IP réservée pour la doc, elle ne répondra jamais
            scanner.SetExplicitIps(ipsArray);

            var progressValues = new List<int>();
            var progress = new Progress<int>(v => progressValues.Add(v));

            // Act
            int serveursTrouves = await scanner.ScanAsync(progress, CancellationToken.None);

            // Assert
            serveursTrouves.Should().Be(0);

            // Même si le ping a échoué (et a donc skippé l'étape TCP complète pour cette IP),
            // le math de la progression ("testsCompleted += portsPerIp") doit avoir fait sauter la barre à 100%.
            progressValues.Should().Contain(100, "L'échec d'un ping doit quand même valider le lot d'IPs dans le pourcentage de progression.");
        }

        [Fact]
        public async Task SetNetworkRange_AdressesInvalides_AbsorbeLerreurEtNePlantePas()
        {
            // Arrange
            var scanner = new ScannerServeurJudo();

            // Act
            // SetNetworkRange contient un try/catch pour le parsing uint.Parse()
            Action act = () => scanner.SetNetworkRange("ceci_nest_pas_une_ip", "255.255.255.0");

            // Assert
            act.Should().NotThrow("La méthode doit absorber l'exception de parsing et la confier au LogTools.");

            // Si la plage a échoué silencieusement, _ipsToScan est vide, le scan doit retourner 0 instantanément.
            int result = await scanner.ScanAsync(null, CancellationToken.None);
            result.Should().Be(0);
        }
    }
}