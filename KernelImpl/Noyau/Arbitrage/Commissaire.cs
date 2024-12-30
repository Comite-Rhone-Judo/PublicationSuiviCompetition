
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Arbitrage
{
    public class Commissaire
    {
        public string licence { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public System.DateTime naissance { get; set; }
        private bool _sexe;
        public bool sexe
        {
            get
            {
                return _sexe;
            }
            set
            {
                _sexe = value;
                _sexeEnum = new EpreuveSexe(_sexe);
            }
        }

        private EpreuveSexe _sexeEnum;
        public EpreuveSexe sexeEnum
        {
            get
            {
                return _sexeEnum;
            }
            set
            {
                _sexeEnum = value;
                _sexe = (bool) _sexeEnum;
            }
        }
        public int categorie { get; set; }
        public bool modification { get; set; }
        public string club { get; set; }
        public string comite { get; set; }
        public string ligue { get; set; }
        public int pays { get; set; }
        public string clubID { get; set; }
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
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Commissaire_ID));
            //this.niveau = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Commissaire_Niveau));

            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_Nom));
            this.prenom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_Prenom));
            this.licence = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_Licence));

            this.club = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_Club));
            this.comite = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_Comite));
            this.ligue = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_Ligue));

            this.naissance = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Commissaire_Naissance), "ddMMyyyy", DateTime.Now);

            this.sexeEnum = new EpreuveSexe(XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_Sexe)));

            this.modification = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Commissaire_Modification));
            this.estResponsable = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Commissaire_EstResponsable));
            this.present = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Commissaire_Present));

            this.remoteID = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Commissaire_RemoteID));
        }


        public XElement ToXml()
        {
            XElement xcommissaire = new System.Xml.Linq.XElement(ConstantXML.Commissaire);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_ID, id);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Licence, licence);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Nom, nom);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Prenom, prenom);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Naissance, naissance.ToString("ddMMyyyy"));
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Sexe, sexeEnum.ToString());
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Modification, modification);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_RemoteID, remoteID);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Club, club);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_EstResponsable, estResponsable);
            xcommissaire.SetAttributeValue(ConstantXML.Commissaire_Present, present);

            return xcommissaire;
        }

        /// <summary>
        /// Lecture des Commissaire
        /// </summary>
        /// <param name="xelement">élément décrivant les Commissaire</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Categories Age</returns>

        public static ICollection<Commissaire> LectureCommissaire(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Commissaire> commissaires = new List<Commissaire>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Commissaire))
            {
                Commissaire commissaire = new Commissaire();
                commissaire.LoadXml(xinfo);
                commissaires.Add(commissaire);
            }
            return commissaires;
        }
    }
}
