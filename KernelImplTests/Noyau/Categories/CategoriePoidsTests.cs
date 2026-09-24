#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Categories;

namespace KernelImpl.Tests.Noyau.Categories
{
    public class CategoriePoidsTests
    {
        [Fact]
        public void Sexe_Set_SynchroniseSexeEnum()
        {
            CategoriePoids categorie = new CategoriePoids
            {
                // Dans CategoriePoids, sexe est un 'int'
                sexe = 1
            };

            categorie.sexeEnum.Should().NotBeNull();
            ((int)categorie.sexeEnum).Should().Be(1);
        }

        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            CategoriePoids original = new CategoriePoids
            {
                id = 42,
                nom = "-60 kg",
                poidsMin = 0,
                poidsMax = 60,
                ordre = "2",
                categorieAge = 5,
                sexe = 1,
                equipe = false,
                discipline = "Judo",
                remoteId = "REMOTE_60"
            };

            XElement xml = original.ToXml();

            CategoriePoids copie = new CategoriePoids();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.nom.Should().Be(original.nom);
            copie.poidsMin.Should().Be(original.poidsMin);
            copie.poidsMax.Should().Be(original.poidsMax);
            copie.ordre.Should().Be(original.ordre);
            copie.categorieAge.Should().Be(original.categorieAge);
            copie.sexe.Should().Be(original.sexe);
            copie.equipe.Should().Be(original.equipe);
            copie.discipline.Should().Be(original.discipline);
            copie.remoteId.Should().Be(original.remoteId);
        }

        [Fact]
        public void LectureCategoriePoids_ParseUneListeDepuisUnXElement()
        {
            XElement cp1 = new CategoriePoids { id = 1, nom = "-48 kg" }.ToXml();
            XElement root = new XElement("Root", cp1);

            ICollection<CategoriePoids> liste = CategoriePoids.LectureCategoriePoids(root);

            liste.Should().HaveCount(1);
            liste.First().nom.Should().Be("-48 kg");
        }
    }
}