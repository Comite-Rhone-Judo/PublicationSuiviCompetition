using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Diagnostic;

namespace FranceJudo.Core.Tests.Diagnostic
{
    public class ActionWatcherTests
    {
        [Fact]
        public void Execute_Action_MesureLeTempsDExecution()
        {
            // Act
            long duration = ActionWatcher.Execute(() => Thread.Sleep(50));

            // Assert
            // Thread.Sleep n'est pas ultra-précis, on vérifie que le temps est cohérent
            duration.Should().BeGreaterThanOrEqualTo(45);
        }

        [Fact]
        public void Execute_Fonction_RetourneLeResultatEtLeTemps()
        {
            // Act
            var result = ActionWatcher.Execute(() =>
            {
                Thread.Sleep(50);
                return "Judo";
            });

            // Assert
            result.Result.Should().Be("Judo");
            result.DurationMs.Should().BeGreaterThanOrEqualTo(45);
        }

        [Fact]
        public async Task ExecuteAsync_Task_MesureLeTempsDExecution()
        {
            // Act
            long duration = await ActionWatcher.ExecuteAsync(async () => await Task.Delay(50));

            // Assert
            duration.Should().BeGreaterThanOrEqualTo(45);
        }

        [Fact]
        public void Methodes_Null_LeventArgumentNullException()
        {
            // Assert
            Action act1 = () => ActionWatcher.Execute((Action)null!);
            Action act2 = () => ActionWatcher.Execute((Func<int>)null!);
            Func<Task> act3 = async () => await ActionWatcher.ExecuteAsync((Func<Task>)null!);

            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
            act3.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}