using System.Collections.Generic;
using System.Linq;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;

namespace AppPublication.Export
{
    public class CombatExportCaches
    {
        // ==========================================
        // DICTIONNAIRES (Relation 1-1 / Clé Unique)
        // ==========================================

        public Dictionary<int, IPhase> PhasesDict { get; set; }
        public Dictionary<int, IVueEpreuve> EpreuvesDict { get; set; }
        public Dictionary<int, IVueEpreuveEquipe> EpreuvesEqDict { get; set; }
        public Dictionary<int, IJudoka> JudokasDict { get; set; }
        public Dictionary<int, IEquipe> EquipesDict { get; set; }

        // ==========================================
        // LOOKUPS (Relation 1-N / Listes pré-filtrées)
        // ==========================================

        public ILookup<int, IJudoka> JudokasByEquipe { get; set; }
        public ILookup<int, IRencontre> RencontresByCombat { get; set; }
        public ILookup<int, IPoule> PoulesByPhase { get; set; }
        public ILookup<int, IParticipant> ParticipantsByPhase { get; set; }
        public ILookup<int, IVueGroupe> GroupesByTapis { get; set; }
        public ILookup<int?, IVueEpreuve> EpreuvesByEquipe { get; set; }

        // BONUS OPTIMISATION (Pour ExportEpreuve)
        public ILookup<int, IEpreuveJudoka> InscriptionsByEpreuve { get; set; }
    }
}