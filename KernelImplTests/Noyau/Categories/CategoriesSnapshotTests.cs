#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Categories;

namespace KernelImpl.Tests.Noyau.Categories
{
    public class CategoriesSnapshotTests
    {
        [Fact]
        public void Constructeur_AvecSourceNulle_NePlantePas()
        {
            CategoriesSnapshot snapshot = new CategoriesSnapshot(null!);

            snapshot.CAges.Should().BeNull();
            snapshot.CPoids.Should().BeNull();
            snapshot.Grades.Should().BeNull();
        }

        [Fact]
        public void Constructeur_AvecSourceValide_CopieLesReferencesDesListes()
        {
            DataCategories source = new DataCategories();

            source.ChargeCategorieAges(new XElement("Root", new CategorieAge { id = 1 }.ToXml()));
            source.ChargeCategoriePoids(new XElement("Root", new CategoriePoids { id = 1 }.ToXml()));
            source.ChargeCeintures(new XElement("Root", new Ceintures { id = 1 }.ToXml()));

            CategoriesSnapshot snapshot = new CategoriesSnapshot(source);

            snapshot.CAges.Should().BeSameAs(source.CAges);
            snapshot.CPoids.Should().BeSameAs(source.CPoids);
            snapshot.Grades.Should().BeSameAs(source.Grades);
        }
    }
}