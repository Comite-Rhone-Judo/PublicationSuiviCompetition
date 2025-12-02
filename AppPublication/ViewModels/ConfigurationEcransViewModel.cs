using AppPublication.Config;
using AppPublication.Controles;
using AppPublication.Models;
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
        private readonly ObservableCollection<EcranAppelModel> _sourceCollection;
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
        public ConfigurationEcransViewModel(ObservableCollection<EcranAppelModel> models, int nbMaxTapis)
        {
            // TODO l'adresse IP est le host ne sont pas chargés correctement au démarrage
            // TODO la zonde de saisie adresse/host est bizarre
            // TODO la résolution des noms ne semble pas fonctionner correctement
            // TODO la sélection des tapis ne fonctionne pas
            // TODO le RAZ host/IP ne fonctionne pas correctement
            // TODO Traiter le cas ou on a des tapis plus loin que le nbMaxTapis (ex: tapis 10 alors que nbMaxTapis=8)

            _sourceCollection = models;
            _tapisDisponibles = Enumerable.Range(1, nbMaxTapis).ToList();

            EcransViewModels = new ObservableCollection<EcranAppelConfigViewModel>();

            // Charger les ViewModels à partir de la collection Runtime de GestionSite
            // Cette collection a déjà été initialisée depuis la config au démarrage de GestionSite
            if (_sourceCollection != null)
            {
                foreach (var model in _sourceCollection)
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
            var nouveauModel = new EcranAppelModel();

            // 2. Ajout à la collection source (GestionSite est mis à jour par référence)
            _sourceCollection.Add(nouveauModel);

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
                // Le VM encapsule le modèle, on peut donc l'utiliser pour la suppression
                // (Note: vm.ModelOriginal n'est pas exposé publiquement dans le code précédent, 
                // on peut soit l'exposer, soit chercher par ID).
                // Ici, on cherche par ID pour être sûr.
                var modelToRemove = _sourceCollection.FirstOrDefault(m => m.Id == vm.Id);
                if (modelToRemove != null) _sourceCollection.Remove(modelToRemove);

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