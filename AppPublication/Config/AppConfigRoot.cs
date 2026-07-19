using System;
using System.IO;
using FranceJudo.Core.Configuration.Json;
using AppPublication.Config.Publication;
using AppPublication.Config.Generation;

namespace AppPublication.Config
{
    /// <summary>
    /// Racine de configuration de l'application AppPublication.
    /// Gère le Singleton, le cycle de vie du service et la réactivité de l'arbre.
    /// </summary>
    public class AppConfigRoot : JsonConfigSection
    {
        private const string kConfigFileName = "appsettings.json";
        private static readonly System.Threading.Lock _initLock = new();
        private static JsonConfigurationService<AppConfigRoot> _service;

        /// <summary>
        /// Point d'accès Singleton. 
        /// Initialise le service et câble la réactivité au premier appel.
        /// </summary>
        public static AppConfigRoot Instance
        {
            get
            {
                if (_service == null)
                {
                    lock (_initLock)
                    {
                        if (_service == null)
                        {
                            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, kConfigFileName);

                            // 1. Création du service (le constructeur effectue le Load automatique)
                            _service = new JsonConfigurationService<AppConfigRoot>(path);

                            // 2. Câblage récursif de la réactivité (OnChanged et ObservableCollections)
                            // On passe la méthode RequestSave du service à toute la hiérarchie.
                            _service.Root.InitializeSync(() => _service.RequestSave());
                        }
                    }
                }
                return _service.Root;
            }
        }

        #region SECTIONS DE CONFIGURATION

        /// <summary>
        /// Paramètres de publication (FTP, Mini-sites, etc.)
        /// </summary>
        public PublicationConfig Publication { get; set; } = new PublicationConfig();

        /// <summary>
        /// Paramètres de génération (Écrans d'appel, Générateur Web, etc.)
        /// </summary>
        public GenerationConfig Generation { get; set; } = new GenerationConfig();

        #endregion

        /// <summary>
        /// Initialise la synchronisation réactive pour l'ensemble de l'arbre.
        /// </summary>
        /// <param name="notify">L'action de sauvegarde à déclencher sur modification.</param>
        public void InitializeSync(Action notify)
        {
            this.OnChanged = notify;

            // Propagation de la synchronisation aux sections enfants
            // C'est ici que les listes (ObservableCollections) sont abonnées
            Publication?.InitializeSync(notify);
            Generation?.InitializeSync(notify);
        }

        /// <summary>
        /// Arrête le service de configuration.
        /// Force une sauvegarde synchrone de sécurité sur le disque.
        /// </summary>
        public static void Stop()
        {
            _service?.Dispose();
        }
    }
}