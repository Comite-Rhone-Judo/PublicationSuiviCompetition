using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Outils;

namespace AppPublication.Tools.FranceJudo
{
    public class EntitePublicationFFJudo : NotificationBase
    {
        private string _nom;
        private string _libelle;
        private int _echelon;
        private string _login;
        private string _repFtp;
        private string _repHttp;

        public string Nom
        {
            get
            {
                return _nom;
            }
            set
            {
                _nom = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        public string RacineFtp
        {
            get
            {
                return _repFtp;
            }
            set
            {
                _repFtp = value;
                NotifyPropertyChanged();
            }
        }

        public string RacineHttp
        {
            get
            {
                return _repHttp;
            }
            set
            {
                _repHttp = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        public EntitePublicationFFJudo(string nom, string libelle, int echelon, string login, string repFtp, string racineHttp)
        {
            _nom = nom;
            _libelle = libelle;
            _echelon = echelon;
            _login = login;
            _repFtp = repFtp;
            _repHttp = racineHttp;
        }
    }
}
