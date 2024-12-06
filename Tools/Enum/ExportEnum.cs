namespace Tools.Enum
{
    /// <summary>
    /// Enumération des type d'export
    /// </summary>
    public enum ExportEnum
    {
        Participants = 1,
        Pesee = 2,
        Poule_Repartition = 3,
        Poule_Competition1 = 4,
        Poule_Resultat = 5,
        Poule_Resultat_Shiai = 34,
        Tableau_Competition = 6, // Poule classique
        Poule_Competition2 = 19, // Poule verticale
        Tableau_Resultat = 7,
        ClassementPoule = 8,
        ClassementTableau = 9,
        ClassementFinal = 10,
        FeuilleCombat = 11,

        Rapport_Sportif = 15,
        Rapport_Admin = 16,
        Tableau_Competition_Repechage = 17,
        Excel = 18,

        Rapport_Selection = 20,
        RelationGrCh = 21,
        Resultat = 22,
        Competition = 23,
        RapportErreurJudoka = 24,
        PeseeEquipe = 25,
        ParticipantsEquipe = 26,
        Diplome = 27,
        Dispatch = 28,
        Participation = 29,
        FeuilleCombatTableau = 30,
        FeuilleCombatPoule = 31,
        CompetitionExtranet = 32,
        CompetitionXML = 33,

        //SITE
        Site_Menu = 100,
        Site_Index = 101,
        Site_QrCode = 102,
        Site_Tapis1 = 103,
        Site_Tapis2 = 104,
        Site_Tapis4 = 105,
        Site_ListTapis = 106,
        Site_FeuilleCombat = 107,
        Site_Poule_Resultat = 108,
        Site_Tableau_Competition = 109,
        Site_ClassementFinal = 110,
        Site_FeuilleCombatTapis = 111,
        Site_AffectationTapis = 112,
        Site_Checksum = 113,
        Site_MenuClassement = 114,
        Site_MenuAvancement = 115,
        Site_MenuProchainCombats = 116,
        Site_Participants = 117
    }
}
