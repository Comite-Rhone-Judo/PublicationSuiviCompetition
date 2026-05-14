#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Configuration;

namespace FranceJudo.Core.Tests.Configuration
{
    [Collection("ConfigurationSequential")]
    public class ConfigurationServiceTests : IDisposable
    {
        public ConfigurationServiceTests()
        {
            // On s'assure qu'aucun ancien service n'est en mémoire avant le test
            var field = typeof(ConfigurationService).GetField("_instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            field?.SetValue(null, null);
        }

        public void Dispose()
        {
            if (ConfigurationService.Instance != null)
            {
                ConfigurationService.Instance.Dispose();
            }
        }

        [Fact]
        public void CreateInstance_ImplementeLeSingletonCorrectement()
        {
            // Act
            var instance = ConfigurationService.CreateInstance();

            // Assert
            instance.Should().NotBeNull();
            ConfigurationService.Instance.Should().BeSameAs(instance);

            // Act 2 : Tentative de double création
            Action act = () => ConfigurationService.CreateInstance();
            act.Should().Throw<InvalidOperationException>("Le service ne peut être instancié qu'une seule fois.");
        }

        [Fact]
        public void StopAndCommit_AnnuleLeWorkerProprement()
        {
            // Arrange
            var service = ConfigurationService.CreateInstance();

            // Act : StopAndCommit appelle _cts.Cancel() en interne
            Action act = () => service.StopAndCommit();

            // Assert
            act.Should().NotThrow("L'annulation du Task (OperationCanceledException) doit être gérée silencieusement par le bloc catch interne.");
        }

        [Fact]
        public void CommitChangesSync_NettoieLesFlagsDirtyDesSections()
        {
            // Arrange
            var service = ConfigurationService.CreateInstance();

            // On utilise notre StubSection précédent pour générer une modification
            var section = ConfigComponentsTests.StubSection.Instance;
            section.Title = "Test de sauvegarde " + Guid.NewGuid();

            section.IsDirty.Should().BeTrue("Précondition : la section doit être 'Dirty'.");

            // Act : On force la sauvegarde immédiate sans attendre les 10 secondes du Worker
            service.CommitChangesSync();

            // Assert
            section.IsDirty.Should().BeFalse("Le service doit avoir appelé ClearDirtyFlag() après la sauvegarde réussie sur le disque.");
        }
    }
}