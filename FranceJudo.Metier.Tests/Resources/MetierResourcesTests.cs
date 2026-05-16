#nullable enable
using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Resources;

namespace FranceJudo.Metier.Tests.Resources
{
    public class MetierResourcesTests
    {
        [Fact]
        public void GetPublicationFFJudoXml_RetourneUnFluxValide()
        {
            // Act
            using Stream? stream = MetierResources.GetPublicationFFJudoXml();

            // Assert
            stream.Should().NotBeNull("Le fichier PublicationFFJudo.xml doit être présent en tant que ressource incorporée dans la DLL.");
            stream!.Length.Should().BeGreaterThan(0, "Le flux du fichier XML ne doit pas être vide.");
        }

        [Fact]
        public void GetStructuresXml_RetourneUnFluxValide()
        {
            // Act
            using Stream? stream = MetierResources.GetStructuresXml();

            // Assert
            stream.Should().NotBeNull("Le fichier structures.xml doit être présent en tant que ressource incorporée dans la DLL.");
            stream!.Length.Should().BeGreaterThan(0, "Le flux des structures ne doit pas être vide.");
        }

        [Fact]
        public void GetDefaultLogo_RetourneUnFluxValide()
        {
            // Act
            using Stream? stream = MetierResources.GetDefaultLogo();

            // Assert
            stream.Should().NotBeNull("L'image logo-France-Judo.png doit être présente dans le dossier SiteImg en tant que ressource incorporée.");
            stream!.Length.Should().BeGreaterThan(0, "Le flux de l'image ne doit pas être vide.");
        }
    }
}