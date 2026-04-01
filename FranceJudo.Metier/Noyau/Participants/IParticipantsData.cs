using System.Collections.Generic;

namespace FranceJudo.Metier.Noyau.Participants
{
    public interface IParticipantsData
    {
        IReadOnlyList<IJudoka> Judokas { get; }
        IReadOnlyList<IEquipe> Equipes { get; }
        IReadOnlyList<IEpreuveJudoka> EpreuveJudokas { get; }
        IReadOnlyList<IVueJudoka> Vuejudokas { get; }
    }
}
