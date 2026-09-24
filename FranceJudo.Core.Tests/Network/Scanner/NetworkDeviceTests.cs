#nullable enable
using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Scanner;

namespace FranceJudo.Core.Tests.Network.Scanner
{
    public class NetworkDeviceTests
    {
        [Fact]
        public void ToString_FormatageAlignementCorrect()
        {
            // Arrange
            var device = new NetworkDevice
            {
                IpAddress = "192.168.1.10",
                Hostname = "PC-Arbitrage-1",
                MacAddress = "00:1A:2B:3C:4D:5E",
                Category = DeviceType.WindowsPc
            };

            // Act
            string result = device.ToString();

            // Assert
            result.Should().Contain("192.168.1.10");
            result.Should().Contain("PC-Arbitrage-1");
            result.Should().Contain("WindowsPc");
            result.Should().Contain("MAC: 00:1A:2B:3C:4D:5E");
        }
    }
}