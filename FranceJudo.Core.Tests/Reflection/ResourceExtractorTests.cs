using System;
using System.IO;
using System.Reflection;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Export;
using FranceJudo.Core.Reflection;

namespace FranceJudo.Core.Tests.Reflection
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
        public void ExtractToFile_DictionaryNull_LeveException()
        {
            // Arrange
            string targetFile = Path.Combine(_tempDirectory, "output.xslt");

            // Act
            Action act = () => ResourceExtractor.ExtractToFile(null!, "resource", targetFile);

            // Assert
            act.Should().Throw<NullReferenceException>("L'absence de vérification null du dictionnaire doit lever cette exception native.");
        }

        [Fact]
        public void ExtractToFile_ExtractionValide_CreeDossierEtFichier()
        {
            // Arrange
            // Astuce : On récupère le premier assembly chargé en mémoire qui possède au moins une ressource
            var assemblyAvecRessource = System.Linq.Enumerable.FirstOrDefault(
                AppDomain.CurrentDomain.GetAssemblies(),
                a => !a.IsDynamic && a.GetManifestResourceNames().Length > 0);

            if (assemblyAvecRessource == null) return; // Sécurité

            var dict = new AssemblyResourceDictionary(assemblyAvecRessource);
            string resourceToExtract = dict.AllResources.First();

            // On force un sous-dossier qui n'existe pas pour tester Directory.CreateDirectory
            string targetFile = Path.Combine(_tempDirectory, "NouveauDossier", "extract.bin");

            // Act
            bool result = ResourceExtractor.ExtractToFile(dict, resourceToExtract, targetFile);

            // Assert
            result.Should().BeTrue();
            File.Exists(targetFile).Should().BeTrue("Le fichier doit avoir été créé physiquement sur le disque.");
        }

        [Fact]
        public void ExtractToFile_ErreurEcriture_AttrapeExceptionEtRetourneFalse()
        {
            // Arrange
            var assemblyAvecRessource = System.Linq.Enumerable.FirstOrDefault(
                AppDomain.CurrentDomain.GetAssemblies(),
                a => !a.IsDynamic && a.GetManifestResourceNames().Length > 0);

            if (assemblyAvecRessource == null) return;

            var dict = new AssemblyResourceDictionary(assemblyAvecRessource);
            string resourceToExtract = dict.AllResources.First();

            // Astuce pour déclencher le bloc 'catch' : 
            // On essaie d'écrire dans un répertoire (UnauthorizedAccessException) au lieu d'un fichier.
            string targetFile = _tempDirectory;

            // Act
            bool result = ResourceExtractor.ExtractToFile(dict, resourceToExtract, targetFile);

            // Assert
            result.Should().BeFalse("L'exception de droits ou d'accès disque doit être attrapée et retourner false.");
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