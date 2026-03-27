using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Structures
{
    /// <summary>
    /// Description des Secteurs
    /// </summary>
    public interface ISecteur : IXMLSerializable
    {
        public string id { get; set; }
        public string nom { get; set; }
        public string nomCourt { get; set; }
    }
}
