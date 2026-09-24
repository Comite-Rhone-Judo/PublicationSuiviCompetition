using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.IO;

namespace FranceJudo.Core.Tests.IO
{
    public class FilteredFileInfoTests
    {
        [Fact]
        public void Constructeur_FichierValide_FiltreLeNomCorrectement()
        {
            // Arrange
            // Ton composant ResourcePath retire le préfixe spécifié (baseSuffix) du nom de fichier.
            var fileInfo = new FileInfo("FranceJudo.Config.Systeme.xml");
            string prefixToRemove = "FranceJudo.Config";

            // Act
            var filtered = new FilteredFileInfo(fileInfo, prefixToRemove);

            // Assert
            filtered.FullName.Should().Be(fileInfo.FullName);

            // Le prefixe "FranceJudo.Config." (avec le point) doit être retiré
            filtered.Name.Should().Be("Systeme.xml");
        }
    }
}