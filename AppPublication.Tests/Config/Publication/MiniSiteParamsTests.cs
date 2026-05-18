using AppPublication.Config.Publication;
using Xunit;

namespace AppPublication.Tests.Config.Publication
{
    public class MiniSiteParamsTests
    {
        [Fact]
        public void Proprietes_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            MiniSiteParams parametres = new MiniSiteParams();

            // Assert
            Assert.Equal(string.Empty, parametres.ID);
            Assert.False(parametres.Local);
            Assert.Equal(string.Empty, parametres.FtpLogin);
            Assert.Equal(string.Empty, parametres.FtpSite);
            Assert.False(parametres.FtpModeActif);
            Assert.True(parametres.SyncDiff);
            Assert.Equal(string.Empty, parametres.InterfaceLocalPublication);

            Assert.Equal(8080, parametres.PortMin);
            Assert.Equal(8085, parametres.PortMax);
            Assert.Equal("ServeurHttpBase", parametres.HttpServer);
            Assert.Equal(string.Empty, parametres.HttpModules);
        }

        [Fact]
        public void FtpPassword_SansValeur_RetourneChaineVide()
        {
            // Arrange
            MiniSiteParams parametres = new MiniSiteParams();

            // Act & Assert
            Assert.Equal(string.Empty, parametres.FtpPassword);
        }

        [Fact]
        public void FtpPassword_SetEtGet_AppliqueLeCryptage()
        {
            // Arrange
            MiniSiteParams parametres = new MiniSiteParams();
            string motDePasseClair = "MonMotDePasseSecret123";

            // Act
            parametres.FtpPassword = motDePasseClair;
            string motDePasseRecupere = parametres.FtpPassword;

            // Assert
            // Si la classe Encryption fonctionne correctement en isolation, 
            // le mot de passe doit faire un aller-retour parfait.
            Assert.Equal(motDePasseClair, motDePasseRecupere);
        }

        [Fact]
        public void Setters_ModifientLesValeurs()
        {
            // Arrange
            MiniSiteParams parametres = new MiniSiteParams
            {
                // Act
                ID = "SiteLocal1",
                Local = true,
                PortMin = 9000
            };

            // Assert
            Assert.Equal("SiteLocal1", parametres.ID);
            Assert.True(parametres.Local);
            Assert.Equal(9000, parametres.PortMin);
        }
    }
}