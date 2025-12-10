using KernelImpl.Noyau.Deroulement;

namespace KernelManager
{
    public class ManagerDeroulement
    {
        public Combat CreateCombat()
        {
            return new Combat();
        }

        public Feuille CreateFeuille()
        {
            return new Feuille();
        }

        public Groupe_Combats CreateGroupeCombats()
        {
            return new Groupe_Combats();
        }

        public Phase CreatePhase()
        {
            return new Phase();
        }

        public Phase_Decoupage CreatePhaseDecoupage()
        {
            return new Phase_Decoupage();
        }

        public Poule CreatPoule()
        {
            return new Poule();
        }

        public Participant CreateParticipant()
        {
            return new Participant();
        }


        public Rencontre CreateRencontre()
        {
            return new Rencontre();
        }



        public vue_combat CreateVueCombat(Combat combat, KernelImpl.IJudoData DC)
        {
            return new vue_combat(combat, DC);
        }

        public vue_epreuve_phase CreateVueEpreuvePhase(Phase phase)
        {
            return new vue_epreuve_phase(phase);
        }

        public vue_groupe CreateVueGroupe(Groupe_Combats groupe, KernelImpl.IJudoData DC)
        {
            return new vue_groupe(groupe, DC);
        }
    }
}
