using FranceJudo.Metier.Noyau.Participants;
using System.Collections.Generic;

namespace KernelImpl.Noyau.Participants
{
    public class ParticipantsSnapshot : IParticipantsData
    {
        public IReadOnlyList<IJudoka> Judokas { get; private set; }
        public IReadOnlyList<IEquipe> Equipes { get; private set; }
        public IReadOnlyList<IEpreuveJudoka> EpreuveJudokas { get; private set; }

        public IReadOnlyList<IVueJudoka> Vuejudokas { get; private set; }

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
