using FranceJudo.Metier.Site;
using System;


namespace AppPublication.Publication
{ 
    /// <summary>
    /// Generateur d'URL du site interne
    /// </summary>
    public class SiteUrlGenerator : UrlGeneratorBase<SitePhysicalStructure>
    {
        #region CONSTANTES
        public const string kCourante = "courante";
        #endregion

        #region MEMBRES
        private bool _isolate = true;
        #endregion

        #region CONSTRUCTEURS
        public SiteUrlGenerator(SitePhysicalStructure physicalStructure, string baseUriString = "http://localhost/")
            : base(physicalStructure, baseUriString)
        {
        }
        #endregion

        #region PROPRIETES PUBLIQUES

        // --- URLs ABSOLUES ---
        public Uri UrlEngagements => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireEngagements());
        public Uri UrlCommon => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireCommon());
        public Uri UrlIndex => GetUrlFromPhysicalPath(PhysicalStructure.FichierIndex());

        public Uri GetUrlGroupeEngagements(string idGroupe)
            => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireGroupeEngagements(idGroupe));

        public Uri GetUrlEpreuve(string idEpreuve, string nomEpreuve)
            => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireEpreuve(idEpreuve, nomEpreuve));

        // --- URLs RELATIVES (Pour le XSLT) ---
        public string GetRelativeUrlEngagements(string targetFile)
            => GetRelativeWebPath(targetFile, PhysicalStructure.RepertoireEngagements());

        public string GetRelativeUrlCommon(string targetFile)
            => GetRelativeWebPath(targetFile, PhysicalStructure.RepertoireCommon());

        public string GetRelativeUrlGroupeEngagements(string targetFile, string idGroupe)
            => GetRelativeWebPath(targetFile, PhysicalStructure.RepertoireGroupeEngagements(idGroupe));

        public string GetRelativeUrlEpreuve(string targetFile, string idEpreuve, string nomEpreuve)
            => GetRelativeWebPath(targetFile, PhysicalStructure.RepertoireEpreuve(idEpreuve, nomEpreuve));

        public string GetRelativeUrlEpreuveFromCompetition(string idEpreuve, string nomEpreuve)
            => GetRelativeWebPathFromCompetition(PhysicalStructure.RepertoireEpreuve(idEpreuve, nomEpreuve));

        #endregion

        /// <summary>
        /// Indique si une competition est isolee ou non cote serveur Web
        /// </summary>
        public bool CompetitionIsolee
        {
            get { return _isolate; }
            set
            {
                if (_isolate != value)
                {
                    _isolate = value;
                    // Demande à la classe mère de relancer BuildCompetitionUrl et de mettre à jour le cache
                    ForceRecalculateUrls();
                }
            }
        }

        #region METHODES PRIVEES
        protected override void BuildCompetitionUrl(string competitionId, Uri rootDomain, out string urlPath, out Uri baseUri)
        {
            urlPath = _isolate
                ? $"{OutilsTools.TraiteChaineURL(competitionId)}/"
                : $"{kCourante}/";

            baseUri = new Uri(rootDomain, urlPath);
        }
        #endregion
    }
}