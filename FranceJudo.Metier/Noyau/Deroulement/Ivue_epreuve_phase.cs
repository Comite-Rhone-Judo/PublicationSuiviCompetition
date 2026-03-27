
using System.ComponentModel;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface Ivue_epreuve_phase
    {
        public int id { get; set; }
        public int type_phase { get; set; }
        public string nom { get; set; }
        public string etat { get; set; }
        public int nbcombat { get; set; }
        public int nbcombatRep { get; set; }
        public int nbcombattotal { get; set; }
        public int valid { get; set; }
    }
}
