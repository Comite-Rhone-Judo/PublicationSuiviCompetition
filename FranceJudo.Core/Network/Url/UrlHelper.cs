using System.Globalization;
using System.IO;
using System.Text;

namespace FranceJudo.Core.Network.Url
{
    public static class UrlHelper
    {
        /// <summary>
        /// Traite une chaine pour quelle soit compatible avec une URL
        /// Il ne s'agit pas d'un traitement d'encodage mais de remplacer les symboles dans les categories, etc.
        /// '+' => 'p'
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string TraiteChaineURL(this string url)
        {
            return url.Replace("+", "p");
        }

        /// <summary>
        /// Traite une chaine pour la rendre compatible PATH
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>

        public static string TraiteChaine(this string chaine)
        {
            string result = chaine.Replace(" ", "_");

            char[] invalidPathChars = Path.GetInvalidFileNameChars();
            foreach (char invalid in invalidPathChars)
            {
                result = result.Replace(invalid, '_');
            }

            return result.ReplaceDiacritics();
        }

        /// <summary>
        /// Remplace les accents, etc.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string ReplaceDiacritics(this string source)
        {
            string sourceInFormD = source.Normalize(NormalizationForm.FormD);

            var output = new StringBuilder();
            foreach (char c in sourceInFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    output.Append(c);
            }

            return (output.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}
