
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Structures
{
    /// <summary>
    /// Description des Comites
    /// </summary>
    public class Comite : IIdEntity<string>
    {
        private string _id;
        public string id
        {
            get
            {
                int com = 0;
                if (int.TryParse(_id, out com))
                {
                    return com.ToString("00");
                }
                else
                {
                    return _id;
                }
            }
            set
            {
                _id = value;
            }
        }
        public string nom { get; set; }
        public string nomCourt { get; set; }

        public string ligue { get; set; }
        public string code { get; set; }
        public string secteur { get; set; }


        public void LoadXml(XElement xcomite)
        {
            this.id = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_ID));
            this.ligue = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_Ligue));
            this.nom = XMLTools.LectureString(xcomite.Element(ConstantXML.Comite_Nom));
            this.nomCourt = this.id;
            this.code = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_RemoteID));
            this.secteur = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_Secteur));

        }

        public XElement ToXml()
        {
            XElement xcomite = new XElement(ConstantXML.Comite);

            int com = 0;
            if (int.TryParse(id, out com))
            {
                xcomite.SetAttributeValue(ConstantXML.Comite_ID, com.ToString("00"));
            }
            else
            {
                xcomite.SetAttributeValue(ConstantXML.Comite_ID, id);
            }
            xcomite.SetAttributeValue(ConstantXML.Comite_Ligue, ligue.ToString());
            xcomite.Add(new XElement(ConstantXML.Comite_Nom, nom.ToString()));
            xcomite.Add(new XElement(ConstantXML.Comite_NomCourt, nomCourt.ToString()));

            xcomite.SetAttributeValue(ConstantXML.Comite_RemoteID, code.ToString());
            xcomite.SetAttributeValue(ConstantXML.Comite_Secteur, secteur.ToString());

            return xcomite;
        }

        /// <summary>
        /// Lecture des Comites
        /// </summary>
        /// <param name="xelement">élément décrivant les Comites</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Comites</returns>

        public static ICollection<Comite> LectureComites(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Comite> comites = new List<Comite>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Comite))
            {
                Comite comite = new Comite();
                comite.LoadXml(xinfo);
                comites.Add(comite);
            }
            return comites;
        }
    }
}
