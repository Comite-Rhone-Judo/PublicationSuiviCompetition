using Xunit;
using FranceJudo.Metier.Noyau.Organisation;
using AppPublication.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.XML;

namespace AppPublication.Tests.ExtensionNoyau.StatistiquesCombats
{
    public class GroupeStatistiquesTests
    {
        [Fact]
        public void Equals_MemesValeurs_RetourneTrue()
        {
            var cle1 = new GroupeStatistiques(123, new EpreuveSexe("M"), "CLUB1", EchelonEnum.Club);
            var cle2 = new GroupeStatistiques(123, new EpreuveSexe("M"), "CLUB1", EchelonEnum.Club);

            Assert.True(cle1.Equals(cle2));
            Assert.Equal(cle1.GetHashCode(), cle2.GetHashCode());
        }

        [Fact]
        public void Equals_ValeursDifferentes_RetourneFalse()
        {
            var cleBase = new GroupeStatistiques(123, new EpreuveSexe("M"), "CLUB1", EchelonEnum.Club);
            var cleDiffSexe = new GroupeStatistiques(123, new EpreuveSexe("F"), "CLUB1", EchelonEnum.Club);
            var cleDiffId = new GroupeStatistiques(123, new EpreuveSexe("M"), "CLUB99", EchelonEnum.Club);
            var cleDiffType = new GroupeStatistiques(123, new EpreuveSexe("M"), "CLUB1", EchelonEnum.Aucun);

            Assert.False(cleBase.Equals(cleDiffSexe));
            Assert.False(cleBase.Equals(cleDiffId));
            Assert.False(cleBase.Equals(cleDiffType));
        }

        [Fact]
        public void ProprieteId_GenereChaineBienFormatee()
        {
            // Arrange
            var groupe = new GroupeStatistiques(456, new EpreuveSexe("F"), "LIGUE5", EchelonEnum.Ligue);

            // Act
            string expectedId = $"456-F-LIGUE5-{(int)EchelonEnum.Ligue}";

            // Assert
            Assert.Equal(expectedId, groupe.Id);
        }

        [Fact]
        public void ToXml_GenereElementAvecAttributsCorrects()
        {
            // Arrange
            var groupe = new GroupeStatistiques(789, new EpreuveSexe("M"), "COMITE69", EchelonEnum.Departement);

            // Act
            var xml = groupe.ToXml();

            // Assert
            Assert.Equal(ConstantXML.GroupeStatistiques_groupe, xml.Name.LocalName);
            Assert.Equal("789", xml.Attribute(ConstantXML.GroupeStatistiques_Competition)?.Value);
            Assert.Equal("M", xml.Attribute(ConstantXML.GroupeStatistiques_Sexe)?.Value);
            Assert.Equal("COMITE69", xml.Attribute(ConstantXML.GroupeStatistiques_Entite)?.Value);
            Assert.Equal(((int)EchelonEnum.Departement).ToString(), xml.Attribute(ConstantXML.GroupeStatistiques_Type)?.Value);
        }
    }
}