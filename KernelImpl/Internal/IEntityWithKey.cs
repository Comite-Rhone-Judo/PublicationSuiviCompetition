using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Internal
{
    internal interface IEntityWithKey<IDType>
    {
        // TODO Il faut changer Id ici car on a des collisions dans certaines classes metier (vue_judoka)
        IDType EntityKey {  get; }
    }
}
