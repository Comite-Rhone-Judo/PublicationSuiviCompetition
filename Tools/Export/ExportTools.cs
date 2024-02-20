using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tools.Enum;
using Tools.Outils;

namespace Tools.Export
{
    public static class ExportTools
    {
        public static string default_competition = null;

        /// <summary>
        /// Retourne le repertoire de destination d'un fichier d'export en fonction de son type et de son contenu
        /// Le repertoire est cree s'il n'existe pas
        /// </summary>
        /// <param name="site"></param>
        /// <param name="epreuve_nom"></param>
        /// <param name="competition_nom"></param>
        /// <returns></returns>
        // TODO Remove plus utilise car utilise uniquement des constantes
        /*
        public static string getDirectory(bool site, string epreuve_nom, string competition_nom)
        {
            string directory = "";

            if (site)
            {
                directory = ConstantFile.ExportSite_dir;
            }
            else
            {
                directory = ConstantFile.Export_dir;
            }

            directory += OutilsTools.TraiteChaine(OutilsTools.SubString(default_competition, 0, 30)) + "/";

            if (string.IsNullOrWhiteSpace(epreuve_nom) && string.IsNullOrWhiteSpace(competition_nom))
            {
                directory = directory + "common";
            }
            else if (!string.IsNullOrWhiteSpace(competition_nom) && string.IsNullOrWhiteSpace(epreuve_nom))
            {
                string nom = OutilsTools.SubString(competition_nom, 0, 30);
                directory = directory + OutilsTools.TraiteChaine(nom);
            }
            else
            {
                string nom = "" + epreuve_nom;
                directory = directory + OutilsTools.SubString(OutilsTools.TraiteChaine(nom), 0, 30);
            }
            
            // Remplace les symboles le necessitant (+ des categories de poids, etc.)
            directory = OutilsTools.TraiteChaineURL(directory);

            FileAndDirectTools.CreateDirectorie(directory);

            return directory;
        }
        */

