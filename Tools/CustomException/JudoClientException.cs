using System;
using System.Runtime.Serialization;

namespace Tools.CustomException
{
    /// <summary>
    /// ConnexionException : Exception liée aux transferts TCP
    /// </summary>
    [Serializable()]
    public class JudoClientException : System.Exception
    {
        public JudoClientException() : base() { }
        public JudoClientException(string message) : base(message) { }
        public JudoClientException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected JudoClientException(SerializationInfo info, StreamingContext context) { }
    }
}
