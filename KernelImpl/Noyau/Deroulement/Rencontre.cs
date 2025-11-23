
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    /// <summary>
    /// Description des Rencontres
    /// </summary>
    public class Rencontre : INotifyPropertyChanged, IIdEntity<int>
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

        public void Save(int? vainqueur, int score1, int score2, int penalite1, int penalite2, int ippon1, int ippon2,EtatCombattantEnum etat1, EtatCombattantEnum etat2,
            bool isProLeague)
        {
            //DialogControleur DC = Controles.DialogControleur.currentControleur;

            int old_vainqueur = 0;
            //Participant p = null;
            if (this.vainqueur != null && this.vainqueur != 0)
            {
                old_vainqueur = (int)this.vainqueur;
            }

            this.vainqueur = vainqueur;
            if (this.judoka1 != null && this.judoka2 != null)
            {
                this.vainqueur = vainqueur;

                this.score1 = score1;
                this.score2 = score2;

                this.ippon1 = ippon1;
                this.ippon2 = ippon2;

                this.penalite1 = penalite1;
                this.penalite2 = penalite2;

                this.etatJ1 = (int)etat1;
                this.etatJ2 = (int)etat2;

                if (this.vainqueur == null)
                {
                    ////----
                    /*if (this.penalite1 == 3 && this.penalite2 == 3)
                    {
                        this.vainqueur = null;
                    }
                    ////----
                    else*/
                    if (this.penalite1 == 3)
                    {
                        this.vainqueur = this.judoka2;
                    }
                    else if (this.penalite2 == 3)
                    {
                        this.vainqueur = this.judoka1;
                    }
                    else if (this.score1 > this.score2)
                    {
                        this.vainqueur = this.judoka1;
                    }
                    else if (this.score1 < this.score2)
                    {
                        this.vainqueur = this.judoka2;
                    }
                    else if (this.penalite1 > this.penalite2)
                    {
                        this.vainqueur = this.judoka2;
                    }
                    else if (this.penalite1 < this.penalite2)
                    {
                        this.vainqueur = this.judoka1;
                    }
                }

                this.etat = (int)EtatCombatEnum.Normal;
            }
            else
            {
                this.vainqueur = this.judoka1 != null ? this.judoka1 : this.judoka2 != null ? this.judoka2 : 0;
                this.etatJ1 = (int)EtatCombatEnum.Normal;
                this.etatJ2 = (int)EtatCombatEnum.Normal;
            }


            if (this.etatJ1 != (int)EtatCombattantEnum.Normal || this.etatJ2 != (int)EtatCombattantEnum.Normal)
            {
                if (this.etatJ1 == (int)EtatCombattantEnum.Decision)
                {
                    this.vainqueur = this.judoka1;
                }
                else if (this.etatJ2 == (int)EtatCombattantEnum.Decision)
                {
                    this.vainqueur = this.judoka2;
                }
                else if (this.etatJ1 != (int)EtatCombattantEnum.Normal && this.etatJ2 != (int)EtatCombattantEnum.Normal)
                {
                    this.vainqueur = 0;
                }
                else if (this.etatJ1 != (int)EtatCombattantEnum.Normal)
                {
                    if (!isProLeague)
                    {
                        this.score2 = 100;//16092024
                        this.score1 = 0;//16092024
                    }
                   
                    this.vainqueur = this.judoka2;


                }
                else if (this.etatJ2 != (int)EtatCombattantEnum.Normal)
                {
                    if (!isProLeague)
                    {
                        this.score1 = 100;//16092024
                        this.score2 = 0;//16092024
                    }
                    
                    this.vainqueur = this.judoka1;
                }
            }
        }

        //public string GetScore(int participant, CombatParameterStruct parameters)
        //{
        //    IList<Rencontre> rencontres = parameters.Rencontres.Where(o => o.combat == this.combat).ToList();


        //    Combat combat = parameters.Combats.FirstOrDefault(o => o.id == this.combat);

        //    combat.debut = rencontres.Min(o => o.debut);
        //    combat.fin = rencontres.Max(o => o.fin);

        //    int score1 = 0;
        //    int score2 = 0;

        //    int cumul1 = 0;
        //    int cumul2 = 0;

        //    foreach (Rencontre rencontre in rencontres)
        //    {
        //        Judoka judoka = parameters.Judokas.FirstOrDefault(p => p.id == rencontre.vainqueur);

        //        if (judoka == null)
        //        {
        //            continue;
        //        }
        //        if (judoka.equipe == combat.participant1)
        //        {
        //            score1++;
        //            cumul1 += rencontre.CalculeScore();
        //        }
        //        if (judoka.equipe == combat.participant2)
        //        {
        //            score2++;
        //            cumul2 += rencontre.CalculeScore();
        //        }
        //    }

        //    //int v = 0;

        //    int? v = null;

        //    if (score1 > score2)
        //    {
        //        v = combat.participant1;
        //    }
        //    else if (score2 > score1)
        //    {
        //        v = combat.participant2;
        //    }
        //    else if (score1 == score2)
        //    {
        //        if (cumul1 > cumul2)
        //        {
        //            v = combat.participant1;
        //        }
        //        else
        //        {
        //            v = combat.participant2;
        //        }
        //    }

        //    string result = "";

        //    if (participant == 1)
        //    {                
        //        result = score1 + "v." + cumul1;
        //    }
        //    else if (participant == 2)
        //    {
        //        result = score2 + "v." + cumul2;
        //    }

        //    return result;
        //}

        public Combat UpdateCombat(int? vainqueur, JudoData DC)
        {

            IList<Rencontre> rencontres = DC.Deroulement.Rencontres.Where(o => o.combat == this.combat).ToList();

            bool combatDecisifProLeagueEnAttente = false;
            if (DC.competition.IsProLeague())
            {
                Combat c = DC.Deroulement.Combats.FirstOrDefault(o => o.id == this.combat);
                if(!(c is null))
                {
                    Feuille feuille = DC.Deroulement.Feuilles.FirstOrDefault(o => o.combat == this.combat);
                    if (feuille != null) //TABLEAU
                    {
                        int niveau = (int)Math.Pow(2, c.niveau - 1);
                        if (niveau == 1 || niveau == 2 || niveau == 4)
                        {
                            IList<Rencontre> rencontresTmp = DC.Deroulement.Rencontres.Where(o => o.combat == this.combat).ToList();
                            //bool egaliteCombat = false;
                            int combatScore1 = 0;
                            int combatScore2 = 0;
                            int nbVictoire1 = 0;
                            int nbVictoire2 = 0;
                            foreach (Rencontre r in rencontresTmp)
                            {
                                combatScore1 += r.score1;
                                combatScore2 += r.score2;
                                if (r.judoka1 == r.vainqueur)
                                {
                                    nbVictoire1++;
                                }
                                if (r.judoka2 == r.vainqueur)
                                {
                                    nbVictoire2++;
                                }
                            }
                            if (combatScore1 == combatScore2 && nbVictoire1 == nbVictoire2)
                            {
                                //egaliteCombat = true;
                                combatDecisifProLeagueEnAttente = true;
                            }
                        }
                    }
                }
            }
            if (rencontres.Count(o => o.vainqueur == null) == 0 && !combatDecisifProLeagueEnAttente)//si toutes les rencontres d'un combat sont terminés
            {
                Combat combat = DC.Deroulement.Combats.FirstOrDefault(o => o.id == this.combat);

                if (combat == null)
                {
                    return null;
                }

                if (rencontres.Count == 0)
                {
                    combat.debut = DateTime.Now;
                    combat.fin = DateTime.Now;
                }
                else
                {
                    combat.debut = rencontres.Min(o => o.debut);
                    combat.fin = rencontres.Max(o => o.fin);
                }


                int score1 = 0;
                int score2 = 0;

                int cumul1 = 0;
                int cumul2 = 0;

                int ippon1 = 0;
                int ippon2 = 0;
                int shido1 = 0;
                int shido2 = 0;

                int? v = null;

                if (DC.competition.IsProLeague())
                {
                    Phase phase = DC.Deroulement.Phases.FirstOrDefault(o => o.id == combat.phase);

                    int nbVictoireDecisif1 = 0;
                    int nbVictoireDecisif2 = 0;

                    foreach (Rencontre rencontre in rencontres)
                    {
                        ippon1 += rencontre.ippon1;
                        ippon2 += rencontre.ippon2;
                        shido1 += rencontre.penalite1;
                        shido2 += rencontre.penalite2;

                        Participants.Judoka judoka = DC.Participants.Judokas.FirstOrDefault(p => p.id == rencontre.vainqueur);
                        if (judoka == null)
                        {
                            if (!rencontre.estDecisif)
                            {
                            cumul1 += rencontre.score1;
                            cumul2 += rencontre.score2;
                            }
                        }
                        else
                        {
                            if (judoka.equipe == combat.participant1)
                            {
                                if (!rencontre.estDecisif)
                                {
                                score1++;
                                cumul1 += rencontre.score1;
                                cumul2 += rencontre.score2;
                                }
                                else
                                {
                                    nbVictoireDecisif1++;
                                }
                            }
                            if (judoka.equipe == combat.participant2)
                            {
                                if (!rencontre.estDecisif)
                                {
                                score2++;
                                cumul1 += rencontre.score1;
                                cumul2 += rencontre.score2;
                            }
                                else
                                {
                                    nbVictoireDecisif2++;
                        }
                            }
                        }
                        #region old
                        /*if (rencontre.penalite1 == 1 && rencontre.penalite2 == 1) //si 2 shidos à 1 = match nul
                        {
                            if (phase.typePhase == (int)TypePhaseEnum.Poule)
                            {
                                cumul1 += 1; // 1 point pour un match nul
                                cumul2 += 1; // 1 point pour un match nul
                            }
                            else //tableau
                            {
                                cumul1 += rencontre.score1;
                                cumul2 += rencontre.score2;
                            }
                        }
                        else
                        {
                            Participants.Judoka judoka = DC.Participants.Judokas.FirstOrDefault(p => p.id == rencontre.vainqueur);
                            if (judoka == null)
                            {
                                if (phase.typePhase == (int)TypePhaseEnum.Poule)
                                {
                                    cumul1 += 1; // 1 point pour un match nul
                                    cumul2 += 1; // 1 point pour un match nul
                                }
                                else //tableau
                                {
                                    cumul1 += rencontre.score1;
                                    cumul2 += rencontre.score2;
                                }
                            }
                            else
                            {
                                if (judoka.equipe == combat.participant1)
                                {
                                    if (phase.typePhase == (int)TypePhaseEnum.Poule)
                                    {
                                        score1++;
                                        cumul1 += 3;
                                    }
                                    else //tableau
                                    {
                                        score1++;
                                        cumul1 += rencontre.score1;
                                        cumul2 += rencontre.score2;
                                    }
                                }
                                if (judoka.equipe == combat.participant2)
                                {
                                    if (phase.typePhase == (int)TypePhaseEnum.Poule)
                                    {
                                        score2++;
                                        cumul2 += 3;
                                    }
                                    else //tableau
                                    {
                                        score2++;
                                        cumul1 += rencontre.score1;
                                        cumul2 += rencontre.score2;
                                    }
                                }
                            }
                        }*/
                        #endregion

                    }

                    //int bonusIppon1 = ippon1 / 6;
                    //int bonusIppon2 = ippon2 / 6;
                    //if (ippon1 >= 6) cumul1 += 1; //1 point de bonus pour si total ippon >= 6
                    //if (ippon2 >= 6) cumul2 += 1; //1 point de bonus pour si total ippon >= 6
                    //cumul1 += bonusIppon1 * 6; //1 point de bonus pour 6 ippon marqués sur toutes les rencontres du combat de l'équipe 1
                    //cumul2 += bonusIppon2 * 6; //1 point de bonus pour 6 ippon marqués sur toutes les rencontres du combat de l'équipe 2

                    if ((score1 + nbVictoireDecisif1) > (score2 + nbVictoireDecisif2))//nb de victoire
                    {
                        v = combat.participant1;
                    }
                    else if ((score2 + nbVictoireDecisif2) > (score1 + nbVictoireDecisif1))
                    {
                        v = combat.participant2;
                    }
                    else if ((score1 + nbVictoireDecisif1) == (score2 + nbVictoireDecisif2))
                    {
                        if (cumul1 > cumul2) //nb points marqués
                        {
                            v = combat.participant1;
                        }
                        else if(cumul2 > cumul1)
                        {
                            v = combat.participant2;
                        }
                    }

                    //if (score1 > score2) //nb de victoire
                    //{
                    //    v = combat.participant1;
                    //}
                    //else if (score2 > score1)
                    //{
                    //    v = combat.participant2;
                    //}
                    //else if (score1 == score2)
                    //{
                    //    if (cumul1 > cumul2) //nb points marqués
                    //    {
                    //        v = combat.participant1;
                    //    }
                    //    else if(cumul2 > cumul1)
                    //    {
                    //        v = combat.participant2;
                    //    }
                    //    else if(cumul1 == cumul2)
                    //    {
                    //        if (ippon1 > ippon2) //nb de ippon
                    //        {
                    //            v = combat.participant1;
                    //        }
                    //        else if (ippon2 > ippon1)
                    //        {
                    //            v = combat.participant2;
                    //        }else if(ippon1 == ippon2)
                    //        {
                    //            if (shido1 < shido2) //nb de shido
                    //            {
                    //                v = combat.participant1;
                    //            }
                    //            else if (ippon2 < ippon1)
                    //            {
                    //                v = combat.participant2;
                    //            }
                    //        }
                    //    }
                    //}
                }
                else
                {
                    foreach (Rencontre rencontre in rencontres)
                    {
                        Participants.Judoka judoka = DC.Participants.Judokas.FirstOrDefault(p => p.id == rencontre.vainqueur);

                    if (judoka == null)
                    {
                        continue;
                    }
                    if (judoka.equipe == combat.participant1)
                    {
                        score1++;
                        cumul1 += rencontre.CalculeScore();
                        cumul2 += rencontre.CalculeScorePerdant();
                    }
                    if (judoka.equipe == combat.participant2)
                    {
                        score2++;
                        cumul2 += rencontre.CalculeScore();
                        cumul1 += rencontre.CalculeScorePerdant();
                    }
                }

                    //int v = 0;
                    if (score1 > score2)
                    {
                        v = combat.participant1;
                    }
                    else if (score2 > score1)
                    {
                        v = combat.participant2;
                    }
                    else if (score1 == score2)
                    {
                        if (cumul1 > cumul2)
                        {
                            v = combat.participant1;
                        }
                        else
                        {
                            v = combat.participant2;
                        }
                    }
                }
                

                combat.Save(v, cumul1, cumul2, combat.kinza1, combat.kinza2, 0, 0, score1, score2, EtatCombattantEnum.Normal, EtatCombattantEnum.Normal, DC);

                return combat;
                //SaveAndUpdateCombat(combat, DC, vainqueur, score1, score2, 0, 0, EtatCombattantEnum.Normal, EtatCombattantEnum.Normal, ref updated_feuilles, ref updated_tapis);
            }
            return null;
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
