using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Enum
{
    /// <summary>
    /// Enumération des différents type d'échange possoble entre un client-serveur
    /// </summary>
    public enum ServerCommandEnum
    {
        All = 0,

        #region CONNECTION

        DemandConnectionPesee = 100,
        AcceptConnectionPesee = 101,

        DemandConnectionCS = 110,
        AcceptConnectionCS = 111,

        DemandConnectionCOM = 120,
        AcceptConnectionCOM = 121,

        DemandConnectionTest = 130,
        AcceptConnectionTest = 131,

        #endregion

        #region ARBITRAGE

        DemandArbitrage = 200,
        EnvoieArbitrage = 201,
        UpdateArbitrage = 202,

        DemandCommissaires = 210,
        EnvoieCommissaires = 211,
        UpdateCommissaires = 212,

        DemandArbitres = 220,
        EnvoieArbitres = 221,
        UpdateArbitres = 222,

        DemandDelegues = 230,
        EnvoieDelegues = 231,
        UpdateDelegues = 232,

        #endregion

        #region CATES

        DemandCategories = 300,
        EnvoieCategories = 301,
        UpdateCategories = 302,

        DemandCatePoids = 310,
        EnvoieCatePoids = 311,
        UpdateCatePoids = 312,

        DemandGrade = 320,
        EnvoieGrade = 321,
        UpdateGrade = 322,

        DemandCateAge = 330,
        EnvoieCateAge = 331,
        UpdateCateAge = 332,

        #endregion

        #region STRUCTURE

        DemandStructures = 400,
        EnvoieStructures = 401,
        UpdateStructures = 402,

        DemandPays = 410,
        EnvoiePays = 411,
        UpdatePays = 412,

        DemandLigues = 420,
        EnvoieLigues = 421,
        UpdateLigues = 422,

        DemandComites = 430,
        EnvoieComites = 431,
        UpdateComites = 432,

        DemandClubs = 440,
        EnvoieClubs = 441,
        UpdateClubs = 442,

        DemandSecteurs = 450,
        EnvoieSecteurs = 451,
        UpdateSecteurs = 452,

        #endregion

        #region LOGO

        DemandLogos = 500,
        EnvoieLogos = 501,
        UpdateLogos = 502,

        #endregion

        #region ORGANISATION

        DemandOrganisation = 600,
        EnvoieOrganisation = 601,
        UpdateOrganisation = 602,

        DemandCompetitions = 610,
        EnvoieCompetitions = 611,
        UpdateCompetitions = 612,

        DemandEpreuves = 620,
        EnvoieEpreuves = 621,
        UpdateEpreuves = 622,

        DemandTapis = 630,
        EnvoieTapis = 631,
        UpdateTapis = 632,

        #endregion

        #region PARTICIPANTS

        DemandEquipes = 700,
        EnvoieEquipes = 701,
        UpdateEquipes = 702,

        DemandJudokas = 710,
        EnvoieJudokas = 711,
        UpdateJudokas = 712,

        DemandLicencies = 720,
        EnvoieLicencies = 721,
        UpdateLicencies = 722,

        #endregion

        #region DEROULEMENT

        DemandPhases = 800,
        EnvoiePhases = 801,
        UpdatePhases = 802,

        DemandCombats = 810,
        EnvoieCombats = 811,
        UpdateCombats = 812,
        UpdateTapisCombats = 813,

        #endregion

        #region TRAITEMENT

        ResultInscription = 930,
        TraiteInscription = 931,

        ResultCombats = 932,
        TraiteCombats = 933,

        ResultRencontres = 934,
        TraiteRencontres = 935,

        ResultTapis = 936,
        TraiteTapis = 937,

        NonTraite = 938,
       

        #endregion

    }
}
