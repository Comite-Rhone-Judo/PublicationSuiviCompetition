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
    public class ExportSiteUrls    {
        #region MEMBRES
        private const string kCourante = "courante";

        private ExportSiteStructure _parentStructure = null;
        private string _rootCompetUrlPath = String.Empty;
        private string _idCompetitionLast = string.Empty;
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
        public ExportSiteUrls(ExportSiteStructure localStructure)
        {
            if(localStructure == null)
            {
                throw new ArgumentNullException("Impossible d'initialiser une structure de site avec une structure de repertoire null");
            }

            _parentStructure = localStructure;
            _isolate = true;
        }
        #endregion

        #region PROPRIETES

        public bool IsConfigured
        {
            get
            {
                return (_parentStructure == null) ? false : _parentStructure.IsFullyConfigured;
            }
        }

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
        /// Le chemin URL pour la competition configuree
        /// </summary>
        public string UrlPathCompetition
        {
            get
            {
                IsConfiguredGuardRail();
                return GetCompetUrlPath();  // Calcul le path URL en fonction de l'isolation ou non
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
        /// Retourne l'URL path du repertoire Style
        /// </summary>
        public string UrlPathStyle
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
        /// Calcul le path URL a partir d'un repertoire du site (par rapport a la racine du site)
        /// </summary>
        /// <param name="repertoire"></param>
        /// <returns></returns>
        private string GetUrlPath(string repertoire)
        {
            string output = String.Empty;
            try
            {
                if (_parentStructure != null && _parentStructure.IsFullyConfigured)
                {
                    return repertoire.Replace(_parentStructure.RepertoireRacine, "").Remove(0, 1);
                }
            }
            catch
            {
                output = string.Empty;
            }

            return output;
        }

        /// <summary>
        /// Retourne l'URL racine de la competition
        /// </summary>
        /// <returns></returns>
        private string GetCompetUrlPath()
        {
            // On doit verifier si le parent n'a pas change son ID
            if (_parentStructure != null && _parentStructure.IsFullyConfigured)
            {
                if (_idCompetitionLast != _parentStructure.IdCompetition)
                {
                    _idCompetitionLast = _parentStructure.IdCompetition;
                    CalculCompetUrlPath();
                }
            }

            return _rootCompetUrlPath;
        }
        /// <summary>
        /// Effectue le calcul du path racine de la competition pour initialiser le membre interne
        /// </summary>
        private void CalculCompetUrlPath()
        {
            if (_parentStructure != null && _parentStructure.IsFullyConfigured)
            {
                _rootCompetUrlPath = (_isolate) ? GetUrlPath(_parentStructure.RepertoireCompetition) : kCourante;
            }
        }

        private void IsConfiguredGuardRail()
        {
            if (_parentStructure == null || !_parentStructure.IsFullyConfigured)
            {
                LogTools.Logger.Debug("Tentative d'acces a un ExportSiteUrl non configure");
                throw new InvalidOperationException("La structure de site n'est pas configuree");
            }
        }
        #endregion
    }
}
