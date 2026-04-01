using System.Collections.Generic;

namespace FranceJudo.Metier.Noyau.Categories
{
    public interface ICategoriesData
    {
        IReadOnlyList<ICategorieAge> CAges { get; }

        IReadOnlyList<ICategoriePoids> CPoids { get; }

        IReadOnlyList<ICeintures> Grades { get; }
    }
}
