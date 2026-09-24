using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Threading;

namespace FranceJudo.Core.Tests.Threading
{
    public class ParallelTaskBatcherTests
    {
        [Fact]
        public void WaitAllAndGetResults_AttendToutesLesTachesEtRetourneResultats()
        {
            // Arrange
            var globalProgress = new Progress<string>();
            static string progressMapper(float percent) => $"Progression : {percent}%";

            // CORRECTION CS1674 : Retrait du 'using'
            var batcher = new ParallelTaskBatcher<string, string>(globalProgress, progressMapper, 2, 5000);

            batcher.AddWork((taskProgress) =>
            {
                System.Threading.Thread.Sleep(50);
                return new[] { "Travail 1" };
            }, 1);

            batcher.AddWork((taskProgress) =>
            {
                System.Threading.Thread.Sleep(50);
                return new[] { "Travail 2" };
            }, 1);

            // Act
            var results = batcher.WaitAllAndGetResults();

            // Assert
            results.Should().NotBeNull();
            results.Should().Contain(new[] { "Travail 1", "Travail 2" });
            batcher.HasPendingWork.Should().BeFalse("Toutes les tâches doivent être terminées.");
        }

        [Fact]
        public void ConcurrencyLevel_Set_ModifieLaConcurrenceEnCoursDeRoute()
        {
            // Arrange
            var globalProgress = new Progress<string>();
            var batcher = new ParallelTaskBatcher<string, string>(globalProgress, p => "", 1, 5000);

            batcher.AddWork((taskProgress) =>
            {
                System.Threading.Thread.Sleep(100);
                return new[] { "Test 1" };
            }, 1);

            // Act
            Action act = () =>
            {
                // C'est le setter public qui doit appeler ta méthode privée UpdateSchedulerConfiguration
                batcher.ConcurrencyLevel = 4;
            };

            // Assert
            act.Should().NotThrow("La modification du niveau de concurrence via la propriété publique ne doit pas planter.");

            // On s'assure que le batcher n'a pas été corrompu par ce changement à la volée
            var results = batcher.WaitAllAndGetResults();
            results.Should().Contain(new[] { "Test 1" }, "Le batcher doit terminer son travail même si sa configuration a changé en cours de route.");
        }

        #region Bouchon de Test (Stub)

        // Un reporter synchrone et Thread-Safe pour capter les mises à jour sans délai asynchrone
        private class TestProgressReporter : IProgress<float>
        {
            public List<float> Reports { get; } = new List<float>();

            public void Report(float value)
            {
                lock (Reports)
                {
                    Reports.Add(value);
                }
            }
        }

        #endregion

        [Fact]
        public async Task WaitAllAndGetResultsAsync_AgregationMultiTaches_RecupereTousLesResultats()
        {
            // Arrange
            // On désactive la limitation de concurrence (-1) et le throttling (0) pour ce test
            var batcher = new ParallelTaskBatcher<float, string>(null, f => f, concurrencyLevel: -1, throttlingIntervalTicks: 0);

            // Act
            batcher.AddWork(progress =>
            {
                return new List<string> { "Judo", "Karate" };
            });

            batcher.AddWork(progress =>
            {
                return new List<string> { "Aikido" };
            });

            // Assert
            batcher.HasPendingWork.Should().BeTrue();

            var results = await batcher.WaitAllAndGetResultsAsync();

            results.Should().HaveCount(3);
            results.Should().Contain(new[] { "Judo", "Karate", "Aikido" });
            batcher.HasPendingWork.Should().BeFalse("Le batcher doit se réinitialiser automatiquement après WaitAll.");
        }

        [Fact]
        public async Task Progression_CalculGlobalDesTaches_AtteintLes100Pourcent()
        {
            // Arrange
            var reporter = new TestProgressReporter();
            var batcher = new ParallelTaskBatcher<float, int>(reporter, f => f, concurrencyLevel: -1, throttlingIntervalTicks: 0);

            // Act
            // Tâche 1 : 10 étapes
            batcher.AddWork(p =>
            {
                p.Report(BatchProgressInfo.Init(10));
                for (int i = 1; i <= 10; i++)
                {
                    p.Report(BatchProgressInfo.Step(i));
                    Thread.Sleep(5); // Mini pause pour simuler un traitement
                }
                return Array.Empty<int>();
            });

            // Tâche 2 : 5 étapes
            batcher.AddWork(p =>
            {
                p.Report(BatchProgressInfo.Init(5));
                for (int i = 1; i <= 5; i++)
                {
                    p.Report(BatchProgressInfo.Step(i));
                    Thread.Sleep(5);
                }
                return Array.Empty<int>();
            });

            await batcher.WaitAllAndGetResultsAsync();

            // Assert
            var allReports = reporter.Reports;

            allReports.Should().NotBeEmpty();
            // Grâce au Reset() dans le bloc finally de ton batcher, 
            // la toute dernière valeur remontée doit être 0 (réinitialisation pour le prochain batch)
            allReports.Last().Should().Be(0f);

            // Juste avant la réinitialisation, le système a dû remonter l'atteinte des 100% (1.0f)
            float maxValueReported = allReports.Max();
            maxValueReported.Should().Be(1.0f, "L'addition atomique globale n'a pas atteint 100%.");
        }

        [Fact]
        public async Task FaultTolerance_TacheEnEchec_RecupereLesResultatsPartiels()
        {
            // Arrange
            var batcher = new ParallelTaskBatcher<float, string>(null, f => f, concurrencyLevel: -1, throttlingIntervalTicks: 0);

            // Act
            // 1. Tâche saine
            batcher.AddWork(p => new List<string> { "Succès" });

            // 2. Tâche empoisonnée (Crash !)
            batcher.AddWork(p =>
            {
                throw new InvalidOperationException("Explosion en vol de la tâche parallèle");
            });

            // Act & Assert en une seule passe
            // L'exécution ne plantera pas grâce à tes blocs catch internes, et on récupère le résultat avant le Reset().
            var results = await batcher.WaitAllAndGetResultsAsync();

            // On doit avoir récupéré la donnée de la tâche 1 malgré le crash de la tâche 2
            results.Should().ContainSingle("Succès");
        }
    }
}