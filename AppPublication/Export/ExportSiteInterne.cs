using AppPublication.ExtensionNoyau;
using KernelImpl;
using System;
using System.Collections.Generic;
using System.Xml.Xsl;
using Tools.Export;
using Tools.Files;
using Tools.Logging;
using AppPublication.Generation;

namespace AppPublication.Export
{
    public class ExportSiteInterne : ExportSiteBase
    {
        /// <summary>
        /// Genere les pages des ecrans d'Appels
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public List<FileWithChecksum> GenereWebSiteEcransAppel(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSiteInterne config, ExportSiteInterneStructure siteStruct, IProgress<GenerationProgressInfo> progress, int workId)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            if (DC != null && EDC != null && config != null && siteStruct != null)
            {
            }

            LogTools.Logger.Debug("EcransAppel = {0}", output.Count);

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
            ExportSiteStructure theSiteStruct = siteStruct as ExportSiteStructure;

            // Ajoute les repertoires de base de la structure
            siteStruct.TargetPath = targetFile;
            base.AddStructureArgument(argsList, theSiteStruct, targetFile);
        }
    }
}