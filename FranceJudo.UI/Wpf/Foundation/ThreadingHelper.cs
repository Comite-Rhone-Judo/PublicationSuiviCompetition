using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FranceJudo.UI.Wpf.Foundation
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
            if (app?.Dispatcher == null) return;

            if (app.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                // InvokeAsync est le successeur moderne de BeginInvok_
                _ = app.Dispatcher.InvokeAsync(action, priority);
            }
        }

        /// <summary>
        /// Version asynchrone pour pouvoir attendre la fin de l'exécution UI.
        /// Pratique pour .NET 10 et les flux de données modernes.
        /// </summary>
        public static async Task ExecOnUiThreadAsync(this Application app, Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (app?.Dispatcher == null) return;

            if (app.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                await app.Dispatcher.InvokeAsync(action, priority);
            }
        }
    }
}
