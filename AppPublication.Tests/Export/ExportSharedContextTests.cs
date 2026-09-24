#nullable enable
using AppPublication.Export;
using FranceJudo.Metier.ExtensionNoyau;
using FranceJudo.Metier.Noyau;
using Moq;
using System;
using Xunit;

namespace AppPublication.Tests.Export
{
    public class ExportSharedContextTests
    {
        [Fact]
        public void Create_AvecDependancesNulles_LeveArgumentNullException()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            ExtendedJudoData donneesEtendues = new ExtendedJudoData(mockJudoData.Object);
            ConfigurationExportSite config = new ConfigurationExportSite();

            // Fonctions locales pour isoler chaque cas de test
            void ActionDCNull()
            {
                _ = ExportSharedContext.Create(null!, donneesEtendues, config);
            }

            void ActionEDCNull()
            {
                _ = ExportSharedContext.Create(mockJudoData.Object, null!, config);
            }

            void ActionConfigNull()
            {
                _ = ExportSharedContext.Create(mockJudoData.Object, donneesEtendues, null!);
            }

            // Act & Assert
            ArgumentNullException exceptionDC = Assert.Throws<ArgumentNullException>(ActionDCNull);
            Assert.Equal("DC", exceptionDC.ParamName);

            ArgumentNullException exceptionEDC = Assert.Throws<ArgumentNullException>(ActionEDCNull);
            Assert.Equal("EDC", exceptionEDC.ParamName);

            ArgumentNullException exceptionConfig = Assert.Throws<ArgumentNullException>(ActionConfigNull);
            Assert.Equal("config", exceptionConfig.ParamName);
        }
    }
}