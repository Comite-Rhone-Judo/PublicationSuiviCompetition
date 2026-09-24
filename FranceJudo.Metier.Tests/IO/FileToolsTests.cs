#nullable enable
using System;
using System.IO;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.IO;

namespace FranceJudo.Metier.Tests.IO
{
    // On utilise la MÊME collection pour être sûr qu'ils ne tournent pas en même temps que AppDirectoryManagerTests
    [Collection("IO_Tests")]
    public class FileToolsTests : IDisposable
    {
        private readonly string _tempRootDir;

        public FileToolsTests()
        {
            _tempRootDir = Path.Combine(Path.GetTempPath(), "JudoFileToolsTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRootDir);

            // Prérequis absolu : Initialiser l'arborescence pour que SaveCOMDir existe !
            AppDirectoryManager.Initialize(_tempRootDir, "");
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempRootDir))
            {
                try { Directory.Delete(_tempRootDir, true); } catch { /* Ignorer en test */ }
            }
        }

        [Fact]
        public void SaveFile_EcritLeFichierXmlSurLeDisque()
        {
            // Arrange
            string fileType = "test_document";
            var doc = new XDocument(new XElement("Root", new XElement("Data", "ValeurTest")));

            string expectedFilePath = Path.Combine(AppDirectoryManager.SaveCOMDir, fileType + ".xml");

            // Act
            FileTools.SaveFile(doc, fileType);

            // Assert
            File.Exists(expectedFilePath).Should().BeTrue("Le fichier XML doit avoir été physiquement sauvegardé sur le disque.");

            string content = File.ReadAllText(expectedFilePath);
            content.Should().Contain("ValeurTest", "Le contenu XML sauvegardé doit correspondre à celui en mémoire.");
        }

        [Fact]
        public void SaveFile_EcraseLeFichierExistant()
        {
            // Arrange
            string fileType = "test_document_overwrite";
            var doc1 = new XDocument(new XElement("Root", "Version1"));
            var doc2 = new XDocument(new XElement("Root", "Version2"));

            string expectedFilePath = Path.Combine(AppDirectoryManager.SaveCOMDir, fileType + ".xml");

            // Act
            FileTools.SaveFile(doc1, fileType); // Première écriture
            FileTools.SaveFile(doc2, fileType); // Seconde écriture (Écrasement)

            // Assert
            File.Exists(expectedFilePath).Should().BeTrue();

            string content = File.ReadAllText(expectedFilePath);
            content.Should().Contain("Version2", "Le fichier doit contenir la dernière version sauvegardée.");
            content.Should().NotContain("Version1", "L'ancienne version doit avoir été écrasée.");
        }
    }
}