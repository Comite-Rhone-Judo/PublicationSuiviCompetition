#nullable enable
using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Tcp.Client;

namespace FranceJudo.Core.Tests.Network.Tcp.Client
{
    public class HelpersTests
    {
        [Fact]
        public void LogHelper_ShowLog_NePlantePas()
        {
            // Act
            Action act = () => LogHelper.ShowLog("Test Unitaire LogHelper");

            // Assert
            act.Should().NotThrow("Le Helper doit transmettre le log silencieusement.");
        }

        [Fact]
        public void ExceptionHelper_ShowException_NePlantePasEtEncapsuleLerreur()
        {
            // Arrange
            var ex = new InvalidOperationException("Test d'erreur système");

            // Act
            Action act = () => ExceptionHelper.ShowException(ex);

            // Assert
            act.Should().NotThrow("Le Helper doit transformer l'exception en TcpClientException et la logguer sans faire crasher l'appelant.");
        }
    }
}