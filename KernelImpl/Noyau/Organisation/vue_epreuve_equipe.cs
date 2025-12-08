
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;

namespace KernelImpl.Noyau.Organisation
{
    public class vue_epreuve_equipe : i_vue_epreuve_interface, IIdEntity<int>
    {
        public int id { get; set; }
        public string nom { get; set; }
        public EpreuveEquipeTypeEnum type { get; set; }
        public int epreuveRef { get; set; }
        public System.DateTime debut { get; set; }
        public System.DateTime fin { get; set; }
        public string remoteID { get; set; }
        public int competition { get; set; }
        public int ceintureMin { get; set; }
        public int ceintureMax { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }
        public int categorieAge { get; set; }
        public string nom_cateage { get; set; }
        public string ordre { get; set; }
        public string remoteId_cateage { get; set; }
        public Nullable<int> phase1 { get; set; }
        public Nullable<int> phase2 { get; set; }
        public string nom_compet { get; set; }
        public CompetitionDisciplineEnum discipline_competition { get; set; }

        private string _lib_sexe = "";
        public string lib_sexe
        {
            get { return _lib_sexe; }
            set
            {
                if (_lib_sexe != value)
                {
                    _lib_sexe = value;
                }
            }
        }

        private string _nom_catepoids = "";
        public string nom_catepoids
        {
            get { return _nom_catepoids; }
            set
            {
                if (_nom_catepoids != value)
                {
                    _nom_catepoids = value;
                }
            }
        }


        public vue_epreuve_equipe(Epreuve_Equipe epreuve, JudoData DC)
        {
            id = epreuve.id;
            nom = epreuve.libelle;
            type = epreuve.type;
            epreuveRef = epreuve.epreuveRef;
            debut = epreuve.debut;
            fin = epreuve.fin;
            remoteID = epreuve.remoteID;
            competition = epreuve.competition;
            ceintureMin = epreuve.ceintureMin;
            ceintureMax = epreuve.ceintureMax;
            anneeMin = epreuve.anneeMin;
            anneeMax = epreuve.anneeMax;
            categorieAge = epreuve.categorieAge;

            Categories.CategorieAge c_age = DC.Categories.CAges.FirstOrDefault(o => o.id == epreuve.categorieAge);

            nom_cateage = c_age != null ? c_age.nom : "";
            ordre = c_age != null ? c_age.ordre : "0";
            remoteId_cateage = c_age != null ? c_age.remoteId : "";

            //phase1 = ;
            //phase2 = ;

            Competition compet = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);

            nom_compet = compet != null ? compet.nom : "";
            discipline_competition = compet != null ? compet.disciplineId : CompetitionDisciplineEnum.Judo;
        }



        public XElement ToXml(JudoData DC)
        {
            XElement xepreuve = new XElement(ConstantXML.Epreuve);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Categorie_Age, categorieAge.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Nom, nom.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Grade_Min, ceintureMin.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Grade_Max, ceintureMax.ToString());
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_RemoteID, remoteID);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_ID, id);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Competition, competition);

            xepreuve.SetAttributeValue(ConstantXML.Epreuve_CateAge_Nom, nom_cateage);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_AnneeMin, anneeMin);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_AnneeMax, anneeMax);
            xepreuve.SetAttributeValue(ConstantXML.Epreuve_CateAge_RemoteId, remoteId_cateage);

            List<Epreuve> epreuves = DC.Organisation.Epreuves.Where(o => o.epreuve_equipe == this.id).ToList();
            EpreuveSexe sexe = new EpreuveSexe(EpreuveSexeEnum.Feminine);
            
            if (epreuves.Count(o => o.sexe == 1) > 0 && epreuves.Count(o => o.sexe == 0) > 0)
            {
                sexe = new EpreuveSexe(EpreuveSexeEnum.Mixte);
            }
            else if (epreuves.Count(o => o.sexe == 1) > 0 && epreuves.Count(o => o.sexe == 0) == 0)
            {
                sexe = new EpreuveSexe(EpreuveSexeEnum.Feminine);
            }
            else if (epreuves.Count(o => o.sexe == 1) == 0 && epreuves.Count(o => o.sexe == 0) > 0)
            {
                sexe = new EpreuveSexe(EpreuveSexeEnum.Masculin);
            }

            xepreuve.SetAttributeValue(ConstantXML.Epreuve_Sexe, sexe.ToString());

            return xepreuve;
        }
    }
}
