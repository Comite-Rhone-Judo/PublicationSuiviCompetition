#nullable enable
using Xunit;
using JudoClient.Communication;

namespace JudoClient.Tests.Communication
{
    public class TraitementLogosTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodes_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementLogos;

            VerifyPassThroughEvent<TraitementLogos.OnListeLogosHandler>(h => t.OnListeLogos += h, t.ListeLogos, client);
            VerifyPassThroughEvent<TraitementLogos.OnUpdateLogosHandler>(h => t.OnUpdateLogos += h, t.UpdateLogos, client);
        }
    }
}