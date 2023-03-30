using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Export
{
    public class UrlProperties
    {

        private string _pfName;        // nom generique du fichier pdf
        private int _epreuveId;   // id de l'epreuve de cet url (file name)
        private int _typePhase; // type de la phase


        public UrlProperties(string name, int epId, int phaseT)
        {
            _pfName = name;
            _epreuveId = epId;
            _typePhase = phaseT; 
        }


        public string PfName
        {
            get 
            {
                return _pfName;
            }
            set 
            {
                _pfName = value;
            }
        } 
        public int EpreuveId
        {
            get
            {
                return _epreuveId;
            }
            set
            {
                _epreuveId = value;
            }
        }
        public int TypePhase
        {
            get
            {
                return _typePhase;
            }
            set
            {
                _typePhase = value;
            }
        }
    }
}
