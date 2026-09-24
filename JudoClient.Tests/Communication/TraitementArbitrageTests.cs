#nullable enable
using Xunit;
using JudoClient.Communication;

namespace JudoClient.Tests.Communication
{
    public class TraitementArbitrageTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodes_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementArbitrage;

            VerifyPassThroughEvent<TraitementArbitrage.OnListeArbitrageHandler>(h => t.OnListeArbitrage += h, t.ListeArbitrage, client);
            VerifyPassThroughEvent<TraitementArbitrage.OnListeArbitreHandler>(h => t.OnListeArbitres += h, t.ListeArbitres, client);
            VerifyPassThroughEvent<TraitementArbitrage.OnListeCommissairesHandler>(h => t.OnListeCommissaires += h, t.ListeCommissaires, client);
            VerifyPassThroughEvent<TraitementArbitrage.OnListeDeleguesHandler>(h => t.OnListeDelegues += h, t.ListeDelegues, client);

            VerifyPassThroughEvent<TraitementArbitrage.OnUpdateArbitrageHandler>(h => t.OnUpdateArbitrage += h, t.UpdateArbitrage, client);
            VerifyPassThroughEvent<TraitementArbitrage.OnUpdateArbitreHandler>(h => t.OnUpdateArbitres += h, t.UpdateArbitres, client);
            VerifyPassThroughEvent<TraitementArbitrage.OnUpdateCommissairesHandler>(h => t.OnUpdateCommissaires += h, t.UpdateCommissaires, client);
            VerifyPassThroughEvent<TraitementArbitrage.OnUpdateDeleguesHandler>(h => t.OnUpdateDelegues += h, t.UpdateDelegues, client);
        }
    }
}