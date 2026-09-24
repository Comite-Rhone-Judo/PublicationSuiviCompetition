namespace AppPublication.Statistiques
{
    internal class StatistiqueItemCompteur : StatistiqueItem
    {
        #region CONSTRUCTEURS
        public StatistiqueItemCompteur(string name, string libelle, string unite = "") : base(name, libelle, unite)
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
