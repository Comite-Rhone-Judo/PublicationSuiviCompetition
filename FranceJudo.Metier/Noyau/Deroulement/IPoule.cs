using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface IPoule : IXMLSerializable
    {
        public int numero { get; set; }
        public int phase { get; set; }
        public int etat { get; set; }
        public int id { get; set; }
        public int nbparticipant { get; set; }

    }
}
