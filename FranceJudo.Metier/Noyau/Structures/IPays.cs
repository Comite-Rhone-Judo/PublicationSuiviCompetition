using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Structures
{
    public interface IPays : IXMLSerializable
    {
        public int id { get; set; }
        public int code { get; set; }
        public string abr2 { get; set; }
        public string abr3 { get; set; }
        public string nom { get; set; }
        public string AbrF { get; set; }
    }
}
