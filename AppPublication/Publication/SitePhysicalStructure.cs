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
        public const string kCommon = "common";
        #endregion

        #region CONSTRUCTEUR
        public SitePhysicalStructure(string rootDir, string competitionId, int maxLen = 30)
            : base(rootDir, competitionId, maxLen) { }
        #endregion

        #region PROPRIETES PUBLIQUES

        public string RepertoireEngagements() => GetAndCreateDirectory(kEngagements);
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

        /// <summary>
        /// Retourne le repertoire pour un groupe d'engagement
        /// </summary>
        /// <param name="idGroupe"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string RepertoireGroupeEngagements(string idGroupe)
        {
            if (string.IsNullOrWhiteSpace(idGroupe)) throw new ArgumentNullException(nameof(idGroupe));

            string path = Path.Combine(RepertoireEngagements(), OutilsTools.TraiteChaine(idGroupe));
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
            string folderName = OutilsTools.SubString(OutilsTools.TraiteChaine(tmp), 0, _maxLen);

            return GetAndCreateDirectory(folderName);
        }
        #endregion
    }
}