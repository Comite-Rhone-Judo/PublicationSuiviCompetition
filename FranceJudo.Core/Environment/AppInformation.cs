using FranceJudo.Core.Foundation;

namespace FranceJudo.Core.Environment
{
    public class AppInformation : NotificationBase
    {
        private static AppInformation _instance = null;

        AppInformation()
        {
            AppVersion = AppEnvironment.GetVersionInformation();
            AppCompany = AppEnvironment.GetCompanyInformation();
            AppCopyright = AppEnvironment.GetCopyrightInformation();
            AppTrademark = AppEnvironment.GetTrademarkInformation();
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static AppInformation Instance
        {
            get
            {
                _instance ??= new AppInformation();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

    }
}
