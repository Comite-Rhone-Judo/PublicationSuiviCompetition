#nullable enable
using System;
using System.IO;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Site;

namespace FranceJudo.Metier.Tests.Site
{
    public class UrlGeneratorBaseTests
    {
        // 1. Bouchon pour la structure physique
        private class DummyPhysicalStructure : PhysicalStructureBase
        {
            public DummyPhysicalStructure(string rootDir, string idCompetition)
                : base(rootDir, idCompetition) { IdCompetition = idCompetition; }
        }

        // 2. Bouchon pour le générateur d'URL
        private class TestUrlGenerator : UrlGeneratorBase<DummyPhysicalStructure>
        {
            public TestUrlGenerator(DummyPhysicalStructure ps, string domain) : base(ps, domain) { }

            protected override void BuildCompetitionUrl(string competitionId, Uri rootDomain, out string urlPath, out Uri baseUri)
            {
                // Implémentation simple pour les tests : on ajoute l'ID au domaine
                urlPath = competitionId;
                baseUri = new Uri(rootDomain, competitionId + "/");
            }
        }

        [Fact]
        public void Constructeur_AssureLeFormatDuDomaineRacine()
        {
            // Arrange
            var physical = new DummyPhysicalStructure(@"C:\Temp", "COMPET_TEST");

            // Act
            var generator = new TestUrlGenerator(physical, "http://www.francejudo.com");

            // Assert
            generator.RootDomain.Should().Be("http://www.francejudo.com/", "Le constructeur doit ajouter un '/' à la fin s'il est manquant.");
            generator.CompetitionBaseUri.ToString().Should().Be("http://www.francejudo.com/COMPET_TEST/");
        }

        [Fact]
        public void GetUrlFromPhysicalPath_CalculeCorrectementLUrlAbsolue()
        {
            // Arrange
            // Utilisation de chemins Windows standard pour le test du comportement de System.Uri
            string rootDir = @"C:\SiteJudo\";
            var physical = new DummyPhysicalStructure(rootDir, "CHAMPIONNAT");
            var generator = new TestUrlGenerator(physical, "http://localhost/");

            string fichierCible = Path.Combine(physical.RepertoireCompetition, @"css\style.css");

            // Act
            Uri absoluteUri = generator.GetUrlFromPhysicalPath(fichierCible);

            // Assert
            absoluteUri.ToString().Should().Be("http://localhost/CHAMPIONNAT/css/style.css", "L'URL absolue générée doit fusionner le domaine et le chemin relatif.");
        }

        [Fact]
        public void GetRelativeWebPath_CalculeLeCheminRelatifEntreDeuxFichiers()
        {
            // Arrange
            var physical = new DummyPhysicalStructure(@"C:\Site", "COMPET");
            var generator = new TestUrlGenerator(physical, "http://localhost/");

            // On imagine qu'on est dans une page XML/HTML située ici :
            string sourceFichier = @"C:\Site\COMPET\poules\poule1.xml";
            // Et on veut pointer vers le dossier CSS :
            string cibleDossier = @"C:\Site\COMPET\css";

            // Act
            // Important : isTargetDirectory = true est le défaut
            string relativePath = generator.GetRelativeWebPath(sourceFichier, cibleDossier);

            // Assert
            // Depuis "poules/poule1.xml", pour aller dans "css/", il faut remonter d'un cran : "../css/"
            relativePath.Should().Be("../css/", "Le générateur doit calculer le chemin relatif pour le XSLT.");
        }

        [Fact]
        public void ChangementDeStructure_InvalideLesCachesEtRecalcule()
        {
            // Arrange
            var physical1 = new DummyPhysicalStructure(@"C:\Site", "COMPET_A");
            var generator = new TestUrlGenerator(physical1, "http://localhost/");

            var oldUri = generator.CompetitionBaseUri;

            // Act
            var physical2 = new DummyPhysicalStructure(@"C:\Site", "COMPET_B");
            generator.PhysicalStructure = physical2; // Déclenche le setter

            // Assert
            generator.CompetitionBaseUri.ToString().Should().Be("http://localhost/COMPET_B/");
            generator.CompetitionBaseUri.Should().NotBe(oldUri, "L'affectation d'une nouvelle structure doit forcer le recalcul des URI de base.");
        }
    }
}