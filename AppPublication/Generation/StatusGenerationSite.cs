using System;
using Tools.Framework;

namespace AppPublication.Generation
{
    public enum StateGenerationEnum
    {
        Stopped = -1,       // La generation est arretee
        None = 0,           // Aucun statut defini
        Idle = 1,           // La generation est en attente
        Generating = 2,     // La generation est en cours
        Cleaning = 3,       // Le nettoyage est en cours
        Starting = 4,       // La generation demarre
        Syncing = 5,        // La synchronisation demarre    
    }


    public class StatusGenerationSite : NotificationBase
    {
        #region CONSTRUCTEURS
        public StatusGenerationSite()
        {
            _nextGenSec = -1;
            _progress = -1;
            State = StateGenerationEnum.Stopped;
            // Message = "-";
        }

        public StatusGenerationSite(StateGenerationEnum pStatus)
        {
            State = pStatus;
        }

        #endregion

        #region PROPRIETES
        private StateGenerationEnum _state;
        /// <summary>
        /// Le statut de la generation
        /// </summary>
        public StateGenerationEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                NotifyPropertyChanged();
                if (_state != StateGenerationEnum.Idle)
                {
                    _nextGenSec = -1;
                }
                CalculMessage();
            }
        }

        private string _msg;
        /// <summary>
        /// Message associe au statut
        /// </summary>
        public string Message
        {
            get
            {
                return _msg;
            }
            private set
            {
                _msg = value;
                NotifyPropertyChanged();
            }
        }

        private int _progress;
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                NotifyPropertyChanged();

                if (_progress > -1)
                {
                    IsProgressUnknown = false;
                }
                else
                {
                    IsProgressUnknown = true;
                }
            }
        }

        private int _nextGenSec;
        public int NextGenerationSec
        {
            get
            {
                return _nextGenSec;
            }
            set
            {
                _nextGenSec = value;
                NotifyPropertyChanged();
                CalculMessage();
            }
        }

        private bool _progressunknown;
        public bool IsProgressUnknown
        {
            get
            {
                return _progressunknown;
            }
            private set
            {
                _progressunknown = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region METHODES PUBLIQUES
        public StatusGenerationSite Clone()
        {
            return this.MemberwiseClone() as StatusGenerationSite;
        }
        #endregion

        #region METHODES
        private void CalculMessage()
        {
            string msg = string.Empty;
            switch (_state)
            {
                case StateGenerationEnum.Stopped:
                    {
                        msg = "-";
                        break;
                    }
                case StateGenerationEnum.Idle:
                    {
                        if (NextGenerationSec > 0)
                        {
                            msg = String.Format("En attente ({0} sec.) ...", NextGenerationSec);
                        }
                        else
                        {
                            msg = "En attente ...";
                        }
                        break;
                    }
                case StateGenerationEnum.Generating:
                    {
                        msg = "Génération du site ...";
                        break;
                    }
                case StateGenerationEnum.Cleaning:
                    {
                        msg = "Nettoyage du site ...";
                        break;
                    }
                case StateGenerationEnum.Starting:
                    {
                        msg = "Démarrage ...";
                        break;
                    }
                default:
                    {
                        msg = "-";
                        break;
                    }
            }

            Message = msg;
        }

        /// <summary>
        /// Retourne une instance du status
        /// </summary>
        /// <param name="pState"></param>
        /// <returns></returns>
        public static StatusGenerationSite Instance(StateGenerationEnum pState)
        {
            return new StatusGenerationSite(pState);
        }
        #endregion
    }
}
