using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau
{
    internal interface IIdEntity<IDType>
    {
        IDType id {  get; }
    }
}
