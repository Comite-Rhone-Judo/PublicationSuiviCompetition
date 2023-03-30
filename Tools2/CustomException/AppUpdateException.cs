using System;
using System.Runtime.Serialization;

namespace Tools.CustomException
{
    /// <summary>
    /// ConnexionException : Exception liée aux transferts TCP
    /// </summary>
    [Serializable()]
    public class AppUpdateException : System.Exception
    {
        public AppUpdateException() : base() { }
        public AppUpdateException(string message) : base(message) { }
        public AppUpdateException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected AppUpdateException(SerializationInfo info, StreamingContext context) { }
    }
}
