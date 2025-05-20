using AppPublication.Controles;
using AppPublication.Tools.Enum;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;

namespace AppPublication.Export
{
    public static class ExportSite
    {
        private const int kTailleMaxNomCompetition = 30;

        /// <summary>
        /// Génére les éléments donnés d'une phase
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="phase">la phase</param>
        public static List<FileWithChecksum> GenereWebSitePhase(JudoData DC, Phase phase, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            i_vue_epreuve_interface i_vue_epreuve = null;
            if (phase.isEquipe)
            {
                i_vue_epreuve = DC.Organisation.vepreuves_equipe.FirstOrDefault(o => o.id == phase.epreuve);
            }
            else
            {
                i_vue_epreuve = DC.Organisation.vepreuves.FirstOrDefault(o => o.id == phase.epreuve);
            }

            //Epreuve epreuve = DC.Epreuve.FirstOrDefault(o => o.id == phase.epreuve);
            Competition compet = DC.Organisation.Competitions.FirstOrDefault(o => o.id == i_vue_epreuve.competition);

            // Ne genere que les fichiers necessaires
            if (config.PublierProchainsCombats)
            {
                // Genere les prochains combats pour une epreuve (istapis = epreuve) via feuille_matchs_site.xslt
                ExportEnum type = ExportEnum.Site_FeuilleCombat;

                string directory = siteStruct.RepertoireEpreuve(i_vue_epreuve.id.ToString(), i_vue_epreuve.nom);
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                argsList.AddParam("istapis", "", "epreuve");

                XmlDocument xmlFeuilleCombat = ExportXML.CreateDocumentFeuilleCombat(DC, phase, null);
                ExportXML.AddPublicationInfo(ref xmlFeuilleCombat, config);
                ExportXML.AddClubs(ref xmlFeuilleCombat, DC);
                LogTools.Logger.Debug("XML genere: '{0}'", xmlFeuilleCombat.InnerXml);

                ExportHTML.ToHTMLSite(xmlFeuilleCombat, type, fileSave, argsList);
                urls.Add(fileSave + ".html");
            }

            if (phase.typePhase == (int)TypePhaseEnum.Poule)
            {
                ExportEnum type2 = ExportEnum.Site_Poule_Resultat;
                string directory2 = siteStruct.RepertoireEpreuve(i_vue_epreuve.id.ToString(), i_vue_epreuve.nom);
                string filename2 = ExportTools.getFileName(type2);
                string fileSave2 = Path.Combine(directory2, filename2.Replace("/", "_"));
                XsltArgumentList argsList2 = new XsltArgumentList();

                // Calcul la disposition de la poule
                int typePoule = (int)TypePouleEnum.Diagonale;
                if (config.PouleEnColonnes)
                {
                    typePoule = (config.PouleToujoursEnColonnes) ? (int)TypePouleEnum.Colonnes : (int)TypePouleEnum.Auto;
                }
                argsList2.AddParam("typePoule", "", typePoule);
                argsList2.AddParam("tailleMaxPouleColonne", "", config.TailleMaxPouleColonnes);

                XmlDocument xmlResultat = ExportXML.CreateDocumentPhase(i_vue_epreuve, phase, DC);
                ExportXML.AddPublicationInfo(ref xmlResultat, config);
                ExportXML.AddCeintures(ref xmlResultat, DC);
                ExportXML.AddClubs(ref xmlResultat, DC);
                LogTools.Logger.Debug("XML genere: '{0}'", xmlResultat.InnerXml);

                ExportHTML.ToHTMLSite(xmlResultat, type2, fileSave2, argsList2);
                urls.Add(fileSave2 + ".html");
                }
            else if (phase.typePhase == (int)TypePhaseEnum.Tableau)
            {
                ExportEnum type2 = ExportEnum.Site_Tableau_Competition;
                string directory2 = siteStruct.RepertoireEpreuve(i_vue_epreuve.id.ToString(), i_vue_epreuve.nom);
                string filename2 = ExportTools.getFileName(type2);
                string fileSave2 = Path.Combine(directory2, filename2.Replace("/", "_"));
                XsltArgumentList argsList2 = new XsltArgumentList();

                XmlDocument xmlResultat = ExportXML.CreateDocumentPhase(i_vue_epreuve, phase, DC);
                ExportXML.AddPublicationInfo(ref xmlResultat, config);
                ExportXML.AddCeintures(ref xmlResultat, DC);
                ExportXML.AddClubs(ref xmlResultat, DC);
                LogTools.Logger.Debug("XML genere: '{0}'", xmlResultat.InnerXml);

                ExportHTML.ToHTMLSite(xmlResultat, type2, fileSave2, argsList2);
                urls.Add(fileSave2 + ".html");
            }

            // Genere les checksums des fichiers generes
            output = urls.Select(o => new FileWithChecksum(o)).ToList();

            // Debug.WriteLine(string.Format("GenereWebSitePhase {0}", output.Count));

            return output;
        }

