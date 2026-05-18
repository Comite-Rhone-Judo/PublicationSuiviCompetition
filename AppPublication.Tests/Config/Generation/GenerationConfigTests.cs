using AppPublication.Config.Generation;
using System;
using Xunit;

namespace AppPublication.Tests.Config.Generation
{
    public class GenerationConfigTests
    {
        [Fact]
        public void Constructeur_InitialiseLesProprietes()
        {
            // Arrange & Act
            GenerationConfig config = new GenerationConfig();

            // Assert
            Assert.NotNull(config.GenerateurSite);
            Assert.NotNull(config.GenerateurSiteInterne);
            Assert.NotNull(config.Ecrans);
        }

        [Fact]
        public void GetEcranById_RetourneLeBonEcran_SiTrouve()
        {
            // Arrange
            GenerationConfig config = new GenerationConfig();
            EcranAppelParams ecranCible = new EcranAppelParams
            {
                Id = 88,
                Description = "Cible"
            };
            EcranAppelParams ecranBruit = new EcranAppelParams
            {
                Id = 99,
                Description = "Bruit"
            };

            config.Ecrans.Add(ecranBruit);
            config.Ecrans.Add(ecranCible);

            // Act
            EcranAppelParams resultat = config.GetEcranById(88);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(88, resultat.Id);
            Assert.Equal("Cible", resultat.Description);
        }

        [Fact]
        public void GetEcranById_RetourneNull_SiNonTrouve()
        {
            // Arrange
            GenerationConfig config = new GenerationConfig();
            EcranAppelParams ecranUn = new EcranAppelParams
            {
                Id = 1
            };
            config.Ecrans.Add(ecranUn);

            // Act
            EcranAppelParams resultat = config.GetEcranById(2);

            // Assert
            Assert.Null(resultat);
        }

        [Fact]
        public void InitializeSync_AssigneNotificationAuxEnfants()
        {
            // Arrange
            GenerationConfig config = new GenerationConfig();
            bool notificationRecue = false;
            Action methodeNotification = delegate ()
            {
                notificationRecue = true;
            };

            // Act
            config.InitializeSync(methodeNotification);

            // Assert
            Assert.NotNull(config.OnChanged);
            Assert.NotNull(config.GenerateurSite.OnChanged);
            Assert.NotNull(config.GenerateurSiteInterne.OnChanged);

            // On simule une modification provenant d'un sous-élément pour valider la remontée
            config.GenerateurSite.OnChanged.Invoke();
            Assert.True(notificationRecue);
        }
    }
}