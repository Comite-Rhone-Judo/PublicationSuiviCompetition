using FranceJudo.Metier.XML;
using System;


namespace FranceJudo.Metier.Noyau.Deroulement
{
    /// <summary>
    /// Description des Feuilles (construction d'un tableau)
    /// </summary>
    public interface IFeuille : IXMLSerializable
    {
        public int id { get; set; }
        public bool repechage { get; set; }
        public int source1 { get; set; }
        public int source2 { get; set; }
        public string reference { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public bool typeSource { get; set; }
        public int numero { get; set; }
        public int ordre { get; set; }
        public int pere { get; set; }
        public int classement1 { get; set; }
        public int classement2 { get; set; }
        public int niveau { get; set; }
        public Nullable<int> combat { get; set; }
        public int phase { get; set; }
    }
}
