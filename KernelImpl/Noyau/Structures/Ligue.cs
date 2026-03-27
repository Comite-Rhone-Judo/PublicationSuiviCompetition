
using KernelImpl.Internal;
using System.Collections.Generic;
using System.Xml.Linq;
using FranceJudo.Core.XML;
using FranceJudo.Metier.XML;
using FranceJudo.Metier.Noyau.Structures;
using FranceJudo.Metier.Noyau;


namespace KernelImpl.Noyau.Structures
{
    /// <summary>
    /// Description des Ligues
    /// </summary>
    public class Ligue : ILigue, IEntityWithKey<string>
    {
        string IEntityWithKey<string>.EntityKey => id;

        public string id { get; set; }
        public string nom { get; set; }
        public string nomCourt { get; set; }
        public string code { get; set; }

        public void LoadXml(XElement xligue)
        {
            this.id = XMLTools.LectureString(xligue.Attribute(ConstantXML.Ligue_ID));
            this.nom = XMLTools.LectureString(xligue.Element(ConstantXML.Ligue_Nom));
            this.nomCourt = XMLTools.LectureString(xligue.Element(ConstantXML.Ligue_NomCourt));

            this.code = XMLTools.LectureString(xligue.Attribute(ConstantXML.Ligue_RemoteID));
        }

        public XElement ToXml(IJudoData DC = null)
        {
            XElement xligue = new XElement(ConstantXML.Ligue);

            xligue.SetAttributeValue(ConstantXML.Ligue_ID, id.ToString());
            xligue.Add(new XElement(ConstantXML.Ligue_Nom, nom.ToString()));
            xligue.Add(new XElement(ConstantXML.Ligue_NomCourt, nomCourt.ToString()));
            xligue.Add(new XElement(ConstantXML.Ligue_RemoteID, code.ToString()));

            return xligue;
        }

        /// <summary>
        /// Lecture des Ligues
        /// </summary>
        /// <param name="xelement">élément décrivant les Ligues</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Ligues</returns>

        public static ICollection<Ligue> LectureLigues(XElement xelement)
        {
            ICollection<Ligue> ligues = new List<Ligue>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Ligue))
            {
                Ligue ligue = new Ligue();
                ligue.LoadXml(xinfo);
                ligues.Add(ligue);
            }
            return ligues;
        }
    }
}
