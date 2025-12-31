
using KernelImpl.Internal;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;
using Tools.XML;

namespace KernelImpl.Noyau.Categories
{
    /// <summary>
    /// Description des Ceintures
    /// </summary>
    public class Ceintures : IEntityWithKey<int>
    {
        int IEntityWithKey<int>.EntityKey => id;

        public int id { get; set; }
        public string nom { get; set; }
        public string ordre { get; set; }
        public string remoteId { get; set; }
        public string couleur1 { get; set; }
        public string couleur2 { get; set; }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Ceinture_id));
            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Ceinture_nom));
            this.ordre = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Ceinture_ordre));
            this.remoteId = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Ceinture_remoteId));
            this.couleur1 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Ceinture_couleur1));
            this.couleur2 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Ceinture_couleur2));
        }

        public XElement ToXml()
        {
            XElement xceinture = new XElement(ConstantXML.Ceinture);
            xceinture.SetAttributeValue(ConstantXML.Ceinture_id, id.ToString());
            xceinture.SetAttributeValue(ConstantXML.Ceinture_nom, nom.ToString());
            xceinture.SetAttributeValue(ConstantXML.Ceinture_ordre, ordre.ToString());
            xceinture.SetAttributeValue(ConstantXML.Ceinture_remoteId, remoteId);
            xceinture.SetAttributeValue(ConstantXML.Ceinture_couleur1, couleur1);
            xceinture.SetAttributeValue(ConstantXML.Ceinture_couleur2, couleur2);

            return xceinture;
        }

        /// <summary>
        /// Lecture des Ceintures
        /// </summary>
        /// <param name="xelement">élément décrivant les Ceintures</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Ceintures</returns>

        public static ICollection<Ceintures> LectureCeintures(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Ceintures> ceintures = new List<Ceintures>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Ceinture))
            {
                Ceintures compet = new Ceintures();
                compet.LoadXml(xinfo);
                ceintures.Add(compet);
            }
            return ceintures;
        }
    }
}
