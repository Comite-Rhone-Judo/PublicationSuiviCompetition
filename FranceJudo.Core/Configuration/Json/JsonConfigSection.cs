using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FranceJudo.Core.Configuration.Json
{
    /// <summary>
    /// Classe de base pour les sections de configuration JSON 
    /// gérant la synchronisation réactive des collections.
    /// </summary>
    public abstract class JsonConfigSection : JsonConfigElement
    {
        /// <summary>
        /// Centralise la logique d'abonnement pour les collections de paramètres.
        /// Un ajout, une suppression ou une modification d'un élément déclenche la sauvegarde.
        /// </summary>
        protected void SetupCollectionSync<T>(ObservableCollection<T> collection, Action notifyAction) where T : JsonConfigElement
        {
            if (collection == null) return;

            // 1. Abonnement aux changements de structure (Ajout/Suppression/Clear)
            collection.CollectionChanged += (s, e) =>
            {
                // Notifie le service pour la sauvegarde immédiate de la structure
                notifyAction?.Invoke();

                // 2. Si de nouveaux objets sont injectés, on les abonne au système de notification
                if (e.NewItems != null)
                {
                    foreach (T item in e.NewItems)
                    {
                        item.OnChanged = notifyAction;
                    }
                }
            };

            // 3. Abonnement initial des objets déjà présents (chargement depuis le disque)
            foreach (var item in collection)
            {
                item.OnChanged = notifyAction;
            }
        }
    }
}