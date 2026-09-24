#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Categories;

namespace KernelImpl.Tests.Noyau.Categories
{
    public class CategorieAgeTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            // Arrange : Utilisation de l'initialiseur d'objet (IDE0017) et typage explicite
            CategorieAge original = new CategorieAge
            {
                id = 1,
                nom = "Seniors",
                anneeMin = 1900,
                anneeMax = 2005,
                ordre = "1",
                remoteId = "REMOTE_SENIORS"
            };

            // Act
            XElement xml = original.ToXml();

            CategorieAge copie = new CategorieAge();
            copie.LoadXml(xml);

            // Assert
            copie.id.Should().Be(original.id);
            copie.nom.Should().Be(original.nom);
            copie.anneeMin.Should().Be(original.anneeMin);
            copie.anneeMax.Should().Be(original.anneeMax);
            copie.ordre.Should().Be(original.ordre);
            copie.remoteId.Should().Be(original.remoteId);
        }

        [Fact]
        public void ToString_RetourneLeNomDeLaCategorie()
        {
            CategorieAge categorie = new CategorieAge { nom = "Minimes" };
            string result = categorie.ToString();
            result.Should().Be("Minimes");
        }

        [Fact]
        public void LectureCategorieAge_ParseUneListeDepuisUnXElement()
        {
            XElement c1 = new CategorieAge { id = 10, nom = "Benjamins" }.ToXml();
            XElement c2 = new CategorieAge { id = 11, nom = "Minimes" }.ToXml();
            XElement root = new XElement("Root", c1, c2);

            ICollection<CategorieAge> liste = CategorieAge.LectureCategorieAge(root);

            liste.Should().HaveCount(2);
            liste.First().id.Should().Be(10);
            liste.Last().id.Should().Be(11);
        }
    }
}