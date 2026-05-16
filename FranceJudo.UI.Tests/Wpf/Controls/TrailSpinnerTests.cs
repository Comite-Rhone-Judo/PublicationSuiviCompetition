#nullable enable
using System.Windows;
using Xunit;
using FluentAssertions;
using FranceJudo.UI.Wpf.Controls;

namespace FranceJudo.UI.Tests.Wpf.Controls
{
    public class TrailSpinnerTests : WpfTestBase
    {
        [Fact]
        public void Constructeur_AssigneLaValeurParDefautCorrecte()
        {
            RunInSTA(() =>
            {
                // Arrange & Act
                var spinner = new TrailSpinner();

                // Assert : Vérification de la PropertyMetadata
                spinner.Thickness.Should().Be(2.0, "L'épaisseur par défaut du spinner définie dans les métadonnées doit être de 2.0.");
            });
        }

        [Fact]
        public void Thickness_SetEtGet_FonctionneCorrectement()
        {
            RunInSTA(() =>
            {
                // Arrange
                var spinner = new TrailSpinner
                {
                    // Act
                    Thickness = 5.5
                };

                // Assert
                spinner.Thickness.Should().Be(5.5, "La propriété de dépendance doit stocker et restituer l'épaisseur modifiée.");
            });
        }
    }
}