
using KernelImpl.Internal;
using KernelImpl.Noyau.Arbitrage;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;
using Tools.XML;

namespace KernelImpl.Noyau.Categories
{
    public class DataCategories : ICategoriesData
    {
        private readonly DeduplicatedCachedData<int, CategorieAge> _cAgesCache = new DeduplicatedCachedData<int, CategorieAge>();
        private readonly DeduplicatedCachedData<int, CategoriePoids> _cPoidsCache = new DeduplicatedCachedData<int, CategoriePoids>();
        private readonly DeduplicatedCachedData<int, Ceintures> _gradesCache = new DeduplicatedCachedData<int, Ceintures>();


        public IReadOnlyList<CategorieAge> CAges { get { return _cAgesCache.Cache; } }

        public IReadOnlyList<CategoriePoids> CPoids { get { return _cPoidsCache.Cache; } }

        public IReadOnlyList<Ceintures> Grades { get { return _gradesCache.Cache; } }


        /// <summary>
        /// lecture des participants
        /// </summary>
        /// <param name="element">element XML contenant les catégories d'âge</param>
        /// <param name="DC"></param>
        public void lecture_cateages(XElement element)
        {
            ICollection<CategorieAge> cateages = CategorieAge.LectureCategorieAge(element, null);
            _cAgesCache.UpdateFullSnapshot(cateages);
        }


        /// <summary>
        /// lecture des participants
        /// </summary>
        /// <param name="element">element XML contenant les catégories de poids</param>
        /// <param name="DC"></param>
        public void lecture_catepoids(XElement element)
        {
            ICollection<CategoriePoids> catepoids = CategoriePoids.LectureCategoriePoids(element, null);
            _cPoidsCache.UpdateFullSnapshot(catepoids);
        }


        /// <summary>
        /// lecture des ceintures
        /// </summary>
        /// <param name="element">element XML contenant les ceintures</param>
        /// <param name="DC"></param>
        public void lecture_ceintures(XElement element)
        {
            ICollection<Ceintures> ceintures = Ceintures.LectureCeintures(element, null);
            _gradesCache.UpdateFullSnapshot(ceintures);

            Ceintures grade = Grades.FirstOrDefault(o => o.nom == "1D");
            if (grade != null)
            {
                OutilsTools.Grade1D_ID = grade.id;
            }

            grade = Grades.FirstOrDefault(o => o.nom == "7D");
            if (grade != null)
            {
                OutilsTools.Grade7D_ID = grade.id;
            }
        }
    }
}
