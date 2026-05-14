using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Threading;

namespace FranceJudo.Core.Tests.Threading
{
    public class SingleShotTimerTests
    {
        [Fact]
        public async Task Start_CasNominal_DeclencheLeTickAuBoutDuDelai()
        {
            // Arrange
            using var timer = new SingleShotTimer();
            var tcs = new TaskCompletionSource<bool>(); // Notre capteur d'événement asynchrone
            int callCount = 0;

            timer.Elapsed += (state) =>
            {
                Interlocked.Increment(ref callCount);
                tcs.TrySetResult(true); // Débloque le test instantanément !
            };

            // Act
            timer.Start(50); // Délai très court de 50ms pour le test

            // Assert
            // On attend que le TCS soit signalé, avec un timeout de sécurité (ex: 1 seconde)
            // pour éviter que le test ne tourne à l'infini si le timer est cassé.
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(1000, TestContext.Current.CancellationToken));

            completedTask.Should().Be(tcs.Task, "Le timer n'a pas tické dans le temps imparti (1 seconde).");
            callCount.Should().Be(1, "L'événement doit se déclencher exactement une fois.");
            timer.IsRunning.Should().BeFalse("Le timer doit s'éteindre de lui-même après un SingleShot.");
        }

        [Fact]
        public async Task Stop_AvantLaFinDuDelai_AnnuleLeTick()
        {
            // Arrange
            using var timer = new SingleShotTimer();
            bool eventFired = false;

            timer.Elapsed += (state) => eventFired = true;

            // Act
            timer.Start(200);
            timer.IsRunning.Should().BeTrue();

            timer.Stop(); // Annulation immédiate

            // Assert
            // On attend volontairement plus longtemps que le délai initial (ex: 300ms)
            await Task.Delay(300, TestContext.Current.CancellationToken);

            eventFired.Should().BeFalse("L'événement ne doit pas se déclencher si Stop() a été appelé.");
            timer.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task Start_AppelsMultiples_ReinitialiseLeTimer_EffetDebounce()
        {
            // Arrange
            using var timer = new SingleShotTimer();
            int callCount = 0;

            timer.Elapsed += (state) => Interlocked.Increment(ref callCount);

            // Act
            timer.Start(150); // Premier appel

            // On attend un peu (mais moins que 150ms)
            await Task.Delay(50, TestContext.Current.CancellationToken);

            timer.Start(150); // Deuxième appel qui doit "écraser" le premier

            // Assert
            // On attend la fin logique du DEUXIÈME timer (soit environ 250ms au total depuis le début)
            await Task.Delay(250, TestContext.Current.CancellationToken);

            callCount.Should().Be(1, "Seul le dernier appel à Start() doit déclencher un tick.");
        }

        [Fact]
        public void Stop_AppelePlusieursFois_NePlantePas()
        {
            // Arrange
            using var timer = new SingleShotTimer();
            timer.Start(100);

            // Act
            // Un classique en multi-threading : vérifier que Stop() est idempotent 
            // (qu'on peut l'appeler plusieurs fois sans NullReferenceException)
            Action act = () =>
            {
                timer.Stop();
                timer.Stop();
            };

            // Assert
            act.Should().NotThrow();
        }
    }
}