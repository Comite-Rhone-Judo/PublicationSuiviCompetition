using System.Threading;
using FluentFTP;
using FranceJudo.UI.Wpf.Foundation;

// TODO, doit aller dans leViewModel

namespace FranceJudo.UI.Wpf.ViewModels.Network.Ftp
{
    public enum TestStatus
    {
        Pending,
        Running,
        Success,
        Failed
    }

    public abstract class FtpTestStepBase : NotificationBase
    {
        public string Name { get; protected set; }

        private TestStatus _status = TestStatus.Pending;
        public TestStatus Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; NotifyPropertyChanged(); }
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get { return _successMessage; }
            set { _successMessage = value; NotifyPropertyChanged(); }
        }

        // Signature purement synchrone avec FtpClient classique
        public abstract bool Execute(MiniSite site, FtpClient client, CancellationToken token);
    }
}