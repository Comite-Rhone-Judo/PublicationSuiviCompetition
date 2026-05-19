#nullable enable
using AppPublication.Models.Publication;
using AppPublication.Models.Statistiques;
using AppPublication.Statistiques;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using Moq;
using Xunit;

namespace AppPublication.Tests.Models.Publication
{
    public class GestionSitePubliqueTests
    {
        [Fact]
        public void OnIdCompetitionChanged_CompetitionIndividuelle_ActiveAffectationEtEngagements()
        {
            // Arrange : Simulation d'une compétition INDIVIDUELLE pure
            Mock<ICompetition> mockCompetition = new Mock<ICompetition>();
            mockCompetition.Setup(c => c.IsIndividuelle()).Returns(true);
            mockCompetition.Setup(c => c.IsShiai()).Returns(false);

            Mock<IOrganisationData> mockOrganisation = new Mock<IOrganisationData>();
            mockOrganisation.SetupGet(o => o.Competition).Returns(mockCompetition.Object);

            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisation.Object);

            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            mockDataManager.SetupGet(m => m.Data).Returns(mockJudoData.Object);

            GestionSitePublique gestionnaire = new GestionSitePublique(mockDataManager.Object, new GestionStatistiques())
            {
                // Act
                IdCompetition = "TEST-INDIV"
            };

            // Assert
            Assert.True(gestionnaire.CanPublierAffectation); // Individuelle = true
            Assert.True(gestionnaire.CanPublierEngagements); // Individuelle || Shiai = true
        }

        [Fact]
        public void OnIdCompetitionChanged_CompetitionShiai_ForceLaffichageEnColonnes()
        {
            // Arrange : Simulation d'une compétition SHIAI
            Mock<ICompetition> mockCompetition = new Mock<ICompetition>();
            mockCompetition.Setup(c => c.IsIndividuelle()).Returns(false);
            mockCompetition.Setup(c => c.IsShiai()).Returns(true); // Est un Shiai

            Mock<IOrganisationData> mockOrganisation = new Mock<IOrganisationData>();
            mockOrganisation.SetupGet(o => o.Competition).Returns(mockCompetition.Object);

            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisation.Object);

            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            mockDataManager.SetupGet(m => m.Data).Returns(mockJudoData.Object);

            GestionSitePublique gestionnaire = new GestionSitePublique(mockDataManager.Object, new GestionStatistiques())
            {
                // Act
                IdCompetition = "TEST-SHIAI"
            };

            // Assert
            Assert.False(gestionnaire.CanPublierAffectation); // Pas individuelle
            Assert.True(gestionnaire.CanPublierEngagements); // Shiai = true

            // Vérification de la logique spécifique au Shiai (forçage des colonnes)
            Assert.True(gestionnaire.PouleEnColonnes);
            Assert.True(gestionnaire.PouleToujoursEnColonnes);
        }

        [Fact]
        public void ModeConfiguration_Advanced_DesactiveEasyConfig()
        {
            // Arrange
            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            GestionSitePublique gestionnaire = new GestionSitePublique(mockDataManager.Object, new GestionStatistiques())
            {
                // Act
                AdvancedConfig = true
            };

            // Assert
            Assert.False(gestionnaire.EasyConfig); // L'un est l'inverse de l'autre
        }
    }
}