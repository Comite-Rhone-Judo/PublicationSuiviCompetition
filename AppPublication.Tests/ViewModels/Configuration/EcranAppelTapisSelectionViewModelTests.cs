#nullable enable
using AppPublication.ViewModels.Configuration;
using System.ComponentModel;
using Xunit;

namespace AppPublication.Tests.ViewModels.Configuration
{
    public class EcranAppelTapisSelectionViewModelTests
    {
        [Fact]
        public void DisplayName_FormateCorrectementLeNumero()
        {
            // Arrange
            EcranAppelTapisSelectionViewModel viewModel = new EcranAppelTapisSelectionViewModel
            {
                // Act
                Numero = 4
            };

            // Assert (xUnit2013 : Expected, Actual)
            Assert.Equal("Tapis 4", viewModel.DisplayName);
        }

        [Fact]
        public void Setters_DeclenchentINotifyPropertyChanged()
        {
            // Arrange
            EcranAppelTapisSelectionViewModel viewModel = new EcranAppelTapisSelectionViewModel();
            bool isSelectedNotifie = false;

            viewModel.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(viewModel.IsSelected)) isSelectedNotifie = true;
            };

            // Act
            viewModel.IsSelected = true;

            // Assert
            Assert.True(isSelectedNotifie);
            Assert.True(viewModel.IsSelected);
        }
    }
}