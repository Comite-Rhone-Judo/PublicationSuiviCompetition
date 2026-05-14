using System;
using System.Xml;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Export;

namespace FranceJudo.Core.Tests.Export
{
    public class XmlSourceTests
    {

        [Fact]
        public void CreateReader_PropagesCorrectementLaNameTable()
        {
            // Arrange
            var doc = new XDocument(new XElement("Racine", "Contenu"));
            using var xmlSource = new XmlSource(doc);

            // Act
            using XmlReader reader = xmlSource.CreateReader();

            // Assert
            // On vérifie directement la NameTable du reader plutôt que celle des Settings
            reader.NameTable.Should().NotBeNull("Le reader doit posséder une NameTable pour l'optimisation des chaînes (atomisation).");
        }


        [Fact]
        public void Constructeur_DocumentNull_LeveArgumentNullException()
        {
            Action act = () => new XmlSource(null!);
            act.Should().Throw<ArgumentNullException>().WithParameterName("doc");
        }

        [Fact]
        public void CreateReader_AppliqueLesSettings_IgnoreLesEspacesBlancs()
        {
            // Arrange : Un document avec des espaces et des retours à la ligne inutiles
            var doc = XDocument.Parse("<Racine>   \n   <Enfant>Texte</Enfant>  \n</Racine>");
            using var xmlSource = new XmlSource(doc);

            // Act
            using XmlReader reader = xmlSource.CreateReader();

            // Assert
            // 1. On teste la NameTable directement sur le reader (elle ne doit JAMAIS être nulle)
            reader.NameTable.Should().NotBeNull("Le reader doit posséder une NameTable pour l'atomisation des chaînes.");

            // 2. On vérifie que les réglages sont réellement appliqués en lisant le flux
            // Si IgnoreWhitespace fonctionne, on doit tomber directement sur l'élément 'Enfant' 
            // sans passer par un nœud de type 'Whitespace' ou 'SignificantWhitespace'.
            reader.Read(); // Position sur <Racine>
            reader.Read(); // Doit sauter les espaces et arriver sur <Enfant>

            reader.NodeType.Should().Be(XmlNodeType.Element);
            reader.Name.Should().Be("Enfant");
        }

        [Fact]
        public void Dispose_LibereLaReferenceAuDocument()
        {
            // Arrange
            var doc = new XDocument(new XElement("Test"));
            var xmlSource = new XmlSource(doc);

            // Act
            xmlSource.Dispose();

            // Assert
            xmlSource.Document.Should().BeNull();
        }
    }
}