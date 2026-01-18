using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Participants
{
    public class ParticipantsSnapshot : IParticipantsData
    {
        public IReadOnlyList<Judoka> Judokas { get; private set; }
        public IReadOnlyList<Equipe> Equipes { get; private set; }
        public IReadOnlyList<EpreuveJudoka> EpreuveJudokas { get; private set; }

        public IReadOnlyList<vue_judoka> Vuejudokas { get; private set; }

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
