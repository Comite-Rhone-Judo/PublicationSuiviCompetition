using System.Collections.Generic;
using System.Linq;

namespace FranceJudo.Metier.Noyau.Participants
{
    public static class DataParticipantsExtension
    {
        public static IEnumerable<IJudoka> GetJudokaEpreuve(this IParticipantsData dataContext, int epreuve)
        {
            // On matérialise instantanément les IDs dans un HashSet.
            // La recherche (Contains) sera désormais ultra-rapide (O(1)).
            HashSet<int> judokasIds = new HashSet<int>(
                dataContext.EpreuveJudokas
                           .Where(o => o.epreuve == epreuve)
                           .Select(o => o.judoka)
            );

            return dataContext.Judokas.Where(o => judokasIds.Contains(o.id));
        }
    }
}
