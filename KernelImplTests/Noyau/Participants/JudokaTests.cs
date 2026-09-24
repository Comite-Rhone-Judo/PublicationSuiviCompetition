using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.XML;
using KernelImpl.Noyau.Participants;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Participants
{
    public class JudokaTests
    {
        [Fact]
        public void Judoka_PropertyChanged_ShouldBeRaised()
        {
            // Arrange
            Judoka judoka = new Judoka();
            List<string> proprietesModifiees = new List<string>();

            judoka.PropertyChanged += (sender, e) =>
            {
                proprietesModifiees.Add(e.PropertyName ?? string.Empty);
            };

            // Act
            judoka.nom = "Riner";
            judoka.poidsMesure = 140000;

            // Assert
            Assert.Contains("nom", proprietesModifiees);
            Assert.Contains("poidsMesure", proprietesModifiees);
        }

        [Fact]
        public void Judoka_ToXml_ShouldIncludePointsAndRemoteId_WhenEpreuveJudokaExists()
        {
            // Arrange
            Judoka judoka = new Judoka
            {
                id = 55,
                nom = "TOTO",
                prenom = "Titi",
                remoteID = "REMOTE_TEST",
                licence = "L123456",
                datePesee = new DateTime(2023, 10, 01, 8, 30, 0)
            };

            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<IParticipantsData> mockParticipants = new Mock<IParticipantsData>();
            Mock<IOrganisationData> mockOrganisation = new Mock<IOrganisationData>();

            Mock<IEpreuveJudoka> mockEpreuveJudoka = new Mock<IEpreuveJudoka>();
            mockEpreuveJudoka.SetupGet(ej => ej.judoka).Returns(55);
            mockEpreuveJudoka.SetupGet(ej => ej.epreuve).Returns(12);
            mockEpreuveJudoka.SetupGet(ej => ej.points).Returns(100);
            mockEpreuveJudoka.SetupGet(ej => ej.serie).Returns(1);

            Mock<IVueEpreuve> mockVueEpreuve = new Mock<IVueEpreuve>();
            mockVueEpreuve.SetupGet(ve => ve.id).Returns(12);
            mockVueEpreuve.SetupGet(ve => ve.remoteId_catepoids).Returns("REMOTE_CATE");

            List<IEpreuveJudoka> listeEpreuveJudokas = new List<IEpreuveJudoka> { mockEpreuveJudoka.Object };
            List<IVueEpreuve> listeVueEpreuves = new List<IVueEpreuve> { mockVueEpreuve.Object };

            mockParticipants.SetupGet(p => p.EpreuveJudokas).Returns(listeEpreuveJudokas);
            mockOrganisation.SetupGet(o => o.VueEpreuves).Returns(listeVueEpreuves);

            mockJudoData.SetupGet(d => d.Participants).Returns(mockParticipants.Object);
            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisation.Object);

            // Act
            XElement resultatXml = judoka.ToXml(mockJudoData.Object);

            // Assert
            Assert.NotNull(resultatXml);
            // Vérification que les valeurs spécifiques liées à l'épreuve sont intégrées
            Assert.Equal("100", resultatXml.Attribute(ConstantXML.Judoka_Points)?.Value);
            Assert.Equal("1", resultatXml.Attribute(ConstantXML.Judoka_Serie)?.Value);
            Assert.Equal("REMOTE_CATE", resultatXml.Attribute(ConstantXML.Judoka_CatePoids_RemoteId)?.Value);
        }
            
         [Fact]
        public void PoidsKg_ShouldReturnCorrectFloatValue()
        {
            // Arrange
            Judoka judoka = new Judoka
            {
                // Act
                poidsKg = 73.5f
            };

            // Assert
            Assert.Equal(73.5f, judoka.poidsKg);
        }
    }
}