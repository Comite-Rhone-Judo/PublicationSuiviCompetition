using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools.Logging;

namespace Tools.Configuration
{
    /// <summary>
    /// Service centralisé qui gère la sauvegarde asynchrone des sections de configuration.
    /// Il implémente le pattern Observer pour réagir aux modifications.
    /// </summary>
    public class ConfigurationService : IDisposable
    {
        // --- SINGLETON ---
        private static ConfigurationService _instance = null;
        public static ConfigurationService Instance { 
        get
            {
                if (_instance == null)
                    throw new InvalidOperationException("ConfigurationService non initialise ! Appelez ConfigurationService.CreateInstance()");
                return _instance;
            }
        }

        // --- CONCURRENCE ET WORKER ---
        // Verrou global (lent) : Protège l'opération I/O sur disque et la liste des sections à sauvegarder.
        private readonly object _saveLock = new object();

        // Liste des sections en attente de sauvegarde (le "batch" à écrire)
        private readonly List<InternalConfigSectionBase> _sectionsToSave = new List<InternalConfigSectionBase>();

        private Task _saveWorker = null;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private const int kSaveIntervalMs = 10000; // Intervalle de vérification du worker (10 secondes)

        private ConfigurationService()
        {
            // S'abonne à l'événement statique de la classe de base
            InternalConfigSectionBase.SectionBecameDirty += HandleSectionDirty;
            StartSaveWorker();
        }

        public static ConfigurationService CreateInstance()
        {
            if (_instance != null)
                throw new InvalidOperationException("Violation du Singleton : ConfigurationService deja instancie.");

            _instance = new ConfigurationService();
            return _instance;
        }



        /// <summary>
        /// Démarre le worker de sauvegarde asynchrone s'il n'est pas déjà en cours d'exécution.
        /// </summary>
        private void StartSaveWorker()
        {
            if (_saveWorker == null || _saveWorker.IsCompleted)
            {
                _saveWorker = Task.Run(() => RunSaveWorker(_cts.Token));
            }
        }

        /// <summary>
        /// Gestionnaire d'événement (appelé par les sections). Ajoute la section à la liste d'attente.
        /// </summary>
        private void HandleSectionDirty(InternalConfigSectionBase section)
        {
            lock (_saveLock) // Protège l'accès à la liste _sectionsToSave
            {
                if (!_sectionsToSave.Contains(section))
                {
                    _sectionsToSave.Add(section);
                    LogTools.Logger.Debug($"Section marked for saving: {section.SectionName}");
                }
            }
        }

        /// <summary>
        /// Tache executée en arrière-plan qui vérifie périodiquement les sections à sauvegarder.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task RunSaveWorker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(kSaveIntervalMs, token).ConfigureAwait(false);

