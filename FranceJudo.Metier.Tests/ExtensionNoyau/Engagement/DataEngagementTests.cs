#nullable enable
using FranceJudo.Metier.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FranceJudo.Metier.Tests.ExtensionNoyau.Engagement
{
    public class DataEngagementTests
    {
        [Fact]
        public void BuildTypesGroupes_EchelonNational_InclusTousLesNiveauxInferieurs()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<IOrganisationData> mockOrga = new Mock<IOrganisationData>();
            Mock<IParticipantsData> mockParts = new Mock<IParticipantsData>();

            Mock<ICompetition> mockCompet = new Mock<ICompetition>();
            mockCompet.SetupGet(c => c.id).Returns(1);
            mockCompet.SetupGet(c => c.niveau).Returns((int)EchelonEnum.National);

            // On s'assure qu'elle ne soit pas traitée par le moteur de groupes (évite les NullRef sur les épreuves vides)
            mockCompet.Setup(c => c.IsShiai()).Returns(false);
            mockCompet.Setup(c => c.IsIndividuelle()).Returns(false);

            List<ICompetition> listeCompetitions = new List<ICompetition> { mockCompet.Object };
            List<IEpreuve> listeEpreuves = new List<IEpreuve>();
            List<IVueJudoka> listeJudokas = new List<IVueJudoka>();

            mockOrga.SetupGet(o => o.Competitions).Returns(listeCompetitions);
            mockOrga.SetupGet(o => o.Epreuves).Returns(listeEpreuves);
            mockParts.SetupGet(p => p.Vuejudokas).Returns(listeJudokas);

            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrga.Object);
            mockJudoData.SetupGet(d => d.Participants).Returns(mockParts.Object);

            // Act
            DataEngagement dataEngagement = new DataEngagement(mockJudoData.Object);
            IReadOnlyDictionary<int, List<EchelonEnum>> typesGroupes = dataEngagement.TypesGroupes;

            // Assert
            Assert.NotNull(typesGroupes);
            Assert.True(typesGroupes.ContainsKey(1));

            List<EchelonEnum> listeEchelons = typesGroupes[1];
            Assert.Equal(5, listeEchelons.Count); // Aucun, Club, Departement, Ligue, National
            Assert.Contains(EchelonEnum.National, listeEchelons);
            Assert.Contains(EchelonEnum.Ligue, listeEchelons);
            Assert.Contains(EchelonEnum.Departement, listeEchelons);
            Assert.Contains(EchelonEnum.Club, listeEchelons);
            Assert.Contains(EchelonEnum.Aucun, listeEchelons);
        }

        [Fact]
        public void BuildTypesGroupes_NiveauInconnu_UtiliseClubParDefaut()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<IOrganisationData> mockOrga = new Mock<IOrganisationData>();
            Mock<IParticipantsData> mockParts = new Mock<IParticipantsData>();

            Mock<ICompetition> mockCompet = new Mock<ICompetition>();
            mockCompet.SetupGet(c => c.id).Returns(2);
            mockCompet.SetupGet(c => c.niveau).Returns(999); // Niveau inexistant

            mockCompet.Setup(c => c.IsShiai()).Returns(false);
            mockCompet.Setup(c => c.IsIndividuelle()).Returns(false);

            List<ICompetition> listeCompetitions = new List<ICompetition> { mockCompet.Object };

            mockOrga.SetupGet(o => o.Competitions).Returns(listeCompetitions);
            mockOrga.SetupGet(o => o.Epreuves).Returns(new List<IEpreuve>());
            mockParts.SetupGet(p => p.Vuejudokas).Returns(new List<IVueJudoka>());
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrga.Object);
            mockJudoData.SetupGet(d => d.Participants).Returns(mockParts.Object);

            // Act
            DataEngagement dataEngagement = new DataEngagement(mockJudoData.Object);
            IReadOnlyDictionary<int, List<EchelonEnum>> typesGroupes = dataEngagement.TypesGroupes;

            // Assert
            Assert.True(typesGroupes.ContainsKey(2));
            List<EchelonEnum> listeEchelons = typesGroupes[2];

            // Le switch doit être passé dans 'default' et ajouter Aucun et Club.
            Assert.Equal(2, listeEchelons.Count);
            Assert.Contains(EchelonEnum.Aucun, listeEchelons);
            Assert.Contains(EchelonEnum.Club, listeEchelons);
        }
    }
}