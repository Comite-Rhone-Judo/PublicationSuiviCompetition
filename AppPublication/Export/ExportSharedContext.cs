using AppPublication.ExtensionNoyau;
using FranceJudo.Metier.Noyau;


namespace AppPublication.Export
{
    public class ExportSharedContext : ExportSharedContextBase
    {
        // Propriété spécifique à ce contexte conservée pour l'appelant
        public ConfigurationExportSite Config { get; private set; }

        // TODO Il faut ajouter la liste des ecrans d'appel en snapshot

        // Constructeur privé pour obliger l'utilisation de la Factory 'Create'
        private ExportSharedContext() : base() { }

        /// <summary>
        /// Factory pour créer une instance initialisée des engagements
        /// </summary>
        public static ExportSharedContext Create(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config)
        {
            if (DC == null) throw new System.ArgumentNullException(nameof(DC));
            if (EDC == null) throw new System.ArgumentNullException(nameof(EDC));
            if (config == null) throw new System.ArgumentNullException(nameof(config));

            var context = new ExportSharedContext { Config = config };

            // Génération du document spécifique aux engagements
            var docEngagements = ExportXML.CreateDocumentEngagements(DC, EDC);

            // Lancement du pipeline centralisé dans la classe mère
            context.ExecuteExportPipeline(DC, config.ToXml(), docEngagements);

            return context;
        }
    }
}