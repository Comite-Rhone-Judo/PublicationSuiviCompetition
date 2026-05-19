#nullable enable
using AppPublication.ViewModels.Main;
using FranceJudo.Core.Network;
using System.ComponentModel;
using Xunit;

namespace AppPublication.Tests.ViewModels.Main
{
    public class TestFtpViewModelTests
    {
        [Fact]
        public void Constructeur_InitialiseLesCommandesEtLaCollection()
        {
            // Arrange
            // On peut passer null! car FtpTestScheduler accepte probablement une instance nulle 
            // ou ne crashe pas immédiatement à l'instanciation.
            TestFtpViewModel viewModel = new TestFtpViewModel(null!);

            // Act & Assert (xUnit2013 : Expected, Actual)
            Assert.NotNull(viewModel.TestSteps);
            Assert.NotNull(viewModel.CmdStartTest);
            Assert.NotNull(viewModel.CmdCancelTest);
            Assert.False(viewModel.IsTestRunning);
        }

        [Fact]
        public void IsTestRunning_Setter_DeclencheLaNotification()
        {
            // Arrange
            TestFtpViewModel viewModel = new TestFtpViewModel(null!);
            bool notificationDeclenchee = false;

            // Abonnement à l'événement de NotificationBase
            viewModel.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "IsTestRunning")
                {
                    notificationDeclenchee = true;
                }
            };

            // Act
            viewModel.IsTestRunning = true;

            // Assert
            Assert.True(notificationDeclenchee);
            Assert.True(viewModel.IsTestRunning);
        }

        [Fact]
        public void Commandes_CanExecute_BasculentEnFonctionDeIsTestRunning()
        {
            // Arrange
            TestFtpViewModel viewModel = new TestFtpViewModel(null!);

            // Act 1 : État de repos (IsTestRunning = false par défaut)
            bool startAutoriseRepos = viewModel.CmdStartTest.CanExecute(null);
            bool cancelAutoriseRepos = viewModel.CmdCancelTest.CanExecute(null);

            // Assert 1 : On peut démarrer, mais pas annuler
            Assert.True(startAutoriseRepos);
            Assert.False(cancelAutoriseRepos);

            // Act 2 : État actif (IsTestRunning = true)
            viewModel.IsTestRunning = true;
            bool startAutoriseActif = viewModel.CmdStartTest.CanExecute(null);
            bool cancelAutoriseActif = viewModel.CmdCancelTest.CanExecute(null);

            // Assert 2 : On ne peut plus démarrer, mais on peut annuler
            Assert.False(startAutoriseActif);
            Assert.True(cancelAutoriseActif);
        }
    }
}