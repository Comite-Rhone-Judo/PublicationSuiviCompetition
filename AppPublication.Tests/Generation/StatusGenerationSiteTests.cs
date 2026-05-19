#nullable enable
using AppPublication.Generation;
using Xunit;

namespace AppPublication.Tests.Generation
{
    public class StatusGenerationSiteTests
    {
        [Fact]
        public void ConstructeurDefaut_InitialiseAStopped()
        {
            // Arrange & Act
            StatusGenerationSite status = new StatusGenerationSite();

            // Assert
            Assert.Equal(StateGenerationEnum.Stopped, status.State);
            // La mise à jour du message se fait via le setter de State, 
            // donc à l'initialisation sans paramètre, il n'est pas mis à jour selon le switch
        }

        [Fact]
        public void State_Set_MetAJourLeMessage_SelonEnum()
        {
            // Arrange
            StatusGenerationSite status = new StatusGenerationSite
            {
                // Act & Assert
                State = StateGenerationEnum.Generating
            };
            Assert.Equal("Génération du site ...", status.Message);

            status.State = StateGenerationEnum.Cleaning;
            Assert.Equal("Nettoyage du site ...", status.Message);

            status.State = StateGenerationEnum.Starting;
            Assert.Equal("Démarrage ...", status.Message);

            // Test du cas d'attente (Idle) sans délai défini (-1 par défaut)
            status.State = StateGenerationEnum.Idle;
            Assert.Equal("En attente ...", status.Message);
        }

        [Fact]
        public void NextGenerationSec_Set_MetAJourLeMessageDAttente()
        {
            // Arrange
            StatusGenerationSite status = new StatusGenerationSite(StateGenerationEnum.Idle)
            {
                // Act
                NextGenerationSec = 45 // Déclenche NotifyPropertyChanged et UpdateMessage()
            };

            // Assert
            Assert.Equal("En attente (45 sec.) ...", status.Message);
        }

        [Fact]
        public void Instance_Factory_RetourneNouvelObjet()
        {
            // Act
            StatusGenerationSite status = StatusGenerationSite.Instance(StateGenerationEnum.Syncing);

            // Assert
            Assert.NotNull(status);
            Assert.Equal(StateGenerationEnum.Syncing, status.State);
        }
    }
}