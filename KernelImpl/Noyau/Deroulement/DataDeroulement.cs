
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    public class DataDeroulement
    {
        private IList<Combat> _combats = new List<Combat>();
        public IList<Combat> Combats { get { return _combats; } }

        private IList<Rencontre> _rencontres = new List<Rencontre>();
        public IList<Rencontre> Rencontres { get { return _rencontres; } }

        private IList<Feuille> _feuilles = new List<Feuille>();
        public IList<Feuille> Feuilles { get { return _feuilles; } }


        private IList<Phase_Decoupage> _decoupages = new List<Phase_Decoupage>();
        public IList<Phase_Decoupage> Decoupages { get { return _decoupages; } }

        private IList<Groupe_Combats> _groupes = new List<Groupe_Combats>();
        public IList<Groupe_Combats> Groupes { get { return _groupes; } }

        private IList<Phase> _phases = new List<Phase>();
        public IList<Phase> Phases { get { return _phases; } }

        private IList<Poule> _poules = new List<Poule>();
        public IList<Poule> Poules { get { return _poules; } }

        private IList<Participant> _participants = new List<Participant>();
        public IList<Participant> Participants { get { return _participants; } }

        private IList<vue_groupe> _vgroupes = new List<vue_groupe>();
        public IList<vue_groupe> vgroupes { get { return _vgroupes; } }

        private IList<vue_combat> _vcombats = new List<vue_combat>();
        public IList<vue_combat> vcombats { get { return _vcombats; } }


        public void clear_deroulement()
        {
            _phases.Clear();
            _groupes.Clear();
            _decoupages.Clear();
            _poules.Clear();
            _participants.Clear();
            _vgroupes.Clear();
        }

        public void clear_combats(Combat combat_en_cours = null)
        {
            int index = 0;
            while (index != _combats.Count)
            {
                if (combat_en_cours == null || _combats.ElementAt(index).id != combat_en_cours.id)
                {
                    List<Rencontre> rencontres = _rencontres.Where(o => o.combat == _combats.ElementAt(index).id).ToList();
                    while (rencontres.Count > 0)
                    {
                        Rencontre rencontre = rencontres.FirstOrDefault();
                        rencontres.Remove(rencontre);
                        _rencontres.Remove(rencontre);
                    }
                    Feuille feuille = _feuilles.FirstOrDefault(o => o.combat == _combats.ElementAt(index).id);
                    _feuilles.Remove(feuille);

                    vue_combat v_combat = vcombats.FirstOrDefault(o => o.combat_id == _combats.ElementAt(index).id);
                    vcombats.Remove(v_combat);

                    _combats.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            //_combats.Clear();
            //_rencontres.Clear();
            //_feuilles.Clear();
            //vcombats.Clear();
        }


        public IEnumerable<Participant> ListeParticipant1(int epreuve, JudoData DC)
        {
            IEnumerable<int> phases = DC.Deroulement.Phases.Where(o => o.epreuve == epreuve && o.suivant != 0).Select(o => o.id).Distinct();
            return _participants.Where(o => phases.Contains(o.phase));
        }

        public IEnumerable<Participant> ListeParticipant2(int epreuve, JudoData DC)
        {
            IEnumerable<int> phases = DC.Deroulement.Phases.Where(o => o.epreuve == epreuve && o.suivant == 0).Select(o => o.id).Distinct();
            return _participants.Where(o => phases.Contains(o.phase));
        }

        public int GetNbCombatJudoka(string licence, JudoData DC)
        {
            int result = 0;
            using (TimedLock.Lock((DC.Participants.vjudokas as ICollection).SyncRoot))
            {
                foreach (Participants.Judoka vj in DC.Participants.Judokas.Where(o => o.licence == licence))
                {
                    result += DC.Deroulement.Combats.Count(o => o.vainqueur.HasValue && o.vainqueur > 0 && (o.participant1 == vj.id || o.participant2 == vj.id));
                }
            }
            return result;
        }

        public int GetNbPointJudoka(string licence, JudoData DC)
        {
            int result = 0;
            using (TimedLock.Lock((DC.Participants.vjudokas as ICollection).SyncRoot))
            {
                foreach (Participants.Judoka vj in DC.Participants.Judokas.Where(o => o.licence == licence))
                {
                    foreach (Participant participant in DC.Deroulement.Participants.Where(o => o.judoka == vj.id))
                    {
                        result += participant.cumulPointsGRCH;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// lecture des participants
        /// </summary>
        /// <param name="element">element XML contenant les participants</param>
        /// <param name="DC"></param>
        public void lecture_participants(XElement element/*, bool suppression*/)
        {
            ICollection<Participant> participants = Participant.LectureParticipant(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_participants as ICollection).SyncRoot))
            {
                _participants.Clear();

                foreach (Participant participant in participants)
                {
                    Participant p = _participants.FirstOrDefault(o => o.judoka == participant.judoka && o.phase == participant.phase);
                    /*
                    if (p != null)
                    {
                        _participants.Remove(p);
                    }
                    */
                    _participants.Add(participant);
                }
            }
        }

        /// <summary>
        /// lecture des phases
        /// </summary>
        /// <param name="element">element XML contenant les phases</param>
        /// <param name="DC"></param>
        public void lecture_phases(XElement element)
        {
            ICollection<Phase> phases = Phase.LecturePhases(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_phases as ICollection).SyncRoot))
            {
                _phases.Clear();
                foreach (Phase phase in phases)
                {
                    Phase p = _phases.FirstOrDefault(o => o.id == phase.id);
                    /*
                    if (p != null)
                    {
                        _phases.Remove(p);
                    }
                    */
                    _phases.Add(phase);
                }
            }
        }

        /// <summary>
        /// lecture des découpages
        /// </summary>
        /// <param name="element">element XML contenant les découpages</param>
        /// <param name="DC"></param>
        public void lecture_decoupages(XElement element)
        {
            ICollection<Phase_Decoupage> decoupages = Phase_Decoupage.LectureDecoupages(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_decoupages as ICollection).SyncRoot))
            {
                _decoupages.Clear();
                foreach (Phase_Decoupage decoupage in decoupages)
                {
                    Phase_Decoupage p = _decoupages.FirstOrDefault(o => o.id == decoupage.id);
                    /*
                    if (p != null)
                    {
                        _decoupages.Remove(p);
                    }
                    */
                    _decoupages.Add(decoupage);
                }
            }
        }

        /// <summary>
        /// lecture des groupes
        /// </summary>
        /// <param name="element">element XML contenant les groupes</param>
        /// <param name="DC"></param>
        public void lecture_groupes(XElement element, JudoData DC)
        {
            ICollection<Groupe_Combats> groupes = Groupe_Combats.LectureGroupes(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_groupes as ICollection).SyncRoot))
            {
                _groupes.Clear();  
                foreach (Groupe_Combats groupe in groupes)
                {
                    Groupe_Combats p = _groupes.FirstOrDefault(o => o.id == groupe.id);
                    /*
                    if (p != null)
                    {
                        _groupes.Remove(p);
                    }
                    */
                    _groupes.Add(groupe);
                }

                lecture_vue_groupes(DC);
            }
        }

        /// <summary>
        /// lecture des poules
        /// </summary>
        /// <param name="element">element XML contenant les poules</param>
        /// <param name="DC"></param>
        public void lecture_poules(XElement element)
        {
            ICollection<Poule> poules = Poule.LecturePoules(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_poules as ICollection).SyncRoot))
            {
                _poules.Clear();

                foreach (Poule poule in poules)
                {
                    Poule p = _poules.FirstOrDefault(o => o.id == poule.id);
                    /*
                    if (p != null)
                    {
                        _poules.Remove(p);
                    }
                    */
                    _poules.Add(poule);
                }
            }
        }


        /// <summary>
        /// lecture des combats
        /// </summary>
        /// <param name="element">element XML contenant les combats</param>
        /// <param name="DC"></param>
        public void lecture_combats(XElement element/*, bool suppression, int? tapis, ICombat CantDelete*/, JudoData DC)
        {
            ICollection<Combat> combats = Combat.LectureCombats(element, null);
            //ICollection<ICombat> deleted_c = new List<ICombat>();

            //Ajout des nouveaux
            using (TimedLock.Lock((_combats as ICollection).SyncRoot))
            {
                _combats.Clear();
                foreach (Combat combat in combats)
                {
                    Combat p = _combats.FirstOrDefault(o => o.id == combat.id);
                    /*
                    if (p != null)
                    {
                        _combats.Remove(p);
                    }
                    */
                    _combats.Add(combat);
                }
                this.lecture_vue_combats(DC);
            }
        }

        public ICollection<Combat> LectureCombats(XElement xelement/*, int? tapis*/, OutilsTools.MontreInformation1 MI)
        {
            return Combat.LectureCombats(xelement, MI);
        }

        /// <summary>
        /// lecture des rencontres
        /// </summary>
        /// <param name="element">element XML contenant les rencontres</param>
        /// <param name="DC"></param>
        public void lecture_rencontres(XElement element/*, bool suppression, int? tapis*/)
        {
            ICollection<Rencontre> rencontres = Rencontre.LectureRencontres(element, null);
            //ICollection<IRencontre> deleted_c = new List<IRencontre>();

            //Ajout des nouveaux
            using (TimedLock.Lock((_rencontres as ICollection).SyncRoot))
            {
                _rencontres.Clear();
                foreach (Rencontre rencontre in rencontres)
                {
                    Rencontre p = _rencontres.FirstOrDefault(o => o.id == rencontre.id);
                    /*
                    if (p != null)
                    {
                        _rencontres.Remove(p);
                    }
                    */
                    _rencontres.Add(rencontre);
                }
            }
        }

        public ICollection<Rencontre> LectureRencontres(XElement xelement, /*int? tapis,*/ OutilsTools.MontreInformation1 MI)
        {
            return Rencontre.LectureRencontres(xelement, MI);
        }

        /// <summary>
        /// lecture des feuilles
        /// </summary>
        /// <param name="element">element XML contenant les feuilles</param>
        /// <param name="DC"></param>
        public void lecture_feuilles(XElement element/*, bool suppression*/)
        {
            ICollection<Feuille> feuilles = Feuille.LectureFeuilles(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_feuilles as ICollection).SyncRoot))
            {
                _feuilles.Clear();
                foreach (Feuille feuille in feuilles)
                {
                    Feuille p = _feuilles.FirstOrDefault(o => o.id == feuille.id);
                    /*
                    if (p != null)
                    {
                        _feuilles.Remove(p);
                    }
                    */
                    _feuilles.Add(feuille);
                    //else
                    //{
                    //    p.classement1 = feuille.classement1;
                    //    p.classement2 = feuille.classement2;
                    //    p.combat = feuille.combat;
                    //    p.niveau = feuille.niveau;
                    //    p.numero = feuille.numero;
                    //    p.ordre = feuille.ordre;
                    //    p.pere = feuille.pere;
                    //    p.phase = feuille.phase;
                    //    p.ref1 = feuille.ref1;
                    //    p.ref2 = feuille.ref2;
                    //    p.reference = feuille.reference;
                    //    p.repechage = feuille.repechage;
                    //    p.source1 = feuille.source1;
                    //    p.source2 = feuille.source2;
                    //    p.typeSource = feuille.typeSource;
                    //}
                }

                //if (suppression)
                //{
                //    //Suppression de ceux qui ont été supprimer
                //    ICollection<IFeuille> deleted_f = new List<IFeuille>();
                //    foreach (IFeuille feuille in _feuilles)
                //    {
                //        IFeuille p = feuilles.FirstOrDefault(o => o.id == feuille.id);
                //        if (p == null)
                //        {
                //            deleted_f.Add(feuille);
                //        }
                //    }
                //    foreach (IFeuille feuille in deleted_f)
                //    {
                //        _feuilles.Remove(feuille);
                //    }
                //}
            }
        }

        public ICollection<Feuille> LectureFeuilles(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Feuille.LectureFeuilles(xelement, MI);
        }


        /// <summary>
        /// lecture des vue groupes
        /// </summary>
        /// <param name="element">element XML contenant les groupes</param>
        /// <param name="DC"></param>
        private void lecture_vue_groupes(JudoData DC)
        {
            //Ajout des nouveaux
            using (TimedLock.Lock((_vgroupes as ICollection).SyncRoot))
            {
                _vgroupes.Clear();
                foreach (Groupe_Combats groupe in _groupes)
                {
                    vue_groupe p = _vgroupes.FirstOrDefault(o => o.groupe_id == groupe.id);
                    vue_groupe vg = new vue_groupe(groupe, DC);
                    /*
                    if (p != null)
                    {
                        _vgroupes.Remove(p);
                    }
                    */
                    _vgroupes.Add(vg);
                    //else
                    //{
                    //    p.groupe_id = vg.groupe_id;
                    //    p.groupe_tapis = vg.groupe_tapis;
                    //    p.groupe_libelle = vg.groupe_libelle;
                    //    p.groupe_debut = vg.groupe_debut;
                    //    p.groupe_fin = vg.groupe_fin;
                    //    p.groupe_verrouille = vg.groupe_verrouille;
                    //    p.nb_combats_restant = vg.nb_combats_restant;
                    //    p.phase_etat = vg.phase_etat;
                    //    p.phase_libelle = vg.phase_libelle;
                    //    p.phase_id = vg.phase_id;
                    //    p.phase_type = vg.phase_type;
                    //    p.epreuve_id = vg.epreuve_id;
                    //    p.epreuve_nom = vg.epreuve_nom;
                    //    p.epreuve_poidsMin = vg.epreuve_poidsMin;
                    //    p.epreuve_poidsMax = vg.epreuve_poidsMax;
                    //    p.epreuve_libsexe = vg.epreuve_libsexe;
                    //}
                }
                ////Suppression de ceux qui ont été supprimer
                //ICollection<i_vue_groupe> deleted_e = new List<i_vue_groupe>();
                //foreach (i_vue_groupe groupe in _vgroupes)
                //{
                //    IGroupe_Combats p = _groupes.FirstOrDefault(o => o.id == groupe.groupe_id);
                //    if (p == null)
                //    {
                //        deleted_e.Add(groupe);
                //    }
                //}
                //foreach (i_vue_groupe groupe in deleted_e)
                //{
                //    _vgroupes.Remove(groupe);
                //}
            }
        }


        /// <summary>
        /// lecture des combats
        /// </summary>
        /// <param name="element">element XML contenant les combats</param>
        /// <param name="DC"></param>
        public void lecture_vue_combats(JudoData DC)
        {
            //ICollection<i_vue_combat> deleted_c = new List<i_vue_combat>();

            //Ajout des nouveaux
            using (TimedLock.Lock((_vcombats as ICollection).SyncRoot))
            {
                _vcombats.Clear();
                foreach (Combat combat in _combats)
                {
                    vue_combat p = _vcombats.FirstOrDefault(o => o.combat_id == combat.id);
                    vue_combat vc = new vue_combat(combat, DC);
                    /*
                    if (p != null)
                    {
                        _vcombats.Remove(p);
                    }
                    */
                    _vcombats.Add(vc);
                    //else  //????? (combat.participant1 == p.participant1 && combat.participant2 == p.participant2)
                    //{
                    //    p.combat_id = vc.combat_id;
                    //    p.combat_numero = vc.combat_numero;
                    //    p.combat_reference = vc.combat_reference;
                    //    p.combat_score1 = vc.combat_score1;
                    //    p.combat_score2 = vc.combat_score2;
                    //    p.combat_penalite1 = vc.combat_penalite1;
                    //    p.combat_penalite2 = vc.combat_penalite2;
                    //    p.combat_programmation = vc.combat_programmation;
                    //    p.combat_debut = vc.combat_debut;
                    //    p.combat_fin = vc.combat_fin;
                    //    p.combat_etat = vc.combat_etat;
                    //    p.combat_vaiqueur = vc.combat_vaiqueur;
                    //    p.combat_tapis = vc.combat_tapis;
                    //    p.combat_groupe = vc.combat_groupe;
                    //    p.combat_details = vc.combat_details;
                    //    p.combat_niveau = vc.combat_niveau;
                    //    p.combat_temps = vc.combat_temps;
                    //    p.combat_tempsRecup = vc.combat_tempsRecup;
                    //    p.judoka1_id = vc.judoka1_id ;
                    //    p.judoka1_club = vc.judoka1_club;
                    //    p.judoka1_licence = vc.judoka1_licence;
                    //    p.judoka1_nom = vc.judoka1_nom;
                    //    p.judoka1_prenom = vc.judoka1_prenom;
                    //    p.judoka2_id = vc.judoka2_id;
                    //    p.judoka2_club = vc.judoka2_club;
                    //    p.judoka2_licence = vc.judoka2_licence;
                    //    p.judoka2_nom = vc.judoka2_nom;
                    //    p.judoka2_prenom = vc.judoka2_prenom;
                    //    p.phase_id = vc.phase_id;
                    //    p.phase_libelle = vc.phase_libelle;
                    //    p.phase_type = vc.phase_type;
                    //    p.phase_etat = vc.phase_etat;
                    //    p.cateAge_id = vc.cateAge_id;
                    //    p.cateAge_nom = vc.cateAge_nom;
                    //    p.catePoids_id = vc.catePoids_id;
                    //    p.catePoids_nom = vc.catePoids_nom;
                    //    p.competition_id = vc.competition_id;
                    //    p.competition_nom = vc.competition_nom;
                    //    p.competition_date = vc.competition_date;
                    //    p.competition_lieu = vc.competition_lieu;
                    //    p.epreuve_id = vc.epreuve_id;
                    //    p.epreuve_nom = vc.epreuve_nom;
                    //    p.epreuve_poidsMin = vc.epreuve_poidsMin;
                    //    p.epreuve_poidsMax = vc.epreuve_poidsMax;
                    //    p.epreuve_ceintureMin = vc.epreuve_ceintureMin;
                    //    p.epreuve_ceintureMax = vc.epreuve_ceintureMax;
                    //    p.epreuve_anneeMin = vc.epreuve_anneeMin;
                    //    p.epreuve_anneeMax = vc.epreuve_anneeMax;
                    //    p.epreuve_debut = vc.epreuve_debut;
                    //    p.epreuve_fin = vc.epreuve_fin;
                    //    p.phase_name = vc.phase_name;
                    //    p.epreuve_sexe = vc.epreuve_sexe;

                    //    p.judoka1_id1 = vc.judoka1_id1;
                    //    p.judoka1_licence1 = vc.judoka1_licence1;
                    //    p.judoka1_nom1 = vc.judoka1_nom1;
                    //    p.judoka1_prenom1 = vc.judoka1_prenom1;
                    //    p.judoka1_club1 = vc.judoka1_club1;
                    //    p.judoka2_id1 = vc.judoka2_id1;
                    //    p.judoka2_licence1 = vc.judoka2_licence1;
                    //    p.judoka2_nom1 = vc.judoka2_nom1;
                    //    p.judoka2_prenom1 = vc.judoka2_prenom1;
                    //    p.judoka2_club1 = vc.judoka2_club1;
                    //}
                }

                ////Suppression de ceux qui ont été supprimer
                //foreach (i_vue_combat combat in _vcombats)
                //{
                //    ICombat p = _combats.FirstOrDefault(o => o.id == combat.combat_id);
                //    if (p == null)
                //    {
                //        deleted_c.Add(combat);
                //    }
                //}

                //foreach (i_vue_combat combat in deleted_c)
                //{
                //    _vcombats.Remove(combat);
                //}
            }
        }
    }
}
