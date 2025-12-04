using System;
using System.Configuration;
using Tools.Configuration;

namespace Tools.Configuration
{
    /// <summary>
    /// Classe de base pour les collections de configuration.
    /// Gère automatiquement la notification vers la section parente lors des ajouts/suppressions.
    /// </summary>
    /// <typeparam name="TSection">Le type de la section parente (Singleton)</typeparam>
    /// <typeparam name="TElement">Le type des éléments contenus</typeparam>
    public abstract class ConfigCollectionBase<TSection, TElement> : ConfigurationElementCollection
        where TSection : ConfigSectionBase<TSection>
        where TElement : ConfigurationElement, new()
    {
        // Création standard d'un élément (factorisée)
        protected override ConfigurationElement CreateNewElement()
        {
            return new TElement();
        }

        // Méthode abstraite typée pour récupérer la clé (plus propre que l'object du framework)
        protected abstract object GetElementKey(TElement element);

        // Pont entre le framework (object) et notre méthode typée
        protected override object GetElementKey(ConfigurationElement element)
        {
            return GetElementKey((TElement)element);
        }

        #region Méthodes Publiques Factorisées (Add / Remove / Clear)

        public void Add(TElement element)
        {
            BaseAdd(element);
            NotifyParentOfModification();
        }

        public void Remove(TElement element)
        {
            // On utilise la clé pour la suppression standard
            if (element != null)
            {
                BaseRemove(GetElementKey(element));
                NotifyParentOfModification();
            }
        }

        // Surcharge pour supprimer par la clé directement (ex: int id ou string nom)
        public void Remove(object key)
        {
            BaseRemove(key);
            NotifyParentOfModification();
        }

        public void Clear()
        {
            BaseClear();
            NotifyParentOfModification();
        }

        #endregion

        #region Accesseurs Typés

        public TElement this[int index]
        {
            get { return (TElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null) BaseRemoveAt(index);
                BaseAdd(index, value);
                NotifyParentOfModification();
            }
        }

        public new TElement this[string key]
        {
            get { return (TElement)BaseGet(key); }
        }

        #endregion

        /// <summary>
        /// Déclenche la sauvegarde via le Singleton parent.
        /// </summary>
        protected void NotifyParentOfModification()
        {
            if (ConfigSectionBase<TSection>.Instance != null)
            {
                ConfigSectionBase<TSection>.Instance.NotifyChildModification();
            }
        }
    }
}