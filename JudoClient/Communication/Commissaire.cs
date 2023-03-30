using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using Tools;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Commissaire
    {      

        public static void DemandeListeTapis(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeTapis);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeListeCombats(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeCombats);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendChoixTapis(this ClientJudo client, int numero)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ChoixTapis);

            XElement tapis = new XElement(ConstantXML.Tapis, numero);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(tapis);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        

        public static void SendResultCombat(this ClientJudo client, XElement resultcombat)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResutatsCombats);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(resultcombat);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultRencontre(this ClientJudo client, XElement resultrencontre)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResutatsRencontres);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(resultrencontre);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultsCombats(this ClientJudo client, IList<XElement> resultcombat)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResutatsCombats);
            foreach (XElement element in resultcombat)
            {
                doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(element);
            }

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultsRencontres(this ClientJudo client, IList<XElement> resultrencontres)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.ResutatsRencontres);
            foreach (XElement element in resultrencontres)
            {
                doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(element);
            }

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }
    }
}
