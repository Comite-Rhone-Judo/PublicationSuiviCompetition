using System;
using System.Runtime.Serialization;

namespace Tools.CustomException
{
    /// <summary>
    /// WebServicesException : Exception liée aux différents appelle de webservice
    /// </summary>
    [Serializable()]
    public class WebServicesException : System.Exception
    {
        public WebServicesException() : base() { }
        public WebServicesException(string message) : base(message) { }
        public WebServicesException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected WebServicesException(SerializationInfo info, StreamingContext context) { }
    }
}
