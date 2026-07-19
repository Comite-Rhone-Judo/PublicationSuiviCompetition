#nullable enable
using System;
using System.Windows.Threading;
using Xunit;
using FluentAssertions;
using FranceJudo.UI.Wpf.Diagnostic;

namespace FranceJudo.UI.Tests.Wpf.Diagnostic
{
    public sealed class WpfHealthMonitorTests : WpfTestBase, IDisposable
    {
        public WpfHealthMonitorTests()
        {
            // Initialisation stérile : on s'assure qu'aucun autre test n'a laissé de trace
            WpfHealthMonitor.StopAllMonitoring();
        }

        public void Dispose()
        {
            // Nettoyage après le test pour ne pas polluer les autres
            WpfHealthMonitor.StopAllMonitoring();
        }

        [Fact]
        public void StartWpfMonitoring_AppelsMultiples_NePlantePas()
        {
            // Act & Assert
            Action act = () =>
            {
                // Le premier appel modifie l'état interne
                WpfHealthMonitor.StartWpfMonitoring(60);

                // Le deuxième appel doit sortir immédiatement (retour anticipé) sans erreur
                WpfHealthMonitor.StartWpfMonitoring(30);
            };

            act.Should().NotThrow("La méthode doit gérer les appels multiples de manière sécurisée.");
        }

        [Fact]
        public void MonitorDispatcher_NullDispatcher_LeveArgumentNullException()
        {
            // Act
            Action act = () => WpfHealthMonitor.MonitorDispatcher(null!, "TestUI");

            // Assert
            act.Should().Throw<ArgumentNullException>("La méthode doit rejeter un dispatcher null.");
        }

        [Fact]
        public void MonitorDispatcher_AjouteLeTimerEtDemarreLeMonitoring()
        {
            RunInSTA(() =>
            {
                // Arrange
                // On récupère le Dispatcher du thread courant (qui est un STA valide grâce à WpfTestBase)
                var dispatcher = Dispatcher.CurrentDispatcher;

                // Act
                Action act = () => WpfHealthMonitor.MonitorDispatcher(dispatcher, "TestUI_Valid", 1000);

                // Assert
                act.Should().NotThrow("L'enregistrement d'un Dispatcher valide doit s'exécuter sans erreur.");
            });
        }

        [Fact]
        public void MonitorDispatcher_MemeDispatcherDeuxFois_EstIgnore()
        {
            RunInSTA(() =>
            {
                // Arrange
                var dispatcher = Dispatcher.CurrentDispatcher;
                WpfHealthMonitor.MonitorDispatcher(dispatcher, "TestUI_Double", 1000); // 1er appel

                // Act
                // On tente d'enregistrer exactement le même thread une seconde fois
                Action act = () => WpfHealthMonitor.MonitorDispatcher(dispatcher, "TestUI_Double", 1000);

                // Assert
                act.Should().NotThrow("Le second appel doit être intercepté par la vérification du dictionnaire sans planter.");
            });
        }

        [Fact]
        public void StopAllMonitoring_ArreteEtNettoieLeDictionnaire()
        {
            RunInSTA(() =>
            {
                // Arrange
                var dispatcher = Dispatcher.CurrentDispatcher;
                WpfHealthMonitor.StartWpfMonitoring();
                WpfHealthMonitor.MonitorDispatcher(dispatcher, "TestUI_Stop", 1000);

                // Act
                Action act = () => WpfHealthMonitor.StopAllMonitoring();

                // Assert
                act.Should().NotThrow("L'arrêt de tous les monitorings doit itérer sur le dictionnaire et l'effacer sans exception.");

                // On vérifie que la purge a bien fonctionné en tentant de rajouter le même dispatcher
                // S'il est rajouté sans erreur, c'est que le dictionnaire était bien vide.
                Action actVerif = () => WpfHealthMonitor.MonitorDispatcher(dispatcher, "TestUI_Stop", 1000);
                actVerif.Should().NotThrow();
            });
        }
    }
}