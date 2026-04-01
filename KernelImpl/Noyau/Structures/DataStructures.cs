
using FranceJudo.Metier.Noyau.Structures;
using KernelImpl.Internal;
using System.Collections.Generic;
using System.Xml.Linq;


namespace KernelImpl.Noyau.Structures
{
    public class DataStructures : IStructuresData
    {
        private readonly DeduplicatedCachedData<string, Club> _clubsCache = new DeduplicatedCachedData<string, Club>();
        private readonly DeduplicatedCachedData<string, Comite> _comitesCache = new DeduplicatedCachedData<string, Comite>();
        private readonly DeduplicatedCachedData<string, Secteur> _secteursCache = new DeduplicatedCachedData<string, Secteur>();
        private readonly DeduplicatedCachedData<string, Ligue> _liguesCache = new DeduplicatedCachedData<string, Ligue>();
        private readonly DeduplicatedCachedData<int, Pays> _paysCache = new DeduplicatedCachedData<int, Pays>();

        // Accesseurs O(1)
        public IReadOnlyList<Club> Clubs { get { return _clubsCache.Cache; } }
        public IReadOnlyList<Comite> Comites { get { return _comitesCache.Cache; } }
        public IReadOnlyList<Secteur> Secteurs { get { return _secteursCache.Cache; } }
        public IReadOnlyList<Ligue> Ligues { get { return _liguesCache.Cache; } }
        public IReadOnlyList<Pays> LesPays { get { return _paysCache.Cache; } }

        IReadOnlyList<IClub> IStructuresData.Clubs => Clubs;
        IReadOnlyList<IComite> IStructuresData.Comites => Comites;
        IReadOnlyList<ISecteur> IStructuresData.Secteurs => Secteurs;
        IReadOnlyList<ILigue> IStructuresData.Ligues => Ligues;
        IReadOnlyList<IPays> IStructuresData.LesPays => LesPays;

        /// <summary>
        /// lecture des clubs
        /// </summary>
        /// <param name="element">element XML contenant les clubs</param>
        /// <param name="DC"></param>
        public void ChargerClubs(XElement element)
        {
            ICollection<Club> clubs = Club.LectureClubs(element);
            _clubsCache.UpdateFullSnapshot(clubs);
        }

        public ICollection<Club> LectureClubs(XElement xelement)
        {
            return Club.LectureClubs(xelement);
        }

        /// <summary>
        /// lecture des comités
        /// </summary>
        /// <param name="element">element XML contenant les comités</param>
        /// <param name="DC"></param>
        public void ChargerComites(XElement element)
        {
            ICollection<Comite> comites = Comite.LectureComites(element);
            _comitesCache.UpdateFullSnapshot(comites);
        }

        public ICollection<Secteur> LectureSecteurs(XElement xelement)
        {
            return Secteur.LectureSecteurs(xelement);
        }

        /// <summary>
        /// lecture des comités
        /// </summary>
        /// <param name="element">element XML contenant les comités</param>
        /// <param name="DC"></param>
        public void ChargerSecteurs(XElement element)
        {
            ICollection<Secteur> secteurs = Secteur.LectureSecteurs(element);
            _secteursCache.UpdateFullSnapshot(secteurs);
        }

        public ICollection<Comite> LectureComites(XElement xelement)
        {
            return Comite.LectureComites(xelement);
        }


        /// <summary>
        /// lecture des ligues
        /// </summary>
        /// <param name="element">element XML contenant les ligues</param>
        /// <param name="DC"></param>
        public void ChargerLigues(XElement element)
        {
            ICollection<Ligue> ligues = Ligue.LectureLigues(element);
            _liguesCache.UpdateFullSnapshot(ligues);
        }

        public ICollection<Ligue> LectureLigues(XElement xelement)
        {
            return Ligue.LectureLigues(xelement);
        }

        /// <summary>
        /// lecture des ligues
        /// </summary>
        /// <param name="element">element XML contenant les ligues</param>
        /// <param name="DC"></param>
        public void ChargerPays(XElement element)
        {
            ICollection<Pays> pays2 = Pays.LecturePays(element);
            _paysCache.UpdateFullSnapshot(pays2);
        }
    }
}
