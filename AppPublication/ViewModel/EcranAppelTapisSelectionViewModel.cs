using Tools.Outils;

namespace AppPublication.Controles
{
    /// <summary>
    /// ViewModel pour la sélection d'un tapis (utilisé dans les CheckBox)
    /// </summary>
    public class EcranAppelTapisSelectionViewModel : NotificationBase
    {
        private int _numero;
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

        private bool _isSelected;
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

        public string DisplayName
        {
            get { return $"Tapis {Numero}"; }
        }
    }
}