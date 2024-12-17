namespace Tools.Outils
{
    public enum StateMiniSiteEnum
    {
        Stopped = -1,   // Le site est arrete
        Listening = 0,      // Le site local est demarre (en ecoute)
        Idle = 1,           // site distant est demarre mais en attente
        Syncing = 2,         // site distant en cours de synchronisation
        Cleaning = 3         // site distant en cours de nettoyage
    }


    public class StatusMiniSite : NotificationBase
    {
        #region CONSTRUCTEURS
        public StatusMiniSite()
        {
            State = StateMiniSiteEnum.Stopped;
            Message = "-";
            MessageDetaille = string.Empty;
            Progress = -1;
        }

        public StatusMiniSite(StateMiniSiteEnum pStatus, string pMsg = "-", string pMsgDet = "")
        {
            State = pStatus;
            Message = pMsg;
            MessageDetaille = pMsgDet;
            Progress = -1;
        }

        #endregion

        #region PROPRIETES
        private StateMiniSiteEnum _state;
        /// <summary>
        /// Le statut du minisite
        /// </summary>
        public StateMiniSiteEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                NotifyPropertyChanged();
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
            set
            {
                _msg = value;
                NotifyPropertyChanged();
            }
        }

        private string _msgDetaille;
        /// <summary>
        /// Message detaille associe au statut
        /// </summary>
        public string MessageDetaille
        {
            get
            {
                return _msgDetaille;
            }
            private set
            {
                _msgDetaille = value;
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
    }
}