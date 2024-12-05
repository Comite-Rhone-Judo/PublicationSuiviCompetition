using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Outils;

namespace Tools.Outils
{
    public class EntitePublicationFFJudo : NotificationBase
    {
        private string _nom;
        private string _libelle;
        private int _echelon;
        private string _login;
        private string _repFtp;

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
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
                NotifyPropertyChanged(nameof(Login));
            }
        }

        public string RepertoireFtp
        {
            get
            {
                return _repFtp;
            }
            set
            {
                _repFtp = value;
                NotifyPropertyChanged(nameof(RepertoireFtp));
            }
        }

        public int Echelon
        {
            get
            {
                return _echelon;
            }
            set
            {
                _echelon = value;
                NotifyPropertyChanged(nameof(Echelon));
            }
        }

        public EntitePublicationFFJudo(string nom, string libelle, int echelon, string login, string rep)
        {
            _nom = nom;
            _libelle = libelle;
            _echelon = echelon;
            _login = login;
            _repFtp = rep;
        }
    }
}
