using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Net
{
    // Cette classe ne fait qu'une chose : gérer le sac de services.
    public class ContextProvider : IContextProvider
    {
        private readonly Dictionary<Type, object> _contexts = new Dictionary<Type, object>();

        public void Register<T>(T context) where T : class
        {
            //if (!_contexts.ContainsKey(typeof(T)))
            //{
            _contexts[typeof(T)] = context;
            //}
        }

        public T GetContext<T>() where T : class
        {
            if (_contexts.TryGetValue(typeof(T), out object context))
            {
                return (T)context;
            }
            return null;
        }
    }
}
