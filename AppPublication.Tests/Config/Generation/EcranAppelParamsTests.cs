using AppPublication.Config.Generation;
using Xunit;

namespace AppPublication.Tests.Config.Generation
{
    public class EcranAppelParamsTests
    {
        [Fact]
        public void Proprietes_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            EcranAppelParams ecran = new EcranAppelParams();

            // Assert
            Assert.Equal(string.Empty, ecran.Hostname);
            Assert.Equal(1, ecran.Id);
            Assert.Equal("Nouvel Ecran", ecran.Description);
            Assert.Equal(string.Empty, ecran.AdresseIp);
            Assert.Equal(1, ecran.Groupement);
            Assert.Equal(string.Empty, ecran.TapisIds);
            Assert.False(ecran.AjusteTexteAuto);
            Assert.Equal(5, ecran.NbCombatsPage);
        }

        [Fact]
        public void Setters_ModifientLesValeurs()
        {
            // Arrange
            EcranAppelParams ecran = new EcranAppelParams
            {
                // Act
                Id = 42,
                Description = "Ecran Principal",
                NbCombatsPage = 10
            };

            // Assert
            Assert.Equal(42, ecran.Id);
            Assert.Equal("Ecran Principal", ecran.Description);
            Assert.Equal(10, ecran.NbCombatsPage);
        }
    }
}