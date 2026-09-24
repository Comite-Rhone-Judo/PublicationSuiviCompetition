using System;
using Xunit;
using FranceJudo.Metier.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau.Organisation;

namespace FranceJudo.Metier.Tests.ExtensionNoyau.StatistiquesCombats
{
    public class CompteurStatistiquesTests
    {
        [Fact]
        public void Ratios_ZeroCombat_RetournentNull()
        {
            // Arrange
            var compteur = new CompteurStatistiques(EchelonEnum.Club) { NbCombats = 0 };

            // Act & Assert - Vérification de la protection contre la division par zéro
            Assert.Null(compteur.PctVictoires);
            Assert.Null(compteur.PctHikiwake);
            Assert.Null(compteur.PctVictoireIpponDirect);
            Assert.Null(compteur.PctVictoireWazaAri);
            Assert.Null(compteur.PctVictoireDecision);
            Assert.Null(compteur.MoyennePenalitesParCombat);
            // Assert.Null(compteur.PctCombatsGoldenScore);
            Assert.Null(compteur.DureeCombatMin);
            Assert.Null(compteur.DureeCombatMax);
            Assert.Null(compteur.DureeCombatMoy);
        }

        [Fact]
        public void PctParticipation_CalculCorrect()
        {
            var compteur = new CompteurStatistiques(EchelonEnum.Club)
            {
                NbParticipants = 10,
                NbCombattants = 8 // 8 présents sur 10 inscrits
            };

            Assert.Equal(0.8, compteur.PctParticipation);
        }

        [Fact]
        public void ProfilVictoires_CalculPourcentagesCorrects()
        {
            // Arrange
            var compteur = new CompteurStatistiques(EchelonEnum.Club)
            {
                NbCombats = 10,
                NbVictoires = 9,
                NbHikiwake = 1,
                NbVictoireIpponDirect = 2,
                NbVictoireWazaAriAwaseteIppon = 1,
                NbVictoireWazaAri = 2,
                NbVictoireYuko = 0, // Ancien système potentiellement à 0
                NbVictoireSogoGachi = 1,
                NbVictoireHansokuMake = 1,
                NbVictoireAbandonForfaitMedical = 1,
                NbVictoireDecision = 1
            };

            // Act & Assert
            Assert.Equal(0.9, compteur.PctVictoires);
            Assert.Equal(0.1, compteur.PctHikiwake);
            Assert.Equal(0.2, compteur.PctVictoireIpponDirect);
            Assert.Equal(0.1, compteur.PctVictoireWazaAriAwaseteIppon);
            Assert.Equal(0.2, compteur.PctVictoireWazaAri);
            Assert.Equal(0.0, compteur.PctVictoireYuko);
            Assert.Equal(0.1, compteur.PctVictoireSogoGachi);
            Assert.Equal(0.1, compteur.PctVictoireHansokuMake);
            Assert.Equal(0.1, compteur.PctVictoireAbandonForfaitMedical);
            Assert.Equal(0.1, compteur.PctVictoireDecision);
        }

        [Fact]
        public void Penalites_MoyenneCalculCorrect()
        {
            var compteur = new CompteurStatistiques(EchelonEnum.Club)
            {
                NbCombats = 4,
                TotalPenalites = 6
            };

            Assert.Equal(1.5, compteur.MoyennePenalitesParCombat); // 6 / 4
        }

        /*
        [Fact]
        public void GoldenScore_RatiosEtTemps_CalculsCorrects()
        {
            var compteur = new CompteurStatistiques(EchelonEnum.Club)
            {
                NbCombats = 5,
                NbCombatsGoldenScore = 2,
                TotalDureeGoldenScore = TimeSpan.FromSeconds(150), // 2 min 30s au total
                DureeMaximaleGoldenScoreInterne = TimeSpan.FromSeconds(100)
            };

            Assert.Equal(0.4, compteur.PctCombatsGoldenScore); // 2 / 5 = 40%
            Assert.Equal(TimeSpan.FromSeconds(75), compteur.DureeMoyenneGoldenScore); // 150s / 2
            Assert.Equal(TimeSpan.FromSeconds(100), compteur.DureeMaximaleGoldenScore);
        }*/

        [Fact]
        public void DureesCombat_MinMaxMoyenne_CalculsCorrects()
        {
            var compteur = new CompteurStatistiques(EchelonEnum.Club)
            {
                NbCombats = 4,
                TotalDureeCombat = TimeSpan.FromMinutes(10), // 10 minutes total
                DureeCombatMinInterne = TimeSpan.FromSeconds(20),
                DureeCombatMaxInterne = TimeSpan.FromMinutes(4)
            };

            Assert.Equal(TimeSpan.FromSeconds(20), compteur.DureeCombatMin);
            Assert.Equal(TimeSpan.FromMinutes(4), compteur.DureeCombatMax);
            Assert.Equal(TimeSpan.FromSeconds(150), compteur.DureeCombatMoy); // 10 mins (600s) / 4 = 150s
        }
    }
}