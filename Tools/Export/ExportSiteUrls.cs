using Tools.Outils;

namespace Tools.Export
{
    public class ExportSiteUrls : ExportUrlsBase
    {
        #region MEMBRES
        private const string kCourante = "courante";
        private bool _isolate = false;
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeyr
        /// </summary>
        /// <param name="racine"></param>
        /// <param name="idCompetition"></param>
        /// <param name="isoleCompet"></param>
        /// <param name="maxlen"></param>
        public ExportSiteUrls(ExportSiteStructure localStructure) : base(localStructure)
        {
            _isolate = true;
        }
        #endregion

        #region PROPRIETES

        /// <summary>
        /// Indique si les competitions sont isolees cote serveur Web
        /// </summary>
        public bool CompetitionIsolee
        {
            get
            {
                return _isolate;
            }
            set
            {
                _isolate = value;
                CalculCompetUrlPath();      // Met a jour le repertoire
            }
        }

        /// <summary>
        /// Le chemin URL de Common de la competition configuree
        /// </summary>
        public string UrlPathCommon
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteStructure.kCommon);
            }
        }

        /// <summary>
        /// Le chemin URL de l'index de la competition configuree
        /// </summary>
        public string UrlPathIndex
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(UrlPathCommon, ExportSiteStructure.kIndex);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteStructure.kImg);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteStructure.kJs);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportSiteStructure.kCss);
            }
        }

        #endregion

        #region METHODES INTERNES
        /// <summary>
        /// Effectue le calcul du path racine de la competition pour initialiser le membre interne
        /// </summary>
        protected override void CalculCompetUrlPath()
        {
            if (_parentStructure != null && _parentStructure.IsFullyConfigured)
            {
                _rootCompetUrlPath = (_isolate) ? GetUrlPath(_parentStructure.RepertoireCompetition()) : kCourante;
            }
        }
        #endregion
    }
}
