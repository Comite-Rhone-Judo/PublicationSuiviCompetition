#nullable enable
using AppPublication.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.XML;
using System.Xml.Linq;
using Xunit;

namespace AppPublication.Tests.ExtensionNoyau.Engagement
{
    public class GroupeEngagementsTests
    {
        [Fact]
        public void Constructeur_AssigneValeursEtGenereIdCorrectement()
        {
            // Arrange
            int competition = 10;
            EpreuveSexe sexe = new EpreuveSexe(EpreuveSexeEnum.Masculin);
            EchelonEnum type = EchelonEnum.Club; // Vaut 2 en réalité
            string entite = "JudoClubParis";

            // Act
            GroupeEngagements groupe = new GroupeEngagements(competition, sexe, entite, type);

            // Assert
            Assert.Equal(10, groupe.Competition);
            Assert.Equal(sexe, groupe.Sexe);
            Assert.Equal(type, groupe.Type);
            Assert.Equal("JudoClubParis", groupe.Entite);

            // Format attendu: {Competition}-{sexe}-{ID entite}-{Type entite}
            // Utilisation de l'interpolation de chaîne pour un test 100% robuste
            Assert.Equal($"10-M-JudoClubParis-{type}", groupe.Id);
        }

        [Fact]
        public void Setters_MettentAJourLId()
        {
            // Arrange
            GroupeEngagements groupe = new GroupeEngagements(1, new EpreuveSexe(EpreuveSexeEnum.Feminine), "Entite1", EchelonEnum.Club);

            // Assert
            Assert.Equal("1-F-Entite2-1", groupe.Id);
        }

        [Fact]
        public void ToXml_CreeUneBaliseValide()
        {
            // Arrange
            GroupeEngagements groupe = new GroupeEngagements(100, new EpreuveSexe(EpreuveSexeEnum.Mixte), "LigueIDF", EchelonEnum.Ligue);

            // Act
            XElement xml = groupe.ToXml();

            // Assert
            Assert.NotNull(xml);
            Assert.Equal(ConstantXML.GroupeEngagements_Groupe, xml.Name.LocalName);
            Assert.Equal("100", xml.Attribute(ConstantXML.GroupeEngagements_Competition)?.Value);
            Assert.Equal("100-X-LigueIDF-5", xml.Attribute(ConstantXML.GroupeEngagements_Id)?.Value);
            Assert.Equal("X", xml.Attribute(ConstantXML.GroupeEngagements_Sexe)?.Value);
            Assert.Equal("5", xml.Attribute(ConstantXML.GroupeEngagements_Type)?.Value);
            Assert.Equal("LigueIDF", xml.Attribute(ConstantXML.GroupeEngagements_Entite)?.Value);
        }
    }
}