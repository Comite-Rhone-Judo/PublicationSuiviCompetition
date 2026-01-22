using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Tools.Http
{
    /// <summary>
    /// Interface permettant de récupérer un contexte par son type.
    /// </summary>
    public interface IContextProvider
    {
        T GetContext<T>() where T : class;
    }
}
