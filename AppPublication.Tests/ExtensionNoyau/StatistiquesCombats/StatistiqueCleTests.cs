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
            var cle1 = new StatistiqueCle(TypeEntiteStatistique.Structure, "123", new EpreuveSexe("M"));
            var cle2 = new StatistiqueCle(TypeEntiteStatistique.Structure, "123", new EpreuveSexe("M"));

            // Act & Assert
            Assert.True(cle1.Equals(cle2));
            Assert.Equal(cle1.GetHashCode(), cle2.GetHashCode());
        }

        [Fact]
        public void Equals_ValeursDifferentes_RetourneFalse()
        {
            // Arrange
            var cleBase = new StatistiqueCle(TypeEntiteStatistique.Structure, "123", new EpreuveSexe("M"));
            var cleDiffSexe = new StatistiqueCle(TypeEntiteStatistique.Structure, "123", new EpreuveSexe("F"));
            var cleDiffId = new StatistiqueCle(TypeEntiteStatistique.Structure, "999", new EpreuveSexe("M"));
            var cleDiffType = new StatistiqueCle(TypeEntiteStatistique.Judoka, "123", new EpreuveSexe("M"));

            // Act & Assert
            Assert.False(cleBase.Equals(cleDiffSexe));
            Assert.False(cleBase.Equals(cleDiffId));
            Assert.False(cleBase.Equals(cleDiffType));
        }

        [Fact]
        public void ToString_FormatAttendu()
        {
            // Arrange
            var cle = new StatistiqueCle(TypeEntiteStatistique.Structure, "Ligue_AURA", new EpreuveSexe("F"));

            // Act
            string resultat = cle.ToString();

            // Assert
            Assert.Equal("Structure-Ligue_AURA-F", resultat);
        }
    }
}