using KernelImpl.Noyau.Organisation;


namespace KernelManager
{
    public class ManagerOrganisation
    {
        public Competition CreateCompetition()
        {
            return new Competition();
        }

        public Epreuve CreateEpreuve()
        {
            return new Epreuve();
        }

        public Epreuve_Equipe CreateEpreuveEquipe()
        {
            return new Epreuve_Equipe();
        }



        public vue_epreuve CreateVueEpreuve(Epreuve ep, KernelImpl.IJudoData DC)
        {
            return new vue_epreuve(ep, DC);
        }

        public vue_epreuve_equipe CreateVueEpreuveEquipe(Epreuve_Equipe ep, KernelImpl.IJudoData DC)
        {
            return new vue_epreuve_equipe(ep, DC);
        }

    }
}
