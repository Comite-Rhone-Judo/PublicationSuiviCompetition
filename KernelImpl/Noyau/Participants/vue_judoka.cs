using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Structures;

using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;

namespace KernelImpl.Noyau.Participants
{

    public class vue_judoka : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region PROPERTIES

        private int _id;

        /// <summary>
        /// ID du judoka
        /// </summary>
        public int id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("id");
                }
            }
        }

        private string _licence;

        /// <summary>
        /// Licence du judoka
        /// </summary>
        public string licence
        {
            get { return _licence; }
            set
            {
                if (_licence != value)
                {
                    _licence = value;
                    OnPropertyChanged("licence");
                }
            }
        }

        private string _nom;

        /// <summary>
        /// Nom du judoka
        /// </summary>
        public string nom
        {
            get { return _nom; }
            set
            {
                if (_nom != value)
                {
                    _nom = value;
                    OnPropertyChanged("nom");
                }
            }
        }

        private string _prenom;

        /// <summary>
        /// Prénom du judoka
        /// </summary>
        public string prenom
        {
            get { return _prenom; }
            set
            {
                if (_prenom != value)
                {
                    _prenom = value;
                    OnPropertyChanged("prenom");
                }
            }
        }



        private int _ceinture;

        /// <summary>
        /// ID de la ceinture du judoka
        /// </summary>
        public int ceinture
        {
            get { return _ceinture; }
            set
            {
                if (_ceinture != value)
                {
                    _ceinture = value;
                    OnPropertyChanged("ceinture");
                }
            }
        }

        private DateTime _naissance;

        /// <summary>
        /// Date de naissance du judoka
        /// </summary>
        public DateTime naissance
        {
            get { return _naissance; }
            set
            {
                if (_naissance != value)
                {
                    _naissance = value;
                    OnPropertyChanged("naissance");
                }
            }
        }

        private DateTime _datePesee;

        /// <summary>
        /// Date et heure de la pesée
        /// </summary>
        public DateTime datePesee
        {
            get { return _datePesee; }
            set
            {
                if (_datePesee != value)
                {
                    _datePesee = value;
                    OnPropertyChanged("datePesee");
                }
            }
        }

        private bool _sexe;

        /// <summary>
        /// Sexe du judoka (true = 1 = F et false = 0 = M)
        /// </summary>
        public bool sexe
        {
            get { return _sexe; }
            set
            {
                if (_sexe != value)
                {
                    _sexe = value;
                    OnPropertyChanged("sexe");
                    sexeEnum = new EpreuveSexe(_sexe);
                }
            }
        }

        private EpreuveSexe _sexeEnum;
        public EpreuveSexe sexeEnum
        {
            get
            {
                return _sexeEnum;
            }
            set
            {
                if (_sexeEnum.Enum != value.Enum)
                {
                    _sexeEnum = value;
                    OnPropertyChanged("sexeEnum");
                    sexe = (bool)_sexeEnum;
                }
            }
        }

        private bool _modification;

        /// <summary>
        /// Le judoka a été modifié (les informations importantes pour la licence)
        /// </summary>
        public bool modification
        {
            get { return _modification; }
            set
            {
                if (_modification != value)
                {
                    _modification = value;
                    OnPropertyChanged("modification");
                }
            }
        }

        private bool _present;

        /// <summary>
        /// Le judoka est présent à la compétition
        /// </summary>
        public bool present
        {
            get { return _present; }
            set
            {
                if (_present != value)
                {
                    _present = value;
                    OnPropertyChanged("present");
                }
            }
        }

        private bool _passeport;

        /// <summary>
        /// le passeport a été pésenté
        /// </summary>
        public bool passeport
        {
            get { return _passeport; }
            set
            {
                if (_passeport != value)
                {
                    _passeport = value;
                    OnPropertyChanged("passeport");
                }
            }
        }

        private decimal _poids;

        /// <summary>
        /// poids du judoka (g)
        /// </summary>
        public decimal poids
        {
            get { return _poids; }
            set
            {
                if (_poids != value)
                {
                    _poids = value;
                    OnPropertyChanged("poids");
                }
            }
        }

        private decimal _poidsMesure;

        /// <summary>
        /// Poids mesuré lors de la pesée du judoka (g)
        /// </summary>
        public decimal poidsMesure
        {
            get { return _poidsMesure; }
            set
            {
                if (_poidsMesure != value)
                {
                    _poidsMesure = value;
                    OnPropertyChanged("poidsMesure");
                }
            }
        }


        private int _categorie;

        /// <summary>
        /// Id de la catégorie d'âge
        /// </summary>
        public int categorie
        {
            get { return _categorie; }
            set
            {
                if (_categorie != value)
                {
                    _categorie = value;
                    OnPropertyChanged("categorie");
                }
            }
        }

        private int _pays;

        /// <summary>
        /// Id du pays
        /// </summary>
        public int pays
        {
            get { return _pays; }
            set
            {
                if (_pays != value)
                {
                    _pays = value;
                    OnPropertyChanged("pays");
                }
            }
        }

        private int _etat;

        /// <summary>
        /// L'état du judoka :
        /// 1-Inscrit
        /// 2-Présent
        /// 3-Absent
        /// 4-Au poids
        /// 5-Hors poids
        /// </summary>
        public int etat
        {
            get { return _etat; }
            set
            {
                if (_etat != value)
                {
                    _etat = value;
                    OnPropertyChanged("etat");
                }
            }
        }

        private int _modeControle;
        public int modeControle
        {
            get { return _modeControle; }
            set
            {
                if (_modeControle != value)
                {
                    _modeControle = value;
                    OnPropertyChanged("modeControle");
                }
            }
        }

        private int _modePesee;
        public int modePesee
        {
            get { return _modePesee; }
            set
            {
                if (_modePesee != value)
                {
                    _modePesee = value;
                    OnPropertyChanged("modePesee");
                }
            }
        }

        private int _anneeMin;

        /// <summary>
        /// Année minimum requise dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int anneeMin
        {
            get { return _anneeMin; }
            set
            {
                if (_anneeMin != value)
                {
                    _anneeMin = value;
                    OnPropertyChanged("anneeMin");
                }
            }
        }

        private int _anneeMax;

        /// <summary>
        /// Année maximum requise dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int anneeMax
        {
            get { return _anneeMax; }
            set
            {
                if (_anneeMax != value)
                {
                    _anneeMax = value;
                    OnPropertyChanged("anneeMax");
                }
            }
        }

        private int _annee;

        /// <summary>
        /// Année de naissance du judoka
        /// </summary>
        public int annee
        {
            get { return _annee; }
            set
            {
                if (_annee != value)
                {
                    _annee = value;
                    OnPropertyChanged("annee");
                }
            }
        }

        private int _idepreuve;

        /// <summary>
        /// Id de l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int idepreuve
        {
            get { return _idepreuve; }
            set
            {
                if (_idepreuve != value)
                {
                    _idepreuve = value;
                    OnPropertyChanged("idepreuve");
                }
            }
        }

        private int _idcompet;

        /// <summary>
        /// Id de la compétition à laquelle est inscrit le judoka
        /// </summary>
        public int idcompet
        {
            get { return _idcompet; }
            set
            {
                if (_idcompet != value)
                {
                    _idcompet = value;
                    OnPropertyChanged("idcompet");
                }
            }
        }

        private int _serie;

        /// <summary>
        /// Numéro de tête de série du judoka
        /// </summary>
        public int serie
        {
            get { return _serie; }
            set
            {
                if (_serie != value)
                {
                    _serie = value;
                    OnPropertyChanged("serie");
                }
            }
        }

        private int _serie2;

        /// <summary>
        /// Classement du judoka à l'échelon - 1 (1-2 ou 0)
        /// </summary>
        public int serie2
        {
            get { return _serie2; }
            set
            {
                if (_serie2 != value)
                {
                    _serie2 = value;
                    OnPropertyChanged("serie2");
                }
            }
        }

        private int _classement;

        /// <summary>
        /// Classement du judoka à l'épreuve
        /// </summary>
        public int classement
        {
            get { return _classement; }
            set
            {
                if (_classement != value)
                {
                    _classement = value;
                    OnPropertyChanged("classement");
                }
            }
        }

        private int _poidsMin;

        /// <summary>
        /// Poids minimum requis dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int poidsMin
        {
            get { return _poidsMin; }
            set
            {
                if (_poidsMin != value)
                {
                    _poidsMin = value;
                    OnPropertyChanged("poidsMin");
                }
            }
        }

        private int _poidsMax;

        /// <summary>
        /// Poids maximum requis dans l'épreuve à laquelle est inscrit le judoka
        /// </summary>
        public int poidsMax
        {
            get { return _poidsMax; }
            set
            {
                if (_poidsMax != value)
                {
                    _poidsMax = value;
                    OnPropertyChanged("poidsMax");
                }
            }
        }

        private int _observation;

        /// <summary>
        /// Observation lors de la pesée (pas de passeport, pas de certificat médical, ...)
        /// </summary>
        public int observation
        {
            get { return _observation; }
            set
            {
                if (_observation != value)
                {
                    _observation = value;
                    OnPropertyChanged("observation");
                }
            }
        }


        private int _points;

        /// <summary>
        /// Points restant pour le Shiai (passage de grade)
        /// </summary>        
        public int points
        {
            get { return _points; }
            set
            {
                if (_points != value)
                {
                    _points = value;
                    OnPropertyChanged("points");
                }
            }
        }


        private string _club;

        /// <summary>
        /// ID du club du judoka
        /// </summary>
        public string club
        {
            get { return _club; }
            set
            {
                if (_club != value)
                {
                    _club = value;
                    OnPropertyChanged("club");
                }
            }
        }


        private string _nomCategorieAge;

        /// <summary>
        /// Nom de la catégorie d'âge du judoka
        /// </summary>
        public string nomCategorieAge
        {
            get { return _nomCategorieAge; }
            set
            {
                if (_nomCategorieAge != value)
                {
                    _nomCategorieAge = value;
                    OnPropertyChanged("nomCategorieAge");
                }
            }
        }

        private string _clubNomCourt;

        /// <summary>
        /// Nom court du club du judoka
        /// </summary>
        public string clubNomCourt
        {
            get { return _clubNomCourt; }
            set
            {
                if (_clubNomCourt != value)
                {
                    _clubNomCourt = value;
                    OnPropertyChanged("clubNomCourt");
                }
            }
        }

        private string _clubNom;

        /// <summary>
        /// Nom du club du judoka
        /// </summary>
        public string clubNom
        {
            get { return _clubNom; }
            set
            {
                if (_clubNom != value)
                {
                    _clubNom = value;
                    OnPropertyChanged("clubNom");
                }
            }
        }

        private string _comiteNomCourt;

        /// <summary>
        /// Nom court du club du judoka
        /// </summary>
        public string comiteNomCourt
        {
            get
            {
                int com = 0;
                if (int.TryParse(_comiteNomCourt, out com))
                {
                    return com.ToString("00");
                }
                else
                {
                    return _comiteNomCourt;
                }
            }
            set
            {
                if (_comiteNomCourt != value)
                {
                    _comiteNomCourt = value;
                    OnPropertyChanged("comiteNomCourt");
                }
            }
        }

        private string _comiteNom;

        /// <summary>
        /// Nom du club du judoka
        /// </summary>
        public string comiteNom
        {
            get { return _comiteNom; }
            set
            {
                if (_comiteNom != value)
                {
                    _comiteNom = value;
                    OnPropertyChanged("comiteNom");
                }
            }
        }

        private string _ligueNomCourt;

        /// <summary>
        /// Nom court du club du judoka
        /// </summary>
        public string ligueNomCourt
        {
            get { return _ligueNomCourt; }
            set
            {
                if (_ligueNomCourt != value)
                {
                    _ligueNomCourt = value;
                    OnPropertyChanged("ligueNomCourt");
                }
            }
        }

        private string _ligueNom;

        /// <summary>
        /// Nom du club du judoka
        /// </summary>
        public string ligueNom
        {
            get { return _ligueNom; }
            set
            {
                if (_ligueNom != value)
                {
                    _ligueNom = value;
                    OnPropertyChanged("ligueNom");
                }
            }
        }

        //private string _comite;

        ///// <summary>
        ///// Nom du comite du judoka
        ///// </summary>
        //public string comite
        //{
        //    get { return _comite; }
        //    set
        //    {
        //        if (_comite != value)
        //        {
        //            _comite = value;
        //            OnPropertyChanged("comite");
        //        }
        //    }
        //}

        //private string _comiteNum;

        ///// <summary>
        ///// Numéro du comite du judoka
        ///// </summary>
        //public string comiteNum
        //{
        //    get { return _comiteNum; }
        //    set
        //    {
        //        if (_comiteNum != value)
        //        {
        //            _comiteNum = value;
        //            OnPropertyChanged("comiteNum");
        //        }
        //    }
        //}

        private string _nomCeinture;

        /// <summary>
        /// Nom de la ceiture du judoka
        /// </summary>
        public string nomCeinture
        {
            get { return _nomCeinture; }
            set
            {
                if (_nomCeinture != value)
                {
                    _nomCeinture = value;
                    OnPropertyChanged("nomCeinture");
                }
            }
        }

        private string _couleur1;

        /// <summary>
        /// Couleur1 de la ceiture du judoka
        /// </summary>
        public string couleur1
        {
            get { return _couleur1; }
            set
            {
                if (_couleur1 != value)
                {
                    _couleur1 = value;
                    OnPropertyChanged("couleur1");
                }
            }
        }

        private string _couleur2;

        /// <summary>
        /// Couleur2 de la ceiture du judoka
        /// </summary>
        public string couleur2
        {
            get { return _couleur2; }
            set
            {
                if (_couleur2 != value)
                {
                    _couleur2 = value;
                    OnPropertyChanged("couleur2");
                }
            }
        }

        private string _lib_sexe;

        /// <summary>
        /// Libellé du sexe du judoka
        /// </summary>
        public string lib_sexe
        {
            get { return _lib_sexe; }
            set
            {
                if (_lib_sexe != value)
                {
                    _lib_sexe = value;
                    OnPropertyChanged("lib_sexe");
                }
            }
        }

        private string _libepreuve;

        /// <summary>
        /// Libellé de l'épreuve à laquelle est incrit le judoka
        /// </summary>
        public string libepreuve
        {
            get { return _libepreuve; }
            set
            {
                if (_libepreuve != value)
                {
                    _libepreuve = value;
                    OnPropertyChanged("libepreuve");
                }
            }
        }

        private string _nom_compet;

        /// <summary>
        /// Nom de la compétition à laquelle est incrit le judoka
        /// </summary>
        public string nom_compet
        {
            get { return _nom_compet; }
            set
            {
                if (_nom_compet != value)
                {
                    _nom_compet = value;
                    OnPropertyChanged("nom_compet");
                }
            }
        }

        private string _remoteId;

        /// <summary>
        /// ID du judoka dans la base de données fédérale
        /// </summary>
        public string remoteId
        {
            get { return _remoteId; }
            set
            {
                if (_remoteId != value)
                {
                    _remoteId = value;
                    OnPropertyChanged("remoteId");
                }
            }
        }

        private string _ligue;

        /// <summary>
        /// Nom de ligue du judoka
        /// </summary>
        public string ligue
        {
            get { return _ligue; }
            set
            {
                if (_ligue != value)
                {
                    _ligue = value;
                    OnPropertyChanged("ligue");
                }
            }
        }

        private string _comite;
        /// <summary>
        /// ID du comite du judoka
        /// </summary>
        public string comite
        {
            get { return _comite; }
            set
            {
                if (_comite != value)
                {
                    _comite = value;
                    OnPropertyChanged("comite");
                }
            }
        }

        private int _qualifie0;

        /// <summary>
        /// Qualifié pour l'échelon supérieur
        /// </summary>
        public int qualifie0
        {
            get { return _qualifie0; }
            set
            {
                if (_qualifie0 != value)
                {
                    _qualifie0 = value;
                    OnPropertyChanged("qualifie0");
                }
            }
        }

        private int _qualifie1;

        /// <summary>
        /// Qualifié pour l'échelon supérieur
        /// </summary>
        public int qualifie1
        {
            get { return _qualifie1; }
            set
            {
                if (_qualifie1 != value)
                {
                    _qualifie1 = value;
                    OnPropertyChanged("qualifie1");
                }
            }
        }

        private int _equipe;

        /// <summary>
        /// Id de l'équipe à laquelle apartient le judoka
        /// </summary>
        public int equipe
        {
            get { return _equipe; }
            set
            {
                if (_equipe != value)
                {
                    _equipe = value;
                    OnPropertyChanged("equipe");
                }
            }
        }

        private string _lib_equipe;

        /// <summary>
        /// Libelle de l'équipe à laquelle apartient le judoka
        /// </summary>
        public string lib_equipe
        {
            get { return _lib_equipe; }
            set
            {
                if (_lib_equipe != value)
                {
                    _lib_equipe = value;
                    OnPropertyChanged("lib_equipe");
                }
            }
        }

        private float _poidsKg;
        public float poidsKg
        {
            get { return _poidsKg; }
            set
            {
                if (_poidsKg != value)
                {
                    _poidsKg = value;
                    OnPropertyChanged("poidsKg");
                }
            }
        }


        private int _idepreuve_equipe;
        public int idepreuve_equipe
        {
            get { return _idepreuve_equipe; }
            set
            {
                if (_idepreuve_equipe != value)
                {
                    _idepreuve_equipe = value;
                    OnPropertyChanged("idepreuve_equipe");
                }
            }
        }

        #endregion

        #region CONSTRUCTEURS

        public vue_judoka(Judoka judoka, JudoData DC)
        {
            EpreuveJudoka ej = DC.Participants.EJS.FirstOrDefault(o => o.judoka == judoka.id);
            Epreuve ep = ej != null ? DC.Organisation.Epreuves.FirstOrDefault(o => o.id == ej.epreuve) : null;
            Ceintures ceinture = DC.Categories.Grades.FirstOrDefault(o => o.id == judoka.ceinture);
            CategorieAge cateAge = DC.Categories.CAges.FirstOrDefault(o => o.id == judoka.categorie);
            Competition compet = ep != null ? DC.Organisation.Competitions.FirstOrDefault(o => o.id == ep.competition) : null;
            Equipe equipe = DC.Participants.Equipes.FirstOrDefault(o => o.id == judoka.equipe);
            Epreuve_Equipe ep2 = ep != null ? DC.Organisation.EpreuveEquipes.FirstOrDefault(o => o.id == ep.epreuve_equipe) : null;

            this.id = judoka.id;
            this.licence = judoka.licence;
            this.nom = judoka.nom;
            this.prenom = judoka.prenom;
            this.ceinture = judoka.ceinture;
            this.naissance = judoka.naissance;
            this.sexe = judoka.sexe;
            this.categorie = judoka.categorie;
            this.modification = judoka.modification;
            this.club = judoka.club;
            this.pays = judoka.pays;
            this.datePesee = judoka.datePesee;
            this.present = judoka.present;
            this.passeport = judoka.passeport;
            this.modeControle = judoka.modeControle;
            this.modePesee = judoka.modePesee;
            this.remoteId = judoka.remoteID;
            this.poidsMesure = judoka.poidsMesure;
            this.poidsKg = judoka.poidsKg;
            this.poids = judoka.poids;
            this.annee = judoka.naissance.Year;
            this.equipe = judoka.equipe;
            this.lib_equipe = equipe != null ? equipe.libelle : "";


            if (ceinture != null)
            {
                this.nomCeinture = ceinture.nom;
                this.couleur1 = ceinture.couleur1;
                this.couleur2 = ceinture.couleur2;
            }
            else
            {
                this.nomCeinture = "";
                this.couleur1 = "";
                this.couleur2 = "";
            }

            if (cateAge != null)
            {
                this.nomCategorieAge = cateAge.nom;
                this.anneeMin = cateAge.anneeMin;
                this.anneeMax = cateAge.anneeMax;
            }
            else
            {
                this.nomCategorieAge = "";
                this.anneeMin = 0;
                this.anneeMax = 0;
            }

            this.lib_sexe = judoka.sexeEnum.ToString();

            if (ep != null)
            {
                this.poidsMin = ep.poidsMin;
                this.poidsMax = ep.poidsMax;
                this.libepreuve = ep.nom;
                this.idepreuve = ep.id;
                this.idcompet = ep.competition;
            }
            else
            {
                this.poidsMin = 0;
                this.poidsMax = 0;
                this.libepreuve = "aucune";
                this.idepreuve = 0;
                this.idcompet = 0;
            }

            if (ej != null)
            {
                this.etat = ej.etat;
                this.serie = ej.serie;
                this.serie2 = ej.serie2;
                this.observation = ej.observation;
                this.classement = ej.classement;
                this.serie2 = ej.serie2;
                this.observation = ej.observation;
                this.classement = ej.classement;
                this.points = ej.points;
            }
            else
            {
                this.serie = 0;
                this.etat = (int)EtatJudokaEnum.Aucun;
                this.serie2 = 0;
                this.observation = 0;
                this.classement = 0;
                this.points = 0;
            }

            Club club = DC.Structures.Clubs.FirstOrDefault(o => o.id == judoka.club);

            if (club != null)
            {
                this.clubNomCourt = club.nomCourt;
                this.clubNom = club.nom;

                Comite comite = DC.Structures.Comites.FirstOrDefault(o => o.id == club.comite && o.ligue == club.ligue);
                if (comite != null)
                {
                    this.comite = comite.id;
                    this.comiteNomCourt = club.comite;
                    this.comiteNom = club.comite;
                }
                else
                {
                    this.comite = String.Empty;
                    this.comiteNomCourt = "0";
                    this.comiteNom = "";
                }

                Ligue ligue = DC.Structures.Ligues.FirstOrDefault(o => o.id == club.ligue);
                if (ligue != null)
                {
                    this.ligue = ligue.id;
                    this.ligueNom = ligue.nom;
                    this.ligueNomCourt = ligue.nomCourt;
                }
                else
                {
                    this.ligue = "";
                    this.ligueNom = "";
                    this.ligueNomCourt = "";
                }

            }
            else
            {
                this.comite = string.Empty;
                this.comiteNomCourt = "0";
                this.comiteNom = "";
                this.ligue = "";
                this.ligueNom = "";
                this.ligueNomCourt = "";
                this.clubNomCourt = judoka.club;
                this.clubNom = judoka.club;
            }

            if (compet != null)
            {
                this.nom_compet = compet.nom;
            }
            else
            {
                this.nom_compet = "Non inscrits";
            }

            if (ep2 == null)
            {
                this.idepreuve_equipe = 0;
            }
            else
            {
                this.idepreuve_equipe = ep2.id;
            }
        }

        #endregion

        #region METHODES

        public bool PeuxParticiter()
        {
            return this.etat == (int)EtatJudokaEnum.AuPoids && this.observation == 0;
        }


        public XElement ToXml()
        {
            XElement xjudoka = new XElement(ConstantXML.Vue_Judoka);

            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_ID, this.id.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Licence, this.licence);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Nom, this.nom);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Prenom, this.prenom);

            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_CeintureID, this.ceinture.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_CeintureNom, nomCeinture);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_CeintureCouleur1, couleur1);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_CeintureCouleur2, couleur2);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Naissance, this.naissance.ToString("dd/MM/yyyy"));
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Serie, this.serie.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Serie2, this.serie2.ToString());

            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Sexe, this.lib_sexe);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_PoidsMesure, this.poidsMesure.ToString("0:#.###"));
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Present, this.present.ToString().ToLower());
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Etat, this.etat.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_NomCategorieAge, this.nomCategorieAge);

            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Club, this.club);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_ClubNomCourt, this.clubNomCourt);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_ClubNom, this.clubNom);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Comite, this.comite);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_ComiteNomCourt, this.comiteNomCourt);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_ComiteNom, this.comiteNom);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Ligue, this.ligue);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_LigueNomCourt, this.ligueNomCourt);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_LigueNom, this.ligueNom);

            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_IdEpreuve, this.idepreuve.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_LibEpreuve, this.libepreuve);
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Qualifie0, this.qualifie0.ToString().ToLower());
            xjudoka.SetAttributeValue(ConstantXML.Vue_Judoka_Qualifie1, this.qualifie1.ToString().ToLower());

            return xjudoka;
        }

        #endregion
    }
}
