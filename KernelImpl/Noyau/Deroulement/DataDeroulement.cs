
using KernelImpl.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    public class DataDeroulement
    {
        private readonly DeduplicatedCachedData<int, Rencontre> _rencontresCache = new DeduplicatedCachedData<int, Rencontre>();
        private readonly DeduplicatedCachedData<int, Feuille> _feuillesCache = new DeduplicatedCachedData<int, Feuille>();
        private readonly DeduplicatedCachedData<int, Combat> _combatsCache = new DeduplicatedCachedData<int, Combat>();
        private readonly DeduplicatedCachedData<int, Phase_Decoupage> _decoupagesCache = new DeduplicatedCachedData<int, Phase_Decoupage>();
        private readonly DeduplicatedCachedData<int, Groupe_Combats> _groupesCache = new DeduplicatedCachedData<int, Groupe_Combats>();
        private readonly DeduplicatedCachedData<int, Phase> _phasesCache = new DeduplicatedCachedData<int, Phase>();
        private readonly DeduplicatedCachedData<int, Poule> _poulesCache = new DeduplicatedCachedData<int, Poule>();
        private readonly DeduplicatedCachedData<int, Participant> _participantsCache = new DeduplicatedCachedData<int, Participant>();
        private readonly DeduplicatedCachedData<int, vue_groupe> _vgroupesCache = new DeduplicatedCachedData<int, vue_groupe>();
        private readonly DeduplicatedCachedData<int, vue_combat> _vcombatsCache = new DeduplicatedCachedData<int, vue_combat>();

        // Accesseurs O(1)
        public IReadOnlyList<Combat> Combats { get { return _combatsCache.Cache; } }
        public IReadOnlyList<Rencontre> Rencontres { get { return _rencontresCache.Cache; } }
        public IReadOnlyList<Feuille> Feuilles { get { return _feuillesCache.Cache; } }
        public IReadOnlyList<Phase_Decoupage> Decoupages { get { return _decoupagesCache.Cache; } }
        public IReadOnlyList<Groupe_Combats> Groupes { get { return _groupesCache.Cache; } }
        public IReadOnlyList<Phase> Phases { get { return _phasesCache.Cache; } }
        public IReadOnlyList<Poule> Poules { get { return _poulesCache.Cache; } }
        public IReadOnlyList<Participant> Participants { get { return _participantsCache.Cache; } }
        public IReadOnlyList<vue_groupe> vgroupes { get { return _vgroupesCache.Cache; } }
        public IReadOnlyList<vue_combat> vcombats { get { return _vcombatsCache.Cache; } }


        public IEnumerable<Participant> ListeParticipant1(int epreuve, JudoData DC)
        {
            IEnumerable<int> phases = DC.Deroulement.Phases.Where(o => o.epreuve == epreuve && o.suivant != 0).Select(o => o.id).Distinct();
            return Participants.Where(o => phases.Contains(o.phase));
        }

        public IEnumerable<Participant> ListeParticipant2(int epreuve, JudoData DC)
        {
            IEnumerable<int> phases = DC.Deroulement.Phases.Where(o => o.epreuve == epreuve && o.suivant == 0).Select(o => o.id).Distinct();
            return Participants.Where(o => phases.Contains(o.phase));
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
            _participantsCache.UpdateSnapshot(participants, o => o.id);
        }

        /// <summary>
        /// lecture des phases
        /// </summary>
        /// <param name="element">element XML contenant les phases</param>
        /// <param name="DC"></param>
        public void lecture_phases(XElement element)
        {
            ICollection<Phase> phases = Phase.LecturePhases(element, null);
            _phasesCache.UpdateSnapshot(phases, o => o.id);
        }

        /// <summary>
        /// lecture des découpages
        /// </summary>
        /// <param name="element">element XML contenant les découpages</param>
        /// <param name="DC"></param>
        public void lecture_decoupages(XElement element)
        {
            ICollection<Phase_Decoupage> decoupages = Phase_Decoupage.LectureDecoupages(element, null);
            _decoupagesCache.UpdateSnapshot(decoupages, o => o.id);
        }

        /// <summary>
        /// lecture des groupes
        /// </summary>
        /// <param name="element">element XML contenant les groupes</param>
        /// <param name="DC"></param>
        public void lecture_groupes(XElement element, JudoData DC)
        {
            ICollection<Groupe_Combats> groupes = Groupe_Combats.LectureGroupes(element, null);
            _groupesCache.UpdateSnapshot(groupes, o => o.id);


            ICollection<vue_groupe> vgroupes = GenereVueGroupe(groupes, DC);    
            _vgroupesCache.UpdateSnapshot(vgroupes, o => o.groupe_id);
        }

        /// <summary
        /// Genere le snapshot des vues groupes
        /// </summary>
        /// <param name="groupes">Le snapshot des groupes</param>
        /// <param name="DC"></param>
        private ICollection<vue_groupe> GenereVueGroupe(ICollection<Groupe_Combats> groupes, JudoData DC)
        {
            return groupes.Select(o => new vue_groupe(o, DC)).ToList();
        }

        /// <summary>
        /// lecture des poules
        /// </summary>
        /// <param name="element">element XML contenant les poules</param>
        /// <param name="DC"></param>
        public void lecture_poules(XElement element)
        {
            ICollection<Poule> poules = Poule.LecturePoules(element, null);
            _poulesCache.UpdateSnapshot(poules, o => o.id);
        }


        /// <summary>
        /// lecture des combats
        /// </summary>
        /// <param name="element">element XML contenant les combats</param>
        /// <param name="DC"></param>
        public void lecture_combats(XElement element/*, bool suppression, int? tapis, ICombat CantDelete*/, JudoData DC)
        {
            ICollection<Combat> combats = Combat.LectureCombats(element, null);
            _combatsCache.UpdateSnapshot(combats, o => o.id);

            ICollection<vue_combat> vcombats = GenereVueCombat(combats, DC);
            _vcombatsCache.UpdateSnapshot(vcombats, o => o.combat_id);
        }

        /// <summary
        /// Genere le snapshot des vues combat
        /// </summary>
        /// <param name="groupes">Le snapshot des groupes</param>
        /// <param name="DC"></param>
        private ICollection<vue_combat> GenereVueCombat(ICollection<Combat> combats, JudoData DC)
        {
            return combats.Select(o => new vue_combat(o, DC)).ToList();
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
            _rencontresCache.UpdateSnapshot(rencontres, o => o.id);
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
            _feuillesCache.UpdateSnapshot(feuilles, o => o.id); 
        }

        public ICollection<Feuille> LectureFeuilles(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Feuille.LectureFeuilles(xelement, MI);
        }
    }
}
