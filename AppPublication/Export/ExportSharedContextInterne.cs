using AppPublication.ExtensionNoyau;
using KernelImpl;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Tools.Export;
using Tools.Logging;
using Tools.Outils;


namespace AppPublication.Export
{
    public class ExportSharedContextInterne : ExportSharedContextBase
    {
        #region Members
        protected static XDocument _docCombats = new XDocument();     // Instance partagees pour la generation combats
        #endregion

        #region PROPERTIES
        public XElement SiteConfiguration { get; private set; }

        public XDocument DocCombats
        {
            get
            {
                return _docCombats;
            }
            private set
            {
                _docCombats = value;
            }
        }
        #endregion

        #region CONSTRUCTORS
        protected ExportSharedContextInterne() : base() { }

        /// <summary>
        /// Factory pour creer une instance initialisee
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        /// <returns></returns>
        public static ExportSharedContextInterne Instance(IJudoData DC, ConfigurationExportSiteInterne config)
        {
            var output = new ExportSharedContextInterne();
            output.Initialize(DC, config);
            return output;
        }
        #endregion

        #region METHODES PUBLIQUES
        protected virtual void Initialize(IJudoData DC, ConfigurationExportSiteInterne config)
        {
            // Execute l'initialisation du parent
            base.Initialize(DC);

            // Recupere la configuration du site
            // TODO Ajouter la méthode ToXml
            // SiteConfiguration = config.ToXml();

            _docCombats = ExportXML.CreateDocumentFeuilleCombat(DC, null, null);
            AddFullXmlContext(_docCombats);
            LogTools.DebugLogData(_docCombats);
        }
        #endregion
    }
}
