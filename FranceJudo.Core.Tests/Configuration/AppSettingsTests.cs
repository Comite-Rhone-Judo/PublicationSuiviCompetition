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

        [Fact]
        public void ReadSetting_BoolEtInt_ParsingInvalide_RetourneDefaut()
        {
            // Arrange
            AppSettings.SaveSetting("TestInt", "PasUnChiffre");
            AppSettings.SaveSetting("TestBool", "PasUnBool");

            // Act
            int resInt = AppSettings.ReadSetting("TestInt", 99);
            bool resBool = AppSettings.ReadSetting("TestBool", true);

            // Assert
            resInt.Should().Be(99, "Si le parsing Int échoue, on doit retourner la valeur par défaut.");
            resBool.Should().BeTrue("Si le parsing Bool échoue, on doit retourner la valeur par défaut.");
        }

        [Fact]
        public void FindSetting_RechercheDansListe_RetourneMatchOuDefaut()
        {
            // Arrange
            var liste = new[] { "Alpha", "Beta", "Gamma" };
            static string predicate(string s) => s; // Le prédicat renvoie simplement la chaîne

            // Act & Assert
            // 1. Cas nominal : la valeur existe
            AppSettings.FindSetting("Beta", liste, predicate).Should().Be("Beta");

            // 2. Cas fallback : la valeur n'existe pas, doit attraper l'exception (First() sur IEnumerable vide ou Where vide)
            // et retourner le tout premier élément de la liste ("Alpha").
            AppSettings.FindSetting("Omega", liste, predicate).Should().Be("Alpha");

            // 3. Cas sécurité : liste vide
            AppSettings.FindSetting("Alpha", Array.Empty<string>(), predicate).Should().BeNull("Une liste vide doit retourner null.");
            AppSettings.FindSetting("Alpha", null!, (Func<string, string>)predicate).Should().BeNull();
        }

        [Fact]
        public void ReadRawSetting_TypeGenerique_LitEnBaseEtChercheDansListe()
        {
            // Arrange
            string cle = "TestGenKey";
            AppSettings.SaveSetting(cle, "Beta");
            var liste = new[] { "Alpha", "Beta", "Gamma" };

            // Act
            var result = AppSettings.ReadRawSetting(cle, liste, s => s);

            // Assert
            result.Should().Be("Beta", "La méthode doit lire 'Beta' dans le fichier et le trouver dans la liste.");
        }
    }
}