using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Arbitrage
{
    public interface IDelegue : IXMLSerializable
    {
        public int id { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public string telephone { get; set; }
        public string mail { get; set; }
        public string fonction { get; set; }
        public string commentaires { get; set; }
    }
}
