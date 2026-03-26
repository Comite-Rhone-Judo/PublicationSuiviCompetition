using System;
using FranceJudo.Core.Exceptions;
using FranceJudo.Core.Logging;

namespace FranceJudo.Core.Network.Tcp.Server
{
    /// <summary>
    /// Exception Helper
    /// </summary>
    public class ExceptionHelper
    {
        /// <summary>
        /// Trace une exception
        /// </summary>
        /// <param name="ex"></param>
        public static void ShowException(Exception ex)
        {
            LogTools.Error(new ServerException(ex.Message, ex));
        }
    }
}
