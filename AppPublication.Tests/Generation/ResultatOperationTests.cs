#nullable enable
using AppPublication.Generation;
using Xunit;

namespace AppPublication.Tests.Generation
{
    public class ResultatOperationTests
    {
        [Fact]
        public void OperationProgress_Constructeur_AssigneLesValeurs()
        {
            // Act
            OperationProgress progression = new OperationProgress(EtapeGenerateurSiteEnum.PrepareGeneration, 75.5f);

            // Assert
            Assert.Equal(EtapeGenerateurSiteEnum.PrepareGeneration, progression.Etape);
            Assert.Equal(75.5f, progression.ProgressPercent);
        }

        [Fact]
        public void ResultatOperation_ConstructeurComplet_ForceIsActiveATrue()
        {
            // Act
            // Le 3ème paramètre est "isComplete"
            ResultatOperation resultat = new ResultatOperation(EtapeGenerateurSiteEnum.ExecuteGeneration, false, false, 42);

            // Assert
            Assert.Equal(EtapeGenerateurSiteEnum.ExecuteGeneration, resultat.Etape);
            Assert.False(resultat.IsSuccess); // Valeur passée
            Assert.False(resultat.IsComplete); // Valeur passée
            Assert.Equal(42, resultat.NbElements);

            // VÉRIFICATION CRITIQUE : Dans votre code métier, IsActive est forcé à 'true' de manière native dans ce constructeur
            Assert.True(resultat.IsActive);
        }

        [Fact]
        public void ResultatOperation_ConstructeurSimplifie_ForceValeursParDefaut()
        {
            // Act
            ResultatOperation resultat = new ResultatOperation(EtapeGenerateurSiteEnum.CleanupInitial, false);

            // Assert
            Assert.Equal(EtapeGenerateurSiteEnum.CleanupInitial, resultat.Etape);
            Assert.False(resultat.IsActive); // Valeur passée

            // VÉRIFICATION CRITIQUE : Comportements forcés par le constructeur simplifié
            Assert.True(resultat.IsSuccess);
            Assert.True(resultat.IsComplete);
            Assert.Equal(-1, resultat.NbElements);
        }
    }
}