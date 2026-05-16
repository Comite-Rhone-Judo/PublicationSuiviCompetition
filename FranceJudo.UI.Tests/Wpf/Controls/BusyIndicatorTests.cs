#nullable enable
using System.Windows;
using Xunit;
using FluentAssertions;
using FranceJudo.UI.Wpf.Controls;

namespace FranceJudo.UI.Tests.Wpf.Controls
{
    public class BusyIndicatorTests : WpfTestBase
    {
        [Fact]
        public void Constructeur_AssigneLesValeursParDefautCorrectes()
        {
            RunInSTA(() =>
            {
                // Arrange & Act
                var indicator = new BusyIndicator();

                // Assert : Vérification des valeurs déclarées dans les PropertyMetadata
                indicator.IsBusy.Should().BeFalse("Par défaut, l'indicateur ne doit pas masquer l'interface.");
                indicator.ProgressValue.Should().Be(0.0, "La progression par défaut doit être à 0.");
                indicator.IsIndeterminate.Should().BeTrue("Le mode par défaut doit être indéterminé (spinner qui tourne indéfiniment).");
                indicator.BusyContent.Should().BeNull("Il ne doit pas y avoir de texte par défaut.");
            });
        }

        [Fact]
        public void ProprietesDeDependance_SetEtGet_FonctionnentCorrectement()
        {
            RunInSTA(() =>
            {
                // Arrange
                var indicator = new BusyIndicator
                {
                    // Act
                    IsBusy = true,
                    ProgressValue = 75.5,
                    IsIndeterminate = false,
                    BusyContent = "Chargement en cours..."
                };

                // Assert : Vérifie que le GetValue/SetValue WPF interne stocke bien les états
                indicator.IsBusy.Should().BeTrue();
                indicator.ProgressValue.Should().Be(75.5);
                indicator.IsIndeterminate.Should().BeFalse();
                indicator.BusyContent.Should().Be("Chargement en cours...");
            });
        }
    }
}