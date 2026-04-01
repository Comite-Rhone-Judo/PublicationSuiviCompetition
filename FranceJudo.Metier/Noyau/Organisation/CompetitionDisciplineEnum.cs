namespace FranceJudo.Metier.Noyau.Organisation
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
            return e switch
            {
                CompetitionDisciplineEnum.JujitsuCombat => "JJ_COMBAT",
                CompetitionDisciplineEnum.JujitsuNeWaza => "JJ_NE WAZA",
                _ => "C_COMPETITION",
            };
        }
        public static CompetitionDisciplineEnum ByString2(this string e)
        {
            return e switch
            {
                "JJ_COMBAT" => CompetitionDisciplineEnum.JujitsuCombat,
                "JJ_NE WAZA" => CompetitionDisciplineEnum.JujitsuNeWaza,
                _ => CompetitionDisciplineEnum.Judo,
            };
        }

        public static CompetitionDisciplineEnum ByString(this string e)
        {
            return e switch
            {
                nameof(CompetitionDisciplineEnum.JujitsuCombat) => CompetitionDisciplineEnum.JujitsuCombat,
                nameof(CompetitionDisciplineEnum.JujitsuNeWaza) => CompetitionDisciplineEnum.JujitsuNeWaza,
                _ => CompetitionDisciplineEnum.Judo,
            };
        }

        public static string ToShort(this CompetitionDisciplineEnum e)
        {
            return e switch
            {
                CompetitionDisciplineEnum.JujitsuCombat => "Combat",
                CompetitionDisciplineEnum.JujitsuNeWaza => "Ne-Waza",
                _ => "Judo",
            };
        }

        public static string ToShort2(this CompetitionDisciplineEnum e)
        {
            return e switch
            {
                CompetitionDisciplineEnum.JujitsuCombat => "COMB",
                CompetitionDisciplineEnum.JujitsuNeWaza => "N W",
                _ => "Judo",
            };
        }

        public static string ToLabel(this CompetitionDisciplineEnum e)
        {
            return e switch
            {
                CompetitionDisciplineEnum.JujitsuCombat => "Jujitsu Combat",
                CompetitionDisciplineEnum.JujitsuNeWaza => "Jujitsu Ne-Waza",
                _ => "Judo",
            };
        }
        public static CompetitionDisciplineEnum ByLabel(this string e)
        {
            return e switch
            {
                "Jujitsu Combat" => CompetitionDisciplineEnum.JujitsuCombat,
                "Jujitsu Ne-Waza" => CompetitionDisciplineEnum.JujitsuNeWaza,
                _ => CompetitionDisciplineEnum.Judo,
            };
        }

    }
}
