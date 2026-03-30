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
// using NLog.LayoutRenderers;

namespace FranceJudo.Metier.Export
{
    public static class ExportTools
    {

        public static string default_competition = null;

        /// <summary>
        /// Retourne le nom d'un fichier d'export
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFileName(ExportEnum type)
        {
            string result = "";
            switch (type)
            {
                /*
                case ExportEnum.Participants:
                    result = "judokas";
                    break;
                case ExportEnum.Poule_Repartition:
                    result = "poules";
                    break;
                case ExportEnum.Poule_Competition2:
                    result = "poules_competition2";
                    break;
                case ExportEnum.Poule_Competition1:
                    result = "poules_competition1";
                    break;
                case ExportEnum.Poule_Resultat:
                    result = "poules_resultats";
                    break;
                case ExportEnum.Poule_Resultat_Shiai:
                    result = "poules_resultats_shiai";
                    break;
                case ExportEnum.Tableau_Competition:
                    result = "tableau_competition";
                    break;
                case ExportEnum.Tableau_Resultat:
                    result = "tableau_resultats";
                    break;
                case ExportEnum.ClassementPoule:
                    result = "classement_poules";
                    break;
                case ExportEnum.ClassementFinal:
                    result = "classement_final";
                    break;
                case ExportEnum.FeuilleCombat:
                    result = "feuille_combats";
                    break;
                case ExportEnum.FeuilleCombatPoule:
                    result = "feuille_combats_poule";
                    break;
                case ExportEnum.FeuilleCombatTableau:
                    result = "feuille_combats_tab";
                    break;
                case ExportEnum.Rapport_Sportif:
                    result = "rapport_sportif";
                    break;
                case ExportEnum.Rapport_Admin:
                    result = "rapport_administratif";
                    break;
                case ExportEnum.Rapport_Selection:
                    result = "selection";
                    break;
                case ExportEnum.RelationGrCh:
                    result = "relation_gr";
                    break;
                case ExportEnum.Pesee:
                    result = "pesee";
                    break;
                case ExportEnum.PeseeEquipe:
                    result = "peseeEquipe";
                    break;
                case ExportEnum.ParticipantsEquipe:
                    result = "judokasEquipe";
                    break;
                case ExportEnum.Diplome:
                    result = "diplome";
                    break;
                case ExportEnum.Participation:
                    result = "particiation";
                    break;
                case ExportEnum.Dispatch:
                    result = "dispatch";
                    break;
                */

                case ExportEnum.Site_Index:
                    result = "index";
                    break;
                case ExportEnum.Site_QrCode:
                    result = "QrCode";
                    break;
                case ExportEnum.Site_Menu:
                    result = "menu";
                    break;
                case ExportEnum.Site_Tapis1:
                    result = "tapis_All1";
                    break;
                case ExportEnum.Site_Tapis2:
                    result = "tapis_All2";
                    break;
                case ExportEnum.Site_Tapis4:
                    result = "tapis_All4";
                    break;
                case ExportEnum.Site_ListTapis:
                    result = "tapis_All0";
                    break;
                case ExportEnum.Site_FeuilleCombat:
                    result = "feuille_combats";
                    break;
                case ExportEnum.Site_FeuilleCombatTapis:
                    // result = "tapis_";
                    result = "se_prepare";
                    break;
                case ExportEnum.Site_Poule_Resultat:
                    result = "poules_resultats";
                    break;
                case ExportEnum.Site_Tableau_Competition:
                    result = "tableau_competition";
                    break;
                case ExportEnum.Site_ClassementFinal:
                    result = "classement_final";
                    break;
                case ExportEnum.Site_Checksum:
                    result = "checksum_fichiers_site";
                    break;
                case ExportEnum.Site_AffectationTapis:
                    result = "affectation_tapis";
                    break;
                case ExportEnum.Site_MenuAvancement:
                    result = "avancement";
                    break;
                case ExportEnum.Site_MenuClassement:
                    result = "classement";
                    break;
                case ExportEnum.Site_MenuProchainCombats:
                    result = "prochains_combats";
                    break;
                case ExportEnum.Site_FooterScript:
                    result = "footer_script";
                    break;
                case ExportEnum.Site_Engagements:
                    result = "groupe_engagements";
                    break;
                case ExportEnum.Site_MenuEngagements:
                    result = "engagements";
                    break;
                case ExportEnum.Site_Interne_EcranAppel:
                    result = "ecran";
                    break;
            }

            result = result.Replace(' ', '_');

            char[] invalidPathChars = Path.GetInvalidFileNameChars();
            foreach (char invalid in invalidPathChars)
            {
                result = result.Replace(invalid, '_');
            }

            return result;
        }

