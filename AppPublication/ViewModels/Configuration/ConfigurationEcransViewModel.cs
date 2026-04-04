using AppPublication.Config.Generation;
using AppPublication.Models.EcransAppel;
using FranceJudo.Core.Foundation;
using FranceJudo.Core.Logging;
using FranceJudo.UI.Wpf.Dialogs;
using FranceJudo.UI.Wpf.Foundation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AppPublication.ViewModels.Configuration
{
    public class ConfigurationEcransViewModel : NotificationBase
    {
        #region CONSTANTES
        public const int kNbTapisDefault = 8;
        #endregion

        #region MEMBERS
        // Collection source (référence vers celle de GestionSite)
        private readonly EcranCollectionManager _ecranManager;
        private readonly List<int> _tapisDisponibles;
        private readonly NetworkScannerContext _scannerContext = new NetworkScannerContext();
        #endregion

        #region PROPERTIES
        // Collection de ViewModels affichée dans la grille
        public ObservableCollection<EcranAppelConfigViewModel> EcransViewModels { get; set; }

        #endregion

        #region COMMANDS

        private ICommand _cmdAjouterEcran;
        public ICommand CmdAjouterEcran
        {
            get
            {
                _cmdAjouterEcran ??= new RelayCommand(AjouterEcranAction);
                return _cmdAjouterEcran;
            }
        }

        private ICommand _cmdOnLoaded;
        public ICommand CmdOnLoaded
        {
            get
            {
                _cmdOnLoaded ??= new RelayCommand(async (o) => await LoadDataAsync());
                return _cmdOnLoaded;
            }
        }

        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur appelé avec la collection de modèles de GestionSite
        /// </summary>
        public ConfigurationEcransViewModel(EcranCollectionManager manager, int nbMaxTapis)
        {
            // Si on n'est pas connecte, on prend par defaut 8 tapis.
            int nbTapisToShow = (nbMaxTapis == 0) ? kNbTapisDefault : nbMaxTapis;

            _ecranManager = manager;
            _tapisDisponibles = Enumerable.Range(1, nbTapisToShow).ToList();

            EcransViewModels = new ObservableCollection<EcranAppelConfigViewModel>();

            Task.Factory.StartNew(async () => { await LoadDataAsync(); });
        }
        #endregion

        #region METHODS

        // Méthode asynchrone appelée par le Behavior
        private async Task LoadDataAsync()
        {
            // Charger les ViewModels à partir de la collection Runtime de GestionSite
            // Cette collection a déjà été initialisée depuis la config au démarrage de GestionSite
            if (_ecranManager != null && _ecranManager.Ecrans != null)
            {
                foreach (var model in _ecranManager.Ecrans)
                {
                    var vm = new EcranAppelConfigViewModel(model, _tapisDisponibles, _scannerContext, () => _ecranManager.InvalidateSnapshot())
                    {
                        DeleteCommand = new RelayCommand(SupprimerLigne)
                    };
                    EcransViewModels.Add(vm);
                }
            }

            if (EcransViewModels.Count > 0) return; // Évite de recharger si déjà fait

            try
            {
                // 3. Travail lourd sur un thread secondaire (Task.Run)
                var listTemp = await Task.Run(() =>
                {
                    var resultList = new List<EcranAppelConfigViewModel>();

                    if (_ecranManager != null && _ecranManager.Ecrans != null)
                    {
                        foreach (var model in _ecranManager.Ecrans)
                        {
                            // La création lourde des sous-VM se fait ici
                            var vm = new EcranAppelConfigViewModel(model, _tapisDisponibles, _scannerContext, () => _ecranManager.InvalidateSnapshot())
                            {
                                DeleteCommand = new RelayCommand(SupprimerLigne)
                            };
                            resultList.Add(vm);
                        }
                    }
                    return resultList;
                });

                // 4. Mise à jour de l'interface sur le Thread Principal
                foreach (var vm in listTemp)
                {
                    EcransViewModels.Add(vm);
                }
            }
            catch (Exception ex)
            {
                // Gérer les erreurs (logging, message utilisateur, etc.)
                LogTools.Logger.Debug(ex, "Erreur lors du chargement des donnees de configuration des ecrans");
            }
        }

        private void AjouterEcranAction(object obj)
        {
            // 1. Création du nouveau modèle
            var nouveauModel = _ecranManager.Add();

            // 3. Ajout à la Configuration (Sauvegarde Disque immédiate)
            var configElement = new EcransAppelConfigElement
            {
                Id = nouveauModel.Id,
                Description = nouveauModel.Description
            };
            GenerationConfigSection.Instance?.Ecrans.Add(configElement);

            // 4. Création du ViewModel et ajout à l'interface
            EcranAppelConfigViewModel vm = new EcranAppelConfigViewModel(nouveauModel, _tapisDisponibles, _scannerContext, () => _ecranManager.InvalidateSnapshot())
            {
                DeleteCommand = new RelayCommand(SupprimerLigne)
            };

            EcransViewModels.Add(vm);
        }

        private void SupprimerLigne(object param)
        {
            if (param is EcranAppelConfigViewModel vm)
            {
                DialogParameters dlgParam = new DialogParameters
                {
                    OkButtonContent = "Oui",
                    CancelButtonContent = "Non",
                    Content = $"Etes-vous sûr de vouloir supprimer l'écran n° {vm.Id}?",
                    Header = "Supprimer un écran"
                };

                ConfirmWindow win = new ConfirmWindow(dlgParam);
                win.ShowDialog();

                if (win.DialogResult.HasValue && (bool)win.DialogResult)
                {
                    // Annuler toute recherche en cours
                    vm.CancelSearch();

                    // 1. Supprimer de l'interface
                    EcransViewModels.Remove(vm);

                    // 2. Supprimer du modèle source (GestionSite)
                    // Ici, on cherche par ID pour être sûr.
                    _ecranManager.Remove(vm.Id);

                    // 3. Supprimer de la Configuration (Disque)
                    GenerationConfigSection.Instance?.Ecrans.Remove(vm.Id);
                }
            }
        }

        public void OnClose()
        {
            foreach (var vm in EcransViewModels) vm.CancelSearch();
        }
        #endregion
    }
}