using AppPublication.ExtensionNoyau;
using KernelImpl;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Tools.Export;
using Tools.Logging;
using Tools.Outils;


namespace AppPublication.Export
{
    public class ExportSharedContext : ExportSharedContextBase
    {
        #region PROPERTIES
        public XDocument DocEngagements { get; private set; }
        public XElement SiteConfiguration { get; private set; }

        public ConfigurationExportSite Config { get; private set; }
        #endregion

        #region CONSTRUCTORS
        protected ExportSharedContext() : base() { }

        /// <summary>
        /// Factory pour creer une instance initialisee
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        /// <returns></returns>
        public static ExportSharedContext Instance(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config)
        {
            // Verifie les arguments
            if (DC== null) { throw new System.ArgumentNullException(nameof(DC)); }
            if (EDC == null) { throw new System.ArgumentNullException(nameof(EDC)); }
            if(config == null) { throw new System.ArgumentNullException(nameof(config)); }

            var output = new ExportSharedContext();
            output.Initialize(DC, EDC, config);

            return output;
        }
        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// Ajoute toutes les informations se trouvant dans le contexte d'export au document XML
        /// </summary>
        /// <param name="doc"></param>
        public override void AddFullXmlContext(XDocument doc)
        {
            if (doc?.Root == null) return;

            // Ajoute les informations de structure de base (clubs, comités, etc.) de manière sécurisée
            base.AddFullXmlContext(doc);

            // Ajoute la configuration spécifique uniquement si elle n'est pas déjà présente
            AddConfigurationXmlContext(doc);
        }

        /// <summary>
        /// Ajoute les informations de structure se trouvant dans le contexte d'export au document XML
        /// </summary>
        /// <param name="doc"></param>
        public void AddBaseXmlContext(XDocument doc)
        {
            // Ajoute les informations de structure de base (clubs, comites, secteurs, ligues, pays)
            base.AddFullXmlContext(doc);
        }

        /// <summary>
        /// Ajoute les informations de configuration se trouvant dans le contexte d'export au document XML
        /// </summary>
        /// <param name="doc"></param>
        public void AddConfigurationXmlContext(XDocument doc)
        {
            // Ajoute la configuration specifiques
            if (SiteConfiguration != null && doc.Root?.Element(SiteConfiguration.Name) == null)
            {
                doc.Root?.Add(SiteConfiguration);
            }
        }

        #endregion

        #region METHODES PRIVEES
        protected virtual void Initialize(IJudoData DC, ExtendedJudoData EDC, ConfigurationExportSite config)
        {
            // Execute l'initialisation du parent
            base.Initialize(DC);

            // Stock la configuration
            Config = config;

            // Recupere la configuration du site en XML
            SiteConfiguration = config.ToXml();

            // Le document general des engagements
            DocEngagements = ExportXML.CreateDocumentEngagements(DC, EDC);
            AddFullXmlContext(DocEngagements);
            LogTools.DebugLogData(DocEngagements);
        }
        #endregion
    }
}
