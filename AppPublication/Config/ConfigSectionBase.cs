using AppPublication.Config;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Tools.Outils;

namespace AppPublication.Config
{

    public delegate void SectionDirtyEventHandler(ConfigSectionBase section);

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SectionNameAttribute : Attribute
    {
        public string Name { get; }
        public SectionNameAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Classe de base pour toutes les sections de configuration.
    /// Gère le drapeau 'Dirty' et l'événement de notification sans connaître le gestionnaire de sauvegarde.
    /// </summary>
    public abstract class ConfigSectionBase : ConfigurationSection
    {
        #region MEMBERS
        // L'objet Configuration unique pour toute l'application
        private static Configuration _sharedConfig;
        private static readonly object _sharedConfigLock = new object();

        // Verrou local de l'instance Singleton pour protéger l'état en mémoire et le drapeau _isDirty.
        private readonly object _writeLock = new object();

        // Drapeau d'état (volatile pour s'assurer que les threads le lisent à jour)
        protected volatile bool _isDirty = false;

        /// <summary>
        /// Événement statique déclenché lorsqu'une section devient "Dirty".
        /// C'est le mécanisme clé du découplage (Pattern Observer).
        /// </summary>
        public static event SectionDirtyEventHandler SectionBecameDirty;
        #endregion

        #region PROPERTIES
        // Propriétés de lecture pour le ConfigurationService
        public bool IsDirty
        {
            get
            {
                // La lecture peut être thread-safe sans verrou pour un bool volatile, 
                // mais l'utilisation du verrou garantit que la lecture est stable
                // si elle était utilisée dans une séquence plus complexe.
                lock (_writeLock)
                {
                    return _isDirty;
                }
            }
        }
        public abstract string SectionName { get; }

        #endregion

        #region CONSTRUCTEUR
        // Assurez-vous que l'initialisation du Singleton est gérée par la classe dérivée.
        internal ConfigSectionBase() { }
        #endregion

        #region METHODES

        internal abstract void InfrastructureSeal();

        /// <summary>
        /// Méthode "Moteur" qui récupère ou crée la section.
        /// Accessible uniquement par les classes héritières.
        /// </summary>
        protected static TModel LoadSectionInstance<TModel>(string sectionName) where TModel : ConfigSectionBase
        {
            lock (_sharedConfigLock)
            {
                if (_sharedConfig == null)
                {
                    _sharedConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }

                var section = _sharedConfig.GetSection(sectionName) as TModel;

                if (section == null)
                {
                    // Création via Activator pour supporter les constructeurs privés
                    section = (TModel)Activator.CreateInstance(typeof(TModel), true);
                    section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToApplication;

                    _sharedConfig.Sections.Add(sectionName, section);
                    _sharedConfig.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(sectionName);
                }
                return section;
            }
        }

        /// <summary>
        /// Méthode factorisée pour tous les Setters. Met à jour la valeur,
        /// et marque la section comme 'Dirty' si la valeur a changé, en notifiant le système.
        /// </summary>
        protected void SetValueAndMarkDirty<T>(string propertyName, T newValue, T defaultValue = default(T))
        {
            // 1. ACQUISITION DU VERROU LOCAL (RAPIDE)
            lock (_writeLock)
            {
                // Utilise la lecture thread-safe des propriétés ConfigurationSection (this[propertyName])
                // Note: Nous ne pouvons pas utiliser GetConfigValue ici sans avoir accès à la méthode parente.
                T currentValue = (T)this[propertyName];

                // Comparaison pour éviter des mises à jour inutiles
                if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
                {
                    // Mise à jour de la valeur en mémoire
                    this[propertyName] = newValue;

                    // Marquer comme sale et notifier si c'était la première modification
                    if (!_isDirty)
                    {
                        _isDirty = true;
                        // Déclenche l'événement pour informer le Service Worker
                        SectionBecameDirty?.Invoke(this);
                    }
                }
            }
            // Le verrou est relâché immédiatement.
        }

        /// <summary>
        /// Réinitialise le drapeau 'Dirty' après une sauvegarde réussie par le Manager.
        /// Doit acquérir le verrou local pour garantir l'atomicité et la cohérence du drapeau.
        /// </summary>
        public void ClearDirtyFlag()
        {
            // ACQUISITION DU VERROU LOCAL PAR LE THREAD WORKER (correction clé)
            lock (_writeLock)
            {
                _isDirty = false;
            }
        }

        /// <summary>
        /// Helper générique pour retourner une valeur par défaut si la clé est absente ou non convertible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected T GetConfigValue<T>(string key, T defaultValue)
        {
            try
            {
                var raw = this[key];
                if (raw == null) return defaultValue;

                // Si le type attendu est string, faire un cast direct
                if (typeof(T) == typeof(string))
                {
                    return (T)raw;
                }

                // Pour les types nullables (ex: bool?) on gère
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

        #endregion
    }


    /// <summary>
    /// Classe de base GÉNÉRIQUE (pour les classes concrètes)
    /// Gère automatiquement le Singleton via l'attribut [SectionName]
    /// </summary>
    /// <typeparam name="T">La classe fille elle-même (Pattern CRTP)</typeparam>
    public abstract class ConfigSectionBase<T> : ConfigSectionBase where T : ConfigSectionBase<T>
    {
        private static T _instance;
        private static readonly object _singletonLock = new object();

        // --- IMPLEMENTATION DU VERROU ---
        // On implémente la méthode abstraite et on la scelle.
        // Ainsi, 'PublicationConfigSection' n'a pas besoin de l'implémenter, 
        // mais une classe qui essaierait d'hériter directement de ConfigSectionBase 
        // aurait une erreur de compilation si elle ne l'implémente pas.
        internal sealed override void InfrastructureSeal() { }

        /// <summary>
        /// Accès Singleton automatisé
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_singletonLock)
                    {
                        if (_instance == null)
                        {
                            // 1. Récupération du nom de section via l'attribut
                            var attr = typeof(T).GetCustomAttribute<SectionNameAttribute>();
                            if (attr == null)
                            {
                                throw new InvalidOperationException($"La classe de configuration {typeof(T).Name} doit avoir l'attribut [SectionName].");
                            }

                            // 2. Chargement via la méthode de base
                            _instance = LoadSectionInstance<T>(attr.Name);
                        }
                    }
                }
                return _instance;
            }
        }

        // Implémentation automatique de la propriété abstraite requise par ConfigSectionBase
        public override string SectionName => typeof(T).GetCustomAttribute<SectionNameAttribute>()?.Name;
    }
}