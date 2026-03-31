
using FranceJudo.Metier.Export;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using KernelImpl.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KernelImpl.Noyau.Organisation
{
    public class DataOrganisation : IOrganisationData
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
        public IReadOnlyList<vue_epreuve_equipe> VueEpreuveEquipes { get { return _vepreuves_equipeCache.Cache; } }
        public IReadOnlyList<vue_epreuve> VueEpreuves { get { return _vepreuvesCache.Cache; } }

        IReadOnlyList<ICompetition> IOrganisationData.Competitions => Competitions;
        IReadOnlyList<IEpreuve> IOrganisationData.Epreuves => Epreuves;
        IReadOnlyList<IEpreuve_Equipe> IOrganisationData.EpreuveEquipes => EpreuveEquipes;
        IReadOnlyList<Ivue_epreuve_equipe> IOrganisationData.VueEpreuveEquipes => VueEpreuveEquipes;
        IReadOnlyList<Ivue_epreuve> IOrganisationData.VueEpreuves => VueEpreuves;

        ICompetition IOrganisationData.Competition => Competition;

        public Competition Competition { get; private set; }

        /// <summary>
        /// lecture des compétitions
        /// </summary>
        /// <param name="element">element XML contenant les compétitions</param>
        /// <param name="DC"></param>
        public void ChargeCompetitions(XElement element, IJudoData DC)
        {
            ICollection<Competition> competitions = Competition.LectureCompetitions(element);
            _competitionsCache.UpdateFullSnapshot(competitions);
            Competition = competitions.FirstOrDefault();

            SiteExportEngine.DefaultCompetition = DC.Organisation.Competition.remoteId;
        }


        /// <summary>
        /// lecture des épreuves (équipe)
        /// </summary>
        /// <param name="element">element XML contenant les épreuves (équipe)</param>
        /// <param name="DC"></param>
        public void ChargeEpreuvesEquipe(XElement element, IJudoData DC)
        {
            ICollection<Epreuve_Equipe> epreuves = Epreuve_Equipe.LectureEpreuveEquipes(element);
            _epreuve_equipesCache.UpdateFullSnapshot(epreuves);

            ICollection<vue_epreuve_equipe> vepreuves = GenereVueEpreuveEquipe(epreuves, DC);
            _vepreuves_equipeCache.UpdateFullSnapshot(vepreuves);
        }

        public ICollection<Epreuve_Equipe> LectureEpreuveEquipes(XElement xelement)
        {
            return Epreuve_Equipe.LectureEpreuveEquipes(xelement);
        }


        /// <summary>
        /// lecture des épreuves
        /// </summary>
        /// <param name="element">element XML contenant les épreuves</param>
        /// <param name="DC"></param>
        public void ChargeEpreuves(XElement element, IJudoData DC)
        {
            ICollection<Epreuve> epreuves = Epreuve.LectureEpreuves(element);
            _epreuvesCache.UpdateFullSnapshot(epreuves);

            ICollection<vue_epreuve> vepreuves = GenereVueEpreuves(epreuves, DC);
            _vepreuvesCache.UpdateFullSnapshot(vepreuves);
        }

        public ICollection<Epreuve> LectureEpreuves(XElement xelement)
        {
            return Epreuve.LectureEpreuves(xelement);
        }

        /// <summary>
        /// Genere la vue_épreuves (équipe)
        /// </summary>
        /// <param name="equipes">element XML contenant les épreuves (équipe)</param>
        /// <param name="DC"></param>
        private ICollection<vue_epreuve_equipe> GenereVueEpreuveEquipe(ICollection<Epreuve_Equipe> epreuves, IJudoData DC)
        {
            return epreuves.Select(epreuve => new vue_epreuve_equipe(epreuve, DC)).ToList();
        }

        /// <summary>
        /// Genere les vues des épreuves
        /// </summary>
        /// <param name="element">element XML contenant les épreuves</param>
        /// <param name="DC"></param>
        private ICollection<vue_epreuve> GenereVueEpreuves(ICollection<Epreuve> epreuves, IJudoData DC)
        {
            return epreuves.Select(epreuve => new vue_epreuve(epreuve, DC)).ToList();
        }
    }
}
