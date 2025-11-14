using System.Collections.ObjectModel;
using System.Linq;
using Tools.Outils;

namespace AppPublication.Controles
{
    /// <summary>
    /// Modèle de données pour un Ecran d'Appel.
    /// Utilisé par le ViewModel de configuration pour la persistance.
    /// </summary>
    public class EcranAppel : NotificationBase
    {
        private int _numero;
        public int Numero
        {
            get { return _numero; }
            set { _numero = value; NotifyPropertyChanged(); }
        }

        private string _hostName;
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; NotifyPropertyChanged(); }
        }

        private string _ipAddress;
        public string IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<int> _tapis;
        public ObservableCollection<int> Tapis
        {
            get { return _tapis; }
            set { _tapis = value; NotifyPropertyChanged(); }
        }

        public EcranAppel()
        {
            Tapis = new ObservableCollection<int>();
        }

        /// <summary>
        /// Clone l'objet pour l'édition.
        /// </summary>
        public EcranAppel Clone()
        {
            return new EcranAppel
            {
                Numero = this.Numero,
                HostName = this.HostName,
                IPAddress = this.IPAddress,
                Tapis = new ObservableCollection<int>(this.Tapis.ToList()) // Copie de la collection
            };
        }

        /// <summary>
        /// Met à jour l'objet depuis une autre instance (utilisé pour l'édition).
        /// </summary>
        public void UpdateFrom(EcranAppel source)
        {
            this.HostName = source.HostName;
            this.IPAddress = source.IPAddress;
            this.Tapis = new ObservableCollection<int>(source.Tapis.ToList());
        }
    }
}