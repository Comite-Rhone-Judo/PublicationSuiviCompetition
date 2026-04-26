using FranceJudo.Metier.Noyau;
using System.Xml.Linq;

namespace AppPublication.Export
{
    public class ExportSharedContextInterne : ExportSharedContextBase
    {
        #region CONSTRUCTEURS

        // Constructeur privé pour obliger l'utilisation de la Factory 'Create'
        private ExportSharedContextInterne(IJudoData DC, ConfigurationExportSiteInterne config) : base(DC, null)
        {
            Config = config;
        }

        /// <summary>
        /// Factory pour créer une instance initialisée des combats
        /// </summary>
        public static ExportSharedContextInterne Create(IJudoData DC, ConfigurationExportSiteInterne config)
        {
            if (DC == null) throw new System.ArgumentNullException(nameof(DC));
            if (config == null) throw new System.ArgumentNullException(nameof(config));

            var context = new ExportSharedContextInterne(DC, config);

            // Génération du document spécifique aux combats (feuilles de combat)
            XDocument outDoc = ExportXML.CreateDocumentFeuilleCombat(context, null, null);

            // Lancement du pipeline centralisé dans la classe mère
            context.ExecuteExportPipeline(config.ToXml(), outDoc);

            return context;
        }

        #endregion

        #region PROPRIETES
        public ConfigurationExportSiteInterne Config { get; private set; }
        #endregion

    }
}