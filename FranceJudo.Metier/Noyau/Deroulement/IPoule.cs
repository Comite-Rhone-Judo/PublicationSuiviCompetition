using System.Collections.Generic;
using System.Xml.Linq;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface IPoule
    {
        public int numero { get; set; }
        public int phase { get; set; }
        public int etat { get; set; }
        public int id { get; set; }
        public int nbparticipant { get; set; }

    }
}
