#nullable enable
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Logos;

namespace KernelImpl.Tests.Noyau.Logos
{
    public class LogosSnapshotTests
    {
        [Fact]
        public void Constructeur_AvecSourceNulle_NePlantePas()
        {
            // Arrange & Act
            LogosSnapshot snapshot = new LogosSnapshot(null!);

            // Assert
            snapshot.Fede.Should().BeNull();
            snapshot.Ligue.Should().BeNull();
            snapshot.Sponsors.Should().BeNull();
        }

        [Fact]
        public void Constructeur_AvecSourceValide_CopieLesReferencesDesListes()
        {
            // Arrange
            DataLogos source = new DataLogos();

            // Act
            LogosSnapshot snapshot = new LogosSnapshot(source);

            // Assert
            snapshot.Fede.Should().BeSameAs(source.Fede);
            snapshot.Ligue.Should().BeSameAs(source.Ligue);
            snapshot.Sponsors.Should().BeSameAs(source.Sponsors);
        }
    }
}