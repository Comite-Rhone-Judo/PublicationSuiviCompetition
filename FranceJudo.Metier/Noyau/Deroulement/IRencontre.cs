using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using FranceJudo.Core.XML;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    /// <summary>
    /// Description des Rencontres
    /// </summary>
    public interface IRencontre
    {
        public int id { get; set; }
        public Nullable<int> judoka1 { get; set; }
        public Nullable<int> judoka2 { get; set; }
        public int score1 { get; set; }
        public int score2 { get; set; }
        public int penalite1 { get; set; }
        public int penalite2 { get; set; }
        public int etatJ1 { get; set; }
        public int etatJ2 { get; set; }
        public string details { get; set; }
        public DateTime programmation { get; set; }
        public DateTime debut { get; set; }
        public DateTime fin { get; set; }
        public double temps { get; set; }
        public int etat { get; set; }
        public int arbitre1 { get; set; }
        public int arbitre2 { get; set; }
        public int arbitre3 { get; set; }
        public Nullable<int> vainqueur { get; set; }
        public Nullable<int> combat { get; set; }
        public int CatePoids { get; set; }
        public int ippon1 { get; set; }
        public int ippon2 { get; set; }
        public int tempsCombat { get; set; }
        public int tempsRecuperation { get; set; }
        public int tempsHippon { get; set; }
        public int tempsWazaAri { get; set; }
        public int tempsYuko { get; set; }

        public int tempsRecupFinal { get; set; }
        public string discipline { get; set; }
        public bool goldenScore { get; set; }
        public bool isNewRencontre { get; set; }

        public bool estDecisif { get; set; }
        // Retourne True si le combat peut etre selectionne (il a tout ses participants), false sinon
        public bool IsPlayable { get; }

        /// <summary>
        /// Etablir le Score d'un combat
        /// </summary>
        /// <param name="DC"></param>

        public int CalculeScore();

        public int CalculeScorePerdant();
    }
}
