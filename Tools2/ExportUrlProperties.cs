using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Export
{
    public class UrlProperties
    {

        private string pfName;        // nom generique du fichier pdf
        private int epreuveId;   // id de l'epreuve de cet url (file name)
        private int typePhase; // type de la phase



        public string PfName { get => pfName; set => pfName = value; }
        public int EpreuveId { get => epreuveId; set => epreuveId = value; }
        public int TypePhase { get => typePhase; set => typePhase = value; }
    }
}
