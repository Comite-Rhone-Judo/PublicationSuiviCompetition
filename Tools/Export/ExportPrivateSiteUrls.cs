using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using Tools.Outils;

namespace Tools.Export
{
    public class ExportPrivateSiteUrls : ExportUrlsBase<ExportPrivateSiteUrls, ExportPrivateSiteStructure>
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
        public ExportPrivateSiteUrls(ExportPrivateSiteStructure localStructure) : base(localStructure)
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
                return FileAndDirectTools.PathJoin(UrlPathEcransAppel, ExportPrivateSiteStructure.kRedirectorTag);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportPrivateSiteStructure.kEcransAppel);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportPrivateSiteStructure.kImg);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportPrivateSiteStructure.kJs);
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
                return FileAndDirectTools.PathJoin(UrlPathCompetition, ExportPrivateSiteStructure.kCss);
            }
        }

        #endregion

        #region METHODES INTERNES
        #endregion
    }
}
