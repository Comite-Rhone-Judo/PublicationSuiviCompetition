using System;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface ICombat
    {
        public int id { get; set; }
        public bool IsPlayable { get; }
        public int numero { get; set; }
        public string reference { get; set; }
        public Nullable<int> participant1 { get; set; }
        public Nullable<int> participant2 { get; set; }
        public int score1 { get; set; }
        public int score2 { get; set; }
        public int penalite1 { get; set; }
        public int penalite2 { get; set; }
        public int etatJ1 { get; set; }
        public int etatJ2 { get; set; }
        public int positionJ1 { get; set; }
        public int positionJ2 { get; set; }
        public int nbVictoire1 { get; set; }
        public int nbVictoire2 { get; set; }
        public string details { get; set; }
        public int phase { get; set; }
        public System.DateTime programmation { get; set; }
        public System.DateTime debut { get; set; }
        public System.DateTime fin { get; set; }
        public double temps { get; set; }
        public int etat { get; set; }
        public int arbitre1 { get; set; }
        public int arbitre2 { get; set; }
        public int arbitre3 { get; set; }
        public int niveau { get; set; }
        public Nullable<int> vainqueur { get; set; }
        public bool virtuel { get; set; }
        public int epreuve { get; set; }
        public Nullable<int> tapis { get; set; }
        public Nullable<int> groupe { get; set; }
        public int first_rencontre { get; set; }

        public int tempsCombat { get; set; }
        public int tempsRecuperation { get; set; }
        public int tempsHippon { get; set; }
        public int tempsWazaAri { get; set; }
        public int tempsYuko { get; set; }

        public int kinza1 { get; set; }
        public int kinza2 { get; set; }

        public bool goldenScore { get; set; }
        public bool isNewCombat { get; set; }

        public bool challenge1Refused { get; set; }
        public bool challenge2Refused { get; set; }

        public string scoresJujitsu { get; set; }

        public int pointsGRCH1 { get; set; }
        public int pointsGRCH2 { get; set; }

        public int tempsRecupFinal { get; set; }

        public string discipline { get; set; }

        public int ippon1 { get; set; }
        public int ippon2 { get; set; }

        public string couleur1 { get; set; }
        public string couleur2 { get; set; }
    }
}
