using Tools.Outils;

namespace AppPublication.ViewModel
{
    public class EcranAppelTapisSelectionViewModel : NotificationBase
    {
        private int _numero;
        private bool _isSelected;

        public int Numero
        {
            get { return _numero; }
            set
            {
                if (_numero != value)
                {
                    _numero = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string DisplayName => $"Tapis {Numero}";
    }
}