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
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // BasicConfigurator.Configure();
            log4net.Config.XmlConfigurator.Configure();

            LogTools.Trace("App is starting");

            CultureInfo culture = new CultureInfo("fr");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            //Window8Palette
            //Windows8Palette.Palette.AccentColor = Color.FromArgb(0xFF, 0x79, 0x25, 0x6B);
            //Windows8Palette.Palette.BasicColor = Color.FromArgb(0xFF, 0x79, 0x25, 0x6B);
            //Windows8Palette.Palette.StrongColor = Color.FromArgb(0xFF, 0x79, 0x25, 0x6B);
            //Windows8Palette.Palette.MainColor = Color.FromArgb(0xFF, 0x79, 0x25, 0x6B);
            //Windows8Palette.Palette.MarkerColor = Color.FromArgb(0xFF, 0x79, 0x25, 0x6B);
            //Windows8Palette.Palette.ValidationColor = Color.FromArgb(0xFF, 0x79, 0x25, 0x6B);            

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
            LogTools.Log(e.Exception);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }


    }
}
