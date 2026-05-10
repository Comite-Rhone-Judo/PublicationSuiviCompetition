using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace FranceJudo.UI.Wpf.Dialogs
{
    public class DialogParameters
    {
        public string Header { get; set; } = "";
        public string Content { get; set; } = "";
        public string OkButtonContent { get; set; } = "Ok";
        public string CancelButtonContent { get; set; } = "Annuler";
        public WindowStartupLocation DialogStartupLocation { get; set; } = WindowStartupLocation.CenterScreen;
    }
}
