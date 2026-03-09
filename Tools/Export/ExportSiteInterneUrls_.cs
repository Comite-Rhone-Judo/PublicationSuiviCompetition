using Tools.Files;
using Tools.Logging;

namespace Tools.Export
{
    public class ExportSiteInterneUrls_ : ExportUrlsBase
    {
        #region MEMBRES
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeyr
        /// </summary>
        /// <param name="racine"></param>
        /// <param name="idCompetition"></param>
        /// <param name="isoleCompet"></param>
        /// <param name="maxlen"></param>
        public ExportSiteInterneUrls_(ExportSiteInterneStructure_ localStructure) : base(localStructure)
        {
        }
        #endregion

        #region PROPRIETES

        /// <summary>
        /// Le chemin URL du redirecteur des écrans d'appel
        /// </summary>
        public string UrlPathEcransAppelRedirecteur
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathEcransAppel, ExportSiteInterneStructure_.kRedirectorTag);
            }
        }

        /// <summary>
        /// Le chemin URL des ecrans d'appel pour les groupes de tapis
        /// </summary>
        public string UrlPathEcransAppel
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure_.kEcransAppel);
            }
        }

        /// <summary>
        /// Retourne l'URL path du repertoire image
        /// </summary>
        public string UrlPathImg
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure_.kImg);
            }
        }

        /// <summary>
        /// Retourne l'URL path du repertoire Js
        /// </summary>
        public string UrlPathJs
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure_.kJs);
            }
        }

        /// <summary>
        /// Retourne l'URL path du repertoire Css
        /// </summary>
        public string UrlPathCss
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure_.kCss);
            }
        }

        #endregion

        #region METHODES INTERNES
        #endregion
    }
}
