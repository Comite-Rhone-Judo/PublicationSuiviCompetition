using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Outils;

namespace AppPublication.Controles
{
    public class GestionStatistiques : NotificationBase
    {
        #region MEMBRES
        private float _SumTpsGeneration = 0F;
        private float _SumDelaiGeneration = 0F;
        private float _SumTpsSynchronisationComplete = 0F;
        private float _SumTpsSynchronisationDifference = 0F;

        private float _SumNbFichierSynchronisationDifference = 0F;
        private float _SumNbFichierSynchronisationComplete = 0F;

        #endregion

        #region CONSTRUCTEURS
        public GestionStatistiques()
        {
            _SumDelaiGeneration = 0F;
            _SumTpsSynchronisationComplete = 0F;
            _SumTpsSynchronisationDifference = 0F;
            _SumTpsGeneration = 0F;
            _SumNbFichierSynchronisationDifference = 0F;
            _SumNbFichierSynchronisationComplete = 0F;
        }
        #endregion

        #region PROPRIETES
        private float _tpsGenerationMinSec = float.NaN;
        public float TpsGenerationMinSec {
            get
            {
                return _tpsGenerationMinSec;
            }
            private set {
                _tpsGenerationMinSec = value;
                NotifyPropertyChanged("TpsGenerationMinSec");
            }
        }


        private float _tpsGenerationMoySec = 0F; 
        public float TpsGenerationMoySec
        {
            get
            {
                return _tpsGenerationMoySec;
            }
            private set
            {
                _tpsGenerationMoySec = value;
                NotifyPropertyChanged("TpsGenerationMoySec");
            }
        }
        
        
        private float _tpsGenerationMaxSec = float.NaN; 
        public float TpsGenerationMaxSec
        {
            get
            {
                return _tpsGenerationMaxSec;
            }
            private set
            {
                _tpsGenerationMaxSec = value;
                NotifyPropertyChanged("TpsGenerationMaxSec");
            }
        }

        private int _nbGeneration = 0; 
        public int NbGeneration
        {
            get
            {
                return _nbGeneration;
            }
            private set
            {
                _nbGeneration = value;
                NotifyPropertyChanged("NbGeneration");
            }
        }

        private int _nbErreurGeneration = 0;
        public int NbErreurGeneration
        {
            get
            {
                return _nbErreurGeneration;
            }
            private set
            {
                _nbErreurGeneration = value;
                NotifyPropertyChanged("NbErreurGeneration");
            }
        }

        private float _delaiGenerationSec = 0F; 
        public float DelaiEntreGenerationSec
        {
            get
            {
                return _delaiGenerationSec;
            }
            private set
            {
                _delaiGenerationSec = value;
                NotifyPropertyChanged("DelaiEntreGenerationSec");
            }
        }

        private float _tpsSynchronisationCompleteMinSec = float.NaN;
        public float TpsSynchronisationCompleteMinSec
        {
            get
            {
                return _tpsSynchronisationCompleteMinSec;
            }
            private set
            {
                _tpsSynchronisationCompleteMinSec = value;
                NotifyPropertyChanged("TpsSynchronisationCompleteMinSec");
            }
        }

        private float _tpsSynchronisationCompleteMoySec = 0F;
        public float TpsSynchronisationCompleteMoySec
        {
            get
            {
                return _tpsSynchronisationCompleteMoySec;
            }
            private set
            {
                _tpsSynchronisationCompleteMoySec = value;
                NotifyPropertyChanged("TpsSynchronisationCompleteMoySec");
            }
        }

        private float _tpsSynchronisationCompleteMaxSec = float.NaN;
        public float TpsSynchronisationCompleteMaxSec
        {
            get
            {
                return _tpsSynchronisationCompleteMaxSec;
            }
            private set
            {
                _tpsSynchronisationCompleteMaxSec = value;
                NotifyPropertyChanged("TpsSynchronisationCompleteMaxSec");
            }
        }

        private int _nbSynchronisationDifference = 0;
        public int NbSynchronisationDifference
        {
            get
            {
                return _nbSynchronisationDifference;
            }
            private set
            {
                _nbSynchronisationDifference = value;
                NotifyPropertyChanged("NbSynchronisationDifference");
            }
        }

