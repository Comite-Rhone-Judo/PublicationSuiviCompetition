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
    }
}