#nullable enable
using FranceJudo.Metier.Noyau.Participants;
using KernelImpl.Noyau.Participants;
using System.Collections.Generic;
using Xunit;

namespace KernelImpl.Tests.Noyau.Participants
{
    public class ParticipantsSnapshotTests
    {
        [Fact]
        public void ParticipantsSnapshot_Constructor_ShouldCopyReferencesFromSource()
        {
            // Arrange
            DataParticipants sourceData = new DataParticipants();

            // Act
            ParticipantsSnapshot snapshot = new ParticipantsSnapshot(sourceData);

            // Assert
            Assert.NotNull(snapshot.Judokas);
            Assert.NotNull(snapshot.Equipes);
            Assert.NotNull(snapshot.EpreuveJudokas);
            Assert.NotNull(snapshot.Vuejudokas);

            Assert.Same(sourceData.Judokas, snapshot.Judokas);
            Assert.Same(sourceData.Equipes, snapshot.Equipes);
            Assert.Same(sourceData.EpreuveJudokas, snapshot.EpreuveJudokas);
            Assert.Same(sourceData.Vuejudokas, snapshot.Vuejudokas);
        }

        [Fact]
        public void ParticipantsSnapshot_Constructor_NullSource_ShouldNotThrow()
        {
            // Arrange
            DataParticipants? sourceData = null;

            // Act
            ParticipantsSnapshot snapshot = new ParticipantsSnapshot(sourceData);

            // Assert
            Assert.Null(snapshot.Judokas);
            Assert.Null(snapshot.Equipes);
            Assert.Null(snapshot.EpreuveJudokas);
            Assert.Null(snapshot.Vuejudokas);
        }
    }
}