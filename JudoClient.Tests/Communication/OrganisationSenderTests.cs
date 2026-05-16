#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using JudoClient.Communication;
using FranceJudo.Metier.Network;

namespace JudoClient.Tests.Communication
{
    public class OrganisationSenderTests : SenderTestBase
    {
        [Fact]
        public void MethodesDeDemande_EnvoientLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandeOrganisation(), ServerCommandEnum.DemandOrganisation);
            VerifyCommandSent(c => c.DemandeCompetitions(), ServerCommandEnum.DemandCompetitions);
            VerifyCommandSent(c => c.DemandeEpreuves(), ServerCommandEnum.DemandEpreuves);
            VerifyCommandSent(c => c.DemandeTapis(), ServerCommandEnum.DemandTapis);
        }

        [Fact]
        public void SendResultInscrition_InjecteLeXElement_DansLaValeur()
        {
            var elementInscript = new XElement("TestInscription", "ValeurDeTest");

            VerifyCommandSent(
                c => c.SendResultInscrition(elementInscript),
                ServerCommandEnum.ResultInscription,
                payload => payload.Should().Contain("<TestInscription>ValeurDeTest</TestInscription>", "L'élément XML fourni doit être imbriqué dans la trame.")
            );
        }
    }
}