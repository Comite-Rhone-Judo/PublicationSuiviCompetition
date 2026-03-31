namespace FranceJudo.Metier.Noyau.Organisation
{
    public interface Ivue_epreuve_equipe : i_vue_epreuve_interface
    {
        public EpreuveEquipeTypeEnum type { get; set; }
        public int epreuveRef { get; set; }
    }
}
