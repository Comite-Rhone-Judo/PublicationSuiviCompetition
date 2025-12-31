
using KernelImpl.Internal;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;
using Tools.XML;

namespace KernelImpl.Noyau.Organisation
{
    /// <summary>
    /// Description des Epreuve Equipe
    /// </summary>
    public class Epreuve_Equipe : IEntityWithKey<int>
    {

        int IEntityWithKey<int>.EntityKey => id;
        public int id { get; set; }
        public string libelle { get; set; }
        public System.DateTime debut { get; set; }
        public System.DateTime fin { get; set; }
        public string remoteID { get; set; }
        public int competition { get; set; }
        public int ceintureMin { get; set; }
        public int ceintureMax { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }
        public int categorieAge { get; set; }

        public int epreuveRef { get; set; }

        public EpreuveEquipeTypeEnum type { get; set; }
        public void LoadXml(XElement xrencontre)
        {
            this.id = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_ID));
            this.libelle = XMLTools.LectureString(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_Libelle));
            this.debut = XMLTools.LectureDate(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_Debut), "ddMMyyyy", DateTime.Now);
            this.fin = XMLTools.LectureDate(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_Fin), "ddMMyyyy", DateTime.Now);
            this.remoteID = XMLTools.LectureString(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_RemoteID));
            this.competition = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_Competition));
            this.categorieAge = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_Categorie_Age));
            this.ceintureMin = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_GradeMin));
            this.ceintureMax = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_GradeMax));
            this.anneeMin = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_AnneeMin));
            this.anneeMax = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_AnneeMax));
            this.epreuveRef = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_EpreuveRef));
            this.type = (EpreuveEquipeTypeEnum) XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Epreuve_Equipe_Type));
        }

        public XElement ToXml()
        {
            XElement xrencontre = new XElement(ConstantXML.Epreuve_Equipe);

            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_ID, id);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_Libelle, libelle);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_Debut, debut.ToString("ddMMyyyy"));
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_Fin, fin.ToString("ddMMyyyy"));

            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_RemoteID, remoteID);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_Competition, competition);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_Categorie_Age, categorieAge);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_GradeMin, ceintureMin);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_GradeMax, ceintureMax);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_AnneeMin, anneeMin);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_AnneeMax, anneeMax);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_EpreuveRef, epreuveRef);
            xrencontre.SetAttributeValue(ConstantXML.Epreuve_Equipe_Type, type);

            return xrencontre;
        }

        /// <summary>
        /// Lecture des Epreuves
        /// </summary>
        /// <param name="xelement">élément décrivant les Epreuves</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Epreuves</returns>

        public static ICollection<Epreuve_Equipe> LectureEpreuveEquipes(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Epreuve_Equipe> epreuves = new List<Epreuve_Equipe>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Epreuve_Equipe))
            {
                Epreuve_Equipe epreuve = new Epreuve_Equipe();
                epreuve.LoadXml(xinfo);
                epreuves.Add(epreuve);
            }
            return epreuves;
        }
    }
}
