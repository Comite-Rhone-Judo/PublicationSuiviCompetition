using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using QRCoder;

namespace FranceJudo.UI.Wpf.Converters // À adapter selon votre namespace
{
    public class StringToQrCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string texte = value as string;

            // Si le texte est vide (ex: le serveur n'est pas démarré), on ne retourne pas d'image
            if (string.IsNullOrWhiteSpace(texte))
                return null;

            // 1. Génération du QR Code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(texte, QRCodeGenerator.ECCLevel.L))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                // On récupère le QR code sous forme de tableau d'octets (Pixels per module = 20)
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                // 2. Conversion propre pour WPF
                return LoadImage(qrCodeImage);
            }
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            // INDISPENSABLE en WPF pour que l'image puisse être affichée dans l'interface UI
            image.Freeze();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}