#nullable enable
using AppPublication.Statistiques;
using System.Collections.Generic;
using Xunit;

namespace AppPublication.Tests.Statistiques
{
    public class StatMgrGenerationTests
    {
        [Fact]
        public void EnregistrerGeneration_MetAJourLaDureeEtLeCompteurDerreurs()
        {
            // Arrange
            StatMgrGeneration manager = new StatMgrGeneration();

            // Act
            manager.EnregistrerGeneration(2.5f); // 2.5 secondes
            manager.EnregistrerErreurGeneration();
            manager.EnregistrerErreurGeneration();

            // Assert
            Assert.Equal(2.5f, manager.CompteursGeneration[StatMgrGeneration.CompteurGenerationEnum.TempsGeneration].Max);

            // 2 erreurs enregistrées
            Assert.Equal(2f, manager.CompteursGeneration[StatMgrGeneration.CompteurGenerationEnum.NbErreurGeneration].Valeur);
        }
    }
}