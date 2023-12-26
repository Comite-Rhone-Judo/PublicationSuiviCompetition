using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Outils
{
    public class FileWithChecksumComparer : IEqualityComparer<FileWithChecksum>
    {
        public bool Equals(FileWithChecksum x, FileWithChecksum y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.File.FullName == y.File.FullName && x.Checksum == y.Checksum;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(FileWithChecksum p)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(p, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashFileName = p.File == null ? 0 : p.File.FullName.GetHashCode();

            //Get hash code for the Code field.
            int hashProductChecksum = p.Checksum.GetHashCode();

            //Calculate the hash code for the product.
            return hashFileName ^ hashProductChecksum;
        }
    }
}
