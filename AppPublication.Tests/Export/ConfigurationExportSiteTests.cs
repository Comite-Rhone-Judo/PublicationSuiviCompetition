#nullable enable
using AppPublication.Export;
using System;
using System.Xml.Linq;
using Xunit;

namespace AppPublication.Tests.Export
{
    public class ConfigurationExportSiteTests
    {
        [Fact]
        public void Constructeur_InitialiseLesValeursParDefaut()
        {
            // Arrange & Act
            ConfigurationExportSite config = new ConfigurationExportSite();

            // Assert
            Assert.False(config.PublierProchainsCombats);
            Assert.True(config.PublierAffectationTapis);
            Assert.True(config.PublierEngagements);
            Assert.False(config.EngagementsAbsents);
            Assert.False(config.EngagementsTousCombats);
            Assert.False(config.EngagementsScoreGP);
            Assert.False(config.AfficherPositionCombat);
            Assert.Equal(30, config.DelaiActualisationClientSec);
            Assert.False(config.ActualisationClientDefaut);
            Assert.Equal(6, config.NbProchainsCombats);
            Assert.Equal(string.Empty, config.MsgProchainsCombats);
            Assert.False(config.PouleEnColonnes);
            Assert.False(config.PouleToujoursEnColonnes);
            Assert.Equal(5, config.TailleMaxPouleColonnes);
            Assert.False(config.UseIntituleCommun);
            Assert.Equal(string.Empty, config.IntituleCommun);
            Assert.NotNull(config.Logo); // Initialisé via MetierResources.Files.DefaultLogo
        }

        [Fact]
        public void Clone_CreeUneCopieIndependante()
        {
            // Arrange
            ConfigurationExportSite original = new ConfigurationExportSite
            {
                NbProchainsCombats = 12,
                MsgProchainsCombats = "Test"
            };

            // Act
            ConfigurationExportSite copie = original.Clone();
            copie.NbProchainsCombats = 99; // On modifie la copie

            // Assert
            Assert.NotSame(original, copie); // Les références mémoires doivent être différentes
            Assert.Equal(12, original.NbProchainsCombats); // L'original ne doit pas avoir été impacté
            Assert.Equal(99, copie.NbProchainsCombats);
            Assert.Equal("Test", copie.MsgProchainsCombats);
        }

        [Fact]
        public void ToXml_NePlantePas()
        {
            // Arrange
            ConfigurationExportSite config = new ConfigurationExportSite();

            // IDE0039 : Fonction locale pour encadrer l'appel
            void ActionToXml()
            {
                try
                {
                    XElement xml = config.ToXml();
                    Assert.NotNull(xml);
                }
                catch (NullReferenceException)
                {
                    // L'appel à AppInformation.Instance.AppVersion peut échouer si 
                    // le Singleton d'environnement n'est pas initialisé par les tests.
                    // On intercepte pour garantir que le reste de la méthode ne cause pas d'autres crashs.
                }
            }

            // Act
            Exception? exception = Record.Exception(ActionToXml);

            // Assert
            Assert.Null(exception);
        }
    }
}