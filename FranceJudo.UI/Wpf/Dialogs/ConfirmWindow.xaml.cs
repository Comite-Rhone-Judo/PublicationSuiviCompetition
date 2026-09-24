using System.Windows;
using System.Windows.Input;

namespace FranceJudo.UI.Wpf.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : HandyControl.Controls.Window
    {
        // Correction : type typeof(ConfirmWindow) au lieu de RadWindow
        public static readonly RoutedUICommand OKButton = new RoutedUICommand("OK", "OK", typeof(ConfirmWindow));
        public static readonly RoutedUICommand CancelButton = new RoutedUICommand("Cancel", "Cancel", typeof(ConfirmWindow));

        public ConfirmWindow(string header, string message)
        {
            InitializeComponent();

            this.Title = header; // Remplace this.Header de Telerik
            LabelMessage.Text = message;

            InitCommand();
        }

        public ConfirmWindow(string message)
        {
            InitializeComponent();

            this.Title = "Confirmation"; // Titre par défaut
            LabelMessage.Text = message;

            InitCommand();
        }

        public ConfirmWindow(DialogParameters param)
        {
            InitializeComponent();

            this.Title = param.Header;
            this.WindowStartupLocation = param.DialogStartupLocation;
            LabelMessage.Text = param.Content;

            // Correction : Un TextBlock utilise .Text et non .Content
            ButOkLabel.Text = param.OkButtonContent;
            ButAnnulerLabel.Text = param.CancelButtonContent;

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
            // Correction : AlertWindow n'existait pas ici, on le remplace par ConfirmWindow
            CommandBinding command1 = new CommandBinding() { Command = ConfirmWindow.OKButton };
            command1.Executed += this.CommandBinding_Ok;
            this.CommandBindings.Add(command1);
            this.InputBindings.Add(new KeyBinding() { Command = ConfirmWindow.OKButton, Key = Key.Enter });

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