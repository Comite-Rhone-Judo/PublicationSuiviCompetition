using AppPublication.Controles;
using FranceJudo.Core.Configuration;
using FranceJudo.Core.Diagnostic;
using FranceJudo.Core.Logging;
using FranceJudo.UI.Wpf.Diagnostic;
using FranceJudo.UI.Wpf.Dialogs;
using FranceJudo.UI.Wpf.Foundation;
using HandyControl.Tools;
using KernelImpl;
using NLog;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Initialisation explicite de NLog
            // Charge la configuration depuis le fichier nlog.config autonome.
            // Cela peuple LogManager.Configuration et active les logs.
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");

            // HandyControl va automatiquement chercher et lier 
            // le dictionnaire du package HandyControl.Lang.fr
            ConfigHelper.Instance.SetLang("fr");

            // Demarrage et configure la couche de Logging
            LogTools.LogStartup();
            LogTools.OnCriticalErrorLogged += LogTools_OnCriticalErrorLogged;

            // 1. Démarrer le monitoring global (RAM, GC) toutes les 60 secondes
            HealthMonitor.StartSystemMonitoring(60);
            WpfHealthMonitor.StartWpfMonitoring(60);

            // Démarrage du Service de Configuration (le worker commence ici)
            _configSvc = ConfigurationService.CreateInstance();

            // Creation du gestionnaire de donnees. C'est le coeur de l'application
            LogTools.Logger?.Debug("Creation du gestionnaire de donnees");
            DataManager = new JudoData();

            // Instanciation du controleur principal en lien avec le gestionnaire de donnees
            LogTools.Logger?.Debug("Creation du controleur principal");
            DialogControleur.CreateInstance(DataManager);

            // Assure que le logger est bien configure
            DialogControleur.Instance.CanManageTracesDebug = LogTools.IsConfigured;

            // Configure la couche de notification des IHMs
            // Avertir le CommandManager de WPF (sur le thread de l'interface graphique)
            FranceJudo.Core.Foundation.NotificationBase.OnPropertyModifiedGlobally = () =>
            {
                Application.Current?.ExecOnUiThread(() => { System.Windows.Input.CommandManager.InvalidateRequerySuggested(); });
            };

            // Demarre la fenetre principale et injecte le Dialog controleur en tant que DataContext
            AppPublication.Views.Main.MainView mainWin = new AppPublication.Views.Main.MainView
            {
                DataContext = Controles.DialogControleur.Instance
            };

            // Modernisation du handler Loaded avec InvokeAsync
            mainWin.Loaded += async (s, ev) =>
            {
                // En .NET 10, on peut directement utiliser await sur le Dispatcher
                await this.Dispatcher.InvokeAsync(() =>
                {
                    WpfHealthMonitor.MonitorDispatcher(this.Dispatcher, "MainUI", 3000);
                }, DispatcherPriority.ContextIdle);
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

            // Remplacement de BeginInvok_ par InvokeAsync
            // On n'a plus besoin de stocker l'opération pour l'avorter manuellement 
            // car InvokeAsync est plus robuste avec le cycle de vie des Frames.
            _ = Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                nestedFrame.Continue = false;
            }, DispatcherPriority.Background);

            Dispatcher.PushFrame(nestedFrame);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // Arrêt propre du service de config (Flush synchrone)
                AppPublication.Config.AppConfigRoot.Stop();

                // Arrêt propre des timers à la fermeture
                HealthMonitor.StopAllMonitoring();
                WpfHealthMonitor.StopAllMonitoring();
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex);
            }
            finally
            {
                // Arrete les loggers
                LogTools.LogStop();
                NLog.LogManager.Shutdown();

                // DÉSABONNEMENT (Bonne pratique pour éviter les fuites de mémoire)
                LogTools.OnCriticalErrorLogged -= LogTools_OnCriticalErrorLogged;

                // Bonne pratique sous .NET moderne : on s'assure que tous les logs 
                // en attente (asynchrones) sont écrits avant la fermeture définitive.
                LogManager.Shutdown();

                base.OnExit(e);
            }
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
            LogTools.Logger?.Error(e.Exception, "Exception non geree ayant atteint le gestionnaire general:");

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
