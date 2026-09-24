using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using FranceJudo.Core.Configuration.Json;

namespace AppPublication.Config.Publication
{
    public class PublicationConfig : JsonConfigSection
    {
        public GeneralParams General { get; set; } = new GeneralParams();

        // Utilisation de ObservableCollection pour la réactivité automatique
        public ObservableCollection<SchedulerParams> Schedulers { get; set; } = new ObservableCollection<SchedulerParams>();
        public ObservableCollection<MiniSiteParams> MiniSites { get; set; } = new ObservableCollection<MiniSiteParams>();

        /// <summary>
        /// Récupère un scheduler par son ID ou le crée s'il n'existe pas.
        /// </summary>
        public SchedulerParams GetScheduler(string instanceName)
        {
            var cfg = Schedulers.FirstOrDefault(s => s.ID == instanceName);
            if (cfg == null)
            {
                cfg = new SchedulerParams { ID = instanceName };
                // L'ajout déclenchera automatiquement la sauvegarde grâce à CollectionChanged
                Schedulers.Add(cfg);
            }
            return cfg;
        }

        /// <summary>
        /// Récupère un minisite par son ID.
        /// </summary>
        public MiniSiteParams GetMiniSiteById(string id)
        {
            return MiniSites.FirstOrDefault(m => m.ID == id);
        }

        public void InitializeSync(Action notifyAction)
        {
            this.OnChanged = notifyAction;
            General.OnChanged = notifyAction;

            // Synchronisation de la liste des Schedulers
            SetupCollectionSync(Schedulers, notifyAction);

            // Synchronisation de la liste des MiniSites
            SetupCollectionSync(MiniSites, notifyAction);
        }
    }
}