        /// <summary>
        /// Retourne le nom d'un fichier d'export
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string getFileName(ExportEnum type)
        {
            string result = "";
            switch (type)
            {
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


                case ExportEnum.Site_Index:
                    result = "Index";
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
        public static List<string> ExportEmbeddedImg(bool regenere, ExportSiteStructure structSite)
        {
            List<string> result = new List<string>();
            string dir = structSite.RepertoireImg;

            // string directory = ExportTools.getDirectory(true, null, null).Replace("common", "");

            if (regenere)
            {
                FileAndDirectTools.DeleteDirectory(dir);
            }

            if (!Directory.Exists(dir))
            {
                FileAndDirectTools.CreateDirectorie(dir);

                foreach (string s1 in ResourcesTools.GetAssembyResourceName())
                {
                    if (s1.Contains(ConstantResource.Export_site_img))
                    {
                        string fileName = Path.Combine(dir, s1.Replace(ConstantResource.Export_site_img, ""));
                        var resource = ResourcesTools.GetAssembyResource(s1);

                        FileAndDirectTools.NeedAccessFile(fileName);
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
                            FileAndDirectTools.ReleaseFile(fileName);
                        }
                        result.Add(fileName);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Exporte les fichiers js et css
        /// </summary>
        /// <param name="regenere"></param>
        /// <returns></returns>
        public static List<string> ExportEmbeddedStyleAndJS(bool regenere, ExportSiteStructure structSite)
        {
            List<string> result = new List<string>();
            string dirJs = structSite.RepertoireJs;
            string dirStyle = structSite.RepertoireStyle;

            // string directory = ExportTools.getDirectory(true, null, null).Replace("common", "");

            if (regenere)
            {
                FileAndDirectTools.DeleteDirectory(dirStyle);
                FileAndDirectTools.DeleteDirectory(dirJs);
            }


            if (!Directory.Exists(dirStyle))
            {
                FileAndDirectTools.CreateDirectorie(dirStyle);

                foreach (string s1 in ResourcesTools.GetAssembyResourceName())
                {
                    if (s1.Contains(ConstantResource.Export_site_style))
                    {
                        string fileName = Path.Combine(dirStyle, s1.Replace(ConstantResource.Export_site_style, ""));
                        var resource = ResourcesTools.GetAssembyResource(s1);

                        FileAndDirectTools.NeedAccessFile(fileName);
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
                            FileAndDirectTools.ReleaseFile(fileName);
                        }
                        result.Add(fileName);
                    }
                }
            }

            if (!Directory.Exists(dirJs))
            {
                FileAndDirectTools.CreateDirectorie(dirJs);

                foreach (string s1 in ResourcesTools.GetAssembyResourceName())
                {
                    if (s1.Contains(ConstantResource.Export_site_js))
                    {
                        string fileName = Path.Combine(dirJs,  s1.Replace(ConstantResource.Export_site_js, ""));

                        var resource = ResourcesTools.GetAssembyResource(s1);

                        FileAndDirectTools.NeedAccessFile(fileName);
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
                            FileAndDirectTools.ReleaseFile(fileName);
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
        public static string getEmbeddedJS()
        {
            string result = "";

            foreach (string js in ResourcesTools.GetAssembyResourceName())
            {
                if (!js.Contains(ConstantResource.Export_site_js))
                {
                    continue;
                }

                var resource = ResourcesTools.GetAssembyResource(js);

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



                case ExportEnum.Site_Menu:
                    name = ConstantResource.Export_Site_res + "menu";
                    break;
                case ExportEnum.Site_Index:
                    name = ConstantResource.Export_Site_res + "index";
                    break;
                case ExportEnum.Site_QrCode:
                    name = ConstantResource.Export_Site_res + "qrcode";
                    break;
                case ExportEnum.Site_Tapis1:
                    name = ConstantResource.Export_Site_res + "temp_1";
                    break;
                case ExportEnum.Site_Tapis2:
                    name = ConstantResource.Export_Site_res + "temp_2";
                    break;
                case ExportEnum.Site_Tapis4:
                    name = ConstantResource.Export_Site_res + "temp_4";
                    break;
                case ExportEnum.Site_ListTapis:
                    name = ConstantResource.Export_Site_res + "list_tapis";
                    break;
                case ExportEnum.Site_FeuilleCombat:
                    name = ConstantResource.Export_Site_res + "feuille_matchs";
                    break;
                case ExportEnum.Site_FeuilleCombatTapis:
                    name = ConstantResource.Export_Site_res + "feuille_matchs";
                    break;
                case ExportEnum.Site_Poule_Resultat:
                    name = ConstantResource.Export_Site_res + "feuille_resultat";
                    break;
                case ExportEnum.Site_Tableau_Competition:
                    name = ConstantResource.Export_Site_res + "feuille_competition";
                    break;
                case ExportEnum.Site_ClassementFinal:
                    name = ConstantResource.Export_Site_res + "classement_final";
                    break;
                case ExportEnum.Site_AffectationTapis:
                    name = ConstantResource.Export_Site_res + "affectation_tapis";
                    break;
                case ExportEnum.Site_MenuClassement:
                    name = ConstantResource.Export_Site_res + "classement";
                    break;
                case ExportEnum.Site_MenuAvancement:
                    name = ConstantResource.Export_Site_res + "avancement";
                    break;
                case ExportEnum.Site_MenuProchainCombats:
                    name = ConstantResource.Export_Site_res + "prochains_combats";
                    break;
                default:
                    return "";
            }

            return name;
        }

        /// <summary>
        /// Calcul d'URL du site en local en fonction de la competition en cours
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="nom_compet"></param>
        /// <returns></returns>
        public static string GetURLSiteLocal(string ip, int port, string nom_compet)
        {
            string result = "";

            result = "http://";
            result += ip;
            result += ":";
            result += port.ToString() + "/";
            // result += "/site/";
            result += OutilsTools.TraiteChaine(OutilsTools.SubString(nom_compet, 0, 30));
            result += "/common/index.html";

            return result;
        }

        /// <summary>
        /// Calcul d'URL du site FTP en fonction de la competition
        /// </summary>
        /// <param name="nom_compet"></param>
        /// <returns></returns>
        public static string GetURLSiteFTP(string nom_compet)
        {
            string result = NetworkTools.HTTP_SUIVI_URL;
            result += OutilsTools.TraiteChaine(OutilsTools.SubString(nom_compet, 0, 30));
            result += "/common/index.html";

            return result;
        }

        /// <summary>
        /// Calcul d'URL du site distant en fonction de la competition en cours
        /// </summary>
        /// <param name="urlRacine"></param>
        /// <param name="nom_compet"></param>
        /// <returns></returns>
        public static string GetURLSiteDistant(string urlRacine, string nom_compet)
        {

            Uri root = new Uri(urlRacine);

            string suffix = OutilsTools.TraiteChaine(OutilsTools.SubString(nom_compet, 0, 30));
            suffix += "/common/index.html";

            // Uri suffixUri = new Uri(suffix);

            Uri fullUri = new Uri(root, suffix);

            return fullUri.ToString();

            /*
            string result = urlRacine;
            result += OutilsTools.TraiteChaine(OutilsTools.SubString(nom_compet, 0, 30));
            result += "/common/index.html";

            return result;
            */
        }
    }
}