using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FranceJudo.Core.Network;


namespace FranceJudo.Core.Network.Ftp.Test
{ 
    public enum FtpTestSequence
    {
        DnsResolution = 10,
        ProfileCheck = 15,
        Connection = 20,
        RemoteDirectory = 30,
        FileTransfer = 40,
        Disconnect = 50
    }

    public class FtpTestScheduler
    {
        private readonly MiniSite _miniSite;
        private CancellationTokenSource _cts;

        // La collection des étapes exposée publiquement pour pouvoir y "binder" l'UI
        public ObservableCollection<FtpTestStepBase> TestSteps { get; private set; }

        public FtpTestScheduler(MiniSite miniSite)
        {
            _miniSite = miniSite;
            InitializeWorkflow();
        }

        private void InitializeWorkflow()
        {
            var testRegistry = new Dictionary<FtpTestSequence, FtpTestStepBase>
            {
                { FtpTestSequence.DnsResolution, new DnsResolutionTest() },
                { FtpTestSequence.ProfileCheck, new ProfileCheckTest() },
                { FtpTestSequence.Connection, new ConnectionTest() },
                { FtpTestSequence.RemoteDirectory, new RemoteDirectoryTest() },
                { FtpTestSequence.FileTransfer, new FileTransferTest() },
                { FtpTestSequence.Disconnect, new DisconnectTest() }
            };

            var orderedTests = testRegistry
                .OrderBy(kvp => (int)kvp.Key)
                .Select(kvp => kvp.Value);

            TestSteps = new ObservableCollection<FtpTestStepBase>(orderedTests);
        }

        public async Task StartTestsAsync()
        {
            // Réinitialisation
            foreach (var step in TestSteps)
            {
                step.Status = TestStatus.Pending;
                step.ErrorMessage = string.Empty;
            }

            _cts = new CancellationTokenSource();

            using (var ftpClient = _miniSite.GetAndConfigureFtpClient())
            {
                try
                {
                    foreach (var step in TestSteps)
                    {
                        if (_cts.IsCancellationRequested) break;

                        step.Status = TestStatus.Running;

                        // Exécution encapsulée pour protéger l'UI appelante
                        bool success = await Task.Run(() =>
                        {
                            if (_cts.IsCancellationRequested) return false;
                            return step.Execute(_miniSite, ftpClient, _cts.Token);
                        });

                        if (success)
                        {
                            step.Status = TestStatus.Success;
                        }
                        else
                        {
                            step.Status = TestStatus.Failed;
                            break;
                        }
                    }
                }
                catch (OperationCanceledException) { }
                finally
                {
                    if (ftpClient != null && ftpClient.IsConnected)
                    {
                        await Task.Run(() => ftpClient.Disconnect());
                    }
                    _cts?.Dispose();
                    _cts = null;
                }
            }
        }

        public void Cancel()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }
    }
}