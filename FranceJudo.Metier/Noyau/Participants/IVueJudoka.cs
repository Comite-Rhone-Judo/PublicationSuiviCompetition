using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.XML;
using System;

namespace FranceJudo.Metier.Noyau.Participants
{

    public interface IVueJudoka : IXMLSerializable
    {

        #region PROPERTIES

        /// <summary>
        /// ID du judoka
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Licence du judoka
        /// </summary>
        public string licence { get; set; }

        /// <summary>
        /// Nom du judoka
        /// </summary>
        public string nom { get; set; }

        /// <summary>
        /// Prénom du judoka
        /// </summary>
        public string prenom { get; set; }

        /// <summary>
        /// ID de la ceinture du judoka
        /// </summary>
        public int ceinture { get; set; }

        /// <summary>
        /// Date de naissance du judoka
        /// </summary>
        public DateTime naissance { get; set; }

        /// <summary>
        /// Date et heure de la pesée
        /// </summary>
        public DateTime datePesee { get; set; }

        /// <summary>
        /// Sexe du judoka (true = 1 = F et false = 0 = M)
        /// </summary>
        public bool sexe { get; set; }
        public EpreuveSexe sexeEnum { get; set; }

        /// <summary>
        /// Le judoka a été modifié (les informations importantes pour la licence)
        /// </summary>
        public bool modification { get; set; }

        /// <summary>
        /// Le judoka est présent à la compétition
        /// </summary>
        public bool present { get; set; }

        /// <summary>
        /// le passeport a été pésenté
        /// </summary>
        public bool passeport { get; set; }

        /// <summary>
        /// poids du judoka (g)
        /// </summary>
        public decimal poids { get; set; }

        /// <summary>
        /// Poids mesuré lors de la pesée du judoka (g)
        /// </summary>
        public decimal poidsMesure { get; set; }

        /// <summary>
        /// Id de la catégorie d'âge
        /// </summary>
        public int categorie { get; set; }

        /// <summary>
        /// Id du pays
        /// </summary>
        public int pays { get; set; }

        /// <summary>
        /// L'état du judoka :
        /// 1-Inscrit
        /// 2-Présent
        /// 3-Absent
        /// 4-Au poids
        /// 5-Hors poids
        /// </summary>
        public int etat { get; set; }
        public int modeControle { get; set; }
        public int modePesee { get; set; }

        /// <summary>
        /// Année minimum requise dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int anneeMin { get; set; }

        /// <summary>
        /// Année maximum requise dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int anneeMax { get; set; }
        /// <summary>
        /// Année de naissance du judoka
        /// </summary>
        public int annee { get; set; }

        /// <summary>
        /// Id de l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int idepreuve { get; set; }

        /// <summary>
        /// Id de la compétition à laquelle est inscrit le judoka
        /// </summary>
        public int idcompet { get; set; }

        /// <summary>
        /// Numéro de tête de série du judoka
        /// </summary>
        public int serie { get; set; }

        /// <summary>
        /// Classement du judoka à l'échelon - 1 (1-2 ou 0)
        /// </summary>
        public int serie2 { get; set; }

        /// <summary>
        /// Classement du judoka à l'épreuve
        /// </summary>
        public int classement { get; set; }

        /// <summary>
        /// Poids minimum requis dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int poidsMin { get; set; }

        /// <summary>
        /// Poids maximum requis dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int poidsMax { get; set; }

        /// <summary>
        /// Observation lors de la pesée (pas de passeport, pas de certificat médical, ...)
        /// </summary>
        public int observation { get; set; }

        /// <summary>
        /// Points restant pour le Shiai (passage de grade)
        /// </summary>        
        public int points { get; set; }

        /// <summary>
        /// ID du club du judoka
        /// </summary>
        public string club { get; set; }

        /// <summary>
        /// Nom de la catégorie d'âge du judoka
        /// </summary>
        public string nomCategorieAge { get; set; }

        /// <summary>
        /// Nom court du club du judoka
        /// </summary>
        public string clubNomCourt { get; set; }

        /// <summary>
        /// Nom du club du judoka
        /// </summary>
        public string clubNom { get; set; }

        /// <summary>
        /// Nom court du club du judoka
        /// </summary>
        public string comiteNomCourt { get; set; }

        /// <summary>
        /// Nom du club du judoka
        /// </summary>
        public string comiteNom { get; set; }

        /// <summary>
        /// Nom court du club du judoka
        /// </summary>
        public string ligueNomCourt { get; set; }

        /// <summary>
        /// Nom du club du judoka
        /// </summary>
        public string ligueNom { get; set; }


        /// <summary>
        /// Nom de la ceiture du judoka
        /// </summary>
        public string nomCeinture { get; set; }

        /// <summary>
        /// Couleur1 de la ceiture du judoka
        /// </summary>
        public string couleur1 { get; set; }

        /// <summary>
        /// Couleur2 de la ceiture du judoka
        /// </summary>
        public string couleur2 { get; set; }

        /// <summary>
        /// Libellé du sexe du judoka
        /// </summary>
        public string lib_sexe { get; set; }

        /// <summary>
        /// Libellé de l'épreuve à laquelle est incrit le judoka
        /// </summary>
        public string libepreuve { get; set; }

        /// <summary>
        /// Nom de la compétition à laquelle est incrit le judoka
        /// </summary>
        public string nom_compet { get; set; }

        /// <summary>
        /// ID du judoka dans la base de données fédérale
        /// </summary>
        public string remoteId { get; set; }

        /// <summary>
        /// Nom de ligue du judoka
        /// </summary>
        public string ligue { get; set; }
        /// <summary>
        /// ID du comite du judoka
        /// </summary>
        public string comite { get; set; }

        /// <summary>
        /// Qualifié pour l'échelon supérieur
        /// </summary>
        public int qualifie0 { get; set; }
        /// <summary>
        /// Qualifié pour l'échelon supérieur
        /// </summary>
        public int qualifie1 { get; set; }

        /// <summary>
        /// Id de l'équipe à laquelle apartient le judoka
        /// </summary>
        public int equipe { get; set; }

        /// <summary>
        /// Libelle de l'équipe à laquelle apartient le judoka
        /// </summary>
        public string lib_equipe { get; set; }
        public float poidsKg { get; set; }
        public int idepreuve_equipe { get; set; }
        public bool isPresent { get; }

        #endregion

        #region METHODES
        public bool PeuxParticiter();
        public bool EstPresent();
        #endregion
    }
}
