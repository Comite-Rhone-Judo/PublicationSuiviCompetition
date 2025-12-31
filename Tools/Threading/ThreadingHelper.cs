using System;
using System.Windows;
using System.Windows.Threading;

namespace Tools.Threading
{
    public static class ThreadingHelper
    {
        /// <summary>
        /// Execute un action dans le thread principale
        /// </summary>
        /// <param name="app"></param>
        /// <param name="action"></param>
        /// <param name="priority"></param>

        public static void ExecOnUiThread(this Application app, Action action, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (app == null)
            {
                return;
            }
            var dispatcher = app.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(priority, action);
        }
    }
}
