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
using System.Xml.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using Tools.Enum;
using Tools.Export;
using Tools.Files;
using Tools.Logging;
using Tools.Outils;
using Tools.Threading;

namespace AppPublication.Export
{
    public class ExportSite<TContext> : ExportSiteBase<TContext> where TContext : ExportSharedContextBase
    {
        #region CONSTRUCTEURS
        public ExportSite(TContext context) : base(context)
        {
        }
        #endregion

        #region METHODES PRIVEES
        /// <summary>
        /// Ajoute les arguments de structure du site pour les templates xslt
        /// </summary>
        /// <param name="argsList">La liste d'argument a actualiser</param>
        /// <param name="siteStruct">La structure du site</param>
        /// <param name="targetFile">Le fichier HTML cible</param>
        protected override void AddStructureArgument(XsltArgumentList argsList, ExportStructureBase siteStruct, string targetFile)
        {
            ExportSiteStructure theSiteStruct = siteStruct as ExportSiteStructure;

            // Ajoute les repertoires de base de la structure
            theSiteStruct.TargetPath = targetFile;
            base.AddStructureArgument(argsList, theSiteStruct, targetFile);

            // Ajoute le repertoire common
            argsList.AddParam("commonPath", "", PathForUrl(theSiteStruct.RepertoireCommon(relatif: true)));
        }

        /// <summary>
        /// Génère le chemin de sauvegarde complet et standardisé pour un fichier d'export lié à une épreuve.
        /// </summary>
        /// <param name="siteStruct">La structure de répertoires du site</param>
        /// <param name="epreuveId">L'identifiant de l'épreuve</param>
        /// <param name="epreuveNom">Le nom de l'épreuve</param>
        /// <param name="exportType">Le type de fichier à exporter</param>
        /// <returns>Le chemin complet du fichier</returns>
        private static string GetFileSavePath(ExportSiteStructure siteStruct, int epreuveId, string epreuveNom, ExportEnum exportType)
        {
            string directory = siteStruct.RepertoireEpreuve(epreuveId.ToString(), epreuveNom);
            string filename = ExportTools.getFileName(exportType).Replace("/", "_");

            return Path.Combine(directory, filename);
        }
        #endregion

        #region METHODES PUBLIQUES

        /// <summary>
        /// Génére les éléments donnés d'une phase
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="phase">la phase</param>
        /// <summary>
        /// Génère les fichiers HTML pour une phase spécifique (Poule ou Tableau) et optionnellement les prochains combats.
        /// Retourne la liste des fichiers générés avec leur checksum pour le suivi.
        /// </summary>
        public List<FileWithChecksum> GenereWebSitePhase(IJudoData DC, Phase phase, ExportSharedContext ctx, ExportSiteStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            LogTools.Logger.Debug("Phase ({1}) '{0}'", phase?.libelle, phase?.id);

            ConfigurationExportSite config = ctx.Config;
            ExportSiteStructure siteStructure = (ExportSiteStructure)structRep.Clone();

            List<FileWithChecksum> output = new List<FileWithChecksum>();

            int nbGen = config.PublierProchainsCombats ? 2 : 1;
            progress?.Report(BatchProgressInfo.Init(nbGen));

            if (DC != null && phase != null && config != null && siteStructure != null)
            {
                i_vue_epreuve_interface vueEpreuve = phase.isEquipe
                    ? (i_vue_epreuve_interface)DC.Organisation.VueEpreuveEquipes.FirstOrDefault(o => o.id == phase.epreuve)
                    : DC.Organisation.VueEpreuves.FirstOrDefault(o => o.id == phase.epreuve);

                if (vueEpreuve == null) return output;

                // --- 1. TRAITEMENTS POULE / TABLEAU ---
                if (phase.typePhase == (int)TypePhaseEnum.Poule || phase.typePhase == (int)TypePhaseEnum.Tableau)
                {
                    bool isPoule = phase.typePhase == (int)TypePhaseEnum.Poule;

                    ExportEnum exportType = isPoule ? ExportEnum.Site_Poule_Resultat : ExportEnum.Site_Tableau_Competition;
                    string savePath = GetFileSavePath(siteStructure, vueEpreuve.id, vueEpreuve.nom, exportType);

                    XsltArgumentList xsltArgs = new XsltArgumentList();
                    AddStructureArgument(xsltArgs, siteStructure, savePath);

                    if (isPoule)
                    {
                        int typePoule = config.PouleEnColonnes ? (config.PouleToujoursEnColonnes ? (int)TypePouleEnum.Colonnes : (int)TypePouleEnum.Auto) : (int)TypePouleEnum.Diagonale;
                        xsltArgs.AddParam("typePoule", "", typePoule);
                        xsltArgs.AddParam("tailleMaxPouleColonne", "", config.TailleMaxPouleColonnes);
                    }

                    XDocument xmlResultat = ExportXML.CreateDocumentPhase(vueEpreuve, phase, DC);
                    ctx.AddFullXmlContext(xmlResultat);
                    LogTools.DebugLogData(xmlResultat);

                    ExportHTML.ToHTMLSite(xmlResultat, exportType, savePath, xsltArgs);

                    output.Add(new FileWithChecksum($"{savePath}.html"));
                    LogTools.Logger.Debug("{0} = 1", isPoule ? "Poule" : "Tableau");

                    progress?.Report(BatchProgressInfo.Step(1));
                }

                // --- 2. PROCHAINS COMBATS ---
                if (config.PublierProchainsCombats)
                {
                    ExportEnum exportType = ExportEnum.Site_FeuilleCombat;
                    string savePath = GetFileSavePath(siteStructure, vueEpreuve.id, vueEpreuve.nom, exportType);

                    XsltArgumentList xsltArgs = new XsltArgumentList();
                    xsltArgs.AddParam("istapis", "", "epreuve");
                    AddStructureArgument(xsltArgs, siteStructure, savePath);

                    XDocument xmlFeuilleCombat = ExportXML.CreateDocumentFeuilleCombat(DC, phase, null);
                    ctx.AddFullXmlContext(xmlFeuilleCombat);
                    LogTools.DebugLogData(xmlFeuilleCombat);

                    ExportHTML.ToHTMLSite(xmlFeuilleCombat, exportType, savePath, xsltArgs);

                    output.Add(new FileWithChecksum($"{savePath}.html"));
                    LogTools.Logger.Debug("ProchainsCombats = 1");

                    progress?.Report(BatchProgressInfo.Step(2));
                }
            }

            progress?.Report(BatchProgressInfo.Step(nbGen));
            return output;
        }

