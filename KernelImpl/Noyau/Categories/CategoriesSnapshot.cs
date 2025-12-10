using KernelImpl.Noyau.Arbitrage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Categories
{
    public class CategoriesSnapshot : ICategoriesData
    {
        public IReadOnlyList<CategorieAge> CAges { get; private set; }

        public IReadOnlyList<CategoriePoids> CPoids { get; private set; }

        public IReadOnlyList<Ceintures> Grades { get; private set; }

        public CategoriesSnapshot(DataCategories source)
        {
            if (source == null) return;
            CAges = source.CAges;
            CPoids = source.CPoids;
            Grades = source.Grades;
        }
    }

}
