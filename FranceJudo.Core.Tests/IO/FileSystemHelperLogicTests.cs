using Xunit;
using FluentAssertions;
using FranceJudo.Core.IO;

namespace FranceJudo.Core.Tests.IO
{
    public class FileSystemHelperLogicTests
    {
        #region SizeSuffix

        [Theory]
        [InlineData(0, "0.0 bytes")]
        [InlineData(1024, "1.0 KB")]
        [InlineData(1536, "1.5 KB")] // 1024 + 512
        [InlineData(1048576, "1.0 MB")] // 1024 * 1024
        [InlineData(1073741824, "1.0 GB")]
        public void SizeSuffix_ValeursConduites_FormateCorrectement(ulong bytes, string expected)
        {
            // Act
            string result = bytes.SizeSuffix();

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region PathJoin

        [Theory]
        [InlineData("C:\\Dossier", "Fichier.txt", false, false, "C:\\Dossier\\Fichier.txt")]
        [InlineData("C:\\Dossier\\", "Fichier.txt", false, false, "C:\\Dossier\\Fichier.txt")] // Gère le slash en double
        [InlineData("C:\\Dossier", "\\Fichier.txt", false, false, "C:\\Dossier\\Fichier.txt")]
        [InlineData("C:\\Dossier", "Fichier.txt", true, false, "C:\\Dossier\\Fichier.txt\\")] // endWithSeparator
        [InlineData("/var/log", "app.log", false, true, "/var/log/app.log")] // unixStyle (Linux/Mac)
        [InlineData("", "Fichier.txt", false, false, "Fichier.txt")] // Path 1 vide
        public void PathJoin_DiversesCombinaisons_CombineProprement(string path1, string path2, bool endWithSep, bool unixStyle, string expected)
        {
            // Act
            string result = FileSystemHelper.PathJoin(path1, path2, endWithSep, unixStyle);

            // Assert
            result.Should().Be(expected);
        }

        #endregion
    }
}