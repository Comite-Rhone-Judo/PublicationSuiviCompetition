using FranceJudo.Core.Foundation;
using System;

namespace AppPublication.Statistiques
{
    public abstract class StatistiqueItem : NotificationBase
    {
        #region CONSTRUCTEUR
        public StatistiqueItem(string name, string libelle, string unite = "")
        {
            Nom = name;
            Libelle = libelle;
            Unite = unite;
        }

        #endregion

        #region PROPRIETES

        private string _nom = String.Empty;
        public string Nom
        {
            get
            {
                return _nom;
            }
            protected set
            {
                _nom = value;
                NotifyPropertyChanged();
            }
        }

        private string _libelle = String.Empty;
        public string Libelle
        {
            get
            {
                return _libelle;
            }
            protected set
            {
                _libelle = value;
                NotifyPropertyChanged();
            }
        }

        private float? _valeur = null;
        public float? Valeur
        {
            get
            {
                return _valeur;
            }
            protected set
            {
                _valeur = value;
                NotifyPropertyChanged();
            }
        }

        private string _unite = string.Empty;
        public string Unite
        {
            get { return _unite; }
            protected set { _unite = value; NotifyPropertyChanged(); }
        }

        private float? _max = null;
        public float? Max
        {
            get
            {
                return _max;
            }
            protected set
            {
                _max = value;
                NotifyPropertyChanged();
            }
        }

        private float? _moy = null;
        public float? Moy
        {
            get
            {
                return _moy;
            }
            protected set
            {
                _moy = value;
                NotifyPropertyChanged();
            }
        }

        private float? _min = null;
        public float? Min
        {
            get
            {
                return _min;
            }
            protected set
            {
                _min = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region METHODES
        public abstract void EnregistrerValeur(float? val = null);
        #endregion

    }
}
