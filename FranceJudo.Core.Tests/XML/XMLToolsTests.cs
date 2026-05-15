using System;
using System.Xml;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.XML;

namespace FranceJudo.Core.Tests.XML
{
    public class XMLToolsTests
    {
        #region LectureInt (XmlAttribute)

        [Theory]
        [InlineData("42", 0, 42)]        // Cas nominal
        [InlineData("-15", 0, -15)]      // Valeur négative
        [InlineData("abc", 99, 99)]      // Mauvais format -> retourne la valeur par défaut
        [InlineData("", 5, 5)]           // Chaine vide -> retourne la valeur par défaut
        [InlineData(null, 10, 10)]       // Attribut null -> retourne la valeur par défaut
        public void LectureInt_XmlAttribute_RetourneValeurAttendue(string? valeurAttr, int defaultVal, int expected)
        {
            // Arrange : L'ancienne API XmlDocument impose de passer par CreateAttribute
            XmlAttribute? attr = null;
            if (valeurAttr != null)
            {
                var doc = new XmlDocument();
                attr = doc.CreateAttribute("node");
                attr.Value = valeurAttr;
            }

            // Act
            int result = XMLTools.LectureInt(attr, defaultVal);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region LectureDate (XmlAttribute)

        [Fact]
        public void LectureDate_XmlAttributeFormatValide_RetourneDate()
        {
            // Arrange
            var doc = new XmlDocument();
            XmlAttribute attr = doc.CreateAttribute("dateNode");
            attr.Value = "2026-05-14";

            string format = "yyyy-MM-dd";
            DateTime expected = new DateTime(2026, 5, 14);

            // Act
            DateTime result = XMLTools.LectureDate(attr, format);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void LectureDate_XmlAttributeNull_RetourneMaintenant()
        {
            // Act
            DateTime result = XMLTools.LectureDate((XmlAttribute?)null, "yyyy-MM-dd");

            // Assert
            result.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
        }

        #endregion

        #region LectureString (XmlAttribute)

        [Theory]
        [InlineData("Judo", "Judo")]
        [InlineData("", "")]
        [InlineData(null, "")] // Doit renvoyer chaine vide et non crasher
        public void LectureString_XmlAttribute_RetourneValeurAttendue(string? valeurAttr, string expected)
        {
            // Arrange
            XmlAttribute? attr = null;
            if (valeurAttr != null)
            {
                var doc = new XmlDocument();
                attr = doc.CreateAttribute("node");
                attr.Value = valeurAttr;
            }

            // Act
            string result = XMLTools.LectureString(attr);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region LectureInt (XAttribute)

        [Theory]
        [InlineData("42", 0, 42)]        // Cas nominal
        [InlineData("-15", 0, -15)]      // Valeur négative
        [InlineData("abc", 99, 99)]      // Mauvais format -> retourne la valeur par défaut
        [InlineData("", 5, 5)]           // Chaine vide -> retourne la valeur par défaut
        [InlineData(null, 10, 10)]       // Attribut null -> retourne la valeur par défaut
        public void LectureInt_XAttribute_RetourneValeurAttendue(string? valeurAttr, int defaultVal, int expected)
        {
            // Arrange
            XAttribute? attr = valeurAttr != null ? new XAttribute("node", valeurAttr) : null;

            // Act
            int result = XMLTools.LectureInt(attr, defaultVal);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region LectureDouble (XAttribute)

        [Theory]
        [InlineData("42.5", 0, 42.5)]    // Culture Invariante attendue (point)
        [InlineData("abc", 1.5, 1.5)]    // Mauvais format
        [InlineData(null, -1.0, -1.0)]   // Attribut null
        public void LectureDouble_XAttribute_RetourneValeurAttendue(string? valeurAttr, double defaultVal, double expected)
        {
            // Arrange
            XAttribute? attr = valeurAttr != null ? new XAttribute("node", valeurAttr) : null;

            // Act
            double result = XMLTools.LectureDouble(attr, defaultVal);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region LectureBool (XAttribute)

        [Theory]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("false", false)]
        [InlineData("False", false)]
        [InlineData("nimportequoi", false)] // Le bool.Parse plantera et le catch retournera false
        [InlineData(null, false)]
        public void LectureBool_XAttribute_RetourneValeurAttendue(string? valeurAttr, bool expected)
        {
            // Arrange
            XAttribute? attr = valeurAttr != null ? new XAttribute("node", valeurAttr) : null;

            // Act
            bool result = XMLTools.LectureBool(attr);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region LectureDate (String)

        [Fact]
        public void LectureDate_StringFormatValide_RetourneDate()
        {
            // Arrange
            string dateStr = "2026-05-14";
            string format = "yyyy-MM-dd";
            DateTime expected = new DateTime(2026, 5, 14);

            // Act
            DateTime result = XMLTools.LectureDate(dateStr, format);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void LectureDate_StringFormatInvalide_RetourneMaintenant()
        {
            // Arrange
            string dateStr = "14/05/2026"; // Incohérent avec le format attendu
            string format = "yyyy-MM-dd";

            // Act
            DateTime result = XMLTools.LectureDate(dateStr, format);

            // Assert
            // Comme la méthode retourne DateTime.Now dans le catch, on vérifie qu'on est très proche de maintenant
            result.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
        }

        #endregion

        #region Conversions XDocument <-> XmlDocument

        [Fact]
        public void ToXmlDocument_DepuisXDocument_ConserveStructure()
        {
            // Arrange
            XDocument xDoc = new XDocument(new XElement("Root", new XElement("Child", "Judo")));

            // Act
            XmlDocument xmlDoc = xDoc.ToXmlDocument();

            // Assert
            xmlDoc.Should().NotBeNull();
            xmlDoc.DocumentElement!.Name.Should().Be("Root");
            xmlDoc.DocumentElement.FirstChild!.Name.Should().Be("Child");
            xmlDoc.DocumentElement.FirstChild.InnerText.Should().Be("Judo");
        }

        #endregion

        [Fact]
        public void LectureDate_CasNominauxEtErreurs_RetourneDateOuDefaut()
        {
            // Arrange
            var dateAttendu = new DateTime(2026, 5, 15);
            var dateDefaut = new DateTime(2000, 1, 1);

            var attrValide = new XAttribute("date", "2026-05-15");
            var attrInvalide = new XAttribute("date", "pas-une-date");

            // Act
            var resValide = XMLTools.LectureDate(attrValide, "yyyy-MM-dd", dateDefaut);
            var resInvalide = XMLTools.LectureDate(attrInvalide, "yyyy-MM-dd", dateDefaut);
            var resNul = XMLTools.LectureDate(null, "yyyy-MM-dd", dateDefaut);

            // Assert
            resValide.Should().Be(dateAttendu, "Une date bien formatée doit être parsée correctement.");
            resInvalide.Should().Be(dateDefaut, "Une date mal formatée doit retourner la valeur par défaut au lieu de planter.");
            resNul.Should().Be(dateDefaut, "Un attribut nul doit retourner la valeur par défaut.");
        }

        [Fact]
        public void LectureTime_CasNominauxEtErreurs_RetourneHeureOuDefaut()
        {
            // Arrange
            var attrValide = new XAttribute("heure", "14:30");
            var attrInvalide = new XAttribute("heure", "99:99");

            // Act
            // CORRECTION 1 : On utilise le format DateTime standard pour le 24h
            var resValide = XMLTools.LectureTime(attrValide, "HH:mm");
            var resInvalide = XMLTools.LectureTime(attrInvalide, "HH:mm");
            var resNul = XMLTools.LectureTime(null, "HH:mm");

            // Assert
            resValide.Should().Be(new TimeSpan(14, 30, 0), "Une heure valide avec le bon format doit être parsée.");

            // CORRECTION 2 : On teste le comportement Legacy "Cible mouvante"
            // On vérifie que la valeur retournée est proche de DateTime.Now à 5 secondes près
            resInvalide.Should().BeCloseTo(DateTime.Now.TimeOfDay, TimeSpan.FromSeconds(5), "Le code historique retourne l'heure actuelle en cas d'échec de conversion.");
            resNul.Should().BeCloseTo(DateTime.Now.TimeOfDay, TimeSpan.FromSeconds(5), "Un attribut nul déclenche aussi le retour de l'heure actuelle.");
        }

        [Fact]
        public void LectureNullableInt_ValeursDiverses_RetourneIntOuNull()
        {
            // Arrange
            var attrValide = new XAttribute("id", "42");
            var attrVide = new XAttribute("id", "");
            var attrInvalide = new XAttribute("id", "abc");

            // Act
            var resValide = XMLTools.LectureNullableInt(attrValide);
            var resVide = XMLTools.LectureNullableInt(attrVide);
            var resInvalide = XMLTools.LectureNullableInt(attrInvalide);
            var resNull = XMLTools.LectureNullableInt(null);

            // Assert
            resValide.Should().Be(42, "Un entier valide doit être converti.");
            resVide.Should().BeNull("Une chaîne vide doit retourner null.");
            resInvalide.Should().BeNull("Des lettres ne peuvent pas être converties et doivent retourner null.");
            resNull.Should().BeNull("Un attribut nul doit retourner null.");
        }

        [Fact]
        public void LectureString_ElementValide_RetourneContenuDeLaChaine()
        {
            // Arrange
            var element = new XElement("categorie", "-81kg");

            // Act
            var resultat = XMLTools.LectureString(element);

            // Assert
            resultat.Should().Be("-81kg", "Un élément XML valide doit renvoyer son contenu textuel.");
        }

        [Fact]
        public void LectureString_ElementNul_NePlantePasEtRetourneValeurParDefaut()
        {
            // Arrange
            System.Xml.Linq.XElement? elementNul = null;

            // Act
            var resultat = XMLTools.LectureString(elementNul);

            // Assert
            // Note : Si ta méthode retourne spécifiquement null, utilise .Should().BeNull()
            // Si elle retourne string.Empty (""), utilise .Should().BeEmpty()
            resultat.Should().BeNullOrEmpty("Un élément nul passé en paramètre ne doit pas lever de NullReferenceException.");
        }

        [Fact]
        public void ToXDocument_XmlDocumentValide_ConvertitSansPerte()
        {
            // Arrange
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<competition><combattant nom=\"Teddy Riner\" /></competition>");

            // Act
            var xDoc = XMLTools.ToXDocument(xmlDoc);

            // Assert
            xDoc.Should().NotBeNull("La conversion doit produire un document.");
            xDoc!.Root.Should().NotBeNull();
            xDoc.Root!.Name.LocalName.Should().Be("competition", "La racine doit être conservée.");
            xDoc.Root.Element("combattant")!.Attribute("nom")!.Value.Should().Be("Teddy Riner", "Les données imbriquées doivent être transférées.");
        }
    }
}