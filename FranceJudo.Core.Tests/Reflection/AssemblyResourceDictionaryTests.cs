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
        public void AllResources_RetourneLaListeCompleteDesRessources()
        {
            // Arrange
            var dict = new AssemblyResourceDictionary(_testAssembly);

            // Act
            var resources = dict.AllResources;

            // Assert
            resources.Should().NotBeNull("La liste des ressources ne doit jamais être nulle.");
        }

        [Fact]
        public void GetFullName_CheminVideOuNul_RetourneLeNamespaceDeBase()
        {
            // Arrange
            var dict = new AssemblyResourceDictionary(_testAssembly, "FranceJudo.Base");

            // Act & Assert
            dict.GetFullName(null!).Should().Be("FranceJudo.Base", "Un chemin nul doit retourner le root namespace.");
            dict.GetFullName(string.Empty).Should().Be("FranceJudo.Base", "Un chemin vide doit retourner le root namespace.");
        }

        [Fact]
        public void Exists_RessourceInexistante_RetourneFalse()
        {
            // Arrange
            var dict = new AssemblyResourceDictionary(_testAssembly);

            // Act
            var result = dict.Exists("FichierFantome.txt");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void FindByFolder_RajouteLePointEtFiltreCorrectement()
        {
            // Arrange
            var dict = new AssemblyResourceDictionary(_testAssembly);

            // Act
            // Même si le dossier n'existe pas, cela couvre la logique de formatage (ajout du '.') et le LINQ Where
            var resultSansPoint = dict.FindByFolder("DossierInexistant");
            var resultAvecPoint = dict.FindByFolder("DossierInexistant.");

            // Assert
            resultSansPoint.Should().BeEmpty();
            resultAvecPoint.Should().BeEmpty();
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