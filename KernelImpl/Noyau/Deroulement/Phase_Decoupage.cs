
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    public class Phase_Decoupage
    {
        public int id { get; set; }
        public int phase { get; set; }
        public int decoupage_finales { get; set; }
        public int decoupage_tableau { get; set; }
        public int decoupage_poule { get; set; }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.PhaseDecoupage_ID));
            this.phase = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.PhaseDecoupage_Phase));
            this.decoupage_finales = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.PhaseDecoupage_Finales));
            this.decoupage_tableau = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.PhaseDecoupage_Tableau));
            this.decoupage_poule = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.PhaseDecoupage_Poule));
        }

        public XElement ToXml()
        {
            XElement xdecoup = new XElement(ConstantXML.PhaseDecoupage);

            xdecoup.SetAttributeValue(ConstantXML.PhaseDecoupage, id);
            xdecoup.SetAttributeValue(ConstantXML.PhaseDecoupage_ID, id);
            xdecoup.SetAttributeValue(ConstantXML.PhaseDecoupage_Phase, phase);
            xdecoup.SetAttributeValue(ConstantXML.PhaseDecoupage_Finales, decoupage_finales);
            xdecoup.SetAttributeValue(ConstantXML.PhaseDecoupage_Poule, decoupage_poule);
            xdecoup.SetAttributeValue(ConstantXML.PhaseDecoupage_Tableau, decoupage_tableau);

            return xdecoup;
        }


        /// <summary>
        /// Lecture des Phase_Decoupage
        /// </summary>
        /// <param name="xelement">élément décrivant les Phase_Decoupage</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Feuilles</returns>

        public static ICollection<Phase_Decoupage> LectureDecoupages(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Phase_Decoupage> decoupages = new List<Phase_Decoupage>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.PhaseDecoupage))
            {
                Phase_Decoupage decoupage = new Phase_Decoupage();
                decoupage.LoadXml(xinfo);
                decoupages.Add(decoupage);
            }
            return decoupages;
        }
    }
}
