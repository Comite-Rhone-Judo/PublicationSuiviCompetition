using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Reflection;

namespace FranceJudo.Core.Tests.Reflection
{
    public class ResourcePathTests
    {
        #region Combine

        [Fact]
        public void Combine_PlusieursSegments_NettoieEtCombineCorrectement()
        {
            // Act
            string result = ResourcePath.Combine("FranceJudo", ".Metier.", "Resources", "logo.png");

            // Assert
            // Remarque que les points excédentaires du segment ".Metier." doivent être nettoyés
            result.Should().Be("FranceJudo.Metier.Resources.logo.png");
        }

        [Fact]
        public void Combine_SegmentsVidesOuNuls_SontIgnores()
        {
            // Act
            string result = ResourcePath.Combine("Dossier", null!, "", "Fichier.xml");

            // Assert
            result.Should().Be("Dossier.Fichier.xml");
        }

        #endregion

        #region GetRelativePath

        [Theory]
        [InlineData("FranceJudo.Resources.Images.logo.png", "FranceJudo.Resources.Images", "logo.png")]
        [InlineData("FranceJudo.Resources.Images.logo.png", "FranceJudo.Resources.Images.", "logo.png")] // Avec point final
        [InlineData("FranceJudo.AutreDossier.logo.png", "FranceJudo.Resources", "FranceJudo.AutreDossier.logo.png")] // Ne matche pas
        [InlineData("fichier_racine.xml", "", "fichier_racine.xml")] // Base path vide
        public void GetRelativePath_DiversCas_RetourneCheminAttendu(string fullPath, string basePath, string expected)
        {
            // Act
            string result = ResourcePath.GetRelativePath(fullPath, basePath);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region GuessFileName

        [Theory]
        // --- Cas Nominaux ---
        [InlineData("Dossier.Images.logo.png", true, "logo.png")]
        [InlineData("Dossier.Images.logo", false, "logo")]
        // --- Cas aux limites ---
        [InlineData("fichier_sans_dossier.xml", true, "fichier_sans_dossier.xml")]
        [InlineData("JusteUnMot", true, "JusteUnMot")] // Aucun point
        [InlineData("", true, "")]
        [InlineData(null, true, "")]
        // --- Le piège documenté ---
        [InlineData("Dossier.Scripts.jquery.min.js", true, "min.js")] // Prouve que ta doc dit vrai : ça tronque !
        public void GuessFileName_DiversCas_RetourneNomAttendu(string? path, bool hasExtension, string expected)
        {
            // Act
            string result = ResourcePath.GuessFileName(path!, hasExtension);

            // Assert
            result.Should().Be(expected);
        }

        #endregion
    }
}