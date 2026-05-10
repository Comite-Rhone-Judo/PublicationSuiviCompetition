using System;

namespace FranceJudo.Core.Configuration.Json
{
    /// <summary>
    /// Interface optionnelle pour les modèles de configuration 
    /// souhaitant s'auto-notifier auprès du service.
    /// </summary>
    public interface IJsonConfigModel
    {
        Action OnChanged { get; set; }
    }
}