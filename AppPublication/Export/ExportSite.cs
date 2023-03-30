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
        /// Création du menu (pour le site)
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static XmlDocument CreateDocumentMenu(JudoData DC)
        {
            XDocument doc = new XDocument();
            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);

            IList<Competition> competitions = DC.Organisation.Competitions.ToList();
            IList<Epreuve> epreuves1 = DC.Organisation.Epreuves.ToList();
            IList<Epreuve_Equipe> epreuves2 = DC.Organisation.EpreuveEquipes.ToList();
            //IList<i_vue_epreuve_interface> vepreuves = DC.Organisation.vepreuves.ToList();
            IList<Phase> phases = DC.Deroulement.Phases.ToList();

            foreach (Competition competition in competitions)
            {
                XElement xcompetition = competition.ToXmlInformations();
                xcompetitions.Add(xcompetition);

                for (int i = 0; i <= competition.nbTapis; i++)
                {
                    string directory = ExportTools.getDirectory(true, null, null);

                    XElement xtapis = new XElement(ConstantXML.Tapis);
                    xtapis.SetAttributeValue(ConstantXML.Tapis, i);
                    //xtapis.SetAttributeValue(ConstantXML.Directory, directory);

                    xtapis.SetAttributeValue(ConstantXML.Tapis, i);

                    xcompetition.Add(xtapis);
                }

                IList<i_vue_epreuve_interface> epreuves_compet = null;
                if(competition.IsEquipe())
                {
                    epreuves_compet = DC.Organisation.vepreuves_equipe.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }
                else
                {
                    epreuves_compet = DC.Organisation.vepreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }
                

                foreach (i_vue_epreuve_interface ep in epreuves_compet)
                {
                    if (phases.Count(o => o.epreuve == ep.id && o.etat > (int)EtatPhaseEnum.Cree) == 0)
                    {
                        continue;
                    }

                    //i_vue_epreuve ep = vepreuves.FirstOrDefault(o => o.id == epreuve.id);
                    string epreuve_nom = ep != null ? (ep.id + "_" + ep.nom) : null;
                    string directory = ExportTools.getDirectory(true, epreuve_nom, null);
                    string directory2 = ExportTools.getDirectory(true, null, null).Replace("/common", "");
                    int index = directory.IndexOf(@"\site\");

                    XElement xepreuve = ep.ToXml(DC);
                    xepreuve.SetAttributeValue(ConstantXML.Directory, directory.Replace(directory2, ""));
                    xcompetition.Add(xepreuve);

                    XElement xphases = new XElement(ConstantXML.Phases);
                    xepreuve.Add(xphases);

                    foreach (Phase phase in DC.Deroulement.Phases.Where(o => o.epreuve == ep.id))
                    {
                        XElement xphase = phase.ToXml();
                        xphases.Add(xphase);
                    }
                }
            }

            return doc.ToXmlDocument();
        }

        /// <summary>
        /// Génére les éléments donnés d'une phase
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="phase">la phase</param>
        public static List<string> GenereWebSitePhase(JudoData DC, Phase phase)
        {
            List<string> urls = new List<string>();

            i_vue_epreuve_interface i_vue_epreuve = null;
            if(phase.isEquipe)
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
            ExportEnum type = ExportEnum.Site_FeuilleCombat;

            string epreuve_nom = i_vue_epreuve != null ? (i_vue_epreuve.id + "_" + i_vue_epreuve.nom) : null;
            string directory = ExportTools.getDirectory(site, epreuve_nom, null);
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("istapis", "", "epreuve");

            XmlDocument xmlFeuilleCombat = ExportXML.CreateDocumentFeuilleCombat(DC, phase, null);
            ExportXML.AddClubs(ref xmlFeuilleCombat, DC);

            ExportHTML.ToHTMLSite(xmlFeuilleCombat, type, fileSave, argsList);
            urls.Add(fileSave + ".html");
            if (phase.typePhase == (int)TypePhaseEnum.Poule)
            {
                ExportEnum type2 = ExportEnum.Site_Poule_Resultat;
                string epreuve_nom2 = i_vue_epreuve != null ? (i_vue_epreuve.id + "_" + i_vue_epreuve.nom) : null;
                string directory2 = ExportTools.getDirectory(site, epreuve_nom2, null);
                string filename2 = ExportTools.getFileName(type2);
                string fileSave2 = directory2 + "/" + filename2.Replace("/", "_");
                XsltArgumentList argsList2 = new XsltArgumentList();

                XmlDocument xmlResultat = ExportXML.CreateDocumentPhase(i_vue_epreuve, phase, DC);
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
                ExportXML.AddCeintures(ref xmlResultat, DC);
                ExportXML.AddClubs(ref xmlResultat, DC);

                ExportHTML.ToHTMLSite(xmlResultat, type2, fileSave2, argsList2);
                urls.Add(fileSave2 + ".html");
            }

            return urls;
        }

        /// <summary>
        /// Génére les élément d'un tapis donné
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="tapis"></param>
        public static List<string> GenereWebSiteTapis(JudoData DC, int tapis)
        {
            bool site = true;
            ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type) + tapis;
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("istapis", "", "tapis");

            XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, tapis);
            ExportXML.AddClubs(ref xml, DC);
            ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);
            return new List<string> { fileSave + ".html" };
        }

        /// <summary>
        /// Génére le classement d'une épreuve
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="epreuve"></param>
        public static List<string> GenereWebSiteClassement(JudoData DC, i_vue_epreuve_interface epreuve)
        {
            bool site = true;
            ExportEnum type = ExportEnum.Site_ClassementFinal;
            string epreuve_nom = epreuve != null ? (epreuve.id + "_" + epreuve.nom) : null;
            string directory = ExportTools.getDirectory(site, epreuve_nom, null);
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument xml = ExportXML.CreateDocumentEpreuve(DC, epreuve);
            ExportXML.AddClubs(ref xml, DC);
            ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);
            return new List<string> { fileSave + ".html" };
        }

        /// <summary>
        /// Génére les premiers combats de tous les tapis
        /// </summary>
        /// <param name="DC"></param>
        public static List<string> GenereWebSiteAllTapis(JudoData DC)
        {
            bool site = true;
            ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type) + "All"; //ExportTools.getFileName(type) + "_tapis_" + "All";
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("istapis", "", "alltapis");

            XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
            ExportXML.AddClubs(ref xml, DC);

            ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);
            return new List<string> { fileSave + ".html" };
        }

        /// <summary>
        /// Génére les premiers combats de tous les tapis
        /// </summary>
        /// <param name="DC"></param>
        public static List<string> GenereWebSitePrintTapis(JudoData DC, int nb)
        {
            bool site = true;
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

            XmlDocument docmenu = ExportSite.CreateDocumentMenu(DC);

            argsList.AddParam("style", "", ExportTools.getStyleDirectory(site: true));
            argsList.AddParam("js", "", ExportTools.getJS());
            //argsList.AddParam("menu", "", ExportTools.getDirectory(true, null, null) + @"\menu.html");
            string xslt = ExportTools.GetXsltSite(type);
            ExportHTML.ToHTML(docmenu, fileSave, argsList, xslt);

            return new List<string> { fileSave + ".html" };
        }

        /// <summary>
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public static List<string> GenereWebSiteIndex()
        {
            List<string> urls = new List<string>();

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
            urls = urls.Concat(ExportTools.ExportStyleAndJS(true)).ToList();
            urls = urls.Concat(ExportTools.ExportImg(true)).ToList();
            urls.Add(directory + @"\index.html");
            return urls;
        }

        /// <summary>
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public static List<string> GenereWebSiteMenu(JudoData DC)
        {
            List<string> urls = new List<string>();

            bool site = true;
            ExportEnum type = ExportEnum.Site_Menu;
            string directory = ExportTools.getDirectory(site, null, null);
            string filename = ExportTools.getFileName(type);
            string fileSave = directory + "/" + filename.Replace("/", "_");
            XsltArgumentList argsList = new XsltArgumentList();

            XmlDocument docmenu = ExportSite.CreateDocumentMenu(DC);

            ExportHTML.ToHTMLSite(docmenu, type, fileSave, argsList);

            urls.Add(fileSave + ".html");
            urls.Add(fileSave + ".xml");
            urls = urls.Concat(GenereWebSitePrintTapis(DC, 0)).ToList();
            urls = urls.Concat(GenereWebSitePrintTapis(DC, 1)).ToList();
            urls = urls.Concat(GenereWebSitePrintTapis(DC, 2)).ToList();
            urls = urls.Concat(GenereWebSitePrintTapis(DC, 4)).ToList();

            return urls;
           
            //ExportHTML.ToHTML_Menu(docmenu);
            //return new List<string> { ExportTools.getDirectory(true, null, null) + @"\menu.html" };
        }

        /// <summary>
        /// Générer le site
        /// </summary>
        /// <param name="DC"></param>
        public static List<string> GenereWebSite(JudoData DC)
        {
            List<string> urls = new List<string>();
            if (DialogControleur.Instance.GestionSite == null)
            {
                return urls;
            }

            urls = urls.Concat(ExportTools.ExportStyleAndJS(true)).ToList();
            urls = urls.Concat(ExportTools.ExportImg(true)).ToList();
            urls = urls.Concat(GenereWebSiteIndex()).ToList();
            urls = urls.Concat(GenereWebSiteMenu(DC)).ToList();
            urls = urls.Concat(GenereWebSiteAllTapis(DC)).ToList();

            List<Competition> competitions = DC.Organisation.Competitions.ToList();
            List<Phase> phases = DC.Deroulement.Phases.ToList();

            int nbtapis = competitions.Max(o => o.nbTapis);
            for (int i = 0; i <= nbtapis; i++)
            {
                urls = urls.Concat(GenereWebSiteTapis(DC, i)).ToList();
            }

            foreach (Competition compet in competitions)
            {
                List<i_vue_epreuve_interface> epreuves = null;
                if(compet.IsEquipe())
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
                        urls = urls.Concat(GenereWebSitePhase(DC, phase)).ToList();
                    }
                    urls = urls.Concat(GenereWebSiteClassement(DC, i_vue_epreuve)).ToList();
                }
            }
            return urls;
        }

    }
}
