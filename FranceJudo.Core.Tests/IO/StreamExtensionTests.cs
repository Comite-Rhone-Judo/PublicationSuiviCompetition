using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.IO;

namespace FranceJudo.Core.Tests.IO
{
    public class StreamExtensionTests
    {
        [Fact]
        public void ReadAllBytes_FluxRempli_ExtraitLeTableauComplet()
        {
            // Arrange
            byte[] dataAttendu = new byte[] { 0x01, 0x02, 0x03, 0xFF };
            using var memoryStream = new MemoryStream(dataAttendu);

            // Act
            byte[] result = memoryStream.ReadAllBytes();

            // Assert
            result.Should().BeEquivalentTo(dataAttendu, "L'extension doit lire l'intégralité des octets du flux.");
        }

        [Fact]
        public void ReadAllBytes_FluxVide_RetourneTableauVide()
        {
            // Arrange
            using var memoryStream = new MemoryStream();

            // Act
            byte[] result = memoryStream.ReadAllBytes();

            // Assert
            result.Should().BeEmpty();
        }
    }
}