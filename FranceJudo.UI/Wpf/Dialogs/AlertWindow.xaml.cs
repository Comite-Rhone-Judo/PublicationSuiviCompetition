using FranceJudo.UI.Wpf.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace FranceJudo.UI.Wpf.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : HandyControl.Controls.Window
    {
        /// <summary>
        /// Command OK
        /// </summary>
        public static readonly RoutedUICommand OKButton = new RoutedUICommand("OK", "OK", typeof(HandyControl.Controls.Window));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">header</param>
        /// <param name="message">message</param>

        public AlertWindow(string header, string message)
        {
            InitializeComponent();

            this.Title = header;
            LabelMessage.Text = message;
            ButOkLabel.Text = "OK";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitCommand();
        }

        private void ButOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void InitCommand()
        {
            CommandBinding command1 = new CommandBinding() { Command = AlertWindow.OKButton };
            command1.Executed += this.CommandBinding_Ok;
            this.CommandBindings.Add(command1);
            this.InputBindings.Add(new KeyBinding() { Command = AlertWindow.OKButton, Key = Key.Enter });
        }

        private void CommandBinding_Ok(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
