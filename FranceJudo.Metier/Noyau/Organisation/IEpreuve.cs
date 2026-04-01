using FranceJudo.Metier.XML;
using System;


namespace FranceJudo.Metier.Noyau.Organisation
{
    /// <summary>
    /// Description des Epreuves
    /// </summary>
    public interface IEpreuve : IXMLSerializable
    {
        public int id { get; set; }
        public string nom { get; set; }
        public System.DateTime debut { get; set; }
        public System.DateTime fin { get; set; }
        public string remoteID { get; set; }
        public int competition { get; set; }
        public int categoriePoids { get; set; }
        public int poidsMin { get; set; }
        public int poidsMax { get; set; }
        public int ceintureMin { get; set; }
        public int ceintureMax { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }

        public int sexe { get; set; }
        public EpreuveSexe sexeEnum { get; set; }

        public int categorieAge { get; set; }
        public Nullable<int> epreuve_equipe { get; set; }
    }
}