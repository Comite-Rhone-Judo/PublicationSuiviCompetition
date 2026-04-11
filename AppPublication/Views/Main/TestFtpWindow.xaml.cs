using AppPublication.ViewModels.Configuration;
using Telerik.Windows.Controls;

namespace AppPublication.Views.Configuration
{
    public partial class TestFtpWindow : RadWindow
    {
        public TestFtpWindow(TestFtpViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void RadWindow_Closed(object sender, WindowClosedEventArgs e)
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