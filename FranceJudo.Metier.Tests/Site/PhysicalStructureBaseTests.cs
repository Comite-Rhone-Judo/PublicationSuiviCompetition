#nullable enable
using System;
using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Site;

namespace FranceJudo.Metier.Tests.Site
{
    public class PhysicalStructureBaseTests : IDisposable
    {
        private readonly string _tempRootDir;

        // Classe bouchon pour tester la classe abstraite
        private class TestPhysicalStructure : PhysicalStructureBase
        {
            public TestPhysicalStructure(string rootDir, string idCompetition)
                : base(rootDir, idCompetition)
            {
                // On force l'initialisation dans le constructeur de test
                IdCompetition = idCompetition;
            }
        }

        public PhysicalStructureBaseTests()
        {
            // Création d'un dossier temporaire unique pour chaque test
            _tempRootDir = Path.Combine(Path.GetTempPath(), "JudoTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRootDir);
        }

        public void Dispose()
        {
            // Nettoyage du disque après chaque test
            if (Directory.Exists(_tempRootDir))
            {
                try { Directory.Delete(_tempRootDir, true); } catch { /* Ignorer en test */ }
            }
        }

        [Fact]
        public void Initialisation_Incomplete_LeveExceptionSurGuardRail()
        {
            // Arrange
            var structure = new TestPhysicalStructure("", ""); // Pas de racine, pas d'ID

            // Act & Assert
            structure.IsFullyConfigured.Should().BeFalse();

            Action act = () => _ = structure.RepertoireCompetition;
            act.Should().Throw<InvalidOperationException>("La structure n'est pas configurée, le GuardRail doit bloquer l'accès.");
        }

        [Fact]
        public void Initialisation_Complete_CalculeLesRepertoiresEtActiveLaConfiguration()
        {
            // Arrange & Act
            var structure = new TestPhysicalStructure(_tempRootDir, "COMPET_01");

            // Assert
            structure.IsFullyConfigured.Should().BeTrue();
            structure.IdCompetition.Should().Be("COMPET_01");
            structure.RepertoireRacine.Should().Be(_tempRootDir);

            // Le répertoire de la compétition doit exister physiquement
            Directory.Exists(structure.RepertoireCompetition).Should().BeTrue();
        }

        [Fact]
        public void RepertoireCssJsImg_UtilisentLeCacheEtCreentLesDossiers()
        {
            // Arrange
            var structure = new TestPhysicalStructure(_tempRootDir, "COMPET_02");

            // Act
            string repCss1 = structure.RepertoireCss();
            string repCss2 = structure.RepertoireCss(); // Appel pour déclencher le cache
            string repJs = structure.RepertoireJs();
            string repImg = structure.RepertoireImg();

            // Assert
            repCss1.Should().Be(repCss2, "Le ConcurrentDictionary doit renvoyer exactement la même chaîne mise en cache.");

            Directory.Exists(repCss1).Should().BeTrue("Le dossier CSS doit avoir été créé sur le disque.");
            Directory.Exists(repJs).Should().BeTrue("Le dossier JS doit avoir été créé.");
            Directory.Exists(repImg).Should().BeTrue("Le dossier IMG doit avoir été créé.");
        }

        [Fact]
        public void EffacerRepertoireCompetition_NettoieEtRecreeLeDossier()
        {
            // Arrange
            var structure = new TestPhysicalStructure(_tempRootDir, "COMPET_03");
            string rootCompet = structure.RepertoireCompetition;

            // On simule des fichiers générés à l'intérieur
            string dummyFile = Path.Combine(rootCompet, "test.txt");
            File.WriteAllText(dummyFile, "data");

            // Act
            bool result = structure.EffacerRepertoireCompetition();

            // Assert
            result.Should().BeTrue();
            Directory.Exists(rootCompet).Should().BeTrue("La méthode doit recréer le dossier racine immédiatement après l'avoir purgé.");
            File.Exists(dummyFile).Should().BeFalse("Le contenu du dossier doit avoir été totalement purgé.");
        }
    }
}