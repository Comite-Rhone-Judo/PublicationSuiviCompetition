#nullable enable
using System;
using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.IO;

namespace FranceJudo.Metier.Tests.IO
{
    // L'attribut Collection empêche l'exécution en parallèle des tests touchant à l'I/O statique
    [Collection("IO_Tests")]
    public class AppDirectoryManagerTests : IDisposable
    {
        private readonly string _tempRootDir;

        public AppDirectoryManagerTests()
        {
            // Création d'un répertoire racine unique pour le test
            _tempRootDir = Path.Combine(Path.GetTempPath(), "JudoAppDirTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRootDir);
        }

        public void Dispose()
        {
            // Nettoyage après le test
            if (Directory.Exists(_tempRootDir))
            {
                try { Directory.Delete(_tempRootDir, true); } catch { /* Ignorer en test */ }
            }
        }

        [Fact]
        public void Initialize_CreeLArborescenceEtDefinitLesProprietes()
        {
            // Act
            AppDirectoryManager.Initialize(_tempRootDir, "");

            // Assert : Vérification des propriétés statiques
            AppDirectoryManager.SaveDir.Should().StartWith(_tempRootDir, "La racine de sauvegarde doit être dans le dossier temporaire.");
            AppDirectoryManager.SaveCOMDir.Should().StartWith(_tempRootDir);
            AppDirectoryManager.RessourcesDir.Should().StartWith(_tempRootDir);
            AppDirectoryManager.ChecksumFile.Should().Be("checksum_fichiers_site.xml");

            AppDirectoryManager.ExtensionXML.Should().Be(".xml");
            AppDirectoryManager.ExtensionTXT.Should().Be(".txt");

            // Assert : Vérification de la création physique sur le disque
            Directory.Exists(AppDirectoryManager.SaveCOMDir).Should().BeTrue("Le dossier Save\\Com doit avoir été créé.");
            Directory.Exists(AppDirectoryManager.Logo1Dir).Should().BeTrue("Le dossier Logos\\Fédé doit avoir été créé.");
            Directory.Exists(AppDirectoryManager.MediaFlagsDir).Should().BeTrue("Le dossier flags doit avoir été créé.");
        }

        [Fact]
        public void GetExportDir_RetourneLeCheminCorrect()
        {
            // Arrange
            string racine = @"C:\JudoData";

            // Act
            string exportDir = AppDirectoryManager.GetExportDir(racine);

            // Assert
            exportDir.Should().Be(Path.Combine(racine, "FRANCE-JUDO"), "Le dossier d'export par défaut doit être calculé correctement.");
        }
    }
}