using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Threading;

namespace FranceJudo.Core.Tests.Threading
{
    public class TimedLockTests
    {
        [Fact]
        public void Lock_ObjetNull_LeveArgumentNullException()
        {
            // Act
            Action act = () => TimedLock.Lock(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Lock_AcquisitionReussie_LibereCorrectement()
        {
            // Arrange
            var objToLock = new object();

            // Act & Assert
            // On vérifie simplement que ça ne plante pas et qu'on sort proprement
            using (TimedLock.Lock(objToLock, 1))
            {
                Monitor.IsEntered(objToLock).Should().BeTrue();
            }

            Monitor.IsEntered(objToLock).Should().BeFalse();
        }

        [Fact]
        public void Lock_VerrouDejaPris_LeveTimeoutException()
        {
            // Arrange
            var objToLock = new object();
            var lockAcquiredEvent = new ManualResetEventSlim(false);
            var testTimeout = TimeSpan.FromMilliseconds(50); // Timeout très court pour le test

            // Thread secondaire qui prend le verrou et ne le lâche pas tout de suite
            Task.Run(() =>
            {
                using (TimedLock.Lock(objToLock, 5))
                {
                    lockAcquiredEvent.Set(); // Signal au thread principal que le verrou est pris
                    Thread.Sleep(200);       // Garde le verrou plus longtemps que le timeout du test
                }
            }, TestContext.Current.CancellationToken);

            lockAcquiredEvent.Wait(1000, TestContext.Current.CancellationToken); // On attend que le thread secondaire soit prêt

            // Act
            // Le thread principal tente de prendre le verrou avec un timeout de 50ms
            Action act = () => TimedLock.Lock(objToLock, testTimeout);

            // Assert
            act.Should().Throw<TimeoutException>()
               .WithMessage("*Impossible d'obtenir le verrou*");
        }
    }
}