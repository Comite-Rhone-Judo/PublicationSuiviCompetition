using FranceJudo.Core.Logging;
using FranceJudo.Metier.ExtensionNoyau;
using FranceJudo.Metier.Noyau;
using System.Xml.Linq;


namespace AppPublication.Export
{
    public class ExportSharedContext : ExportSharedContextBase
    {
        // Propriété spécifique à ce contexte conservée pour l'appelant
        public ConfigurationExportSite Config { get; private set; }

        // Constructeur privé pour obliger l'utilisation de la Factory 'Create'
        private ExportSharedContext(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config) : base(DC, EDC) {
            Config = config;
        }

        /// <summary>
        /// Factory pour créer une instance initialisée des engagements
        /// </summary>
        public static ExportSharedContext Create(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config)
        {
            System.ArgumentNullException.ThrowIfNull(DC);
            System.ArgumentNullException.ThrowIfNull(EDC);
            System.ArgumentNullException.ThrowIfNull(config);

            var context = new ExportSharedContext(DC, EDC, config);

            // Injection dans le parent générique via nameof()
            // Le document spécifique aux engagements
            context.RegisterLazyDocument(
                    nameof(ExportDocumentKey.Engagements),
                    () => {
                        // 1. Génération
                        XDocument doc = ExportXML.CreateDocumentEngagements(context);

                        // 2. Trace unique en XDocument !
                        LogTools.DebugLogData(doc);

                        // 3. Renvoi pour compilation dans le pipeline
                        return doc;
                    });

            context.RegisterLazyDocument(
                    nameof(ExportDocumentKey.Statistiques),
                    () =>
                    {
                        XDocument doc = ExportXML.CreateDocumentStatistiques(context);
                        LogTools.DebugLogData(doc);
                        return doc;
                    }
                );

            // Lancement du pipeline centralisé dans la classe mère
            context.ExecuteExportPipeline(config.ToXml());

            return context;
        }
    }
}