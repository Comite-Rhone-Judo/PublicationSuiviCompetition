using FranceJudo.UI.Wpf.Behaviors;
using System.Windows;
using System.Windows.Input;


namespace FranceJudo.UI.Wpf.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour ChangeLogWindow.xaml
    /// </summary>
    public partial class ChangeLogWindow : HandyControl.Controls.Window
    {
        public static readonly RoutedUICommand OKButton = new RoutedUICommand("OK", "OK", typeof(HandyControl.Controls.Window));


        public ChangeLogWindow(string header, string message)
        {
            InitializeComponent();

            this.Title = header;
            LabelMessage.Text = message;

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
