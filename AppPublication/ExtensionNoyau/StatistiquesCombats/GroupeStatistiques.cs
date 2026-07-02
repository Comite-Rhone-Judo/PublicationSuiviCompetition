using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.XML;
using System.Xml.Linq;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    public class GroupeStatistiques
    {
        public GroupeStatistiques(int c, EpreuveSexe s, int t, string e)
        {
            Competition = c;
            Sexe = s;
            Type = t;
            Entite = e;
            Id = $"{Competition}-{Sexe}-{Entite}-{Type}";
        }

        public string Id { get; private set; }
        public int Competition { get; set; }
        public EpreuveSexe Sexe { get; set; }
        public int Type { get; set; }
        public string Entite { get; set; }

        public XElement ToXml()
        {
            var xgroupe = new XElement(ConstantXML.GroupeStatistiques_groupe);
            xgroupe.SetAttributeValue(ConstantXML.GroupeStatistique_Competition, Competition);
            xgroupe.SetAttributeValue(ConstantXML.GroupeStatistique_Id, Id);
            xgroupe.SetAttributeValue(ConstantXML.GroupeStatistique_Sexe, Sexe.ToString());
            xgroupe.SetAttributeValue(ConstantXML.GroupeStatistique_Type, Type);
            xgroupe.SetAttributeValue(ConstantXML.GroupeStatistique_Entite, Entite);
            return xgroupe;
        }

        public override bool Equals(object obj) => obj is GroupeStatistiques other && Id == other.Id;
        public override int GetHashCode() => Id?.GetHashCode() ?? 0;
    }
}