using System.Collections.Generic;
using Tools.Framework;
using Tools.Logging;

namespace AppPublication.Statistiques
{
    public class StatMgrGeneration : NotificationBase
    {
        public enum CompteurGenerationEnum
        {
            TempsGeneration = 0,
            DelaiGeneration = 1,
            NbErreurGeneration = 3
        }

        private Dictionary<CompteurGenerationEnum, StatistiqueItem> _compteursGeneration;
        public Dictionary<CompteurGenerationEnum, StatistiqueItem> CompteursGeneration
        {
            get => _compteursGeneration;
            private set { _compteursGeneration = value; NotifyPropertyChanged(); }
        }

        public StatMgrGeneration()
        {
            try
            {
                var cpt = new Dictionary<CompteurGenerationEnum, StatistiqueItem>();
                cpt.Add(CompteurGenerationEnum.TempsGeneration, new StatistiqueItemMoyenneur(CompteurGenerationEnum.TempsGeneration.ToString(), "Durée de génération (Sec.)"));
                cpt.Add(CompteurGenerationEnum.DelaiGeneration, new StatistiqueItemMoyenneur(CompteurGenerationEnum.DelaiGeneration.ToString(), "Délai entre génération (Sec.)"));
                cpt.Add(CompteurGenerationEnum.NbErreurGeneration, new StatistiqueItemCompteur(CompteurGenerationEnum.NbErreurGeneration.ToString(), "Nb d'erreurs de génération"));
                CompteursGeneration = cpt;
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'initialisation des statistiques generation");
            }
        }

        public void EnregistrerErreurGeneration()
        {
            try
            {
                StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.NbErreurGeneration];
                item?.EnregistrerValeur();
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement d'une erreur de generation");
            }
        }

        public void EnregistrerGeneration(float duree)
        {
            try
            {
                StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.TempsGeneration];
                item?.EnregistrerValeur(duree);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement d'une generation");
            }
        }

        public void EnregistrerDelaiGeneration(float delai)
        {
            try
            {
                StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.DelaiGeneration];
                item?.EnregistrerValeur(delai);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du delai de generation");
            }
        }
    }
}