using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Net
{
    /// <summary>
    /// Interface à implémenter par les modules qui ont besoin d'accéder aux données de l'application.
    /// </summary>
    public interface IContextAware
    {
        void SetContext(IContextProvider container);
    }
}
