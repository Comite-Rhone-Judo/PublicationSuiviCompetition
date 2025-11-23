
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Categories
{
    /// <summary>
    /// Description des Categorie Age
    /// </summary>
    public class CategorieAge : IIdEntity<int>
    {
        public int id { get; set; }
        public string nom { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }
        public string ordre { get; set; }
        public string remoteId { get; set; }

        /// <summary>
        /// Charge l'instance à partir d'un noeud XML
        /// </summary>
        /// <param name="xinfo"></param>

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CateAge_id));
            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.CateAge_nom));
            this.anneeMin = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CateAge_anneeMin));
            this.anneeMax = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CateAge_anneeMax));
            this.ordre = XMLTools.LectureString(xinfo.Attribute(ConstantXML.CateAge_ordre));
            this.remoteId = XMLTools.LectureString(xinfo.Attribute(ConstantXML.CateAge_remoteId));
        }

        /// <summary>
        /// Export en XML
        /// </summary>
        /// <returns></returns>

        public XElement ToXml()
        {
            System.Xml.Linq.XElement xcateage = new System.Xml.Linq.XElement(ConstantXML.CateAge);
            xcateage.SetAttributeValue(ConstantXML.CateAge_id, id.ToString());
            xcateage.SetAttributeValue(ConstantXML.CateAge_nom, nom.ToString());
            xcateage.SetAttributeValue(ConstantXML.CateAge_ordre, ordre.ToString());
            xcateage.SetAttributeValue(ConstantXML.CateAge_remoteId, remoteId);
            xcateage.SetAttributeValue(ConstantXML.CateAge_anneeMin, anneeMin);
            xcateage.SetAttributeValue(ConstantXML.CateAge_anneeMax, anneeMax);

            return xcateage;
        }

        public override string ToString()
        {
            return nom;
        }

        /// <summary>
        /// Lecture des Categories Age
        /// </summary>
        /// <param name="xelement">élément décrivant les Categories Age</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Categories Age</returns>

        public static ICollection<CategorieAge> LectureCategorieAge(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<CategorieAge> cateages = new List<CategorieAge>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.CateAge))
            {
                CategorieAge cateage = new CategorieAge();
                cateage.LoadXml(xinfo);
                cateages.Add(cateage);
            }
            return cateages;
        }
    }


}
