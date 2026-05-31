using System;
using System.Collections.Generic;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    /// <summary>
    /// Représente le bloc de statistiques exhaustif pour une entité.
    /// Basé sur AppPublication_Spécification Statistiques_2026-04-17-Rev. A.xlsx
    /// </summary>
    public interface IStatistiquesItem
    {
        /// <summary>
        /// Permet au moteur XSLT de conditionner l'affichage.
        /// </summary>
        TypeEntiteStatistique TypeEntite { get; }

        // --- Participation (STAT35, STAT36, STAT37) ---
        // Nullables car N/A pour un Judoka individuel
        int? NbParticipants { get; }
        int? NbCombattants { get; }
        double? PctParticipation { get; }

        // --- Volumétrie Combats (STAT02) ---
        int NbCombats { get; }

        // --- Victoires (STAT07 à STAT12) ---
        // Nullables si NbCombats == 0
        double? PctVictoireIpponDirect { get; }
        double? PctVictoireWazaAriAwaseteIppon { get; }
        double? PctVictoireWazaAri { get; }
        double? PctVictoireYuko { get; }
        double? PctVictoireSogoGachi { get; }
        double? PctVictoireHansokuMake { get; }

        // --- Pénalités (STAT18) ---
        // Nullable si NbCombats == 0
        double? MoyennePenalitesParCombat { get; }

        // --- Golden Score (STAT28 à STAT31) ---
        int NbCombatsGoldenScore { get; }
        double? PctCombatsGoldenScore { get; } // Nullable si NbCombats == 0
        TimeSpan? DureeMoyenneGoldenScore { get; } // Nullable si NbCombatsGoldenScore == 0
        TimeSpan? DureeMaximaleGoldenScore { get; } // Nullable si NbCombatsGoldenScore == 0

        // --- Temps de combat (STAT32 à STAT34) ---
        // Nullables si NbCombats == 0
        TimeSpan? DureeCombatMin { get; }
        TimeSpan? DureeCombatMax { get; }
        TimeSpan? DureeCombatMoy { get; }
    }
}