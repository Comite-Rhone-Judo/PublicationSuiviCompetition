using AppPublication.Controles;
using AppPublication.Tools.Enum;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using AppPublication.ExtensionNoyau;
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
using KernelImpl.Noyau.Structures;
using System.Collections;
using AppPublication.ExtensionNoyau.Deroulement;

namespace AppPublication.Export
{
    public static class ExportSite
    {
        private static XmlDocument _docParticipants = new XmlDocument();     // Instance partagees pour la generation des Participants

        private static List<XElement> _xClubs = new List<XElement>();          // Instance partagees pour la liste des clubs
        private static List<XElement> _xComites = new List<XElement>();          // Instance partagees pour la liste des comites
        private static List<XElement> _xSecteurs = new List<XElement>();          // Instance partagees pour la liste des secteurs
        private static List<XElement> _xLigues = new List<XElement>();          // Instance partagees pour la liste des ligues
        private static List<XElement> _xPays = new List<XElement>();          // Instance partagees pour la liste des pays

        /// <summary>
        /// Génére les éléments donnés d'une phase
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="phase">la phase</param>
        public static List<FileWithChecksum> GenereWebSitePhase(JudoData DC, Phase phase, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && phase != null && config != null && siteStruct != null)
            {

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
                    AddStructures(ref xmlFeuilleCombat);
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
                    AddStructures(ref xmlResultat);
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
                    AddStructures(ref xmlResultat);
                    LogTools.Logger.Debug("XML genere: '{0}'", xmlResultat.InnerXml);

                    ExportHTML.ToHTMLSite(xmlResultat, type2, fileSave2, argsList2);
                    urls.Add(fileSave2 + ".html");
                }

                // Genere les checksums des fichiers generes
                output = urls.Select(o => new FileWithChecksum(o)).ToList();
            }

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

            if (DC != null && epreuve != null && config != null && siteStruct != null)
            {
                ExportEnum type = ExportEnum.Site_ClassementFinal;
                string directory = siteStruct.RepertoireEpreuve(epreuve.id.ToString(), epreuve.nom);
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();

                XmlDocument xml = ExportXML.CreateDocumentEpreuve(DC, epreuve);
                ExportXML.AddPublicationInfo(ref xml, config);
                AddStructures(ref xml);
                LogTools.Logger.Debug("XML genere: '{0}'", xml.InnerXml);

                ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }

