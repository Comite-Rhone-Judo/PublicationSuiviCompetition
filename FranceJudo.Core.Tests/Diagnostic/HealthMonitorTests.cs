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

        [Fact]
        public void LogSystemHealth_MethodePrivee_LitLesDonneesSystemeSansPlanter()
        {
            // Arrange
            // En production, cette méthode est appelée par un Timer sur un thread d'arrière-plan.
            // En test, on l'invoque manuellement par réflexion pour garantir son exécution synchrone et mesurer sa couverture.
            var method = typeof(HealthMonitor).GetMethod("LogSystemHealth", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            // Act
            Action act = () =>
            {
                // L'invocation par réflexion (le paramètre 'state' du Timer est null par défaut)
                method?.Invoke(null, new object[] { null! });
            };

            // Assert
            // La méthode enveloppe toute sa logique dans un try/catch pour protéger le système.
            // On s'assure que la lecture de la RAM, des Threads et du GC se déroule sans lever (ni fuiter) d'exception.
            act.Should().NotThrow("La lecture des performances système doit fonctionner silencieusement, et ses exceptions éventuelles doivent être attrapées.");
        }
    }
}