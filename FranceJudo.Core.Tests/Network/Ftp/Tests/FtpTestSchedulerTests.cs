#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network;
using FranceJudo.Core.Network.Ftp.Test;

namespace FranceJudo.Core.Tests.Network.Ftp.Test
{
    public class FtpTestSchedulerTests
    {
        // Un MiniSite factice requis par le constructeur du Scheduler
        private class StubMiniSite : MiniSite
        {
            public StubMiniSite() : base(local: false, instance: null)
            {
                // On peuple les propriétés obligatoires pour que GetAndConfigureFtpClient() ne crashe pas
                SiteFTPDistant = "127.0.0.1";
                LoginSiteFTPDistant = "admin";
                PasswordSiteFTPDistant = "secret";
            }
        }

        // Un Step factice dont on contrôle la réussite ou l'échec
        private class ControlledTestStep : FtpTestStepBase
        {
            private readonly bool _willSucceed;
            public ControlledTestStep(string name, bool willSucceed)
            {
                Name = name;
                _willSucceed = willSucceed;
            }

            public override bool Execute(IFtpConfiguration site, FtpClient client, CancellationToken token)
            {
                return _willSucceed;
            }
        }

        // Un Step factice qui attend d'être annulé pour tester la Cancellation
        private class LongRunningTestStep : FtpTestStepBase
        {
            public override bool Execute(IFtpConfiguration site, FtpClient client, CancellationToken token)
            {
                // Boucle qui simule un long travail réseau
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(50);
                }
                return false; // Échoue si annulé
            }
        }

        [Fact]
        public void Constructeur_InitialiseLesEtapesDansLeBonOrdre()
        {
            var scheduler = new FtpTestScheduler(new StubMiniSite());

            scheduler.TestSteps.Should().HaveCount(6, "Le workflow doit contenir exactement 6 étapes d'analyse.");
            scheduler.TestSteps[0].Should().BeOfType<DnsResolutionTest>();
            scheduler.TestSteps[5].Should().BeOfType<DisconnectTest>();
        }

        [Fact]
        public async Task StartTestsAsync_InterromptLaSequenceAuPremierEchec()
        {
            // Arrange
            var scheduler = new FtpTestScheduler(new StubMiniSite());

            // On remplace les vraies étapes par nos étapes contrôlées
            scheduler.TestSteps.Clear();
            var successStep = new ControlledTestStep("Etape 1 (OK)", true);
            var failStep = new ControlledTestStep("Etape 2 (FAIL)", false);
            var skippedStep = new ControlledTestStep("Etape 3 (SKIPPED)", true);

            scheduler.TestSteps.Add(successStep);
            scheduler.TestSteps.Add(failStep);
            scheduler.TestSteps.Add(skippedStep);

            // Act : On utilise la VRAIE méthode de ton fichier
            await scheduler.StartTestsAsync();

            // Assert
            successStep.Status.Should().Be(TestStatus.Success, "La première étape doit réussir.");
            failStep.Status.Should().Be(TestStatus.Failed, "La deuxième étape doit être marquée comme échouée.");
            skippedStep.Status.Should().Be(TestStatus.Pending, "L'ordonnanceur doit avoir brisé la boucle 'break' et ne jamais lancer la 3ème étape.");
        }

        [Fact(Timeout = 3000)] // Timeout de sécurité pour xUnit 3
        public async Task Cancel_ArreteExecutionEnCoursProprement()
        {
            // Arrange
            var scheduler = new FtpTestScheduler(new StubMiniSite());

            scheduler.TestSteps.Clear();
            var longStep = new LongRunningTestStep();
            scheduler.TestSteps.Add(longStep);

            // Act : On lance le test (qui va boucler)
            var runTask = scheduler.StartTestsAsync();

            // On attend un tout petit peu pour être sûr que la tâche est bien en mode "Running"
            await Task.Delay(100, TestContext.Current.CancellationToken);

            // On déclenche l'annulation logicielle
            scheduler.Cancel();

            // Assert : On attend que la tâche se termine (elle doit sortir proprement)
            await runTask;

            // CORRECTION : La tâche n'est pas allée au bout, elle a retourné false, donc le Scheduler la passe en Failed.
            longStep.Status.Should().Be(TestStatus.Failed, "L'étape annulée retourne false, l'ordonnanceur doit donc logiquement la marquer comme Failed.");
        }
    }
}