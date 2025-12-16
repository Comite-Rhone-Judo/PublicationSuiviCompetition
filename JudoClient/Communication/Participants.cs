using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Participants
    {
        public static void DemandeEquipes(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandEquipes);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandeJudokas(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandJudokas);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }

        public static void DemandeLicencies(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandLicencies);

            string result = doc.ToString(SaveOptions.None);
            client.NetworkClient.Write(result);
        }
    }
}
