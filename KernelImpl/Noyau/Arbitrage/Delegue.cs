
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Arbitrage
{
    public class Delegue : IIdEntity<int>
    {
        public int id { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public string telephone { get; set; }
        public string mail { get; set; }
        public string fonction { get; set; }
        public string commentaires { get; set; }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Delegue_ID));
            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Delegue_Nom));
            this.prenom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Delegue_Prenom));
            this.telephone = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Delegue_Telephone));
            this.mail = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Delegue_Mail));
            this.fonction = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Delegue_Fonction));
            this.commentaires = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Delegue_Commentaire));
        }


        public XElement ToXml()
        {
            XElement xdelegue = new System.Xml.Linq.XElement(ConstantXML.Delegue);

            xdelegue.SetAttributeValue(ConstantXML.Delegue_ID, id);
            xdelegue.Add(new XElement(ConstantXML.Delegue_Nom, nom.ToUpper().ToString()));
            xdelegue.Add(new XElement(ConstantXML.Delegue_Prenom, prenom.ToString()));
            xdelegue.SetAttributeValue(ConstantXML.Delegue_Mail, mail);
            xdelegue.SetAttributeValue(ConstantXML.Delegue_Telephone, telephone);
            xdelegue.SetAttributeValue(ConstantXML.Delegue_Fonction, fonction);
            xdelegue.Add(new XElement(ConstantXML.Delegue_Commentaire, commentaires.ToString()));


            return xdelegue;
        }

        /// <summary>
        /// Lecture des Delegue
        /// </summary>
        /// <param name="xelement">élément décrivant les Delegue</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Categories Age</returns>

        public static ICollection<Delegue> LectureDelegue(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Delegue> delegues = new List<Delegue>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Delegue))
            {
                Delegue delegue = new Delegue();
                delegue.LoadXml(xinfo);
                delegues.Add(delegue);
            }
            return delegues;
        }
    }
}
