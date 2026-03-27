using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Noyau.Organisation
{
    /// <summary>
    /// Description des Epreuve Equipe
    /// </summary>
    public interface IEpreuve_Equipe : IXMLSerializable
    {
        public int id { get; set; }
        public string libelle { get; set; }
        public System.DateTime debut { get; set; }
        public System.DateTime fin { get; set; }
        public string remoteID { get; set; }
        public int competition { get; set; }
        public int ceintureMin { get; set; }
        public int ceintureMax { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }
        public int categorieAge { get; set; }

        public int epreuveRef { get; set; }

        public EpreuveEquipeTypeEnum type { get; set; }
    }
}
