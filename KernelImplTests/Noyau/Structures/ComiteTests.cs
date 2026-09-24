using KernelImpl.Internal;
using KernelImpl.Noyau.Structures;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Structures
{
    public class ComiteTests
    {
        [Fact]
        public void Comite_Id_ShouldFormatAsTwoDigits_WhenNumeric()
        {
            // Arrange
            Comite comite = new Comite
            {
                nom = "Comite Test",
                ligue = "L1",
                // Act
                id = "7"
            };

            // Assert
            Assert.Equal("07", comite.id);
        }

        [Fact]
        public void Comite_Id_ShouldRemainUnchanged_WhenNotNumeric()
        {
            // Arrange
            Comite comite = new Comite
            {
                nom = "Comite Test",
                ligue = "L1",
                // Act
                id = "NORD"
            };

            // Assert
            Assert.Equal("NORD", comite.id);
        }

        [Fact]
        public void Comite_EntityKey_ShouldUpdateWhenIdOrLigueChanges()
        {
            // Arrange
            Comite comite = new Comite
            {
                id = "9", // Formatera en "09"
                ligue = "LIG1"
            };

            IEntityWithKey<string> interfaceEntity = comite;

            // Act & Assert
            // La construction de la clé est "{id}-{ligue}"
            Assert.Equal("09-LIG1", interfaceEntity.EntityKey);

            // Act : changement de ligue
            comite.ligue = "LIG2";

            // Assert
            Assert.Equal("09-LIG2", interfaceEntity.EntityKey);
        }

        [Fact]
        public void Comite_ToXml_ShouldExecuteWithoutThrowing()
        {
            // Arrange
            Comite comite = new Comite
            {
                id = "75",
                nom = "Paris",
                nomCourt = "75",
                ligue = "IDF",
                code = "CODE_75",
                secteur = "Centre"
            };

            // Act
            XElement xml = comite.ToXml(null);

            // Assert
            Assert.NotNull(xml);
        }
    }
}