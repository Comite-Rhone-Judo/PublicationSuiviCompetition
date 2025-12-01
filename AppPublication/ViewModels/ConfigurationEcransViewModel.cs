using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AppPublication.Config;
using AppPublication.Controles;
using Tools.Outils;
using Tools.Windows;

namespace AppPublication.ViewModel
{
    public class ConfigurationEcransViewModel : NotificationBase
    {
        private const int TotalTapisCompetition = 8;
        private const int MaxTapisParEcran = 4;

        private ICommand _cmdAjouterEcran;
        private ICommand _cmdSupprimerEcran;

        public ObservableCollection<EcranAppelConfigViewModel> Ecrans { get; set; }

        public ConfigurationEcransViewModel()
        {
            Ecrans = new ObservableCollection<EcranAppelConfigViewModel>();
            LoadFromConfig();
        }

        private void LoadFromConfig()
        {
            Ecrans.Clear();
            foreach (EcranConfigElement element in EcransConfigSection.Instance.Ecrans)
            {
                var vm = new EcranAppelConfigViewModel(element, TotalTapisCompetition, MaxTapisParEcran)
                {
                    DeleteCommand = CmdSupprimerEcran
                };
                Ecrans.Add(vm);
            }
        }

        public ICommand CmdAjouterEcran
        {
            get
            {
                if (_cmdAjouterEcran == null)
                    _cmdAjouterEcran = new RelayCommand(AjouterEcran);
                return _cmdAjouterEcran;
            }
        }

        public ICommand CmdSupprimerEcran
        {
            get
            {
                if (_cmdSupprimerEcran == null)
                    _cmdSupprimerEcran = new RelayCommand(param => SupprimerEcran(param as EcranAppelConfigViewModel));
                return _cmdSupprimerEcran;
            }
        }

        private void AjouterEcran(object obj)
        {
            int newId = 1;
            if (EcransConfigSection.Instance.Ecrans.Count > 0)
            {
                int maxId = 0;
                foreach (EcranConfigElement el in EcransConfigSection.Instance.Ecrans)
                {
                    if (el.Id > maxId) maxId = el.Id;
                }
                newId = maxId + 1;
            }

            var newElement = new EcranConfigElement
            {
                Id = newId,
                Nom = $"Ecran {newId}",
                AdresseIp = ""
            };

            EcransConfigSection.Instance.Ecrans.Add(newElement);

            var vm = new EcranAppelConfigViewModel(newElement, TotalTapisCompetition, MaxTapisParEcran)
            {
                DeleteCommand = CmdSupprimerEcran
            };
            Ecrans.Add(vm);
        }

        private void SupprimerEcran(EcranAppelConfigViewModel ecranVm)
        {
            if (ecranVm != null)
            {
                ConfirmWindow confirm = new ConfirmWindow(
                    "Confirmation",
                    $"Supprimer l'écran '{ecranVm.Nom}' ?"
                );

                if (confirm.ShowDialog() == true)
                {
                    var element = ecranVm.GetConfigElement();
                    EcransConfigSection.Instance.Ecrans.Remove(element);
                    Ecrans.Remove(ecranVm);
                }
            }
        }
    }
}