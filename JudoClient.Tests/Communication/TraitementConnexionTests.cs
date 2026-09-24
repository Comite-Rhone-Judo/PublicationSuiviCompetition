#nullable enable
using Xunit;
using JudoClient.Communication;

namespace JudoClient.Tests.Communication
{
    public class TraitementConnexionTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodes_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementConnexion;

            VerifyPassThroughEvent<TraitementConnexion.OnAcceptConnectionPeseeHandler>(h => t.OnAcceptConnectionPesee += h, t.AcceptConnectionPesee, client);
            VerifyPassThroughEvent<TraitementConnexion.OnAcceptConnectionCSHandler>(h => t.OnAcceptConnectionCS += h, t.AcceptConnectionCS, client);
            VerifyPassThroughEvent<TraitementConnexion.OnAcceptConnectionCOMHandler>(h => t.OnAcceptConnectionCOM += h, t.AcceptConnectionCOM, client);
            VerifyPassThroughEvent<TraitementConnexion.OnAcceptConnectionTestHandler>(h => t.OnAcceptConnectionTest += h, t.AcceptConnectionTest, client);
        }
    }
}