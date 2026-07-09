#nullable enable
using Xunit;
using FluentAssertions;
using FluentAssertions.Events;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Internal;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class VueEpreuvePhaseTests
    {
        [Fact]
        public void Constructeur_InitialiseLesProprietesDepuisLaPhase()
        {
            Phase phase = new Phase
            {
                id = 99,
                libelle = "Tableau Principal",
                typePhase = FranceJudo.Metier.Noyau.Deroulement.TypePhaseEnum.Tableau,
                etat = FranceJudo.Metier.Noyau.Deroulement.EtatPhaseEnum.NonCree
            };

            VueEpreuvePhase vue = new VueEpreuvePhase(phase);

            vue.id.Should().Be(99);
            vue.nom.Should().Be("Tableau Principal");
            vue.type_phase.Should().Be(FranceJudo.Metier.Noyau.Deroulement.TypePhaseEnum.Tableau);
            vue.etat.Should().Be(FranceJudo.Metier.Noyau.Deroulement.EtatPhaseEnum.NonCree);

            IEntityWithKey<int> entity = vue;
            entity.EntityKey.Should().Be(99);
        }

        [Fact]
        public void ProprietesCompteurs_DeclenchentOnPropertyChanged()
        {
            Phase phase = new Phase { id = 1 };
            VueEpreuvePhase vue = new VueEpreuvePhase(phase);

            using (IMonitor<VueEpreuvePhase> monitor = vue.Monitor())
            {
                vue.nbcombat = 10;
                vue.nbcombatRep = 5;
                vue.nbcombattotal = 15;
                vue.valid = 1;

                monitor.Should().RaisePropertyChangeFor(v => v.nbcombat);
                monitor.Should().RaisePropertyChangeFor(v => v.nbcombatRep);
                monitor.Should().RaisePropertyChangeFor(v => v.nbcombattotal);
                monitor.Should().RaisePropertyChangeFor(v => v.valid);
            }
        }
    }
}