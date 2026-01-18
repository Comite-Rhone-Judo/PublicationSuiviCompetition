using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Arbitrage
    {
        public static void DemandeArbitrage(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandArbitrage);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandeArbitres(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandArbitres);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandeCommissaires(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandCommissaires);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandeDelegues(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandDelegues);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }
    }
}