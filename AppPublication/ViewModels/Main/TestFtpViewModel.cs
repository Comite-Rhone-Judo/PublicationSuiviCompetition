using System.Collections.ObjectModel;
using System.Windows.Input;
using FranceJudo.UI.Wpf.Foundation;
using FranceJudo.UI.Wpf.ViewModels.Network;
using FranceJudo.UI.Wpf.ViewModels.Network.Ftp;


namespace AppPublication.ViewModels.Configuration
{
    public class TestFtpViewModel : NotificationBase
    {
        private readonly FtpTestScheduler _scheduler;

        private ICommand _cmdStartTest = null;
        private ICommand _cmdCancelTest = null;
        private bool _isTestRunning;

        // Le ViewModel "passe-plat" la collection du scheduler à la Vue
        public ObservableCollection<FtpTestStepBase> TestSteps => _scheduler.TestSteps;

        public bool IsTestRunning
        {
            get { return _isTestRunning; }
            set
            {
                _isTestRunning = value;
                NotifyPropertyChanged("IsTestRunning");
                // Indique à WPF de rafraichir le bouton CanExecute
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public TestFtpViewModel(MiniSite miniSite)
        {
            // Instanciation de la logique métier
            _scheduler = new FtpTestScheduler(miniSite);
        }

        public ICommand CmdStartTest
        {
            get
            {
                if (_cmdStartTest == null)
                {
                    _cmdStartTest = new RelayCommand(
                        async o =>
                        {
                            IsTestRunning = true;

                            // On attend que la tâche de fond gérée par le métier finisse
                            await _scheduler.StartTestsAsync();

                            IsTestRunning = false;
                        },
                        o =>
                        {
                            return !IsTestRunning;
                        }
                    );
                }
                return _cmdStartTest;
            }
        }

        public ICommand CmdCancelTest
        {
            get
            {
                if (_cmdCancelTest == null)
                {
                    _cmdCancelTest = new RelayCommand(
                        o =>
                        {
                            _scheduler.Cancel();
                        },
                        o =>
                        {
                            return IsTestRunning;
                        }
                    );
                }
                return _cmdCancelTest;
            }
        }
    }
}