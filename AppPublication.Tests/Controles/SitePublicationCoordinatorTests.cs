#nullable enable
using AppPublication.Controles;
using AppPublication.Models.Statistiques;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau;
using Moq;
using System;
using Xunit;

namespace AppPublication.Tests.Controles
{
    public class SitePublicationCoordinatorTests
    {
        [Fact]
        public void Constructeur_SansRepertoiresDeBase_LeveExceptionEnveloppee()
        {
            // Arrange
            Mock<IJudoDataManager> mockData = new Mock<IJudoDataManager>();
            GestionStatistiques statistiques = new GestionStatistiques();

            // IDE0039 : Utilisation d'une fonction locale pour isoler l'instanciation
            void ActionInstanciation()
            {
                // CS8600 / CS8602 : L'utilisation du discard '_' signale au compilateur 
                // que l'on sait que l'objet ne sera pas utilisé, évitant les avertissements.
                _ = new SitePublicationCoordinator(mockData.Object, statistiques);
            }

            // Act & Assert
            // On vérifie que le bloc try/catch du constructeur fait bien son travail de blindage
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(ActionInstanciation);

            // On s'assure que c'est bien l'erreur métier prévue et non un autre crash imprévu
            Assert.Equal("Erreur lors de l'initialisation du Controleur", exception.Message);
        }
    }
}