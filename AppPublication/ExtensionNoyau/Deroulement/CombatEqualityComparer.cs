using KernelImpl.Noyau.Deroulement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.ExtensionNoyau.Deroulement
{
    public class CombatEqualityComparer : IEqualityComparer<Combat>
    {
        /// <summary>
        /// Les combats sont egaux si ils ont le meme id
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(Combat x, Combat y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.id == y.id;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Combat j)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(j, null)) return 0;

            //Calculate the hash code for the product.
            return j.id;
        }
    }
}