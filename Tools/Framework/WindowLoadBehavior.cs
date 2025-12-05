using System.Windows;
using System.Windows.Input;

namespace Tools.Framework
{
    public static class WindowLoadBehavior
    {
        // Déclaration de la propriété attachée "LoadedCommand"
        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.RegisterAttached(
                "LoadedCommand",
                typeof(ICommand),
                typeof(WindowLoadBehavior),
                new PropertyMetadata(null, OnLoadedCommandChanged));

        // Accesseurs standard pour XAML
        public static void SetLoadedCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(LoadedCommandProperty, value);
        }

        public static ICommand GetLoadedCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(LoadedCommandProperty);
        }

        // Callback lorsque la propriété change (attachement de l'événement)
        private static void OnLoadedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                // On se désabonne pour éviter les fuites de mémoire si la propriété change
                window.Loaded -= Window_Loaded;

                if (e.NewValue is ICommand)
                {
                    window.Loaded += Window_Loaded;
                }
            }
        }

        // Exécution de la commande lors de l'événement Loaded
        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;
            if (window != null)
            {
                ICommand command = GetLoadedCommand(window);
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }
    }
}