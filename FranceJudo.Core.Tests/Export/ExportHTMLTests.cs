using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Xsl;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Export;
using FranceJudo.Core.Reflection;

namespace FranceJudo.Core.Tests.Export
{
    // On désactive la parallélisation car ExportHTML utilise un cache statique partagé (_xsltCache)
    [CollectionDefinition("ExportHTML Sequential", DisableParallelization = true)]
    [Collection("ExportHTML Sequential")]
    public class ExportHTMLTests : IDisposable
    {
        private readonly string _tempFile;
        private readonly AssemblyResourceDictionary _resDict;

        // ATTENTION : Remplace par le namespace exact de ton projet de test
        private readonly string _xsltResourceName = "FranceJudo.Core.Tests.Resources.test_export.xslt";

        public ExportHTMLTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), "ExportResult_" + Guid.NewGuid().ToString() + ".html");
            _resDict = new AssemblyResourceDictionary(Assembly.GetExecutingAssembly());

            ClearXsltCache();
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile)) File.Delete(_tempFile);
            ClearXsltCache();
        }

        #region Utilitaires de Réflexion (Espionnage du Cache)

        private void ClearXsltCache()
        {
            var cache = GetCacheInstance();
            cache?.Clear();
        }

        private int GetCacheSize()
        {
            return GetCacheInstance()?.Count ?? 0;
        }

        private ConcurrentDictionary<string, Lazy<XslCompiledTransform>>? GetCacheInstance()
        {
            var cacheField = typeof(ExportHTML).GetField("_xsltCache", BindingFlags.Static | BindingFlags.NonPublic);
            return cacheField?.GetValue(null) as ConcurrentDictionary<string, Lazy<XslCompiledTransform>>;
        }

        #endregion

        #region Tests - Génération et GetXsltFromResource

        [Fact]
        public void ToHTML_TransformationValide_GenereLeFichierHtmlCorrectement()
        {
            // Arrange : On prépare la donnée XML source
            var doc = XDocument.Parse("<Root><Title>Judo Championnat 2026</Title></Root>");
            using var xmlSource = new XmlSource(doc);

            // Act : La méthode GetXsltFromResource sera appelée implicitement ici
            ExportHTML.ToHTML(
                source: xmlSource,
                fileSave: _tempFile,
                argsList: null!,
                xslt_st: _xsltResourceName,
                resDict: _resDict,
                useCache: true);

            // Assert
            File.Exists(_tempFile).Should().BeTrue("Le fichier HTML doit être généré sur le disque.");

            string htmlContent = File.ReadAllText(_tempFile);
            htmlContent.Should().Contain("<h1>Judo Championnat 2026</h1>", "Le XSLT doit avoir correctement fusionné avec les données XML.");
        }

        [Fact]
        public void ToHTML_RessourceXsltIntrouvable_NePlantePasEtNeCreePasLeFichier()
        {
            // Arrange
            var doc = new XDocument(new XElement("Root"));
            using var xmlSource = new XmlSource(doc);

            // Act
            // Le XSLT "Fantome.xslt" n'existe pas. Cela déclenche FileNotFoundException dans GetXsltFromResource.
            Action act = () => ExportHTML.ToHTML(
                source: xmlSource,
                fileSave: _tempFile,
                argsList: null!,
                xslt_st: "Fantome.xslt",
                resDict: _resDict,
                useCache: false);

            // Assert
            act.Should().NotThrow("L'erreur doit être capturée par le bloc catch(Exception) de ExecuteTransform.");
            File.Exists(_tempFile).Should().BeFalse("En cas d'erreur de ressource, aucun fichier ne doit être généré.");
        }

        #endregion

        #region Tests - Mécanisme de Cache

        [Fact]
        public void ExecuteTransform_UseCacheTrue_StockeLeXsltCompileDansLeDictionnaire()
        {
            // Arrange
            var doc = new XDocument(new XElement("Root"));
            using var xmlSource = new XmlSource(doc);

            GetCacheSize().Should().Be(0, "Le cache doit être vide au démarrage du test.");

            // Act 1 : Premier appel, compilation et mise en cache
            ExportHTML.ToHTML(xmlSource, _tempFile, null!, _xsltResourceName, _resDict, useCache: true);

            // Assert 1
            GetCacheSize().Should().Be(1, "Le XSLT doit avoir été ajouté au ConcurrentDictionary.");

            // Act 2 : Deuxième appel avec la MÊME ressource
            ExportHTML.ToHTML(xmlSource, _tempFile, null!, _xsltResourceName, _resDict, useCache: true);

            // Assert 2
            GetCacheSize().Should().Be(1, "La taille du cache ne doit pas augmenter, la version compilée a été réutilisée.");
        }

        [Fact]
        public void ExecuteTransform_UseCacheFalse_IgnoreLeDictionnaire()
        {
            // Arrange
            var doc = new XDocument(new XElement("Root"));
            using var xmlSource = new XmlSource(doc);

            // Act
            ExportHTML.ToHTML(
                source: xmlSource,
                fileSave: _tempFile,
                argsList: null!,
                xslt_st: _xsltResourceName,
                resDict: _resDict,
                useCache: false); // On force la désactivation

            // Assert
            GetCacheSize().Should().Be(0, "Si useCache est false, le dictionnaire ne doit jamais être impacté.");
        }

        #endregion
    }
}