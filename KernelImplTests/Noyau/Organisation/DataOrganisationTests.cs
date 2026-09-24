#nullable enable
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Categories;
using KernelImpl.Noyau.Organisation;

namespace KernelImpl.Tests.Noyau.Organisation
{
    public class DataOrganisationTests
    {
        [Fact]
        public void ChargeEpreuves_MetAJourLeCacheDesEpreuves_EtGenereLesVues()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            Mock<ICategoriesData> mockCategories = new Mock<ICategoriesData>();
            mockDc.Setup(m => m.Categories).Returns(mockCategories.Object);
            mockCategories.Setup(c => c.CAges).Returns(new List<ICategorieAge>());
            mockCategories.Setup(c => c.CPoids).Returns(new List<ICategoriePoids>());

            DataOrganisation data = new DataOrganisation();

            Epreuve epreuve1 = new Epreuve { id = 10, nom = "Epreuve 10" };
            Epreuve epreuve2 = new Epreuve { id = 20, nom = "Epreuve 20" };
            XElement root = new XElement("Root", epreuve1.ToXml(dc), epreuve2.ToXml(dc));

            data.ChargeEpreuves(root, dc);

            // Vérification du cache natif
            data.Epreuves.Should().HaveCount(2);
            data.Epreuves.Any(e => e.id == 10).Should().BeTrue();

            // Vérification de la génération automatique des vues
            data.VueEpreuves.Should().HaveCount(2);
            data.VueEpreuves.Any(v => v.id == 20).Should().BeTrue();
        }
    }
}