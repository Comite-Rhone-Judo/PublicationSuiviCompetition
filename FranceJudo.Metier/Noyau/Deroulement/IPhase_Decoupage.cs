using System.Collections.Generic;
using System.Xml.Linq;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public class IPhase_Decoupage
    {
        public int id { get; set; }
        public int phase { get; set; }
        public int decoupage_finales { get; set; }
        public int decoupage_tableau { get; set; }
        public int decoupage_poule { get; set; }
    }
}
