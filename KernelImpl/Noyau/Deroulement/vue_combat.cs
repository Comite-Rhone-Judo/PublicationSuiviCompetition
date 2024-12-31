
using System;
using System.Collections;
using System.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    public partial class vue_combat
    {
        public int combat_id { get; set; }
        public int combat_numero { get; set; }
        public string combat_reference { get; set; }
        public int combat_score1 { get; set; }
        public int combat_score2 { get; set; }
        public int combat_penalite1 { get; set; }
        public int combat_penalite2 { get; set; }
        public DateTime combat_programmation { get; set; }
        public DateTime combat_debut { get; set; }
        public DateTime combat_fin { get; set; }
        public int combat_etat { get; set; }
        public Nullable<int> combat_vaiqueur { get; set; }
        public int combat_tapis { get; set; }
        public int combat_groupe { get; set; }
        public string combat_details { get; set; }
        public int combat_niveau { get; set; }
        public int combat_temps { get; set; }
        public int combat_tempsRecup { get; set; }

        public string combat_discipline { get; set; }
        public int judoka1_id { get; set; }
        public string judoka1_club { get; set; }
        public string judoka1_licence { get; set; }
        public string judoka1_nom { get; set; }
        public string judoka1_prenom { get; set; }
        public int judoka2_id { get; set; }
        public string judoka2_club { get; set; }
        public string judoka2_licence { get; set; }
        public string judoka2_nom { get; set; }
        public string judoka2_prenom { get; set; }
        public int phase_id { get; set; }
        public string phase_libelle { get; set; }
        public int phase_type { get; set; }
        public int phase_etat { get; set; }
        public int cateAge_id { get; set; }
        public string cateAge_nom { get; set; }
        public int catePoids_id { get; set; }
        public string catePoids_nom { get; set; }
        public int competition_id { get; set; }
        public string competition_nom { get; set; }
        public Nullable<DateTime> competition_date { get; set; }
        public string competition_lieu { get; set; }
        public int epreuve_id { get; set; }
        public string epreuve_nom { get; set; }
        public int epreuve_poidsMin { get; set; }
        public int epreuve_poidsMax { get; set; }
        public int epreuve_ceintureMin { get; set; }
        public int epreuve_ceintureMax { get; set; }
        public int epreuve_anneeMin { get; set; }
        public int epreuve_anneeMax { get; set; }
        public Nullable<DateTime> epreuve_debut { get; set; }
        public Nullable<DateTime> epreuve_fin { get; set; }
        public string phase_name { get; set; }

        private string _epreuve_sexe;
        public string epreuve_sexe
        {
            get
            {
                return _epreuve_sexe;
            }
            set
            {
                _epreuve_sexe = value;
                _epreuve_sexeEnum = new EpreuveSexe(_epreuve_sexe);
            }
        }

        private EpreuveSexe _epreuve_sexeEnum;
        public EpreuveSexe epreuve_sexeEnum
        {
            get
            {
                return _epreuve_sexeEnum;
            }
            set
            {
                _epreuve_sexeEnum = value;
                _epreuve_sexe = _epreuve_sexeEnum.ToString();
            }
        }

        public int judoka1_id1 { get; set; }
        public string judoka1_licence1 { get; set; }
        public string judoka1_nom1 { get; set; }
        public string judoka1_prenom1 { get; set; }
        public string judoka1_club1 { get; set; }
        public int judoka2_id1 { get; set; }
        public string judoka2_licence1 { get; set; }
        public string judoka2_nom1 { get; set; }
        public string judoka2_prenom1 { get; set; }
        public string judoka2_club1 { get; set; }

        public vue_combat(Combat combat, JudoData DC)
        {
            combat_id = combat.id;
            combat_numero = combat.numero;
            combat_score1 = combat.score1;
            combat_score2 = combat.score2;
            combat_penalite1 = combat.penalite1;
            combat_penalite2 = combat.penalite2;
            combat_etat = combat.etat;
            combat_niveau = combat.niveau;
            combat_tapis = (int)combat.tapis;
            combat_groupe = (int)combat.groupe;
            combat_vaiqueur = combat.vainqueur;
            combat_reference = combat.reference;
            combat_details = combat.details;
            combat_programmation = combat.programmation;
            combat_debut = combat.debut;
            combat_fin = combat.fin;
			combat_discipline = combat.discipline;

            // Ajout de la lecture des donnees de phase et d'epreuve


            if ((CompetitionTypeEnum)DC.competition.type != CompetitionTypeEnum.Equipe)
            {
                Participants.Judoka judoka1 = null;
                using (TimedLock.Lock((DC.Participants.Judokas as ICollection).SyncRoot))
                {
                    judoka1 = DC.Participants.Judokas.FirstOrDefault(o => o.id == combat.participant1);
                }
                if (judoka1 != null)
                {
                    judoka1_id1 = judoka1.id;
                    judoka1_licence1 = judoka1.licence;
                    judoka1_nom1 = judoka1.nom;
                    judoka1_prenom1 = judoka1.prenom;
                    judoka1_club1 = judoka1.club;
                }

                Participants.Judoka judoka2 = null;
                using (TimedLock.Lock((DC.Participants.Judokas as ICollection).SyncRoot))
                {
                    judoka2 = DC.Participants.Judokas.FirstOrDefault(o => o.id == combat.participant2);
                }

                if (judoka2 != null)
                {
                    judoka2_id1 = judoka2.id;
                    judoka2_licence1 = judoka2.licence;
                    judoka2_nom1 = judoka2.nom;
                    judoka2_prenom1 = judoka2.prenom;
                    judoka2_club1 = judoka2.club;
                }
            }
            else
            {
                Participants.Equipe E1 = DC.Participants.Equipes.FirstOrDefault(o => o.id == combat.participant1);
                if (E1 != null)
                {
                    judoka1_id1 = E1.id;
                    judoka1_nom1 = E1.libelle;
                }

                Participants.Equipe E2 = DC.Participants.Equipes.FirstOrDefault(o => o.id == combat.participant2);
                if (E2 != null)
                {
                    judoka2_id1 = E2.id;
                    judoka2_nom1 = E2.libelle;
                }
            }
        }
    }
}
