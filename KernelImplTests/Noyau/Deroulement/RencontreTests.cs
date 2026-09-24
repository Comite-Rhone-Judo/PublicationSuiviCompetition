#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using FluentAssertions.Events;
using Moq;
using FranceJudo.Metier.Noyau;
using KernelImpl.Noyau.Deroulement;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class RencontreTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            Rencontre original = new Rencontre
            {
                id = 404,
                judoka1 = 100,
                judoka2 = null,
                vainqueur = 100,
                temps = 180,
                isNewRencontre = true,
                estDecisif = false
            };

            XElement xml = original.ToXml(dc);

            Rencontre copie = new Rencontre();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.judoka1.Should().Be(original.judoka1);
            copie.judoka2.Should().BeNull();
            copie.vainqueur.Should().Be(original.vainqueur);
            copie.temps.Should().Be(original.temps);
            copie.isNewRencontre.Should().Be(original.isNewRencontre);
            copie.estDecisif.Should().Be(original.estDecisif);
        }

        [Fact]
        public void Proprietes_DeclenchentOnPropertyChanged()
        {
            Rencontre rencontre = new Rencontre { id = 1, judoka1 = 10 };

            using (IMonitor<Rencontre> monitor = rencontre.Monitor())
            {
                rencontre.judoka1 = 20;
                monitor.Should().RaisePropertyChangeFor(p => p.judoka1);
            }
        }
    }
}