using AppPublication.Config.EcransAppel;
using AppPublication.Controles;
using AppPublication.Models.EcransAppel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Tools.Framework;
using Tools.Logging;
using Tools.Windows;

namespace AppPublication.ViewModels.Configuration
{
    public class ConfigurationEcransViewModel : NotificationBase
    {
        #region MEMBERS
        // Collection source (référence vers celle de GestionSite)
        private readonly EcranCollectionManager _ecranManager;
        private readonly List<int> _tapisDisponibles;
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
                if (_cmdAjouterEcran == null)
                {
                    _cmdAjouterEcran = new RelayCommand(AjouterEcranAction);
                }
                return _cmdAjouterEcran;
            }
        }

        private ICommand _cmdOnLoaded;
        public ICommand CmdOnLoaded
        {
            get
            {
                if (_cmdOnLoaded == null)
                {
                    _cmdOnLoaded = new RelayCommand(async (o) => await LoadDataAsync());
                }
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
            _ecranManager = manager;
            _tapisDisponibles = Enumerable.Range(1, nbMaxTapis).ToList();

            EcransViewModels = new ObservableCollection<EcranAppelConfigViewModel>();

            Task.Factory.StartNew( async () => { await LoadDataAsync(); });
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
                    var vm = new EcranAppelConfigViewModel(model, _tapisDisponibles);
                    vm.DeleteCommand = new RelayCommand(SupprimerLigne);
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
                            var vm = new EcranAppelConfigViewModel(model, _tapisDisponibles);
                            vm.DeleteCommand = new RelayCommand(SupprimerLigne);
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
            catch(Exception ex)
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
            if (EcransAppelConfigSection.Instance != null)
            {
                EcransAppelConfigSection.Instance.Ecrans.Add(configElement);
            }

            // 4. Création du ViewModel et ajout à l'interface
            var vm = new EcranAppelConfigViewModel(nouveauModel, _tapisDisponibles);
            vm.DeleteCommand = new RelayCommand(SupprimerLigne);

            EcransViewModels.Add(vm);
        }

        private void SupprimerLigne(object param)
        {
            var vm = param as EcranAppelConfigViewModel;
            if (vm != null)
            {
                DialogParameters dlgParam = new DialogParameters();
                dlgParam.OkButtonContent = "Oui";
                dlgParam.CancelButtonContent = "Non";
                dlgParam.Content = $"Etes-vous sûr de vouloir supprimer l'écran n° {vm.Id}?";
                dlgParam.Header = "Supprimer un écran";

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
                    if (EcransAppelConfigSection.Instance != null)
                    {
                        EcransAppelConfigSection.Instance.Ecrans.Remove(vm.Id);
                    }
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