        /// <summary>
        /// Génère le classement d'une épreuve au format HTML.
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="epreuve"></param>
        /// <param name="ctx"></param>
        /// <param name="structRep"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteClassement(IJudoData DC, i_vue_epreuve_interface epreuve, ExportSharedContext ctx, ExportSiteStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            // Clone la structure de répertoires pour le contexte multi-thread
            ExportSiteStructure siteStructure = (ExportSiteStructure)structRep.Clone();

            progress?.Report(BatchProgressInfo.Init(1));

            if (DC != null && epreuve != null && ctx != null && siteStructure != null)
            {
                ExportEnum exportType = ExportEnum.Site_ClassementFinal;

                // Utilisation de la méthode mutualisée pour le chemin
                string savePath = GetFileSavePath(siteStructure, epreuve.id, epreuve.nom, exportType);

                XsltArgumentList xsltArgs = new XsltArgumentList();
                AddStructureArgument(xsltArgs, siteStructure, savePath);

                // 1. Génération du document de base
                XDocument xmlClassement = ExportXML.CreateDocumentEpreuve(DC, epreuve);

                // 2. Enrichissement via le contexte (Porte la Config, les structures et les infos de publication)
                ctx.AddFullXmlContext(xmlClassement);

                LogTools.DebugLogData(xmlClassement);

                // 3. Transformation HTML
                ExportHTML.ToHTMLSite(xmlClassement, exportType, savePath, xsltArgs);

                output.Add(new FileWithChecksum($"{savePath}.html"));
            }

            LogTools.Logger.Debug("Classement = {0}", output.Count);
            progress?.Report(BatchProgressInfo.Step(1));

            return output;
        }

        /// <summary>
        /// Génére les premiers combats de tous les tapis
        /// </summary>
        /// <param name="DC"></param>
        public List<FileWithChecksum> GenereWebSiteAllTapis(IJudoData DC, ConfigurationExportSite config, ExportSiteStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            // Clone la structure de repertoires pour ne pas l'altérer dans le contexte multi-thread (changement de path)
            ExportSiteStructure siteStruct = (ExportSiteStructure)structRep.Clone();

            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(BatchProgressInfo.Init(1)); // Report the start of the task with the number of subtask

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

                XDocument xml = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
                ExportXML.AddPublicationInfo(ref xml, config);
                AddStructures(ref xml);
                LogTools.DebugLogData(xml);

                ExportHTML.ToHTMLSite(xml, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }

            LogTools.Logger.Debug("ProchainsCombats Tapis = {0}", output.Count);


            progress?.Report(BatchProgressInfo.Step(1)); // Report the end of the task
            return output;
        }

