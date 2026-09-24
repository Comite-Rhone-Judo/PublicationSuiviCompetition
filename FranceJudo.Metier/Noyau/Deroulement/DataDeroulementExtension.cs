using System.Collections.Generic;
using System.Linq;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public static class DataDeroulementExtension
    {

        /// <summary>
        /// Détermine la phase actuellement "en cours" (ou la plus avancée) parmi une collection de phases.
        /// Conçu pour prendre en entrée un sous-ensemble (ex: via un ILookup) afin de garantir des performances O(1).
        /// </summary>
        /// <param name="phasesEpreuve">La collection des phases d'une épreuve spécifique.</param>
        /// <returns>La phase active la plus avancée, ou null si aucune n'est valide.</returns>
        public static IPhase GetPhaseActive(this IEnumerable<IPhase> phasesEpreuve)
        {
            if (phasesEpreuve == null) return null;

            // 1. On ne conserve que les phases qui ont dépassé le stade de simple création
            var phasesValides = phasesEpreuve.Where(p => p.etat > EtatPhaseEnum.Cree).ToList();

            if (phasesValides.Count == 0) return null;

            // 2. Point de départ : la racine (precedent == 0) ou la première disponible en cas de données orphelines
            IPhase phaseCible = phasesValides.FirstOrDefault(p => p.precedent == 0) ?? phasesValides.First();

            // Sécurité anti-boucle infinie (ex: corruption de DB où A pointe sur B et B pointe sur A)
            var visited = new HashSet<int> { phaseCible.id };

            // 3. Descente dans la liste chaînée métier jusqu'à la dernière phase valide générée
            while (phaseCible.suivant != 0)
            {
                var nextPhase = phasesValides.FirstOrDefault(p => p.id == phaseCible.suivant);

                // On s'arrête si la phase suivante n'est pas encore valide ou si on tourne en rond
                if (nextPhase == null || !visited.Add(nextPhase.id))
                    break;

                phaseCible = nextPhase;
            }

            return phaseCible;
        }

        /// <summary>
        /// Génère la liste des participants pour une épreuve donnée, en filtrant uniquement ceux qui appartiennent à des phases ayant un successeur (c'est-à-dire des phases intermédiaires ou finales).
        /// </summary>
        /// <param name="dataContext">Le contexte de données du déroulement</param>
        /// <param name="epreuve">L'identifiant de l'épreuve</param>
        /// <returns>La liste des participants</returns>
        public static IEnumerable<IParticipant> ListeParticipant1(this IDeroulementData dataContext, int epreuve)
        {
            IEnumerable<int> phases = dataContext.Phases.Where(o => o.epreuve == epreuve && o.suivant != 0).Select(o => o.id).Distinct();
            return dataContext.Participants.Where(o => phases.Contains(o.phase));
        }

        /// <summary>
        /// Génère la liste des participants pour une épreuve donnée, en filtrant uniquement ceux qui appartiennent à des phases n'ayant pas de successeur (c'est-à-dire des phases initiales).
        /// </summary>
        /// <param name="dataContext">Le contexte de données du déroulement</param>
        /// <param name="epreuve">L'identifiant de l'épreuve</param>
        /// <returns>La liste des participants</returns>
        public static IEnumerable<IParticipant> ListeParticipant2(this IDeroulementData dataContext, int epreuve)
        {
            IEnumerable<int> phases = dataContext.Phases.Where(o => o.epreuve == epreuve && o.suivant == 0).Select(o => o.id).Distinct();
            return dataContext.Participants.Where(o => phases.Contains(o.phase));
        }

        
        /// <summary>
        /// Calcule le nombre de combats pour un judoka donné.
        /// </summary>
        /// <param name="dataContext">Le contexte de données du déroulement</param>
        /// <param name="licence">La licence du judoka</param>
        /// <param name="DC">Le contexte de données du judo</param>
        /// <returns></returns>
        public static int GetNbCombatJudoka(this IDeroulementData dataContext, string licence, IJudoData DC)
        {
            int result = 0;
            foreach (Participants.IJudoka vj in DC.Participants.Judokas.Where(o => o.licence == licence))
            {
                result += dataContext.Combats.Count(o => o.vainqueur.HasValue && o.vainqueur > 0 && (o.participant1 == vj.id || o.participant2 == vj.id));
            }
            return result;
        }

        /// <summary>
        /// Calcule le nombre de points cumulés pour un judoka donné.
        /// </summary>
        /// <param name="dataContext">Le contexte de données du déroulement</param>
        /// <param name="licence">La licence du judoka</param>
        /// <param name="DC">Le contexte de données du judo</param>
        /// <returns></returns>
        public static int GetNbPointJudoka(this IDeroulementData dataContext, string licence, IJudoData DC)
        {
            int result = 0;
            foreach (Participants.IJudoka vj in DC.Participants.Judokas.Where(o => o.licence == licence))
            {
                foreach (IParticipant participant in dataContext.Participants.Where(o => o.judoka == vj.id))
                {
                    result += participant.cumulPointsGRCH;
                }
            }
            return result;
        }
    }
}
