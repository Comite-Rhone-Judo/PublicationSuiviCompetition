#nullable enable
using FranceJudo.Metier.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau.Deroulement;
using Moq;
using Xunit;

namespace FranceJudo.Metier.Tests.ExtensionNoyau.Engagement
{
    public class CombatEqualityComparerTests
    {
        [Fact]
        public void Equals_AvecReferencesIdentiques_RetourneTrue()
        {
            // Arrange
            CombatEqualityComparer comparateur = new CombatEqualityComparer();
            Mock<ICombat> mockCombat = new Mock<ICombat>();

            // Act
            bool resultat = comparateur.Equals(mockCombat.Object, mockCombat.Object);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public void Equals_AvecDeuxNull_RetourneTrue()
        {
            // Arrange
            CombatEqualityComparer comparateur = new CombatEqualityComparer();

            // Act
            // Object.ReferenceEquals(null, null) renvoie true, c'est le comportement attendu en C#
            bool resultatDeuxNull = comparateur.Equals(null!, null!);

            // Assert
            Assert.True(resultatDeuxNull);
        }

        [Fact]
        public void Equals_AvecUnSeulNull_RetourneFalse()
        {
            // Arrange
            CombatEqualityComparer comparateur = new CombatEqualityComparer();
            Mock<ICombat> mockCombat = new Mock<ICombat>();

            // Act
            bool resultatXNull = comparateur.Equals(null!, mockCombat.Object);
            bool resultatYNull = comparateur.Equals(mockCombat.Object, null!);

            // Assert
            Assert.False(resultatXNull);
            Assert.False(resultatYNull);
        }

        [Fact]
        public void Equals_AvecMemesId_RetourneTrue()
        {
            // Arrange
            CombatEqualityComparer comparateur = new CombatEqualityComparer();
            Mock<ICombat> mockCombat1 = new Mock<ICombat>();
            mockCombat1.SetupGet(c => c.id).Returns(42);

            Mock<ICombat> mockCombat2 = new Mock<ICombat>();
            mockCombat2.SetupGet(c => c.id).Returns(42);

            // Act
            bool resultat = comparateur.Equals(mockCombat1.Object, mockCombat2.Object);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public void GetHashCode_AvecNull_RetourneZero()
        {
            // Arrange
            CombatEqualityComparer comparateur = new CombatEqualityComparer();

            // Act
            int hash = comparateur.GetHashCode(null!);

            // Assert
            Assert.Equal(0, hash);
        }

        [Fact]
        public void GetHashCode_RetourneId()
        {
            // Arrange
            CombatEqualityComparer comparateur = new CombatEqualityComparer();
            Mock<ICombat> mockCombat = new Mock<ICombat>();
            mockCombat.SetupGet(c => c.id).Returns(99);

            // Act
            int hash = comparateur.GetHashCode(mockCombat.Object);

            // Assert
            Assert.Equal(99, hash);
        }
    }
}