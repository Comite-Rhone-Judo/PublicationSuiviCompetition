using FranceJudo.Core.Configuration.Json;
using Newtonsoft.Json;
using Xunit;

namespace AppPublication.Tests.Config.Json
{
    public class EncryptedStringConverterTests
    {
        // Classe bouchon (Dummy) pour simuler un modèle de configuration JSON
        // Classe bouchon (Dummy) pour simuler un modèle de configuration JSON
        private class DummyConfig
        {
            [JsonConverter(typeof(EncryptedStringConverter))]
            public string Secret { get; set; } = string.Empty; // <-- Ajout de l'initialisation
        }

        [Fact]
        public void Serialize_ChiffreLaValeur()
        {
            // Arrange
            var config = new DummyConfig { Secret = "MonMotDePasse" };

            // Act
            string json = JsonConvert.SerializeObject(config);

            // Assert
            Assert.Contains("Secret", json);
            Assert.DoesNotContain("MonMotDePasse", json); // Le texte en clair ne doit pas fuiter dans le JSON
        }

        [Fact]
        public void Deserialize_DechiffreLaValeur()
        {
            // Arrange
            var config = new DummyConfig { Secret = "SuperSecret123" };
            string json = JsonConvert.SerializeObject(config);

            // Act
            var result = JsonConvert.DeserializeObject<DummyConfig>(json);

            // Assert
            Assert.Equal("SuperSecret123", result?.Secret);
        }

        [Fact]
        public void ChainesVides_SontGereesCorrectement()
        {
            // Arrange
            var config = new DummyConfig { Secret = string.Empty };

            // Act
            string json = JsonConvert.SerializeObject(config);
            var result = JsonConvert.DeserializeObject<DummyConfig>(json);

            // Assert
            Assert.Equal(string.Empty, result?.Secret);
        }

        [Fact]
        public void Deserialize_ChaineInvalideOuCorrompue_RetourneChaineVide()
        {
            // Arrange : on simule un JSON avec une chaîne corrompue (ou venant d'un autre PC/User)
            string json = "{\"Secret\":\"PasDuBase64NiDuDPAPIValide!\"}";

            // Act
            var result = JsonConvert.DeserializeObject<DummyConfig>(json);

            // Assert : Le try/catch du convertisseur doit absorber le plantage de DPAPI et renvoyer string.Empty
            Assert.Equal(string.Empty, result?.Secret);
        }
    }
}