#nullable enable
using AppPublication.Generation;
using Xunit;

namespace AppPublication.Tests.Generation
{
    public class GenerationProgressInfoTests
    {
        [Fact]
        public void InitInstance_ConfigureCorrectementLEtatInitial()
        {
            // Act
            GenerationProgressInfo info = GenerationProgressInfo.InitInstance(101, 50);

            // Assert
            Assert.Equal(101, info.Id);
            Assert.Equal(50, info.NbGeneration);
            Assert.Equal(-1, info.Progress);

            // Un objet fraîchement initialisé a IsInit = true et IsProgress = false
            Assert.True(info.IsInit);
            Assert.False(info.IsProgress);
        }

        [Fact]
        public void ProgressInstance_ConfigureCorrectementLEtatEnCours()
        {
            // Act
            GenerationProgressInfo info = GenerationProgressInfo.ProgressInstance(202, 15);

            // Assert
            Assert.Equal(202, info.Id);
            Assert.Equal(15, info.Progress);
            Assert.Equal(-1, info.NbGeneration);

            // Un objet en cours a IsInit = false et IsProgress = true
            Assert.False(info.IsInit);
            Assert.True(info.IsProgress);
        }
    }
}