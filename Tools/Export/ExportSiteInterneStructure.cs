namespace Tools.Export
{
    public class ExportSiteInterneStructure : ExportStructureBase
    {
        #region MEMBRES
        public const string kEcransAppel = "ecrans-appel";
        public const string kIdCompetitionLive = "live";
        public const string kRedirectorTag = "go";
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeyr
        /// </summary>
        /// <param name="racine"></param>
        /// <param name="idCompetition"></param>
        /// <param name="maxlen"></param>
        public ExportSiteInterneStructure(string racine) : base(racine, kIdCompetitionLive) { }

        #endregion

        #region PROPRIETES

        #endregion

        #region METHODES

        
        /// <summary>
        ///  Retourne le repertoire ecrans-appel
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public string RepertoireEcransAppel(bool relatif = false)
        {
            return GetPathRepertoire(kEcransAppel, relatif);
        }

        #endregion

        #region METHODES INTERNES
        #endregion
    }
}
