using KernelImpl.Noyau.Organisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools.Enum;

namespace AppPublication.ExtensionNoyau.Deroulement
{
    public class GroupeParticipants
    {
        // TODO Voir pour gerer le cas ou l'on veut grouper plusieurs competitions par ex. Jujitsu Combat/Ne waza
        // Identifiant du groupement {IdCompetition}-{sexe}-{ID entite}-{Type entite}
        private string _id;
        public string Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        // Id de la competition associee au groupement
        private int _competition;
        public int Competition
        {
            get { return _competition; }
            set
            {
                _competition = value;
                GetId();
            }
        }

        // le sexe associe au groupement
        EpreuveSexe _sexe;
        public EpreuveSexe Sexe
        {
            get { return _sexe; }
            set
            {
                _sexe = value;
                GetId();
            }
        }

        // Type de l'entite de groupement
        private int _type;
        public int Type
        {
            get { return _type; }
            set
            {
                _type = value;
                GetId();
            }
        }

        // ID de l'entite de groupement
        private string _entite;
        public string Entite
        {
            get { return _entite; }
            set
            {
                _entite = value;
                GetId();
            }
        }

        /// <summary>
        /// Serialize l'objet en XML
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
        {
            XElement xgroupeP = new XElement(ConstantXML.GroupeParticipants_groupe);
            xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_competition, this.Competition);
            xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_id, this.Id);
            xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_sexe, this.Sexe.ToString());
            xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_type, this.Type);
            xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_entite, this.Entite);
            return xgroupeP;
        }

        /// <summary>
        /// Calcul l'identifiant interne du groupe
        /// </summary>
        private void GetId()
        {
            Id = string.Format("{0}{1}{2}{3}", Competition, Sexe.ToString(), Entite, Type);
        }
    }
}
