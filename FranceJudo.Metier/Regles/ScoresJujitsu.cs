using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace FranceJudo.Metier.Regles
{
    public class ScoresJujitsu
    {
        #region NE WAZA
        public int avantages1 { get; set; }

        public int avantages2 { get; set; }

        public int points2_1 { get; set; }

        public int points3_1 { get; set; }

        public int points4_1 { get; set; }

        public int points2_2 { get; set; }

        public int points3_2 { get; set; }

        public int points4_2 { get; set; }

        public int ippon1 { get; set; }

        public int ippon2 { get; set; }

        public int penalites1 { get; set; }

        public int penalites2 { get; set; }
        #endregion

        #region combat
        public int ippon1_1_1 { get; set; } //judoka 1 - partie 1 - ippon 1
        public int ippon1_1_2 { get; set; } //judoka 1 - partie 1 - ippon2
        public int ippon1_2_1 { get; set; } //judoka 1 - partie 2 - ippon 1
        public int ippon1_2_2 { get; set; } //judoka 1 - partie 2 - ippon2
        public int ippon1_3_1 { get; set; } //judoka 1 - partie 3 - ippon 1
        public int ippon1_3_2 { get; set; } //judoka 1 - partie 3 - ippon2
        public int ippon2_1_1 { get; set; } //judoka 2 - partie 1 - ippon 1
        public int ippon2_1_2 { get; set; } //judoka 2 - partie 1 - ippon2
        public int ippon2_2_1 { get; set; } //judoka 2 - partie 2 - ippon 1
        public int ippon2_2_2 { get; set; } //judoka 2 - partie 2 - ippon2
        public int ippon2_3_1 { get; set; } //judoka 2 - partie 3 - ippon 1
        public int ippon2_3_2 { get; set; } //judoka 2 - partie 3 - ippon2
        public int waza1 { get; set; }
        public int waza2 { get; set; }
        public int shido1 { get; set; }
        public int shido2 { get; set; }
        public int chui1 { get; set; }
        public int chui2 { get; set; }
        #endregion
        public int tempsMedical1 { get; set; }
        public int tempsMedical2 { get; set; }

        public bool IsFullIppon(int judoka)
        {
            bool res = false;

            if (judoka == 1)
            {
                if ((this.ippon1_1_1 > 0 || this.ippon1_1_2 > 0) && (this.ippon1_2_1 > 0 || this.ippon1_2_2 > 0) && (this.ippon1_3_1 > 0 || this.ippon1_3_2 > 0))
                {
                    res = true;
                }
            }
            else if (judoka == 2)
            {
                if ((this.ippon2_1_1 > 0 || this.ippon2_1_2 > 0) && (this.ippon2_2_1 > 0 || this.ippon2_2_2 > 0) && (this.ippon2_3_1 > 0 || this.ippon2_3_2 > 0))
                {
                    res = true;
                }
            }

            return res;
        }

        public int NbPartiesValides(int judoka)
        {
            int res = 0;
            if (judoka == 1)
            {
                if (this.ippon1_1_1 > 0 || this.ippon1_1_2 > 0)
                {
                    res++;
                }
                if (this.ippon1_2_1 > 0 || this.ippon1_2_2 > 0)
                {
                    res++;
                }
                if (this.ippon1_3_1 > 0 || this.ippon1_3_2 > 0)
                {
                    res++;
                }
            }
            else if (judoka == 2)
            {
                if (this.ippon2_1_1 > 0 || this.ippon2_1_2 > 0)
                {
                    res++;
                }
                if (this.ippon2_2_1 > 0 || this.ippon2_2_2 > 0)
                {
                    res++;
                }
                if (this.ippon2_3_1 > 0 || this.ippon2_3_2 > 0)
                {
                    res++;
                }
            }
            return res;
        }

        public int NbIppons(int judoka)
        {
            int res = 0;
            if (judoka == 1)
            {
                res += this.ippon1_1_1 + this.ippon1_1_2;
                res += this.ippon1_2_1 + this.ippon1_2_2;
                res += this.ippon1_3_1 + this.ippon1_3_2;
            }
            else if (judoka == 2)
            {
                res += this.ippon2_1_1 + this.ippon2_1_2;
                res += this.ippon2_2_1 + this.ippon2_2_2;
                res += this.ippon2_3_1 + this.ippon2_3_2;
            }
            return res;
        }

        public int GetPointsPenalitesCombat(int judoka)
        {
            int result = 0;

            if (judoka == 1)
            {
                result += (this.shido1 * 1) + (this.chui1 * 3);
            }
            else if (judoka == 2)
            {
                result += (this.shido2 * 1) + (this.chui2 * 3);
            }

            return result;
        }

        public static string ScoreJsonToString(ScoresJujitsu scores)
        {
            ScoresJujitsu scoresJson = new ScoresJujitsu();

            scoresJson.avantages1 = scores.avantages1;
            scoresJson.avantages2 = scores.avantages2;
            scoresJson.points2_1 = scores.points2_1;
            scoresJson.points3_1 = scores.points3_1;
            scoresJson.points4_1 = scores.points4_1;
            scoresJson.points2_2 = scores.points2_2;
            scoresJson.points3_2 = scores.points3_2;
            scoresJson.points4_2 = scores.points4_2;
            scoresJson.ippon1 = scores.ippon1;
            scoresJson.ippon2 = scores.ippon2;
            scoresJson.penalites1 = scores.penalites1;
            scoresJson.penalites2 = scores.penalites2;


            scoresJson.ippon1_1_1 = scores.ippon1_1_1;
            scoresJson.ippon1_1_2 = scores.ippon1_1_2;
            scoresJson.ippon1_2_1 = scores.ippon1_2_1;
            scoresJson.ippon1_2_2 = scores.ippon1_2_2;
            scoresJson.ippon1_3_1 = scores.ippon1_3_1;
            scoresJson.ippon1_3_2 = scores.ippon1_3_2;
            scoresJson.ippon2_1_1 = scores.ippon2_1_1;
            scoresJson.ippon2_1_2 = scores.ippon2_1_2;
            scoresJson.ippon2_2_1 = scores.ippon2_2_1;
            scoresJson.ippon2_2_2 = scores.ippon2_2_2;
            scoresJson.ippon2_3_1 = scores.ippon2_3_1;
            scoresJson.ippon2_3_2 = scores.ippon2_3_2;
            scoresJson.waza1 = scores.waza1;
            scoresJson.waza2 = scores.waza2;
            scoresJson.shido1 = scores.shido1;
            scoresJson.shido2 = scores.shido2;
            scoresJson.chui1 = scores.chui1;
            scoresJson.chui2 = scores.chui2;
            scoresJson.tempsMedical1 = scores.tempsMedical1;
            scoresJson.tempsMedical2 = scores.tempsMedical2;
            return Newtonsoft.Json.JsonConvert.SerializeObject(scoresJson);
        }

    }
}
