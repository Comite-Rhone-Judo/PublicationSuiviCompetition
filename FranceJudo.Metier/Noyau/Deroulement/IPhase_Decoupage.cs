using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface IPhase_Decoupage : IXMLSerializable
    {
        public int id { get; set; }
        public int phase { get; set; }
        public int decoupage_finales { get; set; }
        public int decoupage_tableau { get; set; }
        public int decoupage_poule { get; set; }
    }
}
