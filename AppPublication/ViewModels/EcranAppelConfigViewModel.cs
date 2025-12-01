using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using AppPublication.Config;
using Tools.Outils;

namespace AppPublication.ViewModel
{
    public class EcranAppelConfigViewModel : NotificationBase
    {
        private readonly EcranConfigElement _configElement;
        private readonly int _maxTapisSelectionnables;
        private bool _isLoading;

        public ICommand DeleteCommand { get; set; }

        public EcranAppelConfigViewModel(EcranConfigElement element, int totalTapisDisponibles, int maxTapisSelectionnables = 4)
        {
            _configElement = element;
            _maxTapisSelectionnables = maxTapisSelectionnables;
            ListeTapis = new ObservableCollection<EcranAppelTapisSelectionViewModel>();

            _isLoading = true;
            LoadTapis(totalTapisDisponibles);
            _isLoading = false;
        }

        public int Id
        {
            get { return _configElement.Id; }
        }

        public string Nom
        {
            get { return _configElement.Nom; }
            set
            {
                if (_configElement.Nom != value)
                {
                    _configElement.Nom = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string AdresseIp
        {
            get { return _configElement.AdresseIp; }
            set
            {
                if (_configElement.AdresseIp != value)
                {
                    _configElement.AdresseIp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<EcranAppelTapisSelectionViewModel> ListeTapis { get; private set; }

        private void LoadTapis(int total)
        {
            var tapisActifs = new HashSet<int>();
            if (!string.IsNullOrEmpty(_configElement.TapisIds))
            {
                foreach (var s in _configElement.TapisIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(s, out int id)) tapisActifs.Add(id);
                }
            }

            for (int i = 1; i <= total; i++)
            {
                var vm = new EcranAppelTapisSelectionViewModel
                {
                    Numero = i,
                    IsSelected = tapisActifs.Contains(i)
                };
                vm.PropertyChanged += TapisVm_PropertyChanged;
                ListeTapis.Add(vm);
            }
        }

        private void TapisVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isLoading) return;

            if (e.PropertyName == nameof(EcranAppelTapisSelectionViewModel.IsSelected))
            {
                var item = sender as EcranAppelTapisSelectionViewModel;
                if (item != null && item.IsSelected)
                {
                    if (ListeTapis.Count(t => t.IsSelected) > _maxTapisSelectionnables)
                    {
                        item.IsSelected = false;
                        return;
                    }
                }

                var ids = ListeTapis.Where(t => t.IsSelected).Select(t => t.Numero);
                _configElement.TapisIds = string.Join(";", ids);
            }
        }

        public EcranConfigElement GetConfigElement() => _configElement;
    }
}