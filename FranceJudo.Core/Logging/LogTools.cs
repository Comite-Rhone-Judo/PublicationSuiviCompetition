using FranceJudo.Core.Environment;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace FranceJudo.Core.Logging
{
    public static class LogTools
    {
        #region CONSTANTES
        private const string kloggingLevelVariable = "loggingLevel";
        private const string kDbgDataLoggerName = "dbgDataLogger";
        private const string kDefaultLoggerName = "defaultLogger";
        #endregion

        #region MEMBRES
        /// <summary>
        /// Define a static logger variable so that it references
        /// </summary>
        private static readonly Logger _logger = NLog.LogManager.GetLogger(kDefaultLoggerName);
        private static readonly Logger _dataLogger = NLog.LogManager.GetLogger(kDbgDataLoggerName);
        private static Layout _previousLogLevel = null;
        // private static Logger Logger { get { return _logger; } }

        #endregion

        #region DELEGATES
        public static event EventHandler<ExceptionEventArgs> OnCriticalErrorLogged;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Retourne l'etat de configuration du logger
        /// </summary>
        public static bool IsConfigured
        {
            get
            {
                return LogManager.Configuration != null;
            }
        }
        #endregion

        #region PROXY vers le Logger

        // Access direct au Logger NLog
        public static Logger Logger { get { return _logger; } }
        public static Logger DataLogger { get { return _dataLogger; } }

        public static void Error(string msg) { _logger.Error(msg); }

        public static void Error(Exception ex) { _logger.Error(ex); }

        public static void Warning(string msg) { _logger.Warn(msg); }

        public static void Warning(Exception ex) { _logger.Warn(ex); }

        public static void Info(string msg) { _logger.Info(msg); }

        public static void Info(Exception ex) { _logger.Info(ex); }

        public static void Fatal(string msg) { _logger.Fatal(msg); }

        public static void Fatal(Exception ex) { _logger.Fatal(ex); }

        public static void Debug(string msg) { _logger.Debug(msg); }

        public static void Debug(Exception ex) { _logger.Debug(ex); }

        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// Trace un flux XML en mode Debug. 
        /// La lourde sérialisation (ToString) n'est exécutée que si le log Debug est activé.
        /// </summary>
        /// <param name="xmlNode">Le nœud ou document XML à tracer</param>
        /// <param name="label">Un libellé optionnel pour identifier le log</param>
        public static void DebugLogData(XNode xmlNode)
        {
            DebugLogData("XML genere: '{0}'", xmlNode);
        }

        /// <summary>
        /// Trace un flux XML en mode Debug. 
        /// La lourde sérialisation (ToString) n'est exécutée que si le log Debug est activé.
        /// </summary>
        /// <param name="format">Le message de formatage (ex: "XML genere: '{0}'")</param>
        /// <param name="xmlNode">Le document (XDocument) ou l'élément (XElement) à tracer</param>
        public static void DebugLogData(string format, XNode xmlNode)
        {
            // On ne fait strictement rien en production
            if (DataLogger.IsDebugEnabled && xmlNode != null)
            {
                // La règle de formatage (sur une ligne) est centralisée ici !
                string xmlContent = xmlNode.ToString(SaveOptions.DisableFormatting);

                // On injecte le XML généré dans votre format ("XML genere: '{0}'")
                DataLogger.Debug(string.Format(format, xmlContent));
            }
        }

        /// <summary>
        /// Trace le demarrage de l'application
        /// </summary>
        public static void LogStartup()
        {
            _logger.Info("-----------------------------------------------------------------------------------------------------");
            _logger.Info("App Publication is starting - Version " + AppEnvironment.GetVersionInformation().ToString());
        }

        /// <summary>
        /// Trace l'arret de l'application
        /// </summary>
        public static void LogStop()
        {
            _logger.Info("App Publication is stopped");
            _logger.Info("-----------------------------------------------------------------------------------------------------");
        }

        /// <summary>
        /// Configure le niveau de trace au maximum si enable = true, au niveau configure dans le fichier sinon
        /// </summary>
        /// <param name="enable">Active (true) ou desactive (False) le niveau de trace</param>
        public static void ConfigureDebugLevel(bool enable)
        {
            try
            {
                if (enable)
                {
                    // Sauvegarde l'etat actuel du niveau de trace
                    _previousLogLevel = LogManager.Configuration.Variables[kloggingLevelVariable];

                    // Force le niveau de trace a Debug
                    LogManager.Configuration.Variables[kloggingLevelVariable] = LogLevel.Debug.ToString();

                    _logger.Info("Niveau de trace configure a Debug");
                }
                else
                {
                    // Remet en place le niveau de trace precedent
                    LogManager.Configuration.Variables[kloggingLevelVariable] = _previousLogLevel;

                    _logger.Info("Niveau de trace configure a {0}", _previousLogLevel);

                }

                LogManager.ReconfigExistingLoggers(); // Explicit refresh of Layouts and updates active Logger-objects
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Erreur lors de la modification de la configuration du logger");
            }
        }

        private static string _logDirectory;

        /// <summary>
        /// Propriete exposant le repertoire de trace extrait dynamiquement depuis le fichier de configuration
        /// </summary>
        public static string LogDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_logDirectory))
                {
                    _logDirectory = GetLogDirectory();
                }
                return _logDirectory;
            }
        }

        /// <summary>
        /// Emet un message d'alert (trace l'alerte dans le fichier de LOG)
        /// </summary>
        /// <param name="message"></param>
        public static void Alert(string message)
        {
            _logger.Error(message);
            OnCriticalErrorLogged?.Invoke(null, new ExceptionEventArgs(null, message));
        }

        /// <summary>
        /// Package le contenu du repertoire de log dans une archive compressee Zip
        /// </summary>
        /// <param name="targetArchiveName">Nom de l'archive cible (path absolu avec extension)</param>
        /// <param name="onlyToday">True pour ne prendre en compte que les fichiers du jour, False prend tous les fichiers</param>
        public static void PackageLog(string targetArchiveName, bool onlyToday = false)
        {
            try
            {
                // Recupere le repertoire de Log
                DirectoryInfo logDir = new DirectoryInfo(LogDirectory);

                // Recupere la liste des fichiers a prendre en compte
                List<FileInfo> logFiles = onlyToday ? logDir.GetFiles("*.*", SearchOption.TopDirectoryOnly).Where(f => f.CreationTime >= DateTime.Today).ToList() : logDir.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();

                // Ouvre l'archive Zip
                using (Stream stream = File.Open(targetArchiveName, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: false, entryNameEncoding: null))
                    {
                        // Ajoute les fichiers a l'archive Zip
                        foreach (FileInfo file in logFiles)
                        {
                            ZipArchiveEntry fileEntry = archive.CreateEntry(file.Name);
                            using (Stream outStream = fileEntry.Open())
                            {
                                using (Stream inStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    inStream.CopyTo(outStream);
                                }
                            }

                            // On ne peut pas utiliser ce code directement car le fichier est en cours d'utilisation
                            // archive.CreateEntryFromFile(file.FullName, file.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Impossible de creer l'archive Zip contenant les fichiers de trace de l'application vers '{0}'", targetArchiveName);
                throw new Exception("Impossible de creer l'archive Zip contenant les fichiers de trace de l'application", ex);
            }
        }

        #endregion

        #region METHODES PRIVEES
        /// <summary>
        /// Recherche le nom du fichier de trace dans le fichier de configuration NLog
        /// </summary>
        /// <param name="target">Le nom de la cible dans le fichier de trace, par defaut "logFile"</param>
        /// <returns>Le nom du repertoire cible</returns>
        private static string GetLogDirectory(string target = "logFile")
        {
            string output = string.Empty;

            try
            {
                // Extrait la configuration NLog pour trouver la cible demandee
                Target logTarget = LogManager.Configuration.FindTargetByName(target);
                if (logTarget != null && logTarget.GetType() == typeof(FileTarget))
                {
                    // Recupere le nom du fichier
                    FileTarget logFileTarget = (FileTarget)logTarget;
                    string logFileName = logFileTarget.FileName.Render(LogEventInfo.CreateNullEvent());

                    // Extrait les informations du fichier pour avoir le nom du repertoire parent
                    FileInfo info = new FileInfo(logFileName);

                    output = info.DirectoryName;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Impossible de lire la configuration NLog pour extraire le repertoire cible");
            }

            return output;
        }


        /// <summary>
        /// Méthode interne sécurisée pour déclencher l'événement
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        private static void RaiseCriticalError(Exception ex, string message)
        {
            // Le ?. permet de vérifier que quelqu'un est bien abonné à l'événement avant de l'appeler
            OnCriticalErrorLogged?.Invoke(null, new ExceptionEventArgs(ex, message));
        }
        #endregion
    }
}
