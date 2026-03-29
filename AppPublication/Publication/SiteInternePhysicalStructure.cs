using FranceJudo.Metier.Site;
using System.IO;

namespace AppPublication.Publication
{
    /// <summary>
    /// Description de la structure physique du site interne
    /// </summary>
    public class SiteInternePhysicalStructure : PhysicalStructureBase
    {
        #region Constantes
        public const string kEcransAppel = "ecrans-appel";
        public const string kIdCompetitionLive = "live";
        public const string kRedirectorTag = "go";
        // Le format string standard .NET
        public const string kEcranFormat = "ecran-{0}.html";
        // L'identifiant de repli
        public const string kEcranDefaultId = "default";
        #endregion

        #region CONSTRUCTEUR
        // Force la racine à "live"
        public SiteInternePhysicalStructure(string rootDir, int maxLen = 30)
            : base(rootDir, kIdCompetitionLive, maxLen) { }
        #endregion

        #region PROPRIETES PUBLIQUES
        /// <summary>
        /// Retourne le repertoire des Ecrans d'appel
        /// </summary>
        /// <returns></returns>
        public string RepertoireEcransAppel() => GetAndCreateDirectory(kEcransAppel);

        /// <summary>
        /// Retourne le chemin physique complet du fichier de l'écran d'appel.
        /// Utilise le cache pour des performances optimales.
        /// </summary>
        public string FichierEcranAppel(int idEcran)
        {
            // On détermine ce qui remplace le {0} : "01", "02", ou "default"
            string identifiant = (idEcran >= 0) ? $"{idEcran:00}" : kEcranDefaultId;

            // On génère le nom du fichier (ex: "ecran_01.html")
            string nomFichier = string.Format(kEcranFormat, identifiant);

            // Le nom du fichier est unique, c'est une clé de cache parfaite !
            return GetFilePath(nomFichier, () => Path.Combine(RepertoireEcransAppel(), nomFichier));
        }
        #endregion
    }
}