        /// <summary>
        /// Génére L'index
        /// </summary>
        /// <param name="DC"></param>
        public List<FileWithChecksum> GenereWebSiteIndex(IJudoData DC, ConfigurationExportSite config, ExportSiteStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            List<string> urls = new List<string>();
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            // Clone la structure de repertoires pour ne pas l'altérer dans le contexte multi-thread (changement de path)
            ExportSiteStructure siteStruct = (ExportSiteStructure) structRep.Clone();

            ExportEnum type;

            progress.Report(BatchProgressInfo.Init(2)); // Report the start of the task with the number of subtask

            if (DC != null && config != null && siteStruct != null)
            {
                XDocument docindex = ExportXML.CreateDocumentIndex(DC, siteStruct);
                ExportXML.AddPublicationInfo(ref docindex, config);
                LogTools.DebugLogData(docindex);

                // Genere l'index
                type = ExportEnum.Site_Index;
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(siteStruct.RepertoireCommon(), filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                AddStructureArgument(argsList, siteStruct, fileSave);

                ExportHTML.ToHTMLSite(docindex, type, fileSave, argsList);
                output.Add(new FileWithChecksum(fileSave + ".html"));
                progress.Report(BatchProgressInfo.Step(1)); // Report the end of the task

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
                progress.Report(BatchProgressInfo.Step(2)); // Report the end of the task

                LogTools.Logger.Debug("GenereWebSiteIndex {0}", output.Count);
            }

            progress.Report(BatchProgressInfo.Step(2)); // Report the end of the task
            return output;
        }

        /// <summary>
        /// Génére le menu
        /// </summary>
        /// <param name="DC"></param>
        public List<FileWithChecksum> GenereWebSiteMenu(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config, ExportSiteStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            // Clone la structure de repertoires pour ne pas l'altérer dans le contexte multi-thread (changement de path)
            ExportSiteStructure siteStruct = (ExportSiteStructure)structRep.Clone();

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

                progress?.Report(BatchProgressInfo.Init(nbGen)); // Report the start of the task with the number of subtask

                ExportEnum type;
                string directory = siteStruct.RepertoireCommon();

                XDocument docmenu = ExportXML.CreateDocumentMenu(DC, EDC, siteStruct);
                // TODO Il faut revoir tous les acces a la configuration via la nouvelle balise et plus dans la competition
                ExportXML.AddPublicationInfo(ref docmenu, config);
                LogTools.DebugLogData(docmenu);

                // Genere le menu de d'avancement
                type = ExportEnum.Site_MenuAvancement;
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                AddStructureArgument(argsList, siteStruct, fileSave);

                ExportHTML.ToHTMLSite(docmenu, type, fileSave, argsList);
                output.Add(new FileWithChecksum(fileSave + ".html"));
                progress.Report(BatchProgressInfo.Step(1)); // Report the progress of the task

                // Genere le menu de classement
                type = ExportEnum.Site_MenuClassement;
                string filename2 = ExportTools.getFileName(type);
                string fileSave2 = Path.Combine(directory, filename2.Replace("/", "_"));
                XsltArgumentList argsList2 = new XsltArgumentList();
                AddStructureArgument(argsList2, siteStruct, fileSave2);

                ExportHTML.ToHTMLSite(docmenu, type, fileSave2, argsList2);
                output.Add(new FileWithChecksum(fileSave2 + ".html"));
                progress.Report(BatchProgressInfo.Step(2)); // Report the progress of the task

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
                    progress.Report(BatchProgressInfo.Step(3)); // Report the progress of the task
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
                    progress.Report(BatchProgressInfo.Step(4)); // Report the progress of the task
                }

                progress.Report(BatchProgressInfo.Step(nbGen)); // Report the progress of the task
            }

            LogTools.Logger.Debug("Menu = {0}", output.Count);


            return output;
        }

        /// <summary>
        /// Genere la page d'affectation des tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteAffectation(IJudoData DC, ConfigurationExportSite config, ExportSiteStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            // Clone la structure de repertoires pour ne pas l'altérer dans le contexte multi-thread (changement de path)
            ExportSiteStructure siteStruct = (ExportSiteStructure)structRep.Clone();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(BatchProgressInfo.Init(1)); // Report the start of the task with the number of subtask

            if (DC != null && config != null && siteStruct != null)
            {
                ExportEnum type = ExportEnum.Site_AffectationTapis;
                string directory = siteStruct.RepertoireCommon();
                string filename = ExportTools.getFileName(type);
                string fileSave = Path.Combine(directory, filename.Replace("/", "_"));
                XsltArgumentList argsList = new XsltArgumentList();
                AddStructureArgument(argsList, siteStruct, fileSave);

                XDocument docAffectation = ExportXML.CreateDocumentAffectationTapis(DC);
                ExportXML.AddPublicationInfo(ref docAffectation, config);
                LogTools.DebugLogData(docAffectation);

                ExportHTML.ToHTMLSite(docAffectation, type, fileSave, argsList);

                output.Add(new FileWithChecksum(fileSave + ".html"));
            }

            LogTools.Logger.Debug("Affectation = {0}", output.Count);


            progress?.Report(BatchProgressInfo.Step(1)); // Report the end of the task
            return output;
        }    

        /// <summary>
        /// Genere la page des engages
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteEngagements(IJudoData DC, ExtendedJudoData EDC, List<GroupeEngagements> grps, ConfigurationExportSite config, ExportSiteStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            // Clone la structure de repertoires pour ne pas l'altérer dans le contexte multi-thread (changement de path)
            ExportSiteStructure siteStruct = (ExportSiteStructure)structRep.Clone();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && EDC != null && grps != null && config != null && siteStruct != null)
            {
                int nbGrps = grps.Count;

                progress?.Report(BatchProgressInfo.Init(nbGrps)); // Report the start of the task with the number of subtask


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

                    progress?.Report( BatchProgressInfo.Step(i+1)); // Report the end of the task
                }

                progress?.Report(BatchProgressInfo.Step(nbGrps)); // Report the end of the task
            }

            LogTools.Logger.Debug("Engagements = {0}", output.Count);

            return output;
        }

        #endregion
    }
}