
using KernelImpl.Internal;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Structures
{
    /// <summary>
    /// Description des Secteurs
    /// </summary>
    public class Secteur : IEntityWithKey<string>
    {
        string IEntityWithKey<string>.EntityKey => id;

        public string id { get; set; }
        public string nom { get; set; }
        public string nomCourt { get; set; }

        public void LoadXml(XElement xsecteur)
        {
            this.id = XMLTools.LectureString(xsecteur.Attribute(ConstantXML.Secteur_ID));
            this.nom = XMLTools.LectureString(xsecteur.Element(ConstantXML.Secteur_Nom));
            this.nomCourt = XMLTools.LectureString(xsecteur.Element(ConstantXML.Secteur_NomCourt));
        }

        public XElement ToXml()
        {
            XElement xsecteur = new XElement(ConstantXML.Secteur);

            xsecteur.SetAttributeValue(ConstantXML.Secteur_ID, id.ToString());
            xsecteur.Add(new XElement(ConstantXML.Secteur_Nom, nom.ToString()));
            xsecteur.Add(new XElement(ConstantXML.Secteur_NomCourt, nomCourt.ToString()));

            return xsecteur;
        }

        /// <summary>
        /// Lecture des Secteurs
        /// </summary>
        /// <param name="xelement">élément décrivant les Secteurs</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Secteurs</returns>

        public static ICollection<Secteur> LectureSecteurs(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Secteur> secteurs = new List<Secteur>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Secteur))
            {
                Secteur secteur = new Secteur();
                secteur.LoadXml(xinfo);
                secteurs.Add(secteur);
            }
            return secteurs;
        }
    }
}
