using System;
using System.IO;
using System.Xml.Xsl;
using FranceJudo.Metier.Site;
using FranceJudo.Metier.Export;




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
        /// Crée une XsltArgumentList pré-remplie avec la structure du site et des paramètres optionnels.
        /// </summary>
        /// <param name="siteStruct"></param>
        /// <param name="savePath"></param>
        /// <param name="extraParams"></param>
        /// <returns></returns>
        protected virtual XsltArgumentList CreateAllXsltArgs<T>(UrlGeneratorBase<T> siteStruct, string savePath, params (string name, object value)[] extraParams) where T : PhysicalStructureBase
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
        protected virtual void AddStructureArgument<T>(XsltArgumentList argsList, UrlGeneratorBase<T> urlGen, string targetFile) where T : PhysicalStructureBase
        {
            // Ajoute les parametres en relatif par rapport a   la position du fichier
            argsList.AddParam("imgPath", "", urlGen.GetRelativeUrlImg(targetFile));
            argsList.AddParam("jsPath", "", urlGen.GetRelativeUrlJs(targetFile));
            argsList.AddParam("cssPath", "", urlGen.GetRelativeUrlCss(targetFile));
            argsList.AddParam("competitionPath", "", urlGen.GetRelativeUrlCompetition(targetFile));
        }
    }
}