using System;
using System.Configuration;
using System.Reflection;

namespace Tools.Configuration
{
    /// <summary>
    /// Classe de base GÉNÉRIQUE (pour les classes concrètes)
    /// Gère automatiquement le Singleton via l'attribut [SectionName]
    /// </summary>
    /// <typeparam name="T">La classe fille elle-même (Pattern CRTP)</typeparam>
    public abstract class ConfigSectionBase<T> : InternalConfigSectionBase where T : ConfigSectionBase<T>
    {
        // Constante pour la propriété technique de modification
        protected const string kLastModifiedTick = "lastModifiedTick";

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
                // Double-Check Locking standard
                if (_instance == null)
                {
                    lock (_singletonLock)
                    {
                        if (_instance == null)
                        {
                            var attr = typeof(T).GetCustomAttribute<SectionNameAttribute>();
                            if (attr == null) throw new InvalidOperationException($"Attribut [SectionName] manquant sur {typeof(T).Name}");

                            // Chargement standard
                            _instance = LoadSectionInstance<T>(attr.Name);

                            // --- NOYAU DE LA CORRECTION ---
                            // Une fois l'instance créée, on dit à la base : 
                            // "Si jamais on invalide le contexte, exécute ce code pour me remettre à null"
                            RegisterResetAction(() =>
                            {
                                lock (_singletonLock)
                                {
                                    _instance = null;
                                }
                            });
                            // ------------------------------
                        }
                    }
                }
                return _instance;
            }
        }

        // Implémentation automatique de la propriété abstraite requise par ConfigSectionBase
        public override string SectionName
        {
            get
            {
                var attr = typeof(T).GetCustomAttribute<SectionNameAttribute>();
                return attr?.Name;
            }
        }

        #region Mécanique de Notification Centralisée

        /// <summary>
        /// Propriété technique utilisée pour marquer la section comme "Dirty" 
        /// lorsque des enfants sont modifiés. Définie ici pour factorisation.
        /// </summary>
        [ConfigurationProperty(kLastModifiedTick, DefaultValue = 0L)]
        public long LastModifiedTick
        {
            get { return (long)this[kLastModifiedTick]; }
            set { SetValueAndMarkDirty(kLastModifiedTick, value); }
        }

        /// <summary>
        /// Méthode appelée par les enfants (via ConfigElementBase) quand ils changent.
        /// Cela met à jour le Tick, ce qui déclenche la sauvegarde via le mécanisme Dirty.
        /// </summary>
        public void NotifyChildModification()
        {
            this.LastModifiedTick = DateTime.Now.Ticks;
        }

        #endregion
    }
}
