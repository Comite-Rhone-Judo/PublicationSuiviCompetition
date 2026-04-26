using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Publication;
using AppPublication.Tools.Enum;
using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using FranceJudo.Metier.Export;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Site;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

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
            argsList.AddParam("commonPath", "", theStruct.GetRelativeUrlCommon(targetFile));
        }

        /// <summary>
        /// Génère un fichier de menu spécifique et retourne ses informations de checksum.
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="siteStructure"></param>
        /// <param name="docMenu"></param>
        /// <returns></returns>
        private FileWithChecksum GenerateMenuFile(ExportEnum exportType, string targetDirectory, SiteUrlGenerator siteStructure, XmlSource docMenu)
        {
            // Utilisation de notre utilitaire universel
            string savePath = GetFileSavePath(targetDirectory, exportType);

            var xsltArgs = CreateAllXsltArgs(siteStructure, savePath);

            SiteExportEngine.GenererHtmlSite(docMenu, exportType, savePath, xsltArgs);

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
        public List<FileWithChecksum> GenereWebSitePhase(IPhase phase, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;

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

                    XDocument outDoc = ExportXML.CreateDocumentPhase(ctx, vueEpreuve, phase);
                    ctx.EnrichWithFullContext(outDoc);
                    LogTools.DebugLogData(outDoc);

                    using (var source = new XmlSource(outDoc))
                    {
                        SiteExportEngine.GenererHtmlSite(source, exportType, savePath, xsltArgs);
                    }
                    output.Add(new FileWithChecksum($"{savePath}.html"));
                    LogTools.Logger.Debug("{0} = 1", isPoule ? "Poule" : "Tableau");

                    progress?.Report(BatchProgressInfo.Step(1));
                }

                // --- 2. PROCHAINS COMBATS ---
                if (config.PublierProchainsCombats)
                {
                    // On enregistre le fait qu'on va generer les prochains combats pour cette épreuve dans le contexte, afin d'éviter
                    // les doublons si plusieurs phases de la même épreuve sont traitées (poule/tableau)
                    if (ctx.ProchainsCombatsGeneres.TryAdd(vueEpreuve.id, true))
                    {
                        LogTools.Logger.Debug("ProchainsCombats generes pour l'epreuve {0} (ID: {1}) - Phase ID:{2} {3}", vueEpreuve?.nom, vueEpreuve?.id, phase?.libelle, phase?.id);

                        ExportEnum exportType = ExportEnum.Site_FeuilleCombat;
                        string savePath = GetFileSavePath(targetDirectory, exportType);

                        var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, ("istapis", "epreuve"));

                        XDocument outDoc = ExportXML.CreateDocumentFeuilleCombat(ctx, phase, null);
                        ctx.EnrichWithFullContext(outDoc);
                        LogTools.DebugLogData(outDoc);

                        using (var source = new XmlSource(outDoc))
                        {
                            SiteExportEngine.GenererHtmlSite(source, exportType, savePath, xsltArgs);
                        }

                        output.Add(new FileWithChecksum($"{savePath}.html"));
                        LogTools.Logger.Debug("ProchainsCombats = 1");

                        progress?.Report(BatchProgressInfo.Step(2));
                    }
                    else {
                        // Un autre thread a déjà généré les prochains combats pour cette épreuve !
                        // On signale juste l'avancement pour ne pas fausser la barre de progression
                        progress?.Report(BatchProgressInfo.Step(2));
                        LogTools.Logger.Debug("ProchainsCombats deja generes pour l'epreuve {0} (ID: {1}) - Phase ID:{2} {3} sauf de la generation", vueEpreuve?.nom, vueEpreuve?.id, phase?.libelle, phase?.id);
                    }
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
        public List<FileWithChecksum> GenereWebSiteClassement(i_vue_epreuve_interface epreuve, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;

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
                XDocument outDoc = ExportXML.CreateDocumentEpreuve(ctx, epreuve);

                // 2. Enrichissement via le contexte (Porte la Config, les structures et les infos de publication)
                ctx.EnrichWithFullContext(outDoc);

                LogTools.DebugLogData(outDoc);

                // 3. Transformation HTML
                using (var source = new XmlSource(outDoc))
                {
                    SiteExportEngine.GenererHtmlSite(source, exportType, savePath, xsltArgs);
                }

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
        public List<FileWithChecksum> GenereWebSiteAllTapis(ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;
            List <FileWithChecksum> output = new List<FileWithChecksum>();

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
                XDocument outDoc = ExportXML.CreateDocumentFeuilleCombat(ctx, null, null);

                // 2. Enrichissement via le contexte (PublicationInfo + Structures)
                ctx.EnrichWithFullContext(outDoc);

                LogTools.DebugLogData(outDoc);

                // 3. Transformation HTML
                using (var source = new XmlSource(outDoc))
                {
                    SiteExportEngine.GenererHtmlSite(source, exportType, savePath, xsltArgs);
                }

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
        public List<FileWithChecksum> GenereWebSiteIndex(ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(BatchProgressInfo.Init(2));

            if (DC != null && ctx != null && siteStructure != null)
            {
                // 1. Génération du document d'index de base
                XDocument outDoc = ExportXML.CreateDocumentIndex(ctx);

                // 2. Ajout de la CONFIGURATION uniquement (pas de structures de clubs/ligues)
                // On suppose que cette méthode dans ctx injecte PublicationInfo et SiteConfiguration
                ctx.EnrichWithConfiguration(outDoc);

                LogTools.DebugLogData(outDoc);

                // --- 3. GÉNÉRATION DE L'INDEX HTML ---
                ExportEnum indexType = ExportEnum.Site_Index;
                string indexFilename = SiteExportEngine.GetFileName(indexType).Replace("/", "_");
                string indexSavePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireCommon(), indexFilename);

                var indexArgs = CreateAllXsltArgs(siteStructure, indexSavePath);

                using (var source = new XmlSource(outDoc))
                {
                    SiteExportEngine.GenererHtmlSite(source, indexType, indexSavePath, indexArgs);

                    output.Add(new FileWithChecksum($"{indexSavePath}.html"));

                    progress?.Report(BatchProgressInfo.Step(1));

                    // --- 4. RESSOURCES STATIQUES (CSS, JS, IMG) ---
                    // Export direct des styles et scripts
                    var staticFiles = SiteExportEngine.ExportEmbeddedStyleAndJS(true, siteStructure);
                    output.AddRange(staticFiles.Select(path => new FileWithChecksum(path)));
                    LogTools.Logger.Debug("GenereWebSiteIndex - Style/JS: {0} fichiers", staticFiles.Count);

                    // Export des images
                    var imageFiles = SiteExportEngine.ExportEmbeddedImg(true, true, siteStructure);
                    output.AddRange(imageFiles.Select(path => new FileWithChecksum(path)));
                    LogTools.Logger.Debug("GenereWebSiteIndex - Images: {0} fichiers", imageFiles.Count);

                    // --- 5. GÉNÉRATION DU SCRIPT DE MISE À JOUR (FOOTER) ---
                    ExportEnum footerType = ExportEnum.Site_FooterScript;
                    string footerFilename = SiteExportEngine.GetFileName(footerType).Replace("/", "_");
                    string footerSavePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireJs(), footerFilename);

                    var footerArgs = CreateAllXsltArgs(siteStructure, footerSavePath);

                    // Utilisation du même docIndex pour générer le JS via XSLT
                    SiteExportEngine.GenererHtmlSite(source, footerType, footerSavePath, footerArgs, "js");
                    output.Add(new FileWithChecksum($"{footerSavePath}.js"));
                }

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
        public List<FileWithChecksum> GenereWebSiteMenu(ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;
            IExtendedJudoData EDC = ctx.ExtendedDataContext;

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
            XDocument outDoc = ExportXML.CreateDocumentMenu(ctx, siteStructure);
            
            
            // 2. Ajout de la configuration contextuelle (infos de publication, etc.)
            ctx.EnrichWithConfiguration(outDoc);

            LogTools.DebugLogData(outDoc);

            using (var source = new XmlSource(outDoc))
            {
                output.Add(GenerateMenuFile(ExportEnum.Site_MenuClassement, targetDirectory, siteStructure, source));
                progress?.Report(BatchProgressInfo.Step(++currentStep));

                // 3. Génération des menus de base (toujours présents)
                output.Add(GenerateMenuFile(ExportEnum.Site_MenuAvancement, targetDirectory, siteStructure, source));
                progress?.Report(BatchProgressInfo.Step(++currentStep));

                // 4. Génération du menu des prochains combats
                if (config.PublierProchainsCombats)
                {
                    output.Add(GenerateMenuFile(ExportEnum.Site_MenuProchainCombats, targetDirectory, siteStructure, source));
                    progress?.Report(BatchProgressInfo.Step(++currentStep));
                }

                // 5. Génération du menu des engagements
                if (config.PublierEngagements)
                {
                    output.Add(GenerateMenuFile(ExportEnum.Site_MenuEngagements, targetDirectory, siteStructure, source));
                    progress?.Report(BatchProgressInfo.Step(++currentStep));
                }
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
        public List<FileWithChecksum> GenereWebSiteAffectation(ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;
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
                XDocument outDoc = ExportXML.CreateDocumentAffectationTapis(ctx);
                ctx.EnrichWithConfiguration(outDoc);

                LogTools.DebugLogData(outDoc);

                using (var source = new XmlSource(outDoc))
                {
                    SiteExportEngine.GenererHtmlSite(source, exportType, savePath, xsltArgs);
                }

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
        public List<FileWithChecksum> GenereWebSiteEngagements(List<GroupeEngagements> grps, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;
            IExtendedJudoData EDC = ctx.ExtendedDataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && EDC != null && grps != null && ctx != null && siteStructure != null)
            {
                int nbGrps = grps.Count;
                progress?.Report(BatchProgressInfo.Init(nbGrps));

                ExportEnum exportType = ExportEnum.Site_Engagements;

                // --- DÉBUT DE L'OPTIMISATION XPATH ---
                XPathDocument xpathEngagements;

                // On crée un lecteur optimisé avec NameTable depuis le XDocument du contexte
                var settings = new XmlReaderSettings { NameTable = new NameTable(), IgnoreWhitespace = true };
                using (var reader = XmlReader.Create(ctx.ExportDocument.CreateReader(), settings))
                {
                    // Compilation en RAM (zéro allocation future)
                    xpathEngagements = new XPathDocument(reader);
                }

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
                    SiteExportEngine.GenererHtmlSite(xpathEngagements, exportType, savePath, xsltArgs);

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