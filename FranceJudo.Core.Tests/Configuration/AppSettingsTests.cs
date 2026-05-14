#nullable enable
using System;
using System.Configuration;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Configuration;
using FranceJudo.Core.Security; // Requis pour l'encryption

namespace FranceJudo.Core.Tests.Configuration
{
    // Mode séquentiel obligatoire quand on touche au fichier app.config physique
    [Collection("ConfigurationSequential")]
    public class AppSettingsTests : IDisposable
    {
        private readonly string _testKey;
        private readonly System.Configuration.Configuration _config;

        public AppSettingsTests()
        {
            _testKey = "TEST_KEY_" + Guid.NewGuid().ToString("N");
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public void Dispose()
        {
            // Nettoyage physique du fichier app.config après chaque test
            if (_config.AppSettings.Settings[_testKey] != null)
            {
                _config.AppSettings.Settings.Remove(_testKey);
                _config.Save(ConfigurationSaveMode.Modified);
            }
        }

        [Fact]
        public void SaveSetting_Puis_ReadSetting_RetourneLaValeurExacte()
        {
            // Arrange
            string valeurAttendue = "Judo2026";

            // Act
            AppSettings.SaveSetting(_testKey, valeurAttendue);
            string resultat = AppSettings.ReadSetting(_testKey, "Defaut");

            // Assert
            resultat.Should().Be(valeurAttendue);
        }

        [Fact]
        public void ReadSetting_CleAbsente_RetourneValeurParDefaut()
        {
            // Act
            int resultat = AppSettings.ReadSetting("CLE_FANTOME", 42);

            // Assert
            resultat.Should().Be(42);
        }

        [Fact]
        public void SaveEncryptedSetting_EcritUneChaineChiffree_Et_ReadLaDechiffre()
        {
            // Arrange
            string valeurClaire = "MotDePasseSecret";

            // Act
            AppSettings.SaveEncryptedSetting(_testKey, valeurClaire);

            // On vérifie que le fichier contient bien une valeur chiffrée (différente du clair)
            string valeurBruteFichier = AppSettings.ReadRawSetting(_testKey)!;

            string resultatDechiffre = AppSettings.ReadEncryptedSetting(_testKey, "Defaut");

            // Assert
            valeurBruteFichier.Should().NotBeNull();
            valeurBruteFichier.Should().NotBe(valeurClaire, "La valeur écrite physiquement doit être chiffrée.");
            resultatDechiffre.Should().Be(valeurClaire, "La lecture doit déchiffrer correctement la chaîne.");
        }
    }
}