using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools.Outils;

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
        // 1. ACQUISITION DU VERROU GLOBAL (LENT)
        lock (_saveLock)
        {
            if (!_sectionsToSave.Any()) return;

            // Copie la liste des sections à sauvegarder pour vider la liste partagée en cas de succès
            var sectionsToProcess = new List<ConfigSectionBase>(_sectionsToSave);

            Configuration config = null;

            try
            {
                // Charge le fichier de configuration (opération d'E/S)
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Marquer toutes les sections comme ForceSave
                foreach (var section in sectionsToProcess)
                {
                    // a. Retirer l'instance (ancienne) de la collection basée sur le nom
                    if (config.Sections.Get(section.SectionName) != null)
                    {
                        config.Sections.Remove(section.SectionName);
                    }

                    // b. Ajouter l'instance (nouvelle et modifiée) de notre Singleton.
                    // L'objet 'section' passé en paramètre est la référence à l'instance Singleton modifiée.
                    section.SectionInformation.ForceSave = true;
                    config.Sections.Add(section.SectionName, section);
                }

                // 2. Opération I/O de sauvegarde sur disque
                config.Save(ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection(config.Sections.Keys.Cast<string>().FirstOrDefault() ?? "appSettings");

                // 3. SUCCÈS : Réinitialisation des drapeaux et de la liste
                foreach (var section in sectionsToProcess)
                {
                    ConfigurationManager.RefreshSection(section.SectionName);
                    section.ClearDirtyFlag(); // Utilise le verrou LOCAL _writeLock pour la cohérence
                    _sectionsToSave.Remove(section);
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Échec de la sauvegarde sur disque.");
                // Ne rien faire, le worker réessayera au prochain intervalle.
            }
        } // Le verrou global est relâché
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