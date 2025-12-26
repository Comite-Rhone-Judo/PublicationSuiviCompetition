using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Tools.Enum;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System;
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
using AppPublication.Generation;

namespace AppPublication.Export
{
    public static class ExportSite
    {
        private const int kTailleMaxNomCompetition = 30;
        private static XmlDocument _docEngagements = new XmlDocument();     // Instance partagees pour la generation des engages

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
        public static List<FileWithChecksum> GenereWebSitePhase(IJudoData DC, Phase phase, ConfigurationExportSite config, ExportSiteStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            LogTools.Logger.Debug("Phase ({1}) '{0}'", phase?.libelle, phase?.id);

            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            int nbGen = 1;
            if (config.PublierProchainsCombats)
            {
                nbGen++;
            }

            progress?.Report(GenerationProgressInfo.InitInstance(workId, nbGen)); // Report the start of the task with the number of subtask

            if (DC != null && phase != null && config != null && siteStruct != null)
            {
                i_vue_epreuve_interface i_vue_epreuve = null;
                if (phase.isEquipe)
                {
                    i_vue_epreuve = DC.Organisation.VueEpreuveEquipes.FirstOrDefault(o => o.id == phase.epreuve);
                }
                else
                {
                    i_vue_epreuve = DC.Organisation.VueEpreuves.FirstOrDefault(o => o.id == phase.epreuve);
                }

                //Epreuve epreuve = DC.Epreuve.FirstOrDefault(o => o.id == phase.epreuve);
                Competition compet = DC.Organisation.Competitions.FirstOrDefault(o => o.id == i_vue_epreuve.competition);

                if (phase.typePhase == (int)TypePhaseEnum.Poule)
                {
                    ExportEnum type2 = ExportEnum.Site_Poule_Resultat;
                    string directory2 = siteStruct.RepertoireEpreuve(i_vue_epreuve.id.ToString(), i_vue_epreuve.nom);
                    string filename2 = ExportTools.getFileName(type2);
                    string fileSave2 = Path.Combine(directory2, filename2.Replace("/", "_"));
                    XsltArgumentList argsList2 = new XsltArgumentList();
                    AddStructureArgument(argsList2, siteStruct, fileSave2);

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
                    LogTools.DataLogger.Debug("XML genere: '{0}'", xmlResultat.InnerXml);

                    ExportHTML.ToHTMLSite(xmlResultat, type2, fileSave2, argsList2);
                    urls.Add(fileSave2 + ".html");
                    LogTools.Logger.Debug("Poule = {0}", urls.Count);

                    progress.Report(GenerationProgressInfo.ProgressInstance(workId, 1)); // Report the progress of the task
                }
                else if (phase.typePhase == (int)TypePhaseEnum.Tableau)
                {
                    ExportEnum type2 = ExportEnum.Site_Tableau_Competition;
                    string directory2 = siteStruct.RepertoireEpreuve(i_vue_epreuve.id.ToString(), i_vue_epreuve.nom);
                    string filename2 = ExportTools.getFileName(type2);
                    string fileSave2 = Path.Combine(directory2, filename2.Replace("/", "_"));
                    XsltArgumentList argsList2 = new XsltArgumentList();
                    AddStructureArgument(argsList2, siteStruct, fileSave2);

                    XmlDocument xmlResultat = ExportXML.CreateDocumentPhase(i_vue_epreuve, phase, DC);
                    ExportXML.AddPublicationInfo(ref xmlResultat, config);
                    ExportXML.AddCeintures(ref xmlResultat, DC);
                    AddStructures(ref xmlResultat);
                    LogTools.DataLogger.Debug("XML genere: '{0}'", xmlResultat.InnerXml);

                    ExportHTML.ToHTMLSite(xmlResultat, type2, fileSave2, argsList2);
                    urls.Add(fileSave2 + ".html");
                    LogTools.Logger.Debug("Tableau = {0}", urls.Count);

                    progress.Report(GenerationProgressInfo.ProgressInstance(workId, 1)); // Report the progress of the task
                }

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
                    AddStructureArgument(argsList, siteStruct, fileSave);

                    XmlDocument xmlFeuilleCombat = ExportXML.CreateDocumentFeuilleCombat(DC, phase, null);
                    ExportXML.AddPublicationInfo(ref xmlFeuilleCombat, config);
                    AddStructures(ref xmlFeuilleCombat);
                    LogTools.DataLogger.Debug("XML genere: '{0}'", xmlFeuilleCombat.InnerXml);

                    ExportHTML.ToHTMLSite(xmlFeuilleCombat, type, fileSave, argsList);
                    urls.Add(fileSave + ".html");
                    LogTools.Logger.Debug("ProchainsCombats = {0}", urls.Count);

                    progress.Report(GenerationProgressInfo.ProgressInstance(workId, 2)); // Report the progress of the task
                }

                // Genere les checksums des fichiers generes
                output = urls.Select(o => new FileWithChecksum(o)).ToList();
            }

