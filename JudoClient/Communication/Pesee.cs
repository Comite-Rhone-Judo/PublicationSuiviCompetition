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
    public static class Pesee
    {
        public static void DemandeListeEpreuves(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeEpreuves);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeListeClubs(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeClubs);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        //public static void DemandeListeComites(this ClientJudo client)
        //{
        //    XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeComites);

        //    string result = doc.ToString(SaveOptions.None);
        //    client.Client.Write(result);
        //}

        //public static void DemandeListeLigues(this ClientJudo client)
        //{
        //    XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeLigues);

        //    string result = doc.ToString(SaveOptions.None);
        //    client.Client.Write(result);
        //}

        public static void DemandeListeLogos(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeLogos);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeListeJudokas(this ClientJudo client, XElement epreuve)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandeListeJudokas);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(epreuve);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void SendResultInscrition(this ClientJudo client, XElement xvaleur)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.InscriptionJudoka);
            doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(xvaleur);
            //doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(xepreuve);
            //doc.Element(ConstantXML.ServerJudo).Element(ConstantXML.Valeur).Add(xclub);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }
    }
}
