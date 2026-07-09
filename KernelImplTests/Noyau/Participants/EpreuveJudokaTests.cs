using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using KernelImpl.Noyau.Participants;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Participants
{
    public class EpreuveJudokaTests
    {
        [Fact]
        public void EpreuveJudoka_XmlSerialization_ShouldMapAllProperties()
        {
            // Arrange
            EpreuveJudoka source = new EpreuveJudoka
            {
                id = 1,
                epreuve = 10,
                judoka = 100,
                etat = EtatJudokaEnum.Present,
                classement = 1,
                serie = 3,
                serie2 = 4,
                observation = 5,
                points = 10
            };

            // Act
            XElement xmlElement = source.ToXml(null);
            EpreuveJudoka destination = new EpreuveJudoka();
            destination.LoadXml(xmlElement);

            // Assert
            Assert.Equal(source.id, destination.id);
            Assert.Equal(source.epreuve, destination.epreuve);
            Assert.Equal(source.judoka, destination.judoka);
            Assert.Equal(source.etat, destination.etat);
            Assert.Equal(source.classement, destination.classement);
            Assert.Equal(source.serie, destination.serie);
            Assert.Equal(source.serie2, destination.serie2);
            Assert.Equal(source.observation, destination.observation);
            Assert.Equal(source.points, destination.points);
        }

        [Fact]
        public void Epreuve1_ShouldReturnCorrectEpreuve_FromJudoData()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<IOrganisationData> mockOrganisationData = new Mock<IOrganisationData>();
            Mock<IEpreuve> mockEpreuve = new Mock<IEpreuve>();

            mockEpreuve.SetupGet(e => e.id).Returns(10);

            List<IEpreuve> listeEpreuves = new List<IEpreuve> { mockEpreuve.Object };
            mockOrganisationData.SetupGet(o => o.Epreuves).Returns(listeEpreuves);
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisationData.Object);

            EpreuveJudoka epreuveJudoka = new EpreuveJudoka { epreuve = 10 };

            // Act
            IEpreuve resultat = epreuveJudoka.Epreuve1(mockJudoData.Object);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(10, resultat.id);
        }

        [Fact]
        public void Judoka1_ShouldReturnCorrectJudoka_FromJudoData()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<IParticipantsData> mockParticipantsData = new Mock<IParticipantsData>();
            Mock<IJudoka> mockJudoka = new Mock<IJudoka>();

            mockJudoka.SetupGet(j => j.id).Returns(100);

            List<IJudoka> listeJudokas = new List<IJudoka> { mockJudoka.Object };
            mockParticipantsData.SetupGet(p => p.Judokas).Returns(listeJudokas);
            mockJudoData.SetupGet(d => d.Participants).Returns(mockParticipantsData.Object);

            EpreuveJudoka epreuveJudoka = new EpreuveJudoka { judoka = 100 };

            // Act
            IJudoka resultat = epreuveJudoka.Judoka1(mockJudoData.Object);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(100, resultat.id);
        }
    }
}