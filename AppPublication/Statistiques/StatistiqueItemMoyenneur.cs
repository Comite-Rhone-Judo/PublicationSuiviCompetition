using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Statistiques
{
    internal class StatistiqueItemMoyenneur : StatistiqueItem
    {
        #region MEMBRES
        protected float _sommeValeur = 0F;
        protected int _nValeur = 0;
        #endregion

        #region CONSTRUCTEURS
        public StatistiqueItemMoyenneur(string name, string libelle) : base(name, libelle)
        {

        }
        #endregion

        #region METHODES
        public override void EnregistrerValeur(float? val = null)
        {
            if (val != null)
            {
                // Enregistre la valeur passée si elle n'est pas nulle
                _nValeur++;
                _sommeValeur += val.Value;

                // Actualise les Min/Max/Moy
                Moy = (_nValeur > 0) ? _sommeValeur / _nValeur : float.NaN;
                Max = (Max == null) ? val.Value : Math.Max(Max.Value, val.Value);
                Min = (Min == null) ? val.Value : Math.Min(Min.Value, val.Value);
            }
        }
        #endregion

    }
}
