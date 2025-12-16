using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Connexion
    {
        public static void DemandConnectionPesee(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandConnectionPesee);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandConnectionCS(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandConnectionCS);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandConnectionCOM(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandConnectionCOM);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandConnectionTest(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandConnectionTest);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }
    }
}