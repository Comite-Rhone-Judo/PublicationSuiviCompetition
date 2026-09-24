#nullable enable
using Xunit;
using JudoClient.Communication;

namespace JudoClient.Tests.Communication
{
    public class TraitementStructureTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodes_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementStructure;

            VerifyPassThroughEvent<TraitementStructure.OnListeStructuresHandler>(h => t.OnListeStructures += h, t.ListeStructures, client);
            VerifyPassThroughEvent<TraitementStructure.OnListePaysHandler>(h => t.OnListePays += h, t.ListePays, client);
            VerifyPassThroughEvent<TraitementStructure.OnListeLiguesHandler>(h => t.OnListeLigues += h, t.ListeLigues, client);
            VerifyPassThroughEvent<TraitementStructure.OnListeClubsHandler>(h => t.OnListeClubs += h, t.ListeClubs, client);
            VerifyPassThroughEvent<TraitementStructure.OnListeComitesHandler>(h => t.OnListeComites += h, t.ListeComites, client);

            VerifyPassThroughEvent<TraitementStructure.OnUpdateStructuresHandler>(h => t.OnUpdateStructures += h, t.UpdateStructures, client);
            VerifyPassThroughEvent<TraitementStructure.OnUpdatePaysHandler>(h => t.OnUpdatePays += h, t.UpdatePays, client);
            VerifyPassThroughEvent<TraitementStructure.OnUpdateLiguesHandler>(h => t.OnUpdateLigues += h, t.UpdateLigues, client);
            VerifyPassThroughEvent<TraitementStructure.OnUpdateSecteursHandler>(h => t.OnUpdateSecteurs += h, t.UpdateSecteurs, client);
            VerifyPassThroughEvent<TraitementStructure.OnUpdateClubsHandler>(h => t.OnUpdateClubs += h, t.UpdateClubs, client);
            VerifyPassThroughEvent<TraitementStructure.OnUpdateComitesHandler>(h => t.OnUpdateComites += h, t.UpdateComites, client);
        }
    }
}