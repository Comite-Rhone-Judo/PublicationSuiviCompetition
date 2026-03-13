using System.Windows;
using AppPublication.ViewModels.Configuration;

namespace AppPublication.Views.Configuration
{
    public partial class TestFtpWindow : Window
    {
        public TestFtpWindow(TestFtpViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Sécurité pour stopper un test s'il tourne pendant la fermeture de la fenêtre
            if (this.DataContext as TestFtpViewModel is TestFtpViewModel vm)
            {
                vm.ExecuteCancelTest();
            }
        }
    }
}