using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using KernelImpl.Noyau.Deroulement;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class CombatTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            // Arrange
            Combat original = new Combat
            {
                id = 15,
                numero = 5,
                reference = "REF_1",
                participant1 = 100,
                participant2 = 200,
                score1 = 10,
                score2 = 0,
                penalite1 = 1,
                penalite2 = 2,
                etatJ1 = EtatCombattantEnum.Normal,
                etatJ2 = EtatCombattantEnum.Normal,
                phase = 3,
                details = "Finale",
                temps = 120.5,
                etat = EtatCombatEnum.Creer,
                vainqueur = 100,
                virtuel = false,
                epreuve = 99,
                goldenScore = true
            };

            // Act : Le test valide maintenant que le passage de 'null' 
            // pour IJudoData ne fait plus planter la génération XML !
            XElement xml = original.ToXml(null);

            Combat copie = new Combat();
            copie.LoadXml(xml);

            // Assert
            Assert.Equal(original.id, copie.id);
            Assert.Equal(original.numero, copie.numero);
            Assert.Equal(original.reference, copie.reference);
            Assert.Equal(original.participant1, copie.participant1);
            Assert.Equal(original.participant2, copie.participant2);
            Assert.Equal(original.score1, copie.score1);
            Assert.Equal(original.score2, copie.score2);
            Assert.Equal(original.phase, copie.phase);
            Assert.Equal(original.details, copie.details);
            Assert.Equal(original.etat, copie.etat);
            Assert.Equal(original.vainqueur, copie.vainqueur);
            Assert.Equal(original.virtuel, copie.virtuel);
            Assert.Equal(original.epreuve, copie.epreuve);
            Assert.Equal(original.goldenScore, copie.goldenScore);
        }

        [Fact]
        public void LectureCombats_ParseUneListeDepuisUnXElement()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            Mock<IOrganisationData> mockOrganisation = new Mock<IOrganisationData>();
            Mock<IDeroulementData> mockDeroulement = new Mock<IDeroulementData>();
            Mock<ICompetition> mockCompetition = new Mock<ICompetition>();

            List<IVueEpreuve> listeVuesEpreuves = new List<IVueEpreuve>();
            List<IFeuille> listeFeuilles = new List<IFeuille>();

            // Hydratation de la hiérarchie pour sécuriser les appels ToXml
            mockOrganisation.SetupGet(o => o.VueEpreuves).Returns(listeVuesEpreuves);
            mockOrganisation.SetupGet(o => o.Competition).Returns(mockCompetition.Object);
            mockDeroulement.SetupGet(d => d.Feuilles).Returns(listeFeuilles);

            mockJudoData.SetupGet(d => d.Organisation).Returns(mockOrganisation.Object);
            mockJudoData.SetupGet(d => d.Deroulement).Returns(mockDeroulement.Object);

            Combat c1 = new Combat { id = 1, numero = 10 };
            Combat c2 = new Combat { id = 2, numero = 20 };

            // Act : Génération du document XML (ne crashera plus grâce au Mock + correctif ToXml)
            XElement root = new XElement("Root", c1.ToXml(mockJudoData.Object), c2.ToXml(mockJudoData.Object));
            ICollection<Combat> liste = Combat.LectureCombats(root);

            // Assert
            Assert.Equal(2, liste.Count);
            Assert.Equal(10, liste.First().numero);
            Assert.Equal(20, liste.Last().numero);
        }
    }
}