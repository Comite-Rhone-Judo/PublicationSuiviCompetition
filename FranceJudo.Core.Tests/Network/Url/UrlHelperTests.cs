#nullable enable
using System;
using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Url;

namespace FranceJudo.Core.Tests.Network.Url
{
    public class UrlHelperTests
    {
        [Theory]
        [InlineData("Catégorie+Masc", "CatégoriepMasc")]
        [InlineData("+++", "ppp")]
        [InlineData("SansPlus", "SansPlus")]
        [InlineData("", "")]
        [InlineData(null, "")] // Validation directe via Theory
        public void TraiteChaineURL_RemplaceSignePlus_EtGereLaNullite(string? input, string expected)
        {
            // Act
            string resultat = input.TraiteChaineURL();

            // Assert
            resultat.Should().Be(expected, "La méthode doit convertir les '+' en 'p' et gérer la nullité en retournant string.Empty.");
        }

        [Theory]
        [InlineData("Judo France", "Judo_France")]
        [InlineData("  ", "__")]
        [InlineData("", "")]
        [InlineData(null, "")] // Validation directe via Theory
        public void TraiteChaine_RemplaceLesEspaces_EtGereLaNullite(string? input, string expected)
        {
            // Act
            string resultat = input.TraiteChaine();

            // Assert
            resultat.Should().Be(expected, "Les espaces doivent être convertis en underscores et null doit retourner string.Empty.");
        }

        [Fact]
        public void TraiteChaine_RemplaceLesCaracteresInvalidesDuSystemeDeFichiers()
        {
            // Arrange
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string invalidStr = new string(invalidChars, 0, Math.Min(3, invalidChars.Length));
            string input = $"Dossier{invalidStr}Test";
            string expected = "Dossier___Test";

            // Act
            string resultat = input.TraiteChaine();

            // Assert
            resultat.Should().Be(expected, "Chaque caractère invalide pour un nom de fichier doit devenir un underscore.");
        }

        [Theory]
        [InlineData("Élève", "Eleve")]
        [InlineData("àâäéèêëîïôöûüùç", "aaaeeeeiioouuuc")]
        [InlineData("ÊTRE", "ETRE")]
        public void TraiteChaine_SupprimeLesDiacritiques_Accents(string input, string expected)
        {
            // Act
            string resultat = input.TraiteChaine();

            // Assert
            resultat.Should().Be(expected, "La normalisation Unicode doit retirer les accents proprement.");
        }

        [Fact]
        public void Extensions_TestExpliciteSurNull_RetourneChaineVide()
        {
            // Ce test dédié garantit que le garde-fou a bien été implémenté pour les deux méthodes
            // Arrange
            string? input = null;

            // Act
            string resultUrl = input.TraiteChaineURL();
            string resultPath = input.TraiteChaine();

            // Assert
            resultUrl.Should().BeEmpty("Le garde-fou de TraiteChaineURL doit intercepter null et renvoyer une chaîne vide.");
            resultPath.Should().BeEmpty("Le garde-fou de TraiteChaine doit intercepter null et renvoyer une chaîne vide.");
        }
    }
}