        /// <summary>
        /// Génére le classement d'une épreuve
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="epreuve"></param>
        public static List<FileWithChecksum> GenereWebSiteClassement(JudoData DC, i_vue_epreuve_interface epreuve, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            ExportEnum type = ExportEnum.Site_ClassementFinal;
            // string epreuve_nom = epreuve != null ? (epreuve.id + "_" + epreuve.nom) : null;
            string directory = siteStruct.RepertoireEpreuve(epreuve.id.ToString(), epreuve.nom);
            string filename = ExportTools.getFileName(type);
            string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument xml = ExportXML.CreateDocumentEpreuve(DC, epreuve);
            ExportXML.AddPublicationInfo(ref xml, config);
            ExportXML.AddClubs(ref xml, DC);
            LogTools.Logger.Debug("XML genere: '{0}'", xml.InnerXml);

            ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);
            // return new List<string> { fileSave + ".html" };

            output.Add(new FileWithChecksum(fileSave + ".html"));
            // Debug.WriteLine(string.Format("GenereWebSiteClassement {0}", output.Count));

            return output;
        }

        /// <summary>
        /// Génére les premiers combats de tous les tapis
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteAllTapis(JudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            // Genere les prochains combats de tous les tapis, istapis = alltapis (Se Prepare)  => feuille_matchs_site.xslt
            ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
            string directory =  siteStruct.RepertoireCommon;
            // string filename = ExportTools.getFileName(type) + "All"; //ExportTools.getFileName(type) + "_tapis_" + "All";
            string filename = ExportTools.getFileName(type);
            string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("istapis", "", "alltapis");

            XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
            ExportXML.AddPublicationInfo(ref xml, config);
            ExportXML.AddClubs(ref xml, DC);
            LogTools.Logger.Debug("XML genere: '{0}'", xml.InnerXml);

            ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);
            // return new List<string> { fileSave + ".html" };

            output.Add(new FileWithChecksum(fileSave + ".html"));
            // Debug.WriteLine(string.Format("GenereWebSiteAllTapis {0}", output.Count));

