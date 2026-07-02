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
        public IReadOnlyDictionary<StatistiqueCle, IStatistiquesItem> Statistiques { get; }
        public IReadOnlyList<GroupeStatistiques> GroupesStatistiques { get; }
        public IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes { get; }

        public DataStatistiquesCombats(IJudoData snapshot)
        {
            var setGroupes = new HashSet<GroupeStatistiques>();
            var setTypesParComp = new Dictionary<int, HashSet<EchelonEnum>>();

            Statistiques = BuildStatistiques(snapshot, setGroupes, setTypesParComp);

            GroupesStatistiques = setGroupes.ToList();
            TypesGroupes = setTypesParComp.ToDictionary(k => k.Key, v => v.Value.ToList());
        }

        private Dictionary<StatistiqueCle, IStatistiquesItem> BuildStatistiques(
            IJudoData data,
            HashSet<GroupeStatistiques> setGroupes,
            Dictionary<int, HashSet<EchelonEnum>> setTypesParComp)
        {
            var compteurs = new Dictionary<StatistiqueCle, CompteurStatistiques>();

            CompteurStatistiques GetOrCreateCompteur(StatistiqueCle cle)
            {
                if (string.IsNullOrEmpty(cle.IdEntite)) return null;
                if (!compteurs.TryGetValue(cle, out var c))
                {
                    c = new CompteurStatistiques(cle.TypeEntite);
                    compteurs[cle] = c;
                }
                return c;
            }

            var competition = data.Organisation?.Competitions is { Count: > 0 } comps ? comps[0] : null;
            int idComp = competition?.id ?? 0;
            var echelonsCibles = DeterminerEchelonsCibles(competition != null ? (EchelonEnum)competition.niveau : EchelonEnum.Club);

            void EnregistrerGroupe(EchelonEnum echelon, EpreuveSexe sexe, string idEntite)
            {
                if (string.IsNullOrEmpty(idEntite)) return;
                setGroupes.Add(new GroupeStatistiques(idComp, sexe, (int)echelon, idEntite));

                if (!setTypesParComp.TryGetValue(idComp, out var typesDispos))
                {
                    typesDispos = new HashSet<EchelonEnum>();
                    setTypesParComp[idComp] = typesDispos;
                }
                typesDispos.Add(echelon);
            }

            // 1. Passe des Participants (Initialisation et Participation)
            foreach (IVueJudoka participant in data.Participants?.Vuejudokas ?? Enumerable.Empty<IVueJudoka>())
            {
                bool estPresent = participant.present;

                // Génération en cascade de toutes les clés d'identification de ce judoka
                var clesImpactees = GetClesCascadePourParticipant(participant, echelonsCibles).ToList();

                foreach (var cle in clesImpactees)
                {
                    var c = GetOrCreateCompteur(cle);
                    if (cle.TypeEntite == TypeEntiteStatistique.Structure && c != null)
                    {
                        // On enregistre les groupes structurels pour le XSLT (uniquement Club, Ligue, etc.)
                        EnregistrerGroupe(echelonsCibles.First(), cle.Sexe, cle.IdEntite); // Simplifié pour l'exemple

                        c.NbParticipants++;
                        if (estPresent) c.NbCombattants++;
                    }
                }
            }

            // 2. Passe des Combats 
            foreach (Combat combat in data.Deroulement?.Combats?.OfType<Combat>() ?? Enumerable.Empty<Combat>())
            {
                if (!combat.vainqueur.HasValue || combat.virtuel || !combat.participant1.HasValue || !combat.participant2.HasValue) continue;

                var p1 = data.Participants?.Vuejudokas?.FirstOrDefault(p => p.id == combat.participant1.Value);
                var p2 = data.Participants?.Vuejudokas?.FirstOrDefault(p => p.id == combat.participant2.Value);
                if (p1 == null || p2 == null) continue;

                var clesP1 = GetClesCascadePourParticipant(p1, echelonsCibles);
                var clesP2 = GetClesCascadePourParticipant(p2, echelonsCibles);

                string penP1Str = combat.GetPenalites(1)?.TrimStart('-');
                string penP2Str = combat.GetPenalites(2)?.TrimStart('-');
                int pen1Count = ParseNombrePenalites(penP1Str);
                int pen2Count = ParseNombrePenalites(penP2Str);

                bool p1Gagne = combat.vainqueur == combat.participant1;
                bool p2Gagne = combat.vainqueur == combat.participant2;
                bool estHikiwake = !p1Gagne && !p2Gagne;

                TimeSpan tEffectif = combat.fin - combat.debut;
                if (tEffectif < TimeSpan.Zero) tEffectif = TimeSpan.Zero;
                TimeSpan tNominal = TimeSpan.FromMinutes(combat.temps);
                bool isGoldenScore = combat.goldenScore || tEffectif > tNominal;
                TimeSpan dureeGolden = (isGoldenScore && tEffectif > tNominal) ? tEffectif - tNominal : TimeSpan.Zero;

                void AppliquerStats(IEnumerable<StatistiqueCle> cles, bool estVainqueur, bool hikiwake, int score, string penAdversaire, int nbPenalitesRecues)
                {
                    foreach (var cle in cles)
                    {
                        var c = GetOrCreateCompteur(cle);
                        if (c == null) continue;

                        c.NbCombats++;
                        c.TotalPenalites += nbPenalitesRecues;
                        c.TotalDureeCombat += tEffectif;

                        if (tEffectif < c.DureeCombatMinInterne) c.DureeCombatMinInterne = tEffectif;
                        if (tEffectif > c.DureeCombatMaxInterne) c.DureeCombatMaxInterne = tEffectif;

                        if (estVainqueur)
                        {
                            c.NbVictoires++;
                            AnalyserVictoire(c, score, penAdversaire);
                        }
                        else if (hikiwake) c.NbHikiwake++;

                        if (isGoldenScore)
                        {
                            c.NbCombatsGoldenScore++;
                            c.TotalDureeGoldenScore += dureeGolden;
                            if (dureeGolden > c.DureeMaximaleGoldenScoreInterne) c.DureeMaximaleGoldenScoreInterne = dureeGolden;
                        }
                    }
                }

                AppliquerStats(clesP1, p1Gagne, estHikiwake, combat.score1, penP2Str, pen1Count);
                AppliquerStats(clesP2, p2Gagne, estHikiwake, combat.score2, penP1Str, pen2Count);
            }

            return compteurs.ToDictionary(k => k.Key, v => (IStatistiquesItem)v.Value);
        }

        // --- LA CASCADE (Drill-Down) ---
        private IEnumerable<StatistiqueCle> GetClesCascadePourParticipant(IVueJudoka p, HashSet<EchelonEnum> echelonsCibles)
        {
            if (p == null) yield break;

            // 1. Clé individuelle (La feuille)
            yield return new StatistiqueCle(TypeEntiteStatistique.Judoka, p.id.ToString(), p.sexeEnum);

            // 2. Clés structurelles (Les branches, filtrées par le niveau de la compétition)
            if (echelonsCibles.Contains(EchelonEnum.National) && p.pays != 0)
                yield return new StatistiqueCle(TypeEntiteStatistique.Structure, p.pays.ToString(), p.sexeEnum);

            if (echelonsCibles.Contains(EchelonEnum.Ligue) && !string.IsNullOrEmpty(p.ligue))
                yield return new StatistiqueCle(TypeEntiteStatistique.Structure, p.ligue, p.sexeEnum);

            if (echelonsCibles.Contains(EchelonEnum.Departement) && !string.IsNullOrEmpty(p.comite))
                yield return new StatistiqueCle(TypeEntiteStatistique.Structure, p.comite, p.sexeEnum);

            if (echelonsCibles.Contains(EchelonEnum.Club) && !string.IsNullOrEmpty(p.club))
                yield return new StatistiqueCle(TypeEntiteStatistique.Structure, p.club, p.sexeEnum);
        }

        private HashSet<EchelonEnum> DeterminerEchelonsCibles(EchelonEnum niveau)
        {
            var echelons = new HashSet<EchelonEnum> { EchelonEnum.Club };
            if (niveau >= EchelonEnum.Departement) echelons.Add(EchelonEnum.Departement);
            if (niveau >= EchelonEnum.Ligue) echelons.Add(EchelonEnum.Ligue);
            if (niveau >= EchelonEnum.National) echelons.Add(EchelonEnum.National);
            return echelons;
        }

        private void AnalyserVictoire(CompteurStatistiques c, int score, string pen)
        {
            if (pen == "3") { c.NbVictoireSogoGachi++; return; }
            if (pen == "H" || pen == "X") { c.NbVictoireHansokuMake++; return; }

            int ipponV = score / 100;
            int wazaV = (score / 10) % 10;
            int yukoV = score % 10;

            if (ipponV >= 1) c.NbVictoireIpponDirect++;
            else if (wazaV >= 2) c.NbVictoireWazaAriAwaseteIppon++;
            else if (wazaV == 1) c.NbVictoireWazaAri++;
            else if (yukoV >= 1) c.NbVictoireYuko++;
        }

        private int ParseNombrePenalites(string p) => (p == "H" || p == "X" || p == "3" || p == "A" || p == "M" || p == "F") ? 3 : (p == "2" ? 2 : (p == "1" ? 1 : 0));
    }

    // Le CompteurInterne reste strictement votre classe contenant les propriétés brutes et les calculs de ratios (PctVictoires, etc.)
    internal class CompteurStatistiques : IStatistiquesItem
    {
        public TypeEntiteStatistique TypeEntite { get; }

        // =========================================================
        // 1. COMPTEURS BRUTS (Alimentés par DataStatistiquesCombats)
        // =========================================================

        public int? NbParticipants { get; set; }
        public int? NbCombattants { get; set; }

        public int NbCombats { get; set; }
        public int NbVictoires { get; set; }
        public int NbHikiwake { get; set; }

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

        // Initialisé au max pour permettre de trouver le minimum absolu lors de la passe
        public TimeSpan DureeCombatMinInterne { get; set; } = TimeSpan.MaxValue;

        // =========================================================
        // 2. CONSTRUCTEUR
        // =========================================================

        public CompteurStatistiques(TypeEntiteStatistique typeEntite)
        {
            TypeEntite = typeEntite;

            // Les compteurs de participation n'ont de sens que pour les structures
            if (typeEntite == TypeEntiteStatistique.Structure)
            {
                NbParticipants = 0;
                NbCombattants = 0;
            }
        }

        // =========================================================
        // 3. PROPRIÉTÉS CALCULÉES (Implémentation de IStatistiquesItem)
        // =========================================================

        // --- Participation ---
        public double? PctParticipation => (!NbParticipants.HasValue || NbParticipants.Value == 0)
            ? null
            : (double)NbCombattants.Value / NbParticipants.Value;

        // --- Volumétrie et Ratios Globaux ---
        public double? PctVictoires => NbCombats == 0 ? null : (double)NbVictoires / NbCombats;
        public double? PctHikiwake => NbCombats == 0 ? null : (double)NbHikiwake / NbCombats;

        // --- Détail des victoires ---
        public double? PctVictoireIpponDirect => NbCombats == 0 ? null : (double)NbVictoireIpponDirect / NbCombats;
        public double? PctVictoireWazaAriAwaseteIppon => NbCombats == 0 ? null : (double)NbVictoireWazaAriAwaseteIppon / NbCombats;
        public double? PctVictoireWazaAri => NbCombats == 0 ? null : (double)NbVictoireWazaAri / NbCombats;
        public double? PctVictoireYuko => NbCombats == 0 ? null : (double)NbVictoireYuko / NbCombats;
        public double? PctVictoireSogoGachi => NbCombats == 0 ? null : (double)NbVictoireSogoGachi / NbCombats;
        public double? PctVictoireHansokuMake => NbCombats == 0 ? null : (double)NbVictoireHansokuMake / NbCombats;

        // --- Pénalités ---
        public double? MoyennePenalitesParCombat => NbCombats == 0 ? null : (double)TotalPenalites / NbCombats;

        // --- Golden Score ---
        public double? PctCombatsGoldenScore => NbCombats == 0 ? null : (double)NbCombatsGoldenScore / NbCombats;

        public TimeSpan? DureeMoyenneGoldenScore => NbCombatsGoldenScore == 0
            ? null
            : TimeSpan.FromTicks(TotalDureeGoldenScore.Ticks / NbCombatsGoldenScore);

        public TimeSpan? DureeMaximaleGoldenScore => NbCombatsGoldenScore == 0
            ? null
            : DureeMaximaleGoldenScoreInterne;

        // --- Temps de combat ---
        public TimeSpan? DureeCombatMin => NbCombats == 0
            ? null
            : DureeCombatMinInterne;

        public TimeSpan? DureeCombatMax => NbCombats == 0
            ? null
            : DureeCombatMaxInterne;

        public TimeSpan? DureeCombatMoy => NbCombats == 0
            ? null
            : TimeSpan.FromTicks(TotalDureeCombat.Ticks / NbCombats);
    }
}