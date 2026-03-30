using System;
using System.ComponentModel;

namespace FranceJudo.Core.Foundation
{
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        // Ce délégué servira de "prise" sur laquelle l'UI viendra se brancher au besoin
        public static Action OnPropertyModifiedGlobally { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // On signale au reste du monde (l'UI) qu'une propriété a changé
            OnPropertyModifiedGlobally?.Invoke();
        }
    }
}