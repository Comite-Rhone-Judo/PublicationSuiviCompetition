using FranceJudo.Metier.Noyau.Deroulement;
using System;
using System.Collections.Generic;

namespace AppPublication.ExtensionNoyau.Engagement
{
    public class CombatEqualityComparer : IEqualityComparer<ICombat>
    {
        /// <summary>
        /// Les combats sont egaux si ils ont le meme id
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ICombat x, ICombat y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            //Check whether the products' properties are equal.
            return x.id == y.id;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(ICombat j)
        {
            //Check whether the object is null
            if (j is null) return 0;

            //Calculate the hash code for the product.
            return j.id;
        }
    }
}