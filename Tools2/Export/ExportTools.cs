using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Xsl;
using Tools.Enum;
using Tools.Outils;
using Tools.Struct;

namespace Tools.Export
{
    public static class ExportTools
    {
        public static string default_competition = null;

        public static IList<ExportItemStruct> GetExportItem(CompetitionTypeEnum type_competition, int? type2, TypePhaseEnum phase)
        {
            IList<ExportItemStruct> result = new List<ExportItemStruct>();

            if (!type2.HasValue)
            {
                if (type_competition == CompetitionTypeEnum.Equipe)
                {
                    result.Add(new ExportItemStruct { Text = "Participants", Value = (int)ExportEnum.ParticipantsEquipe });
                    result.Add(new ExportItemStruct { Text = "Pesée", Value = (int)ExportEnum.PeseeEquipe });
                }
                else
                {
                    result.Add(new ExportItemStruct { Text = "Participants", Value = (int)ExportEnum.Participants });
                    result.Add(new ExportItemStruct { Text = "Pesée", Value = (int)ExportEnum.Pesee });
                }

                result.Add(new ExportItemStruct { Text = "Répartition des poules", Value = (int)ExportEnum.Poule_Repartition });
                result.Add(new ExportItemStruct { Text = "Feuille Combats", Value = (int)ExportEnum.FeuilleCombat });
                result.Add(new ExportItemStruct { Text = "Poules et/ou tableau", Value = (int)ExportEnum.Resultat });
                result.Add(new ExportItemStruct { Text = "Classement", Value = (int)ExportEnum.ClassementFinal });
                result.Add(new ExportItemStruct { Text = "Diplôme", Value = (int)ExportEnum.Diplome });
                result.Add(new ExportItemStruct { Text = "Dispatching", Value = (int)ExportEnum.Dispatch });
                result.Add(new ExportItemStruct { Text = "Rapport administratif", Value = (int)ExportEnum.Rapport_Admin });
                result.Add(new ExportItemStruct { Text = "Rapport sportif", Value = (int)ExportEnum.Rapport_Sportif });

                if (type_competition != CompetitionTypeEnum.Shiai)
                {
                    result.Add(new ExportItemStruct { Text = "Rapport de sélection", Value = (int)ExportEnum.Rapport_Selection });
                }

                if (type_competition != CompetitionTypeEnum.Equipe)
                {
                    result.Add(new ExportItemStruct { Text = "Relation grade/championnat", Value = (int)ExportEnum.RelationGrCh });
                }
                result.Add(new ExportItemStruct { Text = "Excel", Value = (int)ExportEnum.Excel });
                result.Add(new ExportItemStruct { Text = "Rapport erreur licence judoka", Value = (int)ExportEnum.RapportErreurJudoka });
                result.Add(new ExportItemStruct { Text = "Compétition (Pour Extranet)", Value = (int)ExportEnum.Competition });
            }
            else
            {
                switch (type2.Value)
                {
                    case 1: // A partir d'inscription
                        if (type_competition != CompetitionTypeEnum.Equipe)
                        {
                            result.Add(new ExportItemStruct { Text = "Participants", Value = (int)ExportEnum.Participants });
                            result.Add(new ExportItemStruct { Text = "Pesée", Value = (int)ExportEnum.Pesee });
                        }
                        else
                        {
                            result.Add(new ExportItemStruct { Text = "Participants", Value = (int)ExportEnum.ParticipantsEquipe });
                            result.Add(new ExportItemStruct { Text = "Pesée", Value = (int)ExportEnum.PeseeEquipe });
                        }
                        break;
                    case 2: // A partir du tirage
                        if (phase == TypePhaseEnum.Poule)
                        {
                            result.Add(new ExportItemStruct { Text = "Répartition des poules", Value = (int)ExportEnum.Poule_Repartition });

                            if (type_competition != CompetitionTypeEnum.Shiai)
                            {
                                result.Add(new ExportItemStruct { Text = "Poules vierges (classiques)", Value = (int)ExportEnum.Poule_Competition1 });
                                result.Add(new ExportItemStruct { Text = "Poules vierges (verticale)", Value = (int)ExportEnum.Poule_Competition2 });
                            }

                            if(type_competition == CompetitionTypeEnum.Shiai)
                            {
                                result.Add(new ExportItemStruct { Text = "Poules résultats", Value = (int)ExportEnum.Poule_Resultat_Shiai });
                            }
                            else
                            {
                                result.Add(new ExportItemStruct { Text = "Poules résultats", Value = (int)ExportEnum.Poule_Resultat });
                            }
                            
                            result.Add(new ExportItemStruct { Text = "Feuille de combats", Value = (int)ExportEnum.FeuilleCombat });
                        }
                        else if (phase == TypePhaseEnum.Tableau)
                        {
                            result.Add(new ExportItemStruct { Text = "Tableau competition", Value = (int)ExportEnum.Tableau_Competition });
                            result.Add(new ExportItemStruct { Text = "Feuille de combats", Value = (int)ExportEnum.FeuilleCombat });
                        }
                        break;
                    case 3: // A partir de classement
                        if (phase == TypePhaseEnum.Poule)
                        {
                            result.Add(new ExportItemStruct { Text = "Classement des poules", Value = (int)ExportEnum.ClassementPoule });
                        }
                        else if (phase == TypePhaseEnum.Tableau)
                        {
                            result.Add(new ExportItemStruct { Text = "Classement", Value = (int)ExportEnum.ClassementTableau });
                        }
                        break;
                    case 4: // A partir du deroulement
                        result.Add(new ExportItemStruct { Text = "Feuille de combats", Value = (int)ExportEnum.FeuilleCombat });
                        result.Add(new ExportItemStruct { Text = "Dispatching", Value = (int)ExportEnum.Dispatch });
                        break;
                    default:
                        result.Add(new ExportItemStruct { Text = "Rapport sportif", Value = (int)ExportEnum.Rapport_Sportif });
                        result.Add(new ExportItemStruct { Text = "Rapport administratif", Value = (int)ExportEnum.Rapport_Admin });
                        result.Add(new ExportItemStruct { Text = "Excel", Value = (int)ExportEnum.Excel });
                        break;

                }
            }

            return result;
        }


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

