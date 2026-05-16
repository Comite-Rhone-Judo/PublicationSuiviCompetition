#nullable enable
using Xunit;
using JudoClient.Communication;
using FranceJudo.Metier.Network;

namespace JudoClient.Tests.Communication
{
    public class ArbitrageSenderTests : SenderTestBase
    {
        [Fact]
        public void ToutesLesMethodes_EnvoientLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandeArbitrage(), ServerCommandEnum.DemandArbitrage);
            VerifyCommandSent(c => c.DemandeArbitres(), ServerCommandEnum.DemandArbitres);
            VerifyCommandSent(c => c.DemandeCommissaires(), ServerCommandEnum.DemandCommissaires);
            VerifyCommandSent(c => c.DemandeDelegues(), ServerCommandEnum.DemandDelegues);
        }
    }
}