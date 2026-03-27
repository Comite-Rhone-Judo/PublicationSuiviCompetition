
using FranceJudo.Metier.Noyau.Organisation;

namespace FranceJudo.Metier.Noyau.Categories
{
    /// <summary>
    /// Description des Categorie Poids
    /// </summary>
    public interface ICategoriePoids
    {
        public int id { get; set; }
        public string nom { get; set; }
        public string remoteId { get; set; }
        public int poidsMin { get; set; }
        public int poidsMax { get; set; }
        public string ordre { get; set; }
        public int categorieAge { get; set; }
        public int sexe { get; set; }
        public EpreuveSexe sexeEnum { get; set; }
        public bool equipe { get; set; }
        public string discipline { get; set; }
    }
}
