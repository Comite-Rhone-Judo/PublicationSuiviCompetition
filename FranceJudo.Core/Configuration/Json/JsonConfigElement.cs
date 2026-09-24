using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace FranceJudo.Core.Configuration.Json
{
    public abstract class JsonConfigElement
    {
        // On ignore cette action lors de la sérialisation
        [JsonIgnore]
        public Action OnChanged { get; set; }

        /// <summary>
        /// Met à jour la valeur et notifie le service si un changement réel a lieu.
        /// </summary>
        protected bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnChanged?.Invoke();
            return true;
        }
    }
}