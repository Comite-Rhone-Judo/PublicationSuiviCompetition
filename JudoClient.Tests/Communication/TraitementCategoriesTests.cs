#nullable enable
using Xunit;
using JudoClient.Communication;

namespace JudoClient.Tests.Communication
{
    public class TraitementCategoriesTests : TraitementTestBase
    {
        [Fact]
        public void ToutesLesMethodes_RelaientLesEvenementsCorrectement()
        {
            var client = GetTestClient();
            var t = client.TraitementCategories;

            VerifyPassThroughEvent<TraitementCategories.OnListeCategoriesHandler>(h => t.OnListeCategories += h, t.ListeCategories, client);
            VerifyPassThroughEvent<TraitementCategories.OnListeCateAgeHandler>(h => t.OnListeCateAge += h, t.ListeCateAge, client);
            VerifyPassThroughEvent<TraitementCategories.OnListeCatePoidsHandler>(h => t.OnListeCatePoids += h, t.ListeCatePoids, client);
            VerifyPassThroughEvent<TraitementCategories.OnListeCeinturesHandler>(h => t.OnListeCeintures += h, t.ListeCeintures, client);

            VerifyPassThroughEvent<TraitementCategories.OnUpdateCategoriesHandler>(h => t.OnUpdateCategories += h, t.UpdateCategories, client);
            VerifyPassThroughEvent<TraitementCategories.OnUpdateCateAgeHandler>(h => t.OnUpdateCateAge += h, t.UpdateCateAge, client);
            VerifyPassThroughEvent<TraitementCategories.OnUpdateCatePoidsHandler>(h => t.OnUpdateCatePoids += h, t.UpdateCatePoids, client);
            VerifyPassThroughEvent<TraitementCategories.OnUpdateCeinturesHandler>(h => t.OnUpdateCeintures += h, t.UpdateCeintures, client);
        }
    }
}