using FranceJudo.Core.Configuration.Json;
using FranceJudo.Metier.Structures; // Pour EntitePublicationFFJudo et FilteredFileInfo
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppPublication.Config.Publication
{
    /// <summary>
    /// Gère les paramètres de publication globaux au format JSON.
    /// Les propriétés utilisent SetValue pour déclencher la sauvegarde automatique.
    /// </summary>
    public class GeneralParams : JsonConfigElement
    {
        #region CHAMPS PRIVÉS (VALEURS PAR DÉFAUT)

        private string _niveauPublicationFFJudo = string.Empty;
        private string _entitePublicationFFJudo = string.Empty;

        // Valeur par défaut dynamique basée sur l'environnement d'origine
        private string _repertoireRacine = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private string _logo = null;
        private string _logoDark = null;
        private bool _useLogoUnique = true;
        private bool _easyConfig = true;
        private string _urlDistant = string.Empty;
        private bool _isolerCompetition = false;
        private string _repertoireRacineSiteFTPDistant = string.Empty;
        private bool _effacerAuDemarrage = true;
        private bool _useIntituleCommun = false;
        private string _intituleCommun = string.Empty;
        #endregion

        #region PROPRIÉTÉS

        public bool UseIntituleCommun { get => _useIntituleCommun; set => SetValue(ref _useIntituleCommun, value); }
        public string IntituleCommun { get => _intituleCommun; set => SetValue(ref _intituleCommun, value); }



        public string NiveauPublicationFFJudo
        {
            get => _niveauPublicationFFJudo;
            set => SetValue(ref _niveauPublicationFFJudo, value);
        }

        public string EntitePublicationFFJudo
        {
            get => _entitePublicationFFJudo;
            set => SetValue(ref _entitePublicationFFJudo, value);
        }

        public string RepertoireRacine
        {
            get => _repertoireRacine;
            set => SetValue(ref _repertoireRacine, value);
        }

        public string Logo
        {
            get => _logo;
            set => SetValue(ref _logo, value);
        }

        public string LogoDark  
        {
            get => _logoDark;
            set => SetValue(ref _logoDark, value);
        }

        public bool UseLogoUnique
        {
            get => _useLogoUnique;
            set => SetValue(ref _useLogoUnique, value);
        }

        public bool EasyConfig
        {
            get => _easyConfig;
            set => SetValue(ref _easyConfig, value);
        }

        public string URLDistant
        {
            get => _urlDistant;
            set => SetValue(ref _urlDistant, value);
        }

        public bool IsolerCompetition
        {
            get => _isolerCompetition;
            set => SetValue(ref _isolerCompetition, value);
        }

        public string RepertoireRacineSiteFTPDistant
        {
            get => _repertoireRacineSiteFTPDistant;
            set => SetValue(ref _repertoireRacineSiteFTPDistant, value);
        }

        public bool EffacerAuDemarrage
        {
            get => _effacerAuDemarrage;
            set => SetValue(ref _effacerAuDemarrage, value);
        }

        #endregion

        #region MÉTHODES DE RECHERCHE (HELPERS)

        /// <summary>
        /// Recherche le niveau sélectionné dans la liste des candidats.
        /// </summary>
        public string GetNiveauPublicationFFJudo(IEnumerable<string> candidates, Func<string, string> valueSelector)
        {
            if (candidates == null) return string.Empty;

            var match = candidates.FirstOrDefault(c => string.Equals(valueSelector(c), NiveauPublicationFFJudo, StringComparison.OrdinalIgnoreCase));
            return match ?? candidates.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Recherche l'entité sélectionnée (générique) à partir d'une valeur initiale pour éviter les resets d'UI.
        /// </summary>
        public T GetEntitePublicationFFJudo<T>(IEnumerable<T> candidates, Func<T, string> valueSelector, string initialValue = null)
        {
            if (candidates == null) return default;

            string target = initialValue ?? EntitePublicationFFJudo;

            if (!string.IsNullOrWhiteSpace(target))
            {
                var match = candidates.FirstOrDefault(c => string.Equals(valueSelector(c), target, StringComparison.OrdinalIgnoreCase));
                if (match != null) return match;
            }

            return candidates.FirstOrDefault();
        }

        /// <summary>
        /// Recherche le logo configuré dans la liste des fichiers disponibles.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logoName">Le nom du logo à rechercher</param>
        /// <param name="candidates">La liste des fichiers disponibles</param>
        /// <param name="valueSelector">La fonction de sélection de la valeur</param>
        /// <returns></returns>
        public T GetLogo<T>(string logoName, IEnumerable<T> candidates, Func<T, string> valueSelector)
        {
            if (candidates == null) return default;

            if (!string.IsNullOrWhiteSpace(logoName))
            {
                var match = candidates.FirstOrDefault(c => string.Equals(valueSelector(c), logoName, StringComparison.OrdinalIgnoreCase));
                if (match != null) return match;
            }

            return candidates.FirstOrDefault();
        }

        #endregion
    }
}