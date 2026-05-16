#nullable enable
using Xunit;
using FluentAssertions;
using JudoClient;

namespace JudoClient.Tests
{
    public class RechercheServeurJudoTests
    {
        [Theory]
        // [Adresse IP standard, Valeur numérique UInt32 correspondante]
        [InlineData("192.168.1.1", 3232235777)]
        [InlineData("255.255.255.255", 4294967295)]
        [InlineData("0.0.0.0", 0)]
        [InlineData("10.0.0.5", 167772165)]
        [InlineData("127.0.0.1", 2130706433)]
        public void ParseIp_Et_ToIpString_SontReversiblesEtCorrects(string ipString, uint expectedInt)
        {
            // Arrange
            var recherche = new RechercheServeurJudo();

            // Act 1 : Conversion String -> Entier (ParseIp)
            uint parsedIp = recherche.ParseIp(ipString);

            // Assert 1
            parsedIp.Should().Be(expectedInt, "L'adresse IP doit être correctement convertie en son équivalent numérique binaire (UInt32).");

            // Act 2 : Conversion Entier -> String (ToIpString)
            string reversedIp = recherche.ToIpString(parsedIp);

            // Assert 2
            reversedIp.Should().Be(ipString, "L'entier numérique doit être correctement reformaté en adresse IP (Notation décimale à point).");
        }
    }
}