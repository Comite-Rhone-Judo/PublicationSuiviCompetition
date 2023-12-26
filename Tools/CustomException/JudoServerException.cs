using System;
using System.Runtime.Serialization;

namespace Tools.CustomException
{
    /// <summary>
    /// JudoServerException : Exception liée aux transferts TCP
    /// </summary>
    [Serializable()]
    public class JudoServerException : System.Exception
    {
        public JudoServerException() : base() { }
        public JudoServerException(string message) : base(message) { }
        public JudoServerException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected JudoServerException(SerializationInfo info, StreamingContext context) { }
    }
}
