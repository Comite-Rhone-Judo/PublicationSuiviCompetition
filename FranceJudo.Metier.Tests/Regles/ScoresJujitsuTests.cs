#nullable enable
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Regles;

namespace FranceJudo.Metier.Tests.Regles
{
    public class ScoresJujitsuTests
    {
        [Fact]
        public void IsFullIppon_RetourneVrai_UniquementSiIpponDansLesTroisParties()
        {
            // Arrange
            var scoresJ1 = new ScoresJujitsu
            {
                // Judoka 1 a un ippon dans chaque partie (peu importe si c'est le 1er ou 2ème)
                ippon1_1_2 = 1, // Partie 1
                ippon1_2_1 = 1, // Partie 2
                ippon1_3_2 = 1  // Partie 3
            };

            var scoresJ2Incomplet = new ScoresJujitsu
            {
                // Judoka 2 manque la partie 3
                ippon2_1_1 = 1,
                ippon2_2_2 = 1
            };

            // Act & Assert
            scoresJ1.IsFullIppon(1).Should().BeTrue("Le Judoka 1 a validé les 3 parties.");
            scoresJ1.IsFullIppon(2).Should().BeFalse();

            scoresJ2Incomplet.IsFullIppon(2).Should().BeFalse("Le Judoka 2 n'a pas validé la partie 3.");
        }

        [Fact]
        public void NbPartiesValides_CompteCorrectementLesPartiesAvecAuMoinsUnIppon()
        {
            // Arrange
            var scores = new ScoresJujitsu
            {
                // Judoka 1 : Valide uniquement la partie 1 et 3
                ippon1_1_1 = 1,
                ippon1_1_2 = 1, // 2 ippons dans la même partie ne comptent que pour 1 partie valide
                ippon1_3_1 = 1,

                // Judoka 2 : Ne valide aucune partie
                ippon2_1_1 = 0
            };

            // Act & Assert
            scores.NbPartiesValides(1).Should().Be(2, "Le Judoka 1 a validé la partie 1 et la partie 3.");
            scores.NbPartiesValides(2).Should().Be(0, "Le Judoka 2 n'a aucun ippon.");
        }

        [Fact]
        public void NbIppons_FaitLaSommeTotaleDesIppons()
        {
            // Arrange
            var scores = new ScoresJujitsu
            {
                ippon2_1_1 = 1,
                ippon2_2_2 = 2,
                ippon2_3_1 = 1
            };

            // Act & Assert
            scores.NbIppons(1).Should().Be(0);
            scores.NbIppons(2).Should().Be(4, "La somme totale des Ippons du Judoka 2 est 4.");
        }

        [Fact]
        public void GetPointsPenalitesCombat_CalculeSelonLeBaremeShidoEtChui()
        {
            // Arrange
            var scores = new ScoresJujitsu
            {
                shido1 = 2, // 2 * 1 = 2
                chui1 = 1,  // 1 * 3 = 3 -> Total = 5

                shido2 = 0,
                chui2 = 2   // 2 * 3 = 6 -> Total = 6
            };

            // Act & Assert
            scores.GetPointsPenalitesCombat(1).Should().Be(5, "2 Shidos (2pts) + 1 Chui (3pts) = 5.");
            scores.GetPointsPenalitesCombat(2).Should().Be(6, "2 Chuis (6pts) = 6.");
        }

        [Fact]
        public void ScoreJsonToString_SerializeCorrectementLobjet()
        {
            // Arrange
            var scores = new ScoresJujitsu
            {
                avantages1 = 5,
                shido2 = 1,
                ippon1_1_1 = 1
            };

            // Act
            string json = ScoresJujitsu.ScoreJsonToString(scores);

            // Assert
            json.Should().NotBeNullOrWhiteSpace();
            json.Should().Contain("\"avantages1\":5");
            json.Should().Contain("\"shido2\":1");
            json.Should().Contain("\"ippon1_1_1\":1");
        }
    }
}