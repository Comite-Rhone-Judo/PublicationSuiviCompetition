#nullable enable
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau.Participants;

namespace FranceJudo.Metier.Tests.Noyau.Participants
{
    public class DataParticipantsExtensionTests
    {
        [Fact]
        public void GetJudokaEpreuve_RetourneUniquementLesJudokasInscritsALEpreuveCible()
        {
            // Arrange
            int targetEpreuveId = 42;
            int otherEpreuveId = 99;

            // 1. Création des Mocks Judokas
            var mockJudoka1 = new Mock<IJudoka>();
            mockJudoka1.Setup(j => j.id).Returns(1);
            mockJudoka1.Setup(j => j.nom).Returns("Riner");

            var mockJudoka2 = new Mock<IJudoka>();
            mockJudoka2.Setup(j => j.id).Returns(2);
            mockJudoka2.Setup(j => j.nom).Returns("Agbegnenou");

            var mockJudoka3 = new Mock<IJudoka>();
            mockJudoka3.Setup(j => j.id).Returns(3);
            mockJudoka3.Setup(j => j.nom).Returns("Douillet");

            // 2. Création des Mocks Inscriptions (EpreuveJudokas)
            var mockInscript1 = new Mock<IEpreuveJudoka>();
            mockInscript1.Setup(e => e.epreuve).Returns(targetEpreuveId);
            mockInscript1.Setup(e => e.judoka).Returns(1); // Judoka 1 inscrit à l'épreuve cible

            var mockInscript2 = new Mock<IEpreuveJudoka>();
            mockInscript2.Setup(e => e.epreuve).Returns(targetEpreuveId);
            mockInscript2.Setup(e => e.judoka).Returns(2); // Judoka 2 inscrit à l'épreuve cible

            var mockInscript3 = new Mock<IEpreuveJudoka>();
            mockInscript3.Setup(e => e.epreuve).Returns(otherEpreuveId);
            mockInscript3.Setup(e => e.judoka).Returns(3); // Judoka 3 inscrit AILLEURS

            // 3. Assemblage du DataContext Mocker
            var mockDataContext = new Mock<IParticipantsData>();
            mockDataContext.Setup(d => d.Judokas).Returns(new List<IJudoka>
            {
                mockJudoka1.Object,
                mockJudoka2.Object,
                mockJudoka3.Object
            });
            mockDataContext.Setup(d => d.EpreuveJudokas).Returns(new List<IEpreuveJudoka>
            {
                mockInscript1.Object,
                mockInscript2.Object,
                mockInscript3.Object
            });

            // Act
            var result = mockDataContext.Object.GetJudokaEpreuve(targetEpreuveId).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2, "Seuls deux judokas sont inscrits à l'épreuve cible.");

            // Vérification que les bons judokas ont été retournés
            result.Select(j => j.id).Should().Contain(1);
            result.Select(j => j.id).Should().Contain(2);
            result.Select(j => j.id).Should().NotContain(3, "Le judoka 3 est inscrit à une autre épreuve.");
        }

        [Fact]
        public void GetJudokaEpreuve_EpreuveVide_RetourneListe_Vide()
        {
            // Arrange
            int targetEpreuveId = 42;

            var mockJudoka = new Mock<IJudoka>();
            mockJudoka.Setup(j => j.id).Returns(1);

            var mockDataContext = new Mock<IParticipantsData>();
            mockDataContext.Setup(d => d.Judokas).Returns(new List<IJudoka> { mockJudoka.Object });
            // Aucune inscription (EpreuveJudokas vide)
            mockDataContext.Setup(d => d.EpreuveJudokas).Returns(new List<IEpreuveJudoka>());

            // Act
            var result = mockDataContext.Object.GetJudokaEpreuve(targetEpreuveId).ToList();

            // Assert
            result.Should().BeEmpty("Aucun judoka n'est inscrit, la méthode doit retourner une séquence vide (et non null).");
        }
    }
}