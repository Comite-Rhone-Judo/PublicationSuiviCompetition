#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Categories;
using FranceJudo.Metier.Noyau.Categories;

namespace KernelImpl.Tests.Noyau.Categories
{
    public class DataCategoriesTests
    {
        [Fact]
        public void ChargeCategorieAges_MetAJourLeCache()
        {
            DataCategories data = new DataCategories();
            XElement ageXml = new CategorieAge { id = 1, nom = "TestAge" }.ToXml();
            XElement root = new XElement("Root", ageXml);

            data.ChargeCategorieAges(root);

            data.CAges.Should().HaveCount(1);
            data.CAges[0].id.Should().Be(1);
        }

        [Fact]
        public void ChargeCategoriePoids_MetAJourLeCache()
        {
            DataCategories data = new DataCategories();
            XElement poidsXml = new CategoriePoids { id = 2, nom = "TestPoids" }.ToXml();
            XElement root = new XElement("Root", poidsXml);

            data.ChargeCategoriePoids(root);

            data.CPoids.Should().HaveCount(1);
            data.CPoids[0].id.Should().Be(2);
        }

        [Fact]
        public void ChargeCeintures_MetAJourLeCache()
        {
            DataCategories data = new DataCategories();
            XElement ceintureXml = new Ceintures { id = 3, nom = "TestCeinture" }.ToXml();
            XElement root = new XElement("Root", ceintureXml);

            data.ChargeCeintures(root);

            data.Grades.Should().HaveCount(1);
            data.Grades[0].id.Should().Be(3);
        }

        [Fact]
        public void InterfaceExplicite_PointeVersLesCachesConcrets()
        {
            DataCategories data = new DataCategories();
            ICategoriesData iData = data;

            iData.CAges.Should().BeSameAs(data.CAges);
            iData.CPoids.Should().BeSameAs(data.CPoids);
            iData.Grades.Should().BeSameAs(data.Grades);
        }
    }
}