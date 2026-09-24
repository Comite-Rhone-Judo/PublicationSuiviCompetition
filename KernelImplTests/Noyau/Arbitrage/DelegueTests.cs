#nullable enable
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Arbitrage;

namespace KernelImpl.Tests.Noyau.Arbitrage
{
    public class DelegueTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            // Arrange
            var original = new Delegue
            {
                id = 5,
                nom = "GARDE",
                prenom = "Pierre",
                telephone = "0600000000",
                mail = "pierre@judo.fr",
                fonction = "Superviseur",
                commentaires = "RAS"
            };

            // Act
            XElement xml = original.ToXml();
            var copie = new Delegue();
            copie.LoadXml(xml);

            // Assert
            copie.id.Should().Be(original.id);
            copie.nom.Should().Be(original.nom.ToUpper()); // Le code fait un ToUpper() sur le nom
            copie.prenom.Should().Be(original.prenom);
            copie.telephone.Should().Be(original.telephone);
            copie.mail.Should().Be(original.mail);
            copie.fonction.Should().Be(original.fonction);
            copie.commentaires.Should().Be(original.commentaires);
        }

        [Fact]
        public void LectureDelegue_ParseUneListeDepuisUnXElement()
        {
            var d1 = new Delegue { id = 7, nom = "Test1" }.ToXml();
            var d2 = new Delegue { id = 8, nom = "Test2" }.ToXml();
            var root = new XElement("Root", d1, d2);

            var liste = Delegue.LectureDelegue(root);

            liste.Should().HaveCount(2);
        }
    }
}