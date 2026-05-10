using AppPublication.ViewModels.Configuration;
using HandyControl.Controls;

namespace AppPublication.Views.Configuration
{
    public partial class TestFtpWindow : Window
    {
        public TestFtpWindow(TestFtpViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (this.DataContext as TestFtpViewModel is TestFtpViewModel vm)
            {
                if (vm.CmdCancelTest.CanExecute(null))
                {
                    vm.CmdCancelTest.Execute(null);
                }
            }
        }
    }
}