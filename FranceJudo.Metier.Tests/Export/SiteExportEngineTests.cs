#nullable enable
using System;
using System.IO;
using System.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Export;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Site;

namespace FranceJudo.Metier.Tests.Export
{
    [Collection("IO_Tests")]
    public class SiteExportEngineTests : IDisposable
    {
        private readonly string _tempRootDir;

        // --- STUBS POUR LES TESTS ---
        private class DummyPhysicalStructure : PhysicalStructureBase
        {
            public DummyPhysicalStructure(string rootDir, string idCompetition)
                : base(rootDir, idCompetition) { IdCompetition = idCompetition; }
        }

        private class DummyUrlGenerator : UrlGeneratorBase<DummyPhysicalStructure>
        {
            public DummyUrlGenerator(DummyPhysicalStructure ps, string domain)
                : base(ps, domain) { }

            protected override void BuildCompetitionUrl(string competitionId, Uri rootDomain, out string urlPath, out Uri baseUri)
            {
                urlPath = competitionId;
                baseUri = new Uri(rootDomain, competitionId + "/");
            }
        }
        // ----------------------------

        public SiteExportEngineTests()
        {
            // Initialisation d'un espace de travail I/O isolé
            _tempRootDir = Path.Combine(Path.GetTempPath(), "SiteExportEngineTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRootDir);

            // Requis car EnumerateCustomLogoFiles() lit dans AppDirectoryManager.RessoucesImgDir
            AppDirectoryManager.Initialize(_tempRootDir, "");
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempRootDir))
            {
                try { Directory.Delete(_tempRootDir, true); } catch { }
            }
        }

        [Fact]
        public void GetFileName_TypeConnu_RetourneLeNomDefiniDansLeRegistre()
        {
            // Act
            string fileName = SiteExportEngine.GetFileName(ExportEnum.Site_Index);

            // Assert
            fileName.Should().Be("index", "Le registre indique 'index' pour Site_Index.");
        }

        [Fact]
        public void GetFileName_TypeInconnu_LeveUneArgumentOutOfRangeException()
        {
            // Arrange
            ExportEnum typeInconnu = (ExportEnum)9999;

            // Act
            Action act = () => SiteExportEngine.GetFileName(typeInconnu);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>("Le registre doit rejeter les énumérations non configurées.");
        }

        [Fact]
        public void GenererHtmlSite_TypeInconnu_BloqueLExecutionAvantAppelTechnique()
        {
            // Arrange
            ExportEnum typeInconnu = (ExportEnum)9999;

            // On type explicitement le null pour lever l'ambiguïté entre XmlSource et XPathDocument
            System.Xml.XPath.XPathDocument? dummyXml = null;

            // Act
            Action act = () => SiteExportEngine.GenererHtmlSite(dummyXml!, typeInconnu, "C:\\fake.html", null!);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>("GetXsltResourcePath doit intercepter le type inconnu avant de lancer le moteur XSLT.");
        }

        [Fact]
        public void EnumerateCustomLogoFiles_FiltreCorrectementLesFichiersPngAvecLogoDansLeNom()
        {
            // Arrange
            string targetDir = AppDirectoryManager.RessoucesImgDir;

            // CORRECTION : On vide le répertoire des ressources extraites par défaut lors du Initialize().
            // Cela garantit un environnement stérile pour tester uniquement notre filtre.
            foreach (var file in Directory.GetFiles(targetDir))
            {
                File.Delete(file);
            }

            // Fichiers valides (doivent être trouvés)
            File.WriteAllText(Path.Combine(targetDir, "mon_logo_club.png"), "fake data");
            File.WriteAllText(Path.Combine(targetDir, "LOGO_fede.png"), "fake data"); // Doit gérer la casse

            // Fichiers invalides (doivent être ignorés)
            File.WriteAllText(Path.Combine(targetDir, "image_club.png"), "fake data"); // Pas de 'logo'
            File.WriteAllText(Path.Combine(targetDir, "logo_doc.txt"), "fake data"); // Mauvaise extension

            // Act
            var logos = SiteExportEngine.EnumerateCustomLogoFiles();

            // Assert
            logos.Should().NotBeNull();
            logos.Should().HaveCount(2, "Seuls les fichiers .png contenant 'logo' ajoutés manuellement doivent être retournés.");
            logos.Select(l => l.Name.ToLower()).Should().Contain("mon_logo_club.png");
            logos.Select(l => l.Name.ToLower()).Should().Contain("logo_fede.png");
        }

        [Fact]
        public void ExportEmbeddedStyleAndJS_CreeLesRepertoiresEtTenteLExport()
        {
            // Arrange
            var physicalStructure = new DummyPhysicalStructure(_tempRootDir, "COMPET_TEST");
            var generator = new DummyUrlGenerator(physicalStructure, "http://localhost");

            // Act
            // Le flag 'regenere = true' force la suppression/recréation
            var result = SiteExportEngine.ExportEmbeddedStyleAndJS(regenere: true, generator);

            // Assert
            // Même si la DLL de test n'a pas les ressources embarquées, la méthode doit au moins recréer les dossiers
            Directory.Exists(physicalStructure.RepertoireCss()).Should().BeTrue("Le dossier CSS cible doit être créé par la méthode.");
            Directory.Exists(physicalStructure.RepertoireJs()).Should().BeTrue("Le dossier JS cible doit être créé par la méthode.");

            result.Should().NotBeNull("La méthode doit retourner une liste, même vide.");
        }
    }
}