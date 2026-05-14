using System;
using System.Security;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Security;

namespace FranceJudo.Core.Tests.Security
{
    public class EncryptionTests
    {
        #region Tests - Conversions SecureString

        [Fact]
        public void SecureString_Conversion_EstSymetrique()
        {
            // Arrange
            string motDePasse = "Judo@2026_Secret!";

            // Act
            SecureString secureStr = Encryption.ToSecureString(motDePasse);
            string resultStr = Encryption.ToInsecureString(secureStr);

            // Assert
            secureStr.Should().NotBeNull();
            // Ton code verrouille intelligemment la chaine, on le vérifie
            secureStr.IsReadOnly().Should().BeTrue();
            resultStr.Should().Be(motDePasse);
        }

        [Fact]
        public void ToInsecureString_InputNull_RetourneChaineVide()
        {
            // Act
            string result = Encryption.ToInsecureString(null!);

            // Assert
            result.Should().Be(string.Empty);
        }

        #endregion

        #region Tests - Chiffrement DPAPI

        [Fact]
        public void EncryptString_InputNull_RetourneChaineVide()
        {
            // Act
            string result = Encryption.EncryptString(null!);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void Chiffrement_Dechiffrement_EstSymetrique()
        {
            // Arrange
            string donneeSensible = "Identifiant_API_FFJudo";
            SecureString secureInput = Encryption.ToSecureString(donneeSensible);

            // Act
            string encryptedBase64 = Encryption.EncryptString(secureInput);
            SecureString decryptedSecure = Encryption.DecryptString(encryptedBase64);
            string decryptedString = Encryption.ToInsecureString(decryptedSecure);

            // Assert
            encryptedBase64.Should().NotBeNullOrEmpty();
            // On s'assure que la chaine chiffrée ne contient pas le texte en clair
            encryptedBase64.Should().NotContain(donneeSensible);

            decryptedString.Should().Be(donneeSensible);
        }

        [Fact]
        public void DecryptString_DonneeCorrompue_RetourneSecureStringVide()
        {
            // Arrange
            string corruptedData = "CeciN'estPasDuBase64Valide!!";

            // Act
            SecureString decryptedSecure = Encryption.DecryptString(corruptedData);
            string decryptedString = Encryption.ToInsecureString(decryptedSecure);

            // Assert
            // Ton bloc catch retourne un new SecureString() vide, on le valide
            decryptedSecure.Should().NotBeNull();
            decryptedString.Should().Be(string.Empty);
        }

        #endregion
    }
}