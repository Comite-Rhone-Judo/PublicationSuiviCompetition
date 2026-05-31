using System;
using System.Collections.Generic;
using System.Linq;
using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using KernelImpl.Noyau.Deroulement;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    public class DataStatistiquesCombats : IDataStatistiquesCombats
    {
        private readonly Dictionary<string, IStatistiquesItem> _statistiques;

        public IReadOnlyDictionary<string, IStatistiquesItem> Statistiques => _statistiques;

        public DataStatistiquesCombats(IJudoData snapshot)
        {
            _statistiques = BuildStatistiques(snapshot);
        }
        private Dictionary<string, IStatistiquesItem> BuildStatistiques(IJudoData judoData)
        {
            var compteurs = new Dictionary<string, CompteurStatistiques>();

            CompteurStatistiques GetOrCreateCompteur(string id, TypeEntiteStatistique typeEntite)
            {
                if (string.IsNullOrEmpty(id)) return null;
                if (!compteurs.TryGetValue(id, out var compteur))
                {
                    compteur = new CompteurStatistiques(typeEntite);
                    compteurs[id] = compteur;
                }
                return compteur;
            }

            var competition = ExtractCompetition(judoData);
            EchelonEnum niveauCompetition = competition != null ? (EchelonEnum)competition.niveau : EchelonEnum.Club;
            var echelonsCibles = DeterminerEchelonsCibles(niveauCompetition);

            // 1. Passe des Participants (Pour la participation STAT35, STAT36, STAT37)
            foreach (IVueJudoka participant in ExtractAllParticipants(judoData))
            {
                // Utilisation de la propriété de l'interface IVueJudoka
                bool estPresent = participant.present;

                GetOrCreateCompteur(participant.id.ToString(), TypeEntiteStatistique.Judoka);

                foreach (var idStructure in GetIdsStructuresPourParticipant(participant, echelonsCibles))
                {
                    var c = GetOrCreateCompteur(idStructure, TypeEntiteStatistique.Structure);
                    if (c != null)
                    {
                        c.NbParticipants++;
                        if (estPresent) c.NbCombattants++;
                    }
                }
            }

            // 2. Passe des Combats 
            foreach (Combat combat in ExtractAllCombats(judoData))
            {
                if (!EstCombatValidePourStats(combat)) continue;

                var idsP1 = GetIdsStructuresEtPersoPourParticipant(judoData, combat.participant1, echelonsCibles);
                var idsP2 = GetIdsStructuresEtPersoPourParticipant(judoData, combat.participant2, echelonsCibles);

                string penP1Str = combat.GetPenalites(1);
                string penP2Str = combat.GetPenalites(2);
                int pen1Count = ParseNombrePenalites(penP1Str);
                int pen2Count = ParseNombrePenalites(penP2Str);

                bool p1Gagne = combat.vainqueur == combat.participant1;
                bool p2Gagne = combat.vainqueur == combat.participant2;

                TimeSpan tEffectif = combat.fin - combat.debut;
                if (tEffectif < TimeSpan.Zero) tEffectif = TimeSpan.Zero;

                TimeSpan tNominal = TimeSpan.FromMinutes(combat.temps);
                bool isGoldenScore = combat.goldenScore || tEffectif > tNominal;
                TimeSpan dureeGolden = (isGoldenScore && tEffectif > tNominal) ? tEffectif - tNominal : TimeSpan.Zero;

                void AppliquerStats(string id, TypeEntiteStatistique typeEntite, bool estVainqueur, int score, string penAdversaire, int nbPenalitesRecues)
                {
                    var c = GetOrCreateCompteur(id, typeEntite);
                    if (c == null) return;

                    c.NbCombats++;
                    c.TotalPenalites += nbPenalitesRecues;

                    c.TotalDureeCombat += tEffectif;
                    if (tEffectif < c.DureeCombatMinInterne) c.DureeCombatMinInterne = tEffectif;
                    if (tEffectif > c.DureeCombatMaxInterne) c.DureeCombatMaxInterne = tEffectif;

                    if (isGoldenScore)
                    {
                        c.NbCombatsGoldenScore++;
                        c.TotalDureeGoldenScore += dureeGolden;
                        if (dureeGolden > c.DureeMaximaleGoldenScoreInterne) c.DureeMaximaleGoldenScoreInterne = dureeGolden;
                    }

                    if (estVainqueur) AnalyserVictoire(c, score, penAdversaire);
                }

                foreach (var tuple in idsP1)
                    AppliquerStats(tuple.Id, tuple.Type, p1Gagne, combat.score1, penP2Str, pen1Count);

                foreach (var tuple in idsP2)
                    AppliquerStats(tuple.Id, tuple.Type, p2Gagne, combat.score2, penP1Str, pen2Count);
            }

            var statsFinales = new Dictionary<string, IStatistiquesItem>(compteurs.Count);
            foreach (var kvp in compteurs)
            {
                statsFinales.Add(kvp.Key, kvp.Value);
            }

            return statsFinales;
        }

        // --- Méthodes privées internes ---

        private HashSet<EchelonEnum> DeterminerEchelonsCibles(EchelonEnum niveauCompetition)
        {
            var echelons = new HashSet<EchelonEnum> { EchelonEnum.Club }; // Le club est toujours calculé

            switch (niveauCompetition)
            {
                case EchelonEnum.Departement:
                    echelons.Add(EchelonEnum.Departement);
                    break;
                case EchelonEnum.Ligue:
                    echelons.Add(EchelonEnum.Departement);
                    echelons.Add(EchelonEnum.Ligue);
                    break;
                case EchelonEnum.National:
                case EchelonEnum.International:
                    echelons.Add(EchelonEnum.Departement);
                    echelons.Add(EchelonEnum.Ligue);
                    echelons.Add(EchelonEnum.National);
                    break;
            }

            return echelons;
        }

        private IEnumerable<string> GetIdsStructuresPourParticipant(IVueJudoka p, HashSet<EchelonEnum> echelonsCibles)
        {
            if (p == null) yield break;

            // Calqué sur le mapping de DataEngagement
            foreach (var echelon in echelonsCibles)
            {
                switch (echelon)
                {
                    case EchelonEnum.Club:
                        if (!string.IsNullOrEmpty(p.club)) yield return p.club;
                        break;
                    case EchelonEnum.Departement:
                        if (!string.IsNullOrEmpty(p.comite)) yield return p.comite;
                        break;
                    case EchelonEnum.Ligue:
                        if (!string.IsNullOrEmpty(p.ligue)) yield return p.ligue;
                        break;
                    case EchelonEnum.National:
                        if (p.pays != 0) yield return p.pays.ToString();
                        break;
                }
            }
        }

        private IEnumerable<(string Id, TypeEntiteStatistique Type)> GetIdsStructuresEtPersoPourParticipant(
            IJudoData data,
            int? idParticipant,
            HashSet<EchelonEnum> echelonsCibles)
        {
            if (!idParticipant.HasValue) yield break;

            yield return (idParticipant.Value.ToString(), TypeEntiteStatistique.Judoka);

            // On utilise la collection des vues Judokas
            var participant = data.Participants?.Vuejudokas?.FirstOrDefault(p => p.id == idParticipant.Value);
            if (participant != null)
            {
                foreach (var idStructure in GetIdsStructuresPourParticipant(participant, echelonsCibles))
                {
                    yield return (idStructure, TypeEntiteStatistique.Structure);
                }
            }
        }

        private void AnalyserVictoire(CompteurStatistiques c, int scoreVainqueur, string penalitePerdant)
        {
            if (penalitePerdant == "3") { c.NbVictoireSogoGachi++; return; }
            if (penalitePerdant == "H" || penalitePerdant == "X") { c.NbVictoireHansokuMake++; return; }

            int ipponV = scoreVainqueur / 100;
            int wazaV = (scoreVainqueur / 10) % 10;
            int yukoV = scoreVainqueur % 10;

            if (ipponV >= 1) c.NbVictoireIpponDirect++;
            else if (wazaV >= 2) c.NbVictoireWazaAriAwaseteIppon++;
            else if (wazaV == 1) c.NbVictoireWazaAri++;
            else if (yukoV >= 1) c.NbVictoireYuko++;
        }

        private int ParseNombrePenalites(string penaliteStr)
        {
            if (string.IsNullOrEmpty(penaliteStr)) return 0;

            // Nettoyage du préfixe optionnel "-"
            string cleanPenalite = penaliteStr.TrimStart('-');

            if (cleanPenalite == "H" || cleanPenalite == "X" || cleanPenalite == "3" ||
                cleanPenalite == "A" || cleanPenalite == "M" || cleanPenalite == "F") return 3;

            if (cleanPenalite == "2") return 2;
            if (cleanPenalite == "1") return 1;

            return 0;
        }

        private bool EstCombatValidePourStats(Combat combat)
        {
            return !combat.virtuel && combat.vainqueur.HasValue &&
                   combat.participant1.HasValue && combat.participant2.HasValue;
        }

        private ICompetition ExtractCompetition(IJudoData data)
        {
            // Utilisation du Pattern Matching (C# 8+) pour vérifier que la liste n'est pas nulle et contient des éléments.
            // Si c'est le cas, on l'assigne à la variable 'comps' et on retourne le premier élément.
            return data.Organisation?.Competitions is { Count: > 0 } comps ? comps[0] : null;
        }

        private IEnumerable<Combat> ExtractAllCombats(IJudoData data) => data.Deroulement?.Combats?.OfType<Combat>() ?? Enumerable.Empty<Combat>();

        private IEnumerable<IVueJudoka> ExtractAllParticipants(IJudoData data) => data.Participants?.Vuejudokas ?? Enumerable.Empty<IVueJudoka>();
    }

    // La classe CompteurStatistiques reste strictement identique à la version précédente 
    internal class CompteurStatistiques : IStatistiquesItem
    {
        public TypeEntiteStatistique TypeEntite { get; }

        public int? NbParticipants { get; set; }
        public int? NbCombattants { get; set; }
        public int NbCombats { get; set; }

        public int NbVictoireIpponDirect { get; set; }
        public int NbVictoireWazaAriAwaseteIppon { get; set; }
        public int NbVictoireWazaAri { get; set; }
        public int NbVictoireYuko { get; set; }
        public int NbVictoireSogoGachi { get; set; }
        public int NbVictoireHansokuMake { get; set; }

        public int TotalPenalites { get; set; }

        public int NbCombatsGoldenScore { get; set; }
        public TimeSpan TotalDureeGoldenScore { get; set; }
        public TimeSpan DureeMaximaleGoldenScoreInterne { get; set; }

        public TimeSpan TotalDureeCombat { get; set; }
        public TimeSpan DureeCombatMaxInterne { get; set; }
        public TimeSpan DureeCombatMinInterne { get; set; } = TimeSpan.MaxValue;

        public CompteurStatistiques(TypeEntiteStatistique typeEntite)
        {
            TypeEntite = typeEntite;
            if (typeEntite == TypeEntiteStatistique.Structure)
            {
                NbParticipants = 0;
                NbCombattants = 0;
            }
        }

        public double? PctParticipation => (!NbParticipants.HasValue || NbParticipants.Value == 0) ? null : (double)NbCombattants.Value / NbParticipants.Value;
        public double? PctVictoireIpponDirect => NbCombats == 0 ? null : (double)NbVictoireIpponDirect / NbCombats;
        public double? PctVictoireWazaAriAwaseteIppon => NbCombats == 0 ? null : (double)NbVictoireWazaAriAwaseteIppon / NbCombats;
        public double? PctVictoireWazaAri => NbCombats == 0 ? null : (double)NbVictoireWazaAri / NbCombats;
        public double? PctVictoireYuko => NbCombats == 0 ? null : (double)NbVictoireYuko / NbCombats;
        public double? PctVictoireSogoGachi => NbCombats == 0 ? null : (double)NbVictoireSogoGachi / NbCombats;
        public double? PctVictoireHansokuMake => NbCombats == 0 ? null : (double)NbVictoireHansokuMake / NbCombats;

        public double? MoyennePenalitesParCombat => NbCombats == 0 ? null : (double)TotalPenalites / NbCombats;

        public double? PctCombatsGoldenScore => NbCombats == 0 ? null : (double)NbCombatsGoldenScore / NbCombats;
        public TimeSpan? DureeMoyenneGoldenScore => NbCombatsGoldenScore == 0 ? null : TimeSpan.FromTicks(TotalDureeGoldenScore.Ticks / NbCombatsGoldenScore);
        public TimeSpan? DureeMaximaleGoldenScore => NbCombatsGoldenScore == 0 ? null : DureeMaximaleGoldenScoreInterne;

        public TimeSpan? DureeCombatMin => NbCombats == 0 ? null : DureeCombatMinInterne;
        public TimeSpan? DureeCombatMax => NbCombats == 0 ? null : DureeCombatMaxInterne;
        public TimeSpan? DureeCombatMoy => NbCombats == 0 ? null : TimeSpan.FromTicks(TotalDureeCombat.Ticks / NbCombats);
    }
}