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
        public IReadOnlyList<CategorieAge> CAges { get; }

        public IReadOnlyList<CategoriePoids> CPoids { get; }

        public IReadOnlyList<Ceintures> Grades { get; }

        public CategoriesSnapshot(DataCategories source)
        {
            if (source == null) return;
            CAges = source.CAges;
            CPoids = source.CPoids;
            Grades = source.Grades;
        }
    }

}
