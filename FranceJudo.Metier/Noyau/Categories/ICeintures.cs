
using System.Collections.Generic;
using System.Xml.Linq;


namespace FranceJudo.Metier.Noyau.Categories
{
    /// <summary>
    /// Description des Ceintures
    /// </summary>
    public interface ICeintures
    {
        public int id { get; set; }
        public string nom { get; set; }
        public string ordre { get; set; }
        public string remoteId { get; set; }
        public string couleur1 { get; set; }
        public string couleur2 { get; set; }
    }
}
