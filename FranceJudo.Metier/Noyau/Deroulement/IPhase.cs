using FranceJudo.Metier.XML;
using System;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    /// <summary>
    /// Description des Phases
    /// </summary>
    public interface IPhase : IXMLSerializable
    {
        public int id { get; set; }
        public string libelle { get; set; }
        public int typePhase { get; set; }
        public int nbPoules { get; set; }
        public int niveauRepechage { get; set; }
        public bool bresilien { get; set; }
        public int precedent { get; set; }
        public int suivant { get; set; }
        public Nullable<int> epreuve { get; set; }
        public int niveauRepeches { get; set; }
        public int etat { get; set; }
        public int nbCombatsFinalistes { get; set; }
        public int nbCombatsTotal { get; set; }
        public int nbJudoka { get; set; }
        public int nbQualifieMin { get; set; }
        public int nbQualifieMax { get; set; }
        public int nbJudokaPoule { get; set; }
        public bool isEquipe { get; set; }
        public bool barrage3 { get; set; }
        public bool barrage5 { get; set; }
        public bool barrage7 { get; set; }
        public int ecartement { get; set; }
        public Nullable<DateTime> date { get; set; }
        public int niveauRepechage2 { get; set; }
        public int niveauRepeches2 { get; set; }

        public Organisation.i_vue_epreuve_interface GetVueEpreuve(IJudoData DC);

    }
}
