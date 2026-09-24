#nullable enable
using AppPublication.Models.EcransAppel;
using AppPublication.ViewModels.Configuration;
using Xunit;

namespace AppPublication.Tests.ViewModels.Configuration
{
    public class ConfigurationEcransViewModelTests
    {
        [Fact]
        public void Constructeur_InitialiseLesCollections_EtLesCommandes()
        {
            // Arrange
            EcranCollectionManager manager = new EcranCollectionManager();

            // Act
            ConfigurationEcransViewModel viewModel = new ConfigurationEcransViewModel(manager, 8);

            // Assert
            Assert.NotNull(viewModel.EcransViewModels);
            Assert.Empty(viewModel.EcransViewModels);
            Assert.NotNull(viewModel.CmdAjouterEcran);
            Assert.NotNull(viewModel.CmdOnLoaded);
        }

        [Fact]
        public void CmdAjouterEcran_Execute_AjouteUnEcranAuManagerEtALaVue()
        {
            // Arrange
            EcranCollectionManager manager = new EcranCollectionManager();
            ConfigurationEcransViewModel viewModel = new ConfigurationEcransViewModel(manager, 8);

            // Act
            viewModel.CmdAjouterEcran.Execute(null);

            // Assert
            // 1. Vérification de la Vue (ViewModel local)
            Assert.Single(viewModel.EcransViewModels);

            // 2. Vérification du Métier (Manager)
            Assert.Single(manager.Ecrans);

            // On s'assure que les deux éléments sont bien liés par le même ID (ID 1 généré par le manager)
            Assert.Equal(1, viewModel.EcransViewModels[0].Id);
            Assert.Equal(1, manager.Ecrans[0].Id);
        }
    }
}