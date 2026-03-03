using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Generation;
using AppPublication.Tools.Enum;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;

namespace AppPublication.Export
{
    public abstract class ExportSiteBase<TContext> where TContext : ExportSharedContextBase
    {
        protected const int kTailleMaxNomCompetition = 30;
        protected readonly TContext _context;

        public ExportSiteBase(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Nettoie le path specifie pour passer de Repertoire Windows à URL
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string PathForUrl(string path)
        {
            string output = path.Replace('\\', '/');

            return output;
        }

        /// <summary>
        /// Crée une XsltArgumentList pré-remplie avec la structure du site et des paramètres optionnels.
        /// </summary>
        /// <param name="siteStruct"></param>
        /// <param name="savePath"></param>
        /// <param name="extraParams"></param>
        /// <returns></returns>
        protected virtual XsltArgumentList CreateAllXsltArgs(ExportSiteStructure siteStruct, string savePath, params (string name, object value)[] extraParams)
        {
            XsltArgumentList args = new XsltArgumentList();

            // On factorise l'appel systématique
            AddStructureArgument(args, siteStruct, savePath);

            // On ajoute les paramètres à la volée s'il y en a
            if (extraParams != null)
            {
                foreach (var (name, value) in extraParams)
                {
                    if (value != null)
                        args.AddParam(name, "", value);
                }
            }

            return args;
        }

        /// <summary>
        /// Génère le chemin de sauvegarde complet et standardisé pour un fichier d'export.
        /// </summary>
        /// <param name="targetDirectory">Le répertoire cible (commun ou spécifique à une épreuve)</param>
        /// <param name="exportType">Le type de fichier à exporter</param>
        /// <returns>Le chemin complet du fichier (sans l'extension)</returns>
        protected virtual string GetFileSavePath(string targetDirectory, ExportEnum exportType, string suffix = "")
        {
            string filename = $"{ExportTools.getFileName(exportType).Replace("/", "_")}{(string.IsNullOrEmpty(suffix) ? "" : $"-{suffix}")}";
            return Path.Combine(targetDirectory, filename);
        }

        /// <summary>
        /// Ajoute les arguments de structure du site pour les templates xslt
        /// </summary>
        /// <param name="argsList">La liste d'argument a actualiser</param>
        /// <param name="siteStruct">La structure du site</param>
        /// <param name="targetFile">Le fichier HTML cible</param>
        protected virtual void AddStructureArgument(XsltArgumentList argsList, ExportStructureBase siteStruct, string targetFile)
        {
            siteStruct.TargetPath = targetFile;

            // Ajoute les parametres en relatif par rapport a la position du fichier
            argsList.AddParam("imgPath", "", PathForUrl(siteStruct.RepertoireImg(relatif: true)));
            argsList.AddParam("jsPath", "", PathForUrl(siteStruct.RepertoireJs(relatif: true)));
            argsList.AddParam("cssPath", "", PathForUrl(siteStruct.RepertoireCss(relatif: true)));
            argsList.AddParam("competitionPath", "", PathForUrl(siteStruct.RepertoireCompetition(relatif: true)));
        }
    }
}