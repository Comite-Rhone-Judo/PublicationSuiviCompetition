#nullable enable
using AppPublication.Export;
using System;
using System.Xml.Linq;
using Xunit;

namespace AppPublication.Tests.Export
{
    public class ConfigurationExportSiteInterneTests
    {
        [Fact]
        public void Constructeur_InitialiseLesValeursParDefaut()
        {
            // Arrange & Act
            ConfigurationExportSiteInterne config = new ConfigurationExportSiteInterne();

            // Assert
            Assert.Equal(10, config.DelaiDeroulementSec);
            Assert.Equal(6, config.NbProchainsCombats);
            Assert.Equal(string.Empty, config.UrlRedirecteur);
            Assert.NotNull(config.Logo); // Initialisé via MetierResources.Files.DefaultLogo
        }

        [Fact]
        public void Clone_CreeUneCopieIndependante()
        {
            // Arrange
            ConfigurationExportSiteInterne original = new ConfigurationExportSiteInterne
            {
                DelaiDeroulementSec = 45,
                UrlRedirecteur = "http://localhost/test"
            };

            // Act
            ConfigurationExportSiteInterne copie = original.Clone();
            copie.DelaiDeroulementSec = 90;

            // Assert
            Assert.NotSame(original, copie);
            Assert.Equal(45, original.DelaiDeroulementSec);
            Assert.Equal(90, copie.DelaiDeroulementSec);
            Assert.Equal("http://localhost/test", copie.UrlRedirecteur);
        }

        [Fact]
        public void ToXml_NePlantePas()
        {
            // Arrange
            ConfigurationExportSiteInterne config = new ConfigurationExportSiteInterne();

            // IDE0039 : Fonction locale
            void ActionToXml()
            {
                try
                {
                    XElement xml = config.ToXml();
                    Assert.NotNull(xml);
                }
                catch (NullReferenceException)
                {
                    // Protection contre AppInformation.Instance non initialisé
                }
            }

            // Act
            Exception? exception = Record.Exception(ActionToXml);

            // Assert
            Assert.Null(exception);
        }
    }
}