            return output;
        }

        /// <summary>
        /// Génére les premiers combats de tous les tapis
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteAllTapis(JudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && config != null && siteStruct != null)
            {
                // Genere les prochains combats de tous les tapis, istapis = alltapis (Se Prepare)  => feuille_matchs_site.xslt
                ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
                string directory = siteStruct.RepertoireCommon;
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                argsList.AddParam("istapis", "", "alltapis");

                XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
                ExportXML.AddPublicationInfo(ref xml, config);
                AddStructures(ref xml);
                LogTools.Logger.Debug("XML genere: '{0}'", xml.InnerXml);

                ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }

            return output;
        }

        /// <summary>
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteIndex(ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (config != null && siteStruct != null)
            {
                string directory = siteStruct.RepertoireCommon;
                XDocument doc = new XDocument();

                XElement xcompetition = new XElement(ConstantXML.Competition);
                xcompetition.SetAttributeValue(ConstantXML.Competition_Titre, OutilsTools.TraiteChaine(OutilsTools.SubString(DialogControleur.Instance.ServerData.competition.nom, 0, 30)));

                doc.Add(xcompetition);

                // Load the style sheet.
                var resource = ResourcesTools.GetAssembyResource(ExportTools.GetXsltClassique(ExportEnum.Site_Index));
                XmlReader xsltReader = XmlReader.Create(resource);

                XslCompiledTransform xslt_index = new XslCompiledTransform();
                xslt_index.Load(xsltReader);

                string indexfile = Path.Combine(directory, "index.html");

                // Create the FileStream.
                using (FileStream fs = new FileStream(indexfile, FileMode.Create))
                {
                    xslt_index.Transform(doc.ToXmlDocument(), null, fs);
                }
                // No need to regenerate those files, they are usually static unless they are updated
                urls = urls.Concat(ExportTools.ExportEmbeddedStyleAndJS(true, siteStruct)).ToList();

                // No need to regenerate those files, they are usually static unless they are updated
                urls = urls.Concat(ExportTools.ExportEmbeddedImg(true, siteStruct)).ToList();

                urls.Add(indexfile);

                output = urls.Select(o => new FileWithChecksum(o)).ToList();
            }
            return output;
        }

        /// <summary>
        /// Génére le menu
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteMenu(JudoData DC, ExtensionJudoData EDC, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && EDC != null && config != null && siteStruct != null)
            {

                ExportEnum type;
                string directory = siteStruct.RepertoireCommon;

                XmlDocument docmenu = ExportXML.CreateDocumentMenu(DC, EDC, siteStruct);
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

                // Genere le menu participants
                if (config.PublierParticipants)
                {
                    // Ajoute les informations necessaire pour les participants
                    ExportXML.AddPublicationInfo(ref docmenu, config);
                    AddStructures(ref docmenu);

                    type = ExportEnum.Site_MenuParticipants;
                    XsltArgumentList argsListPc = new XsltArgumentList();
                    string filenamePc = ExportTools.getFileName(type);
                    string fileSavePc = directory + "/" + filenamePc.Replace("/", "_");
                    ExportHTML.ToHTMLSite(docmenu, type, fileSavePc, argsListPc);
                    output.Add(new FileWithChecksum(fileSavePc + ".html"));
                }
            }

            return output;
        }

        /// <summary>
        /// Genere la page d'affectation des tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<FileWithChecksum> GenereWebSiteAffectation(JudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && config != null && siteStruct != null)
            {
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
            }
            return output;
        }


        /// <summary>
        /// Initialise les structures de donnees partagees pour la generation des documents XML
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        /// <param name="config"></param>
        public static void InitSharedData(JudoData DC, ExtensionJudoData EDC, ConfigurationExportSite config)
        {
            using (TimedLock.Lock(_xClubs))
            {
                _xClubs = ExportXML.GetClubs(DC);
            }
            using (TimedLock.Lock(_xComites))
            {
                _xComites = ExportXML.GetComites(DC);
            }
            using (TimedLock.Lock(_xSecteurs))
            {
                _xSecteurs = ExportXML.GetSecteurs(DC);
            }
            using (TimedLock.Lock(_xLigues))
            {
                _xLigues = ExportXML.GetLigues(DC);
            }
            using (TimedLock.Lock(_xPays))
            {
                _xPays = ExportXML.GetPays(DC);
            }
            using (TimedLock.Lock(_docParticipants))
            {
                _docParticipants = ExportXML.CreateDocumentParticipants(DC, EDC, config.ParticipantsParEntite);
                ExportXML.AddPublicationInfo(ref _docParticipants, config);
                AddStructures(ref _docParticipants);
                LogTools.Logger.Debug("XML genere: '{0}'", _docParticipants.InnerXml);
            }
        }

        /// <summary>
        /// Genere la page des participants
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<FileWithChecksum> GenereWebSiteParticipants(JudoData DC, ExtensionJudoData EDC, GroupeParticipants grp, ConfigurationExportSite config, ExportSiteStructure siteStruct)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && EDC != null && grp != null && config != null && siteStruct != null)
            {
                ExportEnum type = ExportEnum.Site_Participants;
                string directory = siteStruct.RepertoireGroupeParticipants(grp.Id);
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                argsList.AddParam("idgroupe", "", grp.Id);
                argsList.AddParam("idcompetition", "", grp.Competition);

                ExportHTML.ToHTMLSite(_docParticipants, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }
            return output;
        }

        /// <summary>
        /// Ajoute les informations de structure en cache
        /// </summary>
        /// <param name="doc"></param>
        private static void AddStructures(ref XmlDocument doc)
        {
            ExportXML.AddStructures(ref doc, _xClubs, _xComites, _xSecteurs, _xLigues, _xPays);
        }
    }
}