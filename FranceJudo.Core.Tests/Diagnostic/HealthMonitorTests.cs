using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Diagnostic;

namespace FranceJudo.Core.Tests.Diagnostic
{
    // Important : Exécution isolée car la classe contient des états statiques
    [Collection("Sequential - Diagnostic")]
    public class HealthMonitorTests : IDisposable
    {
        public HealthMonitorTests()
        {
            HealthMonitor.StopAllMonitoring(); // S'assure d'un état propre
        }

        public void Dispose()
        {
            HealthMonitor.StopAllMonitoring(); // Nettoyage en fin de test
        }

        [Fact]
        public void StartAndStop_NePlantentPas_EtGerentLEtatCorrectement()
        {
            // Arrange & Act
            Action actStart = () => HealthMonitor.StartSystemMonitoring(1);
            Action actStop = () => HealthMonitor.StopAllMonitoring();

            // Assert
            actStart.Should().NotThrow("Le démarrage ne doit pas lever d'exception.");
            actStart.Should().NotThrow("Le double démarrage (idempotence) doit être géré silencieusement.");

            actStop.Should().NotThrow("L'arrêt doit détruire le timer sans erreur.");
            actStop.Should().NotThrow("Le double arrêt doit être ignoré proprement.");
        }
    }
}