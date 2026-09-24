#nullable enable
using Xunit;
using JudoClient.Communication;
using FranceJudo.Metier.Network;

namespace JudoClient.Tests.Communication
{
    public class CategoriesSenderTests : SenderTestBase
    {
        [Fact]
        public void ToutesLesMethodes_EnvoientLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandeCategories(), ServerCommandEnum.DemandCategories);
            VerifyCommandSent(c => c.DemandeCateAge(), ServerCommandEnum.DemandCateAge);
            VerifyCommandSent(c => c.DemandeCatePoids(), ServerCommandEnum.DemandCatePoids);
            VerifyCommandSent(c => c.DemandeCeintures(), ServerCommandEnum.DemandGrade);
        }
    }
}