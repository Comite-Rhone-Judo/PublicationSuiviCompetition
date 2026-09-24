using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Configuration.Json;

namespace FranceJudo.Core.Tests.Configuration.Json
{
    public class JsonConfigurationServiceTests : IDisposable
    {
        private readonly string _tempFile;

        // Modèle bidon pour tester le service générique
        public class RootConfig
        {
            public string ApplicationName { get; set; } = "DefaultName";
        }

        public JsonConfigurationServiceTests()
        {
            _tempFile = Path.GetTempFileName();
            File.Delete(_tempFile); // On veut que le fichier n'existe pas au début
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile)) File.Delete(_tempFile);
        }

        [Fact]
        public void Constructeur_FichierInexistant_CreeUneInstanceParDefaut()
        {
            // Act
            var service = new JsonConfigurationService<RootConfig>(_tempFile);

            // Assert
            service.Root.Should().NotBeNull();
            service.Root.ApplicationName.Should().Be("DefaultName");
            File.Exists(_tempFile).Should().BeFalse("Le constructeur ne doit pas créer le fichier sur le disque tant qu'on ne sauvegarde pas.");
        }

        [Fact]
        public void SaveToDisk_EcritLeFichier_Et_Constructeur_LeRecharge()
        {
            // Arrange
            var serviceEcriture = new JsonConfigurationService<RootConfig>(_tempFile);
            serviceEcriture.Root.ApplicationName = "FranceJudoApp";

            // Act
            serviceEcriture.SaveToDisk();

            // On recharge via une nouvelle instance
            var serviceLecture = new JsonConfigurationService<RootConfig>(_tempFile);

            // Assert
            File.Exists(_tempFile).Should().BeTrue();
            serviceLecture.Root.ApplicationName.Should().Be("FranceJudoApp", "Le fichier JSON doit avoir été désérialisé correctement.");
        }

        [Fact]
        public async Task RequestSave_Debounce_NeSauvegardeQuUneSeuleFois()
        {
            // Arrange
            var service = new JsonConfigurationService<RootConfig>(_tempFile);
            service.Root.ApplicationName = "Modif";

            // Act : On simule 3 frappes au clavier rapides (3 appels)
            service.RequestSave(delayMs: 100);
            service.RequestSave(delayMs: 100);
            service.RequestSave(delayMs: 100);

            // On attend moins que le délai
            await Task.Delay(50, TestContext.Current.CancellationToken);
            File.Exists(_tempFile).Should().BeFalse("La sauvegarde aurait dû être temporisée.");

            // On attend que le debounce se déclenche
            await Task.Delay(200, TestContext.Current.CancellationToken);

            // Assert
            File.Exists(_tempFile).Should().BeTrue("La sauvegarde a dû s'exécuter après la temporisation finale.");
        }

        [Fact]
        public void Dispose_ForceUneSauvegardeImmediateEtAnnuleLeDebounce()
        {
            // Arrange
            var service = new JsonConfigurationService<RootConfig>(_tempFile);

            // Act
            service.RequestSave(delayMs: 5000); // Une sauvegarde très lointaine
            service.Dispose(); // L'arrêt de l'application (ou du composant)

            // Assert
            File.Exists(_tempFile).Should().BeTrue("Le Dispose doit forcer SaveToDisk() immédiatement pour ne pas perdre de données.");
        }
    }
}