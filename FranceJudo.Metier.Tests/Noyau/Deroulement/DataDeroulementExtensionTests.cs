#nullable enable
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Participants;

// CORRECTION : Utilisation stricte du namespace de test demandé
namespace FranceJudo.Metier.Tests.Noyau.Deroulement
{
    public class DataDeroulementExtensionTests
    {
        [Fact]
        public void ListeParticipant1_RetourneLesParticipants_DesPhasesAvecSuivant()
        {
            // Arrange
            int idEpreuve = 99;

            var mockPhase1 = new Mock<IPhase>();
            mockPhase1.Setup(p => p.id).Returns(1);
            mockPhase1.Setup(p => p.epreuve).Returns(idEpreuve);
            mockPhase1.Setup(p => p.suivant).Returns(2); // A un suivant -> Doit être inclus

            var mockPhase2 = new Mock<IPhase>();
            mockPhase2.Setup(p => p.id).Returns(2);
            mockPhase2.Setup(p => p.epreuve).Returns(idEpreuve);
            mockPhase2.Setup(p => p.suivant).Returns(0); // N'a pas de suivant -> Ne doit pas être inclus

            var mockPart1 = new Mock<IParticipant>();
            mockPart1.Setup(p => p.phase).Returns(1); // Lié à la phase 1
            mockPart1.Setup(p => p.id).Returns(101);

            var mockPart2 = new Mock<IParticipant>();
            mockPart2.Setup(p => p.phase).Returns(2); // Lié à la phase 2
            mockPart2.Setup(p => p.id).Returns(102);

            var mockDeroulement = new Mock<IDeroulementData>();
            mockDeroulement.Setup(d => d.Phases).Returns(new List<IPhase> { mockPhase1.Object, mockPhase2.Object });
            mockDeroulement.Setup(d => d.Participants).Returns(new List<IParticipant> { mockPart1.Object, mockPart2.Object });

            // Act
            var result = mockDeroulement.Object.ListeParticipant1(idEpreuve).ToList();

            // Assert
            result.Should().HaveCount(1);
            result.First().id.Should().Be(101, "Seul le participant de la phase 1 (qui a un suivant) doit être retourné.");
        }

        [Fact]
        public void ListeParticipant2_RetourneLesParticipants_DesPhasesSansSuivant()
        {
            // Arrange
            int idEpreuve = 88;

            var mockPhase1 = new Mock<IPhase>();
            mockPhase1.Setup(p => p.id).Returns(1);
            mockPhase1.Setup(p => p.epreuve).Returns(idEpreuve);
            mockPhase1.Setup(p => p.suivant).Returns(5); // A un suivant -> Ne doit pas être inclus

            var mockPhase2 = new Mock<IPhase>();
            mockPhase2.Setup(p => p.id).Returns(2);
            mockPhase2.Setup(p => p.epreuve).Returns(idEpreuve);
            mockPhase2.Setup(p => p.suivant).Returns(0); // N'a pas de suivant -> Doit être inclus

            var mockPart1 = new Mock<IParticipant>();
            mockPart1.Setup(p => p.phase).Returns(1);
            mockPart1.Setup(p => p.id).Returns(101);

            var mockPart2 = new Mock<IParticipant>();
            mockPart2.Setup(p => p.phase).Returns(2);
            mockPart2.Setup(p => p.id).Returns(102);

            var mockDeroulement = new Mock<IDeroulementData>();
            mockDeroulement.Setup(d => d.Phases).Returns(new List<IPhase> { mockPhase1.Object, mockPhase2.Object });
            mockDeroulement.Setup(d => d.Participants).Returns(new List<IParticipant> { mockPart1.Object, mockPart2.Object });

            // Act
            var result = mockDeroulement.Object.ListeParticipant2(idEpreuve).ToList();

            // Assert
            result.Should().HaveCount(1);
            result.First().id.Should().Be(102, "Seul le participant de la phase 2 (sans suivant) doit être retourné.");
        }

