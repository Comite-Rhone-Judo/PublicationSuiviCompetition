#nullable enable
using AppPublication.Generation;
using AppPublication.Statistiques;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AppPublication.Tests.Generation
{
    public class GenerationSchedulerTests
    {
        [Fact]
        public async Task CycleDeVie_DemarrerEtArreter_SeTermineProprementSansException()
        {
            // Arrange
            Mock<IGenerateurSite> mockGenerateur = new Mock<IGenerateurSite>();

            // On configure le Mock pour répondre avec succès et ne pas bloquer le Scheduler
            ResultatOperation resultatRapide = new ResultatOperation(EtapeGenerateurSiteEnum.PrepareGeneration, true);
            mockGenerateur.Setup(g => g.PrepareGeneration()).Returns(resultatRapide);
            mockGenerateur.Setup(g => g.ExecuteGeneration()).ReturnsAsync(resultatRapide);
            mockGenerateur.Setup(g => g.ExecuteSynchronisation()).ReturnsAsync(resultatRapide);

            // Instanciation stricte avec les 3 paramètres. 
            // Les gestionnaires de stats peuvent être null (protégés par '?.' dans votre code).
            GenerationScheduler scheduler = new GenerationScheduler(null!, null!, mockGenerateur.Object);

            Exception? exceptionDetectee = null;

            // Act
            try
            {
                // 1. On lance le thread de génération (ne prend plus d'arguments !)
                await scheduler.StartGeneration();

                // 2. On laisse le thread s'exécuter un court instant pour entrer dans la boucle while
                await Task.Delay(150, TestContext.Current.CancellationToken);

                // 3. On demande l'arrêt complet (déclenche CancellationToken.Cancel())
                await scheduler.StopGeneration();
            }
            catch (Exception ex)
            {
                exceptionDetectee = ex;
            }

            // Assert
            // On vérifie que la boucle infinie a bien intercepté l'OperationCanceledException 
            // et n'a pas fait fuiter de crash global.
            Assert.Null(exceptionDetectee);

            // On vérifie que la méthode StopGeneration() a bien remis le système à l'arrêt
            Assert.Equal(StateGenerationEnum.Stopped, scheduler.State);
        }

        [Fact]
        public void Constructeur_AvecGenerateurNull_LeveArgumentNullException()
        {
            // Arrange
            void ActionConstructeur()
            {
                // On tente d'instancier sans le IGenerateurSite obligatoire
                _ = new GenerationScheduler(null!, null!, null!);
            }

            // Act & Assert
            // Vérifie que la protection "if (generateur == null) throw new ArgumentNullException();" fonctionne
            Assert.Throws<ArgumentNullException>(ActionConstructeur);
        }
    }
}