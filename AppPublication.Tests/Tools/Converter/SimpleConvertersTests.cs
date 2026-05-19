#nullable enable
using AppPublication.Tools.Converter;
using FranceJudo.Core.Network.Scanner;
using System.Globalization;
using System.Windows;
using Xunit;

namespace AppPublication.Tests.Tools.Converter
{
    public class SimpleConvertersTests
    {
        [Theory]
        [InlineData(true, "Démarré")]
        [InlineData(false, "Arrêté")]
        [InlineData(null, "N/A")]
        [InlineData("chaine_invalide", "N/A")]
        public void RunningStatusConverter_Convert_RetourneLeBonEtat(object? entree, string attendu)
        {
            // Arrange
            RunningStatusConverter convertisseur = new RunningStatusConverter();

            // Act
            object? resultat = convertisseur.Convert(entree!, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(attendu, resultat);
        }

        [Theory]
        [InlineData("Objet", null, Visibility.Visible)]
        [InlineData(null, "Parametre", Visibility.Visible)]
        [InlineData("Objet", "Parametre", Visibility.Hidden)]
        [InlineData(null, null, Visibility.Hidden)] // <-- CORRECTION ICI : Le XOR implique que null/null donne Hidden
        public void ClientConverter_Convert_RetourneLaBonneVisibilite(object? valeur, object? parametre, Visibility attendu)
        {
            // Arrange
            ClientConverter convertisseur = new ClientConverter();

            // Act
            object? resultat = convertisseur.Convert(valeur!, typeof(Visibility), parametre!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(attendu, resultat);
        }

        [Theory]
        [InlineData(DeviceType.WindowsPc, "PC (Windows)")]
        [InlineData(DeviceType.Mac, "Mac (Apple)")]
        [InlineData(DeviceType.SmartTvOrStreaming, "Smart TV")]
        [InlineData(null, "")]
        public void DeviceTypeConverter_Convert_RetourneLeNomLisible(object? entree, string attendu)
        {
            // Arrange
            DeviceTypeConverter convertisseur = new DeviceTypeConverter();

            // Act
            object? resultat = convertisseur.Convert(entree!, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(attendu, resultat);
        }
    }
}