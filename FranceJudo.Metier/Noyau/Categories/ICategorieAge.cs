
namespace FranceJudo.Metier.Noyau.Categories
{
    /// <summary>
    /// Description des Categorie Age
    /// </summary>
    public interface ICategorieAge
    {
        public int id { get; set; }
        public string nom { get; set; }
        public int anneeMin { get; set; }
        public int anneeMax { get; set; }
        public string ordre { get; set; }
        public string remoteId { get; set; }
    }
}
