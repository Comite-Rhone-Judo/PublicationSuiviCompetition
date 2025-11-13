using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Tools.Outils;

namespace AppPublication.Controles
{
    /// <summary>
    /// ViewModel représentant un écran d'appel configuré.
    /// </summary>
    public class EcranAppelViewModel : NotificationBase, IEditableObject
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

        private string _hostName;
        public string HostName
        {
            get { return _hostName; }
            set
            {
                if (_hostName != value)
                {
                    _hostName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _ipAddress;
        public string IPAddress
        {
            get { return _ipAddress; }
            set
            {
                if (_ipAddress != value)
                {
                    _ipAddress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<int> _tapis;
        public ObservableCollection<int> Tapis
        {
            get { return _tapis; }
            set
            {
                if (_tapis != value)
                {
                    _tapis = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(TapisSelectionnesString));
                }
            }
        }

        /// <summary>
        /// Propriété calculée pour l'affichage dans la grille.
        /// </summary>
        public string TapisSelectionnesString
        {
            get
            {
                if (Tapis == null || !Tapis.Any())
                    return "Aucun";
                return string.Join(", ", Tapis.OrderBy(t => t));
            }
        }

        public EcranAppelViewModel()
        {
            Tapis = new ObservableCollection<int>();
        }

        // Permet de cloner l'objet pour l'édition
        public EcranAppelViewModel Clone()
        {
            return new EcranAppelViewModel
            {
                Numero = this.Numero,
                HostName = this.HostName,
                IPAddress = this.IPAddress,
                Tapis = new ObservableCollection<int>(this.Tapis)
            };
        }

        // Permet de recopier les données (pour la sauvegarde)
        public void UpdateFrom(EcranAppelViewModel source)
        {
            this.HostName = source.HostName;
            this.IPAddress = source.IPAddress;
            this.Tapis = new ObservableCollection<int>(source.Tapis);
        }


        #region IEditableObject Implementation (pour RadGridView)

        private EcranAppelViewModel _backupCopy;
        private bool _inEdit = false;

        public void BeginEdit()
        {
            if (_inEdit) return;
            _inEdit = true;
            _backupCopy = this.Clone();
        }

        public void CancelEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            this.UpdateFrom(_backupCopy);
        }

        public void EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            _backupCopy = null;
        }

        #endregion
    }
}