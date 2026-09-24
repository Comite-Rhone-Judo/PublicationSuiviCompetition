#nullable enable
using AppPublication.ViewModels.Configuration;
using System.ComponentModel;
using Xunit;

namespace AppPublication.Tests.ViewModels.Configuration
{
    public class NetworkScannerViewModelTests
    {
        [Fact]
        public void Constructeur_InitialiseLeContexte_EtLesCommandes()
        {
            // Arrange
            NetworkScannerContext contexte = new NetworkScannerContext();

            // Act
            NetworkScannerViewModel viewModel = new NetworkScannerViewModel(contexte);

            // Assert
            Assert.NotNull(viewModel.CmdLancerRecherche);
            Assert.NotNull(viewModel.CmdAnnulerRecherche);
            Assert.NotNull(viewModel.CmdValider);
            Assert.NotNull(viewModel.CmdFermer);

            // Vérifie que le pointeur de la collection pointe bien vers le contexte partagé
            Assert.Same(contexte.Devices, viewModel.Devices);
        }

        [Fact]
        public void IsScanning_Setter_DeclencheLesNotifications()
        {
            // Arrange
            NetworkScannerContext contexte = new NetworkScannerContext();
            NetworkScannerViewModel viewModel = new NetworkScannerViewModel(contexte);
            bool isScanningNotifie = false;

            viewModel.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(viewModel.IsScanning))
                {
                    isScanningNotifie = true;
                }
            };

            // Act
            viewModel.IsScanning = true;

            // Assert
            Assert.True(isScanningNotifie);
            Assert.True(viewModel.IsScanning);
        }

        [Fact]
        public void Commandes_CanExecute_BasculentSelonEtatDuScanner()
        {
            // Arrange
            NetworkScannerContext contexte = new NetworkScannerContext();
            NetworkScannerViewModel viewModel = new NetworkScannerViewModel(contexte)
            {
                // Act 1 : État de repos (IsScanning = false)
                IsScanning = false
            };


            // FIX : Création d'un mock pour l'interface réseau
            var mockInterface = new Moq.Mock<System.Net.NetworkInformation.NetworkInterface>();
            mockInterface.Setup(ni => ni.Id).Returns("test-id-123");

            // Assignation avec un objet complet
            viewModel.SelectedInterface = new NetworkInterfaceDisplay
            {
                Interface = mockInterface.Object
            };

            bool peutChercherRepos = viewModel.CmdLancerRecherche.CanExecute(null);
            bool peutAnnulerRepos = viewModel.CmdAnnulerRecherche.CanExecute(null);

            // Assert 1 : On peut lancer, on ne peut pas annuler
            Assert.True(peutChercherRepos);
            Assert.False(peutAnnulerRepos);

            // Act 2 : État actif (IsScanning = true)
            viewModel.IsScanning = true;
            bool peutChercherActif = viewModel.CmdLancerRecherche.CanExecute(null);
            bool peutAnnulerActif = viewModel.CmdAnnulerRecherche.CanExecute(null);

            // Assert 2 : On ne peut plus lancer, on peut annuler
            Assert.False(peutChercherActif);
            Assert.True(peutAnnulerActif);
        }

        [Fact]
        public void CmdLancerRecherche_CanExecute_BloqueSiAucuneInterfaceSelectionnee()
        {
            // Arrange
            NetworkScannerContext contexte = new NetworkScannerContext();
            NetworkScannerViewModel viewModel = new NetworkScannerViewModel(contexte)
            {
                // Act
                IsScanning = false,
                SelectedInterface = null! // Aucune interface
            };

            bool peutChercher = viewModel.CmdLancerRecherche.CanExecute(null);

            // Assert
            // Même si on ne scanne pas, on ne peut pas lancer si SelectedInterface est nul
            Assert.False(peutChercher);
        }
    }
}