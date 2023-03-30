using AppPublication.Tools;
using JudoClient;
using JudoClient.Communication;
using KernelImpl.Noyau.Deroulement;
using System;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace AppPublication.Controles
{
    public static class GestionEvent
    {
        #region EVENT SERVER

        // La gestion des evenements se borne a actualiser les donnees de la competition en memoire
        // La generation du site est fait de maniere asynchrone par rapport a la reception des donnees

        public static void client_OnEndConnection(object sender)
        {
            if (DialogControleur.Instance.Connection.Client == (ClientJudo)sender)
            {
                DialogControleur.Instance.Connection.Client.Client.Stop();
                DialogControleur.Instance.Connection.Client = null;
            }
        }

        public static void clientjudo_OnAcceptConnectionCOM(object sender, XElement element)
        {
            DialogControleur.Instance.Connection.Client.DemandeStructures();
        }

        public static void client_OnListeStructures(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));
            FileTools.SaveFile(doc, ConstantFile.FileStructures);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (catégorie âge, poids) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesCategories;
            }));

            DialogControleur.Instance.ServerData.Structures.lecture_clubs(element);
            DialogControleur.Instance.ServerData.Structures.lecture_comites(element);
            DialogControleur.Instance.ServerData.Structures.lecture_secteurs(element);
            DialogControleur.Instance.ServerData.Structures.lecture_ligues(element);
            DialogControleur.Instance.ServerData.Structures.lecture_pays(element);

            DialogControleur.Instance.Connection.Client.DemandeCategories();
        }

        public static void client_OnUpdateStructures(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));
            FileTools.SaveFile(doc, ConstantFile.FileStructures);

            DialogControleur.Instance.ServerData.Structures.lecture_clubs(element);
            DialogControleur.Instance.ServerData.Structures.lecture_comites(element);
            DialogControleur.Instance.ServerData.Structures.lecture_secteurs(element);
            DialogControleur.Instance.ServerData.Structures.lecture_ligues(element);
            DialogControleur.Instance.ServerData.Structures.lecture_pays(element);
        }

        public static void client_OnListeCategories(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileCategories);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (logos) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesLogos;
            }));

            DialogControleur DC = DialogControleur.Instance;

            DC.ServerData.Categories.lecture_cateages(element);
            DC.ServerData.Categories.lecture_catepoids(element);
            DC.ServerData.Categories.lecture_ceintures(element);

            DialogControleur.Instance.Connection.Client.DemandeLogos();
        }

        public static void client_OnUpdateCategories(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));
            FileTools.SaveFile(doc, ConstantFile.FileCategories);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Categories.lecture_cateages(element);
            DC.ServerData.Categories.lecture_catepoids(element);
            DC.ServerData.Categories.lecture_ceintures(element);
        }

        public static void client_OnListeLogos(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileLogos);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (épreuves) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesEpreuves;
            }));

            try
            {
                DialogControleur.Instance.ServerData.Logos.lecture_logos(element);
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                DialogControleur.Instance.Connection.Client.DemandeOrganisation();
            }
        }

        public static void client_OnUpdateLogos(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileLogos);
            try
            {
                DialogControleur.Instance.ServerData.Logos.lecture_logos(element);
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
        }

        public static void client_OnListeOrganisation(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileOrganisation);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Organisation.lecture_competitions(element, DC.ServerData);
            DC.ServerData.Organisation.lecture_epreuves_equipe(element, DC.ServerData);
            DC.ServerData.Organisation.lecture_epreuves(element, DC.ServerData);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.InitialisationControl();
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (judokas) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesJudokas;
            }));

            if (DC.ServerData.competition.IsEquipe())
            {
                DialogControleur.Instance.Connection.Client.DemandeEquipes();
            }
            else
            {
                DialogControleur.Instance.Connection.Client.DemandeJudokas();
            }
        }

        public static void client_OnUpdateOrganisation(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileOrganisation);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Organisation.lecture_competitions(element, DC.ServerData);
            DC.ServerData.Organisation.lecture_epreuves_equipe(element, DC.ServerData);
            DC.ServerData.Organisation.lecture_epreuves(element, DC.ServerData);
        }

        public static void client_OnListeEquipes(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileEquipes);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (phases) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesPhases;
            }));

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Participants.lecture_epreuves_judokas(element, DC.ServerData);
            DC.ServerData.Participants.lecture_equipes(element);
            DC.ServerData.Participants.lecture_judokas(element, DC.ServerData);

            DialogControleur.Instance.Connection.Client.DemandePhases();
        }

        public static void client_OnUpdateEquipes(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileEquipes);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Participants.lecture_epreuves_judokas(element, DC.ServerData);
            DC.ServerData.Participants.lecture_equipes(element);
            DC.ServerData.Participants.lecture_judokas(element, DC.ServerData);
        }

        public static void client_OnListeJudokas(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileJudokas);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (phases) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesPhases;
            }));

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Participants.lecture_epreuves_judokas(element, DC.ServerData);
            DC.ServerData.Participants.lecture_judokas(element, DC.ServerData);

            DialogControleur.Instance.Connection.Client.DemandePhases();
        }

        public static void client_OnUpdateJudokas(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileJudokas);

            DialogControleur DC = DialogControleur.Instance;            
            DC.ServerData.Participants.lecture_epreuves_judokas(element, DC.ServerData);
            DC.ServerData.Participants.lecture_judokas(element, DC.ServerData);
        }


        public static void client_OnListePhases(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FilePhases);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (combats) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesCombats;
            }));

            DialogControleur DC = DialogControleur.Instance;

            DC.ServerData.Deroulement.clear_deroulement();
            DC.ServerData.Deroulement.lecture_phases(element);
            DC.ServerData.Deroulement.lecture_participants(element);
            DC.ServerData.Deroulement.lecture_decoupages(element);
            DC.ServerData.Deroulement.lecture_poules(element);
            DC.ServerData.Deroulement.lecture_groupes(element, DC.ServerData);

            DialogControleur.Instance.Connection.Client.DemandeCombats();
        }

        public static void client_OnUpdatePhases(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FilePhases);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Deroulement.clear_deroulement();
            DC.ServerData.Deroulement.lecture_phases(element);
            DC.ServerData.Deroulement.lecture_participants(element);
            DC.ServerData.Deroulement.lecture_decoupages(element);
            DC.ServerData.Deroulement.lecture_poules(element);
            DC.ServerData.Deroulement.lecture_groupes(element, DC.ServerData);          
        }

        public static void client_OnListeCombats(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileCombats);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.BusyContent = "Initialisation des données (arbitres) ...";
                DialogControleur.Instance.BusyStatus = Tools.Enum.BusyStatusEnum.InitDonneesArbitres;
            }));

            DialogControleur DC = DialogControleur.Instance;

            DC.ServerData.Deroulement.lecture_rencontres(element);
            DC.ServerData.Deroulement.lecture_feuilles(element);
            DC.ServerData.Deroulement.lecture_combats(element, DC.ServerData);

            DialogControleur.Instance.Connection.Client.DemandeArbitrage();
        }

        public static void client_OnUpdateCombats2(object sender, XElement element)
        {
            client_OnUpdateCombats(sender, element);
        }

        public static void client_OnUpdateCombats(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileCombats);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Deroulement.lecture_rencontres(element);
            DC.ServerData.Deroulement.lecture_feuilles(element);
            DC.ServerData.Deroulement.lecture_combats(element, DC.ServerData);     
        }

        public static void client_OnListeArbitrage(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileCombats);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Arbitrage.lecture_arbitres(element);
            DC.ServerData.Arbitrage.lecture_commissaires(element);
            DC.ServerData.Arbitrage.lecture_delegues(element);

            Application.Current.ExecOnUiThread(new Action(() =>
            {
                // DialogControleur.CS.RadBusyIndicator1.IsBusy = false;
                DialogControleur.Instance.IsBusy = false;
            }));
        }

        public static void client_OnUpdateArbitrage(object sender, XElement element)
        {
            XDocument doc = new XDocument();
            doc.Add(element);
            doc.Descendants(ConstantXML.Valeur).FirstOrDefault().SetAttributeValue(ConstantXML.Date, DateTime.Today.ToString("dd-MM-yyyy"));

            FileTools.SaveFile(doc, ConstantFile.FileCombats);

            DialogControleur DC = DialogControleur.Instance;
            DC.ServerData.Arbitrage.lecture_arbitres(element);
            DC.ServerData.Arbitrage.lecture_commissaires(element);
            DC.ServerData.Arbitrage.lecture_delegues(element);
        }

        #endregion
    }
}

