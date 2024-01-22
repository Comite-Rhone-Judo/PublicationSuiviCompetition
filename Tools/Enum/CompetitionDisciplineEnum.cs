using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tools.Enum
{
    /// <summary>
    /// Enumération des type de compétition
    /// </summary>
    public enum CompetitionDisciplineEnum
    {
        Judo = 1,
        JujitsuCombat = 2,
        JujitsuNeWaza = 3
    }

    public static class CompetitionDisciplineEnum_Extension
    {
        //public static string ToName(this CompetitionDisciplineEnum e)
        //{
        //    switch (e)
        //    {
        //        case CompetitionDisciplineEnum.JujitsuCombat:
        //            return CompetitionDisciplineEnum.JujitsuCombat.ToName();// ToString();
        //        case CompetitionDisciplineEnum.JujitsuNeWaza:
        //            return CompetitionDisciplineEnum.JujitsuNeWaza.ToName();
        //        default:
        //            return CompetitionDisciplineEnum.Judo.ToName();

        //    }
        //}

        //-	JJ_NE WAZA(pour le jujitsu)
        //-	JJ_COMBAT(pour le jujitsu)
        //-	C_COMPETITION(pour les compétitions judo)
        //-	PG_ UV3 - TEC(shiaï)     (pour les passages de grade)

        public static string ToString2(this CompetitionDisciplineEnum e)
        {
            switch (e)
            {
                case CompetitionDisciplineEnum.JujitsuCombat:
                    return "JJ_COMBAT";
                case CompetitionDisciplineEnum.JujitsuNeWaza:
                    return "JJ_NE WAZA";
                default:
                    return "C_COMPETITION";

            }
        }
        public static CompetitionDisciplineEnum ByString2(this string e)
        {
            switch (e)
            {
                case "JJ_COMBAT":
                    return CompetitionDisciplineEnum.JujitsuCombat;
                case "JJ_NE WAZA":
                    return CompetitionDisciplineEnum.JujitsuNeWaza;
                default:
                    return CompetitionDisciplineEnum.Judo;

            }
        }

        public static CompetitionDisciplineEnum ByString(this string e)
        {
            switch (e)
            {
                case nameof(CompetitionDisciplineEnum.JujitsuCombat):
                    return CompetitionDisciplineEnum.JujitsuCombat;
                case nameof(CompetitionDisciplineEnum.JujitsuNeWaza):
                    return CompetitionDisciplineEnum.JujitsuNeWaza;
            }
            return Enum.CompetitionDisciplineEnum.Judo;
        }

        public static string ToShort(this CompetitionDisciplineEnum e)
        {
            switch (e)
            {
                case CompetitionDisciplineEnum.JujitsuCombat:
                    return "Combat";
                case CompetitionDisciplineEnum.JujitsuNeWaza:
                    return "Ne-Waza";
                default:
                    return "Judo";

            }
        }

        public static string ToShort2(this CompetitionDisciplineEnum e)
        {
            switch (e)
            {
                case CompetitionDisciplineEnum.JujitsuCombat:
                    return "COMB";
                case CompetitionDisciplineEnum.JujitsuNeWaza:
                    return "N W";
                default:
                    return "Judo";

            }
        }

        public static string ToLabel(this CompetitionDisciplineEnum e)
        {
            switch (e)
            {
                case CompetitionDisciplineEnum.JujitsuCombat:
                    return "Jujitsu Combat";
                case CompetitionDisciplineEnum.JujitsuNeWaza:
                    return "Jujitsu Ne-Waza";
                default:
                    return "Judo";

            }
        }
        public static CompetitionDisciplineEnum ByLabel(this string e)
        {
            switch (e)
            {
                case "Jujitsu Combat":
                    return CompetitionDisciplineEnum.JujitsuCombat;
                case "Jujitsu Ne-Waza":
                    return CompetitionDisciplineEnum.JujitsuNeWaza;
            }
            return Enum.CompetitionDisciplineEnum.Judo;
        }

    }
}
