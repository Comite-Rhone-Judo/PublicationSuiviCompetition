
using KernelImpl.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;
using Tools.XML;

namespace KernelImpl.Noyau.Deroulement
{
    /// <summary>
    /// Description des Rencontres
    /// </summary>
    public class Rencontre : INotifyPropertyChanged, IEntityWithKey<int>
    {
        int IEntityWithKey<int>.EntityKey => id;

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


        private Nullable<int> _judoka1;
        public Nullable<int> judoka1
        {
            get { return _judoka1; }
            set
            {
                if (_judoka1 != value)
                {
                    _judoka1 = value;
                    OnPropertyChanged("judoka1");
                }
            }
        }

        private Nullable<int> _judoka2;
        public Nullable<int> judoka2
        {
            get { return _judoka2; }
            set
            {
                if (_judoka2 != value)
                {
                    _judoka2 = value;
                    OnPropertyChanged("judoka2");
                }
            }
        }

        private int _score1;
        public int score1
        {
            get { return _score1; }
            set
            {
                if (_score1 != value)
                {
                    _score1 = value;
                    OnPropertyChanged("score1");
                }
            }
        }

        private int _score2;
        public int score2
        {
            get { return _score2; }
            set
            {
                if (_score2 != value)
                {
                    _score2 = value;
                    OnPropertyChanged("score2");
                }
            }
        }

        private int _penalite1;
        public int penalite1
        {
            get { return _penalite1; }
            set
            {
                if (_penalite1 != value)
                {
                    _penalite1 = value;
                    OnPropertyChanged("penalite1");
                }
            }
        }

        private int _penalite2;
        public int penalite2
        {
            get { return _penalite2; }
            set
            {
                if (_penalite2 != value)
                {
                    _penalite2 = value;
                    OnPropertyChanged("penalite2");
                }
            }
        }

        private int _etatJ1;
        public int etatJ1
        {
            get { return _etatJ1; }
            set
            {
                if (_etatJ1 != value)
                {
                    _etatJ1 = value;
                    OnPropertyChanged("etatJ1");
                }
            }
        }

        private int _etatJ2;
        public int etatJ2
        {
            get { return _etatJ2; }
            set
            {
                if (_etatJ2 != value)
                {
                    _etatJ2 = value;
                    OnPropertyChanged("etatJ2");
                }
            }
        }

        private string _details;
        public string details
        {
            get { return _details; }
            set
            {
                if (_details != value)
                {
                    _details = value;
                    OnPropertyChanged("details");
                }
            }
        }

        private DateTime _programmation;
        public DateTime programmation
        {
            get { return _programmation; }
            set
            {
                if (_programmation != value)
                {
                    _programmation = value;
                    OnPropertyChanged("programmation");
                }
            }
        }

        private DateTime _debut;
        public DateTime debut
        {
            get { return _debut; }
            set
            {
                if (_debut != value)
                {
                    _debut = value;
                    OnPropertyChanged("debut");
                }
            }
        }

        private DateTime _fin;
        public DateTime fin
        {
            get { return _fin; }
            set
            {
                if (_fin != value)
                {
                    _fin = value;
                    OnPropertyChanged("fin");
                }
            }
        }

