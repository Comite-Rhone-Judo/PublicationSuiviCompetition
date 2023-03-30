using System;
using System.Runtime.Serialization;

namespace Tools.CustomException
{
    /// <summary>
    /// ConnexionException : Exception liée aux transferts TCP
    /// </summary>
    [Serializable()]
    public class ConnexionException : System.Exception
    {
        public ConnexionException() : base() { }
        public ConnexionException(string message) : base(message) { }
        public ConnexionException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ConnexionException(SerializationInfo info, StreamingContext context) { }
    }
}
