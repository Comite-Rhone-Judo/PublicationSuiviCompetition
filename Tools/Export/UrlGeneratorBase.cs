using System;
using System.Collections.Concurrent;

namespace Tools.Export
{
    /// <summary>
    /// Calculateur d'URL pour une site
    /// </summary>
    public abstract class UrlGeneratorBase<TPhysicalStructure> where TPhysicalStructure : PhysicalStructureBase
    {
        private TPhysicalStructure _physicalStructure;
        protected Uri _rootDomainUri;
        protected Uri _competitionBaseUri;
        protected string _idCompetitionLast = string.Empty;

        private readonly object _syncLock = new object();

        // Cache pour les calculs d'URI relatifs (Utilisation d'un ValueTuple pour éviter les allocations)
        private readonly ConcurrentDictionary<(string, string), string> _relativePathsCache = new ConcurrentDictionary<(string, string), string>();
        private readonly ConcurrentDictionary<string, Uri> _absoluteUrlsCache = new ConcurrentDictionary<string, Uri>(StringComparer.OrdinalIgnoreCase);

        public string UrlPathCompetition { get; protected set; }

        #region RACCOURCIS URLS COMMUNES (CSS, JS, IMG)

        // --- URLs ABSOLUES ---
        public Uri UrlCss => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireCss());
        public Uri UrlJs => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireJs());
        public Uri UrlImg => GetUrlFromPhysicalPath(PhysicalStructure.RepertoireImg());

        // --- URLs RELATIVES (Pour le XSLT) ---
        public string GetRelativeUrlCss(string targetPhysicalFile)
            => GetRelativeWebPath(targetPhysicalFile, PhysicalStructure.RepertoireCss());

        public string GetRelativeUrlJs(string targetPhysicalFile)
            => GetRelativeWebPath(targetPhysicalFile, PhysicalStructure.RepertoireJs());

        public string GetRelativeUrlImg(string targetPhysicalFile)
            => GetRelativeWebPath(targetPhysicalFile, PhysicalStructure.RepertoireImg());

        public string GetRelativeUrlCompetition(string targetPhysicalFile)
            => GetRelativeWebPath(targetPhysicalFile, PhysicalStructure.RepertoireCompetition);

        /// <summary>
        /// Calcule un chemin web relatif depuis la racine de la compétition.
        /// Idéal pour stocker des chemins génériques dans les fichiers XML de données.
        /// </summary>
        public string GetRelativeWebPathFromCompetition(string targetPhysicalFolder)
        {
            // On force le slash final pour que System.Uri comprenne que c'est le dossier racine
            string rootDir = EnsureTrailingSeparator(PhysicalStructure.RepertoireCompetition);

            return GetRelativeWebPath(rootDir, targetPhysicalFolder);
        }

        #endregion

        public TPhysicalStructure PhysicalStructure
        {
            get => _physicalStructure;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                // PLUS DE GUARDRAIL ICI ! On accepte la structure même incomplète.
                string newId = value.IdCompetition;
                Uri currentDomain = _rootDomainUri;

                // On ne calcule les URLs que si la structure est prête.
                // Sinon on initialise à vide et on attendra.
                if (value.IsFullyConfigured)
                {
                    BuildCompetitionUrl(newId, currentDomain, out string newPath, out Uri newBase);

                    lock (_syncLock)
                    {
                        _physicalStructure = value;
                        _idCompetitionLast = newId;
                        UrlPathCompetition = newPath;
                        _competitionBaseUri = newBase;
                        _relativePathsCache.Clear();
                        _absoluteUrlsCache.Clear();
                    }
                }
                else
                {
                    lock (_syncLock)
                    {
                        _physicalStructure = value;
                        _idCompetitionLast = newId;
                        UrlPathCompetition = string.Empty;
                        _competitionBaseUri = null;
                        _relativePathsCache.Clear();
                        _absoluteUrlsCache.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Racine du domaine URL
        /// </summary>
        public string RootDomain
        {
            get => _rootDomainUri.ToString();
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = "http://localhost/";
                if (!value.EndsWith("/")) value += "/";

                Uri newUri = new Uri(value);

                var currentStruct = _physicalStructure;
                if (currentStruct != null && currentStruct.IsFullyConfigured)
                {
                    string currentId = currentStruct.IdCompetition;
                    BuildCompetitionUrl(currentId, newUri, out string newPath, out Uri newBase);

                    lock (_syncLock)
                    {
                        if (_rootDomainUri != newUri)
                        {
                            _rootDomainUri = newUri;
                            UrlPathCompetition = newPath;
                            _competitionBaseUri = newBase;
                            _absoluteUrlsCache.Clear();
                        }
                    }
                }
                else
                {
                    lock (_syncLock)
                    {
                        if (_rootDomainUri != newUri) _rootDomainUri = newUri;
                    }
                }
            }
        }

        public UrlGeneratorBase(TPhysicalStructure physicalStructure, string baseUriString = "http://localhost/")
        {
            if (string.IsNullOrWhiteSpace(baseUriString)) baseUriString = "http://localhost/";
            if (!baseUriString.EndsWith("/")) baseUriString += "/";
            _rootDomainUri = new Uri(baseUriString);

            PhysicalStructure = physicalStructure;
        }

        /// <summary>
        /// Assure la synchro des caches internes pour eviter le recalcul des URLs
        /// </summary>
        protected void EnsureCacheSynchronization()
        {
            var currentStruct = _physicalStructure;

            // LE GUARDRAIL RESTE UNIQUEMENT ICI !
            // Car c'est la porte d'entrée pour la génération d'URLs. Si on arrive ici, l'objet doit être configuré.
            currentStruct.GuardRail();
            string currentId = currentStruct.IdCompetition;

            if (_idCompetitionLast != currentId)
            {
                Uri currentDomain = _rootDomainUri;

                BuildCompetitionUrl(currentId, currentDomain, out string newPath, out Uri newBase);

                lock (_syncLock)
                {
                    if (_idCompetitionLast != currentId)
                    {
                        _idCompetitionLast = currentId;
                        UrlPathCompetition = newPath;
                        _competitionBaseUri = newBase;

                        _relativePathsCache.Clear();
                        _absoluteUrlsCache.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Callback descendant pour le calcul de l'URL de  la competition
        /// </summary>
        protected abstract void BuildCompetitionUrl(string competitionId, Uri rootDomain, out string urlPath, out Uri baseUri);

        /// <summary>
        /// Force le recalcul des URLs et invalide les caches
        /// </summary>
        protected void ForceRecalculateUrls()
        {
            var currentStruct = _physicalStructure;
            if (currentStruct == null || !currentStruct.IsFullyConfigured) return;

            string currentId = currentStruct.IdCompetition;
            Uri currentDomain = _rootDomainUri;

            BuildCompetitionUrl(currentId, currentDomain, out string newPath, out Uri newBase);

            lock (_syncLock)
            {
                UrlPathCompetition = newPath;
                _competitionBaseUri = newBase;
                _absoluteUrlsCache.Clear();
            }
        }

        /// <summary>
        /// URL de la competition
        /// </summary>
        public Uri CompetitionBaseUri
        {
            get
            {
                EnsureCacheSynchronization();
                return _competitionBaseUri;
            }
        }

        /// <summary>
        /// Extrait l'URL d'une ressource dont on connait le chemin physique
        /// </summary>
        /// <param name="physicalPath"></param>
        /// <returns></returns>
        public Uri GetUrlFromPhysicalPath(string physicalPath)
        {
            if (string.IsNullOrWhiteSpace(physicalPath)) return null;

            EnsureCacheSynchronization();

            return _absoluteUrlsCache.GetOrAdd(physicalPath, path =>
            {
                // Racine physique avec slash obligatoire pour le calcul relatif
                string rootDir = EnsureTrailingSeparator(_physicalStructure.RepertoireCompetition);
                // On garde le path tel quel(sans forcer de slash) pour que les fichiers restent des fichiers.
                Uri rootFileUri = new Uri(rootDir);
                Uri targetFileUri = new Uri(path);

                Uri relativeWebUri = rootFileUri.MakeRelativeUri(targetFileUri);

                // Fusion avec l'URL de base du site
                return new Uri(CompetitionBaseUri, relativeWebUri);
            });
        }

        /// <summary>
        /// Retourne l'URL aboslue par rapport a la racune du serveur dont on connait le chemin physique
        /// </summary>
        /// <param name="physicalPath"></param>
        /// <returns></returns>
        public string GetServerAbsolutePath(string physicalPath)
        {
            return GetUrlFromPhysicalPath(physicalPath).AbsolutePath;
        }

        /// <summary>
        /// Calcule le chemin web relatif (ex: ../../css/ ou ../../common/index.html)
        /// </summary>
        /// <param name="isTargetDirectory">True (défaut) si la cible est un dossier pour forcer le '/' final pour le XSLT.</param>
        public string GetRelativeWebPath(string sourcePhysicalFile, string targetPhysicalPath, bool isTargetDirectory = true)
        {
            if (string.IsNullOrWhiteSpace(sourcePhysicalFile) || string.IsNullOrWhiteSpace(targetPhysicalPath)) return string.Empty;

            EnsureCacheSynchronization();

            return _relativePathsCache.GetOrAdd((sourcePhysicalFile, targetPhysicalPath), key =>
            {
                Uri fromUri = new Uri(key.Item1);

                // 🚨 CORRECTION : On ne force le slash final que si c'est un dossier !
                string destPath = isTargetDirectory ? EnsureTrailingSeparator(key.Item2) : key.Item2;
                Uri toUri = new Uri(destPath);

                Uri relativeUri = fromUri.MakeRelativeUri(toUri);
                return Uri.UnescapeDataString(relativeUri.ToString());
            });
        }

        /// <summary>
        /// Assure la presence de '/' a la fin des paths et URL pour assure le bon fonctionnement des conversion via Uri
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string EnsureTrailingSeparator(string path)
        {
            if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) &&
                !path.EndsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
            {
                return path + System.IO.Path.DirectorySeparatorChar;
            }
            return path;
        }
    }
}