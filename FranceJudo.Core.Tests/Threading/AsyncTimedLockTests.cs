using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Threading;

namespace FranceJudo.Core.Tests.Threading
{
    public class AsyncTimedLockTests
    {
        [Fact]
        public async Task LockAsync_AcquisitionReussie_LibereCorrectement()
        {
            // Arrange
            using var asyncLock = new AsyncTimedLock();

            // Act & Assert
            using (await asyncLock.LockAsync(TimeSpan.FromSeconds(1)))
            {
                // Si on arrive ici, le verrou est acquis
                true.Should().BeTrue();
            }
        }

        [Fact]
        public async Task LockAsync_VerrouDejaPris_LeveTimeoutException()
        {
            // Arrange
            using var asyncLock = new AsyncTimedLock();
            var testTimeout = TimeSpan.FromMilliseconds(50);
            var tcs = new TaskCompletionSource<bool>();

            // Tâche de fond qui bloque le sémaphore
            _ = Task.Run(async () =>
            {
                using (await asyncLock.LockAsync(TimeSpan.FromSeconds(5)))
                {
                    tcs.SetResult(true); // Signal
                    await Task.Delay(200); // Maintien du blocage
                }
            }, TestContext.Current.CancellationToken);

            await tcs.Task; // On attend que la tâche de fond ait verrouillé

            // Act
            Func<Task> act = async () => await asyncLock.LockAsync(testTimeout);

            // Assert
            await act.Should().ThrowAsync<TimeoutException>()
                     .WithMessage("*Impossible d'obtenir le verrou asynchrone*");
        }
    }
}