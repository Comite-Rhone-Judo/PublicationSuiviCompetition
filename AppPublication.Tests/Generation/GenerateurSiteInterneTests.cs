#nullable enable
using AppPublication.Generation;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AppPublication.Tests.Generation
{
    public class GenerateurSiteInterneTests
    {
        [Fact]
        public void Constructeur_AvecDataManagerNull_LeveArgumentNullException()
        {
            // Arrange
            // IDE0039 : Fonction locale pour encapsuler la tentative d'instanciation
            void ActionConstructeur()
            {
                // On passe intentionnellement null au premier paramètre (IJudoDataManager)
                // pour vérifier que la classe se protège bien.
                _ = new GenerateurSiteInterne(null!, null!, null!);
            }

            // Act & Assert
            // On s'assure que le constructeur rejette bien l'instanciation
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(ActionConstructeur);

            // Si votre code utilise nameof(dataManager), on peut même vérifier que la bonne erreur est levée
            Assert.Equal("dataManager", exception.ParamName);
        }

        [Fact]
        public async Task ExecuteSynchronisation_RetourneResultatImmediatEtFaux()
        {
            // Arrange
            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();

            // On fournit le Mock valide pour passer la barrière de sécurité du constructeur
            // Les autres paramètres peuvent rester null s'ils ne sont pas protégés de la même manière.
            GenerateurSiteInterne generateur = new GenerateurSiteInterne(mockDataManager.Object, null!, null!);

            // Act
            ResultatOperation resultat = await generateur.ExecuteSynchronisation();

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(EtapeGenerateurSiteEnum.ExecuteSynchronisation, resultat.Etape);
            // La synchronisation n'est pas active sur le site interne
            Assert.False(resultat.IsActive);
        }
    }
}