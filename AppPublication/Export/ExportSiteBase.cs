using FranceJudo.Core.Logging;
using FranceJudo.Metier.Export;
using FranceJudo.Metier.Site;
using FranceJudo.Metier.XML;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;




namespace AppPublication.Export
{
    public abstract class ExportSiteBase<TContext> where TContext : ExportSharedContextBase
    {
        #region MEMBRES
        protected const int kTailleMaxNomCompetition = 30;
        protected readonly TContext _context;

        // Cache de routage par profondeur ---
        // Utilise la profondeur (int) comme clé. Le Lazy garantit une instanciation thread-safe unique.
        private readonly ConcurrentDictionary<string, Lazy<XElement>> _routesCache = new ConcurrentDictionary<string, Lazy<XElement>>();

        #endregion

        #region CONSTRUCTEURS
        public ExportSiteBase(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion

        #region METHODES PROTECTED VIRTUELLES
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

            // 2. INJECTION DU RÉFÉRENTIEL
            // On passe un Navigator (objet .NET standard) sous le nom "RefData"
            if (_context.ReferenceData != null)
            {
                args.AddParam("RefData", "", _context.ReferenceData.CreateNavigator());
            }

            // INJECTION DU ROUTAGE AUTOMATIQUE AVEC CACHE
            // On calcule la profondeur du fichier cible par rapport à la racine pour la clé de cache
            string relativeSource = Path.GetRelativePath(siteStruct.PhysicalStructure.RepertoireCompetition, savePath);
            string cacheKey = Path.GetDirectoryName(relativeSource) ?? string.Empty;

            XElement routingNode = _routesCache.GetOrAdd(cacheKey, key => new Lazy<XElement>(() =>
            {
                // Appelle la logique spécifique à la classe enfant (Public ou Interne)
                 XElement rootRoutes = GenerateSiteRoutesNode(siteStruct, savePath);
                LogTools.DebugLogData(rootRoutes);
                return rootRoutes;

            })).Value;

            args.AddParam("SiteRoutes", "", routingNode.CreateNavigator());

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
            string filename = $"{SiteExportEngine.GetSanitizedFileName(exportType)}{(string.IsNullOrEmpty(suffix) ? "" : $"-{suffix}")}";
            return Path.Combine(targetDirectory, filename);
        }

        /// <summary>
        /// Méthode à surcharger dans les classes dérivées pour construire l'arbre XML des routes spécifiques.
        /// </summary>
        protected virtual XElement GenerateSiteRoutesNode<T>(UrlGeneratorBase<T> siteStruct, string sourcePhysicalFile) where T : PhysicalStructureBase
        {
            XElement rootRoutes = new XElement(ConstantXML.Routing_SiteRoutes);

            // Appel direct aux méthodes génériques existantes dans UrlGeneratorBase<T>
            rootRoutes.Add(new XAttribute(ConstantXML.Routing_UrlImg, siteStruct.GetRelativeUrlImg(sourcePhysicalFile)));
            rootRoutes.Add(new XAttribute(ConstantXML.Routing_UrlJs, siteStruct.GetRelativeUrlJs(sourcePhysicalFile)));
            rootRoutes.Add(new XAttribute(ConstantXML.Routing_UrlCss, siteStruct.GetRelativeUrlCss(sourcePhysicalFile)));

            // Si UrlCommon est géré de la même façon dans UrlGeneratorBase :
            // rootRoutes.Add(new XAttribute(ConstantXML.Routing_UrlCommon, siteStruct.GetRelativeUrlCommon(sourcePhysicalFile)));

            return rootRoutes;
        }

        #endregion
    }
}