using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Outils;

namespace AppPublication.Tools
{
    public class EntitePublicationFFJudo : NotificationBase
    {
        private string _nom;
        private string _libelle;

        public string Nom
        {
            get
            {
                return _nom;
            }
            set
            {
                _nom = value;
                NotifyPropertyChanged(nameof(Nom));
            }
        }

        public string Libelle
        {
            get
            {
                return _libelle;
            }
            set
            {
                _libelle = value;
                NotifyPropertyChanged(nameof(Libelle));
            }
        }

        public EntitePublicationFFJudo(string nom, string libelle)
        {
            _nom = nom;
            _libelle = libelle;
        }
    }
}
