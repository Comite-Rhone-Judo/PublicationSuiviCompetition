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
            System.ArgumentNullException.ThrowIfNull(DC);
            System.ArgumentNullException.ThrowIfNull(config);

            var context = new ExportSharedContextInterne(DC, config);
            // Enregistrement paresseux (Lazy) du document spécifique aux combats.
            // La méthode CreateDocumentFeuilleCombat ne sera exécutée que si la clé est appelée.
            context.RegisterLazyDocument(
                nameof(ExportDocumentKey.FeuillesCombat),
                () => ExportXML.CreateDocumentFeuilleCombat(context, null, null)
            );

            // Lancement du pipeline centralisé dans la classe mère
            context.ExecuteExportPipeline(config.ToXml());

            return context;
        }

        #endregion

        #region PROPRIETES
        public ConfigurationExportSiteInterne Config { get; private set; }
        #endregion

    }
}