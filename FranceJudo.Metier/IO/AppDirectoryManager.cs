using FranceJudo.Core.Environment;
using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Reflection;
using FranceJudo.Metier.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace FranceJudo.Metier.IO
{
    /// <summary>
    /// Remplace ConstantFile et DirectoryHelper.
    /// Classe statique gérant la définition et la création de l'arborescence de l'application.
    /// </summary>
    public static class AppDirectoryManager
    {
        #region Constantes privées

        private const string kDirSaveCom = @"Save\Com";
        private const string kDirLogoFede = @"Logos\Fédé";
        private const string kDirLogoLigue = @"Logos\Ligue";
        private const string kDirLogoSponsor = @"Logos\sponsor";
        private const string kDirFlags = "flags";

        /*
        private const string kDirLogoFede = @"Logos\Fédé";
        private const string kDirLogoLigue = @"Logos\Ligue";
        private const string kDirLogoSponsor = @"Logos\sponsor";
        private const string kDirLogoCom = @"Logos\com";
        private const string kDirLogoTmp = @"Logos\tmp";
        private const string kDirParams = "Params";
        private const string kDirDataBd = @"data\bd";
        private const string kDirData = "data";
        private const string kDirWebcamTmp = "webcam";
        private const string kDirExport = "Export";
        private const string kDirExportSite = @"Export\site";
        private const string kDirExportStyle = @"Export\style";
        private const string kDirExportStyleSite = @"Export\style\site";
        private const string kDirExportStyleIcon = @"Export\style\icon";
        private const string kDirExportDiplome = @"Export\Diplome";
        private const string kDirVideo = "video";
        private const string kDirSon = "son";
        private const string kDirFlags = "flags";
        private const string kDirLog = "Log";
        private const string kDirSaveCs = @"Save\CS";
        private const string kDirSavePesee = @"Save\Pesee";
        private const string kDirJudoTV = @"FRANCE-JUDO\JudoTV";
        private const string kFileListeClubsXml = "ListeClubs.xml";
        private const string kFileInscriptionXml = "Insciption.xml";
        private const string kFileRecentFilesTxt = "RecentFiles.txt";
        private const string kFileJudokaXml = "Judoka.xml";
        */

        private const string kDirRessources = @"Ressources";
        private const string kDirRessourcesImg = @"Ressources\Images";
        #endregion

        // Liste stricte des répertoires à créer physiquement sur le disque
        private static List<string> _directoriesToCreate;

        #region Propriétés Statiques : Chemins de Répertoires (Remplace ConstantFile)

        public static string RessourcesDir { get; private set; }
        public static string RessoucesImgDir { get; private set; }

        public static string SaveCOMDir { get; private set; }
        public static string SaveDir { get; private set; }
        public static string Logo1Dir { get; private set; }
        public static string Logo2Dir { get; private set; }
        public static string Logo3Dir { get; private set; }

        public static string MediaFlagsDir { get; private set; }

        /*
        public static string Logo1_dir { get; private set; }
        public static string Logo2_dir { get; private set; }
        public static string Logo3_dir { get; private set; }
        public static string LogoCom_dir { get; private set; }
        public static string Logo_tmp_dir { get; private set; }
        public static string Params_dir { get; private set; }
        public static string BD_dir { get; private set; }
        public static string Data_dir { get; private set; }
        public static string Webcam_tmp_dir { get; private set; }
        public static string Export_dir { get; private set; }
        public static string ExportStyle_dir { get; private set; }
        public static string ExportStyleSite_dir { get; private set; }
        public static string ExportStyleIcon_dir { get; private set; }
        public static string ExportStyleDiplome_dir { get; private set; }
        public static string MediaVideo_dir { get; private set; }
        public static string MediaSon_dir { get; private set; }
        public static string MediaFlags_dir { get; private set; }
        public static string Log { get; private set; }
        public static string DirectorySave { get; private set; }
        public static string ExportJudoTV { get; private set; }
        public static string SaveCSDirectory { get; private set; }
        public static string SavePeseeDirectory { get; private set; }
        
        public static string ExportSite_dir { get; private set; }
        */

        #endregion

        #region Propriétés Statiques : Chemins de Fichiers Complets

        /*
        public static string Extra_ClubsFile { get; private set; }
        public static string Extra_InscriptionFile { get; private set; }
        public static string RecentFiles { get; private set; }
        public static string Extra_JudokasFile { get; private set; }
        */

        #endregion

        #region Propriétés Statiques : Noms de Fichiers Constants
        /*
        public static string FilePeseeAll { get; } = "les_pesee_all";
        public static string FileCSAll { get; } = "les_cs_all";
        public static string FileTapis { get; } = "le_tapis";
        public static string FileStructures { get; } = "les_structure";
        public static string FileLigues { get; } = "les_ligues";
        public static string FileComites { get; } = "les_comites";
        public static string FilePays { get; } = "les_pays";
        public static string FileClubs { get; } = "les_clubs";
        public static string FileCategories { get; } = "les_categories";
        public static string FileCateAges { get; } = "les_cate_ages";
        public static string FileCatePoids { get; } = "les_cate_poids";
        public static string FileGrades { get; } = "les_grades";
        public static string FileArbitrage { get; } = "les_arbitrage";
        public static string FileArbitres { get; } = "les_arbitres";
        public static string FileCommissaires { get; } = "les_commissaires";
        public static string FileDelegues { get; } = "les_delegues";
        public static string FileLogos { get; } = "les_logos";
        public static string FileOrganisation { get; } = "les_organisation";
        public static string FileCompetitions { get; } = "les_competitions";
        public static string FileEpreuves { get; } = "les_epreuves";
        public static string FileJudokas { get; } = "les_judokas";
        public static string FileEquipes { get; } = "les_equipes";
        public static string FileCombats { get; } = "les_combats";
        public static string FileRencontres { get; } = "les_rencontres";
        public static string FilePhases { get; } = "les_phases";
        public static string FileCombatsRealises { get; } = "les_combats_realises";
        public static string FileInscription { get; } = "les_inscriptions";
        public static string FileJudoTV { get; } = "params_judo_tv";
        public static string FileParams { get; } = "params_judo_tv";
        */
        public static string ExtensionXML { get; } = ".xml";
        public static string ExtensionTXT { get; } = ".txt";

        #endregion

        /// <summary>
        /// Remplace rigoureusement l'ancienne fonction InitDataDirectories.
        /// Initialise les chemins statiques, crée les dossiers physiques et extrait les ressources.
        /// ATTENTION : Doit être appelée une seule fois au démarrage de l'application !
        /// </summary>
        public static void Initialize(string dataPath, string _)
        {
            _directoriesToCreate = new List<string>();
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // 1. Définition et Enregistrement des chemins

            RessourcesDir = RegisterDirectory(dataPath, kDirRessources, createPhysicalFolder: true);
            RessoucesImgDir = RegisterDirectory(dataPath, kDirRessourcesImg, createPhysicalFolder: true);

            SaveDir = RegisterDirectory(dataPath, string.Empty, createPhysicalFolder: true);
            SaveCOMDir = RegisterDirectory(dataPath, kDirSaveCom, createPhysicalFolder: true);

            Logo1Dir = RegisterDirectory(dataPath, kDirLogoFede, createPhysicalFolder: true);
            Logo2Dir = RegisterDirectory(dataPath, kDirLogoLigue, createPhysicalFolder: true);
            Logo3Dir = RegisterDirectory(dataPath, kDirLogoSponsor, createPhysicalFolder: true);

            MediaFlagsDir = RegisterDirectory(dataPath, kDirFlags, createPhysicalFolder: true);


            /*

            BD_dir = RegisterDirectory(dataPath, kDirDataBd, createPhysicalFolder: false);
            Data_dir = RegisterDirectory(dataPath, kDirData, createPhysicalFolder: false);
            Export_dir = RegisterDirectory(dataPath, kDirExport, createPhysicalFolder: false);
            ExportStyle_dir = RegisterDirectory(dataPath, kDirExportStyle, createPhysicalFolder: false);
            ExportStyleSite_dir = RegisterDirectory(dataPath, kDirExportStyleSite, createPhysicalFolder: false);
            ExportStyleIcon_dir = RegisterDirectory(dataPath, kDirExportStyleIcon, createPhysicalFolder: false);
            ExportStyleDiplome_dir = RegisterDirectory(dataPath, kDirExportDiplome, createPhysicalFolder: false);
            ExportSite_dir = RegisterDirectory(dataPath, kDirExportSite, createPhysicalFolder: false);

            ExportJudoTV = RegisterDirectory(documentsPath, kDirJudoTV, createPhysicalFolder: false).Replace('\\', '/');

            DirectorySave = RegisterDirectory(dataPath, string.Empty, createPhysicalFolder: false);
            SaveCSDirectory = RegisterDirectory(dataPath, kDirSaveCs, createPhysicalFolder: false);
            SavePeseeDirectory = RegisterDirectory(dataPath, kDirSavePesee, createPhysicalFolder: false);
            SaveCOMDirectory = RegisterDirectory(dataPath, kDirSaveCom, createPhysicalFolder: false);

            Params_dir = RegisterDirectory(dataPath, kDirParams, createPhysicalFolder: false);
            Logo1_dir = RegisterDirectory(dataPath, kDirLogoFede, createPhysicalFolder: false);
            Logo2_dir = RegisterDirectory(dataPath, kDirLogoLigue, createPhysicalFolder: false);
            Logo3_dir = RegisterDirectory(dataPath, kDirLogoSponsor, createPhysicalFolder: false);
            Logo_tmp_dir = RegisterDirectory(dataPath, kDirLogoTmp, createPhysicalFolder: false);

            MediaSon_dir = RegisterDirectory(dataPath, kDirSon, createPhysicalFolder: false);
            MediaVideo_dir = RegisterDirectory(dataPath, kDirVideo, createPhysicalFolder: false);
            MediaFlags_dir = RegisterDirectory(dataPath, kDirFlags, createPhysicalFolder: false);

            // Chemins définis mais non créés physiquement
            LogoCom_dir = RegisterDirectory(dataPath, kDirLogoCom, createPhysicalFolder: false);
            Webcam_tmp_dir = RegisterDirectory(dataPath, kDirWebcamTmp, createPhysicalFolder: false);
            Log = RegisterDirectory(appPath, kDirLog, createPhysicalFolder: false);

            // Fichiers
            Extra_ClubsFile = Path.Combine(Data_dir, kFileListeClubsXml);
            Extra_InscriptionFile = Path.Combine(Data_dir, kFileInscriptionXml);
            RecentFiles = Path.Combine(dataPath, kFileRecentFilesTxt);
            Extra_JudokasFile = Path.Combine(dataPath, kFileJudokaXml);
            */

            // 2. Création stricte des dossiers
            foreach (var directory in _directoriesToCreate)
            {
                FileSystemHelper.CreateDirectorie(directory);
            }

            // 3. Logique conservée à l'identique pour l'extraction
            if (AppEnvironment.GetAppDirectory() == AppEnvironment.GetDataDirectory())
            {
                //return;
            }

            ExtractResources();
        }

        /// <summary>
        /// Extrait les ressources necessaires
        /// </summary>
        private static void ExtractResources()
        {
            // On ne boucle QUE sur les ressources du dossier des images du site
            foreach (string resourceName in MetierResources.Dictionary.FindByFolder(MetierResources.Folders.SiteImg))
            {
                var (targetDir, fileName) = ResolveResourceDestination(resourceName);

                if (string.IsNullOrEmpty(targetDir) || string.IsNullOrEmpty(fileName))
                    continue;

                string fullFilePath = Path.Combine(targetDir, fileName);

                // Appel de l'extracteur général (qui gère les droits d'accès et l'écriture disque)
                ResourceExtractor.ExtractToFile(MetierResources.Dictionary, resourceName, fullFilePath);
            }
        }

        private static (string targetDir, string fileName) ResolveResourceDestination(string resourceName )
        {
            /*
            if (resourceName.Contains(ResourceDictionnay.Export_site_style))
                return (ExportStyleSite_dir, resourceName.Replace(ResourceDictionnay.Export_site_style, string.Empty));

            if (resourceName.Contains(ResourceDictionnay.Export_Icon))
                return (ExportStyleIcon_dir, resourceName.Replace(ResourceDictionnay.Export_Icon, string.Empty));

            if (resourceName.Contains(ResourceDictionnay.Export_Diplome))
                return (ExportStyleDiplome_dir, resourceName.Replace(ResourceDictionnay.Export_Diplome, string.Empty));

            if (resourceName.Contains(ResourceDictionnay.Media_Son))
                return (MediaSon_dir, resourceName.Replace(ResourceDictionnay.Media_Son, string.Empty));

            if (resourceName.Contains(ResourceDictionnay.Media_Video))
                return (MediaVideo_dir, resourceName.Replace(ResourceDictionnay.Media_Video, string.Empty));

            if (resourceName.Contains(ResourceDictionnay.Media_Flags))
                return (MediaFlags_dir, resourceName.Replace(ResourceDictionnay.Media_Flags, string.Empty));

            */

            // TODO ici il faut faire attention car MetierResources.Folders.SiteImg est en relatif
            if (resourceName.Contains(MetierResources.Folders.SiteImg)) {

                // TODO A remplacer par ResourcePath.XXXX
                string root = MetierResources.Dictionary.GetFullName(MetierResources.Folders.SiteImg);
                root = root.EndsWith(".") ? root : root + ".";
                return (RessoucesImgDir, resourceName.Replace(root , string.Empty));
            }

            // Aucune des resources n'est nécessaire pour la generation (on ne travaille que avec des fichiers embarques)
            return (string.Empty, string.Empty);
        }

        private static string RegisterDirectory(string basePath, string subPath, bool createPhysicalFolder = true)
        {
            string fullPath = string.IsNullOrEmpty(subPath)
                ? basePath
                : Path.Combine(basePath, subPath);

            if (!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !fullPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                fullPath += Path.DirectorySeparatorChar;
            }

            if (createPhysicalFolder)
            {
                _directoriesToCreate.Add(fullPath);
            }

            return fullPath;
        }

        public static string GetExportDir(string racine)
        {
            return Path.Combine(racine, "FRANCE-JUDO");
        }
    }
}