using AppPublication.Publication;
using AppPublication.Tools.Enum;
using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using FranceJudo.Metier.Export;
using FranceJudo.Metier.ExtensionNoyau;
using FranceJudo.Metier.ExtensionNoyau.Engagement;
using FranceJudo.Metier.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Site;
using FranceJudo.Metier.XML;
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
        /// Génère un fichier de menu spécifique à partir d'un XmlSource classique.
        /// </summary>
        /// <param name="exportType">type d'export</param>
        /// <param name="targetDirectory">le répertoire cible</param>
        /// <param name="siteStructure">la structure du site</param>
        /// <param name="docMenu">le document XML du menu</param>
        /// <param name="routingNode">le nœud de routage</param>
        /// <returns>le fichier généré avec son checksum</returns>
        private FileWithChecksum GenerateMenuFile(ExportEnum exportType, string targetDirectory, SiteUrlGenerator siteStructure, XmlSource docMenu, XElement routingNode)
        {
            string savePath = GetFileSavePath(targetDirectory, exportType);

            // Injection du dictionnaire de routage externe
            var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, ("SiteRoutes", routingNode.CreateNavigator()));

            SiteExportEngine.GenererHtmlSite(docMenu, exportType, savePath, xsltArgs);

            return new FileWithChecksum($"{savePath}.html");
        }

        /// <summary>
        /// Surcharge pour générer un fichier de menu à partir d'un document XPath optimisé (lecture seule).
        /// </summary>
        /// <param name="exportType">type d'export</param>
        /// <param name="targetDirectory">le répertoire cible</param>
        /// <param name="siteStructure">la structure du site</param>
        /// <param name="docMenu">le document XPath du menu</param>
        /// <param name="routingNode">le nœud de routage</param>
        /// <returns>le fichier généré avec son checksum</returns>
        private FileWithChecksum GenerateMenuFile(ExportEnum exportType, string targetDirectory, SiteUrlGenerator siteStructure, XPathDocument docMenu, XElement routingNode)
        {
            string savePath = GetFileSavePath(targetDirectory, exportType);

            // Injection du dictionnaire de routage externe
            var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, ("SiteRoutes", routingNode.CreateNavigator()));

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
            if (phase == null || ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération de la phase.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null)
            {
                LogTools.Logger?.Error("DataContext est null dans le contexte d'export.");
                return output;
            }

            LogTools.Logger?.Debug("Phase ({1}) '{0}'", phase?.libelle, phase?.id);

            ConfigurationExportSite config = ctx.Config;

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
                if (phase.typePhase == TypePhaseEnum.Poule || phase.typePhase == TypePhaseEnum.Tableau)
                {
                    bool isPoule = phase.typePhase == TypePhaseEnum.Poule;

                    ExportEnum exportType = isPoule ? ExportEnum.Site_Poule_Resultat : ExportEnum.Site_Tableau_Competition;
                    string savePath = GetFileSavePath(targetDirectory, exportType);

                    var phaseParams = new List<(string, object)>();
                    if (isPoule)
                    {
                        TypePouleEnum typePoule = config.PouleEnColonnes ? (config.PouleToujoursEnColonnes ? TypePouleEnum.Colonnes : TypePouleEnum.Auto) : TypePouleEnum.Diagonale;
                        phaseParams.Add(("typePoule", (int)typePoule));
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
                    LogTools.Logger?.Debug("{0} = 1", isPoule ? "Poule" : "Tableau");

                    progress?.Report(BatchProgressInfo.Step(1));
                }

                // --- 2. PROCHAINS COMBATS ---
                if (config.PublierProchainsCombats)
                {
                    // On enregistre le fait qu'on va generer les prochains combats pour cette épreuve dans le contexte, afin d'éviter
                    // les doublons si plusieurs phases de la même épreuve sont traitées (poule/tableau)
                    if (ctx.ProchainsCombatsGeneres.TryAdd(vueEpreuve.id, true))
                    {
                        LogTools.Logger?.Debug("ProchainsCombats generes pour l'epreuve {0} (ID: {1}) - Phase ID:{2} {3}", vueEpreuve?.nom, vueEpreuve?.id, phase?.libelle, phase?.id);

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
                        LogTools.Logger?.Debug("ProchainsCombats = 1");

                        progress?.Report(BatchProgressInfo.Step(2));
                    }
                    else
                    {
                        // Un autre thread a déjà généré les prochains combats pour cette épreuve !
                        // On signale juste l'avancement pour ne pas fausser la barre de progression
                        progress?.Report(BatchProgressInfo.Step(2));
                        LogTools.Logger?.Debug("ProchainsCombats deja generes pour l'epreuve {0} (ID: {1}) - Phase ID:{2} {3} sauf de la generation", vueEpreuve?.nom, vueEpreuve?.id, phase?.libelle, phase?.id);
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
            if (epreuve == null || ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération du classement.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null)
            {
                LogTools.Logger?.Error("DataContext est null dans le contexte d'export.");
                return output;
            }

            LogTools.Logger?.Debug("Epreuve ({1}) '{0}'", epreuve?.nom, epreuve?.id);

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

            LogTools.Logger?.Debug("Classement = {0}", output.Count);
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
            if (ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération AllTapis.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null)
            {
                LogTools.Logger?.Error("DataContext est null dans le contexte d'export.");
                return output;
            }

            // Report the start of the task
            progress?.Report(BatchProgressInfo.Init(1));

            if (DC != null && ctx != null && siteStructure != null)
            {
                string targetDirectory = siteStructure.PhysicalStructure.RepertoireCommon();
                ExportEnum exportType = ExportEnum.Site_FeuilleCombatTapis;

                // Construction du chemin pour le répertoire commun
                string savePath = GetFileSavePath(targetDirectory, exportType);

                bool useIntituleCommun = DC.Organisation.Competitions.Count > 1
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

            LogTools.Logger?.Debug("ProchainsCombats Tapis = {0}", output.Count);

            // Report the end of the task
            progress?.Report(BatchProgressInfo.Step(1));

            return output;
        }


        /// <summary>
        /// Génère la page d'index du site, les scripts de mise à jour et exporte les ressources statiques.
        /// </summary>
        public List<FileWithChecksum> GenereWebSiteIndex(ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            if (ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération de l'index du site.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null)
            {
                LogTools.Logger?.Error("DataContext est null dans le contexte d'export.");
                return output;
            }
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
                string indexFilename = SiteExportEngine.GetSanitizedFileName(indexType);
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
                    LogTools.Logger?.Debug("GenereWebSiteIndex - Style/JS: {0} fichiers", staticFiles.Count);

                    // Export des images
                    var imageFiles = SiteExportEngine.ExportEmbeddedImg(true, true, siteStructure);
                    output.AddRange(imageFiles.Select(path => new FileWithChecksum(path)));
                    LogTools.Logger?.Debug("GenereWebSiteIndex - Images: {0} fichiers", imageFiles.Count);

                    // --- 5. GÉNÉRATION DU SCRIPT DE MISE À JOUR (FOOTER) ---
                    ExportEnum footerType = ExportEnum.Site_FooterScript;
                    string footerFilename = SiteExportEngine.GetSanitizedFileName(footerType);
                    string footerSavePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireJs(), footerFilename);

                    var footerArgs = CreateAllXsltArgs(siteStructure, footerSavePath);

                    // Utilisation du même docIndex pour générer le JS via XSLT
                    SiteExportEngine.GenererHtmlSite(source, footerType, footerSavePath, footerArgs, "js");
                    output.Add(new FileWithChecksum($"{footerSavePath}.js"));
                }

                LogTools.Logger?.Debug("GenereWebSiteIndex Terminé - Total: {0} ressources", output.Count);
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
            if(ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération des menus du site.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            IExtendedJudoData EDC = ctx.ExtendedDataContext;


            if (DC == null || EDC == null)
            {
                LogTools.Logger?.Error("DataContext ou ExtendedDataContext est null dans le contexte d'export.");
                return output;
            }

            ConfigurationExportSite config = ctx.Config;

            // Calcul dynamique du nombre d'étapes pour la barre de progression
            int nbGen = 2 + (config.PublierProchainsCombats ? 1 : 0) + (config.PublierEngagements ? 1 : 0);
            progress?.Report(BatchProgressInfo.Init(nbGen));

            int currentStep = 0;

            // Le répertoire cible est défini une seule fois pour tous les menus (racine du site)
            string targetDirectory = siteStructure.PhysicalStructure.RepertoireCommon();
            string sourcePhysicalFile = Path.Combine(targetDirectory, "menu.html");

            // 1. Création du document XML de base
            XDocument outDoc = ExportXML.CreateDocumentMenu(ctx, siteStructure, config.PublierEngagements, config.PublierStatistiques);
            
            // 2. Ajout de la configuration contextuelle (infos de publication, etc.)
            ctx.EnrichWithConfiguration(outDoc);

            LogTools.DebugLogData(outDoc);

            // 2. Génération du dictionnaire de routage C# indépendant
            XElement routingNode = GenerateSiteRoutes(ctx, siteStructure, sourcePhysicalFile);

            LogTools.DebugLogData(routingNode);

            using (var source = new XmlSource(outDoc))
            {
                output.Add(GenerateMenuFile(ExportEnum.Site_MenuClassement, targetDirectory, siteStructure, source, routingNode));
                progress?.Report(BatchProgressInfo.Step(++currentStep));

                // 3. Génération des menus de base (toujours présents)
                output.Add(GenerateMenuFile(ExportEnum.Site_MenuAvancement, targetDirectory, siteStructure, source, routingNode));
                progress?.Report(BatchProgressInfo.Step(++currentStep));

                // 4. Génération du menu des prochains combats
                if (config.PublierProchainsCombats)
                {
                    output.Add(GenerateMenuFile(ExportEnum.Site_MenuProchainCombats, targetDirectory, siteStructure, source, routingNode));
                    progress?.Report(BatchProgressInfo.Step(++currentStep));
                }

                // 5. Génération du menu des engagements
                if (config.PublierEngagements)
                {
                    output.Add(GenerateMenuFile(ExportEnum.Site_MenuEngagements, targetDirectory, siteStructure, source, routingNode));
                    progress?.Report(BatchProgressInfo.Step(++currentStep));
                }

                // 6. Génération du menu des statistiques (optionnel)
                if (config.PublierStatistiques)
                {
                    output.Add(GenerateMenuFile(ExportEnum.Site_MenuStatistiques, targetDirectory, siteStructure, source, routingNode));
                    progress?.Report(BatchProgressInfo.Step(++currentStep));
                }
            }

            LogTools.Logger?.Debug("Menu = {0}", output.Count);

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
            if (ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération de l'affectation des tapis.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null)
            {
                LogTools.Logger?.Error("DataContext est null dans le contexte d'export.");
                return output;
            }

            progress?.Report(BatchProgressInfo.Init(1));

            string targetDirectory = siteStructure.PhysicalStructure.RepertoireCommon();
            ExportEnum exportType = ExportEnum.Site_AffectationTapis;

            // Appel unifié avec notre méthode utilitaire
            string savePath = GetFileSavePath(targetDirectory, exportType);
            // Le chemin physique final réel (avec extension)
            string fullHtmlPath = $"{savePath}.html";

            // 1. Génération du dictionnaire de routage indépendant
            XElement routingNode = GenerateSiteRoutes(ctx, siteStructure, fullHtmlPath, false);

            // 2. Injection du dictionnaire dans les paramètres XSLT
            var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, ("SiteRoutes", routingNode.CreateNavigator()));

            // Génération du document et enrichissement via le contexte
            XDocument outDoc = ExportXML.CreateDocumentAffectationTapis(ctx);
            ctx.EnrichWithConfiguration(outDoc);

            LogTools.DebugLogData(outDoc);

            using (var source = new XmlSource(outDoc))
            {
                SiteExportEngine.GenererHtmlSite(source, exportType, savePath, xsltArgs);
            }

            output.Add(new FileWithChecksum($"{savePath}.html"));


            LogTools.Logger?.Debug("Affectation = {0}", output.Count);
            progress?.Report(BatchProgressInfo.Step(1));

            return output;
        }

        /// <summary>
        /// Genere la page des engages
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        /// <summary>
        public List<FileWithChecksum> GenereWebSiteEngagements(IReadOnlyCollection<GroupeEngagements> grps, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            if (grps == null || ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération des engagements.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            IExtendedJudoData EDC = ctx.ExtendedDataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null || EDC == null)
            {
                LogTools.Logger?.Error("DataContext ou ExtendedDataContext est null dans le contexte d'export.");
                return output;
            }
            int nbGrps = grps.Count;
            progress?.Report(BatchProgressInfo.Init(nbGrps));

            ExportEnum exportType = ExportEnum.Site_Engagements;

            // --- DÉBUT DE L'OPTIMISATION XPATH ---
            // La plomberie de compilation XML est maintenant 100% encapsulée.
            // L'appel au registre déclenche le Lazy (si ce n'est pas déjà fait) et renvoie directement l'arbre optimisé.
            XPathDocument xpathEngagements = ctx.GetCompiledDocument(nameof(ExportDocumentKey.Engagements));

            // Sécurité : vérifier que la génération a bien eu lieu
            if (xpathEngagements == null)
            {
                LogTools.Logger?.Error("Impossible de récupérer le document XPath pour les engagements.");
                return output; // Retourne une liste vide si le document n'est pas disponible
            }

            string dummyId = grps.FirstOrDefault()?.Id ?? "0";
            string dummySourcePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireGroupeEngagements(dummyId), "page.html");

            XElement routingNode = GenerateSiteRoutes(ctx, siteStructure, dummySourcePath, true);

            int currentStep = 0;

            // Remplacement de la boucle 'for' par un 'foreach' plus lisible
            foreach (GroupeEngagements grp in grps)
            {
                // Détermination du répertoire cible dynamique pour ce groupe
                string targetDirectory = siteStructure.PhysicalStructure.RepertoireGroupeEngagements(grp.Id);
                string savePath = GetFileSavePath(targetDirectory, exportType);

                var xsltArgs = CreateAllXsltArgs(siteStructure, savePath,
                    ("idgroupe", grp.Id),
                    ("idcompetition", grp.Competition),
                    ("SiteRoutes", routingNode.CreateNavigator())
                );

                // Transformation HTML à partir du document contextuel
                SiteExportEngine.GenererHtmlSite(xpathEngagements, exportType, savePath, xsltArgs);

                output.Add(new FileWithChecksum($"{savePath}.html"));

                progress?.Report(BatchProgressInfo.Step(++currentStep));
            }


            LogTools.Logger?.Debug("Engagements = {0}", output.Count);

            return output;
        }


        /// <summary>
        /// Genere la page des statistiques
        /// </summary>
        /// <param name="grps">Liste des groupes de statistiques</param>
        /// <param name="ctx">Le contexte d'export</param>
        /// <param name="siteStructure">Générateur d'URL</param>
        /// <param name="progress">Rapporteur de progression</param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteStatistiques(IReadOnlyCollection<GroupeStatistiques> grps, ExportSharedContext ctx, SiteUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {


            if (grps == null || ctx == null || siteStructure == null)
            {
                LogTools.Logger?.Error("Paramètres invalides pour la génération des statistiques.");
                return new List<FileWithChecksum>();
            }

            IJudoData DC = ctx.DataContext;
            IExtendedJudoData EDC = ctx.ExtendedDataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC == null || EDC == null)
            {
                LogTools.Logger?.Error("DataContext ou ExtendedDataContext est null dans le contexte d'export.");
                return output;
            }

            int nbGrps = grps.Count;
            progress?.Report(BatchProgressInfo.Init(nbGrps));

            // Note: Assurez-vous que ExportEnum.Site_Statistiques existe
            ExportEnum exportType = ExportEnum.Site_Statistiques;

            // --- UTILISATION DE L'OPTIMISATION XPATH ---
            // Note: Assurez-vous d'avoir ajouté "Statistiques" dans l'énumération ExportDocumentKey
            XPathDocument xpathStatistiques = ctx.GetCompiledDocument(nameof(ExportDocumentKey.Statistiques));
            
            // Sécurité : vérifier que la génération a bien eu lieu
            if (xpathStatistiques == null)
            {
                LogTools.Logger?.Error("Impossible de récupérer le document XPath pour les statistiques.");
                return output;
            }

            string dummyId = grps.FirstOrDefault()?.Id ?? "0";
            string dummySourcePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireGroupeStatistiques(dummyId), "page.html");

            XElement routingNode = GenerateSiteRoutes(ctx, siteStructure, dummySourcePath, false);

            int currentStep = 0;

            foreach (var grp in grps)
            {
                // Détermination du répertoire cible dynamique pour ce groupe de statistiques
                string targetDirectory = siteStructure.PhysicalStructure.RepertoireGroupeStatistiques(grp.Id);
                string savePath = GetFileSavePath(targetDirectory, exportType);

                var xsltArgs = CreateAllXsltArgs(siteStructure, savePath,
                    ("idgroupe", grp.Id),
                    ("idcompetition", grp.Competition),
                    ("SiteRoutes", routingNode.CreateNavigator())
                );

                // Transformation HTML à partir du document contextuel
                SiteExportEngine.GenererHtmlSite(xpathStatistiques, exportType, savePath, xsltArgs);

                output.Add(new FileWithChecksum($"{savePath}.html"));

                progress?.Report(BatchProgressInfo.Step(++currentStep));
            }

            LogTools.Logger?.Debug("Statistiques = {0}", output.Count);

            return output;
        }
        #endregion

        #region METHODES PRIVEES

        /// <summary>
        /// Génère un dictionnaire de routage XML indépendant contenant toutes les URLs pré-calculées du site.
        /// Ce dictionnaire est destiné à être injecté en tant que paramètre XSLT.
        /// </summary>
        private XElement GenerateSiteRoutes(ExportSharedContext ctx, SiteUrlGenerator siteStructure, string sourcePhysicalFile, bool includeGroupes = true)
        {
            XElement rootRoutes = new XElement(ConstantXML.Routing_SiteRoutes);

            // Récupération des caches O(1) depuis le contexte
            IJudoData DC = ctx.DataContext;
            var phasesByEpreuve = DC.Deroulement.Phases.ToLookup(p => p.epreuve);

            // =========================================================================
            // 1. ROUTES DES COMPETITIONS, EPREUVES ET LEURS PHASES
            // =========================================================================
            foreach (var competition in DC.Organisation.Competitions)
            {
                // Sélection exacte selon le type d'équipe (Individuel vs Équipe)
                IEnumerable<i_vue_epreuve_interface> epreuves_compet = competition.IsEquipe()
                    ? DC.Organisation.VueEpreuveEquipes.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>()
                    : DC.Organisation.VueEpreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>();

                // Application du même filtre que CreateDocumentMenu (uniquement les épreuves ayant des phases valides)
                var epreuvesFiltrees = epreuves_compet
                    .Where(ep => phasesByEpreuve[ep.id].Any(o => o.etat > EtatPhaseEnum.Cree));

                foreach (var epreuve in epreuvesFiltrees)
                {
                    string targetPhysicalDir = siteStructure.PhysicalStructure.RepertoireEpreuve(epreuve.id.ToString(), epreuve.nom);
                    string webPath = siteStructure.GetRelativeWebPath(sourcePhysicalFile, targetPhysicalDir, true);

                    string classementFichier = SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_ClassementFinal);
                    string prochainsCombatsFichier = SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_FeuilleCombat);

                    var phasesValides = phasesByEpreuve[epreuve.id].Where(p => p.etat > EtatPhaseEnum.Cree).ToList();

                    // A. Phase active (pour le lien direct au niveau de l'épreuve)
                    IPhase phaseActive = phasesValides.GetPhaseActive();
                    string urlAvancementActive = string.Empty;

                    if (phaseActive != null)
                    {
                        ExportEnum exportTypeActive = (phaseActive.typePhase == TypePhaseEnum.Poule)
                            ? ExportEnum.Site_Poule_Resultat
                            : ExportEnum.Site_Tableau_Competition;

                        string phaseFichierActive = SiteExportEngine.GetSanitizedFileName(exportTypeActive);
                        urlAvancementActive = $"{webPath}{phaseFichierActive}.html";
                    }

                    // --- Route Épreuve ---
                    rootRoutes.Add(new XElement(ConstantXML.Routing_RouteEpreuve,
                        new XAttribute(ConstantXML.Routing_EpreuveId, epreuve.id),
                        new XAttribute(ConstantXML.Routing_EpreuveUrlClassement, $"{webPath}{classementFichier}.html"),
                        new XAttribute(ConstantXML.Routing_EpreuveUrlProchainsCombats, $"{webPath}{prochainsCombatsFichier}.html"),
                        new XAttribute(ConstantXML.Routing_EpreuveUrlAvancement, urlAvancementActive)
                    ));

                    // --- B. Route de CHAQUE Phase valide ---
                    foreach (var phase in phasesValides)
                    {
                        ExportEnum exportType = (phase.typePhase == TypePhaseEnum.Poule)
                            ? ExportEnum.Site_Poule_Resultat
                            : ExportEnum.Site_Tableau_Competition;

                        string phaseFichier = SiteExportEngine.GetSanitizedFileName(exportType);

                        rootRoutes.Add(new XElement(ConstantXML.Routing_RoutePhase,
                            new XAttribute(ConstantXML.Routing_EpreuveId, epreuve.id),
                            new XAttribute(ConstantXML.Routing_PhaseId, phase.id),
                            new XAttribute(ConstantXML.Routing_PhaseUrlAvancement, $"{webPath}{phaseFichier}.html")
                        ));
                    }
                }
            }

            // =========================================================================
            // 2. ROUTES DES GROUPES (Engagements)
            // =========================================================================
            if (includeGroupes && ctx.Config.PublierEngagements)
            {
                string fileNameEngagements = SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_Engagements);
                foreach (var grp in ctx.ExtendedDataContext.Engagement.GroupesEngages)
                {
                    string targetPhysicalDir = siteStructure.PhysicalStructure.RepertoireGroupeEngagements(grp.Id);
                    string webPath = siteStructure.GetRelativeWebPath(sourcePhysicalFile, targetPhysicalDir, true);

                    rootRoutes.Add(new XElement(ConstantXML.Routing_RouteGroupe,
                        new XAttribute(ConstantXML.Routing_GroupeId, grp.Id),
                        new XAttribute(ConstantXML.Routing_TypeGroupe, ConstantXML.Routing_TypeEngagement),
                        new XAttribute(ConstantXML.Routing_UrlGroupe, $"{webPath}{fileNameEngagements}.html")
                    ));
                }
            }

            // =========================================================================
            // 3. ROUTES DES GROUPES (Statistiques)
            // =========================================================================
            if (includeGroupes && ctx.Config.PublierStatistiques)
            {
                string fileNameStats = SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_Statistiques);
                foreach (var grp in ctx.ExtendedDataContext.StatistiquesCombats.GroupesStatistiques)
                {
                    string targetPhysicalDir = siteStructure.PhysicalStructure.RepertoireGroupeStatistiques(grp.Id);
                    string webPath = siteStructure.GetRelativeWebPath(sourcePhysicalFile, targetPhysicalDir, true);

                    rootRoutes.Add(new XElement(ConstantXML.Routing_RouteGroupe,
                        new XAttribute(ConstantXML.Routing_GroupeId, grp.Id),
                        new XAttribute(ConstantXML.Routing_TypeGroupe, ConstantXML.Routing_TypeStatistique),
                        new XAttribute(ConstantXML.Routing_UrlGroupe, $"{webPath}{fileNameStats}.html")
                    ));
                }
            }

            return rootRoutes;
        }

        #endregion
    }
}