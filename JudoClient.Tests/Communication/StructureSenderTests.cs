#nullable enable
using Xunit;
using JudoClient.Communication;
using FranceJudo.Metier.Network;

namespace JudoClient.Tests.Communication
{
    public class StructureSenderTests : SenderTestBase
    {
        [Fact]
        public void ToutesLesMethodes_EnvoientLaBonneCommandeXML()
        {
            VerifyCommandSent(c => c.DemandeStructures(), ServerCommandEnum.DemandStructures);
            VerifyCommandSent(c => c.DemandePays(), ServerCommandEnum.DemandPays);
            VerifyCommandSent(c => c.DemandeLigues(), ServerCommandEnum.DemandLigues);
            VerifyCommandSent(c => c.DemandeSecteurs(), ServerCommandEnum.DemandSecteurs);
            VerifyCommandSent(c => c.DemandeComites(), ServerCommandEnum.DemandComites);
            VerifyCommandSent(c => c.DemandeClubs(), ServerCommandEnum.DemandClubs);
        }
    }
}