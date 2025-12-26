using System;
using System.Configuration;
using Tools.Configuration;

namespace AppPublication.Config.EcransAppel
{
    /// <summary>
    /// Élément de configuration représentant un écran d'appel.
    /// Hérite de ConfigElementBase pour l'intégration automatique avec le système de sauvegarde.
    /// </summary>
    public class EcransAppelConfigElement : ConfigElementBase<EcransAppelConfigSection>
    {
        // Constantes pour les noms des propriétés XML
        private const string kId = "id";
        private const string kNom = "description";
        private const string kAdresseIp = "adresseIp";
        private const string kTapisIds = "tapisIds";
        private const string kHostname = "hostname";
        private const string kGroupement = "groupement";

        /// <summary>
        /// Méthode héritée de ConfigElementBase.
        /// Notifie la section parente qu'une propriété a changé pour déclencher le mécanisme de sauvegarde différée.
        /// </summary>
        protected override void NotifyParentOfModification()
        {
            if (EcransAppelConfigSection.Instance != null)
            {
                EcransAppelConfigSection.Instance.NotifyChildModification();
            }
        }

        [ConfigurationProperty(kHostname, DefaultValue = "")]
        public string Hostname
        {
            get { return GetConfigValue<string>(kHostname, string.Empty); }
            set { SetValueAndMarkDirty(kHostname, value); }
        }

        [ConfigurationProperty(kId, IsKey = true, IsRequired = true)]
        public int Id
        {
            get { return GetConfigValue<int>(kId, 1); }
            set { SetValueAndMarkDirty(kId, value); }
        }

        [ConfigurationProperty(kNom, DefaultValue = "Nouvel Ecran")]
        public string Description
        {
            get { return GetConfigValue<string>(kNom, "Nouvel Ecran"); }
            set { SetValueAndMarkDirty(kNom, value); }
        }

        [ConfigurationProperty(kAdresseIp, DefaultValue = "")]
        public string AdresseIp
        {
            get { return GetConfigValue<string>(kAdresseIp, string.Empty); }
            set { SetValueAndMarkDirty(kAdresseIp, value); }
        }

        [ConfigurationProperty(kGroupement, DefaultValue = 1)]
        public int Groupement
        {
            get { return GetConfigValue<int>(kGroupement, 1); }
            set { SetValueAndMarkDirty(kGroupement, value); }
        }

        /// <summary>
        /// Liste des IDs de tapis séparés par des points-virgules (ex: "1;3;4")
        /// </summary>
        [ConfigurationProperty(kTapisIds, DefaultValue = "")]
        public string TapisIds
        {
            get { return GetConfigValue<string>(kTapisIds, string.Empty); }
            set { SetValueAndMarkDirty(kTapisIds, value); }
        }
    }
}