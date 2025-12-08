using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau
{
    internal interface IIdEntity<IDType>
    {
        // TODO Il faut changer Id ici car on a des collisions dans certaines classes metier (vue_judoka)
        IDType id {  get; }
    }
}
