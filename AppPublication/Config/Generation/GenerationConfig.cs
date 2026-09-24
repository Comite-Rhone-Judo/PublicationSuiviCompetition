using System;
using System.Collections.ObjectModel; // Requis pour ObservableCollection
using System.Collections.Specialized; // Requis pour NotifyCollectionChangedEventArgs
using System.Linq;
using FranceJudo.Core.Configuration.Json;

namespace AppPublication.Config.Generation
{
    public class GenerationConfig : JsonConfigSection
    {
        public GenerateurSiteParams GenerateurSite { get; set; } = new GenerateurSiteParams();
        public GenerateurSiteInterneParams GenerateurSiteInterne { get; set; } = new GenerateurSiteInterneParams();

        // Remplacement de List<T> par ObservableCollection<T>
        public ObservableCollection<EcranAppelParams> Ecrans { get; set; } = new ObservableCollection<EcranAppelParams>();

        public EcranAppelParams GetEcranById(int id)
        {
            return Ecrans.FirstOrDefault(e => e.Id == id);
        }

        public void InitializeSync(Action notifyAction)
        {
            this.OnChanged = notifyAction;
            GenerateurSite.OnChanged = notifyAction;
            GenerateurSiteInterne.OnChanged = notifyAction;

            // 1. Abonnement aux changements de la liste (Ajout, Suppression, Clear)
            SetupCollectionSync(Ecrans, notifyAction);
        }
    }
}