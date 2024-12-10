
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Organisation
{
    /// <summary>
    /// Description des Epreuves
    /// </summary>
    public class Epreuve
    {

        public int id { get; set; }
        public string nom { get; set; }
        public System.DateTime debut { get; set; }
        public System.DateTime fin { get; set; }
        public string remoteID { get; set; }
        public int competition { get; set; }
        public int categoriePoids { get; set; }
        public int poidsMin { get; set; }
        public int poidsMax { get; set; }
        public int ceintureMin { get; set; }
        public int ceintureMax { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }
        public int sexe { get; set; }
        public int categorieAge { get; set; }
        public Nullable<int> epreuve_equipe { get; set; }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_ID));

            string name = "";
            if (xinfo.Attribute(ConstantXML.Epreuve_Nom) != null)
            {
                name += xinfo.Attribute(ConstantXML.Epreuve_Nom).Value;
            }
            else
            {
                if (xinfo.Attribute(ConstantXML.Epreuve_CateAge_Nom) != null)
                {
                    name += " " + xinfo.Attribute(ConstantXML.Epreuve_CateAge_Nom).Value;
                }

                if (xinfo.Attribute(ConstantXML.Epreuve_CatePoids_Nom) != null)
                {
                    name += " " + xinfo.Attribute(ConstantXML.Epreuve_CatePoids_Nom).Value;
                }

                if (xinfo.Attribute(ConstantXML.Epreuve_Sexe) != null)
                {
                    name += " " + xinfo.Attribute(ConstantXML.Epreuve_Sexe).Value;
                }
            }


            this.nom = name;
            //(xinfo.Attribute(ConstantXML.Epreuve_CateAge_Nom) != null ? xinfo.Attribute(ConstantXML.Epreuve_CateAge_Nom).Value : "") + " " +
            // (xinfo.Attribute(ConstantXML.Epreuve_CatePoids_Nom) != null ? xinfo.Attribute(ConstantXML.Epreuve_CatePoids_Nom).Value : "") + " " +
            //xinfo.Attribute(ConstantXML.Epreuve_Sexe).Value;
            this.remoteID = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Epreuve_RemoteID));
            this.categoriePoids = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_Categorie_Poids));
            this.competition = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_Competition));
            this.poidsMin = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_PoidsMin));
            this.poidsMax = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_PoidsMax));
            this.ceintureMin = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_Grade_Min));
            this.ceintureMax = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_Grade_Max));
            this.anneeMin = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_AnneeMin));
            this.anneeMax = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_AnneeMax));
            this.categorieAge = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Epreuve_Categorie_Age));
            this.sexe = (xinfo.Attribute(ConstantXML.Epreuve_Sexe).Value == "M" ? (int) EpreuveSexeEnum.Masculin : (int) EpreuveSexeEnum.Feminine);
            this.epreuve_equipe = XMLTools.LectureNullableInt(xinfo.Attribute(ConstantXML.Epreuve_EquipeEP));
        }

        public XElement ToXml(JudoData DC)
        {
            XElement xepreuve = new XElement(ConstantXML.Epreuve);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Categorie_Age, categorieAge.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Categorie_Poids, categoriePoids.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Nom, nom.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Grade_Min, ceintureMin.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Grade_Max, ceintureMax.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_RemoteID, remoteID);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_ID, id);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Competition, competition);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Sexe, sexe == 0 ? "M" : "F");
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_AnneeMin, anneeMin);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_AnneeMax, anneeMax);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_PoidsMin, poidsMin);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_PoidsMax, poidsMax);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_EquipeEP, epreuve_equipe);

            //CateAge
            if (DC.Categories.CAges != null)
            {
                Categories.CategorieAge cateAge = DC.Categories.CAges.FirstOrDefault(o => o.id == categorieAge);
                if (cateAge != null)
                {
                    xepreuve.SetAttributeValue(ConstantXML.Epreuve_CateAge_Nom, cateAge.nom);
                    xepreuve.SetAttributeValue(ConstantXML.Epreuve_CateAge_RemoteId, cateAge.remoteId);
                }
            }

            if (DC.Categories.CPoids != null)
            {
                //CatePoids
                Categories.CategoriePoids catePoids = DC.Categories.CPoids.FirstOrDefault(o => o.id == categoriePoids);
                if (catePoids != null)
                {
                    xepreuve.SetAttributeValue(ConstantXML.Epreuve_CatePoids_Nom, catePoids.nom);
                    xepreuve.SetAttributeValue(ConstantXML.Epreuve_CatePoids_RemoteId, catePoids.remoteId);
                }
            }
            return xepreuve;
        }


        public override string ToString()
        {
            //Competition compet = competitions.FirstOrDefault(o => o.id == competition);

            return this.nom; //+ " " + (this.sexe == 0 ? "M" : "F");// cateages.FirstOrDefault(o => o.id == this.categorieAge).nom + " " +
                             //(this.sexe == 0 ? "M" : "F") + (compet != null ? (" (" + compet.nom + ")") : "");
        }

        public string ToString(ICollection<Competition> competitions)
        {
            Competition compet = competitions.FirstOrDefault(o => o.id == competition);

            return this.nom + " " + (compet != null ? (" (" + compet.nom + ")") : "");
        }

        /// <summary>
        /// Lecture des Epreuves
        /// </summary>
        /// <param name="xelement">élément décrivant les Epreuves</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Epreuves</returns>

        public static ICollection<Epreuve> LectureEpreuves(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Epreuve> epreuves = new List<Epreuve>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Epreuve))
            {
                Epreuve epreuve = new Epreuve();
                epreuve.LoadXml(xinfo);
                epreuves.Add(epreuve);
            }
            return epreuves;
        }
    }
}