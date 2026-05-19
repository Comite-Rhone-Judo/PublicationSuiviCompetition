#nullable enable
using AppPublication.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau.Participants;
using Moq;
using Xunit;

namespace AppPublication.Tests.ExtensionNoyau.Engagement
{
    public class VueJudokaEqualityComparerTests
    {
        [Fact]
        public void Equals_AvecMemesNomEtPrenom_RetourneTrue()
        {
            // Arrange
            VueJudokaEqualityComparer comparateur = new VueJudokaEqualityComparer();

            Mock<IVueJudoka> mockJ1 = new Mock<IVueJudoka>();
            mockJ1.SetupGet(j => j.nom).Returns("DUPONT");
            mockJ1.SetupGet(j => j.prenom).Returns("Jean");

            Mock<IVueJudoka> mockJ2 = new Mock<IVueJudoka>();
            mockJ2.SetupGet(j => j.nom).Returns("DUPONT");
            mockJ2.SetupGet(j => j.prenom).Returns("Jean");

            // Act
            bool resultat = comparateur.Equals(mockJ1.Object, mockJ2.Object);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public void Equals_AvecNomsDifferents_RetourneFalse()
        {
            // Arrange
            VueJudokaEqualityComparer comparateur = new VueJudokaEqualityComparer();

            Mock<IVueJudoka> mockJ1 = new Mock<IVueJudoka>();
            mockJ1.SetupGet(j => j.nom).Returns("DUPONT");
            mockJ1.SetupGet(j => j.prenom).Returns("Jean");

            Mock<IVueJudoka> mockJ2 = new Mock<IVueJudoka>();
            mockJ2.SetupGet(j => j.nom).Returns("MARTIN");
            mockJ2.SetupGet(j => j.prenom).Returns("Jean");

            // Act
            bool resultat = comparateur.Equals(mockJ1.Object, mockJ2.Object);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public void GetHashCode_GereLesNomsNullsProprement()
        {
            // Arrange
            VueJudokaEqualityComparer comparateur = new VueJudokaEqualityComparer();
            Mock<IVueJudoka> mockJ1 = new Mock<IVueJudoka>();
            // nom et prenom ne sont pas configurés (ils vaudront null)

            // Act
            int hash = comparateur.GetHashCode(mockJ1.Object);

            // Assert
            Assert.Equal(0, hash); // 0 ^ 0 = 0
        }
    }
}