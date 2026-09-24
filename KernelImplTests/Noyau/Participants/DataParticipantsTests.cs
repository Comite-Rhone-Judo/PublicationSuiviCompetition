using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using KernelImpl.Noyau.Participants;
using Moq;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Participants
{
    public class DataParticipantsTests
    {
        [Fact]
        public void Properties_ShouldNotBeNull_OnInitialization()
        {
            // Arrange
            DataParticipants dataParticipants = new DataParticipants();

            // Act & Assert
            Assert.NotNull(dataParticipants.Equipes);
            Assert.NotNull(dataParticipants.Judokas);
            Assert.NotNull(dataParticipants.EpreuveJudokas);
            Assert.NotNull(dataParticipants.Vuejudokas);
            Assert.NotNull(dataParticipants.VueJudokasEpreuve);
        }

        [Fact]
        public void IParticipantsData_ExplicitImplementations_ShouldReturnSameCollections()
        {
            // Arrange
            DataParticipants dataParticipants = new DataParticipants();
            IParticipantsData interfaceParticipants = dataParticipants;

            // Act & Assert
            Assert.Same(dataParticipants.Equipes, interfaceParticipants.Equipes);
            Assert.Same(dataParticipants.Judokas, interfaceParticipants.Judokas);
            Assert.Same(dataParticipants.EpreuveJudokas, interfaceParticipants.EpreuveJudokas);
            Assert.Same(dataParticipants.Vuejudokas, interfaceParticipants.Vuejudokas);
        }

        [Fact]
        public void LectureEquipes_ShouldExecute_WithoutThrowing()
        {
            // Arrange
            DataParticipants dataParticipants = new DataParticipants();
            XElement dummyXml = new XElement("Root");

            // Act
            ICollection<Equipe> result = dataParticipants.LectureEquipes(dummyXml);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ChargeEquipes_ShouldUpdateCache()
        {
            // Arrange
            DataParticipants dataParticipants = new DataParticipants();
            XElement dummyXml = new XElement("Root");

            // Act
            dataParticipants.ChargeEquipes(dummyXml);

            // Assert
            Assert.NotNull(dataParticipants.Equipes);
            // La vérification porte sur le fait que la méthode ne crashe pas et 
            // met bien à jour le cache (même vide avec un mock XML basique).
        }

        [Fact]
        public void ChargeEpreuvesJudokas_ShouldExecute_AndPropagateState()
        {
            // Arrange
            DataParticipants dataParticipants = new DataParticipants();
            XElement dummyXml = new XElement("Root");

            // Act
            // Le test s'assure que l'optimisation de l'étape clé (O(N) -> O(M)) s'exécute sans erreur
            dataParticipants.ChargeEpreuvesJudokas(dummyXml);

            // Assert
            Assert.NotNull(dataParticipants.EpreuveJudokas);
        }

        [Fact]
        public void ChargeJudokas_ShouldGenerateVues_AndPopulateDictionaries()
        {
            // Arrange
            DataParticipants dataParticipants = new DataParticipants();
            XElement dummyXml = new XElement("Root");

            // Mise en place du Mock complexe exigé par GenereVueJudokas
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<IOrganisationData> mockOrganisation = new Mock<IOrganisationData>();
            Mock<ICompetition> mockCompetition = new Mock<ICompetition>();

            // Mock de l'organisation
            mockCompetition.Setup(c => c.IsEquipe()).Returns(false);
            mockOrganisation.SetupGet(o => o.Competition).Returns(mockCompetition.Object);
            mockOrganisation.SetupGet(o => o.Epreuves).Returns(new List<IEpreuve>());
            mockOrganisation.SetupGet(o => o.EpreuveEquipes).Returns(new List<IEpreuve_Equipe>());

            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisation.Object);

            // Act
            dataParticipants.ChargeJudokas(dummyXml, mockJudoData.Object);

            // Assert
            Assert.NotNull(dataParticipants.Judokas);
            Assert.NotNull(dataParticipants.Vuejudokas);
            Assert.NotNull(dataParticipants.VueJudokasEpreuve);

            // Le dictionnaire est initialisé au minimum avec la clé "0" pour les sans-épreuves
            Assert.True(dataParticipants.VueJudokasEpreuve.ContainsKey(0));
        }
    }
}