using System;
using System.Configuration;

namespace Tools.Configuration
{
    /// <summary>
    /// Classe de base pour tous les éléments de configuration (ConfigurationElement).
    /// Factorise les méthodes GetConfigValue et SetValueAndMarkDirty.
    /// </summary>
    /// <typeparam name="TSection">Le type de la section de configuration parente (Singleton)</typeparam>
    public abstract class ConfigElementBase<TSection> : ConfigurationElement
        where TSection : ConfigSectionBase<TSection>
    {
        /// <summary>
        /// Implémentation générique de la notification.
        /// Utilise l'instance Singleton du type TSection fourni.
        /// </summary>
        protected virtual void NotifyParentOfModification()
        {
            // Appel direct au singleton de la section typée
            if (ConfigSectionBase<TSection>.Instance != null)
            {
                ConfigSectionBase<TSection>.Instance.NotifyChildModification();
            }
        }

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