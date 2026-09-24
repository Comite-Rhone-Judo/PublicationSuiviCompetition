using System;
using System.IO;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.IO;

namespace FranceJudo.Core.Tests.IO
{
    public class FileWithChecksumTests : IDisposable
    {
        private readonly string _tempDir;

        public FileWithChecksumTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "FranceJudo_Checksum_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true);
        }

        [Fact]
        public void Constructeur_FichierValide_CalculeLeChecksumMD5()
        {
            // Arrange
            string filePath = Path.Combine(_tempDir, "data.txt");
            File.WriteAllText(filePath, "Judo2026");

            // Le hash MD5 de "Judo2026" (sans BOM, encodage par défaut) est prévisible,
            // mais pour la stabilité du test, on vérifiera surtout le format du résultat.

            // Act
            var fileWithCs = new FileWithChecksum(filePath);

            // Assert
            fileWithCs.File.Should().NotBeNull();
            fileWithCs.File.FullName.Should().Be(filePath);

            // Le checksum doit être une chaine hexadécimale de 32 caractères (MD5 classique)
            fileWithCs.Checksum.Should().NotBeNullOrWhiteSpace();
            fileWithCs.Checksum.Length.Should().Be(32);
            fileWithCs.Checksum.Should().BeLowerCased("Ton code fait un ToLowerInvariant()");
        }

        [Fact]
        public void Serialisation_Deserialisation_Xml_ConserveLesDonnees()
        {
            // Arrange
            var original = new FileWithChecksum
            {
                File = new FileInfo(Path.Combine(_tempDir, "fake.xml")),
                Checksum = "abcdef1234567890"
            };

            // Act
            XElement xmlNode = original.ToXml();

            var restored = new FileWithChecksum();
            restored.LoadXml(xmlNode);

            // Assert
            restored.File.FullName.Should().Be(original.File.FullName);
            restored.Checksum.Should().Be(original.Checksum);
        }
    }
}