        private int _nbSynchronisationComplete = 0;
        public int NbSynchronisationComplete
        {
            get
            {
                return _nbSynchronisationComplete;
            }
            private set
            {
                _nbSynchronisationComplete = value;
                NotifyPropertyChanged("NbSynchronisationComplete");
            }
        }

        private int _nbErreurSynchronisationComplete = 0;
        public int NbErreurSynchronisationComplete
        {
            get
            {
                return _nbErreurSynchronisationComplete;
            }
            private set
            {
                _nbErreurSynchronisationComplete = value;
                NotifyPropertyChanged("NbErreurSynchronisationComplete");
            }
        }

        private float _nbMinFichiersSynchronisationComplete = float.NaN;
        public float NbMinFichiersSynchronisationComplete
        {
            get
            {
                return _nbMinFichiersSynchronisationComplete;
            }
            private set
            {
                _nbMinFichiersSynchronisationComplete = value;
                NotifyPropertyChanged("NbMinFichiersSynchronisationComplete");
            }
        }

        private float _nbMoyFichiersSynchronisationComplete = 0F;
        public float NbMoyFichiersSynchronisationComplete
        {
            get
            {
                return _nbMoyFichiersSynchronisationComplete;
            }
            private set
            {
                _nbMoyFichiersSynchronisationComplete = value;
                NotifyPropertyChanged("NbMoyFichiersSynchronisationComplete");
            }
        }

        private float _nbMaxFichiersSynchronisationComplete = float.NaN;
        public float NbMaxFichiersSynchronisationComplete
        {
            get
            {
                return _nbMaxFichiersSynchronisationComplete;
            }
            private set
            {
                _nbMaxFichiersSynchronisationComplete = value;
                NotifyPropertyChanged("NbMaxFichiersSynchronisationComplete");
            }
        }

        private float _tpsSynchronisationDifferenceMinSec = float.NaN;
        public float TpsSynchronisationDifferenceMinSec
        {
            get
            {
                return _tpsSynchronisationDifferenceMinSec;
            }
            private set
            {
                _tpsSynchronisationDifferenceMinSec = value;
                NotifyPropertyChanged("TpsSynchronisationDifferenceMinSec");
            }
        }

        private float _tpsSynchronisationDifferenceMoySec = 0F;
        public float TpsSynchronisationDifferenceMoySec
        {
            get
            {
                return _tpsSynchronisationDifferenceMoySec;
            }
            private set
            {
                _tpsSynchronisationDifferenceMoySec = value;
                NotifyPropertyChanged("TpsSynchronisationDifferenceMoySec");
            }
        }

        private float _tpsSynchronisationDifferenceMaxSec = float.NaN;
        public float TpsSynchronisationDifferenceMaxSec
        {
            get
            {
                return _tpsSynchronisationDifferenceMaxSec;
            }
            private set
            {
                _tpsSynchronisationDifferenceMaxSec = value;
                NotifyPropertyChanged("TpsSynchronisationDifferenceMaxSec");
            }
        }

        private int _nbErreurSynchronisationDifference = 0;
        public int NbErreurSynchronisationDifference
        {
            get
            {
                return _nbErreurSynchronisationDifference;
            }
            private set
            {
                _nbErreurSynchronisationDifference = value;
                NotifyPropertyChanged("NbErreurSynchronisationDifference");
            }
        }

        private float _nbMinFichiersSynchronisationDifference = float.NaN;
        public float NbMinFichiersSynchronisationDifference
        {
            get
            {
                return _nbMinFichiersSynchronisationDifference;
            }
            private set
            {
                _nbMinFichiersSynchronisationDifference = value;
                NotifyPropertyChanged("NbMinFichiersSynchronisationDifference");
            }
        }

        private float _nbMoyFichiersSynchronisationDifference = 0F;
        public float NbMoyFichiersSynchronisationDifference
        {
            get
            {
                return _nbMoyFichiersSynchronisationDifference;
            }
            private set
            {
                _nbMoyFichiersSynchronisationDifference = value;
                NotifyPropertyChanged("NbMoyFichiersSynchronisationDifference");
            }
        }

