
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Structures
{
    public class DataStructures
    {
        private IList<Club> _clubs = new List<Club>();
        public IList<Club> Clubs { get { return _clubs; } }

        private IList<Comite> _comites = new List<Comite>();
        public IList<Comite> Comites { get { return _comites; } }

        private IList<Secteur> _secteurs = new List<Secteur>();
        public IList<Secteur> Secteurs { get { return _secteurs; } }

        private IList<Ligue> _ligues = new List<Ligue>();
        public IList<Ligue> Ligues { get { return _ligues; } }

        private IList<Pays> _pays = new List<Pays>();
        public IList<Pays> LesPays { get { return _pays; } }


        /// <summary>
        /// lecture des clubs
        /// </summary>
        /// <param name="element">element XML contenant les clubs</param>
        /// <param name="DC"></param>
        public void lecture_clubs(XElement element)
        {
            ICollection<Club> clubs = Club.LectureClubs(element, null);
            using (TimedLock.Lock((_clubs as ICollection).SyncRoot))
            {
                foreach (Club club in clubs)
                {
                    Club p = _clubs.FirstOrDefault(o => o.id == club.id);
                    if (p != null)
                    {
                        _clubs.Remove(p);
                    }
                    _clubs.Add(club);
                }
            }
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
            using (TimedLock.Lock((_comites as ICollection).SyncRoot))
            {
                foreach (Comite comite in comites)
                {
                    //Comite p = _comites.FirstOrDefault(o => o.id == comite.id);
                    Comite p = _comites.FirstOrDefault(o => o.id == comite.id && o.ligue == comite.ligue);
                    if (p != null)
                    {
                        _comites.Remove(p);
                    }
                    _comites.Add(comite);
                }
            }
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
            using (TimedLock.Lock((_secteurs as ICollection).SyncRoot))
            {
                foreach (Secteur secteur in secteurs)
                {
                    Secteur p = _secteurs.FirstOrDefault(o => o.id == secteur.id);
                    if (p != null)
                    {
                        _secteurs.Remove(p);
                    }
                    _secteurs.Add(secteur);
                }
            }
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
            using (TimedLock.Lock((_ligues as ICollection).SyncRoot))
            {
                foreach (Ligue ligue in ligues)
                {
                    Ligue p = _ligues.FirstOrDefault(o => o.id == ligue.id);
                    if (p != null)
                    {
                        _ligues.Remove(p);
                    }
                    _ligues.Add(ligue);
                }
            }
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
            using (TimedLock.Lock((_pays as ICollection).SyncRoot))
            {
                foreach (Pays pays in pays2)
                {
                    Pays p = _pays.FirstOrDefault(o => o.id == pays.id);
                    if (p != null)
                    {
                        _pays.Remove(p);
                    }
                    _pays.Add(pays);
                }
            }
        }
    }
}
