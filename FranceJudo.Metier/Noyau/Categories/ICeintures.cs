using FranceJudo.Metier.XML;


namespace FranceJudo.Metier.Noyau.Categories
{
    /// <summary>
    /// Description des Ceintures
    /// </summary>
    public interface ICeintures : IXMLSerializable
    {
        public int id { get; set; }
        public string nom { get; set; }
        public string ordre { get; set; }
        public string remoteId { get; set; }
        public string couleur1 { get; set; }
        public string couleur2 { get; set; }
    }
}
