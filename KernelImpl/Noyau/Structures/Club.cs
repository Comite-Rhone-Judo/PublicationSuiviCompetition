using KernelImpl.Noyau.Structures;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Structures
{
    /// <summary>
    /// Description des Club
    /// </summary>
    public class Club
    {
        public string id { get; set; }
        public string nomCourt { get; set; }
        public string nom { get; set; }
        public string comite { get; set; }
        public string ligue { get; set; }

        public void LoadXml(XElement xinfo)
        {
            this.comite = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Club_Comite));
            this.ligue = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Club_Ligue));
            this.id = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Club_ID));

            this.nomCourt = XMLTools.LectureString(xinfo.Element(ConstantXML.Club_NomCourt));
            this.nom = XMLTools.LectureString(xinfo.Element(ConstantXML.Club_Nom));

            //this.nom = xinfo.Attribute(ConstantXML.Club_Nom) != null ? xinfo.Attribute(ConstantXML.Club_Nom).Value : "";
            //this.nomCourt = xinfo.Attribute(ConstantXML.Club_NomCourt) != null ? xinfo.Attribute(ConstantXML.Club_NomCourt).Value : "";
        }

        public XElement ToXml()
        {
            XElement xclub = new XElement(ConstantXML.Club);

            xclub.Add(new XElement(ConstantXML.Club_NomCourt, nomCourt.ToString()));
            xclub.Add(new XElement(ConstantXML.Club_Nom, nom.ToString()));
            xclub.SetAttributeValue(ConstantXML.Club_ID, id.ToString());

            int com = 0;
            if (int.TryParse(comite, out com))
            {
                xclub.SetAttributeValue(ConstantXML.Club_Comite, com.ToString("00"));
            }
            else
            {
                xclub.SetAttributeValue(ConstantXML.Club_Comite, comite);
            }

            xclub.SetAttributeValue(ConstantXML.Club_Ligue, ligue.ToString());
            return xclub;
        }

        public override string ToString()
        {
            return this.nomCourt;
        }

        /// <summary>
        /// Lecture des Clubs
        /// </summary>
        /// <param name="xelement">élément décrivant les Clubs</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Clubs</returns>

        public static ICollection<Club> LectureClubs(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Club> clubs = new List<Club>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Club))
            {
                Club club = new Club();
                club.LoadXml(xinfo);
                clubs.Add(club);
            }
            return clubs;
        }
    }
}
