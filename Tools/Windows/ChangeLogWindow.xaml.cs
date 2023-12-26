using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Tools.Outils;

namespace Tools.Windows
{
    /// <summary>
    /// Logique d'interaction pour ChangeLogWindow.xaml
    /// </summary>
    public partial class ChangeLogWindow : RadWindow
    {
        public static readonly RoutedUICommand OKButton = new RoutedUICommand("OK", "OK", typeof(RadWindow));


        public ChangeLogWindow(string header, string message)
        {
            InitializeComponent();
            OutilsTools.ShowInTaskbar(this);

            this.Header = header;
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
