using System;
using System.Reflection;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Reflection;

namespace FranceJudo.Core.Tests.Reflection
{
    public class AssemblyResourceDictionaryTests
    {
        private readonly Assembly _testAssembly;

        public AssemblyResourceDictionaryTests()
        {
            _testAssembly = Assembly.GetExecutingAssembly();
        }

        [Fact]
        public void Constructeur_AssemblyNull_LeveArgumentNullException()
        {
            // Act
            Action act = () => new AssemblyResourceDictionary(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetFullName_AvecEtSansPrefixe_FormateCorrectement()
        {
            // Arrange
            var dict = new AssemblyResourceDictionary(_testAssembly, "FranceJudo.Tests");

            // Act & Assert
            dict.GetFullName("Images.logo.png").Should().Be("FranceJudo.Tests.Images.logo.png");
            // Si on passe un chemin déjà complet, il ne doit pas le doubler
            dict.GetFullName("FranceJudo.Tests.Fichier.xml").Should().Be("FranceJudo.Tests.Fichier.xml");
        }

        [Fact]
        public void GetStream_RessourceInexistante_RetourneNull()
        {
            // Arrange
            var dict = new AssemblyResourceDictionary(_testAssembly);

            // Act
            var stream = dict.GetStream("RessourceFantome.xslt");

            // Assert
            stream.Should().BeNull();
        }
    }
}