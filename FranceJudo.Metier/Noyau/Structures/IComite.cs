using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Structures
{
    /// <summary>
    /// Description des Comites
    /// </summary>
    public interface IComite : IXMLSerializable
    {
        public string id {  get; set; }
        public string nom { get; set; }
        public string nomCourt { get; set; }

        public string ligue { get; set; }
        public string code { get; set; }
        public string secteur { get; set; }

    }
}
