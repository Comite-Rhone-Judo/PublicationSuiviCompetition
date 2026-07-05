using AppPublication.Models.EcransAppel;
using AppPublication.Publication;
using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using FranceJudo.Metier.Export;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Site;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using static AppPublication.Models.EcransAppel.EcranAppelModel;

namespace AppPublication.Export
{
    public class ExportSiteInterne<TContext> : ExportSiteBase<TContext> where TContext : ExportSharedContextBase
    {
        #region CONSTRUCTEURS
        public ExportSiteInterne(TContext context) : base(context)
        {
        }
        #endregion

        /// <summary>
        /// Génère la page d'index du site, les scripts de mise à jour et exporte les ressources statiques.
        /// </summary>
        public List<FileWithChecksum> GenereWebSiteIndex(ExportSharedContextInterne ctx, SiteInterneUrlGenerator siteStructure, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;
            // Clone la structure de répertoires pour le contexte multi-thread
            List<FileWithChecksum> output = new List<FileWithChecksum>();

                

            if (DC != null && ctx != null && siteStructure != null)
            {
                // 1. Génération du document d'index de base
                XDocument outDoc = ExportXML.CreateDocumentIndex(ctx);

                // 2. Ajout de la CONFIGURATION uniquement (pas de structures de clubs/ligues)
                // On suppose que cette méthode dans ctx injecte PublicationInfo et SiteConfiguration
                ctx.EnrichWithConfiguration(outDoc);

                LogTools.DebugLogData(outDoc);

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
                string footerFilename = SiteExportEngine.GetFileName(footerType).Replace("/", "_");
                string footerSavePath = Path.Combine(siteStructure.PhysicalStructure.RepertoireJs(), footerFilename);

                var footerArgs = CreateAllXsltArgs(siteStructure, footerSavePath);

                // Utilisation du même docIndex pour générer le JS via XSLT
                using (var source = new XmlSource(outDoc))
                {
                    SiteExportEngine.GenererHtmlSite(source, footerType, footerSavePath, footerArgs, "js");
                }
                output.Add(new FileWithChecksum($"{footerSavePath}.js"));

                LogTools.Logger?.Debug("GenereWebSiteIndex Terminé - Total: {0} ressources", output.Count);
                progress?.Report(BatchProgressInfo.Step(2));
            }

            progress?.Report(BatchProgressInfo.Step(2));
            return output;
        }

        /// <summary>
        /// Genere les pages des ecrans d'appel pour les groupes de tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="ctx"></param>
        /// <param name="structRep"></param>
        /// <param name="ecran"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereEcransAppel(ExportSharedContextInterne ctx, SiteInterneUrlGenerator siteStructure, IReadOnlyCollection<EcranAppelModel> ecrans, IProgress<BatchProgressInfo> progress)
        {
            IJudoData DC = ctx.DataContext;
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            progress?.Report(BatchProgressInfo.Init(ecrans.Count));
            var exportType = ExportEnum.Site_Interne_EcranAppel;
            var targetDirectory = siteStructure.PhysicalStructure.RepertoireEcransAppel();

            // 1. COMPILATION XPATH (Zéro allocation pour le moteur XSLT)
            XPathDocument xpathEcrans = ctx.GetCompiledDocument(nameof(ExportDocumentKey.FeuillesCombat));

            // Sécurité : vérifier que la génération a bien eu lieu
            if (xpathEcrans == null)
            {
                LogTools.Logger?.Error("Impossible de récupérer le document XPath pour les combats.");
                return output; // Retourne une liste vide si le document n'est pas disponible
            }

            // Ici on ne prend que les numeros de tapis qui sont dans la limite de la competition (cas ou on a plus de tapis configures que de tapis declarés)
            // On cherche le plus grand nombre de tapis si on a plusieurs competitions.
            int nbTapisMax = DC.Organisation.Competitions.Max(c => c.nbTapis);
            int currentStep = 0;

            // Calcul si on doit prendre en compte l'intitulé commun pour les compétitions multiples
            bool useIntituleCommun = DC.Organisation.Competitions.Count > 1
                         && ctx.Config.UseIntituleCommun
                         && !string.IsNullOrEmpty(ctx.Config.IntituleCommun);

            foreach (var ecran in ecrans)
            {
                // Le fichier de destination
                string savePath = GetFileSavePath(targetDirectory, exportType, (ecran.Id >= 0) ? $"{ecran.Id:00}" : "default");

                var ecransParams = new List<(string, object)>();
                ecransParams.Add(("useIntituleCommun", useIntituleCommun.ToString().ToLower()));
                ecransParams.Add(("idEcran", ecran.Id));                 // Le numero de l'ecran d'appel
                ecransParams.Add(("tailleGroupe", ecran.Groupement));     // La taille du groupe
                ecransParams.Add(("dispositionAffichage", ecran.Disposition.ToString().ToLower()));

                XDocument docParams = new XDocument(
                                            new XElement("tapisIds",
                                            ecran.TapisIds.Where(num => (num <= nbTapisMax)).Select(num => new XElement("tapis",
                                                                                new XAttribute("id", num)))));    // La liste des tapis doit etre passee sous forme d'un NodeSet
                ecransParams.Add(("tapisAffiches", docParams.CreateNavigator().Select("/")));

                ecransParams.Add(("combatsParPageEff", ecran.NbCombatsPage));
                // On le garde au cas ou pour la suite, mais normalement, la disposition des combats est gere via la disposition d'affichage
                ecransParams.Add(("isAffichageCombatLigne", ecran.DispositionCombat == DispositionAffichage.Ligne ? "true" : "false"));

                // Option d'auto ajustement du texte en fonction de la taille du groupe
                ecransParams.Add(("ajusteTexteAuto", ecran.AjusteTailleTexte ? "true" : "false"));

                // Les arguments XSLT (inclut la structure du site et le chemin cible)
                var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, ecransParams.ToArray());

                SiteExportEngine.GenererHtmlSite(xpathEcrans, exportType, savePath, xsltArgs);

                output.Add(new FileWithChecksum($"{savePath}.html"));

                progress?.Report(BatchProgressInfo.Step(++currentStep));
            }

            return output;
        }

        /// <summary>
        /// Ajoute les arguments de structure du site pour les templates xslt
        /// </summary>
        /// <param name="argsList">La liste d'argument a actualiser</param>
        /// <param name="siteStruct">La structure du site</param>
        /// <param name="targetFile">Le fichier HTML cible</param>
        protected override void AddStructureArgument<T>(XsltArgumentList argsList, UrlGeneratorBase<T> siteStruct, string targetFile)
        {
            SiteInterneUrlGenerator urlGen = siteStruct as SiteInterneUrlGenerator;

            // Ajoute les repertoires de base de la structure
            base.AddStructureArgument(argsList, urlGen, targetFile);
        }
    }
}