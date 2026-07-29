using FranceJudo.Core.Network.Url;
using FranceJudo.Core.Utils;
using FranceJudo.Metier.Export;
using FranceJudo.Metier.Site;
using System;
using System.IO;

namespace AppPublication.Publication
{
    /// <summary>
    /// Structure physique du Site publique
    /// </summary>
    public class SitePhysicalStructure : PhysicalStructureBase
    {
        #region CONSTANTES
        public const string kEngagements = "engagements";
        public const string kStatistiques = "statistiques";
        public const string kCommon = "common";
        #endregion

        #region CONSTRUCTEUR
        public SitePhysicalStructure(string rootDir, string competitionId, int maxLen = 30)
            : base(rootDir, competitionId, maxLen) { }
        #endregion

        #region PROPRIETES PUBLIQUES

        public string RepertoireEngagements() => GetAndCreateDirectory(kEngagements);
        public string RepertoireStatistiques() => GetAndCreateDirectory(kStatistiques);
        public string RepertoireCommon() => GetAndCreateDirectory(kCommon);

        /// <summary>
        /// Source de vérité unique pour l'emplacement de la page d'accueil sur le disque.
        /// Utilise le cache pour éviter les allocations répétées.
        /// </summary>
        public string FichierIndex()
        {
            // kIndex est hérité de PhysicalStructureBase
            // La clé "index" permet de ne calculer le Path.Combine qu'une seule fois par compétition
            return GetFilePath("index", () => Path.Combine(RepertoireCommon(), kIndex));
        }

        public string FichierSePrepare() =>
            GetFilePath("se_prepare", () => Path.Combine(RepertoireCommon(), $"{SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_FeuilleCombatTapis)}.html"));

        public string FichierProchainsCombats() =>
            GetFilePath("prochains_combats", () => Path.Combine(RepertoireCommon(), $"{SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_MenuProchainCombats)}.html"));

        public string FichierAffectationTapis() =>
            GetFilePath("affectation_tapis", () => Path.Combine(RepertoireCommon(), $"{SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_AffectationTapis)}.html"));

        public string FichierAvancement() =>
            GetFilePath("avancement", () => Path.Combine(RepertoireCommon(), $"{SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_MenuAvancement)}.html"));

        public string FichierClassement() =>
            GetFilePath("classement", () => Path.Combine(RepertoireCommon(), $"{SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_MenuClassement)}.html"));

        // Utilisation stricte des énumérations de type "Menu" pour les liens transversaux
        public string FichierMenuEngagements() =>
            GetFilePath("menu_engagements", () => Path.Combine(RepertoireCommon(), $"{SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_MenuEngagements)}.html"));

        public string FichierMenuStatistiques() =>
            GetFilePath("menu_statistiques", () => Path.Combine(RepertoireCommon(), $"{SiteExportEngine.GetSanitizedFileName(ExportEnum.Site_MenuStatistiques)}.html"));

        /// <summary>
        /// Retourne le repertoire pour un groupe d'engagement
        /// </summary>
        /// <param name="idGroupe"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string RepertoireGroupeEngagements(string idGroupe)
        {
            if (string.IsNullOrWhiteSpace(idGroupe)) throw new ArgumentNullException(nameof(idGroupe));

            string path = Path.Combine(RepertoireEngagements(), idGroupe.TraiteChaine());
            return GetAndCreateDirectory(path, isAbsolute: true);
        }

        /// <summary>
        /// Retourne le repertoire pour un groupe de statistiques
        /// </summary>
        /// <param name="idGroupe"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string RepertoireGroupeStatistiques(string idGroupe)
        {
            if (string.IsNullOrWhiteSpace(idGroupe)) throw new ArgumentNullException(nameof(idGroupe));

            string path = Path.Combine(RepertoireStatistiques(), idGroupe.TraiteChaine());
            return GetAndCreateDirectory(path, isAbsolute: true);
        }



        /// <summary>
        /// Retourne le repertoire d'une epreuve
        /// </summary>
        /// <param name="idEpreuve"></param>
        /// <param name="nomEpreuve"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string RepertoireEpreuve(string idEpreuve, string nomEpreuve)
        {
            if (string.IsNullOrWhiteSpace(idEpreuve) || string.IsNullOrWhiteSpace(nomEpreuve))
                throw new ArgumentNullException("idEpreuve ou nomEpreuve est manquant.");

            string tmp = $"{idEpreuve}_{nomEpreuve}";
            string folderName = tmp.TraiteChaine().SafeSubstring(0, _maxLen);

            return GetAndCreateDirectory(folderName);
        }

        /// <summary>
        /// Retourne le chemin relatif d'un fichier par rapport à la racine de la compétition.
        /// </summary>
        /// <param name="absoluteFilePath">Le chemin absolu du fichier</param>
        /// <returns>Le chemin relatif propre (ex: "css\style.css")</returns>
        public string GetRelativePath(string absoluteFilePath)
        {
            if (string.IsNullOrWhiteSpace(absoluteFilePath))
                return string.Empty;

            // Utilisation native de .NET 10
            return Path.GetRelativePath(RepertoireCompetition, absoluteFilePath);
        }


        #endregion
    }
}