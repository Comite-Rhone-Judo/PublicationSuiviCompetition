using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Organisation
    {
        public static void DemandeOrganisation(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandOrganisation);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeCompetitions(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandCompetitions);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeEpreuves(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandEpreuves);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeTapis(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandTapis);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultInscrition(this ClientJudo client, XElement xvaleur)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResultInscription);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(xvaleur);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }
    }
}
