using FranceJudo.Core.Utils;
using FranceJudo.Core.Export;

namespace AppPublication.Generation
{
    /// <summary>
    /// Contrat pour un générateur qui expose une configuration de type TConfig de manière Thread-Safe.
    /// </summary>
    public interface IConfigurableGenerateur<TConfig> where TConfig : class, ICloneableObject<TConfig>
    {
        ThreadSafeConfigManager<TConfig> ExportConfigurationManager { get; }
    }
}