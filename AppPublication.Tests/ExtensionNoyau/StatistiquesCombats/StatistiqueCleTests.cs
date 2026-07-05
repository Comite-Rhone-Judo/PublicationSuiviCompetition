using Xunit;
using FranceJudo.Metier.Noyau.Organisation;
using AppPublication.ExtensionNoyau.StatistiquesCombats;

namespace AppPublication.Tests.ExtensionNoyau.StatistiquesCombats
{
    public class StatistiqueCleTests
    {
        [Fact]
        public void Equals_MemesValeurs_RetourneTrue()
        {
            // Arrange
            var cle1 = new GroupeStatistiques(123, new EpreuveSexe("M"), "123", (int)EchelonEnum.Club);
            var cle2 = new GroupeStatistiques(123, new EpreuveSexe("M"), "123", (int)EchelonEnum.Club);

            // Act & Assert
            Assert.True(cle1.Equals(cle2));
            Assert.Equal(cle1.GetHashCode(), cle2.GetHashCode());
        }

        [Fact]
        public void Equals_ValeursDifferentes_RetourneFalse()
        {
            // Arrange
            var cleBase = new GroupeStatistiques(123, new EpreuveSexe("M"), "123", (int) EchelonEnum.Club);
            var cleDiffSexe = new GroupeStatistiques(123, new EpreuveSexe("F"), "123", (int)EchelonEnum.Club);
            var cleDiffId = new GroupeStatistiques(123, new EpreuveSexe("M"), "999", (int)EchelonEnum.Club);
            var cleDiffType = new GroupeStatistiques(123, new EpreuveSexe("M"), "123", (int)EchelonEnum.Aucun);

            // Act & Assert
            Assert.False(cleBase.Equals(cleDiffSexe));
            Assert.False(cleBase.Equals(cleDiffId));
            Assert.False(cleBase.Equals(cleDiffType));
        }

        [Fact]
        public void ToString_FormatAttendu()
        {
            // Arrange
            var cle = new GroupeStatistiques(123, new EpreuveSexe("F"), "Ligue_AURA", (int)EchelonEnum.Ligue);

            // Act
            string? resultat = cle.ToString();

            // Assert
            Assert.Equal("123-F-Ligue_AURA-Structure", resultat);
        }
    }
}