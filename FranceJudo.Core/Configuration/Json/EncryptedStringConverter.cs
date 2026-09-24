using Newtonsoft.Json;
using System;
using FranceJudo.Core.Security; // Espace de noms de votre classe de sécurité

namespace FranceJudo.Core.Configuration.Json
{
    public class EncryptedStringConverter : JsonConverter<string>
    {
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var encryptedText = (string)reader.Value;

            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            try
            {
                // 1. Déchiffre le texte base64 en SecureString
                using (var secureString = Encryption.DecryptString(encryptedText))
                {
                    // 2. Convertit le SecureString en string standard pour hydrater le modèle
                    return Encryption.ToInsecureString(secureString);
                }
            }
            catch
            {
                // En cas de corruption ou de changement d'utilisateur Windows
                return string.Empty;
            }
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.WriteValue(value);
                return;
            }

            // 1. Transforme la string en clair en SecureString
            using (var secureString = Encryption.ToSecureString(value))
            {
                // 2. Chiffre le SecureString en base64 via DPAPI
                string encryptedText = Encryption.EncryptString(secureString);

                // 3. Écrit la valeur chiffrée dans le JSON
                writer.WriteValue(encryptedText);
            }
        }
    }
}