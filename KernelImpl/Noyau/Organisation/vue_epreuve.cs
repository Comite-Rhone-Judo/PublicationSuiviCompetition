
using System;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;

namespace KernelImpl.Noyau.Organisation
{
    public class vue_epreuve : i_vue_epreuve_interface
    {
        public int id { get; set; }
        public string nom { get; set; }
        public DateTime debut { get; set; }
        public DateTime fin { get; set; }
        public string remoteID { get; set; }
        public int competition { get; set; }
        public int categoriePoids { get; set; }
        public int poidsMin { get; set; }
        public int poidsMax { get; set; }
        public int ceintureMin { get; set; }
        public int ceintureMax { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }
        private int _sexe;
        public int sexe
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
                _sexe = (int)_sexeEnum;
            }
        }
        public int categorieAge { get; set; }
        public string lib_sexe { get; set; }
        public string nom_catepoids { get; set; }
        public string remoteId_catepoids { get; set; }
        public string nom_cateage { get; set; }
        public string ordre { get; set; }
        public string remoteId_cateage { get; set; }
        public Nullable<int> id_epreuve_equipe { get; set; }
        public string lib_epreuve_equipe { get; set; }
        public Nullable<int> phase1 { get; set; }
        public Nullable<int> phase2 { get; set; }
        public string nom_compet { get; set; }


        public vue_epreuve(Epreuve epreuve, JudoData DC)
        {
            id = epreuve.id;
            nom = epreuve.nom;
            debut = epreuve.debut;
            fin = epreuve.fin;
            remoteID = epreuve.remoteID;
            competition = epreuve.competition;
            categoriePoids = epreuve.categoriePoids;
            poidsMin = epreuve.poidsMin;
            poidsMax = epreuve.poidsMax;
            ceintureMin = epreuve.ceintureMin;
            ceintureMax = epreuve.ceintureMax;
            anneeMin = epreuve.anneeMin;
            anneeMax = epreuve.anneeMax;
            sexe = epreuve.sexe;
            categorieAge = epreuve.categorieAge;

            lib_sexe = epreuve.sexeEnum.ToString();

            Categories.CategoriePoids c_poids = DC.Categories.CPoids.FirstOrDefault(o => o.id == epreuve.categoriePoids);

            nom_catepoids = c_poids != null ? c_poids.nom : epreuve.nom;
            remoteId_catepoids = c_poids != null ? c_poids.remoteId : "";

            Categories.CategorieAge c_age = DC.Categories.CAges.FirstOrDefault(o => o.id == epreuve.categorieAge);

            nom_cateage = c_age != null ? c_age.nom : "";
            ordre = c_age != null ? c_age.ordre : "0";
            remoteId_cateage = c_age != null ? c_age.remoteId : "";

            Epreuve_Equipe ep = DC.Organisation.EpreuveEquipes.FirstOrDefault(o => o.id == epreuve.epreuve_equipe);

            id_epreuve_equipe = ep != null ? ep.id : 0;
            lib_epreuve_equipe = ep != null ? ep.libelle : "";

            //phase1 = ;
            //phase2 = ;

            Competition compet = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);

            nom_compet = compet != null ? compet.nom : "";
        }



        public override string ToString()
        {
            return nom_cateage + " " + lib_sexe + " " + nom_catepoids;
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
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Sexe, sexeEnum.ToString());

            xepreuve.SetAttributeValue(ConstantXML.Epreuve_CateAge_Nom, nom_cateage);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_AnneeMin, anneeMin);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_AnneeMax, anneeMax);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_CateAge_RemoteId, remoteId_cateage);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_CatePoids_Nom, nom_catepoids);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_PoidsMin, poidsMin);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_PoidsMax, poidsMax);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_CatePoids_RemoteId, remoteId_catepoids);
            xepreuve.SetAttributeValue(ConstantXML.Vue_Epreuve_Nom_Competition, nom_compet);

            return xepreuve;
        }
    }
}
