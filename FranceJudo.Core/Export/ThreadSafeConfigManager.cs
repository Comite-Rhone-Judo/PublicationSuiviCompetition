using FranceJudo.Core.Utils;
using System;

namespace FranceJudo.Core.Export
{
    /// <summary>
    /// Conteneur générique Thread-Safe pour gérer n'importe quelle configuration.
    /// Garantit l'isolation entre les modifications de l'UI et la lecture par le générateur.
    /// </summary>
    public class ThreadSafeConfigManager<T> where T : class, ICloneableObject<T>
    {
        private T _configurationActive;
        private readonly object _lock = new object();

        public ThreadSafeConfigManager(T initialConfig)
        {
            _configurationActive = initialConfig ?? throw new ArgumentNullException(nameof(initialConfig));
        }

        /// <summary>
        /// Renvoie un Snapshot (Cliché figé) de la configuration actuelle.
        /// Toute modification de l'extérieur n'impactera pas cette copie.
        /// </summary>
        public T Snapshot
        {
            get
            {
                lock (_lock)
                {
                    return _configurationActive.Clone();
                }
            }
        }

        /// <summary>
        /// Permet de modifier la vraie configuration de manière sécurisée via un délégué.
        /// </summary>
        public void Modifier(Action<T> actionModification)
        {
            lock (_lock)
            {
                actionModification(_configurationActive);
            }
        }

        /// <summary>
        /// Remplace l'intégralité de la configuration active de manière sécurisée.
        /// </summary>
        public void SetConfiguration(T nouvelleConfiguration)
        {
            lock (_lock)
            {
                _configurationActive = nouvelleConfiguration.Clone();
            }
        }
    }
}