            return output;
        }

        /// <summary>
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteIndex(JudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            ExportEnum type;

            XmlDocument docindex = ExportXML.CreateDocumentIndex(DC, siteStruct);
            ExportXML.AddPublicationInfo(ref docindex, config);
            LogTools.Logger.Debug("XML genere: '{0}'", docindex.InnerXml);

            // Genere l'index
            type = ExportEnum.Site_Index;
            XsltArgumentList argsList = new XsltArgumentList();
            string filename = ExportTools.getFileName(type);
            string fileSave = Path.Combine(siteStruct.RepertoireCommon, filename.Replace("/", "_"));
            ExportHTML.ToHTMLSite(docindex, type, fileSave, argsList);
            output.Add(new FileWithChecksum(fileSave + ".html"));

            // No need to regenerate those files, they are usually static unless they are updated
            urls = urls.Concat(ExportTools.ExportEmbeddedStyleAndJS(true, siteStruct)).ToList();
            LogTools.Logger.Debug("GenereWebSiteIndex - ExportStyleAndJS {0}", urls.Count);

            // No need to regenerate those files, they are usually static unless they are updated
            // Genere les images "par defaut" contenues dans l'application et les images personnalises de l'utilisateur
            urls = urls.Concat(ExportTools.ExportEmbeddedImg(true, true, siteStruct)).ToList();
            LogTools.Logger.Debug("GenereWebSiteIndex - ExportImg {0}", urls.Count);

            output.AddRange(urls.Select(o => new FileWithChecksum(o)).ToList());

            // Genere le script de mise a jour
            type = ExportEnum.Site_FooterScript;
            XsltArgumentList argsListFooter = new XsltArgumentList();
            string filenameFooter = ExportTools.getFileName(type);
            string fileSaveFooter = Path.Combine(siteStruct.RepertoireJs, filenameFooter.Replace("/", "_"));
            ExportHTML.ToHTMLSite(docindex, type, fileSaveFooter, argsListFooter, "js");
            output.Add(new FileWithChecksum(fileSaveFooter + ".js"));


            LogTools.Logger.Debug("GenereWebSiteIndex {0}", output.Count);
            return output;
        }

        /// <summary>
        /// Génére le menu
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteMenu(JudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            ExportEnum type;
            string directory = siteStruct.RepertoireCommon;

            XmlDocument docmenu = ExportXML.CreateDocumentMenu(DC, siteStruct);
            ExportXML.AddPublicationInfo(ref docmenu, config);
            LogTools.Logger.Debug("XML genere: '{0}'", docmenu.InnerXml);

            // Genere le menu de d'avancement
            type = ExportEnum.Site_MenuAvancement;
            XsltArgumentList argsList = new XsltArgumentList();
            string filename = ExportTools.getFileName(type);
            string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
            ExportHTML.ToHTMLSite(docmenu, type, fileSave, argsList);
            output.Add(new FileWithChecksum(fileSave + ".html"));

            // Genere le menu de classement
            type = ExportEnum.Site_MenuClassement;
            XsltArgumentList argsList2 = new XsltArgumentList();
            string filename2 = ExportTools.getFileName(type);
            string fileSave2 = directory + "/" + filename2.Replace("/", "_");
            ExportHTML.ToHTMLSite(docmenu, type, fileSave2, argsList2);
            output.Add(new FileWithChecksum(fileSave2 + ".html"));

            // Genere le menu de prochain combat
            if (config.PublierProchainsCombats)
            {
                type = ExportEnum.Site_MenuProchainCombats;
                XsltArgumentList argsListPc = new XsltArgumentList();
                string filenamePc = ExportTools.getFileName(type);
                string fileSavePc = directory + "/" + filenamePc.Replace("/", "_");
                ExportHTML.ToHTMLSite(docmenu, type, fileSavePc, argsListPc);
                output.Add(new FileWithChecksum(fileSavePc + ".html"));
            }

            // Debug.WriteLine(string.Format("GenereWebSiteMenu {0}", output.Count));
            return output;

            //ExportHTML.ToHTML_Menu(docmenu);
            //return new List<string> { ExportTools.getDirectory(true, null, null) + @"\menu.html" };
        }

        /// <summary>
        /// Genere la page d'affectation des tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<FileWithChecksum> GenereWebSiteAffectation(JudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            ExportEnum type = ExportEnum.Site_AffectationTapis;
            string directory = siteStruct.RepertoireCommon;
            string filename = ExportTools.getFileName(type);
            string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument docAffectation = ExportXML.CreateDocumentAffectationTapis(DC);
            ExportXML.AddPublicationInfo(ref docAffectation, config);
            LogTools.Logger.Debug("XML genere: '{0}'", docAffectation.InnerXml);

            ExportHTML.ToHTMLSite(docAffectation, type, fileSave, argsList);

            output.Add(new FileWithChecksum(fileSave + ".html"));
            return output;
        }
    }
}
