using KernelImpl.Noyau.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Categories
{
    public interface ICategoriesData
    {
        IReadOnlyList<CategorieAge> CAges { get; }

        IReadOnlyList<CategoriePoids> CPoids { get; }

        IReadOnlyList<Ceintures> Grades { get; }
    }
}
