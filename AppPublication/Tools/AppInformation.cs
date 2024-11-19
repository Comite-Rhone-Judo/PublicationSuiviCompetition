using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Outils;

namespace AppPublication.Tools
{
    public class AppInformation : NotificationBase
    {
        private static AppInformation _instance = null;

        AppInformation()
        {
            AppVersion = OutilsTools.GetVersionInformation();
            AppCompany = OutilsTools.GetCompanyInformation();
            AppCopyright = OutilsTools.GetCopyrightInformation();
            AppTrademark = OutilsTools.GetTrademarkInformation();
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static AppInformation Instance
        {
            get
            {
                if( _instance == null ) {  _instance = new AppInformation(); }
                return _instance;
            }
        }

        private string _appVersion = string.Empty;
        public string AppVersion
        {
            get
            {
                return _appVersion;
            }
            private set
            {
                _appVersion = value;
                NotifyPropertyChanged("AppVersion");
            }
        }

        private string _appCompany = string.Empty;
        public string AppCompany
        {
            get
            {
                return _appCompany;
            }
            private set
            {
                _appCompany = value;
                NotifyPropertyChanged("AppCompany");
            }
        }

        private string _appCopyright = string.Empty;
        public string AppCopyright
        {
            get
            {
                return _appCopyright;
            }
            private set
            {
                _appCopyright = value;
                NotifyPropertyChanged("AppCopyright");
            }
        }

        private string _appTrademark = string.Empty;
        public string AppTrademark
        {
            get
            {
                return _appTrademark;
            }
            private set
            {
                _appTrademark = value;
                NotifyPropertyChanged("AppTrademark");
            }
        }

    }
}
