
using System.ComponentModel;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface Ivue_epreuve_phase
    {
        public int id { get;  }
        public int type_phase { get;  }
        public string nom { get;  }
        public string etat { get;  }
        public int nbcombat { get; set; }
        public int nbcombatRep { get; set; }
        public int nbcombattotal { get; set; }
        public int valid { get; set; }
    }
}
