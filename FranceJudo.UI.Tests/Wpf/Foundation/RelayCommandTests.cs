#nullable enable
using System;
using Xunit;
using FluentAssertions;
using FranceJudo.UI.Wpf.Foundation;

namespace FranceJudo.UI.Tests.Wpf.Foundation
{
    public class RelayCommandTests
    {
        [Fact]
        public void Constructeur_SansCondition_CanExecuteRetourneToujoursTrue()
        {
            // Arrange
            var command = new RelayCommand(param => { /* Action vide */ });

            // Act & Assert
            command.CanExecute(null!).Should().BeTrue("Sans condition définie, la commande doit toujours pouvoir s'exécuter.");
            command.CanExecute("Un paramètre").Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecute_AvecCondition_RetourneLeResultatDeLaCondition(bool conditionAttendue)
        {
            // Arrange
            var command = new RelayCommand(
                execute: param => { },
                canExecute: param => conditionAttendue
            );

            // Act & Assert
            command.CanExecute(null!).Should().Be(conditionAttendue, "La commande doit respecter le retour de la fonction CanExecute fournie.");
        }

        [Fact]
        public void Execute_DeclencheLAction_AvecLeBonParametre()
        {
            // Arrange
            string? parametreRecu = null;

            // On stocke le paramètre reçu dans notre variable locale lors de l'exécution
            var command = new RelayCommand(param => parametreRecu = param?.ToString());

            // Act
            command.Execute("ValeurDeTest");

            // Assert
            parametreRecu.Should().Be("ValeurDeTest", "La commande doit transmettre le paramètre exact à l'action sous-jacente.");
        }

        [Fact]
        public void CanExecute_ParametreEstTransmisALaCondition()
        {
            // Arrange
            int valeurRecue = 0;
            var command = new RelayCommand(
                execute: param => { },
                canExecute: param =>
                {
                    if (param is int i) valeurRecue = i;
                    return true;
                }
            );

            // Act
            command.CanExecute(42);

            // Assert
            valeurRecue.Should().Be(42, "Le paramètre doit être transmis correctement à la fonction d'évaluation.");
        }
    }
}