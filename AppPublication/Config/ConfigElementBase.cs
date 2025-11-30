using System;
using System.Configuration;
using Tools.Outils; // Pour Encryption

namespace AppPublication.Config
{
    /// <summary>
    /// Classe de base pour tous les éléments de configuration (ConfigurationElement).
    /// Factorise les méthodes GetConfigValue et SetValueAndMarkDirty.
    /// </summary>
    public abstract class ConfigElementBase : ConfigurationElement
    {
        /// <summary>
        /// Méthode abstraite que l'élément concret devra implémenter pour notifier 
        /// la section parente (Singleton) qu'un changement a eu lieu.
        /// </summary>
        protected abstract void NotifyParentOfModification();

        /// <summary>
        /// Helper générique pour récupérer une valeur avec gestion des types et valeur par défaut.
        /// </summary>
        protected T GetConfigValue<T>(string key, T defaultValue)
        {
            try
            {
                var raw = this[key];
                if (raw == null) return defaultValue;

                // Gestion directe des strings
                if (typeof(T) == typeof(string)) return (T)raw;

                // Gestion des types nullables
                var targetType = typeof(T);
                if (Nullable.GetUnderlyingType(targetType) != null)
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }

                return (T)Convert.ChangeType(raw, targetType);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Helper pour définir une valeur et déclencher la sauvegarde si changement.
        /// </summary>
        protected void SetValueAndMarkDirty<T>(string key, T newValue)
        {
            var currentValue = (T)this[key];

            // On ne déclenche la sauvegarde que si la valeur a réellement changé
            if (!object.Equals(currentValue, newValue))
            {
                this[key] = newValue;
                NotifyParentOfModification();
            }
        }
    }
}