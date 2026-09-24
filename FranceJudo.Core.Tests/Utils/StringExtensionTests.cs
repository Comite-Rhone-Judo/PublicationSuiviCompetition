using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Utils;

namespace FranceJudo.Core.Tests.Utils
{
    public class StringExtensionTests
    {
        #region Tests - SafeSubstring

        [Theory]
        // --- Cas Nominaux ---
        [InlineData("Bonjour", 0, 4, "Bonj")]
        [InlineData("Bonjour", 3, 4, "jour")]
        // --- Cas aux limites ---
        [InlineData("Bonjour", 0, 10, "Bonjour")] // Demande plus que la longueur dispo
        [InlineData("Bonjour", 7, 2, "")]         // Start pile à la fin de la chaine
        [InlineData("Bonjour", 10, 2, "")]        // Start au-delà de la chaine
        [InlineData("Bonjour", -1, 2, "")]        // Start négatif
        [InlineData("", 0, 2, "")]                // Chaine vide
        [InlineData(null, 0, 2, "")]              // Chaine nulle
        public void SafeSubstring_DifferentsCas_RetourneResultatAttendu(string? valeur, int start, int longueur, string expected)
        {
            // Act
            string result = valeur.SafeSubstring(start, longueur);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region Tests - FormatPrenom

        [Theory]
        // --- Cas Nominaux ---
        [InlineData("TEDDY", "Teddy")]
        [InlineData("teddy", "Teddy")]
        [InlineData("david douillet", "David Douillet")] // Avec espace
        [InlineData("JEAN-LUC", "Jean-Luc")]             // Avec tiret
        [InlineData(" marie-claire ", "Marie-Claire")]   // Avec espaces superflus
        [InlineData("", "")]                             // Chaine vide
        public void FormatPrenom_CasNominaux_FormateCorrectement(string? input, string expected)
        {
            // Act
            string result = input.FormatPrenom();

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region Tests - ScanneTraiteLicence

        [Theory]
        // --- Cas Nominaux : La regex attend M/F, chiffres, lettres, chiffres ---
        [InlineData("M12012999ABCDE12", "M12012999ABCDE12")] // Déjà valide, pas de conversion

        // --- Cas Douchette (Clavier AZERTY en minuscules) ---
        // Remplacement : 'à'->0, '&'->1, 'é'->2, '-'->6, 'ç'->9
        [InlineData("M&éà&éçççABCDE&é", "M12012999ABCDE12")]

        // --- Cas aux limites ---
        [InlineData("NIMPORTEQUOI", "NIMPORTEQUOI")] // Ne matche pas la regex, retourne l'original
        [InlineData("", "")]                         // Chaine vide
        public void ScanneTraiteLicence_DifferentsCas_TraiteCorrectement(string? input, string expected)
        {
            // Act
            string result = input.ScanneTraiteLicence();

            // Assert
            result.Should().Be(expected);
        }

        #endregion
    }
}