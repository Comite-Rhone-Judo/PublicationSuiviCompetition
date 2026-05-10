using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FranceJudo.Core.Logging;

namespace FranceJudo.Core.Configuration.Json
{
    public class JsonConfigurationService<T> where T : class, new()
    {
        private readonly string _filePath;
        private readonly JsonSerializerSettings _settings;
        private readonly object _ioLock = new object();
        private CancellationTokenSource _debounceToken;

        public T Root { get; private set; }

        public JsonConfigurationService(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException(nameof(fullPath));

            _filePath = fullPath;

            // Configuration Newtonsoft : on préserve l'indentation pour la lisibilité humaine
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            Load();
        }

        /// <summary>
        /// Charge la configuration depuis le fichier JSON.
        /// </summary>
        private void Load()
        {
            lock (_ioLock)
            {
                try
                {
                    if (File.Exists(_filePath))
                    {
                        string json = File.ReadAllText(_filePath);
                        Root = JsonConvert.DeserializeObject<T>(json, _settings) ?? new T();
                    }
                    else
                    {
                        Root = new T();
                    }
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, $"Erreur lors du chargement JSON : {_filePath}");
                    Root = new T();
                }
            }
        }

        /// <summary>
        /// Demande une sauvegarde asynchrone temporisée (Debounce).
        /// </summary>
        public void RequestSave(int delayMs = 500)
        {
            lock (_ioLock)
            {
                _debounceToken?.Cancel();
                _debounceToken = new CancellationTokenSource();
            }

            var token = _debounceToken.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delayMs, token);

                    if (!token.IsCancellationRequested)
                    {
                        SaveToDisk();
                    }
                }
                catch (TaskCanceledException) { /* Annulé par une nouvelle modif */ }
            }, token);
        }

        /// <summary>
        /// Sauvegarde immédiate de la configuration sur le disque.
        /// </summary>
        public void SaveToDisk()
        {
            lock (_ioLock)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(Root, _settings);
                    File.WriteAllText(_filePath, json);
                    LogTools.Logger.Debug($"Config sauvée (Newtonsoft) : {_filePath}");
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur d'écriture de la configuration JSON.");
                }
            }
        }

        /// <summary>
        /// Termine propremet le service
        /// </summary>
        public void Dispose()
        {
            // 1. On annule le timer de sauvegarde différée s'il est en cours
            _debounceToken?.Cancel();

            // 2. On exécute une sauvegarde synchrone immédiate pour sécuriser les données
            SaveToDisk();

            _debounceToken?.Dispose();
        }
    }
}