using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Models.EcransAppel;
using AppPublication.Publication;
using AppPublication.Tools.Enum;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using OfficeOpenXml.Table.PivotTable;
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
        protected override void AddStructureArgument<T>(XsltArgumentList argsList, UrlGeneratorBase<T> siteStruct, string targetFile)
        {
            SiteUrlGenerator theStruct = siteStruct as SiteUrlGenerator;

            // Ajoute les repertoires de base de la structure
            base.AddStructureArgument(argsList, siteStruct, targetFile);

            // Ajoute le repertoire common
            var commonPath = siteStruct.GetUrlFromPhysicalPath(theStruct.PhysicalStructure.RepertoireCommon());
            argsList.AddParam("commonPath", "", commonPath.AbsoluteUri);
        }

        /// <summary>
        /// Génère un fichier de menu spécifique et retourne ses informations de checksum.
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="siteStructure"></param>
        /// <param name="docMenu"></param>
        /// <returns></returns>
        private FileWithChecksum GenerateMenuFile(ExportEnum exportType, string targetDirectory, SiteUrlGenerator siteStructure, XDocument docMenu)
        {
            // Utilisation de notre utilitaire universel
            string savePath = GetFileSavePath(targetDirectory, exportType);

            var xsltArgs = CreateAllXsltArgs(siteStructure, savePath);

            ExportHTML.ToHTMLSite(docMenu, exportType, savePath, xsltArgs);

            return new FileWithChecksum($"{savePath}.html");
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
        public List<FileWithChecksum> GenereWebSitePhase(IJudoData DC, Phase phase, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            LogTools.Logger.Debug("Phase ({1}) '{0}'", phase?.libelle, phase?.id);

            ConfigurationExportSite config = ctx.Config;

            List<FileWithChecksum> output = new List<FileWithChecksum>();

            int nbGen = config.PublierProchainsCombats ? 2 : 1;
            progress?.Report(BatchProgressInfo.Init(nbGen));

            if (DC != null && phase != null && config != null && siteStructure != null)
            {
                i_vue_epreuve_interface vueEpreuve = phase.isEquipe
                    ? (i_vue_epreuve_interface)DC.Organisation.VueEpreuveEquipes.FirstOrDefault(o => o.id == phase.epreuve)
                    : DC.Organisation.VueEpreuves.FirstOrDefault(o => o.id == phase.epreuve);

                if (vueEpreuve == null) return output;

                // Détermination du répertoire cible UNE SEULE FOIS pour cette épreuve
                string targetDirectory = siteStructure.PhysicalStructure.RepertoireEpreuve(vueEpreuve.id.ToString(), vueEpreuve.nom);

                // --- 1. TRAITEMENTS POULE / TABLEAU ---
                if (phase.typePhase == (int)TypePhaseEnum.Poule || phase.typePhase == (int)TypePhaseEnum.Tableau)
                {
                    bool isPoule = phase.typePhase == (int)TypePhaseEnum.Poule;

                    ExportEnum exportType = isPoule ? ExportEnum.Site_Poule_Resultat : ExportEnum.Site_Tableau_Competition;
                    string savePath = GetFileSavePath(targetDirectory, exportType);

                    var phaseParams = new List<(string, object)>();
                    if (isPoule)
                    {
                        int typePoule = config.PouleEnColonnes ? (config.PouleToujoursEnColonnes ? (int)TypePouleEnum.Colonnes : (int)TypePouleEnum.Auto) : (int)TypePouleEnum.Diagonale;
                        phaseParams.Add(("typePoule", typePoule));
                        phaseParams.Add(("tailleMaxPouleColonne", config.TailleMaxPouleColonnes));
                    }

                    // Utilisation de la fabrique (AddStructureArgument est inclus dedans)
                    var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, phaseParams.ToArray());

                    XDocument xmlResultat = ExportXML.CreateDocumentPhase(vueEpreuve, phase, DC);
                    ctx.EnrichWithFullContext(xmlResultat);
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
                    string savePath = GetFileSavePath(targetDirectory, exportType);

                    var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, ("istapis", "epreuve"));

                    XDocument xmlFeuilleCombat = ExportXML.CreateDocumentFeuilleCombat(DC, phase, null);
                    ctx.EnrichWithFullContext(xmlFeuilleCombat);
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
        public List<FileWithChecksum> GenereWebSiteClassement(IJudoData DC, i_vue_epreuve_interface epreuve, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            LogTools.Logger.Debug("Epreuve ({1}) '{0}'", epreuve?.nom, epreuve?.id);

            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(BatchProgressInfo.Init(1));

            if (DC != null && epreuve != null && ctx != null && siteStructure != null)
            {
                // Détermination du répertoire cible pour cette épreuve
                string targetDirectory = siteStructure.PhysicalStructure.RepertoireEpreuve(epreuve.id.ToString(), epreuve.nom);
                ExportEnum exportType = ExportEnum.Site_ClassementFinal;

                // Utilisation de la méthode mutualisée pour le chemin
                string savePath = GetFileSavePath(targetDirectory, exportType);

                var xsltArgs = CreateAllXsltArgs(siteStructure, savePath);

                // 1. Génération du document de base
                XDocument xmlClassement = ExportXML.CreateDocumentEpreuve(DC, epreuve);

                // 2. Enrichissement via le contexte (Porte la Config, les structures et les infos de publication)
                ctx.EnrichWithFullContext(xmlClassement);

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
        /// Génère les prochains combats de tous les tapis (vue d'ensemble pour le site).
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="ctx"></param>
        /// <param name="structRep"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteAllTapis(IJudoData DC, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            // Report the start of the task
            progress?.Report(BatchProgressInfo.Init(1));

            if (DC != null && ctx != null && siteStructure != null)
            {
                string targetDirectory = siteStructure.PhysicalStructure.RepertoireCommon();
                ExportEnum exportType = ExportEnum.Site_FeuilleCombatTapis;

                // Construction du chemin pour le répertoire commun
                string savePath = GetFileSavePath(targetDirectory, exportType);

                bool useIntituleCommun = DC.Organisation.Competitions.Count() > 1
                         && ctx.Config.UseIntituleCommun
                         && !string.IsNullOrEmpty(ctx.Config.IntituleCommun);

                var xsltArgs = CreateAllXsltArgs(siteStructure, savePath,
                    ("istapis", "alltapis"),
                    ("useIntituleCommun", useIntituleCommun.ToString().ToLower())
                );

                // 1. Génération du document (Utilise notre version optimisée de CreateDocumentFeuilleCombat)
                // Les paramètres null, null indiquent qu'on veut tous les tapis et toutes les phases.
                XDocument xmlAllTapis = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);

                // 2. Enrichissement via le contexte (PublicationInfo + Structures)
                ctx.EnrichWithFullContext(xmlAllTapis);

                LogTools.DebugLogData(xmlAllTapis);

                // 3. Transformation HTML
                ExportHTML.ToHTMLSite(xmlAllTapis, exportType, savePath, xsltArgs);

                output.Add(new FileWithChecksum($"{savePath}.html"));
            }

            LogTools.Logger.Debug("ProchainsCombats Tapis = {0}", output.Count);

            // Report the end of the task
            progress?.Report(BatchProgressInfo.Step(1));

            return output;
        }


        /// <summary>
        /// Génère la page d'index du site, les scripts de mise à jour et exporte les ressources statiques.
        /// </summary>
        public List<FileWithChecksum> GenereWebSiteIndex(IJudoData DC, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(BatchProgressInfo.Init(2));

            if (DC != null && ctx != null && siteStructure != null)
            {
                // 1. Génération du document d'index de base
                XDocument docIndex = ExportXML.CreateDocumentIndex(DC);

                // 2. Ajout de la CONFIGURATION uniquement (pas de structures de clubs/ligues)
                // On suppose que cette méthode dans ctx injecte PublicationInfo et SiteConfiguration
                ctx.EnrichWithConfiguration(docIndex);

                LogTools.DebugLogData(docIndex);

                // --- 3. GÉNÉRATION DE L'INDEX HTML ---
                ExportEnum indexType = ExportEnum.Site_Index;
                string indexFilename = ExportTools.getFileName(indexType).Replace("/", "_");
                string indexSavePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireCommon(), indexFilename);

                var indexArgs = CreateAllXsltArgs(siteStructure, indexSavePath);

                ExportHTML.ToHTMLSite(docIndex, indexType, indexSavePath, indexArgs);
                output.Add(new FileWithChecksum($"{indexSavePath}.html"));

                progress?.Report(BatchProgressInfo.Step(1));

                // --- 4. RESSOURCES STATIQUES (CSS, JS, IMG) ---
                // Export direct des styles et scripts
                var staticFiles = ExportTools.ExportEmbeddedStyleAndJS(true, siteStructure);
                output.AddRange(staticFiles.Select(path => new FileWithChecksum(path)));
                LogTools.Logger.Debug("GenereWebSiteIndex - Style/JS: {0} fichiers", staticFiles.Count);

                // Export des images
                var imageFiles = ExportTools.ExportEmbeddedImg(true, true, siteStructure);
                output.AddRange(imageFiles.Select(path => new FileWithChecksum(path)));
                LogTools.Logger.Debug("GenereWebSiteIndex - Images: {0} fichiers", imageFiles.Count);

                // --- 5. GÉNÉRATION DU SCRIPT DE MISE À JOUR (FOOTER) ---
                ExportEnum footerType = ExportEnum.Site_FooterScript;
                string footerFilename = ExportTools.getFileName(footerType).Replace("/", "_");
                string footerSavePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireJs(), footerFilename);

                var footerArgs = CreateAllXsltArgs(siteStructure, footerSavePath);

                // Utilisation du même docIndex pour générer le JS via XSLT
                ExportHTML.ToHTMLSite(docIndex, footerType, footerSavePath, footerArgs, "js");
                output.Add(new FileWithChecksum($"{footerSavePath}.js"));

                LogTools.Logger.Debug("GenereWebSiteIndex Terminé - Total: {0} ressources", output.Count);
                progress?.Report(BatchProgressInfo.Step(2));
            }

            progress?.Report(BatchProgressInfo.Step(2));
            return output;
        }

        /// <summary>
        /// Génère les différents menus de navigation du site (Avancement, Classement, et optionnellement Prochains Combats / Engagements).
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        /// <param name="ctx"></param>
        /// <param name="structRep"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteMenu(IJudoData DC, ExtendedJudoData EDC, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null || EDC == null || ctx == null || siteStructure == null)
                return output;

            ConfigurationExportSite config = ctx.Config;

            // Calcul dynamique du nombre d'étapes pour la barre de progression
            int nbGen = 2 + (config.PublierProchainsCombats ? 1 : 0) + (config.PublierEngagements ? 1 : 0);
            progress?.Report(BatchProgressInfo.Init(nbGen));

            int currentStep = 0;

            // Le répertoire cible est défini une seule fois pour tous les menus (racine du site)
            string targetDirectory = siteStructure.PhysicalStructure.RepertoireCommon();

            // 1. Création du document XML de base
            XDocument docMenu = ExportXML.CreateDocumentMenu(DC, EDC, siteStructure);

            // 2. Ajout de la configuration contextuelle (infos de publication, etc.)
            ctx.EnrichWithConfiguration(docMenu);
            LogTools.DebugLogData(docMenu);

            output.Add(GenerateMenuFile(ExportEnum.Site_MenuClassement, targetDirectory, siteStructure, docMenu));
            progress?.Report(BatchProgressInfo.Step(++currentStep));

            // 3. Génération des menus de base (toujours présents)
            output.Add(GenerateMenuFile(ExportEnum.Site_MenuAvancement, targetDirectory, siteStructure, docMenu));
            progress?.Report(BatchProgressInfo.Step(++currentStep));

            // 4. Génération du menu des prochains combats
            if (config.PublierProchainsCombats)
            {
                output.Add(GenerateMenuFile(ExportEnum.Site_MenuProchainCombats, targetDirectory, siteStructure, docMenu));
                progress?.Report(BatchProgressInfo.Step(++currentStep));
            }

            // 5. Génération du menu des engagements
            if (config.PublierEngagements)
            {
                // Enrichissement lourd (structures géographiques, clubs) uniquement pour ce dernier fichier
                // afin de ne pas alourdir inutilement le XML des menus précédents
                ctx.EnrichWithFullContext(docMenu);

                output.Add(GenerateMenuFile(ExportEnum.Site_MenuEngagements, targetDirectory, siteStructure, docMenu));
                progress?.Report(BatchProgressInfo.Step(++currentStep));
            }

            LogTools.Logger.Debug("Menu = {0}", output.Count);

            return output;
        }


        /// <summary>
        /// Genere la page d'affectation des tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="config"></param>
        /// <param name="structRep"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteAffectation(IJudoData DC, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(BatchProgressInfo.Init(1));

            if (DC != null && ctx != null && siteStructure != null)
            {
                string targetDirectory = siteStructure.PhysicalStructure.RepertoireCommon();
                ExportEnum exportType = ExportEnum.Site_AffectationTapis;

                // Appel unifié avec notre méthode utilitaire
                string savePath = GetFileSavePath(targetDirectory, exportType);
                
                var xsltArgs = CreateAllXsltArgs(siteStructure, savePath);

                // Génération du document et enrichissement via le contexte
                XDocument docAffectation = ExportXML.CreateDocumentAffectationTapis(DC);
                ctx.EnrichWithConfiguration(docAffectation);

                LogTools.DebugLogData(docAffectation);

                ExportHTML.ToHTMLSite(docAffectation, exportType, savePath, xsltArgs);

                output.Add(new FileWithChecksum($"{savePath}.html"));
            }

            LogTools.Logger.Debug("Affectation = {0}", output.Count);
            progress?.Report(BatchProgressInfo.Step(1));

            return output;
        }

        /// <summary>
        /// Genere la page des engages
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        /// <summary>
        public List<FileWithChecksum> GenereWebSiteEngagements(IJudoData DC, ExtendedJudoData EDC, List<GroupeEngagements> grps, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && EDC != null && grps != null && ctx != null && siteStructure != null)
            {
                int nbGrps = grps.Count;
                progress?.Report(BatchProgressInfo.Init(nbGrps));

                ExportEnum exportType = ExportEnum.Site_Engagements;

                // On récupère le document des engagements depuis notre contexte unifié 
                // au lieu d'utiliser une vieille variable globale de classe (_docEngagements)
                XDocument docEngagements = ctx.ExportDocument;

                int currentStep = 0;

                // Remplacement de la boucle 'for' par un 'foreach' plus lisible
                foreach (GroupeEngagements grp in grps)
                {
                    // Détermination du répertoire cible dynamique pour ce groupe
                    string targetDirectory = siteStructure.PhysicalStructure.RepertoireGroupeEngagements(grp.Id);
                    string savePath = GetFileSavePath(targetDirectory, exportType);

                    var xsltArgs = CreateAllXsltArgs(siteStructure, savePath,
                        ("idgroupe", grp.Id),
                        ("idcompetition", grp.Competition)
                    );

                    // Transformation HTML à partir du document contextuel
                    ExportHTML.ToHTMLSite(docEngagements, exportType, savePath, xsltArgs);

                    output.Add(new FileWithChecksum($"{savePath}.html"));

                    progress?.Report(BatchProgressInfo.Step(++currentStep));
                }
            }

            LogTools.Logger.Debug("Engagements = {0}", output.Count);

            return output;
        }

        #endregion
    }
}