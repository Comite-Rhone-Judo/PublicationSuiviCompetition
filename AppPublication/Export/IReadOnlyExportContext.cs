using AppPublication.ExtensionNoyau;
using FranceJudo.Metier.Noyau;

namespace AppPublication.Export
{
    public interface IReadOnlyExportContext
    {
        /// <summary>
        /// Contrat en lecture seule fourni au moteur ExportXML.
        /// Empêche ExportXML de modifier la configuration ou de relancer des processus du contexte.
        /// </summary>
        IJudoData DataContext { get; }
        IExtendedJudoData ExtendedDataContext { get; }
        CombatExportCaches Caches { get; }
    }
}