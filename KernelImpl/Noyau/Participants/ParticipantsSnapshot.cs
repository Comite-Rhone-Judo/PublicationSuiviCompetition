using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Participants
{
    public class ParticipantsSnapshot : IParticipantsData
    {
        public IReadOnlyList<Judoka> Judokas { get; }
        public IReadOnlyList<Equipe> Equipes { get; }
        public IReadOnlyList<EpreuveJudoka> EpreuveJudokas { get; }

        public IReadOnlyList<vue_judoka> Vuejudokas { get; }

        public ParticipantsSnapshot(DataParticipants source)
        {
            if (source == null) return;
            Judokas = source.Judokas;
            Equipes = source.Equipes;
            EpreuveJudokas = source.EpreuveJudokas;
            Vuejudokas = source.Vuejudokas;
        }
    }
}
