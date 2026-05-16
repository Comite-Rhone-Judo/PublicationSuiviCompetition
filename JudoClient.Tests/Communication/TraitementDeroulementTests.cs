#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using JudoClient.Communication;
using FranceJudo.Metier.XML;

namespace JudoClient.Tests.Communication
{
    public class TraitementDeroulementTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodesDeRelais_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementDeroulement;

            // Utilisation de notre Helper pour tous les relais purs
            VerifyPassThroughEvent<TraitementDeroulement.OnListePhasesHandler>(h => t.OnListePhases += h, t.ListePhases, client);
            VerifyPassThroughEvent<TraitementDeroulement.OnListeCombatsHandler>(h => t.OnListeCombats += h, t.ListeCombats, client);

            VerifyPassThroughEvent<TraitementDeroulement.OnUpdatePhasesHandler>(h => t.OnUpdatePhases += h, t.UpdatePhases, client);
            VerifyPassThroughEvent<TraitementDeroulement.OnUpdateCombatsHandler>(h => t.OnUpdateCombats += h, t.UpdateCombats, client);
            VerifyPassThroughEvent<TraitementDeroulement.OnUpdateTapisCombatsHandler>(h => t.OnUpdateTapisCombats += h, t.UpdateTapisCombats, client);
            VerifyPassThroughEvent<TraitementDeroulement.OnUpdateRencontreReceivedHandler>(h => t.OnUpdateRencontreReceived += h, t.UpdateRencontreReceived, client);
        }

        [Fact]
        public void CombatReceived_XMLValide_ParseLIdentifiantEtDeclencheLEvenement()
        {
            // Arrange
            var client = GetTestClient();
            var t = client.TraitementDeroulement;
            int idAttendu = 42;

            var elementEnvoye = new XElement("Root",
                new XElement(ConstantXML.Combat, idAttendu.ToString())
            );

            bool eventTriggered = false;

            t.OnCombatReceived += (sender, idRecu) =>
            {
                eventTriggered = true;
                sender.Should().BeSameAs(client);
                idRecu.Should().Be(idAttendu, "L'identifiant du combat doit être correctement parsé.");
            };

            // Act
            t.CombatReceived(elementEnvoye);

            // Assert
            eventTriggered.Should().BeTrue();
        }

        [Fact]
        public void RencontreReceived_XMLValide_ParseLIdentifiantEtDeclencheLEvenement()
        {
            // Arrange
            var client = GetTestClient();
            var t = client.TraitementDeroulement;
            int idAttendu = 99;

            var elementEnvoye = new XElement("Root",
                new XElement(ConstantXML.Rencontre, idAttendu.ToString())
            );

            bool eventTriggered = false;

            t.OnRencontreReceived += (sender, idRecu) =>
            {
                eventTriggered = true;
                sender.Should().BeSameAs(client);
                idRecu.Should().Be(idAttendu, "L'identifiant de la rencontre doit être correctement parsé.");
            };

            // Act
            t.RencontreReceived(elementEnvoye);

            // Assert
            eventTriggered.Should().BeTrue();
        }
    }
}