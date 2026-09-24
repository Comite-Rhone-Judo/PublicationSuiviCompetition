#nullable enable
using System;
using System.Windows;
using System.Windows.Input;
using Xunit;
using Moq;
using FranceJudo.UI.Wpf.Behaviors;

namespace FranceJudo.UI.Tests.Wpf.Behaviors
{
    public class WindowLoadBehaviorTests : WpfTestBase
    {
        [Fact]
        public void SetLoadedCommand_AffecteCorrectementLaProprieteAttachee()
        {
            RunInSTA(() =>
            {
                // Arrange
                var window = new Window();
                var mockCommand = new Mock<ICommand>();

                // Act
                WindowLoadBehavior.SetLoadedCommand(window, mockCommand.Object);
                var retrievedCommand = WindowLoadBehavior.GetLoadedCommand(window);

                // Assert
                Assert.Same(mockCommand.Object, retrievedCommand);
            });
        }

        [Fact]
        public void WindowLoaded_ExecuteLaCommande_SiCanExecuteEstVrai()
        {
            RunInSTA(() =>
            {
                // Arrange
                var window = new Window();
                var mockCommand = new Mock<ICommand>();
                mockCommand.Setup(c => c.CanExecute(null)).Returns(true);

                WindowLoadBehavior.SetLoadedCommand(window, mockCommand.Object);

                // Act
                window.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

                // Assert
                mockCommand.Verify(c => c.Execute(null), Times.Once);
            });
        }

        [Fact]
        public void WindowLoaded_NExecutePasLaCommande_SiCanExecuteEstFaux()
        {
            RunInSTA(() =>
            {
                // Arrange
                var window = new Window();
                var mockCommand = new Mock<ICommand>();
                mockCommand.Setup(c => c.CanExecute(null)).Returns(false);

                WindowLoadBehavior.SetLoadedCommand(window, mockCommand.Object);

                // Act
                window.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

                // Assert
                mockCommand.Verify(c => c.Execute(It.IsAny<object>()), Times.Never);
            });
        }

        [Fact]
        public void SetLoadedCommand_ANull_DesabonneLEvenement()
        {
            RunInSTA(() =>
            {
                // Arrange
                var window = new Window();
                var mockCommand = new Mock<ICommand>();
                mockCommand.Setup(c => c.CanExecute(null)).Returns(true);

                WindowLoadBehavior.SetLoadedCommand(window, mockCommand.Object);

                // Act
                WindowLoadBehavior.SetLoadedCommand(window, null!);
                window.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

                // Assert
                mockCommand.Verify(c => c.Execute(It.IsAny<object>()), Times.Never);
            });
        }
    }
}