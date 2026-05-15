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
            ConfigurationService.Instance?.Dispose();
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

        [Fact]
        public void HandleSectionDirty_AppelsSuccessifs_N_AjouteLaSectionQuUneSeuleFois()
        {
            // Arrange
            var service = ConfigurationService.CreateInstance();
            var section = ConfigComponentsTests.StubSection.Instance;

            // On récupère le délégué statique pour l'invoquer manuellement comme si plusieurs enfants criaient "Je suis modifié !"
            var eventDelegate = typeof(InternalConfigSectionBase).GetField("SectionBecameDirty", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var handler = (MulticastDelegate)eventDelegate!.GetValue(null)!;

            // Act
            handler.DynamicInvoke(section);
            handler.DynamicInvoke(section);
            handler.DynamicInvoke(section);

            // Assert
            // On vérifie la liste privée du service pour s'assurer qu'il a bien dédoublonné
            var listField = typeof(ConfigurationService).GetField("_sectionsToSave", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var list = (System.Collections.IList)listField!.GetValue(service)!;

            list.Count.Should().Be(1, "Le service possède un _sectionsToSave.Contains() qui doit éviter les doublons.");
        }

        [Fact]
        public void PerformFallbackSave_ClonageEtSauvegarde_S_ExecuteSansPlanter()
        {
            // Note de l'Architecte : Forcer une ConfigurationErrorsException native depuis le code est hasardeux
            // (blocage de fichier). On va donc directement tester la solidité de la méthode de secours (le Fallback) via Réflexion.

            // Arrange
            var service = ConfigurationService.CreateInstance();
            var section = ConfigComponentsTests.StubSection.Instance;
            section.Title = "SauvegardeFallback_" + Guid.NewGuid();

            var list = new System.Collections.Generic.List<InternalConfigSectionBase> { section };
            var method = typeof(ConfigurationService).GetMethod("PerformFallbackSave", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            // Act
            Action act = () => method!.Invoke(service, new object[] { list });

            // Assert
            act.Should().NotThrow("La méthode de secours (création d'instance via Activator + DeepCopyRecursive + Save) doit s'exécuter jusqu'au bout.");

            // Cleanup
            InternalConfigSectionBase.InvalidateContext(); // Laisse le contexte propre
        }
    }
}