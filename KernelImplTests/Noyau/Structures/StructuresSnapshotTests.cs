using FranceJudo.Metier.Noyau.Structures;
using KernelImpl.Noyau.Structures;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Structures
{
    public class StructuresSnapshotTests
    {
        [Fact]
        public void StructuresSnapshot_Constructor_ShouldCopyReferences()
        {
            // Arrange
            DataStructures sourceData = new DataStructures();

            // Act
            StructuresSnapshot snapshot = new StructuresSnapshot(sourceData);

            // Assert
            Assert.Same(sourceData.Clubs, snapshot.Clubs);
            Assert.Same(sourceData.Comites, snapshot.Comites);
            Assert.Same(sourceData.Ligues, snapshot.Ligues);
            Assert.Same(sourceData.Secteurs, snapshot.Secteurs);
            Assert.Same(sourceData.LesPays, snapshot.LesPays);
        }

        [Fact]
        public void StructuresSnapshot_Constructor_NullSource_ShouldNotThrow()
        {
            // Arrange
            DataStructures? sourceData = null;

            // Act
            StructuresSnapshot snapshot = new StructuresSnapshot(sourceData);

            // Assert
            Assert.Null(snapshot.Clubs);
            Assert.Null(snapshot.Comites);
            Assert.Null(snapshot.Ligues);
            Assert.Null(snapshot.Secteurs);
            Assert.Null(snapshot.LesPays);
        }
    }
}