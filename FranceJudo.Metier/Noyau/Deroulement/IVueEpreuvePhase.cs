namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface IVueEpreuvePhase
    {
        public int id { get; }
        public TypePhaseEnum type_phase { get; }
        public string nom { get; }
        public EtatPhaseEnum etat { get; }
        public int nbcombat { get; set; }
        public int nbcombatRep { get; set; }
        public int nbcombattotal { get; set; }
        public int valid { get; set; }
    }
}
