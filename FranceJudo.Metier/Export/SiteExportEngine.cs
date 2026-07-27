using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Reflection;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Resources;
using FranceJudo.Metier.Site;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl; // Pour le moteur XSLT

namespace FranceJudo.Metier.Export
{
    public static class SiteExportEngine
    {
        private static string _defaultCompetition = null;

        public static string DefaultCompetition
        {
            get { return _defaultCompetition; }
            set { _defaultCompetition = value; }
        }

        #region 1. REGISTRE CENTRAL DE CONFIGURATION
        // ==============================================================================
        // C'est le SEUL endroit à modifier si vous ajoutez un nouvel export à l'avenir !
        // Format : { ExportEnum, (NomDuFichierGenere, NomDeLaRessourceXslt) }
        // ==============================================================================
        private static readonly Dictionary<ExportEnum, (string FileName, string XsltName)> _exportRegistry =
            new Dictionary<ExportEnum, (string, string)>
        {
                // Enum = (FileName, XSL)
            { ExportEnum.Site_Index,               ("index", "index_site.xslt") },
            { ExportEnum.Site_FeuilleCombat,       ("feuille_combats", "feuille_matchs_site.xslt") },
            { ExportEnum.Site_FeuilleCombatTapis,  ("se_prepare", "feuille_matchs_site.xslt") },
            { ExportEnum.Site_Poule_Resultat,      ("poules_resultats", "feuille_resultat_site.xslt") },
            { ExportEnum.Site_Tableau_Competition, ("tableau_competition", "feuille_competition_site.xslt") },
            { ExportEnum.Site_ClassementFinal,     ("classement_final", "classement_final_site.xslt") },
            { ExportEnum.Site_AffectationTapis,    ("affectation_tapis", "affectation_tapis_site.xslt") },
            { ExportEnum.Site_MenuAvancement,      ("avancement", "avancement_site.xslt") },
            { ExportEnum.Site_MenuClassement,      ("classement", "classement_site.xslt") },
            { ExportEnum.Site_MenuProchainCombats, ("prochains_combats", "prochains_combats_site.xslt") },
            { ExportEnum.Site_FooterScript,        ("footer_script", "footer_script_site.xslt") },
            { ExportEnum.Site_Engagements,         ("groupe_engagements", "groupe_engagements_site.xslt") },
            { ExportEnum.Site_MenuEngagements,     ("engagements", "engagements_site.xslt") },
            { ExportEnum.Site_MenuStatistiques,    ("statistiques", "statistiques_site.xslt") },
            { ExportEnum.Site_Statistiques,        ("groupe_statistiques", "groupe_statistiques_site.xslt") },
            { ExportEnum.Site_Interne_EcranAppel,  ("ecran", "ecrans_appel_site.xslt") }
        };
        #endregion

        #region 2. RÉSOLUTION DES NOMS ET CHEMINS

        /// <summary>
        /// Retourne le nom nettoyé d'un fichier d'export
        /// </summary>
        public static string GetFileName(ExportEnum type)
        {
            if (!_exportRegistry.TryGetValue(type, out var config) || string.IsNullOrEmpty(config.FileName))
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, $"Le nom de fichier pour le type d'export '{type}' n'est pas défini dans le registre.");
            }

            string result = config.FileName.Replace(' ', '_');
            char[] invalidPathChars = Path.GetInvalidFileNameChars();

            foreach (char invalid in invalidPathChars)
            {
                result = result.Replace(invalid, '_');
            }

