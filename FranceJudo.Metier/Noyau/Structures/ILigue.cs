using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Structures
{
    /// <summary>
    /// Description des Ligues
    /// </summary>
    public interface ILigue : IXMLSerializable
    {
        public string id { get; set; }
        public string nom { get; set; }
        public string nomCourt { get; set; }
        public string code { get; set; }

    }
}
