#nullable enable
using Xunit;
using JudoClient.Communication;
using FranceJudo.Metier.Network;

namespace JudoClient.Tests.Communication
{
    public class ConnexionSenderTests : SenderTestBase
    {
        [Fact]
        public void ToutesLesMethodes_EnvoientLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandConnectionPesee(), ServerCommandEnum.DemandConnectionPesee);
            VerifyCommandSent(c => c.DemandConnectionCS(), ServerCommandEnum.DemandConnectionCS);
            VerifyCommandSent(c => c.DemandConnectionCOM(), ServerCommandEnum.DemandConnectionCOM);
            VerifyCommandSent(c => c.DemandConnectionTest(), ServerCommandEnum.DemandConnectionTest);
        }
    }
}