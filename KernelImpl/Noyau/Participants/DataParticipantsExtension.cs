using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Participants
{
    public static class DataParticipantsExtension
    {
        public static IEnumerable<Judoka> GetJudokaEpreuve(this IParticipantsData dataContext, int epreuve)
        {
            IEnumerable<int> judokas = dataContext.EpreuveJudokas.Where(o => o.epreuve == epreuve).Select(o => o.judoka).Distinct();
            return dataContext.Judokas.Where(o => judokas.Contains(o.id));
        }
    }
}
