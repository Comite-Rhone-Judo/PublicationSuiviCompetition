using KernelImpl.Noyau.Organisation;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Participants
{
    /// <summary>
    /// Description des Epreuve auxquelles sont inscrit les Judokas
    /// </summary>
    public class EpreuveJudoka
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

        public Epreuve Epreuve1(JudoData DC)
        {
            return DC.Organisation.Epreuves.FirstOrDefault(o => o.id == epreuve);
        }
        public Judoka Judoka1(JudoData DC)
        {
            return DC.Participants.Judokas.FirstOrDefault(o => o.id == judoka);
        }

        public XElement ToXml()
        {
            XElement xej = new XElement(ConstantXML.EpreuveJudoka);
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_ID, id.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Judoka, judoka.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Classement, classement.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Serie, serie.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Serie2, serie2.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Etat, etat.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Epreuve, epreuve.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Observation, observation.ToString());
            xej.SetAttributeValue(ConstantXML.EpreuveJudoka_Points, points.ToString());

            return xej;
        }

        public void LoadXml(XElement xinfo)
        {
            this.epreuve = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Epreuve));
            this.judoka = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Judoka));
            this.etat = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Etat));
            this.classement = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Classement));
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_ID));
            this.serie = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Serie));
            this.serie2 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Serie2));
            this.observation = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Observation));
            this.points = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.EpreuveJudoka_Points));
        }


        /// <summary>
        /// Lecture des Epreuve des Judoka
        /// </summary>
        /// <param name="xelement">élément décrivant les Epreuve des Judoka</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Epreuve des Judoka</returns>

        public static ICollection<EpreuveJudoka> LectureEpreuveJudokas(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<EpreuveJudoka> ejs = new List<EpreuveJudoka>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.EpreuveJudoka))
            {
                EpreuveJudoka ej = new EpreuveJudoka();
                ej.LoadXml(xinfo);
                ejs.Add(ej);
            }
            return ejs;
        }
    }
}
