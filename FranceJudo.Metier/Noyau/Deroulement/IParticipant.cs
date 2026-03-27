using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    /// <summary>
    /// Description des Participants
    /// </summary>
    public interface IParticipant
    {
        public int judoka { get; set;  }
        public int id {  get; set; }
        public int phase {  get; set; }
        public int ranking { get; set; }
        public int classementAvant { get; set; }

        public int classementFinal { get; set; }
        public bool qualifie { get; set; }
        public int position { get; set; }
        public int ordreTirage { get; set; }
        public int poule { get; set; }
        public int positionOriginal { get; set; }
        public int nbVictoires { get; set; }
        public int nbVictoiresInd { get; set; }
        public int cumulPoints { get; set; }
        public int cumulPointsGRCH { get; set; }
        public DateTime dernierCombat { get; set; }

        public Participants.IJudoka Judoka1(IJudoData DC);

        public Participants.IEquipe Equipe1(IJudoData DC);
    }
}