            FileAndDirectTools.CreateDirectorie(directory);

            //if (!Directory.Exists(directory))
            //{
            //    Directory.CreateDirectory(directory);
            //}

            return directory;
        }

        public static string getStyleDirectory(bool site)
        {
            string directoryStyle = "";

            if (site)
            {
                directoryStyle = ConstantFile.ExportStyleSite_dir;
            }
            else
            {
                directoryStyle = ConstantFile.ExportStyle_dir;
            }
            return directoryStyle;
        }

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
                    result = "tapis_";
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
            }

            result = result.Replace(' ', '_');

            char[] invalidPathChars = Path.GetInvalidFileNameChars();
            foreach (char invalid in invalidPathChars)
            {
                result = result.Replace(invalid, '_');
            }

            return result;
        }

        public static List<string> ExportImg(bool regenere)
        {
            List<string> result = new List<string>();

            string directory = ExportTools.getDirectory(true, null, null).Replace("common", "");

            if (regenere)
            {
                FileAndDirectTools.DeleteDirectory(directory + "img");
            }

            if (!Directory.Exists(directory + "img"))
            {
                FileAndDirectTools.CreateDirectorie(directory + "img");

                foreach (string s1 in ResourcesTools.GetAssembyResourceName())
                {
                    if (s1.Contains(ConstantResource.Export_site_img))
                    {
                        string fileName = directory + "img/" + s1.Replace(ConstantResource.Export_site_img, "");
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


        public static List<string> ExportStyleAndJS(bool regenere)
        {
            List<string> result = new List<string>();

            string directory = ExportTools.getDirectory(true, null, null).Replace("common", "");

            if(regenere)
            {
                FileAndDirectTools.DeleteDirectory(directory + "style");
                FileAndDirectTools.DeleteDirectory(directory + "js");
            }


            if (!Directory.Exists(directory + "style"))
            {
                FileAndDirectTools.CreateDirectorie(directory + "style");
                
                foreach (string s1 in ResourcesTools.GetAssembyResourceName())
                {
                    if (s1.Contains(ConstantResource.Export_site_style))
                    {
                        string fileName = directory + "style/" + s1.Replace(ConstantResource.Export_site_style, "");
                        var resource = ResourcesTools.GetAssembyResource(s1);

                        FileAndDirectTools.NeedAccessFile(fileName);
                        try
                        {
                            using (FileStream fs = new FileStream( fileName, FileMode.Create))
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

            if (!Directory.Exists(directory + "js"))
            {
                FileAndDirectTools.CreateDirectorie(directory + "js");

                foreach (string s1 in ResourcesTools.GetAssembyResourceName())
                {
                    if (s1.Contains(ConstantResource.Export_site_js))
                    {
                        string fileName = directory + "js/" + s1.Replace(ConstantResource.Export_site_js, "");

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

        public static string getJS()
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

        public static string GetXsltSite(ExportEnum type)
        {
            return ExportTools.GetXsltFile(type) + "_site.xslt";
        }

        public static string GetXsltHeader(ExportEnum type)
        {
            return ExportTools.GetXsltFile(type) + "_header.xslt";
        }

        public static string GetXsltFooter(ExportEnum type)
        {
            return ExportTools.GetXsltFile(type) + "_footer.xslt";
        }

        public static string GetXsltClassique(ExportEnum type)
        {
            return ExportTools.GetXsltFile(type) + ".xslt";
        }

        public static string GetXsltTabeau(ExportEnum type, int niveau)
        {
            return ExportTools.GetXsltFile(type) + "_" + Math.Pow(2, (int)niveau) + ".xslt";
        }

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
                default:
                    return "";
            }

            return name;
        }


        public static string GetURLSiteLocal(string ip, int port, string nom_compet)
        {
            string result = "";

            result = "http://";
            result += ip;
            result += ":";
            result += port.ToString();
            result += "/site/";
            result += OutilsTools.TraiteChaine(OutilsTools.SubString(nom_compet, 0, 30));
            result += "/common/index.html";

            return result;
        }

        public static string GetURLSiteFTP(string nom_compet)
        {
            string result = NetworkTools.HTTP_SUIVI_URL;
            result += OutilsTools.TraiteChaine(OutilsTools.SubString(nom_compet, 0, 30));
            result += "/common/index.html";

            return result;
        }

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


        public static byte[] ExportQrCode(string url/*, int port*/)
        {
            XDocument doc = new XDocument();
            XElement xcompetition = new XElement(ConstantXML.Competition);
            xcompetition.SetAttributeValue(ConstantXML.Address, url);
            //xcompetition.SetAttributeValue(ConstantXML.Port, port);
            doc.Add(xcompetition);

            string directory = ExportTools.getDirectory(true, null, null);
            string data = @"data:image/png;base64," + Convert.ToBase64String(File.ReadAllBytes(directory + @"qrcode.png"));
            xcompetition.SetAttributeValue(ConstantXML.Image, data);
            ExportEnum type = ExportEnum.Site_QrCode;
            string directory2 = ExportTools.getDirectory(true, null, null);
            string filename = ExportTools.getFileName(type);
            bool site = true;
            bool landscape = false;
            string fileSave = directory + "/" + "QrCode";
            XsltArgumentList argsList = new XsltArgumentList();

            ExportHTML.ToHTMLClassique(doc.ToXmlDocument(), type, site, fileSave, argsList);

            ExportPDF export = new ExportPDF_EVO();
            export.Margin = true;
            export.Landscape = landscape;
            export.Regenere = true;
            export.Background = true;
            export.Withlogo = false;

            return File.ReadAllBytes(export.ToPDF(new List<string>() { directory2 + "/" + filename }));

            //List<ExportPDFItem> exportItemList = new List<ExportPDFItem> { new ExportPDFItem { PdfName = directory2 + "/" + filename, IsLandscape = false } };
        }
    }
}