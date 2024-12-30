using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Tools.Outils;

namespace Tools.Export
{
    public class ExportSiteStructure
    {
        #region MEMBRES
        private string _rootDir = string.Empty;
        private string _rootCompetDir = string.Empty;
        private string _idCompetition = string.Empty;
        private const int _maxLen = 30;
        #endregion

        #region CONSTRUCTEURS
        public ExportSiteStructure(string racine, string idCompetition)
        {
            _rootDir = racine;
            IdCompetition = idCompetition;
        }

        public ExportSiteStructure(string racine)
        {
            _rootDir = racine;
        }
        #endregion

        #region PROPRIETES

        public string IdCompetition
        {
            get
            {
                return _idCompetition;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value) && _idCompetition != value)
                {
                    _idCompetition = value;
                    RepertoireCompetition = GetRootCompetition();
                }
            }
        }

        /// <summary>
        /// La racine absolue de la structure
        /// </summary>
        public string Racine
        {
            get
            {
                return _rootDir;
            }
        }

        /// <summary>
        /// La racine de la structure pour une competition specifique
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
        /// Le repertoire de la competition relatif a la racine
        /// </summary>
        public string RepertoireCompetitionRelatif
        {
            get
            {
                return GetRelativePath(RepertoireCompetition);
            }
        }

        /// <summary>
        /// Retourne le repertoire Common
        /// </summary>
        public string RepertoireCommon
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, "common"));
            }
        }

        /// <summary>
        /// Le repertoireCommon relatif a la racine
        /// </summary>
        public string RepertoireCommonRelatif
        {
            get
            {
                return GetRelativePath(RepertoireCommon);
            }
        }


        /// <summary>
        /// Retourne le repertoire Participants
        /// </summary>
        public string RepertoireParticipants
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, "participants"));
            }
        }

        /// <summary>
        /// Le repertoire des participants relatif a la racine
        /// </summary>
        public string RepertoireParticipantsRelatif
        {
            get
            {
                return GetRelativePath(RepertoireParticipants);
            }
        }


        /// <summary>
        /// Retourne le repertoire image
        /// </summary>
        public string RepertoireImg
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, "img"));
            }
        }

        /// <summary>
        /// Le repertoire Img relatif a la racine
        /// </summary>
        public string RepertoireImgRelatif
        {
            get
            {
                return GetRelativePath(RepertoireImg);
            }
        }


        /// <summary>
        /// Retourne le repertoire js
        /// </summary>
        public string RepertoireJs
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, "js"));
            }
        }

        /// <summary>
        /// Le repertoire Js relatif a la racine
        /// </summary>
        public string RepertoireJsRelatif
        {
            get
            {
                return GetRelativePath(RepertoireJs);
            }
        }


        /// <summary>
        /// Retourne le repertoire style
        /// </summary>
        public string RepertoireCss
        {
            get
            {
                return FiltreEtControleRepertoire(Path.Combine(_rootCompetDir, "css"));
            }
        }

        /// <summary>
        /// Le repertoire de la competition relatif a la racine
        /// </summary>
        public string RepertoireCssRelatif
        {
            get
            {
                return GetRelativePath(RepertoireCss);
            }
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Retourne le repertoire d'un groupe
        /// </summary>
        /// <param name="idGroupe">Id du groupe</param>
        /// <param name="relatif">True si le repertoire est relatif a celui de la competition, False si absolu</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public string RepertoireGroupeParticipants(string idGroupe, bool relatif = false)
        {
            if (string.IsNullOrWhiteSpace(idGroupe))
            {
                throw new NullReferenceException();
            }

            // On calcul le path complet pour faire le controle d'existence
            string directory = Path.Combine(RepertoireParticipants, OutilsTools.TraiteChaine(idGroupe));
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
            if(string.IsNullOrWhiteSpace(idEpreuve) || string.IsNullOrWhiteSpace(nomEpreuve))
            {
                throw new NullReferenceException();
            }

            // Le repertoire de l'epreuve (code)
            string tmp= idEpreuve + "_" + nomEpreuve;

            // On calcul le path complet pour faire le controle d'existence
            string directory = Path.Combine(RepertoireCompetition, OutilsTools.SubString(OutilsTools.TraiteChaine(tmp), 0, _maxLen));
            directory = FiltreEtControleRepertoire(directory);

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
            // Path absolu avec traitement des caracteres URL (+, etc.)
            string output = OutilsTools.TraiteChaineURL(repertoire);

            // On s'assure que le repertoire existe bien
            FileAndDirectTools.CreateDirectorie(output);

            return output;
        }

        /// <summary>
        /// Calcule la racune de la competition
        /// </summary>
        /// <returns></returns>
        private string GetRootCompetition()
        {
            return Path.Combine(_rootDir, OutilsTools.TraiteChaine(OutilsTools.SubString(_idCompetition, 0, _maxLen)));
        }

        /// <summary>
        /// Retourne le repertoire relatif par rapport a la racine du site (ajoute un / a la fin systematiquement)
        /// {Racine}/{ID Competition | courante}/{Path} ==> {ID Competition | courante}/{Path}/
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private string GetRelativePath(string fullPath)
        {
            string output = fullPath.Replace(Racine, "").Remove(0, 1);

            if (output.Last() != Path.DirectorySeparatorChar)
            {
                output += Path.DirectorySeparatorChar;
            }

            return output;
        }
        #endregion
    }
}
