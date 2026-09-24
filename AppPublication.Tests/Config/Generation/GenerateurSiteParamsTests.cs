using AppPublication.Config.Generation;
using Xunit;

namespace AppPublication.Tests.Config.Generation
{
    public class GenerateurSiteParamsTests
    {
        [Fact]
        public void Proprietes_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            GenerateurSiteParams parametres = new GenerateurSiteParams();

            // Assert
            Assert.Equal(30, parametres.DelaiActualisationClientSec);
            Assert.False(parametres.ActualisationClientDefaut);
            Assert.Equal(5, parametres.TailleMaxPouleColonnes);
            Assert.False(parametres.PouleEnColonnes);
            Assert.False(parametres.PouleToujoursEnColonnes);
            Assert.False(parametres.PublierProchainsCombats);
            Assert.Equal(6, parametres.NbProchainsCombats);
            Assert.Equal(string.Empty, parametres.MsgProchainsCombats);
            Assert.True(parametres.PublierAffectationTapis);
            Assert.True(parametres.PublierEngagements);
            Assert.False(parametres.EngagementsAbsents);
            Assert.False(parametres.EngagementsTousCombats);
            Assert.False(parametres.ScoreEngagesGagnantPerdant);
            Assert.True(parametres.AfficherPositionCombat);
        }
    }
}