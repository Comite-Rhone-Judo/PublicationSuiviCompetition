using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

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

        /// <summary>
        /// Retourne l'élément de la collection <c>candidates</c> dont la représentation string
        /// (fourni par <c>valueSelector</c>) correspond à la valeur stockée pour <c>key</c>.
        /// Si aucune correspondance, renvoie le premier élément de la collection (ou default si vide).
        /// Encapsule la logique "valeur présente dans la liste => la retourner, sinon => première valeur".
        /// </summary>
        /// <typeparam name="T">Type des éléments de la collection.</typeparam>
        /// <param name="key">Clé de configuration (attribut dans la section).</param>
        /// <param name="candidates">Collection des éléments valides.</param>
        /// <param name="valueSelector">Fonction qui extrait la représentation string d'un élément (ex: f => f.Name).</param>
        /// <returns>L'élément trouvé ou le premier élément de la collection.</returns>
        protected T GetItemFromList<T>(string key, IEnumerable<T> candidates, Func<T, string> valueSelector)
        {
            if (candidates == null) return default(T);
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

            string stored = GetConfigValue<string>(key, null);
            return FindItemFromList(candidates, valueSelector, stored);
        }

        /// <summary>
        /// Recherche dans la collection <c>candidates</c> l'élément dont la représentation string (via <c>valueSelector</c>)
        /// correspond à <c>stored</c>. Si aucune correspondance, retourne le premier élément de la collection (ou default si vide).
        /// Méthode extraite pour séparer la logique de recherche de l'accès à la configuration.
        /// </summary>
        /// <typeparam name="T">Type des éléments.</typeparam>
        /// <param name="candidates">Collection candidate.</param>
        /// <param name="valueSelector">Sélecteur de valeur string pour chaque élément.</param>
        /// <param name="stored">Valeur à rechercher (peut être null ou vide).</param>
        /// <returns>Élément correspondant ou premier élément / default.</returns>
        protected T FindItemFromList<T>(IEnumerable<T> candidates, Func<T, string> valueSelector, string stored)
        {
            if (candidates == null) return default(T);
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

            if (!string.IsNullOrWhiteSpace(stored))
            {
                var match = candidates.FirstOrDefault(c => string.Equals(valueSelector(c) ?? string.Empty, stored, StringComparison.OrdinalIgnoreCase));
                if (match != null) return match;
            }

            // fallback : première valeur ou default
            return candidates.FirstOrDefault();
        }
    }
}