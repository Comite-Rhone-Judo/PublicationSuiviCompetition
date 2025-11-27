using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Tools.Outils;

namespace AppPublication
{ /// <summary>
  /// Logique d'interaction pour App.xaml
  /// </summary>
    public partial class App : Application
    {
        ConfigurationService _configSvc = null;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Attention a l'ordre des initialisations !

            // LogTools.Trace("App is starting");
            LogTools.LogStartup();

            // Démarrage du Service de Configuration (le worker commence ici)
            // L'accès à .Instance suffit à démarrer le Singleton et le Worker
            _configSvc = ConfigurationService.Instance;

            // Demarrer le controleur et assure que le logger est bien configure
            Controles.DialogControleur.Instance.CanManageTracesDebug = LogTools.IsConfigured;

            CultureInfo culture = new CultureInfo("fr");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            StyleManager.ApplicationTheme = new Windows8Theme();
            // Controles.DialogControleur.DC = new Controles.DialogControleur();

            // Demarre la fenetre principale et injecte le Dialog controleur en tant que DataContext
            AppPublication.IHM.Commissaire.ExportWindow mainWin = new AppPublication.IHM.Commissaire.ExportWindow();
            mainWin.DataContext = Controles.DialogControleur.Instance;
            mainWin.Show();
        }



        private static DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);

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
                ConfigurationService.Instance.StopAndCommit();
                (ConfigurationService.Instance as IDisposable)?.Dispose();
            }

            // Arrete les loggers
            LogTools.LogStop();
            NLog.LogManager.Shutdown();

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


    }
}
