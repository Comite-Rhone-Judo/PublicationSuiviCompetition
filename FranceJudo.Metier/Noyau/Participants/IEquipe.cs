using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Participants
{
    /// <summary>
    /// Description des Equipes
    /// </summary>
    public interface IEquipe : IXMLSerializable
    {
        public int id { get; set; }
        public string libelle { get; set; }
        public string club { get; set; }
        public string comite { get; set; }
        public string ligue { get; set; }
        public int pays { get; set; }
        public string remoteId { get; set; }
    }
}
