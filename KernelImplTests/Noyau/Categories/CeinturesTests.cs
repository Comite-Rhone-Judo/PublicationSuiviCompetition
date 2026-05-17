#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Categories;

namespace KernelImpl.Tests.Noyau.Categories
{
    public class CeinturesTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Ceintures original = new Ceintures
            {
                id = 8,
                nom = "Ceinture Noire 1er Dan",
                ordre = "10",
                couleur1 = "Noir",
                couleur2 = "",
                remoteId = "CN_1"
            };

            XElement xml = original.ToXml();

            Ceintures copie = new Ceintures();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.nom.Should().Be(original.nom);
            copie.ordre.Should().Be(original.ordre);
            copie.couleur1.Should().Be(original.couleur1);
            copie.couleur2.Should().Be(original.couleur2);
            copie.remoteId.Should().Be(original.remoteId);
        }

        [Fact]
        public void LectureCeintures_ParseUneListeDepuisUnXElement()
        {
            XElement ceinture1 = new Ceintures { id = 1, nom = "Blanche" }.ToXml();
            XElement ceinture2 = new Ceintures { id = 2, nom = "Jaune" }.ToXml();
            XElement root = new XElement("Root", ceinture1, ceinture2);

            ICollection<Ceintures> liste = Ceintures.LectureCeintures(root);

            liste.Should().HaveCount(2);
            liste.First().nom.Should().Be("Blanche");
        }
    }
}