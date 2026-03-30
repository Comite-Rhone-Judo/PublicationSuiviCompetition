using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FranceJudo.Core.IO;
using FranceJudo.Core.Reflection;
using FranceJudo.Metier.Site;
using FranceJudo.Metier.Resources;
using FranceJudo.Metier.IO;
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
            List<string> result = new List<string>();
            string dir = structSite.PhysicalStructure.RepertoireImg();

            // string directory = ExportTools.getDirectory(true, null, null).Replace("common", "");

            if (regenere)
            {
                FileSystemHelper.DeleteDirectory(dir);
            }

            if (!Directory.Exists(dir))
            {
                FileSystemHelper.CreateDirectorie(dir);

                foreach (string s1 in AssemblyResourceHelper.GetAssembyResourceName())
                {
                    if (s1.Contains(ResourceDictionnay.Site_Img))
                    {
                        string fileName = Path.Combine(dir, s1.Replace(ResourceDictionnay.Site_Img, ""));
                        var resource = AssemblyResourceHelper.GetAssembyResource(s1);

                        FileSystemHelper.NeedAccessFile(fileName);
                        try
                        {
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            {
                                byte[] bytes = new byte[resource.Length];
                                resource.Read(bytes, 0, (int)resource.Length);
                                fs.Write(bytes, 0, bytes.Length);
                                resource.Close();
                            }
                        }
                        catch { }
                        finally
                        {
                            FileSystemHelper.ReleaseFile(fileName);
                        }
                        result.Add(fileName);
                    }
                }

                // Si on doit ajouter des fichiers personnalises
                if (addCustom)
                {
                    // Enumere les fichiers dans le repertoire de travail
                    List<FileInfo> customFiles = EnumerateCustomLogoFiles();

                    // Copie les nouveaux fichiers trouves
                    foreach (FileInfo cfile in customFiles)
                    {
                        // il faut tenir compte du nom compose pour les resources
                        string destFile = Path.Combine(dir, cfile.Name.Replace(ResourceDictionnay.Site_Img, ""));
                        if (!result.Contains(destFile))
                        {
                            File.Copy(cfile.FullName, destFile);
                            result.Add(destFile);
                        }
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
            string dirJs = structSite.PhysicalStructure.RepertoireJs();
            string dirStyle = structSite.PhysicalStructure.RepertoireCss();

            // string directory = ExportTools.getDirectory(true, null, null).Replace("common", "");

            if (regenere)
            {
                FileSystemHelper.DeleteDirectory(dirStyle);
                FileSystemHelper.DeleteDirectory(dirJs);
            }


            if (!Directory.Exists(dirStyle))
            {
                FileSystemHelper.CreateDirectorie(dirStyle);

                foreach (string s1 in AssemblyResourceHelper.GetAssembyResourceName())
                {
                    if (s1.Contains(ResourceDictionnay.Site_Style))
                    {
                        string fileName = Path.Combine(dirStyle, s1.Replace(ResourceDictionnay.Site_Style, ""));
                        var resource = AssemblyResourceHelper.GetAssembyResource(s1);

                        FileSystemHelper.NeedAccessFile(fileName);
                        try
                        {
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            {
                                byte[] bytes = new byte[resource.Length];
                                resource.Read(bytes, 0, (int)resource.Length);
                                fs.Write(bytes, 0, bytes.Length);
                                resource.Close();
                            }
                        }
                        catch { }
                        finally
                        {
                            FileSystemHelper.ReleaseFile(fileName);
                        }
                        result.Add(fileName);
                    }
                }
            }

            if (!Directory.Exists(dirJs))
            {
                FileSystemHelper.CreateDirectorie(dirJs);

                foreach (string s1 in AssemblyResourceHelper.GetAssembyResourceName())
                {
                    if (s1.Contains(ResourceDictionnay.Site_Js))
                    {
                        string fileName = Path.Combine(dirJs, s1.Replace(ResourceDictionnay.Site_Js, ""));

                        var resource = AssemblyResourceHelper.GetAssembyResource(s1);

                        FileSystemHelper.NeedAccessFile(fileName);
                        try
                        {
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            {
                                byte[] bytes = new byte[resource.Length];
                                resource.Read(bytes, 0, (int)resource.Length);
                                fs.Write(bytes, 0, bytes.Length);
                                resource.Close();
                            }
                        }
                        catch { }
                        finally
                        {
                            FileSystemHelper.ReleaseFile(fileName);
                        }
                        result.Add(fileName);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Recupere la liste des fichiers js
        /// </summary>
        /// <returns></returns>
        public static string GetEmbeddedJS()
        {
            string result = "";

            foreach (string js in AssemblyResourceHelper.GetAssembyResourceName())
            {
                if (!js.Contains(ResourceDictionnay.Site_Js))
                {
                    continue;
                }

                var resource = AssemblyResourceHelper.GetAssembyResource(js);

                using (StreamReader reader = new StreamReader(resource, Encoding.UTF8))
                {
                    result += reader.ReadToEnd() + Environment.NewLine;
                }
            }

            return result;
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


                case ExportEnum.Site_Menu:
                    name = ResourceDictionnay.Site_Xslt + "menu";
                    break;
                case ExportEnum.Site_Index:
                    name = ResourceDictionnay.Site_Xslt + "index";
                    break;
                case ExportEnum.Site_QrCode:
                    name = ResourceDictionnay.Site_Xslt + "qrcode";
                    break;
                case ExportEnum.Site_Tapis1:
                    name = ResourceDictionnay.Site_Xslt + "temp_1";
                    break;
                case ExportEnum.Site_Tapis2:
                    name = ResourceDictionnay.Site_Xslt + "temp_2";
                    break;
                case ExportEnum.Site_Tapis4:
                    name = ResourceDictionnay.Site_Xslt + "temp_4";
                    break;
                case ExportEnum.Site_ListTapis:
                    name = ResourceDictionnay.Site_Xslt + "list_tapis";
                    break;
                case ExportEnum.Site_FeuilleCombat:
                    name = ResourceDictionnay.Site_Xslt + "feuille_matchs";
                    break;
                case ExportEnum.Site_FeuilleCombatTapis:
                    name = ResourceDictionnay.Site_Xslt + "feuille_matchs";
                    break;
                case ExportEnum.Site_Poule_Resultat:
                    name = ResourceDictionnay.Site_Xslt + "feuille_resultat";
                    break;
                case ExportEnum.Site_Tableau_Competition:
                    name = ResourceDictionnay.Site_Xslt + "feuille_competition";
                    break;
                case ExportEnum.Site_ClassementFinal:
                    name = ResourceDictionnay.Site_Xslt + "classement_final";
                    break;
                case ExportEnum.Site_AffectationTapis:
                    name = ResourceDictionnay.Site_Xslt + "affectation_tapis";
                    break;
                case ExportEnum.Site_Engagements:
                    name = ResourceDictionnay.Site_Xslt + "groupe_engagements";
                    break;
                case ExportEnum.Site_MenuEngagements:
                    name = ResourceDictionnay.Site_Xslt + "engagements";
                    break;
                case ExportEnum.Site_MenuClassement:
                    name = ResourceDictionnay.Site_Xslt + "classement";
                    break;
                case ExportEnum.Site_MenuAvancement:
                    name = ResourceDictionnay.Site_Xslt + "avancement";
                    break;
                case ExportEnum.Site_MenuProchainCombats:
                    name = ResourceDictionnay.Site_Xslt + "prochains_combats";
                    break;
                case ExportEnum.Site_FooterScript:
                    name = ResourceDictionnay.Site_Xslt + "footer_script";
                    break;
                case ExportEnum.Site_Interne_EcranAppel:
                        name = ResourceDictionnay.Site_Xslt + "ecrans_appel";
                    break;
                default:
                    return "";
            }

            return name;
        }
    }
}