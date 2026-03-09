using Tools.Logging;
using System;

namespace Tools.Export
{
    public abstract class ExportUrlsBase
    {
        // TODO il va falloir reprendre ExportURl afin de retourner des vrais URLs (avec des / et pas des \) mais attention aux effets de bord
        #region MEMBRES

        protected ExportStructureBase_ _parentStructure = null;
        protected string _rootCompetUrlPath = String.Empty;
        protected string _idCompetitionLast = String.Empty;
        protected Uri _fakeBaseUri = new Uri("http://localhost/");
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeyr
        /// </summary>
        /// <param name="racine"></param>
        /// <param name="idCompetition"></param>
        /// <param name="isoleCompet"></param>
        /// <param name="maxlen"></param>
        public ExportUrlsBase(ExportStructureBase_ localStructure)
        {
            if (localStructure == null)
            {
                throw new ArgumentNullException("Impossible d'initialiser une structure de site avec une structure de repertoire null");
            }

            _parentStructure = localStructure;
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
        /// Le chemin URL pour la competition configuree
        /// </summary>
        public string UrlPathCompetition
        {
            get
            {
                IsConfiguredGuardRail();
                return GetCompetUrlPath();  // Calcul le path URL
            }
        }

        #endregion

        #region METHODES INTERNES

        /// <summary>
        /// Calcul le path URL a partir d'un repertoire du site (par rapport a la racine du site)
        /// </summary>
        /// <param name="repertoire"></param>
        /// <returns></returns>
        protected virtual string GetUrlPath(string repertoire)
        {
            // TODO il va falloir reprendre ExportURl afin de retourner des vrais URLs (avec des / et pas des \) mais attention aux effets de bord
            string output = String.Empty;
            try
            {
                if (_parentStructure != null && _parentStructure.IsFullyConfigured)
                {
                    string tmpPath = repertoire.Replace(_parentStructure.RepertoireRacine, "").Remove(0, 1);
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
        protected virtual string GetCompetUrlPath()
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
        protected virtual void CalculCompetUrlPath()
        {
            if (_parentStructure != null && _parentStructure.IsFullyConfigured)
            {
                _rootCompetUrlPath = GetUrlPath(_parentStructure.RepertoireCompetition());
            }
        }

        /// <summary>
        /// Verifie si la structure de site est completement configuree avant d'acceder a une URL, sinon leve une exception
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual void IsConfiguredGuardRail()
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
