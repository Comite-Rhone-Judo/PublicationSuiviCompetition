using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Tools.Outils;

namespace Tools.Configuration
{

    public delegate void SectionDirtyEventHandler(InternalConfigSectionBase section);

    /// <summary>
    /// Classe permettant de définir le nom de la section de configuration via un attribut.
    /// </summary>
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
    /// Non héritable
    /// </summary>
    public abstract class InternalConfigSectionBase : ConfigurationSection
    {
        #region MEMBERS
        // L'objet Configuration unique pour toute l'application
        private static System.Configuration.Configuration _sharedConfig;
        private static readonly object _sharedConfigLock = new object();

        // Verrou local de l'instance Singleton pour protéger l'état en mémoire et le drapeau _isDirty.
        private readonly object _writeLock = new object();

        // Drapeau d'état (volatile pour s'assurer que les threads le lisent à jour)
        protected bool _isDirty = false;

        /// <summary>
        /// Événement statique déclenché lorsqu'une section devient "Dirty".
        /// C'est le mécanisme clé du découplage (Pattern Observer).
        /// </summary>
        public static event SectionDirtyEventHandler SectionBecameDirty;

        // Registre des actions pour remettre à zéro les Singletons des classes filles
        private static readonly List<Action> _resetCacheActions = new List<Action>();

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

        /// <summary>
        /// Le nom de la section dans le fichier de configuration.
        /// </summary>
        public abstract string SectionName { get; }

        #endregion

        #region CONSTRUCTEUR
        // Assurez-vous que l'initialisation du Singleton est gérée par la classe dérivée.
        internal InternalConfigSectionBase() { }
        #endregion

        #region METHODES

        /// <summary>
        /// Verrou d'heritage pour empêcher les classes externes d'hériter directement de cette classe.
        /// </summary>
        internal abstract void InfrastructureSeal();

        /// <summary>
        /// Méthode "Moteur" qui récupère ou crée la section.
        /// Accessible uniquement par les classes héritières.
        /// </summary>
        protected static TModel LoadSectionInstance<TModel>(string sectionName) where TModel : InternalConfigSectionBase
        {
            lock (_sharedConfigLock)
            {
                // ouvre la configuration de l'application si pas déjà fait
                if (_sharedConfig == null)
                {
                    _sharedConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }

                // Recupere une section
                var section = _sharedConfig.GetSection(sectionName) as TModel;

                // Si la section n'existe pas, on la crée et l'ajoute
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
            bool notify = false;

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
                        notify = true; // On note qu'il faut notifier, mais on ne le fait pas encore
                    }
                }
            }
            // 2. Notification (Hors du verrou)
            if (notify)
            {
                // Si le Service acquiert le Verrou B ici, pas de problème car nous ne tenons plus A.
                SectionBecameDirty?.Invoke(this);
            }
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

        #region GESTION DU CONTEXTE ET INVALIDATION

        /// <summary>
        /// Permet aux classes génériques de s'enregistrer pour être notifiées lors d'un reset.
        /// </summary>
        protected static void RegisterResetAction(Action resetAction)
        {
            lock (_resetCacheActions)
            {
                _resetCacheActions.Add(resetAction);
            }
        }

        // <summary>
        /// Copie les valeurs des propriétés de cette instance vers une instance cible.
        /// Nécessaire car la collection 'Properties' est protected et inaccessible depuis le Service.
        /// </summary>
        /// <param name="target">L'instance (vierge) qui va recevoir les valeurs.</param>
        public void CopyValuesTo(InternalConfigSectionBase target)
        {
            DeepCopyRecursive(this, target);
        }


