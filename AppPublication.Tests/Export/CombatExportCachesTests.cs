#nullable enable
using AppPublication.Export;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppPublication.Tests.Export
{
    public class CombatExportCachesTests
    {
        [Fact]
        public void Proprietes_Dictionnaires_PeuventEtreAssignees()
        {
            // Arrange
            CombatExportCaches cache = new CombatExportCaches();
            Dictionary<int, IPhase> dictionnairePhases = new Dictionary<int, IPhase>();
            Dictionary<int, IVueEpreuve> dictionnaireEpreuves = new Dictionary<int, IVueEpreuve>();

            // Act
            cache.PhasesDict = dictionnairePhases;
            cache.EpreuvesDict = dictionnaireEpreuves;

            // Assert
            Assert.NotNull(cache.PhasesDict);
            Assert.NotNull(cache.EpreuvesDict);
            Assert.Same(dictionnairePhases, cache.PhasesDict);
        }

        [Fact]
        public void Proprietes_Lookups_PeuventEtreAssignees()
        {
            // Arrange
            CombatExportCaches cache = new CombatExportCaches();

            // Création d'un ILookup vide pour le test via une liste vide
            List<IRencontre> rencontres = new List<IRencontre>();
            ILookup<int, IRencontre> lookupRencontres = rencontres.ToLookup(r => r.id);

            // Act
            cache.RencontresByCombat = lookupRencontres;

            // Assert
            Assert.NotNull(cache.RencontresByCombat);
            Assert.Same(lookupRencontres, cache.RencontresByCombat);
        }
    }
}