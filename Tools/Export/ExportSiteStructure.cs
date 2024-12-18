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
using Tools.Outils;

namespace Tools.Export
{
    public class ExportSiteStructure
    {
        #region MEMBRES
        public const string kCourante = "courante";
        public const string kCommon = "common";
        public const string kImg = "img";
        public const string kJs = "js";
        public const string kStyle = "style";
        public const string kIndex = "index.html";

        private string _rootDir = string.Empty;
        private string _rootCompetDir = string.Empty;
        private string _idCompetition = string.Empty;
        private int _maxLen = 30;
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
            IdCompetition = idCompetition;
            _maxLen = maxlen;
        }
        #endregion

        #region PROPRIETES
        /// <summary>
        /// Identifiant de la competition consideree
        /// </summary>
        public string IdCompetition
        {
            get
            {
                return _idCompetition;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value) &&! string.IsNullOrEmpty(value) )
                {
                    _idCompetition = value;

                    // Calcul la racine locale de la competition
                    RepertoireCompetition = GetRootCompetition();
                }
            }
        }

        /// <summary>
        /// La racine absolue de la structure 
        /// </summary>
        public string RepertoireRacine
        {
            get
            {
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
        /// Retourne le repertoire Common de la competition configuree
        /// </summary>
        public string RepertoireCommon
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kCommon));
            }
        }

        /// <summary>
        /// Retourne le repertoire image
        /// </summary>
        public string RepertoireImg
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kImg));
            }
        }

        /// <summary>
        /// Retourne le repertoire js
        /// </summary>
        public string RepertoireJs
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kJs));
            }
        }

        /// <summary>
        /// Retourne le repertoire style
        /// </summary>
        public string RepertoireStyle
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, kStyle));
            }
        }

        #endregion

        #region METHODES
        /// <summary>
        /// Calcul le repertoire d'une epreuve
        /// </summary>
        /// <param name="idEpreuve">Id de l'epreuve</param>
        /// <param name="nomEpreuve">Nom de l'epreuve</param>
        /// <param name="relatif">True pour avoir le chemin relatif au dossier de la competition (pas de la racine)</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public string RepertoireEpreuve(string idEpreuve, string nomEpreuve, bool relatif = false)
        {
            if(string.IsNullOrWhiteSpace(idEpreuve) || string.IsNullOrWhiteSpace(nomEpreuve))
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

        private string FiltreEtControleRepertoire(string repertoire)
        {
            // Filtre le nom du repertoire
            string output = OutilsTools.TraiteChaineURL(repertoire);
            
            // On s'assure que le repertoire existe bien
            FileAndDirectTools.CreateDirectorie(repertoire);
            return output;
        }

        private string GetRootCompetition()
        {
            // Path absolu avec traitement des caracteres URL (+, etc.)
            return OutilsTools.TraiteChaineURL(Path.Combine(_rootDir, OutilsTools.TraiteChaine(OutilsTools.SubString(_idCompetition, 0, _maxLen))));
        }

        #endregion
    }
}
