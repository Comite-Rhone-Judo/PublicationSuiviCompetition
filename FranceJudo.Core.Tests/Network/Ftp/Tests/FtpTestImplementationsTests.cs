#nullable enable
using System;
using System.Threading;
using FluentFTP;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Ftp.Test;

namespace FranceJudo.Core.Tests.Network.Ftp.Test
{
    public class FtpTestImplementationsTests
    {
        // Bouchon (Stub) minimaliste pour simuler la configuration
        private class StubFtpConfig : IFtpConfiguration
        {
            public string Host { get; set; } = "127.0.0.1";
            public string Username { get; set; } = "admin";
            public string Password { get; set; } = "secret";
            public string RemotePath { get; set; } = "/";
            public bool UseActiveMode { get; set; } = false;
            public FtpProfile CurrentProfile { get; set; } = new FtpProfile();

            public bool ProfileResolveResult { get; set; } = true;

            public bool ResolveProfile(FtpClient client) => ProfileResolveResult;
        }

        [Fact]
        public void DnsResolutionTest_HostValide_RetourneTrue()
        {
            var test = new DnsResolutionTest();
            var config = new StubFtpConfig { Host = "localhost" }; // Toujours résolvable

            bool result = test.Execute(config, new FtpClient(), CancellationToken.None);

            result.Should().BeTrue("La résolution DNS de 'localhost' doit réussir.");
            test.SuccessMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void DnsResolutionTest_HostInvalide_IntercepteErreurEtRetourneFalse()
        {
            var test = new DnsResolutionTest();
            var config = new StubFtpConfig { Host = "domaine-invalide-impossible-a-resoudre-123.local" };

            bool result = test.Execute(config, new FtpClient(), CancellationToken.None);

            result.Should().BeFalse("Le catch doit intercepter l'échec de Dns.GetHostAddresses.");
            test.ErrorMessage.Should().Contain("Erreur DNS");
        }

        [Fact]
        public void ProfileCheckTest_ResolutionReussie_RetourneTrue()
        {
            var test = new ProfileCheckTest();
            var config = new StubFtpConfig { ProfileResolveResult = true };

            bool result = test.Execute(config, new FtpClient(), CancellationToken.None);

            result.Should().BeTrue();
            test.SuccessMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ConnectionTest_ServeurInjoignable_IntercepteErreurEtRetourneFalse()
        {
            var test = new ConnectionTest();
            var config = new StubFtpConfig();
            // On tente une vraie connexion sur un port fermé
            using var client = new FtpClient("127.0.0.1", "user", "pass", 55555);

            bool result = test.Execute(config, client, CancellationToken.None);

            result.Should().BeFalse("Le client.Connect() doit échouer et l'exception doit être gérée.");
            test.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void DisconnectTest_NePlantePasSiDejaDeconnecte()
        {
            var test = new DisconnectTest();
            using var client = new FtpClient(); // Non connecté

            bool result = test.Execute(new StubFtpConfig(), client, CancellationToken.None);

            result.Should().BeTrue("La méthode doit vérifier IsConnected avant de déconnecter, évitant tout crash.");
            test.SuccessMessage.Should().NotBeNullOrEmpty();
        }
    }
}