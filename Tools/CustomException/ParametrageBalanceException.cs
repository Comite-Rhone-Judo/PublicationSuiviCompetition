using System;

namespace Tools.CustomException
{
    /// <summary>
    /// ParametrageBalanceException : Exception liée aux paramétrage de la balance
    /// </summary>
    [Serializable()]
    public class ParametrageBalanceException : System.Exception
    {
        public ParametrageBalanceException() : base() { }
        public ParametrageBalanceException(string message) : base(message) { }
        public ParametrageBalanceException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ParametrageBalanceException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
