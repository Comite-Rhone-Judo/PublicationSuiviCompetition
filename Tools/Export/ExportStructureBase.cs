using Tools.Logging;
using Tools.Files;
using Tools.Outils;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Tools.Export
{
    public abstract class ExportStructureBase
    {
        #region CONSTANTES
        public const string kImg = "img";
        public const string kJs = "js";
        public const string kCss = "css";
        #endregion

        #region MEMBRES
        protected string _rootDir = string.Empty;
        protected string _rootCompetDir = string.Empty;
        protected string _idCompetition = string.Empty;
        protected bool _isFullyConfigured = false;             // Indique l'etat de configuration 
        protected bool _hasRootDir = false;
        protected int _maxLen = 30;
        protected string _targetPath = string.Empty;
        protected string _relativeToRoot = string.Empty;
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeyr
        /// </summary>
        /// <param name="racine"></param>
        /// <param name="idCompetition"></param>
        /// <param name="maxlen"></param>

        public ExportStructureBase(string racine, string idCompetition, int maxlen = 30)
        {
            _rootDir = racine;
            IdCompetition = idCompetition;  // L'assignation va automatiquement calculer si la configuration est correcte (full & root)
            _maxLen = maxlen;
        }

        #endregion

        #region PROPRIETES

        /// <summary>
        /// Path cible pour pouvoir calculer les repertoires relatifs
        /// </summary>
        public string TargetPath
        {
            get
            {
                return _targetPath;
            }
            set
            {
                if (_targetPath != value)
                {
                    _targetPath = value;

                    if (IsFullyConfigured)
                    {
                        // Supprime la racine dans le nom cible
                        string workstr = _targetPath.Replace(RepertoireCompetition(), "");

                        // Eclate la chaine sur le separateur de repertoire et supprime les chaines vides
                        List<string> pathLevels = workstr.Split(Path.DirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s)).ToList();

                        // Le nombre d'element permet de connaitre le niveau relatif de la racine par rapport au fichier = taille de la liste -1 ( le fichier final)
                        int level = pathLevels.Count() - 1;
                        _relativeToRoot = string.Empty;
                        if (level > 0)
                        {
                            for (int i = 0; i < level; i++)
                            {
                                _relativeToRoot += ".." + Path.DirectorySeparatorChar;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifiant de la competition consideree
        /// </summary>
        public string IdCompetition
        {
            get
            {
                IsConfiguredGuardRail();
                return _idCompetition;
            }
            set
            {
                _idCompetition = value;

                // Valide l'etat de la configuration
                GetConfigurationStatus();

                // Calcul le repertoire de la competition seulement si la structure est bien configuree
                if (IsFullyConfigured)
                {
                    // Calcul la racine locale de la competition
                    SetRepertoireCompetition(GetRootCompetition());
                }
            }
        }

        /// <summary>
        /// Retourne l'etat de configuration de la structure
        /// </summary>
        public bool IsFullyConfigured
        {
            get
            {
                return _isFullyConfigured;
            }
        }

        /// <summary>
        /// Retourne si la structure possede au moins un repertoire racine
        /// </summary>
        public bool HasRootDir
        {
            get
            {
                return _hasRootDir;
            }
        }

        /// <summary>
        /// La racine absolue de la structure 
        /// </summary>
        public string RepertoireRacine
        {
            get
            {
                // On peut accéder a la racine en cas de configuration partielle
                IsConfiguredGuardRail(false);
                return _rootDir;
            }
        }
        #endregion

        #region METHODES

        /// <summary>
        /// Le repertoire racine de la structure pour la competition configuree
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public string RepertoireCompetition(bool relatif = false)
        {
            IsConfiguredGuardRail();
            return (relatif) ? _relativeToRoot : _rootCompetDir;

        }

        /// <summary>
        ///  Retourne le repertoire Css
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public virtual string RepertoireCss(bool relatif = false)
        {
            return GetPathRepertoire(kCss, relatif);
        }

        /// <summary>
        ///  Retourne le repertoire Js
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public virtual string RepertoireJs(bool relatif = false)
        {
            return GetPathRepertoire(kJs, relatif);
        }

        /// <summary>
        ///  Retourne le repertoire Img
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public virtual string RepertoireImg(bool relatif = false)
        {
            return GetPathRepertoire(kImg, relatif);
        }

        /// <summary>
        /// Clone l'instance de la structure de repertoire (utile dans un contexte de multi-threading)
        /// </summary>
        /// <returns></returns>
        public ExportStructureBase Clone()
        {
            // 1. Création de la copie superficielle (Shallow Copy)
            // Le runtime crée bien une instance du type enfant réel.
            ExportStructureBase clone = (ExportStructureBase)this.MemberwiseClone();

            // 2. Appel de la méthode virtuelle pour personnaliser la copie
            this.SetupClone(clone);

            return clone;
        }

        #endregion

        #region METHODES INTERNES
        /// <summary>
        /// Calcul le path d'un repertoire de la competition
        /// </summary>
        /// <param name="repertoireName">Nom du repertoire</param>
        /// <param name="relatif">si le path doit etre relatif (True) ou non (False)</param>
        /// <returns></returns>
        protected string GetPathRepertoire(string repertoireName, bool relatif)
        {
            IsConfiguredGuardRail();
            return (relatif) ? FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, repertoireName)) : FileAndDirectTools.PathJoin(_relativeToRoot, repertoireName, true);
        }

        /// <summary>
        /// "Hook" virtuel pour permettre aux enfants d'ajouter leur logique
        /// </summary>
        /// <param name="clone"></param>
        protected virtual void SetupClone(ExportStructureBase clone) { }

        /// <summary>
        /// Verifie le nom du repertoire et assure la creation de ce dernier sur le disque (avec le nom filtre)
        /// </summary>
        /// <param name="repertoire"></param>
        /// <returns></returns>
        protected virtual string FiltreEtControleRepertoire(string repertoire)
        {
            // Filtre le nom du repertoire
            string output = OutilsTools.TraiteChaineURL(repertoire);

            // On s'assure que le repertoire existe bien
            FileAndDirectTools.CreateDirectorie(repertoire);
            return output;
        }

        /// <summary>
        /// Calcule la racune de la competition
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRootCompetition()
        {
            // Path absolu avec traitement des caracteres URL (+, etc.)
            return OutilsTools.TraiteChaineURL(Path.Combine(_rootDir, OutilsTools.TraiteChaine(OutilsTools.SubString(_idCompetition, 0, _maxLen))));
        }

        /// <summary>
        /// Check si la configuration permet l'acces a cette information
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual void IsConfiguredGuardRail(bool full = true)
        {
            if (full && !_isFullyConfigured)
            {
                LogTools.Logger.Debug("Tentative d'acces a une structure non configurez");

                throw new InvalidOperationException("La structure de repertoire n'est pas complement configuree");
            }
            if (!full && !_hasRootDir)
            {
                LogTools.Logger.Debug("Tentative d'acces a une structure sans racine");

                throw new InvalidOperationException("La structure de repertoire n'est pas configuree");
            }
        }

        /// <summary>
        /// Calcul l'etat de la configuration de la structure
        /// </summary>
        protected virtual void GetConfigurationStatus()
        {
            bool idCompetOk = !String.IsNullOrWhiteSpace(_idCompetition) && !string.IsNullOrEmpty(_idCompetition);
            bool rootDirOk = !String.IsNullOrWhiteSpace(_rootDir) && !string.IsNullOrEmpty(_rootDir);

            if (rootDirOk)
            {
                try
                {
                    _ = Path.GetFullPath(_rootDir);
                }
                catch
                {
                    rootDirOk = false;
                }
            }

            _hasRootDir = rootDirOk;
            _isFullyConfigured = idCompetOk && rootDirOk;
        }

        protected void SetRepertoireCompetition(string value)
        {
            if (!String.IsNullOrWhiteSpace(value) && _rootCompetDir != value)
            {
                _rootCompetDir = FiltreEtControleRepertoire(value);
            }
        }

        #endregion
    }
}
