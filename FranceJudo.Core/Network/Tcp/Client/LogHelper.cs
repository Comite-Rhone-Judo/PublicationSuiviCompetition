using FranceJudo.Core.Logging;

namespace FranceJudo.Core.Network.Tcp.Client
{
    /// <summary>
    /// log helper
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// trace un message client
        /// </summary>
        /// <param name="mes"></param>
        public static void ShowLog(string mes)
        {
            LogTools.Logger?.Debug(mes);
        }
    }
}
