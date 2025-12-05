using System.ComponentModel;
using System.Windows.Input;

namespace Tools.Framework
{
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // use this to force calculation of CanExecute on RelayCommand
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
