using System.Threading;
using FluentFTP;
using Tools.Framework;
using Tools.Net;

namespace AppPublication.Models.Publication.FtpTests
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
            set { _status = value; NotifyPropertyChanged("Status"); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; NotifyPropertyChanged("ErrorMessage"); }
        }

        // Signature purement synchrone avec FtpClient classique
        public abstract bool Execute(MiniSite site, FtpClient client, CancellationToken token);
    }
}