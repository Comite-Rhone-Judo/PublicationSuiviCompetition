#nullable enable
using AppPublication.Statistiques;
using AppPublication.Tools.Converter;
using System;
using System.Globalization;
using Xunit;

namespace AppPublication.Tests.Tools.Converter
{
    public class StatGenerationConverterTests
    {
        [Fact]
        public void Convert_SiteNonGenere_RetourneTiret()
        {
            // Arrange
            StatGenerationConverter convertisseur = new StatGenerationConverter();
            object[] valeurs = new object[] { new TaskExecutionInformation(), false }; // genere = false

            // Act
            object resultat = convertisseur.Convert(valeurs, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("-", resultat);
        }

        [Fact]
        public void Convert_SansDateProchaine_FormateCorrectement()
        {
            // Arrange
            StatGenerationConverter convertisseur = new StatGenerationConverter();
            TaskExecutionInformation stat = new TaskExecutionInformation
            {
                DateDemarrage = new DateTime(2026, 1, 1, 14, 30, 00),
                DateProchaineGeneration = DateTime.MinValue,
                DelaiExecutionMs = 2600 // 3 secondes (arrondi mathématique de 2.5s)
            };
            object[] valeurs = new object[] { stat, true };

            // Act
            object resultat = convertisseur.Convert(valeurs, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("Dernière à 14:30:00 (en 3s)", resultat);
        }

        [Fact]
        public void Convert_AvecDateProchaine_FormateCompletement()
        {
            // Arrange
            StatGenerationConverter convertisseur = new StatGenerationConverter();
            TaskExecutionInformation stat = new TaskExecutionInformation
            {
                DateDemarrage = new DateTime(2026, 1, 1, 14, 30, 00),
                DateProchaineGeneration = new DateTime(2026, 1, 1, 14, 30, 30),
                DelaiExecutionMs = 1200 // 1 seconde
            };
            object[] valeurs = new object[] { stat, true };

            // Act
            object resultat = convertisseur.Convert(valeurs, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("Dernière à 14:30:00 (en 1s), Prochaine à 14:30:30", resultat);
        }
    }
}