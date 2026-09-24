#nullable enable
using System;
using System.Linq;
using System.Net;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Scanner;

namespace FranceJudo.Core.Tests.Network.Scanner
{
    public class NetworkCalculatorTests
    {
        [Fact]
        public void GetUsableIps_ClasseC_Standard_Retourne254Adresses()
        {
            // Arrange
            var ip = IPAddress.Parse("192.168.1.50");
            var mask = IPAddress.Parse("255.255.255.0"); // Réseau /24

            // Act
            var usableIps = NetworkCalculator.GetUsableIps(ip, mask).ToList();

            // Assert
            usableIps.Should().HaveCount(254, "Un sous-réseau /24 a toujours 254 adresses utilisables (hors réseau et broadcast).");
            usableIps.First().Should().Be("192.168.1.1");
            usableIps.Last().Should().Be("192.168.1.254");
        }

        [Fact]
        public void GetUsableIps_PetitSousReseau_ClasseC_RetourneLesBonnesBornes()
        {
            // Arrange
            var ip = IPAddress.Parse("10.0.0.130");
            var mask = IPAddress.Parse("255.255.255.128"); // Réseau /25 (128 adresses)

            // Act
            var usableIps = NetworkCalculator.GetUsableIps(ip, mask).ToList();

            // Assert
            usableIps.Should().HaveCount(126, "Un sous-réseau /25 laisse 126 adresses utilisables.");
            usableIps.First().Should().Be("10.0.0.129");
            usableIps.Last().Should().Be("10.0.0.254");
        }

        [Fact]
        public void GetUsableIps_ReseauTresPetit_Slash30_Retourne2Adresses()
        {
            // Arrange : Réseau point-à-point typique
            var ip = IPAddress.Parse("172.16.0.2");
            var mask = IPAddress.Parse("255.255.255.252");

            // Act
            var usableIps = NetworkCalculator.GetUsableIps(ip, mask).ToList();

            // Assert
            usableIps.Should().HaveCount(2);
            usableIps.Should().Contain("172.16.0.1");
            usableIps.Should().Contain("172.16.0.2");
        }
    }
}