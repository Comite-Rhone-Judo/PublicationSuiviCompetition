using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Internal
{
    internal interface IEntityWithKey<IDType>
    {
        IDType EntityKey {  get; }
    }
}
