#nullable enable
using AppPublication.Tools.Converter;
using System.Globalization;
using Xunit;
using static AppPublication.Models.EcransAppel.EcranAppelModel;

namespace AppPublication.Tests.Tools.Converter
{
    public class MultiValueConvertersTests
    {
        [Fact]
        public void DisplayURLStatusConverter_TableauValide_RetourneUrlOuNA()
        {
            // Arrange
            DisplayURLStatusConverter convertisseur = new DisplayURLStatusConverter();
            object[] valeursActives = new object[] { "http://mon-site.fr", true };
            object[] valeursInactives = new object[] { "http://mon-site.fr", false };
            object[] valeursIncompletes = new object[] { "http://mon-site.fr" };

            // Act & Assert
            Assert.Equal("http://mon-site.fr", convertisseur.Convert(valeursActives, typeof(string), null!, CultureInfo.InvariantCulture));
            Assert.Equal("N/A", convertisseur.Convert(valeursInactives, typeof(string), null!, CultureInfo.InvariantCulture));
            Assert.Equal("URL indéfinie", convertisseur.Convert(valeursIncompletes, typeof(string), null!, CultureInfo.InvariantCulture));
        }

        [Theory]
        [InlineData(1, DispositionAffichage.Colonne, "1x1 (1 tapis)")]
        [InlineData(2, DispositionAffichage.Ligne, "2x1 (2 tapis)")]
        [InlineData(6, DispositionAffichage.Colonne, "2x3 (6 tapis)")]
        [InlineData(8, DispositionAffichage.Ligne, "4x2 (8 tapis)")]
        [InlineData(99, DispositionAffichage.Colonne, "99 (99 tapis)")] // Test du fallback
        public void GroupementToLayoutConverter_Convert_FormateLaGrille(int totalTapis, DispositionAffichage disposition, string attendu)
        {
            // Arrange
            GroupementToLayoutConverter convertisseur = new GroupementToLayoutConverter();
            object[] valeurs = new object[] { totalTapis, disposition };

            // Act
            object resultat = convertisseur.Convert(valeurs, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(attendu, resultat);
        }

        [Fact]
        public void GroupementToLayoutConverter_ValeursInvalides_RetourneChaineVide()
        {
            // Arrange
            GroupementToLayoutConverter convertisseur = new GroupementToLayoutConverter();

            // Act
            object resultat = convertisseur.Convert(new object[] { "pas un int", "pas une enum" }, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("pas un int", resultat);
        }
    }
}