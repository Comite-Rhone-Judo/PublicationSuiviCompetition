using FranceJudo.Metier.XML;
using System;

namespace FranceJudo.Metier.Noyau.Organisation
{
    /// <summary>
    /// Description des Competitions
    /// </summary>
    public interface ICompetition : IXMLSerializable
    {
        public int id { get; set; }
        public string nom { get; set; }
        public DateTime date { get; set; }
        public string lieu { get; set; }
        public string siteInternet { get; set; }

        public string remoteId { get; set; }
        public string codeAcces { get; set; }
        public CompetitionTypeEnum type { get; set; }
        public CompetitionType2Enum type2 { get; set; }

        public string discipline { get; set; }

        public CompetitionDisciplineEnum disciplineId { get; }

        public int nbTapis { get; set; }
        public int tempsCombat { get; set; }
        public int niveau { get; set; }
        public string couleur1 { get; set; }
        public string couleur2 { get; set; }
        public string version { get; set; }

        public int afficheCSA { get; set; }

        public bool afficheKinzas { get; set; }
        public bool afficheAutoTempsRecuperation { get; set; }

        public bool afficheAnimationVainqueur { get; set; }

        public int tempsMedical { get; set; }
        public bool isRandomCombat { get; set; }
        public ReglementEquipeEnum reglementEquipe { get; set; }

        public bool IsOfficielle();

        public bool IsProLeague();
        public bool IsIndividuelle();

        public bool IsShiai();

        public bool IsEquipe();

    }
}
