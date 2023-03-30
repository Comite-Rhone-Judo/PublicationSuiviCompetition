using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using Tools.CustomException;
using Tools.Outils;

namespace Tools.TCP_Tools.Server
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
            LogTools.Log(new JudoServerException(ex.Message, ex));
        }
    }
}
