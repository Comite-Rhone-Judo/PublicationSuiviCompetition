
using KernelImpl.Noyau.Deroulement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Participants
{
    /// <summary>
    /// Description des Judokas
    /// </summary>
    public class Judoka : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id;
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

        private bool _sexe;
        public bool sexe
        {
            get
            {
                return _sexe;
            }
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
                    sexe = (bool) _sexeEnum;
                }
            }
        }



        private bool _modification;
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

        private int _poids;
        public int poids
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

        private int _poidsMesure;
        public int poidsMesure
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

        private int _categorie;
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

        private string _club;
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

        private string _remoteID;
        public string remoteID
        {
            get { return _remoteID; }
            set
            {
                if (_remoteID != value)
                {
                    _remoteID = value;
                    OnPropertyChanged("remoteID");
                }
            }
        }

        private DateTime _datePesee;
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

        private int _qualifieE0;
        public int qualifieE0
        {
            get { return _qualifieE0; }
            set
            {
                if (_qualifieE0 != value)
                {
                    _qualifieE0 = value;
                    OnPropertyChanged("qualifieE0");
                }
            }
        }

        private int _qualifieE1;
        public int qualifieE1
        {
            get { return _qualifieE1; }
            set
            {
                if (_qualifieE1 != value)
                {
                    _qualifieE1 = value;
                    OnPropertyChanged("qualifieE1");
                }
            }
        }

        private bool _ajoute;
        public bool ajoute
        {
            get { return _ajoute; }
            set
            {
                if (_ajoute != value)
                {
                    _ajoute = value;
                    OnPropertyChanged("ajoute");
                }
            }
        }

        private int _equipe;
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


        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_ID));
            this.remoteID = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Judoka_RemoteID));
            this.licence = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Judoka_Licence));

            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Judoka_Nom));
            this.prenom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Judoka_Prenom));
            this.sexeEnum = new EpreuveSexe(XMLTools.LectureString(xinfo.Attribute(ConstantXML.Judoka_Sexe)));
            this.naissance = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Judoka_Naissance), "ddMMyyyy", DateTime.MinValue);
            this.pays = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_Pays));
            this.club = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Judoka_Club));
            this.ceinture = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_Grade));
            this.categorie = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_Categorie));
            this.present = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Judoka_Present));
            this.etat = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_Etat));

            this.datePesee =
                XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Judoka_DatePesee_Date), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Judoka_DatePesee_Time), "HHmmss");

            this.poids = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_Poids));
            this.poidsMesure = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_PoidsM));
            this.poidsKg = this.poidsMesure / 1000F;


            this.modification = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Judoka_Modification));
            this.passeport = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Judoka_Passeport));
            this.ajoute = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Judoka_Ajoute));
            this.qualifieE1 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_QualifieE1));
            this.qualifieE0 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_QualifieE0));

            this.modePesee = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_ModePesee));
            this.modeControle = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_ModeControle));
            this.equipe = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Judoka_Equipe));
        }

        public XElement ToXml(JudoData DC)
        {
            XElement xjudoka = new XElement(ConstantXML.Judoka);

            string real = remoteID;
            string[] r1 = remoteID.Split('_');
            if (r1.Count() == 7)
            {
                real = "";
            }

            xjudoka.SetAttributeValue(ConstantXML.Judoka_RemoteID, real.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Judoka_ID, id.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Licence, licence);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Nom, nom.ToUpper());
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Prenom, OutilsTools.FormatPrenom(prenom));
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Sexe, sexeEnum.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Naissance, naissance.ToString("ddMMyyyy"));
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Pays, pays);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Club, club);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Grade, ceinture);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Categorie, categorie);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Present, etat == (int)EtatJudokaEnum.AuPoids || etat == (int)EtatJudokaEnum.HorsPoids || etat == (int)EtatJudokaEnum.HorsCategorie);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Etat, etat.ToString());
            xjudoka.SetAttributeValue(ConstantXML.Judoka_PoidsM, poidsMesure);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Poids, poids);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Modification, modification);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_DatePesee_Date, datePesee.ToString("ddMMyyyy"));
            xjudoka.SetAttributeValue(ConstantXML.Judoka_DatePesee_Time, datePesee.ToString("HHmmss"));
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Passeport, passeport);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Ajoute, ajoute);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_QualifieE1, (QualifieEnum)qualifieE1);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_QualifieE0, qualifieE0);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_ModePesee, modePesee);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_ModeControle, modeControle);
            xjudoka.SetAttributeValue(ConstantXML.Judoka_Equipe, equipe);

            EpreuveJudoka judoka = null;
            using (TimedLock.Lock((DC.Participants.EJS as ICollection).SyncRoot))
            {
                judoka = DC.Participants.EJS.FirstOrDefault(o => o.judoka == this.id);
            }

            if (judoka != null)
            {
                xjudoka.SetAttributeValue(ConstantXML.Judoka_Points, judoka == null ? 0 : judoka.points);
                xjudoka.SetAttributeValue(ConstantXML.Judoka_Serie, judoka == null ? 0 : judoka.serie);
                xjudoka.SetAttributeValue(ConstantXML.Judoka_Serie2, judoka == null ? 0 : judoka.serie2);
                xjudoka.SetAttributeValue(ConstantXML.Judoka_Observation, judoka == null ? 0 : judoka.observation);

                Organisation.vue_epreuve epreuve = null;
                using (TimedLock.Lock((DC.Organisation.vepreuves as ICollection).SyncRoot))
                {
                    epreuve = DC.Organisation.vepreuves.FirstOrDefault(o => o.id == judoka.epreuve);
                }

                if (epreuve != null)
                {
                    xjudoka.SetAttributeValue(ConstantXML.Judoka_CatePoids_RemoteId, epreuve.remoteId_catepoids);
                }
            }

            return xjudoka;

        }

        /// <summary>
        /// Lecture des Judokas
        /// </summary>
        /// <param name="xelement">élément décrivant les Judokas</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Judokas</returns>

        public static ICollection<Judoka> LectureJudoka(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Judoka> judokas = new List<Judoka>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Judoka))
            {
                Judoka judoka = new Judoka();
                judoka.LoadXml(xinfo);
                judokas.Add(judoka);
            }
            return judokas;
        }
    }
}
