using Xunit;
using FluentAssertions;
using FranceJudo.Core.IO;
using System.IO;

namespace FranceJudo.Core.Tests.IO
{
    public class FileWithChecksumComparerTests
    {
        private readonly FileWithChecksumComparer _comparer = new FileWithChecksumComparer();

        [Fact]
        public void Equals_MemeReference_RetourneTrue()
        {
            var file = new FileWithChecksum { File = new FileInfo("test.txt"), Checksum = "ABC" };
            _comparer.Equals(file, file).Should().BeTrue();
        }

        [Fact]
        public void Equals_ValeursIdentiques_RetourneTrue()
        {
            var file1 = new FileWithChecksum { File = new FileInfo("C:\\data.xml"), Checksum = "12345" };
            var file2 = new FileWithChecksum { File = new FileInfo("C:\\data.xml"), Checksum = "12345" };

            _comparer.Equals(file1, file2).Should().BeTrue();
            // L'architecture .NET exige que si Equals est vrai, GetHashCode DOIT être identique
            _comparer.GetHashCode(file1).Should().Be(_comparer.GetHashCode(file2));
        }

        [Fact]
        public void Equals_ChecksumDifferent_RetourneFalse()
        {
            var file1 = new FileWithChecksum { File = new FileInfo("C:\\data.xml"), Checksum = "12345" };
            var file2 = new FileWithChecksum { File = new FileInfo("C:\\data.xml"), Checksum = "99999" };

            _comparer.Equals(file1, file2).Should().BeFalse();
        }

        [Fact]
        public void Equals_GestionDesNulls_NePlantePas()
        {
            var file = new FileWithChecksum { File = new FileInfo("test.txt"), Checksum = "ABC" };

            _comparer.Equals(null!, null!).Should().BeTrue();
            _comparer.Equals(file, null!).Should().BeFalse();
            _comparer.Equals(null!, file).Should().BeFalse();
        }
    }
}