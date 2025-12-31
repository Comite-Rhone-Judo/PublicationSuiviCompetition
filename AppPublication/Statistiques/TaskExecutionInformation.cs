using System;
using Tools.Framework;

namespace AppPublication.Statistiques
{
    public class TaskExecutionInformation : NotificationBase
    {
        #region CONSTRUCTEUR
        public TaskExecutionInformation()
        {
            DateDemarrage = DateTime.Now;
            DateProchaineGeneration = DateTime.MinValue;
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
