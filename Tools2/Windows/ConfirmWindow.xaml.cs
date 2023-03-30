using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Tools.Outils;

namespace Tools.Windows
{
    /// <summary>
    /// Logique d'interaction pour ChangeLogWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : RadWindow
    {
        public static readonly RoutedUICommand OKButton = new RoutedUICommand("OK", "OK", typeof(RadWindow));
        public static readonly RoutedUICommand CancelButton = new RoutedUICommand("Cancel", "Cancel", typeof(RadWindow));


        public ConfirmWindow(string header, string message)
        {
            InitializeComponent();
            OutilsTools.ShowInTaskbar(this);
            this.DialogResult = false;

            this.Header = header;
            LabelMessage.Text = message;

            InitCommand();
        }

        public ConfirmWindow(string message)
        {
            InitializeComponent();
            OutilsTools.ShowInTaskbar(this);
            this.DialogResult = false;

            this.Header = "";
            LabelMessage.Text = message;

            InitCommand();
        }

        public ConfirmWindow(DialogParameters param)
        {
            InitializeComponent();
            OutilsTools.ShowInTaskbar(this);
            this.DialogResult = false;

            this.Header = param.Header;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //this.WindowStartupLocation = param.DialogStartupLocation;
            LabelMessage.Text = param.Content as string;
            ButOkLabel.Content = param.OkButtonContent;
            ButAnnulerLabel.Content = param.CancelButtonContent;

            InitCommand();
        }

        private void ButOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void ButAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void InitCommand()
        {
            CommandBinding command1 = new CommandBinding() { Command = AlertWindow.OKButton };
            command1.Executed += this.CommandBinding_Ok;
            this.CommandBindings.Add(command1);
            this.InputBindings.Add(new KeyBinding() { Command = AlertWindow.OKButton, Key = Key.Enter });

            CommandBinding command2 = new CommandBinding() { Command = ConfirmWindow.CancelButton };
            command2.Executed += this.CommandBinding_Cancel;
            this.CommandBindings.Add(command2);
            this.InputBindings.Add(new KeyBinding() { Command = ConfirmWindow.CancelButton, Key = Key.Escape });
        }

        private void CommandBinding_Ok(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CommandBinding_Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
