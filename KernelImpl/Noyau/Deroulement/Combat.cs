using KernelImpl;
using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    public class Combat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id = 0;
        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("id");
            }
        }

        // Retourne True si le combat peut etre selectionne (il a tout ses participants), false sinon
        public bool IsPlayable
        {
            get { return ((participant1 != null) && (participant1 != 0) && (participant2 != null) && (participant2 != 0) && (vainqueur == null) && (vainqueur != 0)); }
        }

        public int numero { get; set; }
        public string reference { get; set; }
        public Nullable<int> participant1 { get; set; }
        public Nullable<int> participant2 { get; set; }
        public int score1 { get; set; }
        public int score2 { get; set; }
        public int penalite1 { get; set; }
        public int penalite2 { get; set; }
        public int etatJ1 { get; set; }
        public int etatJ2 { get; set; }
        public int positionJ1 { get; set; }
        public int positionJ2 { get; set; }
        public int nbVictoire1 { get; set; }
        public int nbVictoire2 { get; set; }
        public string details { get; set; }
        public int phase { get; set; }
        public System.DateTime programmation { get; set; }
        public System.DateTime debut { get; set; }
        public System.DateTime fin { get; set; }
        public double temps { get; set; }
        public int etat { get; set; }
        public int arbitre1 { get; set; }
        public int arbitre2 { get; set; }
        public int arbitre3 { get; set; }
        public int niveau { get; set; }
        public Nullable<int> vainqueur { get; set; }
        public bool virtuel { get; set; }
        public int epreuve { get; set; }
        public Nullable<int> tapis { get; set; }
        public Nullable<int> groupe { get; set; }
        public int first_rencontre { get; set; }

        public int tempsCombat { get; set; }
        public int tempsRecuperation { get; set; }
        public int tempsHippon { get; set; }
        public int tempsWazaAri { get; set; }
        public int tempsYuko { get; set; }

        public int kinza1 { get; set; }
        public int kinza2 { get; set; }

        public bool goldenScore { get; set; }
        public bool isNewCombat { get; set; }

        public bool challenge1Refused { get; set; }
        public bool challenge2Refused { get; set; }

        public string scoresJujitsu { get; set; }

        public int pointsGRCH1 { get; set; }
        public int pointsGRCH2 { get; set; }

        public int tempsRecupFinal { get; set; }
        public string discipline { get; set; }
        public int ippon1 { get; set; }
        public int ippon2 { get; set; }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_ID));
            this.numero = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Numero));
            this.reference = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Combat_Reference));

            if (xinfo.Elements(ConstantXML.Combat_Score).Count() > 0)
            {
                XElement ele_Judoka1 = xinfo.Elements(ConstantXML.Combat_Score).ElementAt(0);
                if (ele_Judoka1.Attribute(ConstantXML.Combat_Judoka) != null)
                {
                    this.participant1 = XMLTools.LectureNullableInt(ele_Judoka1.Attribute(ConstantXML.Combat_Judoka));
                    this.score1 = XMLTools.LectureInt(ele_Judoka1.Attribute(ConstantXML.Combat_Score));
                    this.penalite1 = XMLTools.LectureInt(ele_Judoka1.Attribute(ConstantXML.Combat_Penalite));
                    this.etatJ1 = XMLTools.LectureInt(ele_Judoka1.Attribute(ConstantXML.Combat_Etat));
                    this.nbVictoire1 = XMLTools.LectureInt(ele_Judoka1.Attribute(ConstantXML.Combat_NbVictoire));
                }
            }

            if (xinfo.Elements(ConstantXML.Combat_Score).Count() > 1)
            {
                XElement ele_Judoka2 = xinfo.Elements(ConstantXML.Combat_Score).ElementAt(1);
                if (ele_Judoka2.Attribute(ConstantXML.Combat_Judoka) != null)
                {
                    this.participant2 = XMLTools.LectureNullableInt(ele_Judoka2.Attribute(ConstantXML.Combat_Judoka));
                    this.score2 = XMLTools.LectureInt(ele_Judoka2.Attribute(ConstantXML.Combat_Score));
                    this.penalite2 = XMLTools.LectureInt(ele_Judoka2.Attribute(ConstantXML.Combat_Penalite));
                    this.etatJ2 = XMLTools.LectureInt(ele_Judoka2.Attribute(ConstantXML.Combat_Etat));
                    this.nbVictoire2 = XMLTools.LectureInt(ele_Judoka2.Attribute(ConstantXML.Combat_NbVictoire));
                }
            }

            this.details = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Combat_Detail));
            this.phase = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Phase));

            this.programmation =
                XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Combat_Date_programmation), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Combat_Time_programmation), "HHmmss");

            this.fin =
                XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Combat_Date_fin), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Combat_Time_fin), "HHmmss");

            this.debut =
                XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Combat_Date_debut), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Combat_Time_debut), "HHmmss");


            this.temps = XMLTools.LectureDouble(xinfo.Attribute(ConstantXML.Combat_Temps));
            this.etat = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Etat));
            this.arbitre1 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Arbitre1));
            this.arbitre2 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Arbitre2));
            this.arbitre3 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Arbitre3));
            this.niveau = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Niveau));
            this.vainqueur = XMLTools.LectureNullableInt(xinfo.Attribute(ConstantXML.Combat_Vainqueur));
            this.vainqueur = this.vainqueur == -1 ? null : this.vainqueur;
            this.virtuel = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Combat_Virtuel));
            this.epreuve = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Epreuve));

            this.tapis = XMLTools.LectureNullableInt(xinfo.Attribute(ConstantXML.Combat_Tapis));
            this.groupe = XMLTools.LectureNullableInt(xinfo.Attribute(ConstantXML.Combat_Groupe));
            this.first_rencontre = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_FirstRencontre));

            this.tempsCombat = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_TempsCombat));
            this.tempsRecuperation = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_TempsRecuperation));
            this.tempsHippon = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_TempsHippon));
            this.tempsWazaAri = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_TempsWazaAri));
            this.tempsYuko = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_TempsYuko));

            this.goldenScore = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Combat_TempsGolden));
            this.isNewCombat = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Combat_IsNewCombat));
            

            this.kinza1 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Kinza1));
            this.kinza2 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_Kinza2));

            this.challenge1Refused = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Combat_Challenge1Refused));
            this.challenge2Refused = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Combat_Challenge2Refused));
            this.scoresJujitsu = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Combat_ScoresJujitsu));
            this.ippon1 = 0;
            this.ippon2 = 0;


            this.tempsRecupFinal = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_TempsRecupFinal));
            this.discipline = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Combat_Discipline));

            this.pointsGRCH1 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_PointsGRCH1));
            this.pointsGRCH2 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Combat_PointsGRCH2));


            //this.epreuve = OutilsTools.LectureInt(xinfo.Parent.Parent.Element(ConstantXML.Epreuve).Attribute(ConstantXML.Epreuve_ID));
        }

        public XElement ToXml(JudoData DC)
        {
            XElement xcombat = new XElement(ConstantXML.Combat);

            xcombat.SetAttributeValue(ConstantXML.Combat_ID, id);
            xcombat.SetAttributeValue(ConstantXML.Combat_Numero, numero);
            xcombat.SetAttributeValue(ConstantXML.Combat_Reference, reference);
            xcombat.SetAttributeValue(ConstantXML.Combat_Groupe, groupe);
            xcombat.SetAttributeValue(ConstantXML.Combat_FirstRencontre, first_rencontre);
            using (TimedLock.Lock((DC.Organisation.vepreuves as ICollection).SyncRoot))
            {
                vue_epreuve ep = DC.Organisation.vepreuves.FirstOrDefault(o => o.id == first_rencontre);
                xcombat.SetAttributeValue(ConstantXML.Combat_FirstRencontreLib, ep != null ? ep.nom_catepoids : "");
            }
            xcombat.SetAttributeValue(ConstantXML.Combat_Niveau, niveau);
            xcombat.SetAttributeValue(ConstantXML.Combat_Temps, temps.ToString(CultureInfo.InvariantCulture));
            xcombat.SetAttributeValue(ConstantXML.Combat_Date_debut, debut.ToString("ddMMyyyy"));
            xcombat.SetAttributeValue(ConstantXML.Combat_Time_debut, debut.ToString("HHmmss"));
            xcombat.SetAttributeValue(ConstantXML.Combat_Date_fin, fin.ToString("ddMMyyyy"));
            xcombat.SetAttributeValue(ConstantXML.Combat_Time_fin, fin.ToString("HHmmss"));
            xcombat.SetAttributeValue(ConstantXML.Combat_Date_programmation, programmation.ToString("ddMMyyyy"));
            xcombat.SetAttributeValue(ConstantXML.Combat_Time_programmation, programmation.ToString("HHmmss"));
            xcombat.SetAttributeValue(ConstantXML.Combat_Vainqueur, (vainqueur.HasValue ? (int)vainqueur : -1));
            xcombat.SetAttributeValue(ConstantXML.Combat_ScoreVainqueur, GetScoreVainqueur(DC));
            xcombat.SetAttributeValue(ConstantXML.Combat_ScorePerdant, GetScorePerdant(DC));
            xcombat.SetAttributeValue(ConstantXML.Combat_PenVainqueur, GetPenaliteVainqueur());
            xcombat.SetAttributeValue(ConstantXML.Combat_PenPerdant, GetPenalitePerdant());

            xcombat.SetAttributeValue(ConstantXML.Combat_Kinza1, kinza1.ToString());
            xcombat.SetAttributeValue(ConstantXML.Combat_Kinza2, kinza2.ToString());

            xcombat.SetAttributeValue(ConstantXML.Combat_Tapis, tapis);
            xcombat.SetAttributeValue(ConstantXML.Combat_Etat, etat);
            xcombat.SetAttributeValue(ConstantXML.Combat_Arbitre1, arbitre1);
            xcombat.SetAttributeValue(ConstantXML.Combat_Arbitre2, arbitre2);
            xcombat.SetAttributeValue(ConstantXML.Combat_Arbitre3, arbitre3);
            xcombat.SetAttributeValue(ConstantXML.Combat_Virtuel, virtuel);
            xcombat.SetAttributeValue(ConstantXML.Combat_Phase, phase);
            xcombat.SetAttributeValue(ConstantXML.Combat_Detail, details);
            xcombat.SetAttributeValue(ConstantXML.Combat_Epreuve, epreuve);
            xcombat.SetAttributeValue(ConstantXML.Combat_TempsCombat, tempsCombat);
            xcombat.SetAttributeValue(ConstantXML.Combat_TempsRecuperation, tempsRecuperation);
            xcombat.SetAttributeValue(ConstantXML.Combat_TempsHippon, tempsHippon);
            xcombat.SetAttributeValue(ConstantXML.Combat_TempsWazaAri, tempsWazaAri);
            xcombat.SetAttributeValue(ConstantXML.Combat_TempsYuko, tempsYuko);
            xcombat.SetAttributeValue(ConstantXML.Combat_IsPlayable, IsPlayable);
            xcombat.SetAttributeValue(ConstantXML.Combat_TempsGolden, goldenScore);
            xcombat.SetAttributeValue(ConstantXML.Combat_Challenge1Refused, challenge1Refused);
            xcombat.SetAttributeValue(ConstantXML.Combat_Challenge2Refused, challenge2Refused);
            xcombat.SetAttributeValue(ConstantXML.Combat_ScoresJujitsu, scoresJujitsu);

            xcombat.SetAttributeValue(ConstantXML.Combat_TempsRecupFinal, tempsRecupFinal);
            xcombat.SetAttributeValue(ConstantXML.Combat_Discipline, discipline);

            xcombat.SetAttributeValue(ConstantXML.Combat_IsNewCombat, isNewCombat);

            xcombat.SetAttributeValue(ConstantXML.Combat_PointsGRCH1, pointsGRCH1);
            xcombat.SetAttributeValue(ConstantXML.Combat_PointsGRCH2, pointsGRCH2);

            Feuille feuille = DC.Deroulement.Feuilles.FirstOrDefault(o => o.combat == this.id);
            if (feuille != null)
            {
                xcombat.SetAttributeValue(ConstantXML.Combat_Repechage, feuille.repechage);
                xcombat.SetAttributeValue(ConstantXML.Combat_Niveau, feuille.niveau);
                XElement xfeuille = feuille.ToXml();
                xcombat.Add(xfeuille);
            }



            XElement xjudoka1 = new XElement(ConstantXML.Combat_Score);
            xjudoka1.SetAttributeValue(ConstantXML.Combat_Judoka, participant1 != null ? participant1.ToString() : "null");
            xjudoka1.SetAttributeValue(ConstantXML.Combat_Etat, etatJ1);
            xjudoka1.SetAttributeValue(ConstantXML.Combat_Score, score1.ToString("000"));
            xjudoka1.SetAttributeValue(ConstantXML.Combat_Kinza, kinza1.ToString());
            xjudoka1.SetAttributeValue(ConstantXML.Combat_Penalite, penalite1);
            xjudoka1.SetAttributeValue(ConstantXML.Combat_NbVictoire, nbVictoire1);
            xjudoka1.SetAttributeValue(ConstantXML.Combat_Points, (vainqueur != null && vainqueur == participant1 ? CalculeScore() : CalculeScorePerdant()));
            int scoreGRCH1 = CalculeScoreGRCH(DC, participant1);
            xjudoka1.SetAttributeValue(ConstantXML.Combat_PointsGRCH, (scoreGRCH1 == -1) ? 0 : scoreGRCH1);
            xjudoka1.SetAttributeValue(ConstantXML.Combat_Niveau, niveau);
            xcombat.Add(xjudoka1);


            XElement xjudoka2 = new XElement(ConstantXML.Combat_Score);
            xjudoka2.SetAttributeValue(ConstantXML.Combat_Judoka, participant2 != null ? participant2.ToString() : "null");
            xjudoka2.SetAttributeValue(ConstantXML.Combat_Etat, etatJ2);
            xjudoka2.SetAttributeValue(ConstantXML.Combat_Score, score2.ToString("000"));
            xjudoka2.SetAttributeValue(ConstantXML.Combat_Kinza, kinza2.ToString());
            xjudoka2.SetAttributeValue(ConstantXML.Combat_Penalite, penalite2);
            xjudoka2.SetAttributeValue(ConstantXML.Combat_NbVictoire, nbVictoire2);
            xjudoka2.SetAttributeValue(ConstantXML.Combat_Points, (vainqueur != null && vainqueur == participant2 ? CalculeScore() : CalculeScorePerdant()));
            int scoreGRCH2 = CalculeScoreGRCH(DC, participant2);
            xjudoka2.SetAttributeValue(ConstantXML.Combat_PointsGRCH, (scoreGRCH2 == -1) ? 0 : scoreGRCH2);

            xjudoka2.SetAttributeValue(ConstantXML.Combat_Niveau, niveau);
            xcombat.Add(xjudoka2);

            return xcombat;
        }

       
        public void Save(int? vainqueur, int score1, int score2, int kinza1, int kinza2, int penalite1, int penalite2, int nb1, int nb2,
           EtatCombattantEnum etat1, EtatCombattantEnum etat2, JudoData DC)
        {


            this.vainqueur = vainqueur;
            if (this.participant1 != null && this.participant2 != null)
            {
                this.score1 = score1;
                this.score2 = score2;

                this.kinza1 = kinza1;
                this.kinza2 = kinza2;

                this.penalite1 = penalite1;
                this.penalite2 = penalite2;

                this.etatJ1 = (int)etat1;
                this.etatJ2 = (int)etat2;

                this.nbVictoire1 = nb1;
                this.nbVictoire2 = nb2;

                if (this.vainqueur == null)
                {

                    this.vainqueur = this.getVainqueur(etat1, etat2, DC);
                }

                this.etat = (int)EtatCombatEnum.Normal;
            }
            else if (participant1 != null || participant2 != null)
            {
                this.vainqueur = this.participant1 != null ? this.participant1 : this.participant2;
                this.etat = (int)EtatCombatEnum.Normal;
            }

            if (this.etatJ1 != (int)EtatCombattantEnum.Normal || this.etatJ2 != (int)EtatCombattantEnum.Normal)
            {
                if (this.etatJ1 == (int)EtatCombattantEnum.Decision)
                {
                    this.vainqueur = this.participant1;
                }
                else if (this.etatJ2 == (int)EtatCombattantEnum.Decision)
                {
                    this.vainqueur = this.participant2;
                }
                else if (this.etatJ1 != (int)EtatCombattantEnum.Normal && this.etatJ2 != (int)EtatCombattantEnum.Normal)
                {
                    this.vainqueur = 0;
                }
                else if (this.etatJ1 != (int)EtatCombattantEnum.Normal)
                {
                    this.vainqueur = this.participant2;
                }
                else if (this.etatJ2 != (int)EtatCombattantEnum.Normal)
                {
                    this.vainqueur = this.participant1;
                }
            }

            if (this.vainqueur != null && this.vainqueur > 0)
            {
                Participant p1 = DC.Deroulement.Participants.FirstOrDefault(o => o.judoka == this.vainqueur && o.phase == this.phase);


                ////----Gestion de double shido
                if (this.penalite1 != 3 && this.penalite2 != 3 && p1 != null)
                {
                    if (p1 != null)
                    {
                        p1.nbVictoires = p1.nbVictoires + 1;

                        p1.nbVictoiresInd = p1.nbVictoiresInd + (this.vainqueur == this.participant1 ? this.nbVictoire1 : this.nbVictoire2);
                    }
                }
                ////----
                Participant p2 = DC.Deroulement.Participants.FirstOrDefault(o => o.judoka == (this.vainqueur == this.participant1 ? this.participant2 : this.participant1) && o.phase == this.phase);
                if (p2 != null)
                {
                    p2.nbVictoiresInd = p2.nbVictoiresInd + (this.vainqueur == this.participant1 ? this.nbVictoire2 : this.nbVictoire1);
                }

                if ((CompetitionTypeEnum)DC.competition.type == CompetitionTypeEnum.Shiai)
                {
                    int pointsGRCH = this.CalculeScoreGRCH(DC, this.vainqueur);
                    if (this.vainqueur == this.participant1)
                    {
                        this.pointsGRCH1 = pointsGRCH;
                        this.pointsGRCH2 = 0;
                    }
                    else
                    {
                        this.pointsGRCH2 = pointsGRCH;
                        this.pointsGRCH1 = 0;
                    }
                    
                    p1.cumulPointsGRCH = p1.cumulPointsGRCH + pointsGRCH;// this.CalculeScoreGRCH(DC, this.vainqueur);
                    p1.cumulPoints = p1.cumulPointsGRCH; //p1.cumulPoints + this.CalculeScoreGRCH(null, this.vainqueur);
                }
                else if ((CompetitionTypeEnum)DC.competition.type == CompetitionTypeEnum.Equipe)
                {
                    if (p1 != null)
                    {
                        p1.cumulPoints = p1.cumulPoints + (this.vainqueur == this.participant1 ? this.score1 : this.score2);
                        p1.cumulPointsGRCH = 0;
                    }
                    if (p2 != null)
                    { p2.cumulPoints = p2.cumulPoints + this.CalculeScorePerdant();/**/}
                }
                else
                {
                    if (p1 != null)
                    {
                        if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
                        {
                            p1.cumulPoints = p1.cumulPoints + this.CalculeScore();
                        }
                        else
                        {
                            p1.cumulPoints = p1.cumulPoints + this.CalculePointVainqueurJujitsu(DC, p1);
                            //if (p1.judoka == this.participant1)
                            //{
                            //    p1.cumulPoints = p1.cumulPoints + this.score1;
                            //}
                            //else
                            //{
                            //    p1.cumulPoints = p1.cumulPoints + this.score2;
                            //}
                        }
                            
                        int pointsGRCH = this.CalculeScoreGRCH(DC, this.vainqueur);
                        if (this.vainqueur == this.participant1)
                        {
                            this.pointsGRCH1 = pointsGRCH == -1 ? 0 : pointsGRCH;
                            this.pointsGRCH2 = 0;
                        }
                        else
                        {
                            this.pointsGRCH2 = pointsGRCH == -1 ? 0 : pointsGRCH;
                            this.pointsGRCH1 = 0;
                        }
                        if (DC.competition.discipline != CompetitionDisciplineEnum.Judo.ToString2())
                        {
                            //jujitsu
                            if(pointsGRCH == -1)
                            {
                                p1.cumulPointsGRCH = 0;// this.CalculeScoreGRCH(DC, this.vainqueur);
                            }
                            else
                            {
                                p1.cumulPointsGRCH = p1.cumulPointsGRCH + pointsGRCH;// this.CalculeScoreGRCH(DC, this.vainqueur);
                            }
                        }
                        else
                        {
                            p1.cumulPointsGRCH = p1.cumulPointsGRCH + pointsGRCH;// this.CalculeScoreGRCH(DC, this.vainqueur);
                        }
                        
                    }
                    if (p2 != null)
                    {
                        if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
                        {
                            p2.cumulPoints = p2.cumulPoints + this.CalculeScorePerdant();
                        }
                        else
                        {
                            p2.cumulPoints = p2.cumulPoints + this.CalculePointPerdantJujitsu(DC, p2);

                            //if (p2.judoka == this.participant1)
                            //{
                            //    p2.cumulPoints = p2.cumulPoints + this.score1;
                            //}
                            //else
                            //{
                            //    p2.cumulPoints = p2.cumulPoints + this.score2;
                            //}

                            
                        }
                    }
                }
            }
        }



        public IList<Feuille> Update(JudoData DC)
        {
            IList<Feuille> updated_combats = UpdateTableau(DC);

            if (this.etatJ1 == (int)EtatCombattantEnum.HansokuMakeX)
            {
                IList<Combat> valider_combats = DC.Deroulement.Combats.Where(o => o.phase == this.phase && o.vainqueur == null &&
                    (o.participant1 == this.participant1 || o.participant2 == this.participant1)).ToList();

                foreach (Combat c in valider_combats)
                {
                    if (c.participant1 == this.participant1)
                    {
                        c.Save(null, 0, 0, 0, 0, 0, 0, 0, 0,
                            EtatCombattantEnum.HansokuMakeX, c.etatJ2 == 0 ? EtatCombattantEnum.Normal : (EtatCombattantEnum)c.etatJ2, DC);
                        updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                    }

                    if (c.participant2 == this.participant1)
                    {
                        c.Save(null, 0, 0, 0, 0, 0, 0, 0, 0,
                            c.etatJ1 == 0 ? EtatCombattantEnum.Normal : (EtatCombattantEnum)c.etatJ1, EtatCombattantEnum.HansokuMakeX, DC);
                        updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                    }
                }
            }

            if (this.etatJ2 == (int)EtatCombattantEnum.HansokuMakeX)
            {
                IList<Combat> valider_combats = DC.Deroulement.Combats.Where(o => o.phase == this.phase && o.vainqueur == null &&
                    (o.participant1 == this.participant2 || o.participant2 == this.participant2)).ToList();

                foreach (Combat c in valider_combats)
                {
                    if (c.participant1 == this.participant2)
                    {
                        c.Save(null, 0, 0, 0, 0, 0, 0, 0, 0,
                            EtatCombattantEnum.HansokuMakeX, c.etatJ2 == 0 ? EtatCombattantEnum.Normal : (EtatCombattantEnum)c.etatJ2, DC);
                        updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                    }
                    if (c.participant2 == this.participant2)
                    {
                        c.Save(null, 0, 0, 0, 0, 0, 0, 0, 0,
                            c.etatJ1 == 0 ? EtatCombattantEnum.Normal : (EtatCombattantEnum)c.etatJ1, EtatCombattantEnum.HansokuMakeX, DC);
                        updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                    }
                }
            }


            if (DC.competition.type == (int)CompetitionTypeEnum.Shiai)
            {
                Participant p1 = DC.Deroulement.Participants.FirstOrDefault(o => o.judoka == this.participant1);
                if (p1 != null)
                {
                    vue_judoka vj = null;
                    using (TimedLock.Lock((DC.Participants.vjudokas as ICollection).SyncRoot))
                    {
                        vj = DC.Participants.vjudokas.FirstOrDefault(o => o.id == p1.judoka);
                    }

                    int count_combat = DC.Deroulement.GetNbCombatJudoka(vj.licence, DC);
                    //DC.Deroulement.Combats.Count(o => o.vainqueur != null && o.vainqueur > 0 && (o.participant1 == p1.judoka || o.participant2 == p1.judoka));
                    int points = vj.points - (DC.Deroulement.GetNbPointJudoka(vj.licence, DC));

                    if (points <= 0 || count_combat >= 5)
                    {
                        foreach (Combat c in DC.Deroulement.Combats.Where(o => o.vainqueur == null && (o.participant1 == p1.judoka || o.participant2 == p1.judoka)).ToList())
                        {
                            c.Save(0, 0, 0, 0, 0, 0, 0, 0, 0, EtatCombattantEnum.Normal, EtatCombattantEnum.Normal, DC);
                            updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                            //c.vainqueur = 0;
                            //updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                        }
                    }
                }

                Participant p2 = DC.Deroulement.Participants.FirstOrDefault(o => o.judoka == this.participant2);
                if (p2 != null)
                {
                    vue_judoka vj = null;
                    using (TimedLock.Lock((DC.Participants.vjudokas as ICollection).SyncRoot))
                    {
                        vj = DC.Participants.vjudokas.FirstOrDefault(o => o.id == p2.judoka);
                    }

                    int count_combat = DC.Deroulement.GetNbCombatJudoka(vj.licence, DC);
                    //DC.Deroulement.Combats.Count(o => o.vainqueur != null && o.vainqueur > 0 && (o.participant1 == pp21.judoka || o.participant2 == p2.judoka));
                    int points = vj.points - (DC.Deroulement.GetNbPointJudoka(vj.licence, DC));

                    if (points - p2.cumulPointsGRCH <= 0 || count_combat >= 5)
                    {
                        foreach (Combat c in DC.Deroulement.Combats.Where(o => o.vainqueur == null && (o.participant1 == p2.judoka || o.participant2 == p2.judoka)).ToList())
                        {
                            c.Save(0, 0, 0, 0, 0, 0, 0, 0, 0, EtatCombattantEnum.Normal, EtatCombattantEnum.Normal, DC);
                            updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                            //c.vainqueur = 0;
                            //updated_combats = updated_combats.Concat(c.Update(DC)).ToList();
                        }
                    }
                }
            }

            return updated_combats;
        }


        public IList<Feuille> UpdateTableau(JudoData DC)
        {
            //DialogControleur DC = Controles.DialogControleur.currentControleur;

            //Pas de vainqueur.
            if (this.vainqueur == null)
            {
                return new List<Feuille>();
            }

            Feuille feuille = DC.Deroulement.Feuilles.FirstOrDefault(o => o.combat == this.id);
            //Pas de feuille => on est dans un combat dans une poule, pas de update du tableau.
            if (feuille == null)
            {
                //DC.SaveChanges();
                return new List<Feuille>();
            }

            IList<Feuille> updated_combats = new List<Feuille>();

            //Mise à jour du tableau dans lequel on se trouve
            Feuille f1 = DC.Deroulement.Feuilles.FirstOrDefault(
                o => o.phase == this.phase && o.repechage == feuille.repechage && (o.ref1 == feuille.reference || o.ref2 == feuille.reference));
            if (f1 != null)
            {
                if (f1.ref1 == feuille.reference)
                {
                    if (f1.Combat1(DC) != null)
                    {
                        f1.Combat1(DC).participant1 = this.vainqueur;
                    }
                    updated_combats.Add(f1);
                }
                if (f1.ref2 == feuille.reference)
                {
                    if (f1.Combat1(DC) != null)
                    {
                        f1.Combat1(DC).participant2 = this.vainqueur;
                    }
                    updated_combats.Add(f1);
                }
            }

            //TODO BARRAGE
            if (feuille.repechage)
            {

            }


            if (!feuille.repechage)
            {
                //Mise à jour du tableau des repéchés
                System.Text.RegularExpressions.Regex myRegex = new Regex("^perdant." + feuille.reference + ".[0-9]+$");
                ICollection<Feuille> fps = (from r in DC.Deroulement.Feuilles.AsEnumerable()
                                            where r.phase == this.phase && r.repechage && (myRegex.IsMatch(r.ref1) || myRegex.IsMatch(r.ref2))
                                            select r).ToList();
                foreach (Feuille fp in fps)
                {
                    if (myRegex.IsMatch(fp.ref1))
                    {
                        int niveau = int.Parse(fp.ref1.Split('.').Last());
                        //recupération du combat
                        Combat c = DC.Deroulement.Combats.FirstOrDefault(
                            o => o.phase == this.phase && o.niveau == niveau && (o.participant1 == this.vainqueur || o.participant2 == this.vainqueur));
                        if (c != null)
                        {
                            //on met le perdant du combat
                            int? perdant = c.participant1 == c.vainqueur ? c.participant2 : c.participant1;
                            if (perdant == null)
                            {
                                perdant = 0;
                            }
                            if (fp.Combat1(DC) != null)
                            {
                                fp.Combat1(DC).participant1 = perdant;
                            }
                            updated_combats.Add(fp);
                            if (fp.Combat1(DC) != null)
                            {
                                updated_combats = updated_combats.Concat(fp.Combat1(DC).Update(DC)).ToList();
                            }
                        }
                    }
                    if (myRegex.IsMatch(fp.ref2))
                    {
                        int niveau = int.Parse(fp.ref2.Split('.').Last());
                        //recupération du combat
                        Combat c = DC.Deroulement.Combats.FirstOrDefault(
                            o => o.phase == this.phase && o.niveau == niveau && (o.participant1 == this.vainqueur || o.participant2 == this.vainqueur));
                        if (c != null)
                        {
                            //on met le perdant du combat
                            int? perdant = c.participant1 == c.vainqueur ? c.participant2 : c.participant1;
                            if (perdant == null)
                            {
                                perdant = 0;
                            }
                            if (fp.Combat1(DC) != null)
                            {
                                fp.Combat1(DC).participant2 = perdant;
                            }
                            updated_combats.Add(fp);
                            if (fp.Combat1(DC) != null)
                            {
                                updated_combats = updated_combats.Concat(fp.Combat1(DC).Update(DC)).ToList();
                            }
                        }
                    }
                }


                Feuille f2 = DC.Deroulement.Feuilles.FirstOrDefault(
                    o => o.phase == this.phase && o.repechage && (o.ref1 == this.reference || o.ref2 == this.reference));
                if (f2 != null)
                {
                    //on met le perdant du combat
                    if (f2.ref1 == feuille.reference)
                    {
                        int? perdant = this.vainqueur == this.participant1 ? this.participant2 : this.participant1;
                        if (perdant == null)
                        {
                            perdant = 0;
                        }
                        if (f2.Combat1(DC) != null)
                        {
                            f2.Combat1(DC).participant1 = perdant;
                        }
                        updated_combats.Add(f2);
                        if (f2.Combat1(DC) != null)
                        {
                            updated_combats = updated_combats.Concat(f2.Combat1(DC).Update(DC)).ToList();
                        }
                    }
                    if (f2.ref2 == feuille.reference)
                    {
                        int? perdant = this.vainqueur == this.participant1 ? this.participant2 : this.participant1;
                        if (perdant == null)
                        {
                            perdant = 0;
                        }
                        if (f2.Combat1(DC) != null)
                        {
                            f2.Combat1(DC).participant2 = perdant;
                        }

                        updated_combats.Add(f2);
                        if (f2.Combat1(DC) != null)
                        {
                            updated_combats = updated_combats.Concat(f2.Combat1(DC).Update(DC)).ToList();
                        }
                    }
                }
            }
            foreach (Feuille u_feuille in DC.Deroulement.Feuilles.Where(o => o.repechage))
            {
                Combat combat = u_feuille.Combat1(DC);
                if (combat == null || combat.vainqueur != null)
                {
                    continue;
                }
                if (combat.participant1 == 0 && combat.participant2 != 0 && combat.participant2 != null)
                {
                    combat.vainqueur = combat.participant2;
                    updated_combats = updated_combats.Concat(combat.Update(DC)).ToList();
                }

                if (combat.participant2 == 0 && combat.participant1 != 0 && combat.participant1 != null)
                {
                    combat.vainqueur = combat.participant1;
                    updated_combats = updated_combats.Concat(combat.Update(DC)).ToList();
                }

                if (combat.participant2 == 0 && combat.participant1 == 0)
                {
                    combat.vainqueur = 0;
                    updated_combats = updated_combats.Concat(combat.Update(DC)).ToList();
                }
            }
            //DC.SaveChanges();

            return updated_combats.Distinct().ToList();
        }


        public int CalculePointVainqueurJujitsu(JudoData DC, Participant pVainqueur)
        {
            int res = 0;
            Feuille feuille = DC.Deroulement.Feuilles.FirstOrDefault(o => o.combat == this.id);
            //Pas de feuille => on est dans un combat dans une poule
            if (feuille == null)
            {
                if (pVainqueur.judoka == this.participant1)
                {
                    res = this.score1 - this.score2;
                }
                else
                {
                    res = this.score2 - this.score1;
                }
            }
            else
            {
                if (pVainqueur.judoka == this.participant1)
                {
                    res = this.score1;
                }
                else
                {
                    res = this.score2;
                }
            }

            return res;
        }
        public int CalculePointPerdantJujitsu(JudoData DC, Participant pPerdant)
        {
            int res = 0;
            Feuille feuille = DC.Deroulement.Feuilles.FirstOrDefault(o => o.combat == this.id);
            //Pas de feuille => on est dans un combat dans une poule
            if (feuille == null)
            {
                res = 0;
            }
            else
            {
                if (pPerdant.judoka == this.participant1)
                {
                    res = this.score1;
                }
                else
                {
                    res = this.score2;
                }
            }

            return res;
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

            if (this.vainqueur == this.participant1)
            {
                scoreV = this.score1;
                penV = this.penalite1;
                scoreP = this.score2;
                penP = this.penalite2;
            }
            else if (this.vainqueur == this.participant2)
            {
                scoreV = this.score2;
                penV = this.penalite2;
                scoreP = this.score1;
                penP = this.penalite1;
            }
            /*
            if (this.etatJ1 == (int)EtatCombattantEnum.Normal && this.etatJ2 == (int)EtatCombattantEnum.Normal)
            {*/
            if (this.etatJ1 != (int)EtatCombattantEnum.HansokuMakeX && this.etatJ2 != (int)EtatCombattantEnum.HansokuMakeX &&
                this.etatJ1 != (int)EtatCombattantEnum.HansokuMakeH && this.etatJ2 != (int)EtatCombattantEnum.HansokuMakeH &&
                this.etatJ1 != (int)EtatCombattantEnum.Abandon && this.etatJ2 != (int)EtatCombattantEnum.Abandon &&
                this.etatJ1 != (int)EtatCombattantEnum.Medical && this.etatJ2 != (int)EtatCombattantEnum.Medical &&
                this.etatJ1 != (int)EtatCombattantEnum.Forfait && this.etatJ2 != (int)EtatCombattantEnum.Forfait)
            {
                //Victoire par IPPON 
                if ((scoreV / 100) - (scoreP / 100) >= 1)
                {
                    return 10;
                }

                ////Victoire par 2 WAZA-ARI
                if (scoreV >= 20)
                {
                    return 10;
                }

                /// Victoires par 3 Pénalités => 10 points
                if (penP >= 3 && penV >= 3)
                {
                    // Cas peu probable ... (3 shido de charque cote ...)
                    return scoreV / 10;
                }
                else if (penP >= 3)
                {
                    return 10;
                }

                // Victoire par WAZA-ARI => 1 point
                if ((scoreV / 10) - (scoreP / 10) >= 1)
                {
                    return scoreV / 10;
                    //return 7;
                }

                // Par defaut (decision, etc.) => 0 points
                return 0;
            }
            else
            {
                // Combat gagne par A/F/M/H ou X ==> le gagnant a forcement 10 points
                return 10;
            }
        }

        private int CalculeScorePerdant()
        {
            int scoreV = 0;
            int scoreP = 0;

            int penV = 0;
            int penP = 0;

            if (this.vainqueur == this.participant1)
            {
                scoreV = this.score1;
                penV = this.penalite1;
                scoreP = this.score2;
                penP = this.penalite2;
            }
            else if (this.vainqueur == this.participant2)
            {
                scoreV = this.score2;
                penV = this.penalite2;
                scoreP = this.score1;
                penP = this.penalite1;
            }

            // En cas de Hansokumake X, le perdant perd toutes ses valeurs
            if (this.etatJ1 == (int)EtatCombattantEnum.HansokuMakeX || this.etatJ2 == (int)EtatCombattantEnum.HansokuMakeX)
            {
                return 0;
            }

            // Perdu sur 3 Pénalités, A/M/F ou H: il garde le benefice s'il a un Waza-Ari de difference avec le gagnant
            if (this.etatJ1 != (int)EtatCombattantEnum.HansokuMakeH || this.etatJ2 != (int)EtatCombattantEnum.HansokuMakeH ||
            this.etatJ1 != (int)EtatCombattantEnum.Abandon || this.etatJ2 != (int)EtatCombattantEnum.Abandon ||
            this.etatJ1 != (int)EtatCombattantEnum.Medical || this.etatJ2 != (int)EtatCombattantEnum.Medical ||
            this.etatJ1 != (int)EtatCombattantEnum.Forfait || this.etatJ2 != (int)EtatCombattantEnum.Forfait ||
            penP >= 3)
            {
                // Le perdant doit avoir un Waza-ari de difference avec le gagnant
                if (scoreP - scoreV >= 1)
                {
                    return 1;
                }
            }

            // Victoire par 2 WAZA-ARI => il garde le benefice de son eventuel Waza-Ari
            if (scoreV >= 20)
            {
                return scoreP / 10;
            }

            // Par defaut, le perdant a Zero dans tous les autres cas
            // victoire par Ippon, par Waza-Ari
            return 0;
        }

        /// <summary>
        /// Etablir le nombre de point de relation grade/championnat d'un combat
        /// </summary>
        /// <param name="DC"></param>

        public int CalculeScoreGRCH(JudoData DC, int? participant)
        {
            if (DC.competition.type == (int)CompetitionTypeEnum.Equipe)
            {
                return 0;
            }

            try
            {
                // Flag pour indiquer si les conditions du combat donne 0 point
                bool zero = false;
                zero = zero || participant == null || participant == 0;     // Participant inconnu
                zero = zero || this.vainqueur == null;                      // pas de vainqueur identifie
                zero = zero || this.vainqueur != participant;               // Si le judoka n'est pas le vainqueur (perdant = 0 pts)
                if (DC.competition.discipline  == CompetitionDisciplineEnum.Judo.ToString2())
                {
                    zero = zero || (this.score1 < 10 && this.score2 < 10);      // Si aucun Waza-Ari ou Ippon dans les scores
                }
                else
                {

                }
                    

                // DGR 2022-03-26: On ne peut pas eliminer de suite le resultat car l'un des judokas peut avoir un Waza-Ari
                // zero = zero || this.etatJ1 != (int)EtatCombatEnum.Normal;   // Si le 1er judoka a perdu sur A/M/H/X
                // zero = zero || this.etatJ2 != (int)EtatCombatEnum.Normal;   // Si le 2nd a perdu sur A/M/H/X

                // On sait deja que le combat ne donne aucun point au participant
                if (zero)
                {
                    return 0;
                }

                // Recupere les donnees des judokas pour verifier les grades respectifs
                // On ne peut pas marquer sur un grade inferieur ou si le grade est inferieur a Marron
                Judoka j1 = null;
                Judoka j2 = null;

                using (TimedLock.Lock((DC.Participants.vjudokas as ICollection).SyncRoot))
                {
                    j1 = DC.Participants.Judokas.FirstOrDefault(o => o.id == this.participant1);
                    j2 = DC.Participants.Judokas.FirstOrDefault(o => o.id == this.participant2);
                }

                Ceintures grademin = DC.Categories.Grades.FirstOrDefault(o => o.remoteId == "MA");
                if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
                {
                    zero = zero || ((j1?.ceinture ?? 0) < grademin.id || (j2?.ceinture ?? 0) < grademin.id);  //SI GRADE INFERIEUR A MARRON
                }
                else
                {
                    if (participant == participant1)
                    {
                        zero = zero || ((j1?.ceinture ?? 0) < grademin.id);  //SI au moins le grade MARRON
                    }
                    else
                    {
                        zero = zero || ((j2?.ceinture ?? 0) < grademin.id);  //SI au moins le grade MARRON
                    }
                }


                /*if (DC.competition.discipline != CompetitionDisciplineEnum.Judo.ToString2()) //discipline jujitsu
                {
                    if(this.vainqueur == participant) //si le participant est le vainqueur
                    {
                        if (participant == participant1)
                        {
                            if ((j2?.ceinture ?? 0) < (j1?.ceinture ?? 0)) //pas de points si l'adversaire (perdant) possède un grade inférieur au vainqueur
                            {
                                zero = true;
                            }
                        }
                        else
                        {
                            if ((j1?.ceinture ?? 0) < (j2?.ceinture ?? 0)) //pas de points si l'adversaire (perdant) possède un grade inférieur au vainqueur
                            {
                                zero = true;
                            }
                        }
                    }
                }*/
       

                // DGR 2022-03-26 Vu avec Eric Fauroux, en shiai on ne fait pas ce controle pour permettre le deroulement sur les 2D/3D (manque de participant)
                if (DC.competition.type != (int)CompetitionTypeEnum.Shiai)
                {
                    // A cette etape de code, le participant est forcement le vainqueur (cas contraire elimine au debut du code)
                    zero = zero || (j1?.id == participant && j1?.ceinture > j2?.ceinture);  //SI GRADE INFERIEUR
                    zero = zero || (j2?.id == participant && j2?.ceinture > j1?.ceinture);  //SI GRADE INFERIEUR
                }

                if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
                {
                    // Les comnbats de barrage en individuelle ne marque pas de points
                    zero = zero || (this.reference.StartsWith("3.")); //SI COMBAT DE BARRAGE
                    zero = zero || (this.reference.StartsWith("5.")); //SI COMBAT DE BARRAGE
                    zero = zero || (this.reference.StartsWith("7.")); //SI COMBAT DE BARRAGE
                }


                List<Combat> combats = new List<Combat>();
                using (TimedLock.Lock((DC.Deroulement.Combats as ICollection).SyncRoot))
                {
                    combats = DC.Deroulement.Combats.Where(o => o.participant1 == participant || o.participant2 == participant).ToList();
                }

                /********************************************************************************************************************
                 * 
                 * Règle SUPPRIMER APPEL DU 01-03-2017 ERIC-FAUROUX VU AVEC JC SENAUT
                 * 
                //SI LES DEUX PARTICIPANTS SE SONT DEJA RENCONTRE DANS UNE PHASE PRECEDENTE
                zero = zero || (combats.Count(o => o.participant1 == this.participant1 && o.participant2 == this.participant2 &&
                        o.phase < this.phase) != 0); 

                //SI LES DEUX PARTICIPANTS SE SONT DEJA RENCONTRE DANS LA PHASE
                zero = zero || (this.reference.StartsWith("2.") && combats.Count(o =>
                        o.participant1 == this.participant1 && o.participant2 == this.participant2 &&
                        o.reference != this.reference && o.phase == this.phase) != 0); 
                * 
                *********************************************************************************************************************/

                // Conditions de grade ou de barrage non respectees: pas de points marques
                if (zero)
                {
                    return 0;
                }

                // Verifie la difference de score
                int score11 = 0;    // Le score du participant
                int score22 = 0;    // Le score de son advesaire

                if (participant == participant1)
                {
                    score11 = this.score1;
                    score22 = this.score2;
                }
                else
                {
                    score11 = this.score2;
                    score22 = this.score1;
                }


                if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
                {
                    if (score11 >= 100 || score11 >= 20)
                    {
                        // Le participant a marquer un Ippon ou 2 waza-ari > 10 pts
                        return 10;
                    }
                    else if (score11 - score22 >= 10)
                    {
                        // Il y a une difference de score de au moins un Waza-Ari
                        return 7;
                    }
                }
                else
                {
                    ScoresJujitsu scoresJson = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoresJujitsu>(this.scoresJujitsu);
                    if (scoresJson is null) scoresJson = new ScoresJujitsu();

                    if (DC.competition.discipline == CompetitionDisciplineEnum.JujitsuCombat.ToString2())
                    {
                        if (participant == participant1)
                        {
                            if(this.etatJ1 == (int)EtatCombattantEnum.HansokuMakeX) //perd ses point de la RGC
                            {
                                return -1;
                            }

                            if ((scoresJson.ippon1_1_1 > 0 || scoresJson.ippon1_1_2 > 0) && (scoresJson.ippon1_2_1 > 0 || scoresJson.ippon1_2_2 > 0) &&
                                (scoresJson.ippon1_3_1 > 0 || scoresJson.ippon1_3_2 > 0))
                            {
                                return 10; //donne 10 points pour un full ippon (judoka1)
                            }
                            else
                            {
                                int nbPartiesValidees = 0;
                                if (scoresJson.ippon1_1_1 > 0 || scoresJson.ippon1_1_2 > 0)
                                {
                                    nbPartiesValidees++;
                                }
                                if (scoresJson.ippon1_2_1 > 0 || scoresJson.ippon1_2_2 > 0)
                                {
                                    nbPartiesValidees++;
                                }
                                if (scoresJson.ippon1_3_1 > 0 || scoresJson.ippon1_3_2 > 0)
                                {
                                    nbPartiesValidees++;
                                }
                                if(nbPartiesValidees >= 2)
                                {
                                    return 7; //7 points pour 2 parties validées
                                }
                            }
                        }
                        else
                        {
                            if (this.etatJ2 == (int)EtatCombattantEnum.HansokuMakeX) //perd ses point de la RGC
                            {
                                return -1;

                            }
                            if ((scoresJson.ippon2_1_1 > 0 || scoresJson.ippon2_1_2 > 0) && (scoresJson.ippon2_2_1 > 0 || scoresJson.ippon2_2_2 > 0) &&
                                (scoresJson.ippon2_3_1 > 0 || scoresJson.ippon2_3_2 > 0))
                            {
                                return 10; //donne 10 points pour un full ippon (judoka2)
                            }
                            else
                            {
                                int nbPartiesValidees = 0;
                                if (scoresJson.ippon2_1_1 > 0 || scoresJson.ippon2_1_2 > 0)
                                {
                                    nbPartiesValidees++;
                                }
                                if (scoresJson.ippon2_2_1 > 0 || scoresJson.ippon2_2_2 > 0)
                                {
                                    nbPartiesValidees++;
                                }
                                if (scoresJson.ippon2_3_1 > 0 || scoresJson.ippon2_3_2 > 0)
                                {
                                    nbPartiesValidees++;
                                }
                                if (nbPartiesValidees >= 2)
                                {
                                    return 7; //7 points pour 2 parties validées
                                }
                            }
                        }
                    }
                    else //Ne Waza
                    {
                        if (participant == participant1)
                        {
                            if (scoresJson.ippon1 > 0)
                            {
                                return 10; //donne 10 points si ippon (judoka1)
                            }
                        }
                        else
                        {
                            if (scoresJson.ippon2 > 0)
                            {
                                return 10; //donne 10 points si ippon (judoka2)
                            }
                        }
                    }
                        
                }

               

                return 0;
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
                return 0;
            }
        }

        public string GetScoreVainqueur(JudoData DC)
        {
            if (virtuel || this.vainqueur == null)
            {
                return "";
            }

            bool isEquipe = DC.competition.type == (int)CompetitionTypeEnum.Equipe;

            int nbV = 0;
            int score = 0;
            int pen = 0;

            if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
            {
                if (this.vainqueur == this.participant1)
                {
                    nbV = this.nbVictoire1;
                    score = this.score1;
                    if (!isEquipe && score < 100 && (!int.TryParse(GetPenalites(2), out pen) || pen >= 3))
                    {
                        score += 100;
                    }
                    //else if(!isEquipe && score < 100 && score >= 20)
                    //{
                    //    score += 100;
                    //    score -= 20;
                    //}
                }
                else if (this.vainqueur == this.participant2)
                {
                    nbV = this.nbVictoire2;
                    score = this.score2;
                    if (!isEquipe && score < 100 && (!int.TryParse(GetPenalites(1), out pen) || pen >= 3))
                    {
                        score += 100;
                    }
                    //else if (!isEquipe && score < 100 && score >= 20)
                    //{
                    //    score += 100;
                    //    score -= 20;
                    //}
                }
                else
                {
                    return "";
                }

            if (isEquipe)
            {
                return nbV + "v." + score.ToString("00");
            }

            // string res = score.ToString("000");
            string res = (score / 10).ToString("00");

                //string res = (score >= 100 ? (score / 100).ToString() : "0");
                //score = score % 100;
                //res += (score >= 10 ? (score / 10).ToString() : "0");
                //score = score % 10;
                //res += score;
                return res;
            }
            else
            {
                string res = "";
                CompetitionDisciplineEnum discipline = CompetitionDisciplineEnum_Extension.ByString2(DC.competition.discipline);
                if (this.vainqueur == this.participant1)
                {
                    res = this.GetScoreJujitsu(discipline, DC, 1);
                }
                else if (this.vainqueur == this.participant2)
                {
                    res = this.GetScoreJujitsu(discipline, DC, 2);
                }
                else
                {
                    return "";
                }
                return res;
            }
           
        }

        public string GetScorePerdant(JudoData DC)
        {
            if (virtuel || this.vainqueur == null)
            {
                return "";
            }

            int score = 0;
            int nbV = 0;

            if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
            {
                if (this.vainqueur == this.participant2)
                {
                    score = this.score1;
                    nbV = this.nbVictoire1;
                }
                else if (this.vainqueur == this.participant1)
                {
                    nbV = this.nbVictoire2;
                    score = this.score2;
                }
                else
                {
                    return "";
                }

            if (DC.competition.type == (int)CompetitionTypeEnum.Equipe)
            {
                return nbV + "v." + score.ToString("00");
            }

            // string res = score.ToString("000");
            string res = (score / 10).ToString("00");

                //string res = (score >= 100 ? (score / 100).ToString() : "0");
                //score = score % 100;
                //res += (score >= 10 ? (score / 10).ToString() : "0");
                //score = score % 10;
                //res += score;
                return res;

            }
            else
            {
                CompetitionDisciplineEnum discipline = CompetitionDisciplineEnum_Extension.ByString2(DC.competition.discipline);
                if (this.vainqueur == this.participant2)
                {
                    return this.GetScoreJujitsu(discipline, DC, 1);
                }
                else if (this.vainqueur == this.participant1)
                {
                    return this.GetScoreJujitsu(discipline, DC, 2);
                }
                else
                {
                    return "";
                }
            }
           
        }

        public int? getVainqueur(EtatCombattantEnum etat1, EtatCombattantEnum etat2, JudoData DC)
        {
            int? res = null;
            if (DC.competition.discipline == CompetitionDisciplineEnum.Judo.ToString2())
            {
                if (etat1 == EtatCombattantEnum.Decision)
                {
                    res = this.participant1;
                }
                else if (etat2 == EtatCombattantEnum.Decision)
                {
                    res = this.participant2;
                }
                ////----
                /*else if(this.penalite1 == 3 && this.penalite2 == 3)
                {
                    this.vainqueur = null;
                }*/
                ////----
                else if (this.penalite1 == 3)
                {
                    res = this.participant2;
                }
                else if (this.penalite2 == 3)
                {
                    res = this.participant1;
                }
                else if (this.score1 > this.score2)
                {
                    res = this.participant1;
                }
                else if (this.score1 < this.score2)
                {
                    res = this.participant2;
                }
                else if (this.penalite1 > this.penalite2)
                {
                    res = this.participant2;
                }
                else if (this.penalite1 < this.penalite2)
                {
                    res = this.participant1;
                }
            }
            else
            {
                ScoresJujitsu scoresJson = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoresJujitsu>(this.scoresJujitsu);
                if (scoresJson is null) scoresJson = new ScoresJujitsu();

                if (DC.competition.discipline == CompetitionDisciplineEnum.JujitsuCombat.ToString2())
                {
                    int nbPartiesValideesJujitsuCombat1 = scoresJson.NbPartiesValides(1);
                    int nbPartiesValideesJujitsuCombat2 = scoresJson.NbPartiesValides(2);
                    int nbIppons1 = scoresJson.NbIppons(1);
                    int nbIppons2 = scoresJson.NbIppons(2);
                    int nbPointsPenalites1 = scoresJson.GetPointsPenalitesCombat(1);
                    int nbPointsPenalites2 = scoresJson.GetPointsPenalitesCombat(2);

                    if (nbPointsPenalites1 == 6)
                    {
                        res = this.participant2;
                    }
                    else if (nbPointsPenalites2 == 6)
                    {
                        res = this.participant1;
                    }
                    else if (this.score1 > this.score2) //si full ippon => 100 points contre 0 donc le vainqueur sera celui qui aura 100 points
                    {
                        res = this.participant1;
                    }
                    else if (this.score1 < this.score2)
                    {
                        res = this.participant2;
                    }
                    else if (nbPartiesValideesJujitsuCombat1 > nbPartiesValideesJujitsuCombat2)//Jujitsu combat
                    {
                        res = this.participant1;
                    }
                    else if (nbPartiesValideesJujitsuCombat1 < nbPartiesValideesJujitsuCombat2)//Jujitsu combat
                    {
                        res = this.participant2;
                    }
                    else if (nbIppons1 > nbIppons2)//Jujitsu combat
                    {
                        res = this.participant1;
                    }
                    else if (nbIppons1 < nbIppons2)//Jujitsu combat
                    {
                        res = this.participant2;
                    }
                }
                else
                {
                    int avantages1 = 0;
                    int avantages2 = 0;
                    avantages1 = scoresJson.avantages1;
                    if (scoresJson.penalites2 >= 2)// 2ème pénalités pour l'adversaire donne un avantage supplémentaire
                    {
                        avantages1 += 1;
                    }
                    avantages2 = scoresJson.avantages2;
                    if (scoresJson.penalites1 >= 2)// 2ème pénalités pour l'adversaire donne un avantage supplémentaire
                    {
                        avantages2 += 1;
                    }


                    int maxPenalites = 4;
                    vue_epreuve ep = DC.Organisation.vepreuves.FirstOrDefault(o => o.id == this.epreuve);
                    if (!(ep is null))
                    {
                        CategorieAge cateAge = DC.Categories.CAges.FirstOrDefault(o => o.id == ep.categorieAge);
                        if (!(cateAge is null))
                        {
                            if (cateAge.remoteId == "B" || cateAge.remoteId == "M") //Benjamin ou minime
                            {
                                maxPenalites = 6;
                            }
                        }
                    }

                    if (etat1 == EtatCombattantEnum.Decision)
                    {
                        res = this.participant1;
                    }
                    else if (etat2 == EtatCombattantEnum.Decision)
                    {
                        res = this.participant2;
                    }
                    else if (this.penalite1 >= maxPenalites)
                    {
                        res = this.participant2;
                    }
                    if (this.penalite2 >= maxPenalites)
                    {
                        res = this.participant1;
                    }
                    else if (this.score1 > this.score2)
                    {
                        res = this.participant1;
                    }
                    else if (this.score1 < this.score2)
                    {
                        res = this.participant2;
                    }
                    else if (avantages1 > avantages2)//NE WAZA
                    {
                        res = this.participant1;
                    }
                    else if (avantages1 < avantages2)//NE WAZA
                    {
                        res = this.participant2;
                    }
                    else if (this.penalite1 > this.penalite2)
                    {
                        res = this.participant2;
                    }
                    else if (this.penalite1 < this.penalite2)
                    {
                        res = this.participant1;
                    }

                }

            }

            return res;
        }

        public string GetScoreJujitsu(CompetitionDisciplineEnum discipline, JudoData DC, int judoka = -1)
        {
            string res1 = "";
            string res2 = "";

            res1 = this.score1.ToString();

            int maxPenalites = 6;
            if (discipline == CompetitionDisciplineEnum.JujitsuNeWaza)
            {
                maxPenalites = 4;
                vue_epreuve ep = DC.Organisation.vepreuves.FirstOrDefault(o => o.id == this.epreuve);
                if (!(ep is null))
                {
                    CategorieAge cateAge = DC.Categories.CAges.FirstOrDefault(o => o.id == ep.categorieAge);
                    if (!(cateAge is null))
                    {
                        if (cateAge.remoteId == "B" || cateAge.remoteId == "M") //Benjamin ou minime
                        {
                            maxPenalites = 6;
                        }
                    }
                }
            }

            if (this.etatJ1 != (int)EtatCombattantEnum.Normal)
            {
                res1 += "." + this.GetPenalites(judoka);
                //if (this.etatJ1 == (int)EtatCombattantEnum.HansokuMakeH)
                //{
                //    res1 += ".H";
                //}
                //else if (this.etatJ1 == (int)EtatCombattantEnum.HansokuMakeX)
                //{
                //    res1 += ".H";
                //}
                //else
                //{
                //    res1 += "." + this.etatJ1.ToString().Substring(0, 1);
                //}
            }
            else
            {
                ScoresJujitsu scoresJson = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoresJujitsu>(this.scoresJujitsu);
                if (scoresJson is null) scoresJson = new ScoresJujitsu();

                if(discipline == CompetitionDisciplineEnum.JujitsuCombat)
                {
                    if ((scoresJson.shido1 + (scoresJson.chui1 * 3)) >= maxPenalites)
                    {
                        res1 += ".H";
                    }
                    else if (scoresJson.IsFullIppon(1) && this.etatJ2 == (int)EtatCombattantEnum.Normal)
                    {
                        res1 += "FI";
                    }
                }
                else if (discipline == CompetitionDisciplineEnum.JujitsuNeWaza)
                {
                    if (scoresJson.penalites1 >= maxPenalites)
                    {
                        res1 += ".H";
                    }
                    else
                    {
                        int avantages = 0;
                        avantages = scoresJson.avantages1;
                        if (scoresJson.penalites2 >= 2)// 2ème pénalités pour l'adversaire donne un avantage supplémentaire
                        {
                            avantages += 1;
                        }

                        if (avantages > 0 && this.score1 != 100)
                        {
                            res1 += "." + avantages;
                        }
                    }

                    
                }
                
            }

            res2 = this.score2.ToString();

            if (this.etatJ2 != (int)EtatCombattantEnum.Normal)
            {
                res2 += "." + this.GetPenalites(judoka);
                //if (this.etatJ2 == (int)EtatCombattantEnum.HansokuMakeH)
                //{
                //    res2 += ".H";
                //}
                //else if (this.etatJ2 == (int)EtatCombattantEnum.HansokuMakeX)
                //{
                //    res2 += ".H";
                //}
                //else
                //{
                //    res2 += "." + this.etatJ2.ToString().Substring(0, 1);
                //}
            }
            else
            {
                ScoresJujitsu scoresJson = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoresJujitsu>(this.scoresJujitsu);
                if (scoresJson is null) scoresJson = new ScoresJujitsu();

                if (discipline == CompetitionDisciplineEnum.JujitsuCombat)
                {
                    if ((scoresJson.shido2 + (scoresJson.chui2 * 3)) >= maxPenalites)
                    {
                        res2 += ".H";
                    }
                    else if (scoresJson.IsFullIppon(2) && this.etatJ1 == (int)EtatCombattantEnum.Normal)
                    {
                        res2 += "FI";
                    }
                }
                else if (discipline == CompetitionDisciplineEnum.JujitsuNeWaza)
                {
                    if (scoresJson.penalites2 >= maxPenalites)
                    {
                        res2 += ".H";
                    }
                    else
                    {
                        int avantages = 0;
                        avantages = scoresJson.avantages2;
                        if (scoresJson.penalites1 >= 2)// 2ème pénalités pour l'adversaire donne un avantage supplémentaire
                        {
                            avantages += 1;
                        }

                        if (avantages > 0 && this.score2 != 100)
                        {
                            res2 += "." + avantages;
                        }
                    }

                    
                }
            }


            if (judoka == 1)
            {
                return res1;
            }
            else if (judoka == 2)
            {
                return res2;
            }
            else
            {
                return res1 + " " + res2;
            }

        }

        public string GetPenalites(int judoka)
        {
            string pen1 = "";

            int etat = judoka == 1 ? this.etatJ1 : this.etatJ2;

            switch (etat)
            {
                case (int)EtatCombattantEnum.Abandon:
                    pen1 += "A";
                    break;
                case (int)EtatCombattantEnum.Medical:
                    pen1 += "M";
                    break;
                case (int)EtatCombattantEnum.Forfait:
                    pen1 += "F";
                    break;
                case (int)EtatCombattantEnum.HansokuMakeH:
                    pen1 += "H";
                    break;
                case (int)EtatCombattantEnum.HansokuMakeX:
                    pen1 += "X";
                    break;
                default:
                    pen1 += (judoka == 1 ? this.penalite1.ToString() : this.penalite2.ToString());
                    break;
            }

            return pen1;
        }

        public string GetPenaliteVainqueur()
        {
            if (virtuel || this.vainqueur == null)
            {
                return "";
            }

            if (this.vainqueur == this.participant1)
            {
                return "-" + GetPenalites(1);
            }
            else if (this.vainqueur == this.participant2)
            {
                return "-" + GetPenalites(2);
            }
            else
            {
                return "";
            }
        }

        public string GetPenalitePerdant()
        {
            if (virtuel || this.vainqueur == null)
            {
                return "";
            }

            if (this.vainqueur == this.participant2)
            {
                return "-" + GetPenalites(1);
            }
            else if (this.vainqueur == this.participant1)
            {
                return "-" + GetPenalites(2);
            }
            else
            {
                return "";
            }
        }

        public string GetKinzaVainqueur()
        {
            if (virtuel || this.vainqueur == null)
            {
                return "";
            }

            if (this.vainqueur == this.participant1)
            {
                return this.kinza1.ToString();
            }
            else if (this.vainqueur == this.participant2)
            {
                return this.kinza2.ToString();
            }
            else
            {
                return "";
            }
        }

        public string GetKinzaPerdant()
        {
            if (virtuel || this.vainqueur == null)
            {
                return "";
            }

            if (this.vainqueur == this.participant1)
            {
                return this.kinza2.ToString();
            }
            else if (this.vainqueur == this.participant2)
            {
                return this.kinza1.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Lecture des Combats
        /// </summary>
        /// <param name="xelement">élément décrivant les Combats</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Combats</returns>

        public static ICollection<Combat> LectureCombats(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Combat> combats = new List<Combat>();
            foreach (XElement xcombat in xelement.Descendants(ConstantXML.Combat))
            {
                Combat combat = new Combat();
                combat.LoadXml(xcombat);
                combats.Add(combat);
            }

            return combats;
        }

    }
}
