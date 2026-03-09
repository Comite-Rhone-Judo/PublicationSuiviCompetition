using System;
using System.IO;
using System.Collections.Concurrent;
using Tools.Logging;
using Tools.Outils;
using Tools.Files;

namespace Tools.Export
{
    /// <summary>
    /// Decrit la structure physique d'un site internet
    /// </summary>
    public abstract class PhysicalStructureBase
    {
        #region MEMBRES
        protected string _rootDir = string.Empty;
        protected string _rootCompetDir = string.Empty;
        protected string _idCompetition = string.Empty;
        protected bool _isFullyConfigured = false;
        protected bool _hasRootDir = false;
        protected int _maxLen = 30;

        // Cache pour éviter les accès disques multiples (très performant pour le XSLT)
        private readonly ConcurrentDictionary<string, string> _directoryCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // NOUVEAU : Cache pour les chemins de fichiers (zéro I/O disque, juste de la RAM)
        private readonly ConcurrentDictionary<string, string> _fileCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region CONSTANTES

        // Constantes communes remontées à la racine
        public const string kImg = "img";
        public const string kJs = "js";
        public const string kCss = "css";
        public const string kIndex = "index.html";

        #endregion

        #region CONSTRUCTEURS
        protected PhysicalStructureBase(string rootDir, string idCompetition, int maxLen = 30)
        {
            _rootDir = rootDir;
            _maxLen = maxLen;
            IdCompetition = idCompetition;
        }

        #endregion

        #region PROPRIETES PUBLIQUES
        /// <summary>
        /// ID de la competition
        /// </summary>
        public string IdCompetition
        {
            get
            {
                return _idCompetition ?? string.Empty;
            }
            set
            {
                    if (_idCompetition != value)
                    {
                        _idCompetition = value;
                        _directoryCache.Clear(); // Invalidation du cache si changement à la volée
                        _fileCache.Clear();      // Invalidation du cache des fichiers

                        CalculateConfigurationStatus();

                        if (_isFullyConfigured)
                    {
                        SetRepertoireCompetition(GetRootCompetition());
                    }
                    else
                    {
                        _rootCompetDir = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Racine physique du site
        /// </summary>
        public string RepertoireRacine
        {
            get
            {
                GuardRail(full: false);
                return _rootDir;
            }
        }

        /// <summary>
        /// Racine de la competition
        /// </summary>
        public string RepertoireCompetition
        {
            get
            {
                GuardRail();
                return _rootCompetDir;
            }
        }

        public bool IsFullyConfigured => _isFullyConfigured;

        // Fini l'option relatif, on renvoie de l'absolu !
        public virtual string RepertoireCss() => GetAndCreateDirectory(kCss);
        public virtual string RepertoireJs() => GetAndCreateDirectory(kJs);
        public virtual string RepertoireImg() => GetAndCreateDirectory(kImg);

        /// <summary>
        /// Efface physiquement le répertoire de la compétition et réinitialise les caches internes.
        /// </summary>
        /// <returns>True si le nettoyage a réussi.</returns>
        public bool EffacerRepertoireCompetition()
        {
            if (!_isFullyConfigured || string.IsNullOrWhiteSpace(_rootCompetDir)) return false;

            // 1. On vide la mémoire (Encapsulation : personne d'autre ne sait que ce cache existe)
            _directoryCache.Clear();
            _fileCache.Clear();

            // 2. On efface physiquement sur le disque
            bool isDeleted = FileAndDirectTools.DeleteDirectory(_rootCompetDir, true);

            // 3. On recrée immédiatement la racine vide pour être prêt pour la suite
            if (isDeleted)
            {
                FileAndDirectTools.CreateDirectorie(_rootCompetDir);
            }

            return isDeleted;
        }

        #endregion

        #region PROPRIETES PRIVEES
        /// <summary>
        /// Calcul l'etat de configuration de la structure
        /// </summary>
        private void CalculateConfigurationStatus()
        {
            bool idCompetOk = !string.IsNullOrWhiteSpace(_idCompetition);
            bool rootDirOk = !string.IsNullOrWhiteSpace(_rootDir);

            if (rootDirOk)
            {
                try { _ = Path.GetFullPath(_rootDir); }
                catch { rootDirOk = false; }
            }

            _hasRootDir = rootDirOk;
            _isFullyConfigured = idCompetOk && rootDirOk;
        }

        /// <summary>
        /// Protege les acces aux proprietes si la structure n'est pas initialisee correctement
        /// </summary>
        /// <param name="full"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void GuardRail(bool full = true)
        {
            if (full && !_isFullyConfigured)
            {
                // Sans accents pour les logs
                LogTools.Logger.Debug("Tentative d'acces a une structure physique non configuree");
                throw new InvalidOperationException("La structure physique n'est pas completement configuree.");
            }
            if (!full && !_hasRootDir)
            {
                // Sans accents pour les logs
                LogTools.Logger.Debug("Tentative d'acces a une structure sans racine");
                throw new InvalidOperationException("La structure physique n'a pas de repertoire racine configure.");
            }
        }

        /// <summary>
        /// Calcul le repertoire racine de la competition
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRootCompetition()
        {
            return Path.Combine(_rootDir, OutilsTools.TraiteChaineURL(OutilsTools.SubString(_idCompetition, 0, _maxLen)));
        }

        /// <summary>
        /// Configure le repertoire de la competition
        /// </summary>
        /// <param name="value"></param>
        protected void SetRepertoireCompetition(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && _rootCompetDir != value)
            {
                _rootCompetDir = value;
                FileAndDirectTools.CreateDirectorie(_rootCompetDir);
            }
        }

        /// <summary>
        /// Assure le controle d'existence, la mise en cache et la creation du repertoire
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="isAbsolute"></param>
        /// <returns></returns>
        protected string GetAndCreateDirectory(string folderName, bool isAbsolute = false)
        {
            GuardRail();

            string cacheKey = isAbsolute ? $"abs_{folderName}" : $"rel_{folderName}";

            return _directoryCache.GetOrAdd(cacheKey, key =>
            {
                string path = isAbsolute ? folderName : Path.Combine(_rootCompetDir, folderName);
                string sanitizedPath = OutilsTools.TraiteChaineURL(path);

                FileAndDirectTools.CreateDirectorie(sanitizedPath);
                return sanitizedPath;
            });
        }

        /// <summary>
        /// Retourne un chemin de fichier mis en cache pour éviter les Path.Combine répétitifs.
        /// </summary>
        protected string GetFilePath(string cacheKey, Func<string> pathFactory)
        {
            GuardRail(); // Sécurité standard
            return _fileCache.GetOrAdd(cacheKey, _ => pathFactory());
        }
        #endregion
    }
}