namespace Tools.Struct
{
    public class ListPhaseStruct
    {
        public string id { get; set; }

        public string nom { get; set; }
        public int nbjudoka { get; set; }
        public int typePhase { get; set; }
        public int ordre { get; set; }
        public int nbSortants { get; set; }

        public int nbPoules { get; set; }
        public int nbJudokaPoule { get; set; }
        public int nbQualifiesMin { get; set; }
        public int nbQualifiesMax { get; set; }
        public bool poule_classement { get; set; }

        public bool link_tab { get; set; }


        public int niveauRepechage { get; set; }
        public int niveauRepeches { get; set; }
        public bool barrage3 { get; set; }
        public bool barrage5 { get; set; }
        public bool barrage7 { get; set; }
        //public int bresilien { get; set; }

        public ListPhaseStruct Copy()
        {
            ListPhaseStruct res = new ListPhaseStruct();

            res.id = this.id;
            res.nom = this.nom;
            res.nbjudoka = this.nbjudoka;
            res.typePhase = this.typePhase;
            res.ordre = this.ordre;

            res.nbSortants = this.nbSortants;
            res.nbPoules = this.nbPoules;
            res.nbJudokaPoule = this.nbJudokaPoule;
            res.nbQualifiesMin = this.nbQualifiesMin;
            res.nbQualifiesMax = this.nbQualifiesMax;
            res.niveauRepechage = this.niveauRepechage;
            res.niveauRepeches = this.niveauRepeches;
            res.barrage3 = this.barrage3;
            res.barrage5 = this.barrage5;
            res.barrage7 = this.barrage7;

            return res;
        }

        public bool Egale(ListPhaseStruct other)
        {
            bool egale = true;

            egale = egale && other.id == this.id;
            egale = egale && other.nom == this.nom;
            egale = egale && other.nbjudoka == this.nbjudoka;
            egale = egale && other.typePhase == this.typePhase;
            egale = egale && other.ordre == this.ordre;

            egale = egale && other.nbSortants == this.nbSortants;
            egale = egale && other.nbPoules == this.nbPoules;
            egale = egale && other.nbJudokaPoule == this.nbJudokaPoule;
            egale = egale && other.nbQualifiesMin == this.nbQualifiesMin;
            egale = egale && other.nbQualifiesMax == this.nbQualifiesMax;
            egale = egale && other.niveauRepechage == this.niveauRepechage;
            egale = egale && other.niveauRepeches == this.niveauRepeches;
            egale = egale && other.barrage3 == this.barrage3;
            egale = egale && other.barrage5 == this.barrage5;
            egale = egale && other.barrage7 == this.barrage7;

            return egale;
        }
    }
}
