using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Statistiques
{
    public class StatistiqueItemCompteur : StatistiqueItem
    {
        #region CONSTRUCTEURS
        public StatistiqueItemCompteur(string name, string libelle) : base(name, libelle)
        {
            Valeur = 0;
        }
        #endregion

        #region METHODES
        public override void EnregistrerValeur(float? val = null)
        {
            // Enregistre uniquement le nb de valeur vue
            Valeur = (Valeur == null) ? 0 : Valeur;

            Valeur++;
        }
        #endregion

    }
}
