using System;
using System.Reflection;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Environment;

namespace FranceJudo.Core.Tests.Environment
{
    public class AppEnvironmentTests
    {
        private readonly Assembly _coreAssembly;

        public AppEnvironmentTests()
        {
            // On cible dynamiquement la DLL métier (FranceJudo.Core)
            _coreAssembly = typeof(AppEnvironment).Assembly;
        }

        [Fact]
        public void GetVersionInformation_ConstruitLaVersionExacteAvecBeta()
        {
            // Arrange : On cible dynamiquement la DLL métier
            var coreAssembly = typeof(AppEnvironment).Assembly;
            string expectedVersion = coreAssembly.GetName().Version?.ToString() ?? "";

            // On lit la métadonnée standard
            var metadataAttributes = coreAssembly.GetCustomAttributes<AssemblyMetadataAttribute>();
            var betaAttr = metadataAttributes.FirstOrDefault(a => a.Key == "VersionBeta");

            if (betaAttr != null && int.TryParse(betaAttr.Value, out int betaValue) && betaValue > 0)
            {
                expectedVersion += $"-beta{betaValue:00}";
            }

            // Act
            string actualVersion = AppEnvironment.GetVersionInformation();

            // Assert
            actualVersion.Should().Be(expectedVersion);
        }

        [Fact]
        public void GetCompanyInformation_ExtraitLaValeurDeLAssemblyCore()
        {
            // Arrange
            string expected = _coreAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;

            // Act & Assert
            AppEnvironment.GetCompanyInformation().Should().Be(expected);
        }

        [Fact]
        public void GetCopyrightInformation_ExtraitLaValeurDeLAssemblyCore()
        {
            string expected = _coreAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;
            AppEnvironment.GetCopyrightInformation().Should().Be(expected);
        }

        [Fact]
        public void GetTrademarkInformation_ExtraitLaValeurDeLAssemblyCore()
        {
            string expected = _coreAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark ?? string.Empty;
            AppEnvironment.GetTrademarkInformation().Should().Be(expected);
        }

        [Fact]
        public void GetDataDirectory_ForceLeStyleUnix()
        {
            string dataDir = AppEnvironment.GetDataDirectory();
            dataDir.Should().NotBeNullOrEmpty();
            dataDir.Should().NotContain(@"\");
        }
    }
}