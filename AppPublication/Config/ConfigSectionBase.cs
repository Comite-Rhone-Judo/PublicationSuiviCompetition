using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
 using Tools.Outils; 

public delegate void SectionDirtyEventHandler(ConfigSectionBase section);

/// <summary>
/// Classe de base pour toutes les sections de configuration.
/// Gère le drapeau 'Dirty' et l'événement de notification sans connaître le gestionnaire de sauvegarde.
/// </summary>
public abstract class ConfigSectionBase : ConfigurationSection
{
    // Verrou local de l'instance Singleton pour protéger l'état en mémoire et le drapeau _isDirty.
    private readonly object _writeLock = new object();

    // Drapeau d'état (volatile pour s'assurer que les threads le lisent à jour)
    protected volatile bool _isDirty = false;

    /// <summary>
    /// Événement statique déclenché lorsqu'une section devient "Dirty".
    /// C'est le mécanisme clé du découplage (Pattern Observer).
    /// </summary>
    public static event SectionDirtyEventHandler SectionBecameDirty;

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

    // Assurez-vous que l'initialisation du Singleton est gérée par la classe dérivée.
    protected ConfigSectionBase() { }

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
}