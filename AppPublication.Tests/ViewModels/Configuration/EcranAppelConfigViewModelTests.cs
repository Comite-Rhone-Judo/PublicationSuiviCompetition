#nullable enable
using AppPublication.Models.EcransAppel;
using AppPublication.ViewModels.Configuration;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace AppPublication.Tests.ViewModels.Configuration
{
    public class EcranAppelConfigViewModelTests
    {
        [Fact]
        public void Constructeur_InitialiseLesChamps_APartirDuModel()
        {
            // Arrange
            EcranAppelModel modele = new EcranAppelModel
            {
                Hostname = "PC-JUDO",
                AdresseIP = IPAddress.Loopback,
                TapisIds = new List<int> { 2, 4 }
            };
            List<int> tousLesTapis = new List<int> { 1, 2, 3, 4 };
            NetworkScannerContext contexte = new NetworkScannerContext();

            // Act
            // Le 4ème paramètre est l'Action onModelChanged. On passe une fonction vide.
            EcranAppelConfigViewModel viewModel = new EcranAppelConfigViewModel(modele, tousLesTapis, contexte, delegate { });

            // Assert
            Assert.Equal("PC-JUDO", viewModel.Hostname);
            Assert.Equal("127.0.0.1", viewModel.AdresseIP);
            Assert.Equal("127.0.0.1", viewModel.RawUserInput);

            // On vérifie que les 4 tapis sont instanciés, et que les bons sont cochés
            Assert.Equal(4, viewModel.ListeTapisViewModels.Count);
            Assert.False(viewModel.ListeTapisViewModels[0].IsSelected); // Tapis 1
            Assert.True(viewModel.ListeTapisViewModels[1].IsSelected);  // Tapis 2
            Assert.True(viewModel.ListeTapisViewModels[3].IsSelected);  // Tapis 4
        }

        [Fact]
        public void NbCombatsPage_Setter_IgnoreLesValeursHorsLimites()
        {
            // Arrange
            EcranAppelModel modele = new EcranAppelModel { NbCombatsPage = 8 };
            EcranAppelConfigViewModel viewModel = new EcranAppelConfigViewModel(modele, new List<int>(), new NetworkScannerContext(), delegate { })
            {
                // Act
                // Tentative d'affectation d'une valeur invalide (max est kMaxCombatsPage = 12)
                NbCombatsPage = 99
            };

            // Assert
            // La clause de garde dans votre code (value >= 1 && value <= kMaxCombatsPage) doit avoir bloqué l'assignation
            Assert.Equal(8, viewModel.NbCombatsPage);

            // Act 2
            viewModel.NbCombatsPage = 10; // Valeur valide

            // Assert 2
            Assert.Equal(10, viewModel.NbCombatsPage);
        }

        [Fact]
        public void ListeTapisSelectionnesAffiche_FormateLaChaine_Correctement()
        {
            // Arrange
            EcranAppelModel modele = new EcranAppelModel();
            // On déclare 5 tapis disponibles
            List<int> tousLesTapis = new List<int> { 1, 2, 3, 4, 5 };
            EcranAppelConfigViewModel viewModel = new EcranAppelConfigViewModel(modele, tousLesTapis, new NetworkScannerContext(), delegate { });

            // Act & Assert 1 : Aucun tapis
            Assert.Equal("Aucun tapis", viewModel.ListeTapisSelectionnesAffiche);

            // Act & Assert 2 : Un seul tapis
            viewModel.ListeTapisViewModels[2].IsSelected = true; // Tapis 3
            Assert.Equal("Tapis 3", viewModel.ListeTapisSelectionnesAffiche);

            // Act & Assert 3 : Formatage complexe (Tapis 3 et 5)
            viewModel.ListeTapisViewModels[4].IsSelected = true; // Tapis 5
            Assert.Equal("Tapis 3 et 5", viewModel.ListeTapisSelectionnesAffiche);

            // Act & Assert 4 : Formatage complet (Tapis 1, 3 et 5)
            viewModel.ListeTapisViewModels[0].IsSelected = true; // Tapis 1
            Assert.Equal("Tapis 1, 3 et 5", viewModel.ListeTapisSelectionnesAffiche);
        }
    }
}