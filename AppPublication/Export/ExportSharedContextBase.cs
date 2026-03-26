using KernelImpl;
using KernelImpl.Noyau.Structures;
using System.Xml.Linq;
using Tools.Export;
using FranceJudo.Core.Logging;

namespace AppPublication.Export
{
    public abstract class ExportSharedContextBase
    {
        #region PROPERTIES
        // Référentiels de base
        public XElement Clubs { get; private set; }
        public XElement Comites { get; private set; }
        public XElement Secteurs { get; private set; }
        public XElement Ligues { get; private set; }
        public XElement Pays { get; private set; }
        public XElement Ceintures { get; private set; }

        // Configuration spécifique remontée pour factorisation
        public XElement SiteConfiguration { get; protected set; }

        // Document principal généré (Unifie DocCombats et DocEngagements)
        public XDocument ExportDocument { get; protected set; }
        #endregion

        protected ExportSharedContextBase() { }

        #region METHODES PUBLIQUES D'ENRICHISSEMENT (API EXTERNE)

        /// <summary>
        /// Enrichit le document avec toutes les informations (Structure de base + Configuration du site)
        /// </summary>
        public virtual void EnrichWithFullContext(XDocument doc)
        {
            EnrichWithBaseStructure(doc);
            EnrichWithConfiguration(doc);
        }

        /// <summary>
        /// Enrichit le document uniquement avec les référentiels de base (Clubs, Comités, Ligues...)
        /// </summary>
        public void EnrichWithBaseStructure(XDocument doc)
        {
            if (doc?.Root == null) return;

            XElement[] elementsToInject = { Clubs, Comites, Ligues, Secteurs, Pays, Ceintures };

            foreach (XElement element in elementsToInject)
            {
                if (element != null && doc.Root.Element(element.Name) == null)
                {
                    doc.Root.Add(element);
                }
            }
        }

        /// <summary>
        /// Enrichit le document uniquement avec la configuration spécifique du site
        /// </summary>
        public void EnrichWithConfiguration(XDocument doc)
        {
            if (doc?.Root == null || SiteConfiguration == null) return;

            if (doc.Root.Element(SiteConfiguration.Name) == null)
            {
                doc.Root.Add(SiteConfiguration);
            }
        }

        #endregion

        #region PIPELINE D'INITIALISATION
        /// <summary>
        /// Workflow centralisé garantissant l'ordre d'initialisation pour toutes les classes filles.
        /// </summary>
        protected void ExecuteExportPipeline(IJudoData DC, XElement configXml, XDocument generatedDoc)
        {
            // 1. Chargement des référentiels
            Clubs = ExportXML.GetClubs(DC);
            Comites = ExportXML.GetComites(DC);
            Secteurs = ExportXML.GetSecteurs(DC);
            Ligues = ExportXML.GetLigues(DC);
            Pays = ExportXML.GetPays(DC);
            Ceintures = ExportXML.GetCeintures(DC);

            // 2. Assignation des spécificités transmises par l'enfant
            SiteConfiguration = configXml;
            ExportDocument = generatedDoc;

            // 3. Enrichissement automatique du document généré
            EnrichWithFullContext(ExportDocument);

            // 4. Log
            LogTools.DebugLogData(ExportDocument);
        }
        #endregion
    }
}