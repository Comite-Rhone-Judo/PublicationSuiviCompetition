using Tools.Files;
using Tools.Logging;

namespace Tools.Export
{
    public class ExportSiteInterneUrls : ExportUrlsBase
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
        public ExportSiteInterneUrls(ExportSiteInterneStructure localStructure) : base(localStructure)
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
                return FileAndDirectTools.PathJoin(UrlPathEcransAppel, ExportSiteInterneStructure.kRedirectorTag);
            }
        }

        /// <summary>
        /// Le chemin URL de Common de la competition configuree
        /// </summary>
        public string UrlPathEcransAppel
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure.kEcransAppel);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure.kImg);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure.kJs);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteInterneStructure.kCss);
            }
        }

        #endregion

        #region METHODES INTERNES
        #endregion
    }
}