        private float _nbMaxFichiersSynchronisationDifference = float.NaN;
        public float NbMaxFichiersSynchronisationDifference
        {
            get
            {
                return _nbMaxFichiersSynchronisationDifference;
            }
            private set
            {
                _nbMaxFichiersSynchronisationDifference = value;
                NotifyPropertyChanged("NbMaxFichiersSynchronisationDifference");
            }
        }

        #endregion

        #region METHODES
        public void RegisterErreurGeneration()
        {
            NbErreurGeneration++;
        }

        public void RegisterGeneration(float duree) {
            _SumTpsGeneration += duree;
            NbGeneration++;

            TpsGenerationMoySec = (NbGeneration > 0) ? _SumTpsGeneration / NbGeneration : float.NaN;
            TpsGenerationMaxSec = Math.Max(TpsGenerationMaxSec, duree);
            TpsGenerationMinSec = Math.Min(TpsGenerationMinSec, duree);
        }

        public void RegisterDelaiGeneration(float delai)
        {
            _SumDelaiGeneration += delai;
            DelaiEntreGenerationSec = (NbGeneration > 0) ? _SumDelaiGeneration / NbGeneration : float.NaN;
        }

        public void RegisterSynchronisation(float duree, UploadStatus syncStatus)
        {
            if(syncStatus.IsComplet)
            {
                _SumTpsSynchronisationComplete  += duree;
                NbSynchronisationComplete++;
                if (!syncStatus.IsSuccess)
                {
                    NbErreurSynchronisationComplete++;
                }

                TpsSynchronisationCompleteMoySec = (NbSynchronisationComplete > 0) ? _SumTpsSynchronisationComplete / NbSynchronisationComplete : float.NaN;
                TpsSynchronisationCompleteMaxSec = Math.Max(TpsSynchronisationCompleteMaxSec, duree);
                TpsSynchronisationCompleteMinSec = Math.Min(TpsSynchronisationCompleteMinSec, duree);

                if(syncStatus.nbUpload >     0)
                {
                    _SumNbFichierSynchronisationComplete += syncStatus.nbUpload;
                    NbMoyFichiersSynchronisationComplete = (NbSynchronisationComplete > 0) ? _SumNbFichierSynchronisationComplete / NbSynchronisationComplete : float.NaN;
                    NbMaxFichiersSynchronisationComplete = Math.Max(NbMaxFichiersSynchronisationComplete, syncStatus.nbUpload);
                    NbMinFichiersSynchronisationComplete = Math.Min(NbMinFichiersSynchronisationComplete, syncStatus.nbUpload);
                }
            }
            else
            {
                _SumTpsSynchronisationDifference += duree;
                NbSynchronisationDifference++;
                if (!syncStatus.IsSuccess)
                {
                    NbErreurSynchronisationDifference++;
                }

                TpsSynchronisationDifferenceMoySec = (NbSynchronisationDifference > 0) ? _SumTpsSynchronisationDifference / NbSynchronisationDifference : float.NaN;
                TpsSynchronisationDifferenceMaxSec = Math.Max(TpsSynchronisationDifferenceMaxSec, duree);
                TpsSynchronisationDifferenceMinSec = Math.Min(TpsSynchronisationDifferenceMinSec, duree);

                if (syncStatus.nbUpload > 0)
                {
                    _SumNbFichierSynchronisationDifference += syncStatus.nbUpload;
                    NbMoyFichiersSynchronisationDifference = (NbSynchronisationDifference > 0) ? _SumNbFichierSynchronisationDifference / NbSynchronisationDifference : float.NaN;
                    NbMaxFichiersSynchronisationDifference = Math.Max(NbMaxFichiersSynchronisationDifference, syncStatus.nbUpload);
                    NbMinFichiersSynchronisationDifference = Math.Min(NbMinFichiersSynchronisationDifference, syncStatus.nbUpload);
                }
            }
        }

        #endregion

    }
}
