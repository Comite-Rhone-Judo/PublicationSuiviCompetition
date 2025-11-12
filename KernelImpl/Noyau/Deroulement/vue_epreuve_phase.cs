
using System.ComponentModel;
using Tools.Enum;

namespace KernelImpl.Noyau.Deroulement
{
    public class vue_epreuve_phase : INotifyPropertyChanged, IIdEntity<int>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public vue_epreuve_phase(Phase phase)
        {
            _id = phase.id;
            _nom = phase.libelle;
            _type_phase = phase.typePhase;
            _etat = ((EtatPhaseEnum)phase.etat).ToString();
        }

        int _id = 0;
        public int id
        {
            get
            {
                return _id;
            }
        }

        int _type_phase = 0;
        public int type_phase
        {
            get
            {
                return _type_phase;
            }
        }

        string _nom = "";
        public string nom
        {
            get
            {
                return _nom;
            }
        }

        string _etat = "";
        public string etat
        {
            get
            {
                return _etat;
            }
        }



        int _nbc = 0;
        public int nbcombat
        {
            get
            {
                return _nbc;
            }
            set
            {
                if (_nbc != value)
                {
                    _nbc = value;
                    OnPropertyChanged("nbcombat");
                }
            }
        }

        int _nbcRep = 0;
        public int nbcombatRep
        {
            get
            {
                return _nbcRep;
            }
            set
            {
                if (_nbcRep != value)
                {
                    _nbcRep = value;
                    OnPropertyChanged("nbcombatRep");
                }
            }
        }

        int _nbct = 0;
        public int nbcombattotal
        {
            get
            {
                return _nbct;
            }
            set
            {
                if (_nbct != value)
                {
                    _nbct = value;
                    OnPropertyChanged("nbcombattotal");
                }
            }
        }

        int _valid = -1;
        public int valid
        {
            get
            {
                return _valid;
            }
            set
            {
                if (_valid != value)
                {
                    _valid = value;
                    OnPropertyChanged("valid");
                }
            }
        }
    }
}
