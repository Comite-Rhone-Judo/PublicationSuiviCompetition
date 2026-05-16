#nullable enable
using Xunit;
using JudoClient.Communication;
using FranceJudo.Metier.Network;

namespace JudoClient.Tests.Communication
{
    public class ParticipantsSenderTests : SenderTestBase
    {
        [Fact]
        public void ToutesLesMethodes_EnvoientLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandeEquipes(), ServerCommandEnum.DemandEquipes);
            VerifyCommandSent(c => c.DemandeJudokas(), ServerCommandEnum.DemandJudokas);
            VerifyCommandSent(c => c.DemandeLicencies(), ServerCommandEnum.DemandLicencies);
        }
    }
}