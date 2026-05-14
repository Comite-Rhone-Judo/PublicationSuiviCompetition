using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace FranceJudo.Core.Tests.Logging
{
    // On désactive la parallélisation pour cette classe car LogManager et LogTools ont des états statiques
    [CollectionDefinition("NLog Sequential", DisableParallelization = true)]
    [Collection("NLog Sequential")]
    public class LogToolsTests : IDisposable
    {
        private readonly string _expectedDirectory;
        private readonly MemoryTarget _memoryTarget;
        private readonly string _initialLogLevel = "Info";

        public LogToolsTests()
        {
            // 1. Purge du cache statique interne de LogTools avant chaque test
            typeof(LogTools).GetField("_logDirectory", BindingFlags.Static | BindingFlags.NonPublic)
                            ?.SetValue(null, null);

            // 2. Création d'une configuration NLog 100% en RAM
            var config = new LoggingConfiguration();
            config.Variables["loggingLevel"] = _initialLogLevel;

            // 3. Dossier temporaire unique sur le disque physique pour les logs et ZIPs
            _expectedDirectory = Path.Combine(Path.GetTempPath(), "FranceJudo_Logs_Test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_expectedDirectory);

            var fileTarget = new FileTarget("logFile") { FileName = Path.Combine(_expectedDirectory, "trace.log") };
            config.AddTarget(fileTarget);

            // 4. Cible Mémoire pour espionner les traces sans I/O disque
            _memoryTarget = new MemoryTarget("memoryTarget");
            config.AddTarget(_memoryTarget);

            config.AddRuleForAllLevels(_memoryTarget);
            config.AddRuleForAllLevels(fileTarget, "*defaultLogger");

            LogManager.Configuration = config;
        }

        public void Dispose()
        {
            LogManager.Configuration = null;
            LogManager.Shutdown();

            // Nettoyage complet du disque
            if (Directory.Exists(_expectedDirectory))
            {
                Directory.Delete(_expectedDirectory, true);
            }
        }

        #region Tests - Configuration & Extraction de dossier

        [Fact]
        public void IsConfigured_ConfigurationPresente_RetourneTrue()
        {
            LogTools.IsConfigured.Should().BeTrue();
        }

        [Fact]
        public void ConfigureDebugLevel_EnableTrue_PasseLeNiveauEnDebug()
        {
            LogTools.ConfigureDebugLevel(true);

            var currentLevel = LogManager.Configuration?.Variables["loggingLevel"].Render(LogEventInfo.CreateNullEvent());
            currentLevel.Should().Be(LogLevel.Debug.ToString());
        }

        [Fact]
        public void ConfigureDebugLevel_EnableFalse_RestaureLeNiveauPrecedent()
        {
            LogManager.Configuration?.Variables["loggingLevel"] = _initialLogLevel;

            LogTools.ConfigureDebugLevel(true);
            LogTools.ConfigureDebugLevel(false);

            var restoredLevel = LogManager.Configuration?.Variables["loggingLevel"].Render(LogEventInfo.CreateNullEvent());
            restoredLevel.Should().Be(_initialLogLevel);
        }

        [Fact]
        public void LogDirectory_ExtractionDepuisConfiguration_RetourneLeBonDossier()
        {
            string directory = LogTools.LogDirectory;
            directory.Should().BeEquivalentTo(_expectedDirectory);
        }

        [Fact]
        public void LogDirectory_CibleIntrouvable_RetourneChaineVide()
        {
            // Arrange : On sabote la configuration NLog en retirant la cible "logFile"
            LogManager.Configuration?.RemoveTarget("logFile");
            LogManager.ReconfigExistingLoggers();

            // Act : L'appel va échouer silencieusement (catch) dans la méthode GetLogDirectory
            string directory = LogTools.LogDirectory;

            // Assert
            directory.Should().Be(string.Empty, "Si la configuration ne contient pas de cible 'logFile', la méthode doit retourner une chaîne vide sans crasher l'application.");
        }

        #endregion

        #region Tests - Alertes (OnCriticalErrorLogged)

        [Fact]
        public void Alert_DeclencheL_Evenement_OnCriticalErrorLogged()
        {
            bool eventFired = false;
            string messageAttendu = "Test d'erreur critique";
            EventHandler<ExceptionEventArgs> handler = (sender, args) =>
            {
                eventFired = true;
                args.Message.Should().Be(messageAttendu);
            };

            LogTools.OnCriticalErrorLogged += handler;
            try
            {
                LogTools.Alert(messageAttendu);
                eventFired.Should().BeTrue();
            }
            finally
            {
                LogTools.OnCriticalErrorLogged -= handler;
            }
        }

        #endregion

        #region Tests - Mode Debug et Performances XML

        [Fact]
        public void DebugLogData_ModeInfo_NeTraceRienEtEviteLaSurcharge()
        {
            LogManager.Configuration?.LoggingRules.Clear();
            LogManager.Configuration?.AddRule(LogLevel.Info, LogLevel.Fatal, _memoryTarget, "dbgDataLogger");
            LogManager.ReconfigExistingLoggers();

            var xmlTest = new XElement("Judoka", new XAttribute("Nom", "Riner"));

            LogTools.DebugLogData(xmlTest);

            _memoryTarget.Logs.Should().BeEmpty("En mode Info, le log des données XML doit être purement ignoré.");
        }

        [Fact]
        public void DebugLogData_ModeDebug_TraceLeXmlSansFormatage()
        {
            LogManager.Configuration?.LoggingRules.Clear();
            LogManager.Configuration?.AddRule(LogLevel.Debug, LogLevel.Fatal, _memoryTarget, "dbgDataLogger");
            LogManager.ReconfigExistingLoggers();

            var xmlTest = new XElement("Combat", new XAttribute("Id", "42"));
            string formatAttendu = "XML genere: '{0}'";

            LogTools.DebugLogData(formatAttendu, xmlTest);

            _memoryTarget.Logs.Should().HaveCount(1);
            string logRecu = _memoryTarget.Logs.First();

            logRecu.Should().Contain("Combat").And.Contain("Id=\"42\"");
            logRecu.Should().NotContain("\n").And.NotContain("\r", "Le formatage XML doit être désactivé.");
        }

        [Fact]
        public void DebugLogData_XmlNull_NePlantePas()
        {
            LogManager.Configuration?.LoggingRules.Clear();
            LogManager.Configuration?.AddRuleForAllLevels(_memoryTarget);
            LogManager.ReconfigExistingLoggers();

            XNode? noeudNull = null;

            Action act = () => LogTools.DebugLogData("Format {0}", noeudNull!);

            act.Should().NotThrow();
            _memoryTarget.Logs.Should().BeEmpty();
        }

        #endregion

        #region Tests - PackageLog (Compression ZIP)

        [Fact]
        public void PackageLog_SeulementAujourdHuiFalse_ArchiveTousLesFichiers()
        {
            string file1 = Path.Combine(_expectedDirectory, "log1.txt");
            string file2 = Path.Combine(_expectedDirectory, "log2.txt");
            File.WriteAllText(file1, "Trace 1");
            File.WriteAllText(file2, "Trace 2");

            string archivePath = Path.Combine(_expectedDirectory, "archive_complete.zip");

            LogTools.PackageLog(archivePath, onlyToday: false);

            File.Exists(archivePath).Should().BeTrue();

            using (var archiveStream = File.OpenRead(archivePath))
            using (var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read))
            {
                zip.Entries.Should().HaveCount(2);
                zip.Entries.Select(e => e.Name).Should().Contain(new[] { "log1.txt", "log2.txt" });
            }
        }

        [Fact]
        public void PackageLog_SeulementAujourdHuiTrue_IgnoreLesFichiersAnciens()
        {
            string fileToday = Path.Combine(_expectedDirectory, "today.log");
            string fileYesterday = Path.Combine(_expectedDirectory, "yesterday.log");

            File.WriteAllText(fileToday, "Trace récente");
            File.WriteAllText(fileYesterday, "Trace ancienne");

            File.SetCreationTime(fileYesterday, DateTime.Today.AddDays(-1));
            File.SetCreationTime(fileToday, DateTime.Now);

            string archivePath = Path.Combine(_expectedDirectory, "archive_today.zip");

            LogTools.PackageLog(archivePath, onlyToday: true);

            using (var archiveStream = File.OpenRead(archivePath))
            using (var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read))
            {
                zip.Entries.Should().HaveCount(1);
                zip.Entries.First().Name.Should().Be("today.log");
            }
        }

        [Fact]
        public void PackageLog_ErreurSysteme_LeveExceptionEnveloppee()
        {
            string cheminImpossible = @"Z:\DossierFantome\archive.zip";

            Action act = () => LogTools.PackageLog(cheminImpossible);

            act.Should().Throw<Exception>()
               .WithMessage("Impossible de creer l'archive Zip contenant les fichiers de trace de l'application");
        }

        #endregion
    }
}