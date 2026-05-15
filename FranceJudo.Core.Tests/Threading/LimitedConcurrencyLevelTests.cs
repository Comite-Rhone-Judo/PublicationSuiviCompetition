using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Threading;

namespace FranceJudo.Core.Tests.Threading
{
    public class LimitedConcurrencyLevelTests
    {
        [Fact]
        public void TryExecuteTaskInline_ForceExecutionSurLeMemeThread_NePlantePas()
        {
            // Arrange
            // CORRECTION CS1674 : Retrait du 'using' (TaskScheduler n'est pas IDisposable)
            var scheduler = new LimitedConcurrencyLevel(2);
            var inlineTask = new System.Threading.Tasks.Task(() => { /* dummy work */ });

            // Act
            Action act = () => inlineTask.RunSynchronously(scheduler);

            // Assert
            act.Should().NotThrow("L'exécution inline doit être supportée par LimitedConcurrencyLevel sans lever d'exception.");
        }

        [Fact]
        public void Constructeur_NiveauZeroOuNegatif_LeveArgumentOutOfRangeException()
        {
            // Act
            Action act = () => new LimitedConcurrencyLevel(0);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithParameterName("maxDegreeOfParallelism");
        }

        [Fact]
        public async Task Execution_CasNominal_ExecuteLaTacheCorrectement()
        {
            // Arrange
            var scheduler = new LimitedConcurrencyLevel(1);
            var factory = new TaskFactory(scheduler);
            bool wasExecuted = false;

            // Act
            // Le jeton de TestContext empêche le xUnit1051
            await factory.StartNew(() => wasExecuted = true, TestContext.Current.CancellationToken);

            // Assert
            wasExecuted.Should().BeTrue();
        }

        [Fact]
        public async Task Execution_ChargeMassive_RespecteStrictementLaLimite()
        {
            // Arrange
            int concurrencyLimit = 2;
            int totalTasks = 10;

            var scheduler = new LimitedConcurrencyLevel(concurrencyLimit);
            // On configure une usine à tâches forcée d'utiliser notre planificateur
            var factory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, scheduler);

            var tasks = new Task[totalTasks];

            int currentRunning = 0;
            int maxRunningObserved = 0;
            var lockObj = new object();

            // Le sas de blocage : restera fermé tant qu'on ne donne pas le feu vert
            var blockEvent = new ManualResetEventSlim(false);
            int tasksReadyAtBlock = 0;

            // Act
            for (int i = 0; i < totalTasks; i++)
            {
                // On lance les 10 tâches à la vitesse de l'éclair
                tasks[i] = factory.StartNew(() =>
                {
                    // 1. La tâche démarre : on incrémente les compteurs
                    int current = Interlocked.Increment(ref currentRunning);

                    lock (lockObj)
                    {
                        if (current > maxRunningObserved) maxRunningObserved = current;
                    }

                    Interlocked.Increment(ref tasksReadyAtBlock);

                    // 2. LA TÂCHE GÈLE ICI : Elle occupe un thread du scheduler
                    blockEvent.Wait();

                    // 3. Fin de la tâche
                    Interlocked.Decrement(ref currentRunning);
                }, TestContext.Current.CancellationToken);
            }

            // On attend passivement que le sas soit sous pression (que 'concurrencyLimit' tâches soient coincées)
            // On utilise une boucle asynchrone pour ne pas déclencher le warning xUnit1031
            int timeoutMs = 2000;
            int waitedMs = 0;
            while (Volatile.Read(ref tasksReadyAtBlock) < concurrencyLimit && waitedMs < timeoutMs)
            {
                await Task.Delay(50, TestContext.Current.CancellationToken);
                waitedMs += 50;
            }

            // POINT CRITIQUE : On attend encore un peu pour prouver qu'aucune 3ème tâche ne force le passage
            await Task.Delay(200, TestContext.Current.CancellationToken);

            // On prend une photo des compteurs pendant que le sas est toujours fermé
            int runningDuringBlock = Volatile.Read(ref currentRunning);
            int maxObserved;
            lock (lockObj) maxObserved = maxRunningObserved;

            // On ouvre le sas pour libérer tout le monde et on attend la fin du traitement
            blockEvent.Set();
            await Task.WhenAll(tasks);

            // Assert
            // Si le scheduler fonctionne, seules 2 tâches ont pu atteindre le sas en même temps
            runningDuringBlock.Should().Be(concurrencyLimit, "Seul le nombre exact de la limite doit s'exécuter simultanément.");
            maxObserved.Should().Be(concurrencyLimit, "Le pic d'exécution historique ne doit jamais dépasser la limite imposée.");
        }
    }
}