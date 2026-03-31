using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Structures
{
    /// <summary>
    /// Description des Club
    /// </summary>
    public interface IClub : IXMLSerializable
    {
        public string id { get; set; }
        public string nomCourt { get; set; }
        public string nom { get; set; }
        public string comite { get; set; }
        public string ligue { get; set; }
    }
}
