#nullable enable
using AppPublication.Generation;
using AppPublication.Statistiques;
using Xunit;

namespace AppPublication.Tests.Statistiques
{
    public class StatMgrSynchronisationTests
    {
        [Fact]
        public void EnregistrerSynchronisation_TypeComplete_MetAJourLeBonDictionnaire()
        {
            // Arrange
            StatMgrSynchronisation manager = new StatMgrSynchronisation();

            // Création d'un ResultatOperation (Etape, isSuccess, isComplete, nbElements)
            ResultatOperation syncComplete = new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteSynchronisation, true, true, 42);

            // Act
            // Correction de l'ordre des paramètres : (float duree, ResultatOperation syncStatus)
            manager.EnregistrerSynchronisation(5.0f, syncComplete);

            // Assert
            // Vérification dans le dictionnaire "Complete"
            Assert.Equal(5.0f, manager.CompteursSynchronisationComplete[StatMgrSynchronisation.CompteurSynchronisationEnum.TempsSynchronisation].Max);
            Assert.Equal(42f, manager.CompteursSynchronisationComplete[StatMgrSynchronisation.CompteurSynchronisationEnum.NbFichierSynchronisation].Max);

            // Vérification que le dictionnaire "Difference" n'a pas été touché (Valeur null)
            Assert.Null(manager.CompteursSynchronisationDifference[StatMgrSynchronisation.CompteurSynchronisationEnum.NbFichierSynchronisation].Valeur);
        }

        [Fact]
        public void EnregistrerSynchronisation_AvecErreur_IncrementeLeCompteurDerreur()
        {
            // Arrange
            StatMgrSynchronisation manager = new StatMgrSynchronisation();
            // isSuccess = false, isComplete = false (donc va dans le diff)
            ResultatOperation syncErreur = new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteSynchronisation, false, false, 0);

            // Act
            // Correction de l'ordre des paramètres : (float duree, ResultatOperation syncStatus)
            manager.EnregistrerSynchronisation(1.0f, syncErreur);

            // Assert
            Assert.Equal(1f, manager.CompteursSynchronisationDifference[StatMgrSynchronisation.CompteurSynchronisationEnum.NbErreurSynchronisation].Valeur);
        }
    }
}