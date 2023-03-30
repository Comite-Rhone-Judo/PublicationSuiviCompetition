using AppPublication.Controles;
using System;
using System.Windows;
using Telerik.Windows.Controls;
using Tools.Outils;

namespace AppPublication.Tools
{
    public static class Outils
    {
        public static void InitDialogControleur()
        {
            /*
            if (DialogControleur.CS != null)
            {
                ((Window)DialogControleur.CS).Hide();
            }

            DialogControleur.CS = main_win;
            (DialogControleur.CS as Window).Show();
            */

            //FileAndDirectTools.InitDataDirectories();
            // DialogControleur.DC.InitControleur();
        }

        /*
         * public static void MI_In(bool IsBysy, string info1)
        {
            ICommissaireWindow window = DialogControleur.CS;
            Application.Current.ExecOnUiThread(new Action(() =>
            {
                StyleManager.SetTheme(window.RadBusyIndicator1, new Windows8Theme());

                window.RadBusyIndicator1.IsIndeterminate = true;
                window.RadBusyIndicator1.IsBusy = IsBysy;
                window.RadBusyIndicator1.BusyContent = info1;
            }));

            App.DoEvents();
        }

        private static DateTime _ref = DateTime.Now;
        public static void MI_Dert(int index, int maximum, string info1)
        {
            if ((DateTime.Now - _ref).TotalMilliseconds < 30 && index != maximum)
            {
                return;
            }

            _ref = DateTime.Now;
            ICommissaireWindow window = DialogControleur.CS;
            Application.Current.ExecOnUiThread(new Action(() =>
            {
                if (index != maximum)
                {
                    StyleManager.SetTheme(window.RadBusyIndicator1, new Expression_DarkTheme());

                    window.RadBusyIndicator1.IsIndeterminate = false;
                    window.RadBusyIndicator1.IsBusy = true;

                    window.RadBusyIndicator1.BusyContent = info1;
                    window.RadBusyIndicator1.ProgressValue = index * 100 / maximum;
                }
                else
                {
                    window.RadBusyIndicator1.IsBusy = false;
                }
            }));

            App.DoEvents();
        }
        */
    }
}
