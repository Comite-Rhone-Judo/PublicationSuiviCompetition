using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Deroulement
    {
        public static void DemandePhases(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandPhases);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeCombats(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandCombats);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }


        public static void SendChoixTapis(this ClientJudo client, int numero)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResultTapis);

            XElement tapis = new XElement(ConstantXML.Tapis, numero);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(tapis);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        
        public static void SendResultCombat(this ClientJudo client, XElement resultcombat)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResultCombats);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(resultcombat);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultRencontre(this ClientJudo client, XElement resultrencontre)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResultRencontres);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(resultrencontre);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultsCombats(this ClientJudo client, IList<XElement> resultcombat)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResultCombats);
            foreach (XElement element in resultcombat)
            {
                doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(element);
            }

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultsRencontres(this ClientJudo client, IList<XElement> resultrencontres)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResultRencontres);
            foreach (XElement element in resultrencontres)
            {
                doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(element);
            }

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

    }
}
