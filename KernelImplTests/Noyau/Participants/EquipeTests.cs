#nullable enable
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Participants;
using KernelImpl.Noyau.Participants;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Participants
{
    public class EquipeTests
    {
        [Fact]
        public void Equipe_XmlSerialization_ShouldMapAllProperties()
        {
            // Arrange
            Equipe equipeSource = new Equipe
            {
                id = 123,
                libelle = "Equipe de test",
                club = "Judo Club",
                comite = "75",
                ligue = "IDF",
                pays = 250,
                remoteId = "REMOTE_123"
            };

            // Act
            XElement xmlElement = equipeSource.ToXml(null);
            Equipe equipeDestination = new Equipe();
            equipeDestination.LoadXml(xmlElement);

            // Assert
            Assert.Equal(equipeSource.id, equipeDestination.id);
            Assert.Equal(equipeSource.libelle, equipeDestination.libelle);
            Assert.Equal(equipeSource.club, equipeDestination.club);
            Assert.Equal(equipeSource.comite, equipeDestination.comite);
            Assert.Equal(equipeSource.ligue, equipeDestination.ligue);
            Assert.Equal(equipeSource.pays, equipeDestination.pays);
            Assert.Equal(equipeSource.remoteId, equipeDestination.remoteId);
        }

        [Fact]
        public void Equipe_PropertyChanged_ShouldBeRaised()
        {
            // Arrange
            Equipe equipe = new Equipe();
            List<string> proprietesModifiees = new List<string>();

            equipe.PropertyChanged += (sender, e) =>
            {
                proprietesModifiees.Add(e.PropertyName ?? string.Empty);
            };

            // Act
            equipe.id = 10;
            equipe.libelle = "Nouveau Libelle";

            // Assert
            Assert.Contains("id", proprietesModifiees);
            Assert.Contains("libelle", proprietesModifiees);
            Assert.Equal(2, proprietesModifiees.Count);
        }
    }
}