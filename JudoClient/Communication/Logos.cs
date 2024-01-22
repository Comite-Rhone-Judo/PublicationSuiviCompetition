using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Logos
    {
        public static void DemandeLogos(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandLogos);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }
    }
}
