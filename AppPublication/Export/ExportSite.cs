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
using AppPublication.Tools;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;
using System.Diagnostics;

namespace AppPublication.Export
{
    public static class ExportSite
    {
        /// <summary>
        /// Génére les éléments donnés d'une phase
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="phase">la phase</param>
        public static List<FileWithChecksum> GenereWebSitePhase(JudoData DC, Phase phase, bool publierProchainsCombats, bool publierAffectationTapis, long delaiActualisationClientSec)
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
            if (publierProchainsCombats)
            {
                ExportEnum type = ExportEnum.Site_FeuilleCombat;

                string epreuve_nom = i_vue_epreuve != null ? (i_vue_epreuve.id + "_" + i_vue_epreuve.nom) : null;
                string directory = ExportTools.getDirectory(site, epreuve_nom, null);
                string filename = ExportTools.getFileName(type);
                string fileSave = directory + "/" + filename.Replace("/", "_");
                XsltArgumentList argsList = new XsltArgumentList();
                argsList.AddParam("istapis", "", "epreuve");

                XmlDocument xmlFeuilleCombat = ExportXML.CreateDocumentFeuilleCombat(DC, phase, null);
                ExportXML.AddPublicationInfo(ref xmlFeuilleCombat, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);
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
                ExportXML.AddPublicationInfo(ref xmlResultat, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);
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
                ExportXML.AddPublicationInfo(ref xmlResultat, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);
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
        /// Génére les élément d'un tapis donné
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="tapis"></param>
        public static List<FileWithChecksum> GenereWebSiteTapis(JudoData DC, int tapis, bool publierProchainsCombats, bool publierAffectationTapis, long delaiActualisationClientSec)
        {
            bool site = true;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type) + tapis;
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("istapis", "", "tapis");

            XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, tapis);
            ExportXML.AddPublicationInfo(ref xml, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);

            ExportXML.AddClubs(ref xml, DC);
            ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);
            // return new List<string> { fileSave + ".html" };
            
            output.Add(new FileWithChecksum(fileSave + ".html"));
            // Debug.WriteLine(string.Format("GenereWebSiteTapis {0}", output.Count));

