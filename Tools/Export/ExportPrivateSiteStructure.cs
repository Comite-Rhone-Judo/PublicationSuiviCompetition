using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using Telerik.Windows.Controls;
using Tools.Outils;

namespace Tools.Export
{
    public class ExportPrivateSiteStructure : ExportStructureBase<ExportPrivateSiteStructure>
    {
        #region MEMBRES
        public const string kImg = "img";
        public const string kJs = "js";
        public const string kCss = "css";
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
        public ExportPrivateSiteStructure(string racine) : base(racine, kIdCompetitionLive) {}

        #endregion

        #region PROPRIETES

        #endregion

        #region METHODES

        /// <summary>
        ///  Retourne le repertoire Css
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public string RepertoireCss(bool relatif = false)
        {
            return GetPathRepertoire(kCss, relatif);
        }

        /// <summary>
        ///  Retourne le repertoire Js
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public string RepertoireJs(bool relatif = false)
        {
            return GetPathRepertoire(kJs, relatif);
        }

        /// <summary>
        ///  Retourne le repertoire Img
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public string RepertoireImg(bool relatif = false)
        {
            return GetPathRepertoire(kImg, relatif);
        }

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
