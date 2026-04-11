using FranceJudo.Metier.Noyau;

namespace AppPublication.Export
{
    public class ExportSharedContextInterne : ExportSharedContextBase
    {
        // Constructeur privé pour obliger l'utilisation de la Factory 'Create'
        private ExportSharedContextInterne() : base() { }

        /// <summary>
        /// Factory pour créer une instance initialisée des combats
        /// </summary>
        public static ExportSharedContextInterne Create(IJudoData DC, ConfigurationExportSiteInterne config)
        {
            if (DC == null) throw new System.ArgumentNullException(nameof(DC));
            if (config == null) throw new System.ArgumentNullException(nameof(config));

            var context = new ExportSharedContextInterne();

            // Génération du document spécifique aux combats (feuilles de combat)
            var docCombats = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);

            // Lancement du pipeline centralisé dans la classe mère
            context.ExecuteExportPipeline(DC, config.ToXml(), docCombats);

            return context;
        }
    }
}