        /// <summary>
        /// Genere une image pour l'export (a partir de la bibliotheque de l'application)
        /// </summary>
        /// <param name="regenere"></param>
        /// <returns></returns>
        public static List<string> ExportEmbeddedImg<T>(bool regenere, bool addCustom, UrlGeneratorBase<T> structSite) where T : PhysicalStructureBase
        {
            string dir = structSite.PhysicalStructure.RepertoireImg();

            // On détermine AVANT l'extraction si le dossier va être créé ou purgé.
            // Cela nous indique si on doit copier les fichiers custom.
            bool isNewlyCreated = regenere || !Directory.Exists(dir);

            // 1. Export des images de base via notre méthode commune
            List<string> result = ExportResourceFolder(dir, MetierResources.Folders.SiteImg, regenere);

            // 2. Gestion des fichiers personnalisés
            if (addCustom && isNewlyCreated)
            {
                List<FileInfo> customFiles = EnumerateCustomLogoFiles();
                string prefixToRemove = MetierResources.Dictionary.GetFullName(MetierResources.Folders.SiteImg) + ".";

                foreach (FileInfo cfile in customFiles)
                {
                    string cleanCustomName = cfile.Name.Replace(prefixToRemove, "");
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
        /// Enumerer les fichiers image de type Logo se trouvant dans le repertoire de travail de l'application
        /// </summary>
        /// <returns></returns>
        public static List<FileInfo> EnumerateCustomLogoFiles()
        {
            DirectoryInfo di = new DirectoryInfo(AppDirectoryManager.RessoucesImgDir);
            return di.EnumerateFiles("*.png", SearchOption.TopDirectoryOnly).Where(o => o.Name.ToLower().Contains("logo")).ToList();
        }

        /// <summary>
        /// Exporte les fichiers js et css
        /// </summary>
        /// <param name="regenere"></param>
        /// <returns></returns>
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
        /// Recupere la liste des fichiers js
        /// </summary>
        /// <returns></returns>
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
    /// Retourne le nom de la feuille de style de traitement pour le site
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetXsltSite(ExportEnum type)
        {
            return ExportTools.GetXsltFile(type) + "_site.xslt";
        }

        /// <summary>
        /// Retourne le nom de la feuille de style de traitement
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetXsltClassique(ExportEnum type)
        {
            return ExportTools.GetXsltFile(type) + ".xslt";
        }

        /// <summary>
        /// Retourne une feuille de style d'export
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetXsltFile(ExportEnum type/*, int? niveauMax, bool site, bool header, bool footer*/)
        {
            string name = "";
            switch (type)
            {
                /*
                case ExportEnum.Pesee:
                    name = ConstantResource.Export_Judoka_res + "pesee";
                    break;
                case ExportEnum.PeseeEquipe:
                    name = ConstantResource.Export_Judoka_res + "pesee_equipe";
                    break;
                case ExportEnum.Participants:
                    name = ConstantResource.Export_Judoka_res + "participant";
                    break;
                case ExportEnum.ParticipantsEquipe:
                    name = ConstantResource.Export_Judoka_res + "participant_equipe";
                    break;
                case ExportEnum.Poule_Competition1:
                    name = ConstantResource.Export_Poule_res + "feuille_competition1";
                    break;
                case ExportEnum.Poule_Competition2:
                    name = ConstantResource.Export_Poule_res + "feuille_competition2";
                    break;
                case ExportEnum.Poule_Resultat:
                    name = ConstantResource.Export_Poule_res + "feuille_resultat";
                    break;
                case ExportEnum.Poule_Resultat_Shiai:
                    name = ConstantResource.Export_Poule_res + "feuille_resultat_shiai";
                    break;
                case ExportEnum.Poule_Repartition:
                    name = ConstantResource.Export_Poule_res + "feuille_poule";
                    break;
                case ExportEnum.FeuilleCombat:
                    name = ConstantResource.Export_Common_res + "feuille_matchs";
                    break;
                case ExportEnum.FeuilleCombatPoule:
                    name = ConstantResource.Export_Common_res + "feuille_matchs";
                    break;
                case ExportEnum.FeuilleCombatTableau:
                    name = ConstantResource.Export_Common_res + "feuille_matchs";
                    break;
                case ExportEnum.ClassementPoule:
                    name = ConstantResource.Export_Classement_res + "classement_poule";
                    break;
                case ExportEnum.ClassementFinal:
                    name = ConstantResource.Export_Classement_res + "classement_final";
                    break;
                case ExportEnum.Tableau_Competition:
                    name = ConstantResource.Export_Tableau_res + "feuille_competition";
                    break;
                case ExportEnum.Tableau_Competition_Repechage:
                    name = ConstantResource.Export_Tableau_res + "competition_repechage";
                    break;
                case ExportEnum.Tableau_Resultat:
                    name = ConstantResource.Export_Tableau_res + "feuille_resultat";
                    break;
                case ExportEnum.Rapport_Admin:
                    name = ConstantResource.Export_Common_res + "rapport_administratif";
                    break;
                case ExportEnum.Rapport_Sportif:
                    name = ConstantResource.Export_Common_res + "rapport_sportif";
                    break;
                case ExportEnum.Rapport_Selection:
                    name = ConstantResource.Export_Common_res + "rapport_selection";
                    break;
                case ExportEnum.RelationGrCh:
                    name = ConstantResource.Export_Common_res + "relation_gr_ch";
                    break;
                case ExportEnum.Diplome:
                    name = ConstantResource.Export_Classement_res + "diplome_final";
                    break;
                case ExportEnum.Participation:
                    name = ConstantResource.Export_Classement_res + "participation_final";
                    break;
                case ExportEnum.Dispatch:
                    name = ConstantResource.Export_Common_res + "feuille_dispatch";
                    break;
                */


                // TODO A remplacer par ResourcePath.Combine
                case ExportEnum.Site_Menu:
                    name = MetierResources.Folders.SiteXslt + "menu";
                    break;
                case ExportEnum.Site_Index:
                    name = MetierResources.Folders.SiteXslt + "index";
                    break;
                case ExportEnum.Site_QrCode:
                    name = MetierResources.Folders.SiteXslt + "qrcode";
                    break;
                case ExportEnum.Site_Tapis1:
                    name = MetierResources.Folders.SiteXslt + "temp_1";
                    break;
                case ExportEnum.Site_Tapis2:
                    name = MetierResources.Folders.SiteXslt + "temp_2";
                    break;
                case ExportEnum.Site_Tapis4:
                    name = MetierResources.Folders.SiteXslt + "temp_4";
                    break;
                case ExportEnum.Site_ListTapis:
                    name = MetierResources.Folders.SiteXslt + "list_tapis";
                    break;
                case ExportEnum.Site_FeuilleCombat:
                    name = MetierResources.Folders.SiteXslt + "feuille_matchs";
                    break;
                case ExportEnum.Site_FeuilleCombatTapis:
                    name = MetierResources.Folders.SiteXslt + "feuille_matchs";
                    break;
                case ExportEnum.Site_Poule_Resultat:
                    name = MetierResources.Folders.SiteXslt + "feuille_resultat";
                    break;
                case ExportEnum.Site_Tableau_Competition:
                    name = MetierResources.Folders.SiteXslt + "feuille_competition";
                    break;
                case ExportEnum.Site_ClassementFinal:
                    name = MetierResources.Folders.SiteXslt + "classement_final";
                    break;
                case ExportEnum.Site_AffectationTapis:
                    name = MetierResources.Folders.SiteXslt + "affectation_tapis";
                    break;
                case ExportEnum.Site_Engagements:
                    name = MetierResources.Folders.SiteXslt + "groupe_engagements";
                    break;
                case ExportEnum.Site_MenuEngagements:
                    name = MetierResources.Folders.SiteXslt + "engagements";
                    break;
                case ExportEnum.Site_MenuClassement:
                    name = MetierResources.Folders.SiteXslt + "classement";
                    break;
                case ExportEnum.Site_MenuAvancement:
                    name = MetierResources.Folders.SiteXslt + "avancement";
                    break;
                case ExportEnum.Site_MenuProchainCombats:
                    name = MetierResources.Folders.SiteXslt + "prochains_combats";
                    break;
                case ExportEnum.Site_FooterScript:
                    name = MetierResources.Folders.SiteXslt + "footer_script";
                    break;
                case ExportEnum.Site_Interne_EcranAppel:
                        name = MetierResources.Folders.SiteXslt + "ecrans_appel";
                    break;
                default:
                    return "";
            }

            return name;
        }

        #region METHODES PRIVEES
        /// <summary>
        /// Extrait tout le contenu d'un dossier virtuel de ressources vers un dossier physique sur le disque.
        /// </summary>
        private static List<string> ExportResourceFolder(string targetDirectory, string resourceFolder, bool regenere)
        {
            List<string> result = new List<string>();

            if (regenere)
            {
                FileSystemHelper.DeleteDirectory(targetDirectory);
            }

            if (!Directory.Exists(targetDirectory))
            {
                FileSystemHelper.CreateDirectorie(targetDirectory);

                // On prépare le préfixe à retirer (ex: "FranceJudo.Metier.Resources.Site.style.")
                string prefixToRemove = MetierResources.Dictionary.GetFullName(resourceFolder) + ".";

                foreach (string resourceName in MetierResources.Dictionary.FindByFolder(resourceFolder))
                {
                    string cleanFileName = resourceName.Replace(prefixToRemove, "");
                    string fullFilePath = Path.Combine(targetDirectory, cleanFileName);

                    // On délègue la mécanique des flux et des accès disques au Core
                    if (ResourceExtractor.ExtractToFile(MetierResources.Dictionary, resourceName, fullFilePath))
                    {
                        result.Add(fullFilePath);
                    }
                }
            }

            return result;
        }
        #endregion
    }
}