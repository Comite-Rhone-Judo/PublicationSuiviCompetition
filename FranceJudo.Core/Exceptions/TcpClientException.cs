using System;
using System.Runtime.Serialization;

namespace FranceJudo.Core.Exceptions
{
    /// <summary>
    /// ConnexionException : Exception liée aux transferts TCP
    /// </summary>
    [Serializable()]
    public class TcpClientException : System.Exception
    {
        public TcpClientException() : base() { }
        public TcpClientException(string message) : base(message) { }
        public TcpClientException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected TcpClientException(SerializationInfo info, StreamingContext context) { }
    }
}
