using FranceJudo.Metier.Noyau.Structures;
using KernelImpl.Noyau.Structures;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Structures
{
    public class DataStructuresTests
    {
        [Fact]
        public void DataStructures_InitializesCollections_NotNull()
        {
            // Arrange
            DataStructures data = new DataStructures();

            // Act & Assert
            Assert.NotNull(data.Clubs);
            Assert.NotNull(data.Comites);
            Assert.NotNull(data.Ligues);
            Assert.NotNull(data.Secteurs);
            Assert.NotNull(data.LesPays);
        }

        [Fact]
        public void DataStructures_ExplicitInterface_ShouldReturnSameCollections()
        {
            // Arrange
            DataStructures data = new DataStructures();
            IStructuresData interfaceData = data;

            // Act & Assert
            Assert.Same(data.Clubs, interfaceData.Clubs);
            Assert.Same(data.Comites, interfaceData.Comites);
            Assert.Same(data.Ligues, interfaceData.Ligues);
            Assert.Same(data.Secteurs, interfaceData.Secteurs);
            Assert.Same(data.LesPays, interfaceData.LesPays);
        }

        [Fact]
        public void DataStructures_ChargerMethods_ShouldExecuteWithoutThrowing()
        {
            // Arrange
            DataStructures data = new DataStructures();
            XElement dummyXml = new XElement("Root"); // Produira des listes vides via les méthodes Lecture statiques

            // Act
            data.ChargerClubs(dummyXml);
            data.ChargerComites(dummyXml);
            data.ChargerSecteurs(dummyXml);
            data.ChargerLigues(dummyXml);
            data.ChargerPays(dummyXml);

            // Assert
            // Validation que les listes sont toujours assignées et utilisables après l'update du cache
            Assert.NotNull(data.Clubs);
            Assert.NotNull(data.Comites);
            Assert.NotNull(data.Secteurs);
            Assert.NotNull(data.Ligues);
            Assert.NotNull(data.LesPays);
        }

        [Fact]
        public void DataStructures_LectureMethods_ShouldReturnEmptyCollections_ForDummyXml()
        {
            // Arrange
            DataStructures data = new DataStructures();
            XElement dummyXml = new XElement("Root");

            // Act
            ICollection<Club> clubs = data.LectureClubs(dummyXml);
            ICollection<Comite> comites = data.LectureComites(dummyXml);
            ICollection<Secteur> secteurs = data.LectureSecteurs(dummyXml);
            ICollection<Ligue> ligues = data.LectureLigues(dummyXml);

            // Assert
            Assert.Empty(clubs);
            Assert.Empty(comites);
            Assert.Empty(secteurs);
            Assert.Empty(ligues);
        }
    }
}