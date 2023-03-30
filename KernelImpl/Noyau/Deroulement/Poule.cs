
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    public class Poule
    {
        public int numero { get; set; }
        public int phase { get; set; }
        public int etat { get; set; }
        public int id { get; set; }
        public int nbparticipant { get; set; }

        public XElement ToXml()
        {
            XElement xpoule = new XElement(ConstantXML.Poule);

            xpoule.SetAttributeValue(ConstantXML.Poule_ID, this.id);
            xpoule.SetAttributeValue(ConstantXML.Poule_Numero, this.numero);
            xpoule.SetAttributeValue(ConstantXML.Poule_Nom, "Poule " + this.numero);
            xpoule.SetAttributeValue(ConstantXML.Poule_Phase, this.phase);

            return xpoule;
        }

        public void LoadXml(XElement xinfo)
        {
            this.numero = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Poule_Numero));
            this.phase = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Poule_Phase));
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Poule_ID));
        }

        /// <summary>
        /// Lecture des Poule
        /// </summary>
        /// <param name="xelement">élément décrivant les Poule</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>poules</returns>

        public static ICollection<Poule> LecturePoules(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Poule> poules = new List<Poule>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Poule))
            {
                Poule poule = new Poule();
                poule.LoadXml(xinfo);
                poules.Add(poule);
            }
            return poules;
        }
    }
}
