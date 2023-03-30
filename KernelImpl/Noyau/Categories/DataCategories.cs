
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Categories
{
    public class DataCategories 
    {
        private IList<CategorieAge> _cAges = new List<CategorieAge>();
        public IList<CategorieAge> CAges { get { return _cAges; } }

        private IList<CategoriePoids> _cPoids = new List<CategoriePoids>();
        public IList<CategoriePoids> CPoids { get { return _cPoids; } }

        private IList<Ceintures> _grades = new List<Ceintures>();
        public IList<Ceintures> Grades { get { return _grades; } }


        /// <summary>
        /// lecture des participants
        /// </summary>
        /// <param name="element">element XML contenant les catégories d'âge</param>
        /// <param name="DC"></param>
        public void lecture_cateages(XElement element)
        {
            ICollection<CategorieAge> cateages = CategorieAge.LectureCategorieAge(element, null);
            using (TimedLock.Lock((_cAges as ICollection).SyncRoot))
            {
                //Ajout des nouveaux
                foreach (CategorieAge cateage in cateages)
                {
                    CategorieAge p = _cAges.FirstOrDefault(o => o.id == cateage.id);
                    if (p != null)
                    {
                        _cAges.Remove(p);
                    }
                    _cAges.Add(cateage);
                }
            }
        }


        /// <summary>
        /// lecture des participants
        /// </summary>
        /// <param name="element">element XML contenant les catégories de poids</param>
        /// <param name="DC"></param>
        public void lecture_catepoids(XElement element)
        {
            ICollection<CategoriePoids> catepoids = CategoriePoids.LectureCategoriePoids(element, null);
            using (TimedLock.Lock((_cPoids as ICollection).SyncRoot))
            {
                //Ajout des nouveaux
                foreach (CategoriePoids catepoid in catepoids)
                {
                    CategoriePoids p = _cPoids.FirstOrDefault(o => o.id == catepoid.id);
                    if (p != null)
                    {
                        _cPoids.Remove(p);
                    }
                    _cPoids.Add(catepoid);
                }               
            }
        }


        /// <summary>
        /// lecture des ceintures
        /// </summary>
        /// <param name="element">element XML contenant les ceintures</param>
        /// <param name="DC"></param>
        public void lecture_ceintures(XElement element)
        {
            ICollection<Ceintures> ceintures = Ceintures.LectureCeintures(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_grades as ICollection).SyncRoot))
            {
                foreach (Ceintures ceinture in ceintures)
                {
                    Ceintures p = _grades.FirstOrDefault(o => o.id == ceinture.id);
                    if (p != null)
                    {
                        _grades.Remove(p);
                    }
                    _grades.Add(ceinture);
                }

                Ceintures grade = _grades.FirstOrDefault(o => o.nom == "1D");
                if(grade != null)
                {
                    OutilsTools.Grade1D_ID = grade.id;
                }

                grade = _grades.FirstOrDefault(o => o.nom == "7D");
                if (grade != null)
                {
                    OutilsTools.Grade7D_ID = grade.id;
                }
            }
            
        }
    }
}
