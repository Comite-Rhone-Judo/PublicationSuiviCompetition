using System.Collections.Generic;
using FranceJudo.Metier.Noyau.Categories;

namespace KernelImpl.Noyau.Categories
{
    public class CategoriesSnapshot : ICategoriesData
    {
        public IReadOnlyList<ICategorieAge> CAges { get; private set; }

        public IReadOnlyList<ICategoriePoids> CPoids { get; private set; }

        public IReadOnlyList<ICeintures> Grades { get; private set; }

        public CategoriesSnapshot(DataCategories source)
        {
            if (source == null) return;
            CAges = source.CAges;
            CPoids = source.CPoids;
            Grades = source.Grades;
        }
    }

}
