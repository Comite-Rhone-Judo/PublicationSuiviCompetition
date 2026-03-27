using FranceJudo.Metier.XML;
using FranceJudo.Metier.Noyau.Organisation;

namespace FranceJudo.Metier.Noyau.Arbitrage
{
    public interface ICommissaire : IXMLSerializable
    {
        public string licence { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public System.DateTime naissance { get; set; }
        public bool sexe { get; set; }
        public EpreuveSexe sexeEnum { get; set; }
        public int categorie { get; set; }
        public bool modification { get; set; }
        public string club { get; set; }
        public string comite { get; set; }
        public string ligue { get; set; }
        public int pays { get; set; }
        public string clubID { get; set; }
        public bool present { get; set; }
        public string remoteID { get; set; }
        public int id { get; set; }
        public bool estResponsable { get; set; }
    }
}
