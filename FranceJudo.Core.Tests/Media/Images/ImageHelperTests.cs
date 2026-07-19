using System;
using System.Drawing;
using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Media.Images;

// Attention: Nécessite le package NuGet System.Drawing.Common pour .NET Core / .NET 10
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme (GDI+ est Windows-only, assumé pour WPF)

namespace FranceJudo.Core.Tests.Media.Images
{
    public class ImageHelperTests : IDisposable
    {
        private readonly string _tempImagePath;

        public ImageHelperTests()
        {
            // Création d'une image de test (un carré rouge de 100x100)
            _tempImagePath = Path.Combine(Path.GetTempPath(), $"TestImage_{Guid.NewGuid()}.png");
            using (var bitmap = new Bitmap(100, 100))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Red);
                }
                bitmap.Save(_tempImagePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public void Dispose()
        {
            // Nettoyage radical
            if (File.Exists(_tempImagePath))
            {
                File.Delete(_tempImagePath);
            }
        }

        #region Tests - Sérialisation Base64

        [Fact]
        public void ImageToString_PathNull_LeveArgumentNullException()
        {
            Action act = () => ImageHelper.ImageToString(null!);
            act.Should().Throw<ArgumentNullException>().WithParameterName("path");
        }

        [Fact]
        public void StringToImage_StringNull_LeveArgumentNullException()
        {
            Action act = () => ImageHelper.StringToImage(null!);
            act.Should().Throw<ArgumentNullException>().WithParameterName("imageString");
        }

        [Fact]
        public void Serialisation_Deserialisation_EstSymetrique()
        {
            // Act 1 : Fichier vers Base64
            string base64 = ImageHelper.ImageToString(_tempImagePath);

            // Assert 1
            base64.Should().NotBeNullOrEmpty();

            // Act 2 : Base64 vers Image mémoire
            using (Image restoredImage = ImageHelper.StringToImage(base64))
            {
                // Assert 2
                restoredImage.Should().NotBeNull();
                restoredImage.Width.Should().Be(100);
                restoredImage.Height.Should().Be(100);
            }
        }

        #endregion

        #region Tests - Dimensionnement (CreerImage)

        [Fact]
        public void CreerImage_TailleInferieureAuMax_RetourneNull()
        {
            // Arrange : L'image fait 100x100. On demande max 200x200.
            using (var fs = new FileStream(_tempImagePath, FileMode.Open, FileAccess.Read))
            {
                // Act
                // Ton code spécifie que si l'image est plus petite que le max, on retourne null.
                var resultStream = ImageHelper.CreerImage(fs, 200, 200, "");

                // Assert
                resultStream.Should().BeNull();
            }
        }

        [Fact]
        public void CreerImage_DepasseLeMax_RedimensionneCorrectement()
        {
            // Arrange : L'image fait 100x100. On demande max 50x50.
            using (var fs = new FileStream(_tempImagePath, FileMode.Open, FileAccess.Read))
            {
                // Act
                using (var resultStream = ImageHelper.CreerImage(fs, 50, 50, "JUDO"))
                {
                    // Assert
                    resultStream.Should().NotBeNull();

                    // On vérifie que le flux généré contient bien une image de 50x50
                    using (Image resizedImage = Image.FromStream(resultStream))
                    {
                        resizedImage.Width.Should().Be(50);
                        resizedImage.Height.Should().Be(50);
                    }
                }
            }
        }

        #endregion
    }
}