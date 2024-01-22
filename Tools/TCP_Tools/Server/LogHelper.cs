using System.Net.Sockets;

namespace Tools.TCP_Tools.Server
{
    /// <summary>
    /// log helper
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// Type de trace
        /// </summary>
        public enum TypeLog
        {
            Connect = 1,
            ReceiveData = 2,
            RemoteClose = 3,
            Close = 4,
            ClientClose = 5,
            SentData = 6
        }

        /// <summary>
        /// Trace une info server
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="client"></param>
        /// <param name="type"></param>
        public static void ShowLog(string mes, TcpClient client, TypeLog type)
        {
            string mess = "";
            switch (type)
            {
                case TypeLog.ClientClose:
                    mess += "Client Close\r\n";
                    break;
                case TypeLog.ReceiveData:
                    mess += "Receive ";
                    break;
                case TypeLog.SentData:
                    mess += "Sent ";
                    break;
                case TypeLog.RemoteClose:
                    mess += "Remote Close\r\n";
                    break;
                case TypeLog.Close:
                    mess += "Close\r\n";
                    break;
                case TypeLog.Connect:
                    mess += "Receive connect\r\n";
                    break;
            }

            mess += mes;

            //if(type == TypeLog.ReceiveData || type == TypeLog.SentData)
            //{
            //    LogTools.Trace(mess, LogTools.Level.INFO);
            //}

            //LogTools.Debug(mess);
        }
    }
}
