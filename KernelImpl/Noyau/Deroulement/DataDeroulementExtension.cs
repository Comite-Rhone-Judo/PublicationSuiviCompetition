using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Logging;

namespace KernelImpl.Noyau.Deroulement
{
    public static class DataDeroulementExtension
    {
        public static IEnumerable<Participant> ListeParticipant1(this IDeroulementData dataContext, int epreuve)
        {
            IEnumerable<int> phases = dataContext.Phases.Where(o => o.epreuve == epreuve && o.suivant != 0).Select(o => o.id).Distinct();
            return dataContext.Participants.Where(o => phases.Contains(o.phase));
        }

        public static IEnumerable<Participant> ListeParticipant2(this IDeroulementData dataContext, int epreuve)
        {
            IEnumerable<int> phases = dataContext.Phases.Where(o => o.epreuve == epreuve && o.suivant == 0).Select(o => o.id).Distinct();
            return dataContext.Participants.Where(o => phases.Contains(o.phase));
        }

        public static int GetNbCombatJudoka(this IDeroulementData dataContext, string licence, IJudoData DC)
        {
            int result = 0;
            foreach (Participants.Judoka vj in DC.Participants.Judokas.Where(o => o.licence == licence))
            {
                result += dataContext.Combats.Count(o => o.vainqueur.HasValue && o.vainqueur > 0 && (o.participant1 == vj.id || o.participant2 == vj.id));
            }
            return result;
        }

        public static int GetNbPointJudoka(this IDeroulementData dataContext, string licence, IJudoData DC)
        {
            int result = 0;
            foreach (Participants.Judoka vj in DC.Participants.Judokas.Where(o => o.licence == licence))
            {
                foreach (Participant participant in dataContext.Participants.Where(o => o.judoka == vj.id))
                {
                    result += participant.cumulPointsGRCH;
                }
            }
            return result;
        }
    }
}
