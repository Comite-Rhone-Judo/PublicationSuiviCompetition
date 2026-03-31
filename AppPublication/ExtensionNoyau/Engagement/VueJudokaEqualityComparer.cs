using FranceJudo.Metier.Noyau.Participants;
using System;
using System.Collections.Generic;

namespace AppPublication.ExtensionNoyau.Engagement
{

    // Comparateur de judokas pour trier les listes de judokas
    class VueJudokaEqualityComparer : IEqualityComparer<Ivue_judoka>
    {
        /// <summary>
        /// Les judokas sont egaux si nom et prenom sont egaux
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(Ivue_judoka x, Ivue_judoka y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            //Check whether the products' properties are equal.
            return x.nom == y.nom && x.prenom == y.prenom;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Ivue_judoka j)
        {
            //Check whether the object is null
            if (j is null) return 0;

            //Get hash code for the Name field if it is not null.
            int hashNom = j.nom == null ? 0 : j.nom.GetHashCode();
            int hashPrenom = j.prenom == null ? 0 : j.prenom.GetHashCode();

            //Calculate the hash code for the product.
            return hashNom ^ hashPrenom;
        }
    }
}