        [Fact]
        public void GetNbCombatJudoka_CompteSeulementLesCombatsAvecVainqueur_OùLeJudokaEstImplique()
        {
            // Arrange
            string targetLicence = "12345ABC";
            int targetJudokaId = 55;

            var mockJudoka = new Mock<IJudoka>();
            mockJudoka.Setup(j => j.licence).Returns(targetLicence);
            mockJudoka.Setup(j => j.id).Returns(targetJudokaId);

            var mockParticipantsData = new Mock<IParticipantsData>();
            mockParticipantsData.Setup(p => p.Judokas).Returns(new List<IJudoka> { mockJudoka.Object });

            var mockJudoData = new Mock<IJudoData>();
            mockJudoData.Setup(j => j.Participants).Returns(mockParticipantsData.Object);

            var combats = new List<ICombat>
            {
                // Combat 1: Le judoka y participe et il y a un vainqueur (Doit compter)
                CreateMockCombat(participant1: targetJudokaId, participant2: 99, vainqueur: 1).Object,
                
                // Combat 2: Le judoka y participe mais pas de vainqueur déclaré (Ne doit pas compter)
                CreateMockCombat(participant1: 88, participant2: targetJudokaId, vainqueur: null).Object,
                
                // Combat 3: Le judoka y participe, vainqueur = 0 (Ne doit pas compter)
                CreateMockCombat(participant1: targetJudokaId, participant2: 77, vainqueur: 0).Object,

                // Combat 4: Le judoka n'y participe pas du tout (Ne doit pas compter)
                CreateMockCombat(participant1: 11, participant2: 22, vainqueur: 1).Object,
            };

            var mockDeroulement = new Mock<IDeroulementData>();
            mockDeroulement.Setup(d => d.Combats).Returns(combats);

            // Act
            int result = mockDeroulement.Object.GetNbCombatJudoka(targetLicence, mockJudoData.Object);

            // Assert
            result.Should().Be(1, "Un seul combat correspond aux critères : implication du judoka + vainqueur > 0.");
        }

        [Fact]
        public void GetNbPointJudoka_FaitLaSommeDesPointsCumulGrch_PourToutesLesParticipationsDuJudoka()
        {
            // Arrange
            string targetLicence = "99999XYZ";
            int targetJudokaId = 77;

            var mockJudoka = new Mock<IJudoka>();
            mockJudoka.Setup(j => j.licence).Returns(targetLicence);
            mockJudoka.Setup(j => j.id).Returns(targetJudokaId);

            var mockParticipantsData = new Mock<IParticipantsData>();
            mockParticipantsData.Setup(p => p.Judokas).Returns(new List<IJudoka> { mockJudoka.Object });

            var mockJudoData = new Mock<IJudoData>();
            mockJudoData.Setup(j => j.Participants).Returns(mockParticipantsData.Object);

            var participants = new List<IParticipant>
            {
                // Participation 1 du judoka cible (10 points)
                CreateMockParticipant(judokaId: targetJudokaId, pointsGrch: 10).Object,
                
                // Participation 2 du judoka cible dans une autre phase/poules (5 points)
                CreateMockParticipant(judokaId: targetJudokaId, pointsGrch: 5).Object,
                
                // Participation d'un AUTRE judoka (Ne doit pas être compté)
                CreateMockParticipant(judokaId: 88, pointsGrch: 100).Object
            };

            var mockDeroulement = new Mock<IDeroulementData>();
            mockDeroulement.Setup(d => d.Participants).Returns(participants);

            // Act
            int result = mockDeroulement.Object.GetNbPointJudoka(targetLicence, mockJudoData.Object);

            // Assert
            result.Should().Be(15, "La somme des points GRCH des participations du judoka cible (10 + 5) doit être retournée.");
        }

        // --- Méthodes utilitaires privées ---

        private Mock<ICombat> CreateMockCombat(int participant1, int participant2, int? vainqueur)
        {
            var mock = new Mock<ICombat>();
            mock.Setup(c => c.participant1).Returns(participant1);
            mock.Setup(c => c.participant2).Returns(participant2);
            mock.Setup(c => c.vainqueur).Returns(vainqueur);
            return mock;
        }

        private Mock<IParticipant> CreateMockParticipant(int judokaId, int pointsGrch)
        {
            var mock = new Mock<IParticipant>();
            mock.Setup(p => p.judoka).Returns(judokaId);
            mock.Setup(p => p.cumulPointsGRCH).Returns(pointsGrch);
            return mock;
        }
    }
}