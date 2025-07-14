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
    public class ExportSiteStructure
    {
        #region MEMBRES
        public const string kCourante = "courante";
        public const string kEngagements = "engagements";
        public const string kCommon = "common";
        public const string kImg = "img";
        public const string kJs = "js";
        public const string kCss = "css";
        public const string kIndex = "index.html";

        private string _rootDir = string.Empty;
        private string _rootCompetDir = string.Empty;
        private string _idCompetition = string.Empty;
        private bool _isFullyConfigured = false;             // Indique l'etat de configuration 
        private bool _hasRootDir = false;
        private int _maxLen = 30;
        private string _targetPath = string.Empty;
        private string _relativeToRoot = string.Empty;
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeyr
        /// </summary>
        /// <param name="racine"></param>
        /// <param name="idCompetition"></param>
        /// <param name="maxlen"></param>
        public ExportSiteStructure(string racine, string idCompetition, int maxlen = 30)
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
                if(_targetPath != value)
                {
                    _targetPath = value;

                    if(IsFullyConfigured)
                    {
                        // Supprime la racine dans le nom cible
                        string workstr = _targetPath.Replace(RepertoireCompetition, "");

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
                    RepertoireCompetition = GetRootCompetition();
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

        /// <summary>
        /// Le repertoire racine de la structure pour la competition configuree
        /// </summary>
        public string RepertoireCompetition
        {
            get
            {
                IsConfiguredGuardRail();
                return _rootCompetDir;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value) && _rootCompetDir != value)
                {
                    _rootCompetDir = FiltreEtControleRepertoire(value);
                }
            }
        }

        /// <summary>
        /// Le repertoire Competition en relatif par rapport au path cible
        /// </summary>
        public string RepertoireCompetitionRelatif
        {
            get
            {
                IsConfiguredGuardRail();
                return _relativeToRoot;
            }
        }

        /// <summary>
        /// Retourne le repertoire Common de la competition configuree
        /// </summary>
        public string RepertoireCommon
        {
            get
            {
                IsConfiguredGuardRail();
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kCommon));
            }
        }

        /// <summary>
        /// Le repertoire Common en relatif par rapport au path cible
        /// </summary>
        public string RepertoireCommonRelatif
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(_relativeToRoot, kCommon, true);
            }
        }


        /// <summary>
        /// Retourne le repertoire Engagements
        /// </summary>
        public string RepertoireEngagements
        {
            get
            {
                IsConfiguredGuardRail();
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kEngagements));
            }
        }

        /// <summary>
        /// Le repertoire des engagemenets en relatif par rapport au path cible
        /// </summary>
        public string RepertoireEngagementsRelatif
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(_relativeToRoot, kEngagements, true);
            }
        }


        /// <summary>
        /// Retourne le repertoire image
        /// </summary>
        public string RepertoireImg
        {
            get
            {
                IsConfiguredGuardRail();
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kImg));
            }
        }

        /// <summary>
        /// Le repertoire Img relatif en relatif par rapport au path cible
        /// </summary>
        public string RepertoireImgRelatif
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(_relativeToRoot, kImg, true);
            }
        }


        /// <summary>
        /// Retourne le repertoire js
        /// </summary>
        public string RepertoireJs
        {
            get
            {
                IsConfiguredGuardRail();
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kJs));
            }
        }

        /// <summary>
        /// Le repertoire Js en relatif par rapport au path cible
        /// </summary>
        public string RepertoireJsRelatif
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(_relativeToRoot, kJs, true);
            }
        }


        /// <summary>
        /// Retourne le repertoire Css
        /// </summary>
        public string RepertoireCss
        {
            get
            {
                IsConfiguredGuardRail();
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kCss));
            }
        }

        /// <summary>
        /// Le repertoire Css en relatif par rapport au path cible
        /// </summary>
        public string RepertoireCssRelatif
        {
            get
            {
                IsConfiguredGuardRail();
                return FileAndDirectTools.PathJoin(_relativeToRoot, kCss, true);
            }
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Calcul le repertoire d'un groupe de participants
        /// </summary>
        /// <param name="idGroupe">du groupe</param>
        /// <param name="relatif">True pour avoir le chemin relatif au dossier de la competition (pas de la racine)</param>
        /// <exception cref="NullReferenceException"></exception>
        public string RepertoireGroupeEngagements(string idGroupe, bool relatif = false)
        {
            if (string.IsNullOrWhiteSpace(idGroupe))
            {
                throw new NullReferenceException();
            }

            // On calcul le path complet pour faire le controle d'existence
            string directory = Path.Combine(RepertoireEngagements, OutilsTools.TraiteChaine(idGroupe));
            directory = FiltreEtControleRepertoire(directory);

            return (relatif) ? directory.Replace(RepertoireCompetition, "").Remove(0, 1) : directory;
        }

        /// <summary>
        /// Retourne le repertoire d'une epreuve
        /// </summary>
        /// <param name="idEpreuve">Id de l'epreuve</param>
        /// <param name="nomEpreuve">Nom de l'epreuve</param>
        /// <param name="relatif">True si le repertoire est relatif a celui de la competition, False si absolu</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public string RepertoireEpreuve(string idEpreuve, string nomEpreuve, bool relatif = false)
        {
            IsConfiguredGuardRail();

            if (string.IsNullOrWhiteSpace(idEpreuve) || string.IsNullOrWhiteSpace(nomEpreuve))
            {
                throw new NullReferenceException();
            }

            // Le repertoire de l'epreuve (code)
            string tmp= idEpreuve + "_" + nomEpreuve;

            // On calcul le path complet pour faire le controle d'existence
            string directory = Path.Combine(RepertoireCompetition, OutilsTools.SubString(OutilsTools.TraiteChaine(tmp), 0, _maxLen));
            FiltreEtControleRepertoire(directory);

            return (relatif) ? directory.Replace(RepertoireCompetition, "").Remove(0, 1) : directory;
        }

        #endregion

        #region METHODES INTERNES

       /// <summary>
       /// Verifie le nom du repertoire et assure la creation de ce dernier sur le disque (avec le nom filtre)
       /// </summary>
       /// <param name="repertoire"></param>
       /// <returns></returns>
       private string FiltreEtControleRepertoire(string repertoire)
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
        private string GetRootCompetition()
        {
            // Path absolu avec traitement des caracteres URL (+, etc.)
            return OutilsTools.TraiteChaineURL(Path.Combine(_rootDir, OutilsTools.TraiteChaine(OutilsTools.SubString(_idCompetition, 0, _maxLen))));
        }

        /// <summary>
        /// Check si la configuration permet l'acces a cette information
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void IsConfiguredGuardRail(bool full = true)
        {
            if (full && !_isFullyConfigured)
            {
                LogTools.Logger.Debug("Tentative d'acces a un ExportSiteStructure non configure");

                throw new InvalidOperationException("La structure de repertoire n'est pas complement configuree");
            }
            if (!full && !_hasRootDir)
            {
                LogTools.Logger.Debug("Tentative d'acces a un ExportSiteStructure sans racine");

                throw new InvalidOperationException("La structure de repertoire n'est pas configuree");
            }
        }

        /// <summary>
        /// Calcul l'etat de la configuration de la structure
        /// </summary>
        private void GetConfigurationStatus()
        {
            bool idCompetOk = !String.IsNullOrWhiteSpace(_idCompetition) && !string.IsNullOrEmpty(_idCompetition);
            bool rootDirOk = !String.IsNullOrWhiteSpace(_rootDir) && !string.IsNullOrEmpty(_rootDir);

            if(rootDirOk)
            {
                try
                {
                    _ = Path.GetFullPath(_rootDir);
                }
                catch {
                    rootDirOk = false;
                }
            }

            _hasRootDir = rootDirOk;
            _isFullyConfigured =  idCompetOk && rootDirOk;
        }
        #endregion
    }
}
