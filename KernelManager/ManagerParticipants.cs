using KernelImpl.Noyau.Participants;


namespace KernelManager
{
    public class ManagerParticipants
    {
        public EpreuveJudoka CreateEpreuveJudoka()
        {
            return new EpreuveJudoka();
        }

        public Equipe CreateEquipe()
        {
            return new Equipe();
        }

        public Judoka CreateJudoka()
        {
            return new Judoka();
        }

        
        public vue_judoka CreateVueJudoka(Judoka judoka, KernelImpl.JudoData DC)
        {
            return new vue_judoka(judoka, DC);
        }

    }
}