            progress?.Report(GenerationProgressInfo.ProgressInstance(workId, nbGen)); // Report the end of the task
            return output;
        }

        /// <summary>
        /// Génére le classement d'une épreuve
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="epreuve"></param>
        public static List<FileWithChecksum> GenereWebSiteClassement(IJudoData DC, i_vue_epreuve_interface epreuve, ConfigurationExportSite config, ExportSiteStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(GenerationProgressInfo.InitInstance(workId, 1)); // Report the start of the task with the number of subtask

            if (DC != null && epreuve != null && config != null && siteStruct != null)
            {
                ExportEnum type = ExportEnum.Site_ClassementFinal;
                string directory = siteStruct.RepertoireEpreuve(epreuve.id.ToString(), epreuve.nom);
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                AddStructureArgument(argsList, siteStruct, fileSave);

                XmlDocument xml = ExportXML.CreateDocumentEpreuve(DC, epreuve);
                ExportXML.AddPublicationInfo(ref xml, config);
                AddStructures(ref xml);
                LogTools.DataLogger.Debug("XML genere: '{0}'", xml.InnerXml);

                ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }
            LogTools.Logger.Debug("Classement = {0}", output.Count);


            progress?.Report(GenerationProgressInfo.ProgressInstance(workId, 1)); // Report the end of the task
            return output;
        }

        /// <summary>
        /// Génére les premiers combats de tous les tapis
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteAllTapis(IJudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(GenerationProgressInfo.InitInstance(workId, 1)); // Report the start of the task with the number of subtask

            if (DC != null && config != null && siteStruct != null)
            {
                // Genere les prochains combats de tous les tapis, istapis = alltapis (Se Prepare)  => feuille_matchs_site.xslt
                ExportEnum type = ExportEnum.Site_FeuilleCombatTapis;
                string directory = siteStruct.RepertoireCommon();
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                argsList.AddParam("istapis", "", "alltapis");
                // si plus d'une competition et intitule commun configure, on l'utilise plutot que le titre d'une des competitions
                bool useIntituleCommun = (DC.Organisation.Competitions.Count() > 1) && config.UseIntituleCommun && !string.IsNullOrEmpty(config.IntituleCommun);
                argsList.AddParam("useIntituleCommun", "", useIntituleCommun.ToString().ToLower());
                AddStructureArgument(argsList, siteStruct, fileSave);

                XmlDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
                ExportXML.AddPublicationInfo(ref xml, config);
                AddStructures(ref xml);
                LogTools.DataLogger.Debug("XML genere: '{0}'", xml.InnerXml);

                ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }

            LogTools.Logger.Debug("ProchainsCombats Tapis = {0}", output.Count);


            progress?.Report( GenerationProgressInfo.ProgressInstance(workId, 1)); // Report the end of the task
            return output;
        }

