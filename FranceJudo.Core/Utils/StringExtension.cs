using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FranceJudo.Core.Utils
{
    public static class StringExtension
    {
        #region MEMBRES
        private static Regex _regexLicence = new Regex("[F|M][0-3][0-9][0-1][0-9][1|2][0-9]{3}[A-Z|*|-]{5}[0-9]{2}");
        #endregion

        #region EXTENSIONS
        public static string SafeSubstring(this string valeur, int start, int longueur)
        {
            // Sécurité de base
            if (string.IsNullOrEmpty(valeur) || start >= valeur.Length || start < 0)
                return string.Empty;

            // Math.Min permet de prendre soit la longueur demandée, soit ce qu'il reste, sans faire de if/else
            int longueurReelle = Math.Min(longueur, valeur.Length - start);

            return valeur.Substring(start, longueurReelle);
        }

        /// <summary>
        /// Traite une chaine prénom
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>

        public static string FormatPrenom(this string chaine)
        {
            string result = "";
            bool maj = true;
            foreach (char ch in chaine.ToList())
            {
                result += (maj ? Char.ToUpper(ch) : Char.ToLower(ch));
                if (ch == '-' || ch == ' ')
                {
                    maj = true;
                }
                else
                {
                    maj = false;
                }
            }

            return result.Trim();
        }

        /// <summary>
        /// Traite le texte reçu par la douchette
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ScanneTraiteLicence(this string text)
        {
            string chaine1 = text;

            chaine1 = chaine1.Replace('à', '0');
            chaine1 = chaine1.Replace('&', '1');
            chaine1 = chaine1.Replace('é', '2');
            chaine1 = chaine1.Replace('\"', '3');
            chaine1 = chaine1.Replace('\'', '4');
            chaine1 = chaine1.Replace('(', '5');
            chaine1 = chaine1.Replace('-', '6');
            chaine1 = chaine1.Replace('è', '7');
            chaine1 = chaine1.Replace('_', '8');
            chaine1 = chaine1.Replace('ç', '9');


            if (_regexLicence.IsMatch(chaine1) && !_regexLicence.IsMatch(text))
            {
                return chaine1;
            }
            else
            {
                return text;
            }
        }

        #endregion
    }
}
