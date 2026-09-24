using KernelImpl.Noyau.Arbitrage;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Arbitrage
{
    public class ArbitrageSnapshotTests
    {
        [Fact]
        public void Constructeur_AvecSourceNulle_NePlantePas()
        {
            // Arrange
            DataArbitrage? source = null;

            // Act
            ArbitrageSnapshot snapshot = new ArbitrageSnapshot(source);

            // Assert
            Assert.Null(snapshot.Arbitres);
            Assert.Null(snapshot.Commissaires);
            Assert.Null(snapshot.Delegues);
        }

        [Fact]
        public void Constructeur_AvecSourceValide_CopieLesReferencesDesListes()
        {
            // Arrange
            DataArbitrage source = new DataArbitrage();

            // On initialise 'nom' et 'prenom' pour éviter les NullReferenceException dans ToXml()
            Arbitre arbitre = new Arbitre { id = 1, nom = "NOM_A", prenom = "PRENOM_A" };
            XElement xmlArbitre = arbitre.ToXml(null);

            Commissaire commissaire = new Commissaire { id = 1, nom = "NOM_C", prenom = "PRENOM_C" };
            XElement xmlCommissaire = commissaire.ToXml(null);

            Delegue delegue = new Delegue { id = 1, nom = "NOM_D", prenom = "PRENOM_D" };
            XElement xmlDelegue = delegue.ToXml(null);

            source.ChargeArbitres(new XElement("Root", xmlArbitre));
            source.ChargeCommissaires(new XElement("Root", xmlCommissaire));
            source.ChargeDelegues(new XElement("Root", xmlDelegue));

            // Act
            ArbitrageSnapshot snapshot = new ArbitrageSnapshot(source);

            // Assert : Le snapshot pointe vers la même instance de liste (Read-Only) que la source au moment T
            Assert.Same(source.Arbitres, snapshot.Arbitres);
            Assert.Same(source.Commissaires, snapshot.Commissaires);
            Assert.Same(source.Delegues, snapshot.Delegues);
        }
    }
}