#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Categories;
using KernelImpl.Internal;
using KernelImpl.Noyau.Organisation;

namespace KernelImpl.Tests.Noyau.Organisation
{
    public class VueEpreuveTests
    {
        [Fact]
        public void Constructeur_ResoutLesCategories_DepuisJudoData()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            Mock<ICategoriesData> mockCategories = new Mock<ICategoriesData>();

            mockDc.Setup(dc => dc.Categories).Returns(mockCategories.Object);

            // Simulation Catégorie Age
            Mock<ICategorieAge> mockCateAge = new Mock<ICategorieAge>();
            mockCateAge.Setup(c => c.id).Returns(1);
            mockCateAge.Setup(c => c.nom).Returns("Seniors");
            mockCateAge.Setup(c => c.remoteId).Returns("REMOTE_SENIOR");

            List<ICategorieAge> listeAges = new List<ICategorieAge> { mockCateAge.Object };
            mockCategories.Setup(c => c.CAges).Returns(listeAges);

            // Simulation Catégorie Poids
            Mock<ICategoriePoids> mockCatePoids = new Mock<ICategoriePoids>();
            mockCatePoids.Setup(c => c.id).Returns(2);
            mockCatePoids.Setup(c => c.nom).Returns("-73 kg");
            mockCatePoids.Setup(c => c.remoteId).Returns("REMOTE_73");

            List<ICategoriePoids> listePoids = new List<ICategoriePoids> { mockCatePoids.Object };
            mockCategories.Setup(c => c.CPoids).Returns(listePoids);

            Epreuve epreuve = new Epreuve
            {
                id = 99,
                nom = "Test",
                categorieAge = 1,
                categoriePoids = 2
            };

            VueEpreuve vue = new VueEpreuve(epreuve, mockDc.Object);

            vue.id.Should().Be(99);
            vue.nom_cateage.Should().Be("Seniors");
            vue.remoteId_cateage.Should().Be("REMOTE_SENIOR");
            vue.nom_catepoids.Should().Be("-73 kg");
            vue.remoteId_catepoids.Should().Be("REMOTE_73");

            IEntityWithKey<int> entity = vue;
            entity.EntityKey.Should().Be(99);
        }
    }
}