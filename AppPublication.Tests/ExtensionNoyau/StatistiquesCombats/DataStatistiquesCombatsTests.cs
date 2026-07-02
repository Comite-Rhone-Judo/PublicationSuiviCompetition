using AppPublication.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.Noyau.Deroulement;
using JudoClient.Communication;
using KernelImpl.Noyau.Deroulement;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppPublication.Tests.ExtensionNoyau.StatistiquesCombats
{
    public class DataStatistiquesCombatsTests
    {
        private Mock<IJudoData> _mockJudoData;

        public DataStatistiquesCombatsTests()
        {
            _mockJudoData = new Mock<IJudoData>();
        }

        [Fact]
        public void Constructeur_DataVide_NePlantePas()
        {
            // Arrange

            // Act
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);

            // Assert
            Assert.NotNull(moteur.Statistiques);
            Assert.Empty(moteur.Statistiques);
        }

        [Fact]
        public void PasseParticipants_CascadeStructurelle_GenereToutesLesCles()
        {
            // Arrange
            PreparerMockCompetition(EchelonEnum.National);

            var mockJudoka = new Mock<IVueJudoka>();
            mockJudoka.Setup(j => j.id).Returns(1);
            mockJudoka.Setup(j => j.present).Returns(true);
            mockJudoka.Setup(j => j.sexeEnum).Returns(new EpreuveSexe("M"));
            mockJudoka.Setup(j => j.club).Returns("ClubA");
            mockJudoka.Setup(j => j.comite).Returns("ComiteB");
            mockJudoka.Setup(j => j.ligue).Returns("LigueC");
            mockJudoka.Setup(j => j.pays).Returns(250);

            PreparerMockParticipants(new[] { mockJudoka.Object });

            // Act
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);

            // Assert
            var stats = moteur.Statistiques;

            var sexeAttendu = new EpreuveSexe("M");

            Assert.True(stats.ContainsKey(new StatistiqueCle(TypeEntiteStatistique.Judoka, "1", sexeAttendu)));
            Assert.True(stats.ContainsKey(new StatistiqueCle(TypeEntiteStatistique.Structure, "ClubA", sexeAttendu)));
            Assert.True(stats.ContainsKey(new StatistiqueCle(TypeEntiteStatistique.Structure, "ComiteB", sexeAttendu)));
            Assert.True(stats.ContainsKey(new StatistiqueCle(TypeEntiteStatistique.Structure, "LigueC", sexeAttendu)));
            Assert.True(stats.ContainsKey(new StatistiqueCle(TypeEntiteStatistique.Structure, "250", sexeAttendu)));

            var statsClub = stats[new StatistiqueCle(TypeEntiteStatistique.Structure, "ClubA", sexeAttendu)];
            Assert.Equal(1, statsClub.NbParticipants);
            Assert.Equal(1, statsClub.NbCombattants);
        }

        [Fact]
        public void PasseCombats_VictoireIppon_InscritCorrectement()
        {
            // Arrange
            PreparerMockCompetition(EchelonEnum.Club);

            var sexeTest = new EpreuveSexe("F");

            var mockJudoka1 = new Mock<IVueJudoka>();
            mockJudoka1.Setup(j => j.id).Returns(1);
            mockJudoka1.Setup(j => j.sexeEnum).Returns(sexeTest);
            mockJudoka1.Setup(j => j.club).Returns("ClubGagnant");

            var mockJudoka2 = new Mock<IVueJudoka>();
            mockJudoka2.Setup(j => j.id).Returns(2);
            mockJudoka2.Setup(j => j.sexeEnum).Returns(sexeTest);
            mockJudoka2.Setup(j => j.club).Returns("ClubPerdant");

            PreparerMockParticipants(new[] { mockJudoka1.Object, mockJudoka2.Object });

            var combat = new Combat
            {
                participant1 = 1,
                participant2 = 2,
                vainqueur = 1,
                score1 = 100,
                score2 = 0,
                temps = 4,
                debut = DateTime.Today,
                fin = DateTime.Today.AddMinutes(2),
                virtuel = false
            };
            PreparerMockCombats(new[] { combat });

            // Act
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);
            var stats = moteur.Statistiques;

            // Assert
            var cleClubGagnant = new StatistiqueCle(TypeEntiteStatistique.Structure, "ClubGagnant", sexeTest);
            var statGagnant = stats[cleClubGagnant];

            Assert.Equal(1, statGagnant.NbCombats);
            Assert.Equal(1, statGagnant.NbVictoires);
            Assert.Equal(1, ((CompteurStatistiques)statGagnant).NbVictoireIpponDirect);
            Assert.Equal(TimeSpan.FromMinutes(2), statGagnant.DureeCombatMoy);

            var cleClubPerdant = new StatistiqueCle(TypeEntiteStatistique.Structure, "ClubPerdant", sexeTest);
            var statPerdant = stats[cleClubPerdant];

            Assert.Equal(1, statPerdant.NbCombats);
            Assert.Equal(0, statPerdant.NbVictoires);
            Assert.Equal(0, statPerdant.NbHikiwake);
        }

        private void PreparerMockCompetition(EchelonEnum niveau)
        {
            var mockComp = new Mock<ICompetition>();
            mockComp.Setup(c => c.id).Returns(99);
            mockComp.Setup(c => c.niveau).Returns((int)niveau);

            var mockOrg = new Mock<IOrganisationData>();
            mockOrg.Setup(o => o.Competitions).Returns(new List<ICompetition> { mockComp.Object });

            _mockJudoData.Setup(d => d.Organisation).Returns(mockOrg.Object);
        }

        private void PreparerMockParticipants(IEnumerable<IVueJudoka> judokas)
        {
            var mockParts = new Mock<IParticipantsData>();

            // On matérialise l'IEnumerable en List (qui implémente IReadOnlyList)
            mockParts.Setup(p => p.Vuejudokas).Returns(judokas.ToList());

            _mockJudoData.Setup(d => d.Participants).Returns(mockParts.Object);
        }

        private void PreparerMockCombats(IEnumerable<Combat> combats)
        {
            var mockDeroulement = new Mock<IDeroulementData>();

            // On caste explicitement les Combat concrets en ICombat, puis on matérialise en List
            mockDeroulement.Setup(d => d.Combats).Returns(combats.Cast<ICombat>().ToList());

            _mockJudoData.Setup(d => d.Deroulement).Returns(mockDeroulement.Object);
        }
    }
}