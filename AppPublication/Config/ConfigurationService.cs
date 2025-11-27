using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools.Outils;

namespace AppPublication.Config
{
    /// <summary>
    /// Service centralisé qui gère la sauvegarde asynchrone des sections de configuration.
    /// Il implémente le pattern Observer pour réagir aux modifications.
    /// </summary>
    public class ConfigurationService : IDisposable
    {
        // --- SINGLETON ---
        private static readonly Lazy<ConfigurationService> _lazyInstance =
            new Lazy<ConfigurationService>(() => new ConfigurationService());
        public static ConfigurationService Instance => _lazyInstance.Value;

        // --- CONCURRENCE ET WORKER ---
        // Verrou global (lent) : Protège l'opération I/O sur disque et la liste des sections à sauvegarder.
        private readonly object _saveLock = new object();

        // Liste des sections en attente de sauvegarde (le "batch" à écrire)
        private readonly List<ConfigSectionBase> _sectionsToSave = new List<ConfigSectionBase>();

        private Task _saveWorker = null;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private const int kSaveIntervalMs = 10000; // Intervalle de vérification du worker (10 secondes)

        private ConfigurationService()
        {
            // S'abonne à l'événement statique de la classe de base
            ConfigSectionBase.SectionBecameDirty += HandleSectionDirty;
            StartSaveWorker();
        }

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
        private void HandleSectionDirty(ConfigSectionBase section)
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
                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        // 3. GESTION D'ERREUR : Le fichier a changé sur le disque.
                        // On passe en mode "Force Update" via une instance temporaire.
                        LogTools.Logger.Warn(ex, "Conflit de configuration détecté (fichier modifié). Tentative de sauvegarde forcée via clônage.");

                        try
                        {
                            PerformFallbackSave(_sectionsToSave);
                        }
                        catch (Exception exFallback)
                        {
                            LogTools.Logger.Error(exFallback, "Echec définitif de la sauvegarde de configuration.");
                            return; // On ne vide pas la liste pour retenter plus tard
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Logger.Error(ex, "Erreur inattendue lors de la sauvegarde.");
                        return;
                    }

                // Étape 4 : Nettoyage et Notification
                // On réutilise la boucle par index pour nettoyer les états
                for (int i = 0; i < count; i++)
                    {
                        var section = _sectionsToSave[i];

                        // Rafraichissement du cache statique .NET
                        ConfigurationManager.RefreshSection(section.SectionName);

                        // Reset du flag Dirty (thread-safe via son propre lock interne)
                        section.ClearDirtyFlag();
                    }

                    // Étape 5 : Vidage de la liste en une seule opération O(1) ou O(n) interne optimisée
                    _sectionsToSave.Clear();

                    LogTools.Logger.Debug("Configuration sauvegardée.");
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur sauvegarde configuration.");
                }
            }
        }

        /// <summary>
        /// Méthode de secours : Ouvre une nouvelle instance du fichier, 
        /// y injecte les valeurs XML actuelles des singletons, et sauvegarde.
        /// </summary>
        private void PerformFallbackSave(List<ConfigSectionBase> sections)
        {
            // 1. Ouvrir une configuration temporaire (fraîchement lue du disque)
            var tempConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            foreach (var section in sections)
            {
                // 2. Supprimer l'ancienne version dans ce conteneur temporaire
                if (tempConfig.Sections.Get(section.SectionName) != null)
                {
                    tempConfig.Sections.Remove(section.SectionName);
                }

                // 3. Cloner l'état du Singleton via XML
                // C'est la seule façon de transférer les données sans détacher le Singleton de sa config d'origine
                string rawXml = section.SectionInformation.GetRawXml();

                // Créer une coquille vide du bon type
                var tempSection = (ConfigurationSection)Activator.CreateInstance(section.GetType(), true);

                // Appliquer les données
                tempSection.SectionInformation.SetRawXml(rawXml);
                tempSection.SectionInformation.ForceSave = true;

                // 4. Ajouter à la config temporaire
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
            ConfigSectionBase.SectionBecameDirty -= HandleSectionDirty;
        }

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