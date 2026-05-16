#nullable enable
using Xunit;
using JudoClient.Communication;

namespace JudoClient.Tests.Communication
{
    public class TraitementParticipantsTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodes_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementParticipants;

            // Suite à ta correction, toutes les méthodes renvoient désormais `_client` 
            VerifyPassThroughEvent<TraitementParticipants.OnListeJudokasHandler>(h => t.OnListeJudokas += h, t.ListeJudokas, client);
            VerifyPassThroughEvent<TraitementParticipants.OnListeEquipesHandler>(h => t.OnListeEquipes += h, t.ListeEquipes, client);
            VerifyPassThroughEvent<TraitementParticipants.OnListeLicenciesHandler>(h => t.OnListeLicencies += h, t.ListeLicencies, client);

            VerifyPassThroughEvent<TraitementParticipants.OnUpdateJudokasHandler>(h => t.OnUpdateJudokas += h, t.UpdateJudokas, client);
            VerifyPassThroughEvent<TraitementParticipants.OnUpdateEquipesHandler>(h => t.OnUpdateEquipes += h, t.UpdateEquipes, client);
        }
    }
}