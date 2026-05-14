using System;
using System.Reflection;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.XML;
using FranceJudo.Core.Reflection;

namespace FranceJudo.Core.Tests.XML
{
    public class InAssemblyUrlResolverTests
    {
        // On crée une instance partagée qui pointe vers l'assembly de test en cours
        private readonly AssemblyResourceDictionary _testDictionary;

        public InAssemblyUrlResolverTests()
        {
            // L'assembly appelant est FranceJudo.Core.Tests.dll
            var testAssembly = Assembly.GetExecutingAssembly();
            _testDictionary = new AssemblyResourceDictionary(testAssembly);
        }

        [Fact]
        public void Constructeur_DictionnaireNull_LeveArgumentNullException()
        {
            // Act
            Action act = () => new InAssemblyUrlResolver(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("dictionary");
        }

        [Fact]
        public void ResolveUri_RetourneUriRelative()
        {
            // Arrange
            var resolver = new InAssemblyUrlResolver(_testDictionary);
            var baseUri = new Uri("http://dummy.com");
            string relativePath = "export.xslt";

            // Act
            Uri result = resolver.ResolveUri(baseUri, relativePath);

            // Assert
            result.Should().NotBeNull();
            result.IsAbsoluteUri.Should().BeFalse();
            result.OriginalString.Should().Be("export.xslt");
        }

        [Fact]
        public void GetEntity_RessourceIntrouvable_LeveArgumentOutOfRangeException()
        {
            // Arrange
            var resolver = new InAssemblyUrlResolver(_testDictionary);
            // On invente une URI qui n'existe absolument pas dans l'assembly de test
            var badUri = new Uri("Tools/FichierFantome.xslt", UriKind.RelativeOrAbsolute);

            // Act
            Action act = () => resolver.GetEntity(badUri, "role", typeof(object));

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("*Impossible de trouver la ressource XSLT liée*");
        }
    }
}