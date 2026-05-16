#nullable enable
using System.Globalization;
using System.Windows.Media.Imaging;
using Xunit;
using FluentAssertions;
using FranceJudo.UI.Wpf.Converters;

namespace FranceJudo.UI.Tests.Wpf.Converters
{
    public class StringToQrCodeConverterTests : WpfTestBase
    {
        [Fact]
        public void Convert_TexteValide_RetourneUnBitmapImageGele()
        {
            RunInSTA(() =>
            {
                // Arrange
                var converter = new StringToQrCodeConverter();
                string qrContent = "https://www.francejudo.com";

                // Act
                var result = converter.Convert(qrContent, typeof(BitmapImage), null!, CultureInfo.InvariantCulture) as BitmapImage;

                // Assert
                result.Should().NotBeNull("Le convertisseur doit générer une image pour un texte valide.");
                result!.IsFrozen.Should().BeTrue("L'image doit être gelée (Freeze) pour être utilisable par l'UI WPF inter-threads.");
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Convert_TexteVideOuNull_RetourneNull(string? invalidInput)
        {
            RunInSTA(() =>
            {
                var converter = new StringToQrCodeConverter();

                var result = converter.Convert(invalidInput!, typeof(BitmapImage), null!, CultureInfo.InvariantCulture);

                result.Should().BeNull("Aucun QR Code ne doit être généré si la source est vide.");
            });
        }
    }
}