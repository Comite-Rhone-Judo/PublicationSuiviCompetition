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
    public class GestionSiteInterneTests
    {
        [Fact]
        public void IdCompetition_Setter_RecupereLeNombreDeTapisDuMetier()
        {
            // Arrange : Reconstruction de l'arbre d'interfaces (DataManager -> Data -> Organisation -> Competition)
            Mock<ICompetition> mockCompetition = new Mock<ICompetition>();
            mockCompetition.SetupGet(c => c.nbTapis).Returns(8); // On simule 8 tapis

            Mock<IOrganisationData> mockOrganisation = new Mock<IOrganisationData>();
            mockOrganisation.SetupGet(o => o.Competition).Returns(mockCompetition.Object);

            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisation.Object);

            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            mockDataManager.SetupGet(m => m.Data).Returns(mockJudoData.Object);

            GestionStatistiques statMgr = new GestionStatistiques();

            // Note : L'instanciation de GestionSiteInterne nécessite que AppConfigRoot soit initialisé dans votre projet de test.
            GestionSiteInterne gestionnaire = new GestionSiteInterne(mockDataManager.Object, statMgr)
            {
                // Act
                // Le setter IdCompetition appelle OnIdCompetitionChanged qui va lire _judoDataManager.Data.Organisation.Competition.nbTapis
                IdCompetition = "COMP-001"
            };

            // Assert
            Assert.Equal(8, gestionnaire.NbTapis); // Le gestionnaire a dû extraire la valeur "8" via l'arbre d'interfaces
        }

        [Fact]
        public void DelaiDeroulementSec_Setter_MetAJourLaValeur()
        {
            // Arrange
            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            GestionSiteInterne gestionnaire = new GestionSiteInterne(mockDataManager.Object, new GestionStatistiques())
            {
                // Act
                DelaiDeroulementSec = 45
            };

            // Assert
            Assert.Equal(45, gestionnaire.DelaiDeroulementSec);
        }
    }
}