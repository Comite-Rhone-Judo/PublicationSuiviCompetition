#nullable enable
using Xunit;
using JudoClient.Communication;

namespace JudoClient.Tests.Communication
{
    public class TraitementOrganisationTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodes_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementOrganisation;

            VerifyPassThroughEvent<TraitementOrganisation.OnListeOrganisationHandler>(h => t.OnListeOrganisation += h, t.ListeOrganisation, client);
            VerifyPassThroughEvent<TraitementOrganisation.OnListeCompetitionsHandler>(h => t.OnListeCompetitions += h, t.ListeCompetitions, client);
            VerifyPassThroughEvent<TraitementOrganisation.OnListeEpreuvesHandler>(h => t.OnListeEpreuves += h, t.ListeEpreuves, client);
            VerifyPassThroughEvent<TraitementOrganisation.OnListeTapisHandler>(h => t.OnListeTapis += h, t.ListeTapis, client);

            VerifyPassThroughEvent<TraitementOrganisation.OnUpdateOrganisationHandler>(h => t.OnUpdateOrganisation += h, t.UpdateOrganisation, client);
            VerifyPassThroughEvent<TraitementOrganisation.OnUpdateCompetitionsHandler>(h => t.OnUpdateCompetitions += h, t.UpdateCompetitions, client);
            VerifyPassThroughEvent<TraitementOrganisation.OnUpdateEpreuvesHandler>(h => t.OnUpdateEpreuves += h, t.UpdateEpreuves, client);
            VerifyPassThroughEvent<TraitementOrganisation.OnUpdateTapisHandler>(h => t.OnUpdateTapis += h, t.UpdateTapis, client);
        }
    }
}