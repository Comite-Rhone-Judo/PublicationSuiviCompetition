using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Participants
{
    public interface IParticipantsData
    {
        IReadOnlyList<Judoka> Judokas { get; }
        IReadOnlyList<Equipe> Equipes { get; }
        IReadOnlyList<EpreuveJudoka> EpreuveJudokas { get; }
        IReadOnlyList<vue_judoka> Vuejudokas { get; }
    }
}
