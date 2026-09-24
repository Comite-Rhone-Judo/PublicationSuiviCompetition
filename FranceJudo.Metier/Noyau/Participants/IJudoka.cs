using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.XML;
using System;

namespace FranceJudo.Metier.Noyau.Participants
{
    /// <summary>
    /// Description des Judokas
    /// </summary>
    public interface IJudoka : IXMLSerializable
    {
        public int id { get; set; }
        public string licence { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public int ceinture { get; set; }
        public DateTime naissance { get; set; }
        public bool sexe { get; set; }
        public EpreuveSexe sexeEnum { get; set; }
        public bool modification { get; set; }
        public bool present { get; set; }
        public bool passeport { get; set; }
        public int poids { get; set; }
        public int poidsMesure { get; set; }
        public float poidsKg { get; set; }
        public int categorie { get; set; }
        public int pays { get; set; }
        public EtatJudokaEnum etat { get; set; }
        public int modeControle { get; set; }
        public int modePesee { get; set; }
        public string club { get; set; }
        public string remoteID { get; set; }
        public DateTime datePesee { get; set; }
        public int qualifieE0 { get; set; }
        public int qualifieE1 { get; set; }
        public bool ajoute { get; set; }
        public int equipe { get; set; }

    }
}
