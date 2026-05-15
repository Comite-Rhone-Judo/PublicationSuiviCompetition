#nullable enable
using System.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Structures;

namespace FranceJudo.Core.Tests.Metier.Structures
{
    public class LectureStructuresTests
    {
        [Fact]
        public void GetStructures_DoitToujoursContenirLeNiveauNational()
        {
            // Act
            // Note : Cette méthode va appeler MetierResources.GetStructuresXml() en interne.
            var result = LectureStructures.GetStructures();

            // Assert
            result.Should().NotBeNull("La méthode doit au moins retourner la liste initialisée, même si le XML est introuvable.");
            result.Should().NotBeEmpty();

            // On s'assure que le premier élément "FRANCE JUDO" codé en dur est bien présent
            var structureNationale = result.FirstOrDefault(s => s.Type == TypeStructureEnum.National);

            structureNationale.Should().NotBeNull("Le niveau national 'FRANCE JUDO' doit être inséré par défaut.");
            structureNationale!.Id.Should().Be("FRANCE JUDO");
            structureNationale.Ordre.Should().Be(1);
        }

        [Fact]
        public void GetStructures_ParsingXML_VerifieLaCoherenceDesDonneesSiRessourcePresente()
        {
            // Act
            var result = LectureStructures.GetStructures();

            // Assert
            // Si le fichier XML est bien embarqué dans le projet de test et retourné par MetierResources,
            // on vérifie que le mapping XML -> Objet s'est bien déroulé.
            if (result.Count > 1)
            {
                var ligues = result.Where(s => s.Type == TypeStructureEnum.Ligue).ToList();
                var comites = result.Where(s => s.Type == TypeStructureEnum.Comite).ToList();

                // On vérifie le préfixe ajouté par la méthode
                if (ligues.Any())
                {
                    ligues.First().Nom.Should().StartWith("LIGUE ");
                    ligues.First().Ordre.Should().Be(2);
                }

                if (comites.Any())
                {
                    comites.First().Nom.Should().StartWith("COMITE ");
                    comites.First().Ordre.Should().Be(3);
                }
            }
        }
    }
}