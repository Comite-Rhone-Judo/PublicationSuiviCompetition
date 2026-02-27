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