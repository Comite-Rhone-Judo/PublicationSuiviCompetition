using System;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.XML;
using System.Xml.Linq;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    public class GroupeStatistiques : IEquatable<GroupeStatistiques>
    {
        public int Competition { get; }
        public EpreuveSexe Sexe { get; }
        public string Entite { get; }
        public EchelonEnum Type { get; }

        public string Id => $"{Competition}-{Sexe}-{Entite}-{(int)Type}";

        public GroupeStatistiques(int competition, EpreuveSexe sexe, string entite, EchelonEnum type)
        {
            Competition = competition;
            Sexe = sexe;
            Entite = entite ?? string.Empty;
            Type = type;
        }

        // --- GESTION DE L'UNICITÉ POUR LE HASHSET ET DICTIONNAIRES ---

        public bool Equals(GroupeStatistiques other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Competition == other.Competition &&
                   Sexe.Equals(other.Sexe) &&
                   string.Equals(Entite, other.Entite, StringComparison.OrdinalIgnoreCase) &&
                   Type == other.Type;
        }

        public override bool Equals(object obj) => Equals(obj as GroupeStatistiques);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Competition.GetHashCode();
                hash = hash * 31 + Sexe.GetHashCode();
                hash = hash * 31 + StringComparer.OrdinalIgnoreCase.GetHashCode(Entite);
                hash = hash * 31 + Type.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Serialize l'objet en XML
        /// </summary>
        public XElement ToXml()
        {
            XElement xgroupeS = new XElement(ConstantXML.GroupeStatistiques_groupe);
            xgroupeS.SetAttributeValue(ConstantXML.GroupeStatistiques_Competition, Competition);
            xgroupeS.SetAttributeValue(ConstantXML.GroupeStatistiques_Id, Id);
            xgroupeS.SetAttributeValue(ConstantXML.GroupeStatistiques_Sexe, Sexe.ToString());
            xgroupeS.SetAttributeValue(ConstantXML.GroupeStatistiques_Type, (int) Type);        // On force en int sinon, on a le label de l'enum
            xgroupeS.SetAttributeValue(ConstantXML.GroupeStatistiques_Entite, Entite);
            return xgroupeS;
        }
    }
}