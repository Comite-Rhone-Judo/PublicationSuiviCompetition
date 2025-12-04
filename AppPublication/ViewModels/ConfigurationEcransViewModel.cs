using AppPublication.Config.EcransAppel;
using AppPublication.Controles;
using AppPublication.Managers;
using AppPublication.Models;
using KernelManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Tools.Outils;

namespace AppPublication.ViewModels
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

            // Charger les ViewModels à partir de la collection Runtime de GestionSite
            // Cette collection a déjà été initialisée depuis la config au démarrage de GestionSite
            if(_ecranManager != null && _ecranManager.Ecrans != null)
            {
                foreach (var model in _ecranManager.Ecrans)
                {
                    var vm = new EcranAppelConfigViewModel(model, _tapisDisponibles);
                    vm.DeleteCommand = new RelayCommand(SupprimerLigne);
                    EcransViewModels.Add(vm);
                }
            }
        }
        #endregion

        #region METHODS
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

        public void OnClose()
        {
            foreach (var vm in EcransViewModels) vm.CancelSearch();
        }
        #endregion
    }
}