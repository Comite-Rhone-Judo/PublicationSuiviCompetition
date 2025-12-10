
using KernelImpl.Internal;
using KernelImpl.Noyau.Organisation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

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
        public IReadOnlyList<Comite> Comites { get { return _comitesCache.Cache ; } }
        public IReadOnlyList<Secteur> Secteurs { get { return _secteursCache.Cache; } }
        public IReadOnlyList<Ligue> Ligues { get { return _liguesCache.Cache; } }
        public IReadOnlyList<Pays> LesPays { get { return _paysCache.Cache; } }

        /// <summary>
        /// lecture des clubs
        /// </summary>
        /// <param name="element">element XML contenant les clubs</param>
        /// <param name="DC"></param>
        public void lecture_clubs(XElement element)
        {
            ICollection<Club> clubs = Club.LectureClubs(element, null);
            _clubsCache.UpdateFullSnapshot(clubs);
        }

        public ICollection<Club> LectureClubs(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Club.LectureClubs(xelement, MI);
        }

        /// <summary>
        /// lecture des comités
        /// </summary>
        /// <param name="element">element XML contenant les comités</param>
        /// <param name="DC"></param>
        public void lecture_comites(XElement element)
        {
            ICollection<Comite> comites = Comite.LectureComites(element, null);
            _comitesCache.UpdateFullSnapshot(comites); 
        }

        public ICollection<Secteur> LectureSecteurs(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Secteur.LectureSecteurs(xelement, MI);
        }

        /// <summary>
        /// lecture des comités
        /// </summary>
        /// <param name="element">element XML contenant les comités</param>
        /// <param name="DC"></param>
        public void lecture_secteurs(XElement element)
        {
            ICollection<Secteur> secteurs = Secteur.LectureSecteurs(element, null);
            _secteursCache.UpdateFullSnapshot(secteurs);      
        }

        public ICollection<Comite> LectureComites(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Comite.LectureComites(xelement, MI);
        }


        /// <summary>
        /// lecture des ligues
        /// </summary>
        /// <param name="element">element XML contenant les ligues</param>
        /// <param name="DC"></param>
        public void lecture_ligues(XElement element)
        {
            ICollection<Ligue> ligues = Ligue.LectureLigues(element, null);
            _liguesCache.UpdateFullSnapshot(ligues);
        }

        public ICollection<Ligue> LectureLigues(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Ligue.LectureLigues(xelement, MI);
        }

        /// <summary>
        /// lecture des ligues
        /// </summary>
        /// <param name="element">element XML contenant les ligues</param>
        /// <param name="DC"></param>
        public void lecture_pays(XElement element)
        {
            ICollection<Pays> pays2 = Pays.LecturePays(element, null);
            _paysCache.UpdateFullSnapshot(pays2);
        }
    }
}
