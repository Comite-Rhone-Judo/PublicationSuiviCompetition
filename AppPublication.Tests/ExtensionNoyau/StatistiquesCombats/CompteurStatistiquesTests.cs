using System;
using Xunit;
using AppPublication.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau.Organisation;

namespace AppPublication.Tests.ExtensionNoyau.StatistiquesCombats
{
    public class CompteurStatistiquesTests
    {
        [Fact]
        public void Ratios_ZeroCombat_RetournentNull()
        {
            // Arrange
            var compteur = new CompteurStatistiques(EchelonEnum.Club);
            compteur.NbCombats = 0; // Aucun combat

            // Act & Assert
            Assert.Null(compteur.PctVictoires);
            Assert.Null(compteur.PctHikiwake);
            Assert.Null(compteur.PctVictoireIpponDirect);
            Assert.Null(compteur.MoyennePenalitesParCombat);
            Assert.Null(compteur.DureeCombatMoy);
        }

        [Fact]
        public void PctVictoires_CalculCorrect()
        {
            // Arrange
            var compteur = new CompteurStatistiques(EchelonEnum.Club)
            {
                NbCombats = 4,
                NbVictoires = 1,
                NbHikiwake = 2
            };

            // Act & Assert
            Assert.Equal(0.25, compteur.PctVictoires); // 1 / 4 = 25%
            Assert.Equal(0.50, compteur.PctHikiwake);  // 2 / 4 = 50%
        }

        [Fact]
        public void PctParticipation_CalculCorrect()
        {
            // Arrange
            var compteur = new CompteurStatistiques(EchelonEnum.Club)
            {
                NbParticipants = 10,
                NbCombattants = 8 // 8 présents sur 10 inscrits
            };

            // Act & Assert
            Assert.Equal(0.8, compteur.PctParticipation);
        }

        [Fact]
        public void DureesCombat_MoyenneCalculCorrecte()
        {
            // Arrange
            var compteur = new CompteurStatistiques(EchelonEnum.Aucun)
            {
                NbCombats = 3,
                TotalDureeCombat = TimeSpan.FromMinutes(9) // 9 minutes au total
            };

            // Act & Assert
            Assert.Equal(TimeSpan.FromMinutes(3), compteur.DureeCombatMoy);
        }
    }
}