            return output; 
        }

        /// <summary>
        /// Génére le classement d'une épreuve
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="epreuve"></param>
        public static List<FileWithChecksum> GenereWebSiteClassement(JudoData DC, i_vue_epreuve_interface epreuve, bool publierProchainsCombats, bool publierAffectationTapis, long delaiActualisationClientSec)
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
            ExportXML.AddPublicationInfo(ref xml, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);

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
        public static List<FileWithChecksum> GenereWebSiteAllTapis(JudoData DC, bool publierProchainsCombats, bool publierAffectationTapis, long delaiActualisationClientSec)
        {
            bool site = true;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type) + "All"; //ExportTools.getFileName(type) + "_tapis_" + "All";
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("istapis", "", "alltapis");

            XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
            ExportXML.AddPublicationInfo(ref xml, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);

            ExportXML.AddClubs(ref xml, DC);

            ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);
            // return new List<string> { fileSave + ".html" };
            
            output.Add(new FileWithChecksum(fileSave + ".html"));
            // Debug.WriteLine(string.Format("GenereWebSiteAllTapis {0}", output.Count));

            return output;
        }

        /// <summary>
        /// Génére les premiers combats de tous les tapis
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSitePrintTapis(JudoData DC, int nb, bool publierProchainsCombats, bool publierAffectationTapis, long delaiActualisationClientSec)
        {
            bool site = true;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            ExportEnum type = ExportEnum.Site_Tapis1;
            switch (nb)
            {
                case 0:
                    type = ExportEnum.Site_ListTapis;
                    break;
                case 2:
                    type = ExportEnum.Site_Tapis2;
                    break;
                case 4:
                    type = ExportEnum.Site_Tapis4;
                    break;
                default:
                    type = ExportEnum.Site_Tapis1;
                    break;
            }

            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument docmenu = ExportXML.CreateDocumentMenu(DC);
            ExportXML.AddPublicationInfo(ref docmenu, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);

            argsList.AddParam("style", "", ExportTools.getStyleDirectory(site: true));
            argsList.AddParam("js", "", ExportTools.getJS());
            //argsList.AddParam("menu", "", ExportTools.getDirectory(true, null, null) + @"\menu.html");
            string xslt = ExportTools.GetXsltSite(type);
            ExportHTML.ToHTML(docmenu, fileSave, argsList, xslt);

            // return new List<string> { fileSave + ".html" };
            output.Add(new FileWithChecksum(fileSave + ".html"));
            // Debug.WriteLine(string.Format("GenereWebSitePrintTapis {0}", output.Count));

            return output;
        }

        /// <summary>
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteIndex(long delaiActualisationClientSec)
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
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteMenu(JudoData DC, bool publierProchainsCombats, bool publierAffectationTapis, long delaiActualisationClientSec)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            bool site = true;
            ExportEnum type;
            string directory = ExportTools.getDirectory(site, null, null);

            XmlDocument docmenu = ExportXML.CreateDocumentMenu(DC);
            ExportXML.AddPublicationInfo(ref docmenu, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);
            
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

            // output.Add(new FileWithChecksum(fileSave + ".xml"));
            if (publierProchainsCombats)
            {
                output = output.Concat(GenereWebSitePrintTapis(DC, 0, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec)).ToList();
                output = output.Concat(GenereWebSitePrintTapis(DC, 1, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec)).ToList();
                output = output.Concat(GenereWebSitePrintTapis(DC, 2, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec)).ToList();
                output = output.Concat(GenereWebSitePrintTapis(DC, 4, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec)).ToList();
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
        public static List<FileWithChecksum> GenereWebSiteAffectation(JudoData DC, bool publierProchainsCombats, bool publierAffectationTapis, long delaiActualisationClientSec)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            bool site = true;
            ExportEnum type = ExportEnum.Site_AffectationTapis;
            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument docAffectation = ExportXML.CreateDocumentAffectationTapis(DC);
            ExportXML.AddPublicationInfo(ref docAffectation, publierProchainsCombats, publierAffectationTapis, delaiActualisationClientSec);

            ExportHTML.ToHTMLSite(docAffectation, type, fileSave, argsList);

            output.Add(new FileWithChecksum(fileSave + ".html"));
            return output;
        }

        /// <summary>
        /// Générer le site complet - DEPRECATED
        /// </summary>
        /// <param name="DC"></param>
        /*
        public static List<FileWithChecksum> GenereWebSite(JudoData DC)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DialogControleur.Instance.GestionSite != null)
            {
                urls = urls.Concat(ExportTools.ExportStyleAndJS(true)).ToList();
                urls = urls.Concat(ExportTools.ExportImg(true)).ToList();

                output = output.Concat(urls.Select(o => new FileWithChecksum(o)).ToList()).ToList();
                output = output.Concat(GenereWebSiteIndex()).ToList();
                output = output.Concat(GenereWebSiteMenu(DC, false, true)).ToList();
                output = output.Concat(GenereWebSiteAllTapis(DC)).ToList();
                output = output.Concat(GenereWebSiteAffectation(DC)).ToList();

                List<Competition> competitions = DC.Organisation.Competitions.ToList();
                List<Phase> phases = DC.Deroulement.Phases.ToList();

                int nbtapis = competitions.Max(o => o.nbTapis);
                for (int i = 0; i <= nbtapis; i++)
                {
                    output = output.Concat(GenereWebSiteTapis(DC, i)).ToList();
                }

                foreach (Competition compet in competitions)
                {
                    List<i_vue_epreuve_interface> epreuves = null;
                    if (compet.IsEquipe())
                    {
                        epreuves = DC.Organisation.vepreuves_equipe.Where(o => o.competition == compet.id).Cast<i_vue_epreuve_interface>().ToList();
                    }
                    else
                    {
                        epreuves = DC.Organisation.vepreuves.Where(o => o.competition == compet.id).Cast<i_vue_epreuve_interface>().ToList();
                    }

                    foreach (i_vue_epreuve_interface i_vue_epreuve in epreuves)
                    {
                        foreach (Phase phase in phases.Where(o => o.epreuve == i_vue_epreuve.id))
                        {
                            output = output.Concat(GenereWebSitePhase(DC, phase)).ToList();
                        }
                        output = output.Concat(GenereWebSiteClassement(DC, i_vue_epreuve)).ToList();
                    }
                }
                // output = urls.Select(o => new FileChecksum(o)).ToList();
            }

            // Debug.WriteLine(string.Format("GenereWebSite {0}", output.Count));
            return output;
        }
        */
    }
}
