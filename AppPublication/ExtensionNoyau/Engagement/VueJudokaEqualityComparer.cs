using System;
using System.Collections.Generic;
using KernelImpl.Noyau.Participants;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;


namespace AppPublication.ExtensionNoyau.Engagement
{

    // Comparateur de judokas pour trier les listes de judokas
    class VueJudokaEqualityComparer : IEqualityComparer<vue_judoka>
    {
        /// <summary>
        /// Les judokas sont egaux si nom et prenom sont egaux
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(vue_judoka x, vue_judoka y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.nom == y.nom && x.prenom == y.prenom;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(vue_judoka j)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(j, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashNom = j.nom == null ? 0 : j.nom.GetHashCode();
            int hashPrenom = j.prenom == null ? 0 : j.prenom.GetHashCode();

            //Calculate the hash code for the product.
            return hashNom ^ hashPrenom;
        }
    }
}