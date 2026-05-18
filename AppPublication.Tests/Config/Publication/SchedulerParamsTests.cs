using AppPublication.Config.Publication;
using Xunit;

namespace AppPublication.Tests.Config.Publication
{
    public class SchedulerParamsTests
    {
        [Fact]
        public void Proprietes_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            SchedulerParams parametres = new SchedulerParams();

            // Assert
            Assert.Equal(string.Empty, parametres.ID);
            Assert.Equal(30, parametres.DelaiGenerationSec);
        }

        [Fact]
        public void Setters_ModifientLesValeurs()
        {
            // Arrange
            SchedulerParams parametres = new SchedulerParams
            {
                // Act
                ID = "Tache1",
                DelaiGenerationSec = 60
            };

            // Assert
            Assert.Equal("Tache1", parametres.ID);
            Assert.Equal(60, parametres.DelaiGenerationSec);
        }
    }
}