#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using JudoClient.Communication;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;

namespace JudoClient.Tests.Communication
{
    public class CommonTests
    {
        [Fact]
        public void CreateDocument_GenereLArborescenceXML_Correcte()
        {
            // Act
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandArbitrage);

            // Assert
            doc.Should().NotBeNull();

            var root = doc.Element(ConstantXML.ServerJudo);
            root.Should().NotBeNull("Le noeud racine ServerJudo doit exister.");

            root!.Element(ConstantXML.Command)?.Value.Should().Be(((int)ServerCommandEnum.DemandArbitrage).ToString(), "Le code de commande doit être correctement converti en entier.");

            root.Element(ConstantXML.Valeur).Should().NotBeNull("Le noeud Valeur doit toujours être présent, même s'il est vide.");
        }
    }
}