using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Threading;

namespace FranceJudo.Core.Tests.Threading
{
    public class SynchronizedTests
    {
        [Fact]
        // 1. On passe la méthode en 'async Task'
        public async Task SafeReadAction_PlusieursLecteurs_NeBloquentPas()
        {
            // Arrange
            var syncList = new Synchronized<List<string>>(new List<string> { "Judo" });
            var inReadLockCount = 0;
            var maxConcurrentReaders = 0;
            var taskCount = 10;
            var tasks = new Task[taskCount];
            var startEvent = new ManualResetEventSlim(false);

            // Act
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    startEvent.Wait();

                    syncList.SafeReadAction(list =>
                    {
                        var current = Interlocked.Increment(ref inReadLockCount);
                        lock (syncList)
                        {
                            if (current > maxConcurrentReaders) maxConcurrentReaders = current;
                        }

                        Thread.Sleep(50);
                        Interlocked.Decrement(ref inReadLockCount);
                        return list.Count;
                    });
                }, TestContext.Current.CancellationToken); // Anticipation du warning 1051
            }

            startEvent.Set();

            // 2. CORRECTION xUnit1031 : On remplace Task.WaitAll() par await Task.WhenAll()
            await Task.WhenAll(tasks);

            // Assert
            maxConcurrentReaders.Should().BeGreaterThan(1);
        }

        [Fact]
        // 3. On passe la méthode en 'async Task'
        public async Task SafeWriteAction_VerrouExclusif_EmpecheLecture()
        {
            // Arrange
            var syncData = new Synchronized<string>("Data", timeoutSeconds: 1);
            var writeStartedEvent = new ManualResetEventSlim(false);

            var writeTask = Task.Run(() =>
            {
                syncData.SafeWriteAction(data =>
                {
                    writeStartedEvent.Set();
                    Thread.Sleep(1500);
                });
            }, TestContext.Current.CancellationToken); // Anticipation du warning 1051

            writeStartedEvent.Wait(1000, TestContext.Current.CancellationToken); // Wait() sur ManualResetEventSlim est autorisé (ce n'est pas une Task)

            // Act
            Action act = () => syncData.SafeReadAction(d => d);

            // Assert
            act.Should().Throw<TimeoutException>()
               .WithMessage("*Impossible d'obtenir le verrou de LECTURE*");

            // 4. CORRECTION xUnit1031 : On remplace .Wait() par await
            await writeTask;
        }
    }
}