using AppPublication.ExtensionNoyau;
using FranceJudo.Metier.Noyau;
using System.Xml.XPath;

namespace AppPublication.Export
{
    public interface IReadOnlyExportContext
    {
        /// <summary>
        /// Contrat en lecture seule fourni au moteur ExportXML.
        /// Empêche ExportXML de modifier la configuration ou de relancer des processus du contexte.
        /// </summary>
        // Contexte de données
        IJudoData DataContext { get; }

        // Contexte de données étendu
        IExtendedJudoData ExtendedDataContext { get; }

        // Caches de données pour les combats
        CombatExportCaches Caches { get; }

        // Le référentiel en lecture seule
        XPathDocument ReferenceData { get; }
    }
}