        /// <summary>
        /// Méthode récursive pour copier le contenu d'un ConfigurationElement à un autre.
        /// Utilise l'API publique ElementInformation pour éviter l'erreur CS1540 et l'erreur de type sur IsReadOnly.
        /// </summary>
        private static void DeepCopyRecursive(ConfigurationElement source, ConfigurationElement target)
        {
            // CORRECTION: Utilisation de PropertyInformation dans la boucle
            foreach (PropertyInformation sourcePropInfo in source.ElementInformation.Properties)
            {
                // Ignorer les propriétés systèmes
                if (sourcePropInfo.Name == "lockAttributes" || sourcePropInfo.Name == "lockAllAttributesExcept" ||
                    sourcePropInfo.Name == "lockElements" || sourcePropInfo.Name == "lockItem" ||
                    sourcePropInfo.Name == "xmlns")
                    continue;

                // Récupération de la propriété équivalente sur la cible via ElementInformation
                var targetPropInfo = target.ElementInformation.Properties[sourcePropInfo.Name];

                if (targetPropInfo == null) continue;

                object sourceValue = sourcePropInfo.Value;
                object targetValue = targetPropInfo.Value;

                if (sourceValue == null) continue;

                // CAS 1 : C'est une Collection (ex: MiniSites)
                // Note: ConfigurationElementCollection hérite de ConfigurationElement, donc on teste d'abord la collection
                if (sourceValue is ConfigurationElementCollection sourceColl &&
                    targetValue is ConfigurationElementCollection targetColl)
                {
                    CopyCollection(sourceColl, targetColl);
                }
                // CAS 2 : C'est un sous-élément complexe (ex: un settings imbriqué)
                else if (sourceValue is ConfigurationElement sourceElem &&
                         targetValue is ConfigurationElement targetElem)
                {
                    DeepCopyRecursive(sourceElem, targetElem);
                }
                // CAS 3 : C'est une valeur simple
                else
                {
                    // CORRECTION: Suppression du check IsReadOnly qui n'existe pas sur PropertyInformation.
                    // On tente simplement l'assignation. Si la propriété est en lecture seule ou calculée, cela échouera silencieusement.
                    try
                    {
                        targetPropInfo.Value = sourceValue;
                    }
                    catch { /* Ignorer erreurs conversion ou readonly */ }
                }
            }
        }

        /// <summary>
        /// Helper pour copier le contenu d'une collection à une autre via Réflexion.
        /// </summary>
        private static void CopyCollection(ConfigurationElementCollection source, ConfigurationElementCollection target)
        {
            // On parcourt les éléments de la collection source
            foreach (ConfigurationElement sourceItem in source)
            {
                // 1. Créer un nouvel élément dans la cible du bon type
                // (CreateNewElement est protected, donc appel via Reflection)
                var createMethod = target.GetType().GetMethod("CreateNewElement",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

                // Si non trouvé sur le type dérivé, chercher sur la classe de base
                if (createMethod == null)
                {
                    createMethod = typeof(ConfigurationElementCollection).GetMethod("CreateNewElement",
                       BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                }

                if (createMethod != null)
                {
                    // Appel de CreateNewElement()
                    var newItem = (ConfigurationElement)createMethod.Invoke(target, null);

                    // 2. Copier récursivement les propriétés de l'item source vers le nouveau
                    DeepCopyRecursive(sourceItem, newItem);

                    // 3. Ajouter le nouvel item à la collection cible via BaseAdd (protected)
                    var addMethod = target.GetType().GetMethod("BaseAdd",
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null, new Type[] { typeof(ConfigurationElement) }, null);

                    if (addMethod == null)
                    {
                        addMethod = typeof(ConfigurationElementCollection).GetMethod("BaseAdd",
                            BindingFlags.Instance | BindingFlags.NonPublic,
                            null, new Type[] { typeof(ConfigurationElement) }, null);
                    }

                    addMethod?.Invoke(target, new object[] { newItem });
                }
            }
        }

        /// <summary>
        /// Méthode radicale appelée par le Service en cas de conflit disque.
        /// 1. Abandonne l'objet Configuration actuel.
        /// 2. Force tous les Singletons à null.
        /// </summary>
        public static void InvalidateContext()
        {
            // 1. On lâche la référence vers le fichier de config (il sera rechargé au prochain appel)
            lock (_sharedConfigLock)
            {
                _sharedConfig = null;
            }

            // 2. On exécute les actions de nettoyage (mise à null des _instance static)
            lock (_resetCacheActions)
            {
                foreach (var action in _resetCacheActions)
                {
                    action?.Invoke();
                }
                // On vide la liste, les singletons se ré-enregistreront quand ils seront recréés
                _resetCacheActions.Clear();
            }

            LogTools.Logger.Info("Contexte de configuration invalidé et réinitialisé.");
        }

        // ... (Le reste de LoadSectionInstance reste inchangé, cf plus bas pour l'intégration) ...
        #endregion
    }
}