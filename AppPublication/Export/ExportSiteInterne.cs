using AppPublication.ExtensionNoyau;
using AppPublication.Generation;
using AppPublication.Models.EcransAppel;
using KernelImpl;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using Tools.Enum;
using Tools.Export;
using Tools.Files;
using Tools.Logging;
using Tools.Threading;

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
        public List<FileWithChecksum> GenereWebSiteIndex(IJudoData DC, ExportSharedContextInterne ctx, ExportSiteInterneStructure structRep, IProgress<BatchProgressInfo> progress)
        {
            // Clone la structure de répertoires pour le contexte multi-thread
            ExportSiteInterneStructure siteStructure = (ExportSiteInterneStructure)structRep.Clone();
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
                string footerSavePath = Path.Combine(siteStructure.RepertoireJs(), footerFilename);

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
        /// Genere les pages des ecrans d'appel pour les groupes de tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="ctx"></param>
        /// <param name="structRep"></param>
        /// <param name="ecran"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereEcranAppel(IJudoData DC, ExportSharedContextInterne ctx, ExportSiteInterneStructure structRep, EcranAppelModel ecran, IProgress<BatchProgressInfo> progress)
        {
            ExportSiteInterneStructure siteStructure = (ExportSiteInterneStructure)structRep.Clone();
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            var exportType = ExportEnum.Site_Interne_EcranAppel;
            var targetDirectory = structRep.RepertoireEcransAppel();

            // Le fichier de destination
            string savePath = GetFileSavePath(targetDirectory, exportType, (ecran.Id >= 0) ? $"{ecran.Id:00}" : "default");

            var ecransParams = new List<(string, object)>();
            ecransParams.Add(("idEcran", ecran.Id));                 // Le numero de l'ecran d'appel
            ecransParams.Add(("tailleGroupe", ecran.Groupement));     // La taille du groupe
            XDocument docParams = new XDocument(
                                        new XElement("tapisIds",
                                        ecran.TapisIds.Select(num => new XElement("tapis",
                                                                            new XAttribute("id",num)))));    // La liste des tapis doit etre passee sous forme d'un NodeSet
            ecransParams.Add(("tapisAffiches", docParams.CreateNavigator().Select("/")));

            // Les arguments XSLT (inclut la structure du site et le chemin cible)
            var xsltArgs = CreateAllXsltArgs(siteStructure, savePath, ecransParams.ToArray());

            progress?.Report(BatchProgressInfo.Init(1));

            ExportHTML.ToHTMLSite(ctx.ExportDocument, exportType, savePath, xsltArgs);

            output.Add(new FileWithChecksum($"{savePath}.html"));

            progress?.Report(BatchProgressInfo.Step(1));

            return output;
        }

        /// <summary>
        /// Ajoute les arguments de structure du site pour les templates xslt
        /// </summary>
        /// <param name="argsList">La liste d'argument a actualiser</param>
        /// <param name="siteStruct">La structure du site</param>
        /// <param name="targetFile">Le fichier HTML cible</param>
        protected override void AddStructureArgument(XsltArgumentList argsList, ExportStructureBase siteStruct, string targetFile)
        {
            ExportSiteInterneStructure theSiteStruct = siteStruct as ExportSiteInterneStructure;

            // Ajoute les repertoires de base de la structure
            theSiteStruct.TargetPath = targetFile;
            base.AddStructureArgument(argsList, theSiteStruct, targetFile);
        }
    }
}