using AppPublication.Config;
using AppPublication.Config.Generation;
using AppPublication.Config.Publication;
using System;
using Xunit;

namespace AppPublication.Tests.Config
{
    // L'attribut [Collection] peut être utilisé si vous avez besoin de forcer xUnit 
    // à exécuter les tests touchant au système de fichiers de manière séquentielle.
    // [Collection("Configuration Files")] 
    public class AppConfigRootTests
    {
        [Fact]
        public void Constructeur_InitialiseLesSectionsParDefaut()
        {
            // Arrange & Act
            // On instancie directement la classe sans passer par le Singleton 
            // pour tester son état initial sans déclencher l'accès au disque
            AppConfigRoot config = new AppConfigRoot();

            // Assert
            Assert.NotNull(config.Publication);
            Assert.NotNull(config.Generation);
        }

        [Fact]
        public void InitializeSync_AssigneLActionDeNotification()
        {
            // Arrange
            AppConfigRoot config = new AppConfigRoot();
            bool notificationDeclenchee = false;

            // Typage strict de l'Action (pas de var)
            void methodeNotification()
            {
                notificationDeclenchee = true;
            }

            // Act
            config.InitializeSync(methodeNotification);

            // Assert
            // On vérifie que la propriété OnChanged (héritée de JsonConfigSection) a bien été affectée
            Assert.NotNull(config.OnChanged);

            // On simule un changement pour s'assurer que le délégué est le bon
            config.OnChanged.Invoke();
            Assert.True(notificationDeclenchee);
        }

        [Fact]
        public void Instance_RetourneUneInstanceValideEtUnique()
        {
            // Act
            // Ce test va réellement créer/lire le fichier appsettings.json dans le répertoire bin des tests
            AppConfigRoot instance1 = AppConfigRoot.Instance;
            AppConfigRoot instance2 = AppConfigRoot.Instance;

            // Assert
            Assert.NotNull(instance1);
            Assert.Same(instance1, instance2); // Vérifie que le Singleton fonctionne (référence mémoire identique)
        }

        // Note : Il est difficile de tester Stop() de manière isolée car cela invoque _service?.Dispose()
        // sur une variable statique globale, ce qui pourrait casser les autres tests s'ils tournent en parallèle.
        // Si nécessaire, il faudrait utiliser un exécuteur de test séquentiel (ICollectionFixture dans xUnit).
    }
}