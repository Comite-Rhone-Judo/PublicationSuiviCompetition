using AppPublication.ExtensionNoyau.Engagement;
using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using KernelImpl.Noyau.Deroulement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    public class DataStatistiquesCombats : IDataStatistiquesCombats
    {
        // --- 1. CHAMPS PRIVÉS ---
        private readonly Dictionary<int, IStatistiquesItem> _statsJudokas;
        private readonly Dictionary<GroupeStatistiques, IStatistiquesItem> _statsStructures;
        private readonly Dictionary<GroupeStatistiques, List<IVueJudoka>> _judokasParGroupe;

        private readonly List<GroupeStatistiques> _groupesStatistiques;
        private readonly Dictionary<int, List<EchelonEnum>> _typesGroupes;

        // --- 2. PROPRIÉTÉS PUBLIQUES EN LECTURE SEULE ---
        public IReadOnlyDictionary<int, IStatistiquesItem> StatsJudokas => _statsJudokas;
        public IReadOnlyDictionary<GroupeStatistiques, IStatistiquesItem> StatsStructures => _statsStructures;
        public IReadOnlyDictionary<GroupeStatistiques, List<IVueJudoka>> JudokasParGroupe => _judokasParGroupe;

        public IReadOnlyList<GroupeStatistiques> GroupesStatistiques => _groupesStatistiques;
        public IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes => _typesGroupes;

        // --- 3. CONSTRUCTEUR ---
        public DataStatistiquesCombats(IJudoData snapshot)
        {
            _typesGroupes = BuildTypesGroupes(snapshot);

            // On assigne directement les retours (out) aux champs privés
            BuildStatistiques(snapshot,
                out _statsJudokas,
                out _statsStructures,
                out _judokasParGroupe,
                out _groupesStatistiques);
        }

        private Dictionary<int, List<EchelonEnum>> BuildTypesGroupes(IJudoData dataContext)
        {
            var dict = new Dictionary<int, List<EchelonEnum>>();

            foreach (ICompetition comp in dataContext.Organisation.Competitions.ToList())
            {
                List<EchelonEnum> listEchelon = new List<EchelonEnum> { EchelonEnum.Aucun };

                switch (comp.niveau)
                {
                    case (int)EchelonEnum.Club:
                        listEchelon.Add(EchelonEnum.Club);
                        break;
                    case (int)EchelonEnum.Departement:
                        listEchelon.Add(EchelonEnum.Club);
                        listEchelon.Add(EchelonEnum.Departement);
                        break;
                    case (int)EchelonEnum.Ligue:
                        listEchelon.Add(EchelonEnum.Club);
                        listEchelon.Add(EchelonEnum.Departement);
                        listEchelon.Add(EchelonEnum.Ligue);
                        break;
                    case (int)EchelonEnum.National:
                    case (int)EchelonEnum.International:
                        listEchelon.Add(EchelonEnum.Club);
                        listEchelon.Add(EchelonEnum.Departement);
                        listEchelon.Add(EchelonEnum.Ligue);
                        listEchelon.Add(EchelonEnum.National);
                        break;
                    default:
                        listEchelon.Add(EchelonEnum.Club);
                        break;
                }
                dict.Add(comp.id, listEchelon);
            }
            return dict;
        }

        private void BuildStatistiques(IJudoData data,
            out Dictionary<int, IStatistiquesItem> outStatsJudokas,
            out Dictionary<GroupeStatistiques, IStatistiquesItem> outStatsStructures,
            out Dictionary<GroupeStatistiques, List<IVueJudoka>> outJudokasParGroupe,
            out List<GroupeStatistiques> outGroupes)
        {
            var statsJudokas = new Dictionary<int, CompteurStatistiques>();
            var statsStructures = new Dictionary<GroupeStatistiques, CompteurStatistiques>();
            var judokasParGroupe = new Dictionary<GroupeStatistiques, List<IVueJudoka>>();
            var groupesUniques = new HashSet<GroupeStatistiques>();

            // 1. ISOLATION ABSOLUE DES DONNÉES (Évite les verrous UI)
            var competitionsSnap = data.Organisation?.Competitions?.ToList() ?? new List<ICompetition>();
            var epreuvesSnap = data.Organisation?.Epreuves?.ToList() ?? new List<IEpreuve>();
            var judokasSnap = data.Participants?.Vuejudokas?.ToList() ?? new List<IVueJudoka>();
            var combatsSnap = data.Deroulement?.Combats?.OfType<Combat>().ToList() ?? new List<Combat>();

            var dictJudokas = new Dictionary<int, IVueJudoka>();
            foreach (var j in judokasSnap) dictJudokas[j.id] = j;

            // ==========================================
            // 1. PASSE DES PARTICIPANTS 
            // ==========================================
            foreach (var competition in competitionsSnap)
            {
                if (!competition.IsShiai() && !competition.IsIndividuelle()) continue;
                if (!_typesGroupes.TryGetValue(competition.id, out var echelonsCibles)) continue;

                foreach (EpreuveSexeEnum s in Enum.GetValues<EpreuveSexeEnum>())
                {
                    var sexe = new EpreuveSexe(s);
                    var epreuvesSexe = epreuvesSnap.Where(ep => ep.competition == competition.id && ep.sexeEnum.Enum == s).ToList();

                    var judokasParticipants = judokasSnap
                        .Join(epreuvesSexe, vj => vj.idepreuve, ep => ep.id, (vj, ep) => vj)
                        .Distinct(new VueJudokaEqualityComparer())
                        .ToList();

                    foreach (var judoka in judokasParticipants)
                    {
                        // A. Initialisation des stats individuelles (EchelonEnum.Aucun)
                        statsJudokas[judoka.id] = new CompteurStatistiques(EchelonEnum.Aucun);

                        // B. Calcul des groupes de navigation pour l'UI
                        var groupesImpactes = GetGroupesCascadePourParticipant(judoka, echelonsCibles);

                        foreach (var groupe in groupesImpactes)
                        {
                            groupesUniques.Add(groupe);

                            // Ajout du judoka à la liste de ce groupe
                            if (!judokasParGroupe.TryGetValue(groupe, out var listeJ))
                            {
                                listeJ = new List<IVueJudoka>();
                                judokasParGroupe[groupe] = listeJ;
                            }
                            listeJ.Add(judoka);

                            // C. Initialisation des stats structurelles UNIQUEMENT (On ignore la lettre)
                            if (groupe.Type != EchelonEnum.Aucun)
                            {
                                if (!statsStructures.TryGetValue(groupe, out var cStruct))
                                {
                                    cStruct = new CompteurStatistiques(groupe.Type);
                                    statsStructures[groupe] = cStruct;
                                }
                                cStruct.NbParticipants = (cStruct.NbParticipants ?? 0) + 1;
                                if (judoka.present) cStruct.NbCombattants = (cStruct.NbCombattants ?? 0) + 1;
                            }
                        }
                    }
                }
            }

            // ==========================================
            // 2. PASSE DES COMBATS
            // ==========================================
            foreach (var combat in combatsSnap)
            {
                if (!combat.vainqueur.HasValue || combat.virtuel || !combat.participant1.HasValue || !combat.participant2.HasValue) continue;

                if (!dictJudokas.TryGetValue(combat.participant1.Value, out var p1) ||
                    !dictJudokas.TryGetValue(combat.participant2.Value, out var p2))
                    continue;

                if (!_typesGroupes.TryGetValue(p1.idcompet, out var typesP1) ||
                    !_typesGroupes.TryGetValue(p2.idcompet, out var typesP2))
                    continue;

                var groupesP1 = GetGroupesCascadePourParticipant(p1, typesP1);
                var groupesP2 = GetGroupesCascadePourParticipant(p2, typesP2);

                string penP1Str = combat.GetPenalites(1)?.TrimStart('-');
                string penP2Str = combat.GetPenalites(2)?.TrimStart('-');
                int pen1Count = ParseNombrePenalites(penP1Str);
                int pen2Count = ParseNombrePenalites(penP2Str);

                bool p1Gagne = combat.vainqueur == combat.participant1;
                bool p2Gagne = combat.vainqueur == combat.participant2;
                // On détecte explicitement le Hikiwake avec la valeur spéciale
                bool estHikiwake = combat.vainqueur == int.MinValue;

                TimeSpan tEffectif = combat.fin - combat.debut;
                if (tEffectif < TimeSpan.Zero) tEffectif = TimeSpan.Zero;
                TimeSpan tNominal = TimeSpan.FromMinutes(combat.temps);
                bool isGoldenScore = combat.goldenScore || tEffectif > tNominal;
                TimeSpan dureeGolden = (isGoldenScore && tEffectif > tNominal) ? tEffectif - tNominal : TimeSpan.Zero;

                // Ajout des paramètres etatJudoka et etatAdversaire pour l'analyse précise des victoires
                void AppliquerStats(IVueJudoka judoka, IEnumerable<GroupeStatistiques> groupesDuJudoka, bool estVainqueur, bool hikiwake, int score, string penAdversaire, int nbPenalitesRecues, int etatJudoka, int etatAdversaire)
                {
                    // Met a jour un compteur spécifique
                    void UpdateCompteur(CompteurStatistiques c)
                    {
                        c.NbCombats++;
                        c.TotalPenalites += nbPenalitesRecues;
                        c.TotalDureeCombat += tEffectif;

                        if (tEffectif < c.DureeCombatMinInterne) c.DureeCombatMinInterne = tEffectif;
                        if (tEffectif > c.DureeCombatMaxInterne) c.DureeCombatMaxInterne = tEffectif;

                        if (estVainqueur)
                        {
                            c.NbVictoires++;
                            // Le judoka étant le vainqueur, on passe son état (etatVainqueur) et celui de l'adversaire (etatPerdant)
                            AnalyserVictoire(c, score, penAdversaire, etatJudoka, etatAdversaire);
                        }
                        else if (hikiwake)
                        {
                            c.NbHikiwake++;
                        }

                        if (isGoldenScore)
                        {
                            c.NbCombatsGoldenScore++;
                            c.TotalDureeGoldenScore += dureeGolden;
                            if (dureeGolden > c.DureeMaximaleGoldenScoreInterne) c.DureeMaximaleGoldenScoreInterne = dureeGolden;
                        }
                    }

                    // A. On met à jour les stats individuelles du Judoka
                    if (statsJudokas.TryGetValue(judoka.id, out var cIndiv)) UpdateCompteur(cIndiv);

                    // B. On met à jour les stats structurelles (en ignorant le groupe Alphabétique = Aucun)
                    foreach (var groupe in groupesDuJudoka.Where(g => g.Type != EchelonEnum.Aucun))
                    {
                        if (statsStructures.TryGetValue(groupe, out var cStruct)) UpdateCompteur(cStruct);
                    }
                }

                // Appel de la méthode en passant les états respectifs (combat.etatJ1 et combat.etatJ2)
                AppliquerStats(p1, groupesP1, p1Gagne, estHikiwake, combat.score1, penP2Str, pen1Count, combat.etatJ1, combat.etatJ2);
                AppliquerStats(p2, groupesP2, p2Gagne, estHikiwake, combat.score2, penP1Str, pen2Count, combat.etatJ2, combat.etatJ1);
            } // Fin du foreach (var combat in combatsSnap)

            outGroupes = groupesUniques.ToList();
            outStatsJudokas = statsJudokas.ToDictionary(k => k.Key, v => (IStatistiquesItem)v.Value);
            outStatsStructures = statsStructures.ToDictionary(k => k.Key, v => (IStatistiquesItem)v.Value);
            outJudokasParGroupe = judokasParGroupe;
        }

        private IEnumerable<GroupeStatistiques> GetGroupesCascadePourParticipant(IVueJudoka p, List<EchelonEnum> echelonsCibles)
        {
            var groupes = new List<GroupeStatistiques>();
            if (p == null) return groupes;

            // 1. Groupement alphabétique (Niveau Aucun)
            if (echelonsCibles.Contains(EchelonEnum.Aucun) && !string.IsNullOrWhiteSpace(p.nom))
            {
                string premiereLettre = p.nom.Trim().Substring(0, 1).ToUpper();
                groupes.Add(new GroupeStatistiques(p.idcompet, p.sexeEnum, premiereLettre, EchelonEnum.Aucun));
            }

            // 2. Groupements Structurels
            if (echelonsCibles.Contains(EchelonEnum.National) && p.pays != 0)
                groupes.Add(new GroupeStatistiques(p.idcompet, p.sexeEnum, p.pays.ToString(), EchelonEnum.National));

            if (echelonsCibles.Contains(EchelonEnum.Ligue) && !string.IsNullOrEmpty(p.ligue))
                groupes.Add(new GroupeStatistiques(p.idcompet, p.sexeEnum, p.ligue, EchelonEnum.Ligue));

            if (echelonsCibles.Contains(EchelonEnum.Departement) && !string.IsNullOrEmpty(p.comite))
                groupes.Add(new GroupeStatistiques(p.idcompet, p.sexeEnum, p.comite, EchelonEnum.Departement));

            if (echelonsCibles.Contains(EchelonEnum.Club) && !string.IsNullOrEmpty(p.club))
                groupes.Add(new GroupeStatistiques(p.idcompet, p.sexeEnum, p.club, EchelonEnum.Club));

            return groupes;
        }

        private void AnalyserVictoire(CompteurStatistiques c, int score, string pen, int etatVainqueur, int etatPerdant)
        {
            // 1. Victoire par décision (Le vainqueur a l'état 7)
            if (etatVainqueur == 7)
            {
                c.NbVictoireDecision++;
                return;
            }

            // 2. Victoire par Abandon (2), Forfait (3) ou Médical (4) du perdant
            if (etatPerdant == 2 || etatPerdant == 3 || etatPerdant == 4)
            {
                c.NbVictoireAbandonForfaitMedical++;
                return;
            }

            // 3. Victoires par pénalités du perdant
            // Nettoyage de la chaîne de pénalité (retrait des tirets et espaces cachés)
            string p = pen?.Replace("-", "").Trim().ToUpper() ?? "";

            if (p == "3")
            {
                c.NbVictoireSogoGachi++;
                return;
            }
            // Hansoku-Make (H = 5) direct ou Hansoku-Make cumulé (X = 6)
            if (etatPerdant == 5 || etatPerdant == 6 || p == "H" || p == "X")
            {
                c.NbVictoireHansokuMake++;
                return;
            }

            // 4. Victoires par points techniques (Ippon, Waza-ari, Yuko)
            int ipponV = score / 100;
            int wazaV = (score / 10) % 10;
            int yukoV = score % 10;

            if (ipponV >= 1) c.NbVictoireIpponDirect++;
            else if (wazaV >= 2) c.NbVictoireWazaAriAwaseteIppon++;
            else if (wazaV == 1) c.NbVictoireWazaAri++;
            else if (yukoV >= 1) c.NbVictoireYuko++;
        }

        private int ParseNombrePenalites(string pen)
        {
            // Nettoyage de la chaîne (ex: "-2" devient "2")
            string p = pen?.Replace("-", "").Trim().ToUpper() ?? "";

            if (p == "3") return 3;
            if (p == "2") return 2;
            if (p == "1") return 1;

            // Tous les autres cas (H, X, A, M, F, D, etc.) ne sont pas des shidos techniques
            return 0;
        }
    }

    internal class CompteurStatistiques : IStatistiquesItem
    {
        public EchelonEnum TypeEntite { get; }

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
        public int NbVictoireDecision { get; set; }

        public int NbVictoireAbandonForfaitMedical { get; set; }

        public int TotalPenalites { get; set; }

        public int NbCombatsGoldenScore { get; set; }
        public TimeSpan TotalDureeGoldenScore { get; set; }
        public TimeSpan DureeMaximaleGoldenScoreInterne { get; set; }

        public TimeSpan TotalDureeCombat { get; set; }
        public TimeSpan DureeCombatMaxInterne { get; set; }

        public TimeSpan DureeCombatMinInterne { get; set; } = TimeSpan.MaxValue;

        public CompteurStatistiques(EchelonEnum typeEntite)
        {
            TypeEntite = typeEntite;

            // Les compteurs de participation n'ont de sens que pour les structures (type != Aucun)
            if (typeEntite != EchelonEnum.Aucun)
            {
                NbParticipants = 0;
                NbCombattants = 0;
            }
        }

        public double? PctParticipation => (!NbParticipants.HasValue || NbParticipants.Value == 0) ? null : (double)NbCombattants.Value / NbParticipants.Value;
        public double? PctVictoires => NbCombats == 0 ? null : (double)NbVictoires / NbCombats;
        public double? PctHikiwake => NbCombats == 0 ? null : (double)NbHikiwake / NbCombats;
        public double? PctVictoireIpponDirect => NbCombats == 0 ? null : (double)NbVictoireIpponDirect / NbCombats;
        public double? PctVictoireWazaAriAwaseteIppon => NbCombats == 0 ? null : (double)NbVictoireWazaAriAwaseteIppon / NbCombats;
        public double? PctVictoireWazaAri => NbCombats == 0 ? null : (double)NbVictoireWazaAri / NbCombats;
        public double? PctVictoireYuko => NbCombats == 0 ? null : (double)NbVictoireYuko / NbCombats;
        public double? PctVictoireSogoGachi => NbCombats == 0 ? null : (double)NbVictoireSogoGachi / NbCombats;
        public double? PctVictoireHansokuMake => NbCombats == 0 ? null : (double)NbVictoireHansokuMake / NbCombats;

        public double? PctVictoireAbandonForfaitMedical => NbCombats == 0 ? null : (double)NbVictoireAbandonForfaitMedical / NbCombats;
        public double? PctVictoireDecision => NbCombats == 0 ? null : (double)NbVictoireDecision / NbCombats;

        public double? MoyennePenalitesParCombat => NbCombats == 0 ? null : (double)TotalPenalites / NbCombats;
        public double? PctCombatsGoldenScore => NbCombats == 0 ? null : (double)NbCombatsGoldenScore / NbCombats;
        public TimeSpan? DureeMoyenneGoldenScore => NbCombatsGoldenScore == 0 ? null : TimeSpan.FromTicks(TotalDureeGoldenScore.Ticks / NbCombatsGoldenScore);
        public TimeSpan? DureeMaximaleGoldenScore => NbCombatsGoldenScore == 0 ? null : DureeMaximaleGoldenScoreInterne;
        public TimeSpan? DureeCombatMin => NbCombats == 0 ? null : (DureeCombatMinInterne == TimeSpan.MaxValue ? TimeSpan.Zero : DureeCombatMinInterne);
        public TimeSpan? DureeCombatMax => NbCombats == 0 ? null : DureeCombatMaxInterne;
        public TimeSpan? DureeCombatMoy => NbCombats == 0 ? null : TimeSpan.FromTicks(TotalDureeCombat.Ticks / NbCombats);
    }
}