#nullable enable
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using JudoClient.Communication;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;

namespace JudoClient.Tests.Communication
{
    public class DeroulementSenderTests : SenderTestBase
    {
        [Fact]
        public void MethodesDeDemande_EnvoientLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandePhases(), ServerCommandEnum.DemandPhases);
            VerifyCommandSent(c => c.DemandeCombats(), ServerCommandEnum.DemandCombats);
        }

        [Fact]
        public void SendChoixTapis_InjecteLeNumeroDuTapis()
        {
            int numeroTapis = 42;

            VerifyCommandSent(
                c => c.SendChoixTapis(numeroTapis),
                ServerCommandEnum.ResultTapis,
                payload => payload.Should().Contain($"<{ConstantXML.Tapis}>{numeroTapis}</{ConstantXML.Tapis}>")
            );
        }

        [Fact]
        public void MethodesAvecXElementSimple_InjectentLArbreXML()
        {
            var fakeElement = new XElement("NodeData", "1234");

            VerifyCommandSent(c => c.SendResultCombat(fakeElement), ServerCommandEnum.ResultCombats, p => p.Should().Contain("<NodeData>1234</NodeData>"));
            VerifyCommandSent(c => c.SendResultRencontre(fakeElement), ServerCommandEnum.ResultRencontres, p => p.Should().Contain("<NodeData>1234</NodeData>"));
            VerifyCommandSent(c => c.SendCategoriePoidsTireeAuSort(fakeElement), ServerCommandEnum.CategoriePoidsTireeAuSort, p => p.Should().Contain("<NodeData>1234</NodeData>"));
            VerifyCommandSent(c => c.SendChallenge(fakeElement), ServerCommandEnum.ChallengeRefuse, p => p.Should().Contain("<NodeData>1234</NodeData>"));
        }

        [Fact]
        public void MethodesAvecListesXElement_InjectentTousLesElements()
        {
            var listElements = new List<XElement>
            {
                new XElement("Item1", "A"),
                new XElement("Item2", "B")
            };

            // On vérifie que les listes sont bien parcourues et insérées
            VerifyCommandSent(c => c.SendUpdateRencontres(listElements), ServerCommandEnum.UpdateRencontres, p => p.Should().Contain("<Item1>A</Item1>").And.Contain("<Item2>B</Item2>"));
            VerifyCommandSent(c => c.SendResultsCombats(listElements), ServerCommandEnum.ResultCombats, p => p.Should().Contain("<Item1>A</Item1>").And.Contain("<Item2>B</Item2>"));
            VerifyCommandSent(c => c.SendResultsRencontres(listElements), ServerCommandEnum.ResultRencontres, p => p.Should().Contain("<Item1>A</Item1>").And.Contain("<Item2>B</Item2>"));
        }
    }
}