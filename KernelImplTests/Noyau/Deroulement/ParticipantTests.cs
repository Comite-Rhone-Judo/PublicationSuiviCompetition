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
    public class ParticipantTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            DateTime dateCombat = new DateTime(2026, 05, 17, 14, 30, 0);

            Participant original = new Participant
            {
                id = 55,
                judoka = 1024,
                dernierCombat = dateCombat
            };

            XElement xml = original.ToXml(dc);

            Participant copie = new Participant();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.judoka.Should().Be(original.judoka);
            copie.dernierCombat.Should().Be(original.dernierCombat);
        }

        [Fact]
        public void Proprietes_DeclenchentOnPropertyChanged()
        {
            Participant participant = new Participant { id = 1, judoka = 100 };

            using (IMonitor<Participant> monitor = participant.Monitor())
            {
                participant.judoka = 200;
                monitor.Should().RaisePropertyChangeFor(p => p.judoka);
            }
        }
    }
}