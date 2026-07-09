using System;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.XML;
using System.Xml.Linq;

namespace FranceJudo.Metier.ExtensionNoyau.Engagement
{
    public class GroupeEngagements : IEquatable<GroupeEngagements>
    {
        public int Competition { get; }
        public EpreuveSexe Sexe { get; }
        public string Entite { get; }
        public EchelonEnum Type { get; }

        public string Id => $"{Competition}-{Sexe}-{Entite}-{(int)Type}";

        public GroupeEngagements(int competition, EpreuveSexe sexe, string entite, EchelonEnum type)
        {
            Competition = competition;
            Sexe = sexe;
            Entite = entite ?? string.Empty;
            Type = type;
        }

        // --- GESTION DE L'UNICITÉ POUR LE HASHSET ---

        public bool Equals(GroupeEngagements other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Competition == other.Competition &&
                   Sexe.Equals(other.Sexe) &&
                   string.Equals(Entite, other.Entite, StringComparison.OrdinalIgnoreCase) &&
                   Type == other.Type;
        }

        public override bool Equals(object obj) => Equals(obj as GroupeEngagements);

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
            XElement xgroupeP = new XElement(ConstantXML.GroupeEngagements_Groupe);
            xgroupeP.SetAttributeValue(ConstantXML.GroupeEngagements_Competition, Competition);
            xgroupeP.SetAttributeValue(ConstantXML.GroupeEngagements_Id, Id);
            xgroupeP.SetAttributeValue(ConstantXML.GroupeEngagements_Sexe, Sexe.ToString());
            xgroupeP.SetAttributeValue(ConstantXML.GroupeEngagements_Type, (int) Type);     // On force en int pour ne pas avoir le label de l'enum
            xgroupeP.SetAttributeValue(ConstantXML.GroupeEngagements_Entite, Entite);
            return xgroupeP;
        }
    }
}