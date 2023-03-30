using System;
using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Common
    {
        public static XDocument CreateDocument(ServerCommandEnum command)
        {
            XDocument doc = new XDocument();

            XElement rootElement = new XElement(ConstantXML.ServerJudo);            
            doc.Add(rootElement);

            XElement commandElement = new XElement(ConstantXML.Command, (int)command);
            rootElement.Add(commandElement);

            XElement commandValeur = new XElement(ConstantXML.Valeur);
            rootElement.Add(commandValeur);

            return doc;
        } 
    }
}
