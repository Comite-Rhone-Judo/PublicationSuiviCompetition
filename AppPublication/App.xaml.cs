using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Configuration;
using KernelImpl;
using AppPublication.Controles;
using FranceJudo.UI.Wpf.Dialogs;

namespace AppPublication
{ /// <summary>
  /// Logique d'interaction pour App.xaml
  /// </summary>
    public partial class App : Application
    {
        ConfigurationService _configSvc = null;
        
        #region PROPERTIES
        // Accès global aux données si strictement nécessaire
        public JudoData DataManager { get; private set; }
        #endregion
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            CultureInfo culture = new CultureInfo("fr");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            StyleManager.ApplicationTheme = new Windows8Theme();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LogTools.LogStartup();
            LogTools.OnCriticalErrorLogged += LogTools_OnCriticalErrorLogged;

            // Démarrage du Service de Configuration (le worker commence ici)
            _configSvc = ConfigurationService.CreateInstance();

            // Creation du gestionnaire de donnees. C'est le coeur de l'application
            LogTools.Logger.Debug("Creation du gestionnaire de donnees");
            DataManager = new JudoData();

            // Instanciation du controleur principal en lien avec le gestionnaire de donnees
            LogTools.Logger.Debug("Creation du controleur principal");
            DialogControleur.CreateInstance(DataManager);

            // Assure que le logger est bien configure
            DialogControleur.Instance.CanManageTracesDebug = LogTools.IsConfigured;

            // Demarre la fenetre principale et injecte le Dialog controleur en tant que DataContext
            AppPublication.Views.Main.MainView mainWin = new AppPublication.Views.Main.MainView
            {
                DataContext = Controles.DialogControleur.Instance
            };
            mainWin.Show();
        }



        private static readonly DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);

        /// <summary> 
        /// Processes all UI messages currently in the message queue.
        /// </summary>

        public static void DoEvents()
        {
            // Create new nested message pump.

            DispatcherFrame nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than the that of UI event messages.

            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Background, exitFrameCallback, nestedFrame);

            // pump the nested message loop, the nested message loop will
            // immediately process the messages left inside the message queue.

            Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback doesn't get finished, Abort it.

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Arrêt propre du Service de Configuration
            // Cela force l'arrêt du worker et une dernière sauvegarde synchrone sur disque.
            if (ConfigurationService.Instance != null)
            {
                (ConfigurationService.Instance as IDisposable)?.Dispose();
            }

            // Arrete les loggers
            LogTools.LogStop();
            NLog.LogManager.Shutdown();

            // TODO A ajouter pour le logger
            // DÉSABONNEMENT (Bonne pratique pour éviter les fuites de mémoire)
            // LogTools.OnCriticalErrorLogged -= LogTools_OnCriticalErrorLogged;


            base.OnExit(e);
        }

        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;

            // Exit the nested message loop.

            frame.Continue = false;
            return null;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            LogTools.Logger.Error(e.Exception, "Exception non geree ayant atteint le gestionnaire general:");

            // Prevent default unhandled exception processing
            e.Handled = true;
        }

         /// <summary>
        /// Cette méthode est appelée automatiquement quand LogTools.LogFatal() est exécuté avec notifyUser = true
        /// </summary>
        private void LogTools_OnCriticalErrorLogged(object sender, ExceptionEventArgs e)
        {
            // Sécurité : On s'assure d'être sur le thread de l'interface graphique (UI Thread)
            // C'est indispensable car l'erreur peut provenir d'un processus en arrière-plan (TCP, FTP, etc.)
            Application.Current.Dispatcher.Invoke(() => 
            {
                // C'est SEULEMENT ici que l'on utilise WPF et vos fenêtres personnalisées
                AlertWindow alert = new AlertWindow(
                    header: "Une erreur critique est survenue", 
                    message: $"{e.Message}\n\nDétails techniques : {e.Exception?.Message}"
                );
                alert.ShowDialog();
            });
        }
    }
}
