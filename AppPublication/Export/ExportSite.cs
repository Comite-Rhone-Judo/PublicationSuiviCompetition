using AppPublication.Controles;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <summary>
        /// Génére les éléments donnés d'une phase
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="phase">la phase</param>
        public static List<FileWithChecksum> GenereWebSitePhase(JudoData DC, Phase phase, ConfigurationExportSite config)
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

            bool site = true;
            // Ne genere que les fichiers necessaires
            if (config.PublierProchainsCombats)
            {
                // Genere les prochains combats pour une epreuve (istapis = epreuve) via feuille_matchs_site.xslt
                ExportEnum type = ExportEnum.Site_FeuilleCombat;

                string epreuve_nom = i_vue_epreuve != null ? (i_vue_epreuve.id + "_" + i_vue_epreuve.nom) : null;
                string directory = ExportTools.getDirectory(site, epreuve_nom, null);
                string filename = ExportTools.getFileName(type);
                string fileSave = directory + "/" + filename.Replace("/", "_");
                XsltArgumentList argsList = new XsltArgumentList();
                argsList.AddParam("istapis", "", "epreuve");

                XmlDocument xmlFeuilleCombat = ExportXML.CreateDocumentFeuilleCombat(DC, phase, null);
                ExportXML.AddPublicationInfo(ref xmlFeuilleCombat, config);
                ExportXML.AddClubs(ref xmlFeuilleCombat, DC);

                ExportHTML.ToHTMLSite(xmlFeuilleCombat, type, fileSave, argsList);
                urls.Add(fileSave + ".html");
            }

            if (phase.typePhase == (int)TypePhaseEnum.Poule)
            {
                ExportEnum type2 = ExportEnum.Site_Poule_Resultat;
                string epreuve_nom2 = i_vue_epreuve != null ? (i_vue_epreuve.id + "_" + i_vue_epreuve.nom) : null;
                string directory2 = ExportTools.getDirectory(site, epreuve_nom2, null);
                string filename2 = ExportTools.getFileName(type2);
                string fileSave2 = directory2 + "/" + filename2.Replace("/", "_");
                XsltArgumentList argsList2 = new XsltArgumentList();

                XmlDocument xmlResultat = ExportXML.CreateDocumentPhase(i_vue_epreuve, phase, DC);
                ExportXML.AddPublicationInfo(ref xmlResultat, config);
                ExportXML.AddCeintures(ref xmlResultat, DC);
                ExportXML.AddClubs(ref xmlResultat, DC);

                ExportHTML.ToHTMLSite(xmlResultat, type2, fileSave2, argsList2);
                urls.Add(fileSave2 + ".html");
                }
            else if (phase.typePhase == (int)TypePhaseEnum.Tableau)
            {
                ExportEnum type2 = ExportEnum.Site_Tableau_Competition;
                string epreuve_nom2 = i_vue_epreuve != null ? (i_vue_epreuve.id + "_" + i_vue_epreuve.nom) : null;
                string directory2 = ExportTools.getDirectory(site, epreuve_nom2, null);
                string filename2 = ExportTools.getFileName(type2);
                string fileSave2 = directory2 + "/" + filename2.Replace("/", "_");
                XsltArgumentList argsList2 = new XsltArgumentList();

                XmlDocument xmlResultat = ExportXML.CreateDocumentPhase(i_vue_epreuve, phase, DC);
                ExportXML.AddPublicationInfo(ref xmlResultat, config);
                ExportXML.AddCeintures(ref xmlResultat, DC);
                ExportXML.AddClubs(ref xmlResultat, DC);

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
        public static List<FileWithChecksum> GenereWebSiteClassement(JudoData DC, i_vue_epreuve_interface epreuve, ConfigurationExportSite config)
        {
            bool site = true;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            ExportEnum type = ExportEnum.Site_ClassementFinal;
            string epreuve_nom = epreuve != null ? (epreuve.id + "_" + epreuve.nom) : null;
            string directory = ExportTools.getDirectory(site, epreuve_nom, null);
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument xml = ExportXML.CreateDocumentEpreuve(DC, epreuve);
            ExportXML.AddPublicationInfo(ref xml, config);

            ExportXML.AddClubs(ref xml, DC);
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
        public static List<FileWithChecksum> GenereWebSiteAllTapis(JudoData DC, ConfigurationExportSite config)
        {
            bool site = true;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            // Genere les prochains combats de tous les tapis, istapis = alltapis (Se Prepare)  => feuille_matchs_site.xslt

            // TODO Voir pour mettre un parametre permettant de choisir le nombre de combats par tapis a afficher
            // TODO Ajouter une option pour afficher les combats en attente ou pas

            ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
            string directory = ExportTools.getDirectory(site, null, null);
            // string filename = ExportTools.getFileName(type) + "All"; //ExportTools.getFileName(type) + "_tapis_" + "All";
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("istapis", "", "alltapis");

            XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
            ExportXML.AddPublicationInfo(ref xml, config);

            ExportXML.AddClubs(ref xml, DC);

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
        public static List<FileWithChecksum> GenereWebSiteIndex(ConfigurationExportSite config)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            string directory = ExportTools.getDirectory(true, null, null);
            XDocument doc = new XDocument();

            XElement xcompetition = new XElement(ConstantXML.Competition);
            xcompetition.SetAttributeValue(ConstantXML.Competition_Titre, OutilsTools.TraiteChaine(OutilsTools.SubString(DialogControleur.Instance.ServerData.competition.nom, 0, 30)));

            doc.Add(xcompetition);

            // Load the style sheet.
            var resource = ResourcesTools.GetAssembyResource(ExportTools.GetXsltClassique(ExportEnum.Site_Index));
            XmlReader xsltReader = XmlReader.Create(resource);

            XslCompiledTransform xslt_index = new XslCompiledTransform();
            xslt_index.Load(xsltReader);

            // Create the FileStream.
            using (FileStream fs = new FileStream(directory + @"\index.html", FileMode.Create))
            {
                xslt_index.Transform(doc.ToXmlDocument(), null, fs);
            }
            // No need to regenerate those files, they are usually static unless they are updated
            // urls = urls.Concat(ExportTools.ExportStyleAndJS(true)).ToList();
            urls = urls.Concat(ExportTools.ExportStyleAndJS(true)).ToList();

            // Debug.WriteLine(string.Format("GenereWebSiteIndex - ExportStyleAndJS {0}", urls.Count));

            // No need to regenerate those files, they are usually static unless they are updated
            // urls = urls.Concat(ExportTools.ExportImg(true)).ToList();
            urls = urls.Concat(ExportTools.ExportImg(true)).ToList();

            // Debug.WriteLine(string.Format("GenereWebSiteIndex - ExportImg {0}", urls.Count));

            urls.Add(directory + @"\index.html");

            output = urls.Select(o => new FileWithChecksum(o)).ToList();

            // Debug.WriteLine(string.Format("GenereWebSiteIndex {0}", output.Count));

            return output;
        }

        /// <summary>
        /// Génére le menu
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteMenu(JudoData DC, ConfigurationExportSite config)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            bool site = true;
            ExportEnum type;
            string directory = ExportTools.getDirectory(site, null, null);

            XmlDocument docmenu = ExportXML.CreateDocumentMenu(DC);
            ExportXML.AddPublicationInfo(ref docmenu, config);

            // Genere le menu de d'avancement
            type = ExportEnum.Site_MenuAvancement;
            XsltArgumentList argsList = new XsltArgumentList();
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
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
        public static List<FileWithChecksum> GenereWebSiteAffectation(JudoData DC, ConfigurationExportSite config)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            bool site = true;
            ExportEnum type = ExportEnum.Site_AffectationTapis;
            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument docAffectation = ExportXML.CreateDocumentAffectationTapis(DC);
            ExportXML.AddPublicationInfo(ref docAffectation, config);

            ExportHTML.ToHTMLSite(docAffectation, type, fileSave, argsList);

            output.Add(new FileWithChecksum(fileSave + ".html"));
            return output;
        }
    }
}
