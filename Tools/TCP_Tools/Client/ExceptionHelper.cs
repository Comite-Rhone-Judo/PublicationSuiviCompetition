using System;
using Tools.CustomException;
using Tools.Outils;

namespace Tools.TCP_Tools.Client
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
            LogTools.Error(new JudoClientException(ex.Message, ex));
        }
    }
}
