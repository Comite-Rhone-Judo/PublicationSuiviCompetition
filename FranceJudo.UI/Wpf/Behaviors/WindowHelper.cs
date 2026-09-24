using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FranceJudo.UI.Wpf.Behaviors
{
    public static class WindowHelper
    {

        /// <summary>
        /// Capture un élément visuel WPF et le place dans le presse-papier
        /// </summary>
        public static void CopyVisualToClipboard(FrameworkElement element)
        {
            if (element == null) return;

            try
            {
                // 1. Calculer la taille réelle de l'élément
                double width = element.ActualWidth;
                double height = element.ActualHeight;

                // Cas particulier : si l'élément n'est pas encore rendu
                if (width == 0 || height == 0) return;

                // 2. Créer un rendu bitmap de l'élément visuel (96 DPI standard)
                RenderTargetBitmap bmp = new RenderTargetBitmap(
                    (int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(element);

                // 3. Envoyer directement au presse-papier
                Clipboard.SetImage(bmp);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erreur lors de la copie de l'image.", ex);
            }
        }
    }
}
