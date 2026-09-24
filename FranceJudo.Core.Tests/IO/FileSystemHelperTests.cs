using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.IO;

namespace FranceJudo.Core.Tests.IO
{
    public class FileSystemHelperTests : IDisposable
    {
        private readonly string _tempDir;

        public FileSystemHelperTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "FranceJudo_FSHelper_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true);
        }

        #region Tests - Encodage (BOM)

        [Theory]
        [InlineData(new byte[] { 0xef, 0xbb, 0xbf, 0x41 }, "utf-8")]
        [InlineData(new byte[] { 0xfe, 0xff, 0x00, 0x41 }, "utf-16BE")]
        [InlineData(new byte[] { 0xff, 0xfe, 0x41, 0x00 }, "utf-16")]
        [InlineData(new byte[] { 0x00, 0x00, 0xfe, 0xff }, "utf-32")]
        [InlineData(new byte[] { 0x41, 0x42, 0x43, 0x44 }, null)] // null pour l'encodage par défaut
        public void GetFileEncoding_LitLesBOM_RetourneLeBonEncodage(byte[] fileBytes, string? expectedWebName)
        {
            // Arrange
            string filePath = Path.Combine(_tempDir, Guid.NewGuid() + ".txt");
            File.WriteAllBytes(filePath, fileBytes);

            // Act
            Encoding detected = FileSystemHelper.GetFileEncoding(filePath);

            // Assert
            if (expectedWebName != null)
            {
                detected.WebName.Should().Be(expectedWebName, $"L'encodage détecté pour {expectedWebName} est incorrect.");
            }
            else
            {
                detected.Should().Be(Encoding.Default);
            }
        }

        [Fact]
        public void GetFileEncoding_FichierInexistant_RetourneDefault()
        {
            // Act
            Encoding result = FileSystemHelper.GetFileEncoding("Z:\\Fichier_Qui_N_Existe_Pas.txt");

            // Assert
            result.Should().Be(Encoding.Default);
        }

        #endregion

        #region Tests - Verrous Système et Logiques

        [Fact]
        public void IsFileLocked_FichierOuvertExclusif_RetourneTrue()
        {
            // Arrange
            string filePath = Path.Combine(_tempDir, "locked.txt");
            File.WriteAllText(filePath, "test");

            // Act & Assert
            FileSystemHelper.IsFileLocked(filePath).Should().BeFalse("Le fichier n'est pas encore verrouillé");

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                // Le FileShare.None verrouille le fichier au niveau de l'OS
                FileSystemHelper.IsFileLocked(filePath).Should().BeTrue("L'OS doit refuser l'accès");
            }
        }

        [Fact]
        public void IsFileLocked_FichierInexistant_RetourneFalse()
        {
            // Arrange
            string pathAbsent = Path.Combine(_tempDir, "fichier_fantome.txt");

            // Act
            bool result = FileSystemHelper.IsFileLocked(pathAbsent);

            // Assert
            result.Should().BeFalse("Un fichier qui n'existe pas ne peut pas être verrouillé.");
        }

        [Fact]
        public async Task NeedAccessFile_AccesConcurrentinterne_MetEnAttenteLeSecondThread()
        {
            // Arrange
            string filePath = Path.Combine(_tempDir, "concurrent.xml");
            File.WriteAllText(filePath, "data");
            bool thread2AAttendu = false;

            // Act
            // Le Thread 1 prend le verrou logique
            FileSystemHelper.NeedAccessFile(filePath);

            var task2 = Task.Run(async () =>
            {
                // Le Thread 2 va bloquer ici car le Thread 1 a le sémaphore logique
                FileSystemHelper.NeedAccessFile(filePath);
                thread2AAttendu = true;
                FileSystemHelper.ReleaseFile(filePath);
            }, TestContext.Current.CancellationToken);

            // On attend un peu pour prouver que la tâche 2 est bien bloquée
            await Task.Delay(200, TestContext.Current.CancellationToken);
            thread2AAttendu.Should().BeFalse("Le Thread 2 aurait dû rester bloqué sur NeedAccessFile");

            // Le Thread 1 relâche enfin
            FileSystemHelper.ReleaseFile(filePath);

            // On attend la fin de la tâche 2
            await task2;

            // Assert
            thread2AAttendu.Should().BeTrue("Le Thread 2 a dû pouvoir passer une fois le fichier libéré");
        }

        #endregion

        #region Tests - Suppression sécurisée

        [Fact]
        public void DeleteDirectory_OnlyContent_GardeLaRacine()
        {
            // Arrange
            string rootDir = Path.Combine(_tempDir, "Racine");
            Directory.CreateDirectory(rootDir);
            Directory.CreateDirectory(Path.Combine(rootDir, "SousDossier"));
            File.WriteAllText(Path.Combine(rootDir, "fichier.txt"), "A detruire");

            // Act
            bool result = FileSystemHelper.DeleteDirectory(rootDir, onlyContent: true);

            // Assert
            result.Should().BeTrue();
            Directory.Exists(rootDir).Should().BeTrue("La racine ne doit pas être supprimée");
            Directory.GetFiles(rootDir).Should().BeEmpty("Le contenu doit être purgé");
            Directory.GetDirectories(rootDir).Should().BeEmpty();
        }

        #endregion
    }
}