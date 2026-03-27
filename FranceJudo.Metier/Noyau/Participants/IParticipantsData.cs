using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranceJudo.Metier.Noyau.Participants
{
    public interface IParticipantsData
    {
        IReadOnlyList<IJudoka> Judokas { get; }
        IReadOnlyList<IEquipe> Equipes { get; }
        IReadOnlyList<IEpreuveJudoka> EpreuveJudokas { get; }
        IReadOnlyList<Ivue_judoka> Vuejudokas { get; }
    }
}
