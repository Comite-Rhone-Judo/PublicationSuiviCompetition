using FranceJudo.Metier.Noyau;

namespace AppPublication.Export
{
    public class ExportSharedContextInterne : ExportSharedContextBase
    {
        #region CONSTRUCTEURS

        // Constructeur privé pour obliger l'utilisation de la Factory 'Create'
        private ExportSharedContextInterne(IJudoData DC, ConfigurationExportSiteInterne config) : base(DC)
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
            var docCombats = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);

            // Lancement du pipeline centralisé dans la classe mère
            context.ExecuteExportPipeline(DC, config.ToXml(), docCombats);

            return context;
        }

        #endregion

        #region PROPRIETES
        public ConfigurationExportSiteInterne Config { get; private set; }
        #endregion

    }
}