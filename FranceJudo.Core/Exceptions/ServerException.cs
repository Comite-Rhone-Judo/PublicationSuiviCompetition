using System;
using System.Runtime.Serialization;

namespace FranceJudo.Core.Exceptions
{
    /// <summary>
    /// JudoServerException : Exception liée aux transferts TCP
    /// </summary>
    [Serializable()]
    public class ServerException : System.Exception
    {
        public ServerException() : base() { }
        public ServerException(string message) : base(message) { }
        public ServerException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ServerException(SerializationInfo info, StreamingContext context) { }
    }
}
