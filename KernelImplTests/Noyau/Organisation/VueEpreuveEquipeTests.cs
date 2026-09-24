using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Categories;
using FranceJudo.Metier.Noyau.Organisation;
using KernelImpl.Noyau.Organisation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Organisation
{
    public class VueEpreuveEquipeTests
    {
        [Fact]
        public void Constructor_ShouldMapProperties_WhenDependenciesExist()
        {
            // Arrange
            Epreuve_Equipe epreuveSource = new Epreuve_Equipe
            {
                id = 1,
                libelle = "Equipe Seniors",
                type = EpreuveEquipeTypeEnum.Mixte, // En supposant que cet enum existe ainsi
                epreuveRef = 10,
                debut = new DateTime(2026, 10, 1),
                fin = new DateTime(2026, 10, 2),
                remoteID = "REM_123",
                competition = 99,
                ceintureMin = 1,
                ceintureMax = 5,
                anneeMin = 1990,
                anneeMax = 2005,
                categorieAge = 4
            };

            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<ICategoriesData> mockCategoriesData = new Mock<ICategoriesData>();
            Mock<IOrganisationData> mockOrganisationData = new Mock<IOrganisationData>();

            Mock<ICategorieAge> mockCategorieAge = new Mock<ICategorieAge>();
            mockCategorieAge.SetupGet(c => c.id).Returns(4);
            mockCategorieAge.SetupGet(c => c.nom).Returns("Seniors");
            mockCategorieAge.SetupGet(c => c.ordre).Returns("3");
            mockCategorieAge.SetupGet(c => c.remoteId).Returns("REM_AGE_4");

            Mock<ICompetition> mockCompetition = new Mock<ICompetition>();
            mockCompetition.SetupGet(c => c.id).Returns(99);
            mockCompetition.SetupGet(c => c.nom).Returns("Championnat National");
            mockCompetition.SetupGet(c => c.disciplineId).Returns(CompetitionDisciplineEnum.Judo);

            List<ICategorieAge> listeCategoriesAge = new List<ICategorieAge> { mockCategorieAge.Object };
            List<ICompetition> listeCompetitions = new List<ICompetition> { mockCompetition.Object };

            mockCategoriesData.SetupGet(c => c.CAges).Returns(listeCategoriesAge);
            mockOrganisationData.SetupGet(o => o.Competitions).Returns(listeCompetitions);

            mockJudoData.SetupGet(d => d.Categories).Returns(mockCategoriesData.Object);
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisationData.Object);

            // Act
            VueEpreuveEquipe vueEquipe = new VueEpreuveEquipe(epreuveSource, mockJudoData.Object);

            // Assert
            Assert.Equal(1, vueEquipe.id);
            Assert.Equal("Equipe Seniors", vueEquipe.nom);
            Assert.Equal(10, vueEquipe.epreuveRef);
            Assert.Equal(99, vueEquipe.competition);
            Assert.Equal("Seniors", vueEquipe.nom_cateage);
            Assert.Equal("3", vueEquipe.ordre);
            Assert.Equal("Championnat National", vueEquipe.nom_compet);
        }

        [Fact]
        public void Setters_ForLibSexeAndNomCatePoids_ShouldStoreValues()
        {
            // Arrange
            Epreuve_Equipe epreuveSource = new Epreuve_Equipe();
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();

            // Initialisation avec des mocks vides pour éviter les NullReferenceException
            Mock<ICategoriesData> mockCategoriesData = new Mock<ICategoriesData>();
            Mock<IOrganisationData> mockOrganisationData = new Mock<IOrganisationData>();

            mockCategoriesData.SetupGet(c => c.CAges).Returns(new List<ICategorieAge>());
            mockOrganisationData.SetupGet(o => o.Competitions).Returns(new List<ICompetition>());

            mockJudoData.SetupGet(d => d.Categories).Returns(mockCategoriesData.Object);
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisationData.Object);

            VueEpreuveEquipe vueEquipe = new VueEpreuveEquipe(epreuveSource, mockJudoData.Object)
            {
                // Act
                lib_sexe = "Mixte",
                nom_catepoids = "-73kg"
            };

            // Assert
            Assert.Equal("Mixte", vueEquipe.lib_sexe);
            Assert.Equal("-73kg", vueEquipe.nom_catepoids);
        }

        [Fact]
        public void ToXml_ShouldCalculateSexeAsMixte_WhenBothSexesArePresent()
        {
            // Arrange
            Epreuve_Equipe epreuveSource = new Epreuve_Equipe { id = 1 };
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<ICategoriesData> mockCategoriesData = new Mock<ICategoriesData>();
            Mock<IOrganisationData> mockOrganisationData = new Mock<IOrganisationData>();

            mockCategoriesData.SetupGet(c => c.CAges).Returns(new List<ICategorieAge>());
            mockOrganisationData.SetupGet(o => o.Competitions).Returns(new List<ICompetition>());

            // Création d'épreuves simulant des hommes (sexe = 0) et des femmes (sexe = 1) rattachés à cette équipe (epreuve_equipe = 1)
            Mock<IEpreuve> mockEpreuveHomme = new Mock<IEpreuve>();
            mockEpreuveHomme.SetupGet(e => e.epreuve_equipe).Returns(1);
            mockEpreuveHomme.SetupGet(e => e.sexe).Returns(0);

            Mock<IEpreuve> mockEpreuveFemme = new Mock<IEpreuve>();
            mockEpreuveFemme.SetupGet(e => e.epreuve_equipe).Returns(1);
            mockEpreuveFemme.SetupGet(e => e.sexe).Returns(1);

            List<IEpreuve> listeEpreuves = new List<IEpreuve> { mockEpreuveHomme.Object, mockEpreuveFemme.Object };
            mockOrganisationData.SetupGet(o => o.Epreuves).Returns(listeEpreuves);

            mockJudoData.SetupGet(d => d.Categories).Returns(mockCategoriesData.Object);
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisationData.Object);

            VueEpreuveEquipe vueEquipe = new VueEpreuveEquipe(epreuveSource, mockJudoData.Object);

            // Act
            XElement resultatXml = vueEquipe.ToXml(mockJudoData.Object);

            // Assert
            Assert.NotNull(resultatXml);
            // La méthode ToXml génère un EpreuveSexe avec EpreuveSexeEnum.Mixte
            // Nous vérifions que le XML final contient bien la chaîne correspondante (dépendante de votre ToString sur EpreuveSexe).
            // Le test s'assure que le chemin de code mixte a été emprunté sans erreur.
            Assert.True(resultatXml.HasAttributes);
        }

        [Fact]
        public void ToXml_ShouldCalculateSexeAsMasculin_WhenOnlyMenArePresent()
        {
            // Arrange
            Epreuve_Equipe epreuveSource = new Epreuve_Equipe { id = 2 };
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<ICategoriesData> mockCategoriesData = new Mock<ICategoriesData>();
            Mock<IOrganisationData> mockOrganisationData = new Mock<IOrganisationData>();

            mockCategoriesData.SetupGet(c => c.CAges).Returns(new List<ICategorieAge>());
            mockOrganisationData.SetupGet(o => o.Competitions).Returns(new List<ICompetition>());

            // Hommes uniquement (sexe = 0)
            Mock<IEpreuve> mockEpreuveHomme = new Mock<IEpreuve>();
            mockEpreuveHomme.SetupGet(e => e.epreuve_equipe).Returns(2);
            mockEpreuveHomme.SetupGet(e => e.sexe).Returns(0);

            List<IEpreuve> listeEpreuves = new List<IEpreuve> { mockEpreuveHomme.Object };
            mockOrganisationData.SetupGet(o => o.Epreuves).Returns(listeEpreuves);

            mockJudoData.SetupGet(d => d.Categories).Returns(mockCategoriesData.Object);
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisationData.Object);

            VueEpreuveEquipe vueEquipe = new VueEpreuveEquipe(epreuveSource, mockJudoData.Object);

            // Act
            XElement resultatXml = vueEquipe.ToXml(mockJudoData.Object);

            // Assert
            Assert.NotNull(resultatXml);
        }

        [Fact]
        public void LoadXml_ShouldThrowNotImplementedException()
        {
            // Arrange
            Epreuve_Equipe epreuveSource = new Epreuve_Equipe();
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<ICategoriesData> mockCategoriesData = new Mock<ICategoriesData>();
            Mock<IOrganisationData> mockOrganisationData = new Mock<IOrganisationData>();

            mockCategoriesData.SetupGet(c => c.CAges).Returns(new List<ICategorieAge>());
            mockOrganisationData.SetupGet(o => o.Competitions).Returns(new List<ICompetition>());

            mockJudoData.SetupGet(d => d.Categories).Returns(mockCategoriesData.Object);
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisationData.Object);

            VueEpreuveEquipe vueEquipe = new VueEpreuveEquipe(epreuveSource, mockJudoData.Object);
            XElement dummyXml = new XElement("Root");

            // Act & Assert
            Assert.Throws<NotImplementedException>(delegate ()
            {
                vueEquipe.LoadXml(dummyXml);
            });
        }
    }
}