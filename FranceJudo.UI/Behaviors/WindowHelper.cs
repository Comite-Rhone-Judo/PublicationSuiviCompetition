using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace FranceJudo.UI.Wpf.Behaviors
{
    public class WindowHelper
    {

        /// <summary>
        /// Affiche une RadWindow dans la TaskBar de Windows
        /// </summary>
        /// <param name="control"></param>

        public static void ShowInTaskbar(RadWindow control)
        {
            control.Loaded += new RoutedEventHandler(TaskbarRadWindow_Loaded);

        }

        static void TaskbarRadWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = ((RadWindow)sender).ParentOfType<System.Windows.Window>();
            if (window != null)
            {
                window.ShowInTaskbar = true;
                window.Title = ((RadWindow)sender).Header.ToString();
                //window.StateChanged += new EventHandler(window_StateChanged);
            }
        }

        static void window_StateChanged(object sender, EventArgs e)
        {
            var window = ((RadWindow)sender).ParentOfType<System.Windows.Window>();
            ((RadWindow)sender).WindowState = window.WindowState;
        }
    }
}