        /// <summary>
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteIndex(IJudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            ExportEnum type;

            progress?.Report(GenerationProgressInfo.InitInstance(workId, 2)); // Report the start of the task with the number of subtask

            if (DC != null && config != null && siteStruct != null)
            {
                XmlDocument docindex = ExportXML.CreateDocumentIndex(DC, siteStruct);
                ExportXML.AddPublicationInfo(ref docindex, config);
                LogTools.DataLogger.Debug("XML genere: '{0}'", docindex.InnerXml);

                // Genere l'index
                type = ExportEnum.Site_Index;
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(siteStruct.RepertoireCommon(), filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                AddStructureArgument(argsList, siteStruct, fileSave);

                ExportHTML.ToHTMLSite(docindex, type, fileSave, argsList);
                output.Add(new FileWithChecksum(fileSave + ".html"));
                progress.Report(GenerationProgressInfo.ProgressInstance(workId, 1)); // Report the end of the task

                // No need to regenerate those files, they are usually static unless they are updated
                urls = urls.Concat(ExportTools.ExportEmbeddedStyleAndJS(true, siteStruct)).ToList();
                LogTools.Logger.Debug("GenereWebSiteIndex - ExportStyleAndJS {0}", urls.Count);

                // Genere les images "par defaut" contenues dans l'application et les images personnalises de l'utilisateur
                urls = urls.Concat(ExportTools.ExportEmbeddedImg(true, true, siteStruct)).ToList();
                LogTools.Logger.Debug("GenereWebSiteIndex - ExportImg {0}", urls.Count);

                output.AddRange(urls.Select(o => new FileWithChecksum(o)).ToList());

                // Genere le script de mise a jour
                type = ExportEnum.Site_FooterScript;
                string filenameFooter = ExportTools.getFileName(type);
                string fileSaveFooter = Path.Combine(siteStruct.RepertoireJs(), filenameFooter.Replace("/", "_"));
                XsltArgumentList argsListFooter = new XsltArgumentList();
                AddStructureArgument(argsListFooter, siteStruct, fileSaveFooter);
                ExportHTML.ToHTMLSite(docindex, type, fileSaveFooter, argsListFooter, "js");
                output.Add(new FileWithChecksum(fileSaveFooter + ".js"));
                progress.Report(GenerationProgressInfo.ProgressInstance(workId, 2)); // Report the end of the task

                LogTools.Logger.Debug("GenereWebSiteIndex {0}", output.Count);
            }

            progress.Report(GenerationProgressInfo.ProgressInstance(workId, 2)); // Report the end of the task
            return output;
        }

        /// <summary>
        /// Génére le menu
        /// </summary>
        /// <param name="DC"></param>
        public static List<FileWithChecksum> GenereWebSiteMenu(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config, ExportSiteStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            if (DC != null && EDC != null && config != null && siteStruct != null)
            {
                int nbGen = 2;
                if (config.PublierProchainsCombats)
                {
                    nbGen++;
                }
                if (config.PublierEngagements)
                {
                    nbGen++;
                }

                progress?.Report(GenerationProgressInfo.InitInstance(workId, nbGen)); // Report the start of the task with the number of subtask

                ExportEnum type;
                string directory = siteStruct.RepertoireCommon();

                XmlDocument docmenu = ExportXML.CreateDocumentMenu(DC, EDC, siteStruct);
                ExportXML.AddPublicationInfo(ref docmenu, config);
                LogTools.DataLogger.Debug("XML genere: '{0}'", docmenu.InnerXml);

                // Genere le menu de d'avancement
                type = ExportEnum.Site_MenuAvancement;
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                AddStructureArgument(argsList, siteStruct, fileSave);

                ExportHTML.ToHTMLSite(docmenu, type, fileSave, argsList);
                output.Add(new FileWithChecksum(fileSave + ".html"));
                progress.Report(GenerationProgressInfo.ProgressInstance(workId, 1)); // Report the progress of the task

                // Genere le menu de classement
                type = ExportEnum.Site_MenuClassement;
                string filename2 = ExportTools.getFileName(type);
                string fileSave2 = Path.Combine(directory, filename2.Replace("/", "_"));
                XsltArgumentList argsList2 = new XsltArgumentList();
                AddStructureArgument(argsList2, siteStruct, fileSave2);

                ExportHTML.ToHTMLSite(docmenu, type, fileSave2, argsList2);
                output.Add(new FileWithChecksum(fileSave2 + ".html"));
                progress.Report(GenerationProgressInfo.ProgressInstance(workId, 2)); // Report the progress of the task

                // Genere le menu de prochain combat
                if (config.PublierProchainsCombats)
                {
                    type = ExportEnum.Site_MenuProchainCombats;
                    string filenamePc = ExportTools.getFileName(type);
                    string fileSavePc = Path.Combine(directory, filenamePc.Replace("/", "_"));
                    XsltArgumentList argsListPc = new XsltArgumentList();
                    AddStructureArgument(argsListPc, siteStruct, fileSavePc);

                    ExportHTML.ToHTMLSite(docmenu, type, fileSavePc, argsListPc);
                    output.Add(new FileWithChecksum(fileSavePc + ".html"));
                    progress.Report(GenerationProgressInfo.ProgressInstance(workId, 3)); // Report the progress of the task
                }

                // Genere le menu engageements
                if (config.PublierEngagements)
                {
                    // Ajoute les informations necessaire pour les engages
                    ExportXML.AddPublicationInfo(ref docmenu, config);
                    AddStructures(ref docmenu);

                    type = ExportEnum.Site_MenuEngagements;
                    string filenamePart = ExportTools.getFileName(type);
                    string fileSavePart = Path.Combine(directory, filenamePart.Replace("/", "_"));
                    XsltArgumentList argsListPart = new XsltArgumentList();
                    AddStructureArgument(argsListPart, siteStruct, fileSavePart);

                    ExportHTML.ToHTMLSite(docmenu, type, fileSavePart, argsListPart);
                    output.Add(new FileWithChecksum(fileSavePart + ".html"));
                    progress.Report(GenerationProgressInfo.ProgressInstance(workId, 4)); // Report the progress of the task
                }
            }

            LogTools.Logger.Debug("Menu = {0}", output.Count);


            return output;
        }

        /// <summary>
        /// Genere la page d'affectation des tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<FileWithChecksum> GenereWebSiteAffectation(IJudoData DC, ConfigurationExportSite config, ExportSiteStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(GenerationProgressInfo.InitInstance(workId, 1)); // Report the start of the task with the number of subtask

            if (DC != null && config != null && siteStruct != null)
            {
                ExportEnum type = ExportEnum.Site_AffectationTapis;
                string directory = siteStruct.RepertoireCommon();
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                AddStructureArgument(argsList, siteStruct, fileSave);

                XmlDocument docAffectation = ExportXML.CreateDocumentAffectationTapis(DC);
                ExportXML.AddPublicationInfo(ref docAffectation, config);
                LogTools.DataLogger.Debug("XML genere: '{0}'", docAffectation.InnerXml);

                ExportHTML.ToHTMLSite(docAffectation, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }

            LogTools.Logger.Debug("Affectation = {0}", output.Count);


            progress?.Report(GenerationProgressInfo.ProgressInstance(workId, 1)); // Report the end of the task
            return output;
        }


        /// <summary>
        /// Initialise les structures de donnees partagees pour la generation des documents XML
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        /// <param name="config"></param>
        public static void InitSharedData(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config)
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
            using (TimedLock.Lock(_docEngagements))
            {
                _docEngagements = ExportXML.CreateDocumentEngagements(DC, EDC);
                ExportXML.AddPublicationInfo(ref _docEngagements, config);
                AddStructures(ref _docEngagements);
                LogTools.DataLogger.Debug("XML genere: '{0}'", _docEngagements.InnerXml);
            }
        }

