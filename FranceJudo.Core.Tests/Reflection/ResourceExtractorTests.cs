using System;
using System.IO;
using System.Reflection;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Export;
using FranceJudo.Core.Reflection;

namespace FranceJudo.Core.Tests.Export
{
    public class ResourceExtractorTests : IDisposable
    {
        private readonly AssemblyResourceDictionary _dict;
        private readonly string _tempDirectory;

        public ResourceExtractorTests()
        {
            _dict = new AssemblyResourceDictionary(Assembly.GetExecutingAssembly());

            // Création d'un dossier temporaire unique pour chaque test
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        public void Dispose()
        {
            // Nettoyage radical après chaque test
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        [Fact]
        public void ExtractToFile_RessourceIntrouvable_RetourneFalseEtNeCreeRien()
        {
            // Arrange
            string targetFile = Path.Combine(_tempDirectory, "output.xslt");

            // Act
            bool result = ResourceExtractor.ExtractToFile(_dict, "RessourceInexistante.xslt", targetFile);

            // Assert
            result.Should().BeFalse();
            File.Exists(targetFile).Should().BeFalse();
        }

        // Note de l'architecte : Pour tester un cas "True", il te faut une vraie ressource intégrée 
        // dans le projet de test. Si tu as suivi l'astuce du fichier "export.xslt" en "Ressource Incorporée" 
        // lors de l'étape InAssemblyUrlResolver, tu peux décommenter ce test :

        /*
        [Fact]
        public void ExtractToFile_RessourceValide_EcritSurLeDisque()
        {
            // Arrange
            string resourceName = "FranceJudo.Core.Tests.Resources.export.xslt"; // À adapter selon ton namespace
            string targetFile = Path.Combine(_tempDirectory, "sous_dossier", "vrai_export.xslt");

            // Act
            bool result = ResourceExtractor.ExtractToFile(_dict, resourceName, targetFile);

            // Assert
            result.Should().BeTrue();
            File.Exists(targetFile).Should().BeTrue();
        }
        */
    }
}