            return result;
        }

        /// <summary>
        /// Nettoie un nom de fichier en remplaçant les caractères séparateurs de dossiers par des underscores.
        /// </summary>
        /// <param name="fileName">Le nom du fichier à nettoyer</param>
        /// <returns>Le nom du fichier nettoyé</returns>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            // On peut facilement ajouter d'autres caractères interdits ici plus tard
            return fileName.Replace("/", "_");
        }

        /// <summary>
        /// Récupère le nom de fichier associé à un ExportEnum et le nettoie directement.
        /// </summary>
        /// <param name="exportType">Le type d'export</param>
        /// <returns>Le nom du fichier nettoyé</returns>
        public static string GetSanitizedFileName(ExportEnum exportType)
        {
            string rawFileName = GetFileName(exportType);
            return SanitizeFileName(rawFileName);
        }

        /// <summary>
        /// Retourne le chemin complet de ressource pour une feuille de style d'export
        /// </summary>
        /// <param name="type">Le type d'export</param>
        /// <returns>Le chemin du fichier XSLT</returns>
        private static string GetXsltResourcePath(ExportEnum type)
        {
            if (!_exportRegistry.TryGetValue(type, out var config) || string.IsNullOrEmpty(config.XsltName))
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, $"Le fichier XSLT pour le type d'export '{type}' n'est pas défini dans le registre.");
            }

            return ResourcePath.Combine(MetierResources.Folders.SiteXslt, config.XsltName);
        }

        #endregion

        #region 3. GESTION DES RESSOURCES PHYSIQUES (Images, CSS, JS)

        /// <summary>
        /// Exporte les images embarquées pour un site donné
        /// </summary>
        /// <typeparam name="T">Le type de structure physique</typeparam>
        /// <param name="regenere">Indique si les ressources doivent être régénérées</param>
        /// <param name="addCustom">Indique si les logos personnalisés doivent être ajoutés</param>
        /// <param name="structSite">Le générateur d'URL pour la structure du site</param>
        /// <returns>La liste des fichiers exportés</returns>
        public static List<string> ExportEmbeddedImg<T>(bool regenere, bool addCustom, UrlGeneratorBase<T> structSite) where T : PhysicalStructureBase
        {
            string dir = structSite.PhysicalStructure.RepertoireImg();
            bool isNewlyCreated = regenere || !Directory.Exists(dir);

            List<string> result = ExportResourceFolder(dir, MetierResources.Folders.SiteImg, regenere);

            if (addCustom && isNewlyCreated)
            {
                List<FileInfo> customFiles = EnumerateCustomLogoFiles();
                string prefixToRemove = MetierResources.Dictionary.GetFullName(MetierResources.Folders.SiteImg);

                foreach (FileInfo cfile in customFiles)
                {
                    string cleanCustomName = ResourcePath.GetRelativePath(cfile.Name, prefixToRemove);
                    string destFile = Path.Combine(dir, cleanCustomName);

                    if (!result.Contains(destFile))
                    {
                        File.Copy(cfile.FullName, destFile, true);
                        result.Add(destFile);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Énumère les fichiers de logo personnalisés
        /// </summary>
        /// <returns>La liste des fichiers de logo personnalisés</returns>
        public static List<FileInfo> EnumerateCustomLogoFiles()
        {
            DirectoryInfo di = new DirectoryInfo(AppDirectoryManager.RessoucesImgDir);
            return di.EnumerateFiles("*.png", SearchOption.TopDirectoryOnly)
                     .Where(o => o.Name.ToLower().Contains("logo"))
                     .ToList();
        }

        /// <summary>
        /// Exporte les feuilles de style et les scripts JavaScript embarqués pour un site donné
        /// </summary>
        /// <typeparam name="T">Le type de structure physique</typeparam>
        /// <param name="regenere">Indique si les ressources doivent être régénérées</param>
        /// <param name="structSite">Le générateur d'URL pour la structure du site</param>
        /// <returns>La liste des fichiers exportés</returns>
        public static List<string> ExportEmbeddedStyleAndJS<T>(bool regenere, UrlGeneratorBase<T> structSite) where T : PhysicalStructureBase
        {
            List<string> result = new List<string>();

            string dirStyle = structSite.PhysicalStructure.RepertoireCss();
            string dirJs = structSite.PhysicalStructure.RepertoireJs();

            // 1. Export des Styles
            result.AddRange(ExportResourceFolder(dirStyle, MetierResources.Folders.SiteStyle, regenere));

            // 2. Export des Scripts JS
            result.AddRange(ExportResourceFolder(dirJs, MetierResources.Folders.SiteJs, regenere));

            return result;
        }

        /// <summary>
        /// Récupère le code JavaScript embarqué
        /// </summary>
        /// <returns>Le code JavaScript embarqué</returns>
        public static string GetEmbeddedJS()
        {
            // Utilisation de StringBuilder pour des performances optimales
            StringBuilder result = new StringBuilder();

            // On boucle uniquement sur les ressources contenues dans le dossier JS
            foreach (string jsName in MetierResources.Dictionary.FindByFolder(MetierResources.Folders.SiteJs))
            {
                // On récupère le flux
                using (Stream resourceStream = MetierResources.Dictionary.GetStream(jsName))
                {
                    if (resourceStream == null) continue;

                    // On lit et on ajoute le contenu
                    using (StreamReader reader = new StreamReader(resourceStream, Encoding.UTF8))
                    {
                        result.AppendLine(reader.ReadToEnd());
                    }
                }
            }

            return result.ToString();
        }


        /// <summary>
        /// Exporte un dossier de ressources
        /// </summary>
        /// <param name="targetDirectory">Le répertoire cible</param>
        /// <param name="resourceFolder">Le dossier de ressources</param>
        /// <param name="regenere">Indique si les ressources doivent être régénérées</param>
        /// <returns>La liste des fichiers exportés</returns>
        private static List<string> ExportResourceFolder(string targetDirectory, string resourceFolder, bool regenere)
        {
            List<string> result = new List<string>();

            if (regenere)
            {
                FileSystemHelper.DeleteDirectory(targetDirectory);
            }

            if (!Directory.Exists(targetDirectory))
            {
                FileSystemHelper.CreateDirectory(targetDirectory);
                string baseFolder = MetierResources.Dictionary.GetFullName(resourceFolder);

                foreach (string resourceName in MetierResources.Dictionary.FindByFolder(resourceFolder))
                {
                    string cleanFileName = ResourcePath.GetRelativePath(resourceName, baseFolder);
                    string fullFilePath = Path.Combine(targetDirectory, cleanFileName);

                    if (ResourceExtractor.ExtractToFile(MetierResources.Dictionary, resourceName, fullFilePath))
                    {
                        result.Add(fullFilePath);
                    }
                }
            }

            return result;
        }

        #endregion

        #region 4. MOTEUR DE GÉNÉRATION (Ex-ExportSiteManager)

        /// <summary>
        /// Point d'entrée métier pour générer le site HTML
        /// </summary>
        /// <param name="xml">Le document XML source.</param>
        /// <param name="exportType">Le type d'export à réaliser.</param>
        /// <param name="fileSave">Le chemin du fichier de sortie.</param>
        /// <param name="argsList">Les arguments XSLT.</param>
        /// <param name="fileExtension">L'extension du fichier de sortie.</param>
        /// <param name="useCache">Indique si le cache doit être utilisé.</param>
        public static void GenererHtmlSite(XmlSource xml, ExportEnum exportType, string fileSave, XsltArgumentList argsList, string fileExtension = "html", bool useCache = true)
        {
            // 1. Logique métier : quel est le template XSLT à utiliser pour cet export ?
            string xslt = GetXsltResourcePath(exportType);

            // 2. Appel à l'outil technique : génère le HTML
            ExportHTML.ToHTML(xml, fileSave, argsList, xslt, MetierResources.Dictionary, fileExtension, useCache);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="exportType"></param>
        /// <param name="savePath"></param>
        /// <param name="xsltArgs"></param>
        public static void GenererHtmlSite(XPathDocument xml, ExportEnum exportType, string savePath, XsltArgumentList xsltArgs, string fileExtension = "html", bool useCache = true)
        {
            // 1. Logique métier : quel est le template XSLT à utiliser pour cet export ?
            string xslt = GetXsltResourcePath(exportType);

            // On appelle la nouvelle surcharge de ToHTML
            ExportHTML.ToHTML(xml, savePath, xsltArgs, xslt, MetierResources.Dictionary, fileExtension, useCache);
        }
        #endregion
    }
}