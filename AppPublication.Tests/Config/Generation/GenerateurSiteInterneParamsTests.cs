using AppPublication.Config.Generation;
using Xunit;

namespace AppPublication.Tests.Config.Generation
{
    public class GenerateurSiteInterneParamsTests
    {
        [Fact]
        public void Proprietes_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            GenerateurSiteInterneParams parametres = new GenerateurSiteInterneParams();

            // Assert
            Assert.Equal(30, parametres.DelaiDeroulementSec);
            Assert.Equal(6, parametres.NbProchainsCombats);
        }

        [Fact]
        public void Setters_ModifientLesValeurs()
        {
            // Arrange
            GenerateurSiteInterneParams parametres = new GenerateurSiteInterneParams
            {
                // Act
                DelaiDeroulementSec = 15,
                NbProchainsCombats = 10
            };

            // Assert
            Assert.Equal(15, parametres.DelaiDeroulementSec);
            Assert.Equal(10, parametres.NbProchainsCombats);
        }
    }
}