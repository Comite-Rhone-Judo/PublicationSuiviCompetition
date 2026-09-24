#nullable enable
using Xunit;
using JudoClient.Communication;
using FranceJudo.Metier.Network;

namespace JudoClient.Tests.Communication
{
    public class LogosSenderTests : SenderTestBase
    {
        [Fact]
        public void DemandeLogos_EnvoieLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandeLogos(), ServerCommandEnum.DemandLogos);
        }
    }
}