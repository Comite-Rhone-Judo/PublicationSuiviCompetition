using System.Windows;
using Telerik.Windows.Controls;

namespace AppPublication.Controles
{
    public interface ICommissaireWindow
    {
        Window MainWindow { get; }

        // RadBusyIndicator RadBusyIndicator1 { get; }

        // void InitialisationControl();
        void InitQRCde();
    }
}
