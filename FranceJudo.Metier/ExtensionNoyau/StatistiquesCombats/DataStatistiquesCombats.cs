using FranceJudo.Metier.ExtensionNoyau.Engagement;
using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.Noyau.Deroulement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FranceJudo.Metier.ExtensionNoyau.StatistiquesCombats
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
            var combatsSnap = data.Deroulement?.Combats?.OfType<ICombat>().ToList() ?? new List<ICombat>();

            var dictJudokas = new Dictionary<int, IVueJudoka>();
            foreach (var j in judokasSnap) dictJudokas[j.id] = j;

            // ==========================================
            // 1. PASSE DES PARTICIPANTS 
            // ==========================================
            foreach (var competition in competitionsSnap)
            {
                if (!competition.IsShiai() && !competition.IsIndividuelle()) continue;
                if (!_typesGroupes.TryGetValue(competition.id, out var echelonsCibles)) continue;

                foreach (EpreuveSexeEnum s in Enum.GetValues(typeof(EpreuveSexeEnum)))
                {
                    var sexe = new EpreuveSexe(s);
                    var epreuvesSexe = epreuvesSnap.Where(ep => ep.competition == competition.id && ep.sexeEnum.Enum == s).ToList();

                    var judokasParticipants = judokasSnap
                        .Join(epreuvesSexe, vj => vj.idepreuve, ep => ep.id, (vj, ep) => vj)
                        .Distinct(new VueJudokaEqualityComparer())
                        .ToList();

                    foreach (var judoka in judokasParticipants)
                    {
                        var groupesImpactes = GetGroupesCascadePourParticipant(judoka, echelonsCibles);

                        foreach (var groupe in groupesImpactes)
                        {
                            groupesUniques.Add(groupe);

                            // 1. Comptage structurel global (On compte les inscrits et les présents)
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

                            // 2. FILTRE STRICT : On arrête tout traitement supplémentaire si le judoka est absent
                            if (!judoka.present) continue;

                            // 3. Ajout du judoka à la liste de l'interface (Uniquement les présents)
                            if (!judokasParGroupe.TryGetValue(groupe, out var listeJ))
                            {
                                listeJ = new List<IVueJudoka>();
                                judokasParGroupe[groupe] = listeJ;
                            }
                            listeJ.Add(judoka);
                        }

                        // 4. Initialisation des stats individuelles (Uniquement pour les présents)
                        if (judoka.present)
                        {
                            statsJudokas[judoka.id] = new CompteurStatistiques(EchelonEnum.Aucun);
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

                // --- ANALYSE DES ÉTATS VIA ENUM ---
                EtatCombattantEnum etatP1 = combat.etatJ1;
                EtatCombattantEnum etatP2 = combat.etatJ2;

                // Détection des Hansoku-Make
                bool p1Hansoku = etatP1 == EtatCombattantEnum.HansokuMakeH || etatP1 == EtatCombattantEnum.HansokuMakeX;
                bool p2Hansoku = etatP2 == EtatCombattantEnum.HansokuMakeH || etatP2 == EtatCombattantEnum.HansokuMakeX;

                // Fonction locale pour vérifier si l'état est un motif de fin exceptionnel (A, M, F, H, X)
                bool EstFinExceptionnelle(EtatCombattantEnum etat)
                {
                    return etat == EtatCombattantEnum.Abandon ||
                           etat == EtatCombattantEnum.Medical ||
                           etat == EtatCombattantEnum.Forfait ||
                           etat == EtatCombattantEnum.HansokuMakeH ||
                           etat == EtatCombattantEnum.HansokuMakeX;
                }

                // Les Shidos techniques ne sont comptés que si ce n'est pas une fin exceptionnelle
                int pen1Count = !EstFinExceptionnelle(etatP1) ? combat.penalite1 : 0;
                int pen2Count = !EstFinExceptionnelle(etatP2) ? combat.penalite2 : 0;

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
                void AppliquerStats(IVueJudoka judoka, IEnumerable<GroupeStatistiques> groupesDuJudoka, bool estVainqueur, bool hikiwake, int scoreJudoka, int scoreAdversaire, bool adversaireEstHansoku, int nbPenalitesRecues, EtatCombattantEnum etatJudoka, EtatCombattantEnum etatAdversaire)
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
                            AnalyserVictoire(c, scoreJudoka, scoreAdversaire, adversaireEstHansoku, etatJudoka, etatAdversaire);
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
                AppliquerStats(p1, groupesP1, p1Gagne, estHikiwake, combat.score1, combat.score2, p2Hansoku, pen1Count, combat.etatJ1, combat.etatJ2);
                AppliquerStats(p2, groupesP2, p2Gagne, estHikiwake, combat.score2, combat.score1, p1Hansoku, pen2Count, combat.etatJ2, combat.etatJ1);

            } // Fin du foreach (var combat in combatsSnap)

            outGroupes = groupesUniques.ToList();
            outStatsJudokas = statsJudokas.ToDictionary(k => k.Key, v => (IStatistiquesItem)v.Value);
            outStatsStructures = statsStructures.ToDictionary(k => k.Key, v => (IStatistiquesItem)v.Value);
            outJudokasParGroupe = judokasParGroupe;
        }

        /// <summary>
        /// Cette méthode génère les groupes de statistiques pour un participant donné en fonction des échelons cibles spécifiés. Elle retourne une liste de groupes correspondant aux différents niveaux de regroupement (alphabétique, structurel, etc.) pour le participant.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="echelonsCibles"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Cette méthode analyse le type de victoire d'un combattant et met à jour les compteurs correspondants dans l'objet CompteurStatistiques.
        /// </summary>
        /// <param name="c">L'objet compteur de statistiques à mettre à jour</param>
        /// <param name="scoreVainqueur">Le score du vainqueur</param>
        /// <param name="scorePerdant">Le score du perdant</param>
        /// <param name="adversaireEstHansoku">Indique si l'adversaire est en hansoku</param>
        /// <param name="etatVainqueur">L'état du vainqueur</param>
        /// <param name="etatPerdant">L'état du perdant</param>
        private void AnalyserVictoire(CompteurStatistiques c, int scoreVainqueur, int scorePerdant, bool adversaireEstHansoku, EtatCombattantEnum etatVainqueur, EtatCombattantEnum etatPerdant)
        {
            // 1. Victoire par décision (Assurez-vous que la valeur 7 possède bien un nom dans votre enum, ex: Decision)
            if (etatVainqueur == EtatCombattantEnum.Decision)
            {
                c.NbVictoireDecision++;
                return;
            }

            // 2. Victoire par Abandon, Forfait ou Médical du perdant
            if (etatPerdant == EtatCombattantEnum.Abandon ||
                etatPerdant == EtatCombattantEnum.Forfait ||
                etatPerdant == EtatCombattantEnum.Medical)
            {
                c.NbVictoireAbandonForfaitMedical++;
                return;
            }

            // 3. Victoires par pénalités du perdant
            if (etatPerdant == EtatCombattantEnum.HansokuMakeH ||
                etatPerdant == EtatCombattantEnum.HansokuMakeX ||
                adversaireEstHansoku)
            {
                c.NbVictoireHansokuMake++;
                return;
            }

            // 4. Victoires par points techniques (Ippon, Waza-ari, Yuko)
            int ipponV = scoreVainqueur / 100;
            int wazaV = (scoreVainqueur / 10) % 10;
            int yukoV = scoreVainqueur % 10;

            int wazaP = (scorePerdant / 10) % 10;
            int yukoP = scorePerdant % 10;
            // Application stricte de la valeur décisive :
            if (ipponV >= 1)
            {
                c.NbVictoireIpponDirect++;
            }
            else if (wazaV >= 2)
            {
                c.NbVictoireWazaAriAwaseteIppon++;
            }
            else if (wazaV > wazaP) // On gagne par Waza-ari SEULEMENT si on en a strictement plus que l'adversaire
            {
                c.NbVictoireWazaAri++;
            }
            else if (yukoV > yukoP) // En cas d'égalité de Waza-ari, c'est la différence de Yuko qui qualifie la victoire
            {
                c.NbVictoireYuko++;
            }
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
        public TimeSpan DureeCombatMaxInterne { get; set; } = TimeSpan.MinValue;

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