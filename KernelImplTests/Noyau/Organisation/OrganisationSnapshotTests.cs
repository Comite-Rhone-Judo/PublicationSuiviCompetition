#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Categories;
using KernelImpl.Noyau.Organisation;

namespace KernelImpl.Tests.Noyau.Organisation
{
    public class OrganisationSnapshotTests
    {
        [Fact]
        public void Constructeur_AvecSourceNulle_NePlantePas()
        {
            OrganisationSnapshot snapshot = new OrganisationSnapshot(null!);

            snapshot.Competitions.Should().BeNull();
            snapshot.Epreuves.Should().BeNull();
            snapshot.VueEpreuves.Should().BeNull();
        }

        [Fact]
        public void Constructeur_AvecSourceValide_CopieLesReferencesDesListes()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            Mock<ICategoriesData> mockCategories = new Mock<ICategoriesData>();
            mockDc.Setup(m => m.Categories).Returns(mockCategories.Object);
            mockCategories.Setup(c => c.CAges).Returns(new List<ICategorieAge>());
            mockCategories.Setup(c => c.CPoids).Returns(new List<ICategoriePoids>());

            DataOrganisation source = new DataOrganisation();
            source.ChargeEpreuves(new XElement("Root", new Epreuve { id = 1 }.ToXml(dc)), dc);

            OrganisationSnapshot snapshot = new OrganisationSnapshot(source);

            // Le snapshot doit pointer sur les mêmes instances mémoires IReadOnlyList
            snapshot.Epreuves.Should().BeSameAs(source.Epreuves);
            snapshot.VueEpreuves.Should().BeSameAs(source.VueEpreuves);
            snapshot.Competitions.Should().BeSameAs(source.Competitions); // Vide mais instancié
        }
    }
}