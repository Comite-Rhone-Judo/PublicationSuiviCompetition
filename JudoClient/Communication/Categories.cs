using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public static class Categories
    {
        public static void DemandeCategories(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandCategories);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeCateAge(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandCateAge);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeCatePoids(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandCatePoids);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }

        public static void DemandeCeintures(this ClientJudo client)
        {
            XDocument doc = Common.CreateDocument(ServerCommandEnum.DemandGrade);

            string result = doc.ToString(SaveOptions.None);
            client.Client.Write(result);
        }        
    }
}
