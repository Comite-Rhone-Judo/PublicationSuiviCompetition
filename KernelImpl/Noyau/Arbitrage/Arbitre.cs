
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Arbitrage
{
    public class Arbitre
    {
        public string licence { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public System.DateTime naissance { get; set; }
        public bool sexe { get; set; }
        public bool modification { get; set; }
        public int clubId { get; set; }
        public string club { get; set; }
        public string comite { get; set; }
        public string ligue { get; set; }
        public int pays { get; set; }
        public int niveau { get; set; }
        public bool present { get; set; }
        public string remoteID { get; set; }
        public int id { get; set; }
        public bool estResponsable { get; set; }

        /// <summary>
        /// Charge l'instance à partir d'un noeud XML
        /// </summary>
        /// <param name="xinfo"></param>

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Arbitre_ID));
            this.niveau = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Arbitre_Niveau));

            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_Nom));
            this.prenom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_Prenom));
            this.licence = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_Licence));

            this.club = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_Club));
            this.comite = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_Comite));
            this.ligue = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_Ligue));

            this.naissance = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Arbitre_Naissance), "ddMMyyyy", DateTime.Now);

            this.sexeEnum =  new EpreuveSexe(XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_Sexe)));

            this.modification = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Arbitre_Modification));
            this.estResponsable = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Arbitre_EstResponsable));
            this.present = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Arbitre_Present));

            this.remoteID = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Arbitre_RemoteID));
        }


        public XElement ToXml()
        {
            XElement xarbitre = new XElement(ConstantXML.Arbitre);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_ID, id);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Licence, licence);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Nom, nom.ToUpper());
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Prenom, OutilsTools.FormatPrenom(prenom));
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Naissance, naissance.ToString("ddMMyyyy"));
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Sexe, sexeEnum.ToString());
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Modification, modification);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_RemoteID, remoteID);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Club, club);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Comite, comite);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Ligue, ligue);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Niveau, niveau);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_EstResponsable, estResponsable);
            xarbitre.SetAttributeValue(ConstantXML.Arbitre_Present, present);

            return xarbitre;
        }


        /// <summary>
        /// Lecture des Arbitre
        /// </summary>
        /// <param name="xelement">élément décrivant les Arbitre</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Categories Age</returns>

        public static ICollection<Arbitre> LectureArbitre(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Arbitre> arbitres = new List<Arbitre>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Arbitre))
            {
                Arbitre arbitre = new Arbitre();
                arbitre.LoadXml(xinfo);
                arbitres.Add(arbitre);
            }
            return arbitres;
        }
    }
}
