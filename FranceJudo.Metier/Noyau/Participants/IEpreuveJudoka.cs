using FranceJudo.Metier.XML;
using FranceJudo.Metier.Noyau.Organisation;

namespace FranceJudo.Metier.Noyau.Participants
{
    /// <summary>
    /// Description des Epreuve auxquelles sont inscrit les Judokas
    /// </summary>
    public interface IEpreuveJudoka : IXMLSerializable
    {

        public int epreuve { get; set; }
        public int judoka { get; set; }
        public int etat { get; set; }
        public int classement { get; set; }
        public int id { get; set; }
        public int serie { get; set; }
        public int serie2 { get; set; }
        public int observation { get; set; }
        public int points { get; set; }

        public IEpreuve Epreuve1(IJudoData DC);
        public IJudoka Judoka1(IJudoData DC);
    }
}
