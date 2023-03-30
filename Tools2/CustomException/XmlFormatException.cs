using System;
using System.Runtime.Serialization;

namespace Tools.CustomException
{
    /// <summary>
    /// XmlFormatException : Exception liée aux chargement des données (XML)
    /// </summary>
    [Serializable()]
    public class XmlFormatException : System.Exception
    {
        public XmlFormatException() : base() { }
        public XmlFormatException(string message) : base(message) { }
        public XmlFormatException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected XmlFormatException(SerializationInfo info, StreamingContext context) { }
    }
}
