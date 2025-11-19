
using KernelImpl.Internal;
using KernelImpl.Noyau.Deroulement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Tools.Export;
using Tools.Outils;

namespace KernelImpl.Noyau.Organisation
{
    public class DataOrganisation
    {
        private readonly DeduplicatedCachedData<int, Competition> _competitionsCache = new DeduplicatedCachedData<int, Competition>();
        private readonly DeduplicatedCachedData<int, Epreuve> _epreuvesCache = new DeduplicatedCachedData<int, Epreuve>();
        private readonly DeduplicatedCachedData<int, Epreuve_Equipe> _epreuve_equipesCache = new DeduplicatedCachedData<int, Epreuve_Equipe>();
        private readonly DeduplicatedCachedData<int, vue_epreuve_equipe> _vepreuves_equipeCache = new DeduplicatedCachedData<int, vue_epreuve_equipe>();
        private readonly DeduplicatedCachedData<int, vue_epreuve> _vepreuvesCache = new DeduplicatedCachedData<int, vue_epreuve>();

        // Accesseurs O(1)
        public IReadOnlyList<Competition> Competitions { get { return _competitionsCache.Cache; } }
        public IReadOnlyList<Epreuve> Epreuves { get { return _epreuvesCache.Cache; } }
        public IReadOnlyList<Epreuve_Equipe> EpreuveEquipes { get { return _epreuve_equipesCache.Cache; } }
        public IReadOnlyList<vue_epreuve_equipe> vepreuves_equipe { get { return _vepreuves_equipeCache.Cache; } }
        public IReadOnlyList<vue_epreuve> vepreuves { get { return _vepreuvesCache.Cache; } }

        /// <summary>
        /// lecture des compétitions
        /// </summary>
        /// <param name="element">element XML contenant les compétitions</param>
        /// <param name="DC"></param>
        public void lecture_competitions(XElement element, JudoData DC)
        {
            ICollection<Competition> competitions = Competition.LectureCompetitions(element, null);
            _competitionsCache.UpdateSnapshot(competitions, o => o.id); 

            DC.competition = Competitions.FirstOrDefault();
            DC.competitions = Competitions.ToList();
            ExportTools.default_competition = DC.competition.remoteId;
        }


        /// <summary>
        /// lecture des épreuves (équipe)
        /// </summary>
        /// <param name="element">element XML contenant les épreuves (équipe)</param>
        /// <param name="DC"></param>
        public void lecture_epreuves_equipe(XElement element, JudoData DC)
        {
            ICollection<Epreuve_Equipe> epreuves = Epreuve_Equipe.LectureEpreuveEquipes(element, null);
            ICollection<vue_epreuve_equipe> vepreuves = GenereVueEpreuveEquipe(epreuves, DC);

            _epreuve_equipesCache.UpdateSnapshot(epreuves, o => o.id);
            _vepreuves_equipeCache.UpdateSnapshot(vepreuves, o => o.id);
        }

        public ICollection<Epreuve_Equipe> LectureEpreuveEquipes(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Epreuve_Equipe.LectureEpreuveEquipes(xelement, MI);
        }


        /// <summary>
        /// lecture des épreuves
        /// </summary>
        /// <param name="element">element XML contenant les épreuves</param>
        /// <param name="DC"></param>
        public void lecture_epreuves(XElement element, JudoData DC)
        {
            ICollection<Epreuve> epreuves = Epreuve.LectureEpreuves(element, null);
            ICollection<vue_epreuve> vepreuves = GenereVueEpreuves(epreuves, DC);

            _epreuvesCache.UpdateSnapshot(epreuves, o => o.id);
            _vepreuvesCache.UpdateSnapshot(vepreuves, o => o.id);
        }

        public ICollection<Epreuve> LectureEpreuves(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Epreuve.LectureEpreuves(xelement, MI);
        }

        /// <summary>
        /// Genere la vue_épreuves (équipe)
        /// </summary>
        /// <param name="equipes">element XML contenant les épreuves (équipe)</param>
        /// <param name="DC"></param>
        private ICollection<vue_epreuve_equipe> GenereVueEpreuveEquipe(ICollection<Epreuve_Equipe> epreuves, JudoData DC)
        {
            return epreuves.Select(epreuve => new vue_epreuve_equipe(epreuve, DC)).ToList();
        }

        /// <summary>
        /// Genere les vues des épreuves
        /// </summary>
        /// <param name="element">element XML contenant les épreuves</param>
        /// <param name="DC"></param>
        private ICollection<vue_epreuve> GenereVueEpreuves(ICollection<Epreuve> epreuves, JudoData DC)
        {
            return epreuves.Select(epreuve => new vue_epreuve(epreuve, DC)).ToList();
        }
    }
}