                    // Si la liste est non vide, déclenche la sauvegarde
                    if (!token.IsCancellationRequested && _sectionsToSave.Any())
                    {
                        CommitChangesSync();
                    }
                }
                catch (OperationCanceledException)
                {
                    break; // Arrêt normal
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur dans le worker de sauvegarde.");
                }
            }
        }

        /// <summary>
        /// Exécute la sauvegarde sur disque de toutes les sections en attente (synchrone et bloquant).
        /// </summary>
        public void CommitChangesSync()
        {
            lock (_saveLock)
            {
                // Optimisation : Accès direct à la propriété Count (très rapide)
                int count = _sectionsToSave.Count;
                if (count == 0) return;


                try
                {
                    // 0. Identifier les conteneurs de configuration uniques concernés
                    // (En général il n'y en a qu'un seul : celui géré par ConfigSectionBase)
                    var configsToSave = _sectionsToSave
                        .Select(s => s.CurrentConfiguration)
                        .Where(c => c != null)
                        .Distinct()
                        .ToList();

                    try
                    {
                        // Étape 1 : Marquer toutes les sections comme ForceSave
                        // Optimisation : Boucle for par index (plus rapide que foreach et évite l'allocateur)
                        for (int i = 0; i < count; i++)
                        {
                            _sectionsToSave[i].SectionInformation.ForceSave = true;
                        }

                        // 3. Sauvegarder les conteneurs (Cela sauvegarde toutes les sections modifiées d'un coup)
                        foreach (var config in configsToSave)
                        {
                            // Utilisation de CurrentConfiguration !
                            config.Save(ConfigurationSaveMode.Modified);
                        }

                        // Succès standard : Nettoyage normal
                        CleanupAfterSave(count);
                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        // 3. GESTION D'ERREUR : Le fichier a changé sur le disque.
                        // On passe en mode "Force Update" via une instance temporaire.
                        LogTools.Logger.Warn(ex, "Conflit de configuration detecte (fichier modifie). Tentative de sauvegarde forcee via clônage.");

                        try
                        {
                            PerformFallbackSave(_sectionsToSave);

                            // B. INVALIDATION TOTALE
                            // Puisque le fichier a changé, notre objet _sharedConfig est mort.
                            // On le tue, et on tue tous les Singletons pour forcer un rechargement frais.
                            InternalConfigSectionBase.InvalidateContext();

                            // C. Nettoyage de la liste d'attente
                            // Les objets dans _sectionsToSave sont maintenant des "zombies" (liés à l'ancienne config).
                            // On les nettoie pour ne pas réessayer de les sauver.
                            // Leurs données ont été sauvées par le Fallback.
                            _sectionsToSave.Clear();
                        }
                        catch (Exception exFallback)
                        {
                            LogTools.Logger.Error(exFallback, "Echec definitif de la sauvegarde de configuration.");
                            return; // On ne vide pas la liste pour retenter plus tard
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Logger.Error(ex, "Erreur inattendue lors de la sauvegarde.");
                        return;
                    }

                    LogTools.Logger.Debug("Configuration sauvegardee.");
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur sauvegarde configuration.");
                }
            }
        }

        /// <summary>
        /// Nettoie les états internes après une sauvegarde réussie.
        /// </summary>
        /// <param name="count"></param>
        private void CleanupAfterSave(int count)
        {
            try
            {
                // On réutilise la boucle par index pour nettoyer les états
                for (int i = 0; i < count; i++)
                {
                    var section = _sectionsToSave[i];
                    // Rafraichissement du cache statique .NET
                    ConfigurationManager.RefreshSection(section.SectionName);
                    // Reset du flag Dirty (thread-safe via son propre lock interne)
                    section.ClearDirtyFlag();
                }
                // Vidage de la liste en une seule opération O(1) ou O(n) interne optimisée
                _sectionsToSave.Clear();
                LogTools.Logger.Debug($"Configuration sauvegardee ({count} sections).");
            }
            catch (Exception ex) { LogTools.Logger.Error(ex, "Erreur nettoyage."); }
        }

        /// <summary>
        /// Méthode de secours : Ouvre une nouvelle instance du fichier, 
        /// y injecte les valeurs XML actuelles des singletons, et sauvegarde.
        /// </summary>
        private void PerformFallbackSave(List<InternalConfigSectionBase> sections)
        {
            // 1. Ouvrir une configuration temporaire (fraîchement lue du disque)
            var tempConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            foreach (var section in sections)
            {
                // 2. Créer une instance vierge du même type que notre section
                // (Utilise le constructeur privé via Activator)
                var tempSection = (InternalConfigSectionBase) Activator.CreateInstance(section.GetType(), true);

                // 3. COPIE MANUELLE DES PROPRIÉTÉS (La correction est ici)
                section.CopyValuesTo(tempSection);
                tempSection.SectionInformation.ForceSave = true;

                // 4. Ajouter à la config temporaire
                // Remplacer dans le fichier temporaire
                if (tempConfig.Sections.Get(section.SectionName) != null)
                {
                    tempConfig.Sections.Remove(section.SectionName);
                }
                tempConfig.Sections.Add(section.SectionName, tempSection);
            }

            // 5. Sauvegarder le conteneur temporaire (écrase le fichier disque)
            tempConfig.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Arrêt propre du service (à appeler à la fermeture de l'application).
        /// </summary>
        public void Dispose()
        {
            StopAndCommit();
            InternalConfigSectionBase.SectionBecameDirty -= HandleSectionDirty;
        }

        /// <summary>
        /// Arrête le worker et effectue une sauvegarde synchrone finale.
        /// </summary>
        public void StopAndCommit()
        {
            // Tente d'arrêter le worker
            _cts.Cancel();
            try { _saveWorker?.Wait(kSaveIntervalMs * 2); } catch (OperationCanceledException) { /* Attendu */ }

            // Sauvegarde synchrone finale pour garantir que tout est écrit sur disque.
            CommitChangesSync();
        }
    }
}