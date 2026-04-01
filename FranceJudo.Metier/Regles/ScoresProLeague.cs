using FranceJudo.Metier.Noyau.Deroulement;

namespace FranceJudo.Metier.Regles
{
    public class ScoresProLeague
    {
        /*
 * 20 points max par combat
 * waza ari = 1 point
 * ippon ou 2 waza ari = 10 points
 * hansoku make = 20 points
 * ex: 
 * 1 ippon + 1 waza ari = 11 points
 * 1 ippon + 2 waza ari = 20 points
 * 3 waza ari = 11 points
 * 2 shidos à 1 = match nul
 */
        public static System.Collections.Generic.List<int> getScoresProLeague(int waza1, int ippon1, int yuko1, int shido1, EtatCombattantEnum etatCombattant1,
            int waza2, int ippon2, int yuko2, int shido2, EtatCombattantEnum etatCombattant2)
        {
            System.Collections.Generic.List<int> res = new System.Collections.Generic.List<int>();
            //int res = 0;

            int score1 = getScoreProLeague(waza1, ippon1, yuko1);
            int score2 = getScoreProLeague(waza2, ippon2, yuko2);

            if (etatCombattant1 == EtatCombattantEnum.HansokuMakeX || etatCombattant1 == EtatCombattantEnum.HansokuMakeH || etatCombattant1 == EtatCombattantEnum.Forfait
                || etatCombattant1 == EtatCombattantEnum.Abandon || etatCombattant1 == EtatCombattantEnum.Medical)
            {
                score2 += 20;
            }
            if (etatCombattant2 == EtatCombattantEnum.HansokuMakeX || etatCombattant2 == EtatCombattantEnum.HansokuMakeH || etatCombattant2 == EtatCombattantEnum.Forfait
                || etatCombattant2 == EtatCombattantEnum.Abandon || etatCombattant2 == EtatCombattantEnum.Medical)
            {
                score1 += 20;
            }

            if (shido1 >= 3)
            {
                score2 += 20;
            }
            if (shido2 >= 3)
            {
                score1 += 20;
            }

            if (score1 > 20) score1 = 20;
            if (score2 > 20) score2 = 20;

            res.Add(score1);
            res.Add(score2);

            return res;
        }
        private static int getScoreProLeague(int waza, int ippon, int yuko)
        {
            int y = yuko;
            int res = 0;
            int scoreWaza = 0;
            if (waza == 1)
            {
                scoreWaza = 1;
            }
            else if (waza == 2)
            {
                scoreWaza = 10;
            }
            else if (waza == 3)
            {
                scoreWaza = 11;
            }
            if (waza > 3)
            {
                scoreWaza = 20;
            }

            res = scoreWaza + (ippon * 10);
            //res = scoreWaza + (ippon * 10) + (etatCombattant == EtatCombattantEnum.HansokuMakeX ? 20 : 0);

            //if (res > 20) res = 20;

            return res;
        }
    }
}
