using System.Windows;
using System.Windows.Controls;

// Indispensable pour que WPF aille chercher dans Themes/Generic.xaml
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)]

namespace FranceJudo.UI.Wpf.Controls
{
    public class BusyIndicator : ContentControl
    {
        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator),
                new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }

        // Affiche ou masque l'overlay complet
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false));

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        // Valeur de la barre (0 à 100)
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(BusyIndicator), new PropertyMetadata(0.0));

        public double ProgressValue
        {
            get => (double)GetValue(ProgressValueProperty);
            set => SetValue(ProgressValueProperty, value);
        }

        // Mode indéterminé (barre qui défile)
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(true));

        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        // Contenu texte (BusyStatus)
        public static readonly DependencyProperty BusyContentProperty =
            DependencyProperty.Register("BusyContent", typeof(object), typeof(BusyIndicator), new PropertyMetadata(null));

        public object BusyContent
        {
            get => GetValue(BusyContentProperty);
            set => SetValue(BusyContentProperty, value);
        }
    }
}