        private double _temps;
        public double temps
        {
            get { return _temps; }
            set
            {
                if (_temps != value)
                {
                    _temps = value;
                    OnPropertyChanged("temps");
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

        private int _arbitre1;
        public int arbitre1
        {
            get { return _arbitre1; }
            set
            {
                if (_arbitre1 != value)
                {
                    _arbitre1 = value;
                    OnPropertyChanged("arbitre1");
                }
            }
        }

        private int _arbitre2;
        public int arbitre2
        {
            get { return _arbitre2; }
            set
            {
                if (_arbitre2 != value)
                {
                    _arbitre2 = value;
                    OnPropertyChanged("arbitre2");
                }
            }
        }

        private int _arbitre3;
        public int arbitre3
        {
            get { return _arbitre3; }
            set
            {
                if (_arbitre3 != value)
                {
                    _arbitre3 = value;
                    OnPropertyChanged("arbitre3");
                }
            }
        }

        private Nullable<int> _vainqueur;
        public Nullable<int> vainqueur
        {
            get { return _vainqueur; }
            set
            {
                if (_vainqueur != value)
                {
                    _vainqueur = value;
                    OnPropertyChanged("vainqueur");
                }
            }
        }

        private Nullable<int> _combat;
        public Nullable<int> combat
        {
            get { return _combat; }
            set
            {
                if (_combat != value)
                {
                    _combat = value;
                    OnPropertyChanged("combat");
                }
            }
        }

        private int _CatePoids;
        public int CatePoids
        {
            get { return _CatePoids; }
            set
            {
                if (_CatePoids != value)
                {
                    _CatePoids = value;
                    OnPropertyChanged("CatePoids");
                }
            }
        }

        private int _ippon1;
        public int ippon1
        {
            get { return _ippon1; }
            set
            {
                if (_ippon1 != value)
                {
                    _ippon1 = value;
                    OnPropertyChanged("ippon1");
                }
            }
        }

        private int _ippon2;
        public int ippon2
        {
            get { return _ippon2; }
            set
            {
                if (_ippon2 != value)
                {
                    _ippon2 = value;
                    OnPropertyChanged("ippon2");
                }
            }
        }

        public int tempsCombat { get; set; }
        public int tempsRecuperation { get; set; }
        public int tempsHippon { get; set; }
        public int tempsWazaAri { get; set; }
        public int tempsYuko { get; set; }

        public int tempsRecupFinal { get; set; }
        public string discipline { get; set; }
        public bool goldenScore { get; set; }
        public bool isNewRencontre { get; set; }

        public bool estDecisif { get; set; }
        // Retourne True si le combat peut etre selectionne (il a tout ses participants), false sinon
        public bool IsPlayable
        {
            get { return ((judoka1 != null) && (judoka1 != 0) && (judoka2 != null) && (judoka2 != 0) && (vainqueur == null) && (vainqueur != 0)); }
        }

        /// <summary>
        /// Etablir le Score d'un combat
        /// </summary>
        /// <param name="DC"></param>

        public int CalculeScore()
        {
            int scoreV = 0;
            int scoreP = 0;

            int penV = 0;
            int penP = 0;

            if (this.vainqueur == this.judoka1)
            {
                scoreV = this.score1;
                penV = this.penalite1;
                scoreP = this.score2;
                penP = this.penalite2;
            }
            else if (this.vainqueur == this.judoka2)
            {
                scoreV = this.score2;
                penV = this.penalite2;
                scoreP = this.score1;
                penP = this.penalite1;
            }

            int ipponV = scoreV / 100;               // Récupère le chiffre des centaines
            int wazaV = (scoreV / 10) % 10;          // Récupère le chiffre des dizaines
            int yukkoV = scoreV % 10;                // Récupère le chiffre des unités
            int ipponP = scoreP / 100;               // Récupère le chiffre des centaines
            int wazaP = (scoreP / 10) % 10;          // Récupère le chiffre des dizaines
            int yukkoP = scoreP % 10;                // Récupère le chiffre des unités
            /* if (this.etatJ1 == (int)EtatCombattantEnum.Normal && this.etatJ2 == (int)EtatCombattantEnum.Normal)
            {*/
            //if (this.etatJ1 != (int)EtatCombattantEnum.HansokuMakeX && this.etatJ2 != (int)EtatCombattantEnum.HansokuMakeX &&
            //    this.etatJ1 != (int)EtatCombattantEnum.HansokuMakeH && this.etatJ2 != (int)EtatCombattantEnum.HansokuMakeH)
                if (this.etatJ1 == (int)EtatCombattantEnum.Normal && this.etatJ2 == (int)EtatCombattantEnum.Normal)
            {
                if ((this.judoka1 == null && this.judoka2 == this.vainqueur) || (this.judoka2 == null && this.judoka1 == this.vainqueur))
                {
                    return 100; //10
                }

                //Victoire par IPPON 
                if ((scoreV / 100) - (scoreP / 100) >= 1)
                {
                    return 100; //10
                }

                ////Victoire par 2 WAZA-ARI
                if (scoreV >= 20)
                {
                    return 100; // 10;
                }

                //Pénalités
                if (penP >= 3 && penV >= 3)
                {
                    return scoreV / 10;
                }else if (penP >= 3)
                {
                    return 100; // 10;
                    ////----return 0;
                }

                //Victoire par WAZA-ARI
                /*if ((scoreV / 10) - (scoreP / 10) >= 1)
                {
                    return scoreV / 10;
                    //return 7;
                }*/
                return (wazaV * 10) + yukkoV;

                //if (scoreV >= 10 && scoreP < 10)
                //{
                //    return 7;
                //}

                //Victoire par YUKO
                /*if (scoreV - scoreP > 0)
                {
                    return 5;
                }*/

                // return 0;

                //if (penP > penV)
                //{
                //    return 1;
                //}
                //else
                //{
                //    return 0;
                //}
            }
            else
            {
                return 10;
            }
            /*  }
              else if (this.etatJ1 == (int)EtatCombattantEnum.Decision || this.etatJ2 == (int)EtatCombattantEnum.Decision)
              {
                  return 0;
              }
              else
              {
                  return 10;
              }*/
        }

        public int CalculeScorePerdant()
        {
            int scoreV = 0;
            int scoreP = 0;

            int penV = 0;
            int penP = 0;

            if (this.vainqueur == this.judoka1)
            {
                scoreV = this.score1;
                penV = this.penalite1;
                scoreP = this.score2;
                penP = this.penalite2;
            }
            else if (this.vainqueur == this.judoka2)
            {
                scoreV = this.score2;
                penV = this.penalite2;
                scoreP = this.score1;
                penP = this.penalite1;
            }


            if (penP >= 3)
            {
                return scoreP / 10;
            }

            //Victoire par WAZA-ARI
            /*if ((scoreV / 10) - (scoreP / 10) >= 1)
            {
                return scoreP / 10;
                //return 7;
            }*/

            int ipponV = scoreV / 100;               // Récupère le chiffre des centaines
            int wazaV = (scoreV / 10) % 10;          // Récupère le chiffre des dizaines
            int yukkoV = scoreV % 10;                // Récupère le chiffre des unités

            int ipponP = scoreP / 100;               // Récupère le chiffre des centaines
            int wazaP = (scoreP / 10) % 10;          // Récupère le chiffre des dizaines
            int yukkoP = scoreP % 10;                // Récupère le chiffre des unités

            return (wazaP * 10) + yukkoP; //maximum 1 waza

            // return 0;
        }

        public void LoadXml(XElement xrencontre)
        {
            this.id = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Id));
            this.judoka1 = XMLTools.LectureNullableInt(xrencontre.Attribute(ConstantXML.Rencontre_Judoka1));
            this.judoka2 = XMLTools.LectureNullableInt(xrencontre.Attribute(ConstantXML.Rencontre_Judoka2));
            this.score1 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Score1));
            this.score2 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Score2));
            this.penalite1 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Penalite1));
            this.penalite2 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Penalite2));

            this.details = XMLTools.LectureString(xrencontre.Attribute(ConstantXML.Rencontre_Details));

            this.programmation =
                XMLTools.LectureDate(xrencontre.Attribute(ConstantXML.Rencontre_Date_Programmation), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xrencontre.Attribute(ConstantXML.Rencontre_Time_Programmation), "HHmmss");

            this.fin =
                XMLTools.LectureDate(xrencontre.Attribute(ConstantXML.Rencontre_Date_Fin), "ddMMyyyy", DateTime.MinValue) +
                XMLTools.LectureTime(xrencontre.Attribute(ConstantXML.Rencontre_Time_Fin), "HHmmss");

            this.debut =
                XMLTools.LectureDate(xrencontre.Attribute(ConstantXML.Rencontre_Date_Debut), "ddMMyyyy", DateTime.MinValue) +
                XMLTools.LectureTime(xrencontre.Attribute(ConstantXML.Rencontre_Time_Debut), "HHmmss");

            this.temps = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Temps));
            this.etat = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Etat));
            this.arbitre1 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Arbitre1));
            this.arbitre2 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Arbitre2));
            this.arbitre3 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Arbitre3));
            this.vainqueur = XMLTools.LectureNullableInt(xrencontre.Attribute(ConstantXML.Rencontre_Vainqueur));
            this.combat = XMLTools.LectureNullableInt(xrencontre.Attribute(ConstantXML.Rencontre_Combat));
            this.CatePoids = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_CatePoids));

            this.etatJ1 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_EtatJ1));
            this.etatJ2 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_EtatJ2));


            this.tempsCombat = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_TempsCombat));
            this.tempsRecuperation = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_TempsRecuperation));
            this.tempsHippon = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_TempsHippon));
            this.tempsWazaAri = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_TempsWazaAri));
            this.tempsYuko = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_TempsYuko));

            this.tempsRecupFinal = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_TempsRecupFinal));
            this.discipline = XMLTools.LectureString(xrencontre.Attribute(ConstantXML.Rencontre_Discipline));
            this.ippon1 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Ippon1));
            this.ippon2 = XMLTools.LectureInt(xrencontre.Attribute(ConstantXML.Rencontre_Ippon2));

            this.goldenScore = XMLTools.LectureBool(xrencontre.Attribute(ConstantXML.Rencontre_GoldenScore));
            this.isNewRencontre = XMLTools.LectureBool(xrencontre.Attribute(ConstantXML.Rencontre_IsNewRencontre));
            
            this.estDecisif = XMLTools.LectureBool(xrencontre.Attribute(ConstantXML.Rencontre_EstDecisif));
        }

        public XElement ToXml()
        {
            XElement xrencontre = new XElement(ConstantXML.Rencontre);

            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Id, id);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Temps, temps);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Date_Debut, debut.ToString("ddMMyyyy"));
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Time_Debut, debut.ToString("HHmmss"));
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Date_Fin, fin.ToString("ddMMyyyy"));
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Time_Fin, fin.ToString("HHmmss"));
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Date_Programmation, programmation.ToString("ddMMyyyy"));
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Time_Programmation, programmation.ToString("HHmmss"));
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Vainqueur, vainqueur);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Etat, etat);

            xrencontre.SetAttributeValue(ConstantXML.Rencontre_EtatJ1, etatJ1);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_EtatJ2, etatJ2);

            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Arbitre1, arbitre1);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Arbitre2, arbitre2);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Arbitre3, arbitre3);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Details, details);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Judoka1, judoka1);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Judoka2, judoka2);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Score1, score1);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Score2, score2);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Penalite1, penalite1);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Penalite2, penalite2);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Combat, combat);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_CatePoids, CatePoids);


            xrencontre.SetAttributeValue(ConstantXML.Rencontre_TempsRecupFinal, tempsRecupFinal);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Discipline, discipline);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Ippon1, ippon1);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_Ippon2, ippon2);

            xrencontre.SetAttributeValue(ConstantXML.Rencontre_GoldenScore, goldenScore);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_IsNewRencontre, isNewRencontre);
            xrencontre.SetAttributeValue(ConstantXML.Rencontre_EstDecisif, estDecisif);

            return xrencontre;
        }


        /// <summary>
        /// Lecture des Rencontres
        /// </summary>
        /// <param name="xelement">élément décrivant les Rencontres</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Rencontres</returns>

        public static ICollection<Rencontre> LectureRencontres(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Rencontre> rencontres = new List<Rencontre>();

            foreach (XElement xrencontre in xelement.Descendants(ConstantXML.Rencontre))
            {
                Rencontre rencontre = new Rencontre();
                rencontre.LoadXml(xrencontre);
                rencontres.Add(rencontre);
            }

            //foreach (XElement xinfo in xelement.Descendants(ConstantXML.Tapis))
            //{
            //    //if (tapis.HasValue && int.Parse(xinfo.Attribute(ConstantXML.Tapis).Value) != tapis)
            //    //{
            //    //    continue;
            //    //}
            //    int maximum = xelement.Descendants(ConstantXML.Rencontre).Count();
            //    int index = 0;

            //    Parallel.ForEach(xelement.Descendants(ConstantXML.Rencontre), xrencontre =>
            //    {
            //        if (MI != null)
            //        {
            //            using (TimedLock.Lock((rencontres as ICollection).SyncRoot))
            //            {
            //                Interlocked.Add(ref index, 1);
            //            }
            //            MI(index, maximum, "Importation des judokas");
            //        }

            //        Rencontre rencontre = new Rencontre();
            //        rencontre.LoadXml(xrencontre);
            //        using (TimedLock.Lock((rencontres as ICollection).SyncRoot))
            //        {
            //            rencontres.Add(rencontre);
            //        }
            //    });
            //}


            //foreach (XElement xinfo in xelement.Descendants(ConstantXML.Tapis))
            //{
            //    if (int.Parse(xinfo.Attribute(ConstantXML.Tapis).Value) != tapis)
            //    {
            //        continue;
            //    }

            //    foreach (XElement xrencontre in xelement.Descendants(ConstantXML.Rencontre))
            //    {
            //        Rencontre rencontre = new Rencontre();
            //        rencontre.LoadXml(xrencontre);
            //        rencontres.Add(rencontre);
            //    }
            //}
            return rencontres;
        }
    }
}
