using Tools.Outils;
using System;
using System.IO;


namespace Tools.Export
{
    public class ExportSiteStructure : ExportStructureBase
    {
        #region MEMBRES
        public const string kCourante = "courante";
        public const string kEngagements = "engagements";
        public const string kCommon = "common";
        public const string kIndex = "index.html";
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="racine">Racine dans laquelle on va creer la structure "site"</param>
        /// <param name="idCompetition">ID de la competition</param>
        /// <param name="maxlen">Taille max pour les noms de répertoire</param>
        public ExportSiteStructure(string racine, string idCompetition, int maxlen = 30) : base(racine, idCompetition, maxlen) { }

        #endregion

        #region PROPRIETES
        #endregion

        #region METHODES

        /// <summary>
        ///  Retourne le repertoire Engagements
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public string RepertoireEngagements(bool relatif = false)
        {
            return GetPathRepertoire(kEngagements, relatif);
        }

        /// <summary>
        /// Retourne le repertoire Common de la competition configuree
        /// </summary>
        /// <param name="relatif">True si le path retourne est en relatif</param>
        /// <returns></returns>
        public string RepertoireCommon(bool relatif = false)
        {
            return GetPathRepertoire(kCommon, relatif);
        }

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
            string directory = Path.Combine(RepertoireEngagements(), OutilsTools.TraiteChaine(idGroupe));
            directory = FiltreEtControleRepertoire(directory);

            return (relatif) ? directory.Replace(RepertoireCompetition(), "").Remove(0, 1) : directory;
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
            string tmp = idEpreuve + "_" + nomEpreuve;

            // On calcul le path complet pour faire le controle d'existence
            string directory = Path.Combine(RepertoireCompetition(), OutilsTools.SubString(OutilsTools.TraiteChaine(tmp), 0, _maxLen));
            directory = FiltreEtControleRepertoire(directory);

            return (relatif) ? directory.Replace(RepertoireCompetition(), "").Remove(0, 1) : directory;
        }

        #endregion
    }
}