        /// <summary>
        /// Genere la page des engages
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<FileWithChecksum> GenereWebSiteEngagements(IJudoData DC, ExtendedJudoData EDC, List<GroupeEngagements> grps, ConfigurationExportSite config, ExportSiteStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && EDC != null && grps != null && config != null && siteStruct != null)
            {
                int nbGrps = grps.Count;

                progress?.Report(GenerationProgressInfo.InitInstance(workId, nbGrps)); // Report the start of the task with the number of subtask


                for (int i = 0; i < nbGrps; i++) {
                    GroupeEngagements grp = grps[i];
                    ExportEnum type = ExportEnum.Site_Engagements;
                    string filename = ExportTools.getFileName(type);
                    string directory = siteStruct.RepertoireGroupeEngagements(grp.Id);
                    string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                    XsltArgumentList argsList = new XsltArgumentList();
                    argsList.AddParam("idgroupe", "", grp.Id);
                    argsList.AddParam("idcompetition", "", grp.Competition);
                    AddStructureArgument(argsList, siteStruct, fileSave);

                    ExportHTML.ToHTMLSite(_docEngagements, type, fileSave, argsList);

                    output.Add(new FileWithChecksum(fileSave + ".html"));

                    progress?.Report( GenerationProgressInfo.ProgressInstance(workId, i+1)); // Report the end of the task
                }
            }

            LogTools.Logger.Debug("Engagements = {0}", output.Count);


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

        /// <summary>
        /// Nettoie le path specifie pour passer de Repertoire Windows à URL
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string PathForUrl(string path)
        {
            string output = path.Replace('\\', '/');

            /*
             if(output.First() != '/')
            {
                output = string.Format("/{0}", output);
            }
            */

            return output;
        }

        /// <summary>
        /// Ajoute les arguments de structure du site pour les templates xslt
        /// </summary>
        /// <param name="argsList">La liste d'argument a actualiser</param>
        /// <param name="siteStruct">La structure du site</param>
        /// <param name="targetFile">Le fichier HTML cible</param>
        private static void AddStructureArgument(XsltArgumentList argsList, ExportSiteStructure siteStruct, string targetFile)
        {
            siteStruct.TargetPath = targetFile;

            // Ajoute les parametres en relatif par rapport a la position du fichier
            argsList.AddParam("imgPath", "", PathForUrl(siteStruct.RepertoireImg(relatif: true)));
            argsList.AddParam("jsPath", "", PathForUrl(siteStruct.RepertoireJs(relatif: true)));
            argsList.AddParam("cssPath", "", PathForUrl(siteStruct.RepertoireCss(relatif: true)));
            argsList.AddParam("commonPath", "", PathForUrl(siteStruct.RepertoireCommon(relatif: true)));
            argsList.AddParam("competitionPath", "", PathForUrl(siteStruct.RepertoireCompetition(relatif: true)));
        }
    }
}