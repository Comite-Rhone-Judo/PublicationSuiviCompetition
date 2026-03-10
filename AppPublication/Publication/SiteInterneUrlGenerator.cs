using System;
using Tools.Outils;
using Tools.Export;

namespace AppPublication.Publication
{
    /// <summary>
    /// Generateur d'URL pour le site interne
    /// </summary>
    public class SiteInterneUrlGenerator : UrlGeneratorBase<SiteInternePhysicalStructure>
    {
        #region CONSTRUCTEURS
        public SiteInterneUrlGenerator(SiteInternePhysicalStructure physicalStructure, string baseUriString = "http://localhost/")
            : base(physicalStructure, baseUriString)
        {
        }
        #endregion

        #region METHODES PRIVEES
        /// <summary>
        /// CAlcul l'URL de la competition
        /// </summary>
        protected override void BuildCompetitionUrl(string competitionId, Uri rootDomain, out string urlPath, out Uri baseUri)
        {
            urlPath = $"{OutilsTools.TraiteChaineURL(competitionId)}/";
            baseUri = new Uri(rootDomain, urlPath);
        }
        #endregion


        #region PROPRIETES PUBLIQUES

        // --- URLs ABSOLUES ---
        public Uri UrlEcransAppel => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireEcransAppel());

        // Le redirecteur est une simple concaténation Web (pas besoin de disque)
        // Le redirecteur est une simple concaténation Web (pas besoin de disque)
        public Uri UrlEcransAppelRedirecteur
        {
            get
            {
                // On s'assure que le dossier parent se termine par un slash Web 
                // pour que Uri ne remplace pas "ecrans-appel" par "go"
                string baseUriStr = UrlEcransAppel.AbsoluteUri;
                if (!baseUriStr.EndsWith("/")) baseUriStr += "/";

                return new Uri(new Uri(baseUriStr), SiteInternePhysicalStructure.kRedirectorTag);
            }
        }

        // --- URLs RELATIVES (Pour le XSLT) ---
        public string GetRelativeUrlEcransAppel(string targetFile)
            => GetRelativeWebPath(targetFile, PhysicalStructure.RepertoireEcransAppel());

        /// <summary>
        /// Si le redirecteur doit être appelé en relatif depuis un fichier HTML :
        /// </summary>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public string GetRelativeUrlEcransAppelRedirecteur(string targetFile)
        {
            // On calcule le chemin relatif jusqu'au dossier "ecrans-appel"
            string baseRelative = GetRelativeUrlEcransAppel(targetFile);

            // On s'assure de la présence du slash pour éviter "../../ecrans-appelgo"
            if (!baseRelative.EndsWith("/")) baseRelative += "/";

            return baseRelative + SiteInternePhysicalStructure.kRedirectorTag;
        }

        /// <summary>
        /// URL absolue vers le fichier spécifique de l'écran d'appel 
        /// (ex: http://serveur:port/macompet/ecrans-appel/ecran_01.html)
        /// </summary>
        public Uri GetUrlUnEcranAppel(int idEcran)
            => GetUrlFromPhysicalPath(PhysicalStructure.FichierEcranAppel(idEcran));

        /// <summary>
        /// URL relative vers le fichier de l'écran (ex: "../../ecrans-appel/ecran_01.html")
        /// Idéal pour les liens <a> dans le XSLT depuis un autre fichier généré.
        /// </summary>
        public string GetRelativeUrlUnEcranAppel(string targetFile, int idEcran)
            => GetRelativeWebPath(targetFile, PhysicalStructure.FichierEcranAppel(idEcran), false);

        #endregion
    }
}