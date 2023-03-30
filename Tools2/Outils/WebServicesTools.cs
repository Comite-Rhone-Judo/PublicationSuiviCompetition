using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Tools.CustomException;
using Tools.Enum;

namespace Tools.Outils
{
    public static class WebServicesTools
    {       

        /// <summary>
        /// Chargement des compétitions à partir d'un compte extranet (triplet login, mdp, structure à laquelle il est affilié)
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="mdp">password</param>
        /// <param name="structure">structure (Ligue ou comité)</param>
        /// <returns></returns>

        public static XmlDocument GetCompetitions(string login, string mdp, string structure)
        {
            XmlDocument doc = new XmlDocument();

            string events = "";
            try
            {
                events = service.listeEventsCRITT(login, mdp, structure);
            }
            catch (Exception ex)
            {
                events = ConstantXML.Event_erreur_2;
                LogTools.Trace(ex, LogTools.Level.WARN);
            }

            if (!WebServicesTools.TraiteErreur(events))
            {
                return doc;
            }

            try
            {
                doc.LoadXml(events);
            }
            catch (Exception ex)
            {
                events = ConstantXML.Event_erreur_3;
                LogTools.Trace(ex, LogTools.Level.ERROR);
            }

            WebServicesTools.TraiteErreur(events);

            return doc;
        }

        /// <summary>
        /// Chargement du fichier compétition
        /// </summary>
        /// <param name="event_id">id de la compétition</param>
        /// <returns></returns>

        public static string GetInscription(int event_id)
        {
            XmlDocument doc = new XmlDocument();

            string events = "";
            try
            {
                events = service.listeInscriptionsCRITT(event_id);
            }
            catch (Exception ex)
            {
                events = ConstantXML.Event_erreur_2;
                LogTools.Trace(ex, LogTools.Level.WARN);
            }

            if (!WebServicesTools.TraiteErreur(events))
            {
                return "";
            }

            try
            {
                doc.LoadXml(events);
            }
            catch (Exception ex)
            {
                events = ConstantXML.Event_erreur_3;
                LogTools.Trace(ex, LogTools.Level.ERROR);
            }

            if (!WebServicesTools.TraiteErreur(events))
            {
                return "";
            }

            doc.Save(ConstantFile.Extra_InscriptionFile);
            return ConstantFile.Extra_InscriptionFile;
        }


        /// <summary>
        /// Liste complète des clubs de la fédération
        /// </summary>
        /// <returns></returns>

        public static void ListeClubs()
        {
            XmlDocument doc = new XmlDocument();

            string clubs = "";
            try
            {
                clubs = service.listeClubsCRITT();
            }
            catch (Exception ex)
            {
                clubs = ConstantXML.Event_erreur_2;
                LogTools.Trace(ex, LogTools.Level.WARN);
            }

            if (!WebServicesTools.TraiteErreur(clubs))
            {
                return;
            }

            try
            {
                doc.LoadXml(clubs);
            }
            catch (Exception ex)
            {
                clubs = ConstantXML.Event_erreur_3;
                LogTools.Trace(ex, LogTools.Level.ERROR);
            }

            WebServicesTools.TraiteErreur(clubs);

            XmlAttribute dateAtt = doc.CreateAttribute("date");
            dateAtt.Value = DateTime.Now.ToString("u");

            doc.DocumentElement.Attributes.Append(dateAtt);
            doc.Save(ConstantFile.Extra_ClubsFile);
        }

        /// <summary>
        /// Liste des licenciés d'un OTD
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="mdp">mot de passe</param>
        /// <param name="structure">structure</param>
        /// <param name="comite">comité à charger</param>

        public static XmlDocument ListeLicencies(string login, string mdp, string structure, string comite)
        {
            XmlDocument doc = new XmlDocument();

            string judokas = "";
            try
            {
                judokas = service.listePopulationCRITT(login, mdp, structure, comite);
            }
            catch (Exception ex)
            {
                judokas = ConstantXML.Event_erreur_2;
                LogTools.Trace(ex, LogTools.Level.WARN);
            }

            if (!WebServicesTools.TraiteErreur(judokas))
            {
                return null;
            }

            try
            {
                doc.LoadXml(judokas);
            }
            catch (Exception ex)
            {
                judokas = ConstantXML.Event_erreur_3;
                LogTools.Trace(ex, LogTools.Level.ERROR);
                return null;
            }

            WebServicesTools.TraiteErreur(judokas);

            return doc;
        }

        /// <summary>
        /// Exporte un fichier résultat d'une compétition
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="mdp">mot de passe</param>
        /// <param name="structure">structure</param>
        /// <param name="competiton">les résultat</param>

        public static void ExportToExtranet(string login, string mdp, string structure, XmlDocument competiton)
        {
            string judokas = "";
            try
            {
                judokas = service.majResultatCRITT(login, mdp, structure, competiton.InnerXml);
                judokas = service.majResultatFFJDA(login, mdp, structure, competiton.InnerXml);
            }
            catch (Exception ex)
            {
                judokas = ConstantXML.Event_erreur_2;
                LogTools.Trace(ex, LogTools.Level.WARN);
            }

            WebServicesTools.TraiteErreur(judokas);
        }

        /// <summary>
        /// Exporte un fichier résultat d'une compétition
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="mdp">mot de passe</param>
        /// <param name="structure">structure</param>
        /// <param name="competiton">les résultat</param>

        public static void ExportTest(string login, string mdp, string structure, XmlDocument competiton)
        {
            string judokas = "";
            try
            {
                judokas = service.test(competiton.InnerXml);
            }
            catch (Exception ex)
            {
                judokas = ConstantXML.Event_erreur_2;
                LogTools.Trace(ex, LogTools.Level.WARN);
            }

            WebServicesTools.TraiteErreur(judokas);
        }


        private static FFJudoServices.WebService1SoapClient service = new FFJudoServices.WebService1SoapClient(WebServicesTools.GetBinding(), WebServicesTools.GetRemoteAdress());

        private static EndpointAddress GetRemoteAdress()
        {
            Uri baseAddress = new Uri("http://www.ffjda.org/ws_Tas/WebService1.asmx");
            EndpointAddress remote = new EndpointAddress(baseAddress);
            return remote;
        }

        private static Binding GetBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            binding.Name = "WebService1Soap";
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.Security.Mode = BasicHttpSecurityMode.None;

            binding.MaxBufferSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.SendTimeout = TimeSpan.FromMinutes(15);

            return binding;
        }

        private static bool TraiteErreur(string events)
        {
            if (events == ConstantXML.Event_erreur_0)
            {
                LogTools.Log(new WebServicesException("Le compte utilisateur n\'existe pas (Login ou mot de passe incorrect).\nVeuiller réessayer."), LogTools.Level.INFO);
                return false;
            }

            if (events == ConstantXML.Event_erreur_1)
            {
                LogTools.Log(new WebServicesException("Vous n\'avez pas de droit sur la structure sélectionnée.\nVeuiller réessayer ultérieurement."), LogTools.Level.INFO);
                return false;
            }

            if (events == ConstantXML.Event_erreur_2)
            {
                LogTools.Log(new WebServicesException("Impossible de se connecter au web-services.\nVeuiller vérifier votre connexion internet."), LogTools.Level.INFO);
                return false;
            }

            if (events == ConstantXML.Event_erreur_3)
            {
                LogTools.Log(new ConnexionException("Problème lors du chargement du fichier."), LogTools.Level.INFO);
                return false;
            }

            return true;
        }
    }
}
