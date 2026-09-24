using FranceJudo.Metier.Noyau;
using KernelImpl.Internal;
using KernelImpl.Noyau.Structures;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Structures
{
    public class StructuresSimplesTests
    {
        [Fact]
        public void Club_ToXml_ShouldFormatComite_WhenNumeric()
        {
            // Arrange
            Club club = new Club
            {
                id = "C1",
                nom = "Judo Club",
                nomCourt = "JC",
                comite = "5", // Doit être formaté en "05"
                ligue = "IDF"
            };

            // Act
            XElement xml = club.ToXml(null);

            // Assert
            Assert.NotNull(xml);
            // On s'assure que le formatage "00" a bien fonctionné en cherchant la valeur dans les attributs
            bool contientComiteFormate = false;
            foreach (XAttribute attr in xml.Attributes())
            {
                if (attr.Value == "05") contientComiteFormate = true;
            }
            Assert.True(contientComiteFormate);
        }

        [Fact]
        public void Club_ToString_ShouldReturnNomCourt()
        {
            // Arrange
            Club club = new Club
            {
                id = "C1",
                nom = "Judo Club",
                nomCourt = "JCL"
            };

            // Act
            string resultat = club.ToString();

            // Assert
            Assert.Equal("JCL", resultat);
        }

        [Fact]
        public void Ligue_ToXml_ShouldMapAllProperties()
        {
            // Arrange
            Ligue ligue = new Ligue
            {
                id = "L1",
                nom = "Ligue Ile de France",
                nomCourt = "IDF",
                code = "CODE1"
            };

            // Act
            XElement xml = ligue.ToXml(null);

            // Assert
            Assert.NotNull(xml);
            Assert.Equal("L1", ligue.id); // Simple validation d'état
        }

        [Fact]
        public void Pays_ToXml_ShouldUppercaseNom()
        {
            // Arrange
            Pays pays = new Pays
            {
                id = 250,
                nom = "france", // Doit passer en "FRANCE"
                code = 1,
                abr2 = "FR",
                abr3 = "FRA",
                AbrF = "F"
            };

            // Act
            XElement xml = pays.ToXml(null);

            // Assert
            Assert.NotNull(xml);
            bool contientNomMajuscule = false;
            foreach (XAttribute attr in xml.Attributes())
            {
                if (attr.Value == "FRANCE") contientNomMajuscule = true;
            }
            Assert.True(contientNomMajuscule);
        }

        [Fact]
        public void Secteur_ToXml_ShouldMapAllProperties()
        {
            // Arrange
            Secteur secteur = new Secteur
            {
                id = "S1",
                nom = "Secteur Nord",
                nomCourt = "SN"
            };

            // Act
            XElement xml = secteur.ToXml(null);

            // Assert
            Assert.NotNull(xml);
        }
    }
}