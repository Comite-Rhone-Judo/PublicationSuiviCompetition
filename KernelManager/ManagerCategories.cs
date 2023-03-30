using KernelImpl.Noyau.Categories;


namespace KernelManager
{
    public class ManagerCategories
    {
        public CategorieAge CreateCategorieAge()
        {
            return new CategorieAge();
        }

        public CategoriePoids CreateCategoriePoids()
        {
            return new CategoriePoids();
        }

        public Ceintures CreateCeinture()
        {
            return new Ceintures();
        }

    }
}
