using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tools.Outils
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

    }
}
