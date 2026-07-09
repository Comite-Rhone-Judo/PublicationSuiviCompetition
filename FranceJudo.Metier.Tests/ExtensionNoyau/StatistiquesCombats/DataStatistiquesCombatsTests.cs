using FranceJudo.Metier.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.Noyau.Deroulement;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FranceJudo.Metier.Tests.ExtensionNoyau.StatistiquesCombats
{
    public class DataStatistiquesCombatsTests
    {
        private readonly Mock<IJudoData> _mockJudoData;
        private readonly Mock<IOrganisationData> _mockOrg;
        private readonly Mock<IParticipantsData> _mockParts;
        private readonly Mock<IDeroulementData> _mockDeroulement;

        public DataStatistiquesCombatsTests()
        {
            _mockJudoData = new Mock<IJudoData>();
            _mockOrg = new Mock<IOrganisationData>();
            _mockParts = new Mock<IParticipantsData>();
            _mockDeroulement = new Mock<IDeroulementData>();

            // INITIALISATION DE SÉCURITÉ (Évite les NullReferenceException)
            // Par défaut, un IJudoData vide renvoie des listes vides, et non des objets nuls.
            _mockOrg.Setup(o => o.Competitions).Returns(new List<ICompetition>());
            _mockOrg.Setup(o => o.Epreuves).Returns(new List<IEpreuve>());
            _mockParts.Setup(p => p.Vuejudokas).Returns(new List<IVueJudoka>());
            _mockDeroulement.Setup(d => d.Combats).Returns(new List<ICombat>());

            _mockJudoData.Setup(d => d.Organisation).Returns(_mockOrg.Object);
            _mockJudoData.Setup(d => d.Participants).Returns(_mockParts.Object);
            _mockJudoData.Setup(d => d.Deroulement).Returns(_mockDeroulement.Object);
        }

        [Fact]
        public void Constructeur_DataVide_NePlantePas()
        {
            // Act : Le moteur lit dataContext.Organisation.Competitions qui est maintenant une liste vide
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);

            // Assert
            Assert.NotNull(moteur);
            Assert.Empty(moteur.StatsJudokas);
        }

        [Fact]
        public void TraitementCombats_PointsTechniques_ComptabiliseCorrectement()
        {
            // Arrange
            PreparerEnvironnement(EchelonEnum.Club);
            PreparerMockParticipants(new[] { CreerMockJudoka(1, "M"), CreerMockJudoka(2, "M") });

            var combats = new List<ICombat>
            {
                // Cbt 1: Ippon Direct (Score 100) -> J1 gagne
                CreerCombat(1, 2, vainqueur: 1, score1: 100),
                // Cbt 2: Waza-ari Awasete Ippon (Score 20) -> J1 gagne
                CreerCombat(1, 2, vainqueur: 1, score1: 20),
                // Cbt 3: Waza-ari (Score 10) -> J2 gagne
                CreerCombat(1, 2, vainqueur: 2, score2: 10),
                // Cbt 4: Yuko (Score 1) -> J2 gagne
                CreerCombat(1, 2, vainqueur: 2, score2: 1)
            };
            PreparerMockCombats(combats);

            // Act
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);
            var statJ1 = (CompteurStatistiques)moteur.StatsJudokas[1];
            var statJ2 = (CompteurStatistiques)moteur.StatsJudokas[2];

            // Assert J1
            Assert.Equal(4, statJ1.NbCombats);
            Assert.Equal(2, statJ1.NbVictoires);
            Assert.Equal(1, statJ1.NbVictoireIpponDirect);
            Assert.Equal(1, statJ1.NbVictoireWazaAriAwaseteIppon);

            // Assert J2
            Assert.Equal(4, statJ2.NbCombats);
            Assert.Equal(2, statJ2.NbVictoires);
            Assert.Equal(1, statJ2.NbVictoireWazaAri);
            Assert.Equal(1, statJ2.NbVictoireYuko);
        }

        [Fact]
        public void TraitementCombats_EtatsVictoire_ComptabiliseCorrectement()
        {
            // Arrange
            PreparerEnvironnement(EchelonEnum.Club);
            PreparerMockParticipants(new[] { CreerMockJudoka(1, "M"), CreerMockJudoka(2, "M") });

            var combats = new List<ICombat>
            {
                // Cbt 1: Decision (etat Vainqueur = 7) -> J1 gagne
                CreerCombat(1, 2, vainqueur: 1, etatJ1: EtatCombattantEnum.Decision),
                // Cbt 2: Abandon (etat Perdant = 2) -> J1 gagne
                CreerCombat(1, 2, vainqueur: 1, etatJ2: EtatCombattantEnum.Abandon),
                // Cbt 3: Forfait (etat Perdant = 3) -> J1 gagne
                CreerCombat(1, 2, vainqueur: 1, etatJ2: EtatCombattantEnum.Forfait),
                // Cbt 4: Medical (etat Perdant = 4) -> J1 gagne
                CreerCombat(1, 2, vainqueur: 1, etatJ2: EtatCombattantEnum.Medical),
                // Cbt 5: Hansoku Make direct ou cumulé (etat Perdant = 5) -> J1 gagne
                CreerCombat(1, 2, vainqueur: 1, etatJ2: EtatCombattantEnum.HansokuMakeH)
            };
            PreparerMockCombats(combats);

            // Act
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);
            var statJ1 = (CompteurStatistiques)moteur.StatsJudokas[1];
            var statJ2 = (CompteurStatistiques)moteur.StatsJudokas[2];

            // Assert
            Assert.Equal(5, statJ1.NbCombats);
            Assert.Equal(5, statJ1.NbVictoires);

            // Décision
            Assert.Equal(1, statJ1.NbVictoireDecision);

            // Les 3 états de défaite par abandon/forfait/médical sont regroupés
            Assert.Equal(3, statJ1.NbVictoireAbandonForfaitMedical);

            // Hansoku Make
            Assert.Equal(1, statJ1.NbVictoireHansokuMake);

            // Le perdant n'a aucune stat de victoire
            Assert.Equal(5, statJ2.NbCombats);
            Assert.Equal(0, statJ2.NbVictoires);
        }

        [Fact]
        public void TraitementCombats_Hikiwake_IncrementePourLesDeuxCombattants()
        {
            // Arrange
            PreparerEnvironnement(EchelonEnum.Club);
            PreparerMockParticipants(new[] { CreerMockJudoka(1, "M"), CreerMockJudoka(2, "M") });

            var combats = new List<ICombat>
            {
                CreerCombat(1, 2, vainqueur: int.MinValue)
            };
            PreparerMockCombats(combats);

            // Act
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);
            var statJ1 = (CompteurStatistiques)moteur.StatsJudokas[1];
            var statJ2 = (CompteurStatistiques)moteur.StatsJudokas[2];

            // Assert
            Assert.Equal(1, statJ1.NbCombats);
            Assert.Equal(1, statJ1.NbHikiwake);
            Assert.Equal(0, statJ1.NbVictoires);

            Assert.Equal(1, statJ2.NbCombats);
            Assert.Equal(1, statJ2.NbHikiwake);
            Assert.Equal(0, statJ2.NbVictoires);
        }

        [Fact]
        public void TraitementCombats_GoldenScoreEtDurees_CalculsCorrects()
        {
            // Arrange
            PreparerEnvironnement(EchelonEnum.Club);
            PreparerMockParticipants(new[] { CreerMockJudoka(1, "M"), CreerMockJudoka(2, "M") });

            var debut = DateTime.Today;

            var combats = new List<ICombat>
            {
                // Cbt 1: Dure 6 min pour un temps nominal de 4 min -> Golden Score (2 min)
                CreerCombat(1, 2, vainqueur: 1, temps: 4, debut: debut, fin: debut.AddMinutes(6)),
                // Cbt 2: Combat rapide -> 1 min -> Pas de GS
                CreerCombat(1, 2, vainqueur: 1, temps: 4, debut: debut, fin: debut.AddMinutes(1))
            };
            PreparerMockCombats(combats);

            // Act
            var moteur = new DataStatistiquesCombats(_mockJudoData.Object);
            var statJ1 = (CompteurStatistiques)moteur.StatsJudokas[1];

            // Assert
            Assert.Equal(2, statJ1.NbCombats);

            // Validation Temps de Combat globaux
            Assert.Equal(TimeSpan.FromMinutes(1), statJ1.DureeCombatMinInterne);
            Assert.Equal(TimeSpan.FromMinutes(6), statJ1.DureeCombatMaxInterne);
            Assert.Equal(TimeSpan.FromMinutes(7), statJ1.TotalDureeCombat); // 6 + 1

            // Validation Golden Score
            Assert.Equal(1, statJ1.NbCombatsGoldenScore);
            Assert.Equal(TimeSpan.FromMinutes(2), statJ1.TotalDureeGoldenScore);
            Assert.Equal(TimeSpan.FromMinutes(2), statJ1.DureeMaximaleGoldenScoreInterne);
        }

        // =================================================================================
        // OUTILS DE PREPARATION ET DE MOCKING (Refactorisés pour s'appuyer sur le constructeur)
        // =================================================================================

        private void PreparerEnvironnement(EchelonEnum niveau)
        {
            var mockComp = new Mock<ICompetition>();
            mockComp.Setup(c => c.id).Returns(99);
            mockComp.Setup(c => c.niveau).Returns((int)niveau);
            mockComp.Setup(c => c.IsShiai()).Returns(true);

            var mockEpM = new Mock<IEpreuve>();
            mockEpM.Setup(e => e.id).Returns(10);
            mockEpM.Setup(e => e.competition).Returns(99);
            mockEpM.Setup(e => e.sexeEnum).Returns(new EpreuveSexe("M"));

            var mockEpF = new Mock<IEpreuve>();
            mockEpF.Setup(e => e.id).Returns(11);
            mockEpF.Setup(e => e.competition).Returns(99);
            mockEpF.Setup(e => e.sexeEnum).Returns(new EpreuveSexe("F"));

            // On surcharge les listes vides créées dans le constructeur avec nos données
            _mockOrg.Setup(o => o.Competitions).Returns(new List<ICompetition> { mockComp.Object });
            _mockOrg.Setup(o => o.Epreuves).Returns(new List<IEpreuve> { mockEpM.Object, mockEpF.Object });
        }

        private void PreparerMockParticipants(IEnumerable<IVueJudoka> judokas)
        {
            _mockParts.Setup(p => p.Vuejudokas).Returns(judokas.ToList());
        }

        private void PreparerMockCombats(IEnumerable<ICombat> combats)
        {
            _mockDeroulement.Setup(d => d.Combats).Returns(combats.Cast<ICombat>().ToList());
        }

        private IVueJudoka CreerMockJudoka(int id, string sexeStr)
        {
            var mock = new Mock<IVueJudoka>();
            mock.Setup(j => j.id).Returns(id);

            // NOUVEAU : Propriétés uniques pour éviter que le .Distinct() du moteur ne les fusionne !
            mock.Setup(j => j.licence).Returns($"LICENCE_{id}");
            mock.Setup(j => j.nom).Returns($"NOM_{id}");
            mock.Setup(j => j.prenom).Returns($"PRENOM_{id}");

            mock.Setup(j => j.sexeEnum).Returns(new EpreuveSexe(sexeStr));

            mock.Setup(j => j.idcompet).Returns(99);
            mock.Setup(j => j.idepreuve).Returns(sexeStr == "M" ? 10 : 11);

            mock.Setup(j => j.club).Returns("CLUB_TEST");
            mock.Setup(j => j.present).Returns(true);

            return mock.Object;
        }

        private ICombat CreerCombat(int id1, int id2, int vainqueur, int score1 = 0, int score2 = 0, EtatCombattantEnum etatJ1 = EtatCombattantEnum.Normal, EtatCombattantEnum etatJ2 = EtatCombattantEnum.Normal, int temps = 4, DateTime? debut = null, DateTime? fin = null)
        {
            // On crée un faux objet (Mock) basé uniquement sur l'interface
            var mock = new Mock<ICombat>();

            // On configure les retours des propriétés
            mock.Setup(c => c.participant1).Returns(id1);
            mock.Setup(c => c.participant2).Returns(id2);
            mock.Setup(c => c.vainqueur).Returns(vainqueur);
            mock.Setup(c => c.score1).Returns(score1);
            mock.Setup(c => c.score2).Returns(score2);

            mock.Setup(c => c.etatJ1).Returns(etatJ1);
            mock.Setup(c => c.etatJ2).Returns(etatJ2);

            mock.Setup(c => c.temps).Returns(temps);
            mock.Setup(c => c.debut).Returns(debut ?? DateTime.Today);
            mock.Setup(c => c.fin).Returns(fin ?? DateTime.Today.AddMinutes(temps));
            mock.Setup(c => c.virtuel).Returns(false);

            // Retourne l'instance générée par Moq (qui implémente ICombat)
            return mock.Object;
        }
    }
}