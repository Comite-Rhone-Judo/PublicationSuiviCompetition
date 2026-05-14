#nullable enable
using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network;

namespace FranceJudo.Core.Tests.Network
{
    public class StatusMiniSiteTests
    {
        [Fact]
        public void Constructeur_ParDefaut_InitialiseLesValeursAuRepos()
        {
            // Act
            var status = new StatusMiniSite();

            // Assert
            status.State.Should().Be(StateMiniSiteEnum.Stopped);
            status.Message.Should().Be("-");
            status.Progress.Should().Be(-1);
            status.IsProgressUnknown.Should().BeTrue();
        }

        [Fact]
        public void Progress_MiseAJour_AltereAutomatiquementIsProgressUnknown()
        {
            // Arrange
            var status = new StatusMiniSite();

            // Act 1 : Progression connue
            status.Progress = 50;

            // Assert 1
            status.IsProgressUnknown.Should().BeFalse("Une progression de 50% est une valeur connue.");

            // Act 2 : Progression inconnue
            status.Progress = -1;

            // Assert 2
            status.IsProgressUnknown.Should().BeTrue("Une progression de -1 doit lever le flag IsProgressUnknown.");
        }

        [Theory]
        [InlineData(StateMiniSiteEnum.Stopped)]
        [InlineData(StateMiniSiteEnum.Idle)]
        [InlineData(StateMiniSiteEnum.Listening)]
        public void State_PassageEnEtatDeRepos_ReinitialiseLaProgression(StateMiniSiteEnum targetState)
        {
            // Arrange
            var status = new StatusMiniSite { Progress = 80 }; // On simule un statut en cours

            // Act
            status.State = targetState;

            // Assert
            status.Progress.Should().Be(-1, $"Le passage à l'état {targetState} doit écraser la progression à -1.");
        }

        [Fact]
        public void State_PassageEnEtatActif_NeReinitialisePasLaProgression()
        {
            // Arrange
            var status = new StatusMiniSite { Progress = 42 };

            // Act
            status.State = StateMiniSiteEnum.Syncing;

            // Assert
            status.Progress.Should().Be(42, "L'état Syncing ne fait pas partie des états réinitialisant la progression.");
        }
    }
}