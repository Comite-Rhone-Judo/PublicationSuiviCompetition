using System;
using Tools.Outils;

namespace AppPublication.Tools
{
    public class StatExecution : NotificationBase
    {
        #region CONSTRUCTEUR
        public StatExecution()
        {
            DateDemarrage = DateTime.Now;
            DateFin = DateTime.Now;
            DelaiExecutionMs = 0;
        }
        #endregion

        #region PROPRIETES
        private DateTime _dateStart;
        public DateTime DateDemarrage
        {
            get
            {
                return _dateStart;
            }
            set
            {
                _dateStart = value;
                NotifyPropertyChanged("DateDemarrage");
            }

        }

        private DateTime _dateStop;
        public DateTime DateFin
        {
            get
            {
                return _dateStop;
            }
            set
            {
                _dateStop = value;
                NotifyPropertyChanged("DateFin");
            }
        }

        private DateTime _dateNext;
        public DateTime DateProchaineGeneration
        {
            get
            {
                return _dateNext;
            }
            set
            {
                _dateNext = value;
                NotifyPropertyChanged("DateProchaineGeneration");
            }
        }

        private long _delaiExecMs;
        public long DelaiExecutionMs
        {
            get
            {
                return _delaiExecMs;
            }
            set
            {
                _delaiExecMs = value;
                NotifyPropertyChanged("DelaiExecutionMs");
            }
        }
        #endregion
    }
}
