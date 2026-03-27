using FranceJudo.Metier.XML;
using System;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface IGroupe_Combats : IXMLSerializable
    {
        public int id { get; set; }
        public int decoupage { get; set; }
        public int tapis { get; set; }
        public string libelle { get; set; }
        public int numero { get; set; }
        public Nullable<DateTime> horaire_debut { get; set; }
        public Nullable<DateTime> horaire_fin { get; set; }
        public bool verrouille { get; set; }


        public Organisation.IEpreuve GetEpreuve(IJudoData DC);


        public IPhase GetPhase(IJudoData DC);
    }
}
