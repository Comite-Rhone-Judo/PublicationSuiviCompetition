using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Outils;

namespace Tools.Enum
{ 
    /// <summary>
    /// Enumération des constants pour la gestion des fichier
    /// </summary>
    public class ConstantFile
    {
        public static readonly string Logo1_dir = OutilsTools.GetDataDirectory() + @"Logos/Fédé/";
        public static readonly string Logo2_dir = OutilsTools.GetDataDirectory() + @"Logos/Ligue/";
        public static readonly string Logo3_dir = OutilsTools.GetDataDirectory() + @"Logos/sponsor/";
        public static readonly string LogoCom_dir = OutilsTools.GetDataDirectory() + @"Logos/com/";
        public static readonly string Logo_tmp_dir = OutilsTools.GetDataDirectory() + @"Logos/tmp/";

        public static readonly string Params_dir = OutilsTools.GetDataDirectory() + @"Params/";
        public static readonly string BD_dir = OutilsTools.GetDataDirectory() + @"data/bd/";
        public static readonly string Data_dir = OutilsTools.GetDataDirectory() + @"data/";
        public static readonly string Extra_ClubsFile = ConstantFile.Data_dir + "ListeClubs.xml";
        public static readonly string Extra_InscriptionFile = ConstantFile.Data_dir + "Insciption.xml";

        public static readonly string Webcam_tmp_dir = OutilsTools.GetDataDirectory() + @"webcam/";
        public static readonly string Export_dir = OutilsTools.GetDataDirectory() + @"Export/";
        public static readonly string ExportStyle_dir = ConstantFile.Export_dir + "style/";
        public static readonly string ExportStyleSite_dir = ConstantFile.Export_dir + "style/site/";
        public static readonly string ExportStyleIcon_dir = ConstantFile.Export_dir + "style/icon/";
        public static readonly string ExportStyleDiplome_dir = ConstantFile.Export_dir + "Diplome/";

        public static readonly string MediaVideo_dir = OutilsTools.GetDataDirectory() + @"video/";
        public static readonly string MediaSon_dir = OutilsTools.GetDataDirectory() + @"son/";
        public static readonly string MediaFlags_dir = OutilsTools.GetDataDirectory() + @"flags/";

        public static readonly string Log = OutilsTools.GetAppDirectory() + @"Log/";

        public static string DirectorySave = OutilsTools.GetDataDirectory();
        public static readonly string RecentFiles = OutilsTools.GetDataDirectory() + @"RecentFiles.txt";

        public static string ExportSite_dir =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace(@"\", "/") + @"/FRANCE-JUDO/site/";
        public static string ExportJudoTV =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace(@"\", "/") + @"/FRANCE-JUDO/JudoTV/";

        public static readonly string Extra_JudokasFile = OutilsTools.GetDataDirectory() + "Judoka.xml";
        

        public static string SaveCSDirectory = OutilsTools.GetDataDirectory() + "Save/CS";
        public static string SavePeseeDirectory = OutilsTools.GetDataDirectory() + "Save/Pesee";
        public static string SaveCOMDirectory = OutilsTools.GetDataDirectory() + "Save/Com";


        public static string FilePeseeAll = "les_pesee_all";
        public static string FileCSAll = "les_cs_all";

        public static string FileTapis = "le_tapis";

        public static string FileStructures = "les_structure";
        public static string FileLigues = "les_ligues";
        public static string FileComites = "les_comites";
        public static string FilePays = "les_pays";
        public static string FileClubs = "les_clubs";

        public static string FileCategories = "les_categories";
        public static string FileCateAges = "les_cate_ages";
        public static string FileCatePoids = "les_cate_poids";
        public static string FileGrades = "les_grades";

        public static string FileArbitrage = "les_arbitrage";
        public static string FileArbitres = "les_arbitres";
        public static string FileCommissaires = "les_commissaires";
        public static string FileDelegues = "les_delegues";

        public static string FileLogos = "les_logos";

        public static string FileOrganisation = "les_organisation";
        public static string FileCompetitions = "les_competitions";
        public static string FileEpreuves = "les_epreuves";

        public static string FileJudokas = "les_judokas";
        public static string FileEquipes = "les_equipes";


        public static string FileCombats = "les_combats";
        public static string FilePhases = "les_phases";


        public static string FileCombatsRealises = "les_combats_realises";
        
        public static string FileInscription = "les_inscriptions";
        public static string FileJudoTV = "params_judo_tv";
        public static string FileParams = "params_judo_tv";

        public static string ExtensionXML = ".xml";
        public static string ExtensionTXT = ".txt";
    }
}
