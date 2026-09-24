#nullable enable
using AppPublication.Export;
using FranceJudo.Metier.Noyau;
using System;
using Xunit;

namespace AppPublication.Tests.Export
{
    public class ExportSharedContextInterneTests
    {
        [Fact]
        public void Create_AvecDataContextNull_LeveArgumentNullException()
        {
            // Arrange
            ConfigurationExportSiteInterne config = new ConfigurationExportSiteInterne();

            // IDE0039 : Fonction locale
            void ActionCreate()
            {
                // L'opérateur null-forgiving (!) indique qu'on force l'erreur pour le test
                _ = ExportSharedContextInterne.Create(null!, config);
            }

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(ActionCreate);
            Assert.Equal("DC", exception.ParamName);
        }

        [Fact]
        public void Create_AvecConfigurationNull_LeveArgumentNullException()
        {
            // Arrange
            Moq.Mock<IJudoData> mockJudoData = new Moq.Mock<IJudoData>();

            // IDE0039 : Fonction locale
            void ActionCreate()
            {
                _ = ExportSharedContextInterne.Create(mockJudoData.Object, null!);
            }

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(ActionCreate);
            Assert.Equal("config", exception.ParamName);
        }
    }
}