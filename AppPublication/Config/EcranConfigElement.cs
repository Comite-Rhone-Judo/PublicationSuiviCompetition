using System;
using System.Configuration;

namespace AppPublication.Config
{
    /// <summary>
    /// Élément de configuration représentant un écran d'appel.
    /// Hérite de ConfigElementBase pour l'intégration automatique avec le système de sauvegarde.
    /// </summary>
    public class EcranConfigElement : ConfigElementBase
    {
        // Constantes pour les noms des propriétés XML
        private const string kId = "id";
        private const string kNom = "nom";
        private const string kAdresseIp = "adresseIp";
        private const string kTapisIds = "tapisIds";

        /// <summary>
        /// Méthode héritée de ConfigElementBase.
        /// Notifie la section parente qu'une propriété a changé pour déclencher le mécanisme de sauvegarde différée.
        /// </summary>
        protected override void NotifyParentOfModification()
        {
            if (EcransConfigSection.Instance != null)
            {
                EcransConfigSection.Instance.NotifyChildModification();
            }
        }

        [ConfigurationProperty(kId, IsKey = true, IsRequired = true)]
        public int Id
        {
            get { return GetConfigValue<int>(kId, 0); }
            set { SetValueAndMarkDirty(kId, value); }
        }

        [ConfigurationProperty(kNom, DefaultValue = "Nouvel Ecran")]
        public string Nom
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