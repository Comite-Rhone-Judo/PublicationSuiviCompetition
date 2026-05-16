#nullable enable
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Regles;
using FranceJudo.Metier.Noyau.Deroulement;

namespace FranceJudo.Metier.Tests.Regles
{
    public class ScoresProLeagueTests
    {
        [Theory]
        // Règle 1 : Waza-ari classique (1 Waza = 1 pt)
        [InlineData(1, 0, 0, EtatCombattantEnum.Normal, 0, 0, 0, EtatCombattantEnum.Normal, 1, 0)]
        // Règle 2 : Deux Waza-ari = 10 pts
        [InlineData(2, 0, 0, EtatCombattantEnum.Normal, 0, 0, 0, EtatCombattantEnum.Normal, 10, 0)]
        // Règle 3 : Trois Waza-ari = 11 pts
        [InlineData(3, 0, 0, EtatCombattantEnum.Normal, 0, 0, 0, EtatCombattantEnum.Normal, 11, 0)]
        // Règle 4 : Quatre Waza-ari = 20 pts (Maximum)
        [InlineData(4, 0, 0, EtatCombattantEnum.Normal, 0, 0, 0, EtatCombattantEnum.Normal, 20, 0)]
        // Règle 5 : 1 Ippon = 10 pts
        [InlineData(0, 1, 0, EtatCombattantEnum.Normal, 0, 0, 0, EtatCombattantEnum.Normal, 10, 0)]
        // Règle 6 : 1 Ippon + 1 Waza-ari = 11 pts
        [InlineData(1, 1, 0, EtatCombattantEnum.Normal, 0, 0, 0, EtatCombattantEnum.Normal, 11, 0)]
        // Règle 7 : 1 Ippon + 2 Waza-ari (ou plus) est plafonné à 20 pts
        [InlineData(3, 1, 0, EtatCombattantEnum.Normal, 0, 0, 0, EtatCombattantEnum.Normal, 20, 0)]
        public void GetScoresProLeague_Calcul_Des_Points_Waza_Et_Ippon(
            int waza1, int ippon1, int shido1, EtatCombattantEnum etat1,
            int waza2, int ippon2, int shido2, EtatCombattantEnum etat2,
            int scoreAttenduJ1, int scoreAttenduJ2)
        {
            // Act (Yuko n'a pas d'impact dans les règles métier actuelles, on passe 0)
            var result = ScoresProLeague.getScoresProLeague(
                waza1, ippon1, 0, shido1, etat1,
                waza2, ippon2, 0, shido2, etat2);

            // Assert
            result.Should().HaveCount(2);
            result[0].Should().Be(scoreAttenduJ1, "Le score du J1 est incorrect.");
            result[1].Should().Be(scoreAttenduJ2, "Le score du J2 est incorrect.");
        }

        [Theory]
        // J1 prend 3 Shidos => J2 gagne 20 pts
        [InlineData(3, 0, 0, 20)]
        // Les deux prennent 3 Shidos => Les deux adversaires prennent 20 pts (donc 20 - 20)
        [InlineData(3, 3, 20, 20)]
        public void GetScoresProLeague_Plafond_20_Points_Pour_3_Shidos(
            int shidoJ1, int shidoJ2,
            int scoreAttenduJ1, int scoreAttenduJ2)
        {
            // Act
            var result = ScoresProLeague.getScoresProLeague(
                0, 0, 0, shidoJ1, EtatCombattantEnum.Normal,
                0, 0, 0, shidoJ2, EtatCombattantEnum.Normal);

            // Assert
            result[0].Should().Be(scoreAttenduJ1);
            result[1].Should().Be(scoreAttenduJ2);
        }

        [Theory]
        [InlineData(EtatCombattantEnum.HansokuMakeX)]
        [InlineData(EtatCombattantEnum.HansokuMakeH)]
        [InlineData(EtatCombattantEnum.Forfait)]
        [InlineData(EtatCombattantEnum.Abandon)]
        [InlineData(EtatCombattantEnum.Medical)]
        public void GetScoresProLeague_EtatsDiciplinaires_Donnent20PointsAlAdversaire(EtatCombattantEnum etatJ1)
        {
            // Arrange & Act
            // Le Judoka 1 a un état qui déclenche la victoire automatique du Judoka 2
            var result = ScoresProLeague.getScoresProLeague(
                0, 0, 0, 0, etatJ1,
                0, 0, 0, 0, EtatCombattantEnum.Normal);

            // Assert
            result[0].Should().Be(0, "Le J1 ne marque pas de points.");
            result[1].Should().Be(20, "Le J2 reçoit 20 points à cause de la pénalité majeure du J1.");
        }

        [Fact]
        public void GetScoresProLeague_Plafond_Absolu_De_20_Points()
        {
            // Arrange & Act
            // J1 fait 1 ippon (10) + 4 waza (20) + J2 est disqualifié (20) = 50 points théoriques
            var result = ScoresProLeague.getScoresProLeague(
                4, 1, 0, 0, EtatCombattantEnum.Normal,
                0, 0, 0, 0, EtatCombattantEnum.HansokuMakeX);

            // Assert
            result[0].Should().Be(20, "Le score ne doit JAMAIS dépasser 20 points, peu importe le cumul des actions et pénalités.");
        }
    }
}