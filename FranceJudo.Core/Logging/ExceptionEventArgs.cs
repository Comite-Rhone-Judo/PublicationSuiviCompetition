using System;

namespace FranceJudo.Core.Logging
{
    /// <summary>
    /// Contient les informations relatives à une erreur critique remontée par le moteur de log.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public string Message { get; private set; }

        public ExceptionEventArgs(Exception ex, string message)
        {
            Exception = ex;
            Message = message;
        }
    }
}