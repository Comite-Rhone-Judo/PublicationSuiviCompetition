#nullable enable
using AppPublication.Export;
using FranceJudo.Metier.XML;
using Moq;
using System.Xml.Linq;
using Xunit;

namespace AppPublication.Tests.Export
{
    public class ExportXMLTests
    {
        [Fact]
        public void GetComites_EnCasDExceptionInterne_RetourneBaliseVide()
        {
            // Arrange
            Mock<IReadOnlyExportContext> mockContext = new Mock<IReadOnlyExportContext>();
            // En ne configurant pas le DataContext, l'appel à DC.Structures.Comites
            // va générer une NullReferenceException à l'intérieur de ExportXML.GetComites.

            // Act
            XElement resultat = ExportXML.GetComites(mockContext.Object);

            // Assert
            Assert.NotNull(resultat);
            // Utilisation stricte de la constante métier au lieu du littéral "Comites"
            Assert.Equal(ConstantXML.Comites, resultat.Name.LocalName);
            Assert.Empty(resultat.Elements()); // Vérifie que la balise est bien vide et n'a pas fait crasher l'app
        }

        [Fact]
        public void GetLigues_EnCasDExceptionInterne_RetourneBaliseVide()
        {
            // Arrange
            Mock<IReadOnlyExportContext> mockContext = new Mock<IReadOnlyExportContext>();

            // Act
            XElement resultat = ExportXML.GetLigues(mockContext.Object);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(ConstantXML.Ligues, resultat.Name.LocalName);
            Assert.Empty(resultat.Elements());
        }

        [Fact]
        public void GetSecteurs_EnCasDExceptionInterne_RetourneBaliseVide()
        {
            // Arrange
            Mock<IReadOnlyExportContext> mockContext = new Mock<IReadOnlyExportContext>();

            // Act
            XElement resultat = ExportXML.GetSecteurs(mockContext.Object);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(ConstantXML.Secteurs, resultat.Name.LocalName);
            Assert.Empty(resultat.Elements());
        }

        [Fact]
        public void GetPays_EnCasDExceptionInterne_RetourneBaliseVide()
        {
            // Arrange
            Mock<IReadOnlyExportContext> mockContext = new Mock<IReadOnlyExportContext>();

            // Act
            XElement resultat = ExportXML.GetPays(mockContext.Object);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(ConstantXML.LesPays, resultat.Name.LocalName);
            Assert.Empty(resultat.Elements());
        }

        [Fact]
        public void GetCeintures_EnCasDExceptionInterne_RetourneBaliseVide()
        {
            // Arrange
            Mock<IReadOnlyExportContext> mockContext = new Mock<IReadOnlyExportContext>();

            // Act
            XElement resultat = ExportXML.GetCeintures(mockContext.Object);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(ConstantXML.Ceintures, resultat.Name.LocalName);
            Assert.Empty(resultat.Elements());
        }

        [Fact]
        public void GetClubs_EnCasDExceptionInterne_RetourneBaliseVide()
        {
            // Arrange
            Mock<IReadOnlyExportContext> mockContext = new Mock<IReadOnlyExportContext>();

            // Act
            XElement resultat = ExportXML.GetClubs(mockContext.Object);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(ConstantXML.Clubs, resultat.Name.LocalName);
            Assert.Empty(resultat.Elements());
        }
    }
}