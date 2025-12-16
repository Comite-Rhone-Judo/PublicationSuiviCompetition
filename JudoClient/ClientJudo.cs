using JudoClient.Communication;
using System;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;
using Tools.TCP_Tools.Client;

namespace JudoClient
{
    public delegate void OnEndConnectionHandler(object sender);

    public class ClientJudo
    {
        ClientGenerique _client = null;

        public event OnEndConnectionHandler OnEndConnection;

        public ClientGenerique NetworkClient { get { return _client; } }


        private TraitementArbitrage _traitement_arb = null;
        public TraitementArbitrage TraitementArbitrage { get { return _traitement_arb; } }

        private TraitementCategories _traitement_cate = null;
        public TraitementCategories TraitementCategories { get { return _traitement_cate; } }

        private TraitementConnexion _traitement_con = null;
        public TraitementConnexion TraitementConnexion { get { return _traitement_con; } }

        private TraitementDeroulement _traitement_der = null;
        public TraitementDeroulement TraitementDeroulement { get { return _traitement_der; } }

        private TraitementOrganisation _traitement_org = null;
        public TraitementOrganisation TraitementOrganisation { get { return _traitement_org; } }

        private TraitementParticipants _traitement_part = null;
        public TraitementParticipants TraitementParticipants { get { return _traitement_part; } }

        private TraitementStructure _traitement_struc = null;
        public TraitementStructure TraitementStructure { get { return _traitement_struc; } }

        private TraitementLogos _traitement_log = null;
        public TraitementLogos TraitementLogos { get { return _traitement_log; } }




        public bool IsConnected
        {
            get
            {
                try
                {
                    return _client.IsConnected;
                }
                catch
                {
                    return false;
                }
            }
        }

        public ClientJudo(string ip, int port)
        {
            _traitement_arb = new TraitementArbitrage(this);
            _traitement_cate = new TraitementCategories(this);
            _traitement_con = new TraitementConnexion(this);
            _traitement_der = new TraitementDeroulement(this);
            _traitement_log = new TraitementLogos(this);
            _traitement_org = new TraitementOrganisation(this);
            _traitement_part = new TraitementParticipants(this);
            _traitement_struc = new TraitementStructure(this);

            _client = new ClientGenerique(ip, port);

            _client.OnConnection += client_OnConnection;
            _client.OnDataRecieve += client_OnDataRecieve;
            _client.OnDataSent += client_OnDataSent;
            _client.OnEndConnection += client_OnEndConnection;

            _client.Connect();
        }

        void client_OnConnection(object sender)
        {

        }

        void client_OnEndConnection(object sender)
        {
            if (OnEndConnection != null)
            {
                OnEndConnection(this);
            }
        }

        void client_OnDataSent(object sender)
        {
            //SaveToLog("message envoyé");
        }

        void client_OnDataRecieve(object sender, string donnees)
        {
            try
            {
                XDocument doc = XDocument.Parse(donnees);
                if (doc.Element(ConstantXML.ServerJudo) != null)
                {
                    XElement serverJudo = doc.Element(ConstantXML.ServerJudo);
                    XElement xvaleur = serverJudo.Element(ConstantXML.Valeur);
                    XElement commandElement = serverJudo.Element(ConstantXML.Command);

                    ServerCommandEnum command = (ServerCommandEnum)int.Parse(commandElement.Value);

                    switch (command)
                    {
                        #region CONNECTION

                        case ServerCommandEnum.DemandConnectionPesee:
                            break;
                        case ServerCommandEnum.AcceptConnectionPesee:
                            _traitement_con.AcceptConnectionPesee(xvaleur);
                            break;

                        case ServerCommandEnum.DemandConnectionCS:
                            break;
                        case ServerCommandEnum.AcceptConnectionCS:
                            _traitement_con.AcceptConnectionCS(xvaleur);
                            break;

                        case ServerCommandEnum.DemandConnectionCOM:
                            break;
                        case ServerCommandEnum.AcceptConnectionCOM:
                            _traitement_con.AcceptConnectionCOM(xvaleur);
                            break;

                        case ServerCommandEnum.DemandConnectionTest:
                            break;
                        case ServerCommandEnum.AcceptConnectionTest:
                            _traitement_con.AcceptConnectionTest(xvaleur);
                            break;

                        #endregion

                        #region ARBITRAGE

                        case ServerCommandEnum.DemandArbitrage:
                            break;
                        case ServerCommandEnum.EnvoieArbitrage:
                            _traitement_arb.ListeArbitrage(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateArbitrage:
                            _traitement_arb.UpdateArbitrage(xvaleur);
                            break;

                        case ServerCommandEnum.DemandCommissaires:
                            break;
                        case ServerCommandEnum.EnvoieCommissaires:
                            _traitement_arb.ListeCommissaires(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateCommissaires:
                            _traitement_arb.UpdateCommissaires(xvaleur);
                            break;

                        case ServerCommandEnum.DemandArbitres:
                            break;
                        case ServerCommandEnum.EnvoieArbitres:
                            _traitement_arb.ListeArbitres(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateArbitres:
                            _traitement_arb.UpdateArbitres(xvaleur);
                            break;

                        case ServerCommandEnum.DemandDelegues:
                            break;
                        case ServerCommandEnum.EnvoieDelegues:
                            _traitement_arb.ListeDelegues(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateDelegues:
                            _traitement_arb.UpdateDelegues(xvaleur);
                            break;

                        #endregion

                        #region CATES

                        case ServerCommandEnum.DemandCategories:
                            break;
                        case ServerCommandEnum.EnvoieCategories:
                            _traitement_cate.ListeCategories(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateCategories:
                            _traitement_cate.UpdateCategories(xvaleur);
                            break;

                        case ServerCommandEnum.DemandCatePoids:
                            break;
                        case ServerCommandEnum.EnvoieCatePoids:
                            _traitement_cate.ListeCatePoids(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateCatePoids:
                            _traitement_cate.UpdateCatePoids(xvaleur);
                            break;

                        case ServerCommandEnum.DemandGrade:
                            break;
                        case ServerCommandEnum.EnvoieGrade:
                            _traitement_cate.ListeCeintures(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateGrade:
                            _traitement_cate.UpdateCeintures(xvaleur);
                            break;

                        case ServerCommandEnum.DemandCateAge:
                            break;
                        case ServerCommandEnum.EnvoieCateAge:
                            _traitement_cate.ListeCateAge(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateCateAge:
                            _traitement_cate.UpdateCateAge(xvaleur);
                            break;

                        #endregion

                        #region STRUCTURE

                        case ServerCommandEnum.DemandStructures:
                            break;
                        case ServerCommandEnum.EnvoieStructures:
                            _traitement_struc.ListeStructures(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateStructures:
                            _traitement_struc.UpdateStructures(xvaleur);
                            break;

                        case ServerCommandEnum.DemandPays:
                            break;
                        case ServerCommandEnum.EnvoiePays:
                            _traitement_struc.ListePays(xvaleur);
                            break;
                        case ServerCommandEnum.UpdatePays:
                            _traitement_struc.UpdatePays(xvaleur);
                            break;

                        case ServerCommandEnum.DemandLigues:
                            break;
                        case ServerCommandEnum.EnvoieLigues:
                            _traitement_struc.ListeLigues(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateLigues:
                            _traitement_struc.UpdateLigues(xvaleur);
                            break;

                        case ServerCommandEnum.DemandSecteurs:
                            break;
                        case ServerCommandEnum.EnvoieSecteurs:
                            _traitement_struc.ListeLigues(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateSecteurs:
                            _traitement_struc.UpdateSecteurs(xvaleur);
                            break;

                        case ServerCommandEnum.DemandComites:
                            break;
                        case ServerCommandEnum.EnvoieComites:
                            _traitement_struc.ListeComites(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateComites:
                            _traitement_struc.UpdateComites(xvaleur);
                            break;

                        case ServerCommandEnum.DemandClubs:
                            break;
                        case ServerCommandEnum.EnvoieClubs:
                            _traitement_struc.ListeClubs(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateClubs:
                            _traitement_struc.UpdateClubs(xvaleur);
                            break;

                        #endregion

                        #region LOGO

                        case ServerCommandEnum.DemandLogos:
                            break;
                        case ServerCommandEnum.EnvoieLogos:
                            _traitement_log.ListeLogos(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateLogos:
                            _traitement_log.UpdateLogos(xvaleur);
                            break;

                        #endregion

                        #region ORGANISATION

                        case ServerCommandEnum.DemandOrganisation:
                            break;
                        case ServerCommandEnum.EnvoieOrganisation:
                            _traitement_org.ListeOrganisation(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateOrganisation:
                            _traitement_org.UpdateOrganisation(xvaleur);
                            break;

                        case ServerCommandEnum.DemandCompetitions:
                            break;
                        case ServerCommandEnum.EnvoieCompetitions:
                            _traitement_org.ListeCompetitions(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateCompetitions:
                            _traitement_org.UpdateCompetitions(xvaleur);
                            break;

                        case ServerCommandEnum.DemandEpreuves:
                            break;
                        case ServerCommandEnum.EnvoieEpreuves:
                            _traitement_org.ListeEpreuves(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateEpreuves:
                            _traitement_org.UpdateEpreuves(xvaleur);
                            break;

                        case ServerCommandEnum.DemandTapis:
                            break;
                        case ServerCommandEnum.EnvoieTapis:
                            _traitement_org.ListeTapis(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateTapis:
                            _traitement_org.UpdateTapis(xvaleur);
                            break;

                        #endregion

                        #region PARTICIPANTS

                        case ServerCommandEnum.DemandEquipes:
                            break;
                        case ServerCommandEnum.EnvoieEquipes:
                            _traitement_part.ListeEquipes(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateEquipes:
                            _traitement_part.UpdateEquipes(xvaleur);
                            break;

                        case ServerCommandEnum.DemandJudokas:
                            break;
                        case ServerCommandEnum.EnvoieJudokas:
                            _traitement_part.ListeJudokas(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateJudokas:
                            _traitement_part.UpdateJudokas(xvaleur);
                            break;

                        case ServerCommandEnum.DemandLicencies:
                            break;
                        case ServerCommandEnum.EnvoieLicencies:
                            _traitement_part.ListeLicencies(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateLicencies:
                            break;

                        #endregion

                        #region DEROULEMENT

                        case ServerCommandEnum.DemandPhases:
                            break;
                        case ServerCommandEnum.EnvoiePhases:
                            _traitement_der.ListePhases(xvaleur);
                            break;
                        case ServerCommandEnum.UpdatePhases:
                            _traitement_der.UpdatePhases(xvaleur);
                            break;

                        case ServerCommandEnum.DemandCombats:
                            break;
                        case ServerCommandEnum.EnvoieCombats:
                            _traitement_der.ListeCombats(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateCombats:
                            _traitement_der.UpdateCombats(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateTapisCombats:
                            _traitement_der.UpdateTapisCombats(xvaleur);
                            break;

                        #endregion

                        #region TRAITEMENT

                        case ServerCommandEnum.ResultInscription:
                            break;
                        case ServerCommandEnum.TraiteInscription:
                            break;

                        case ServerCommandEnum.ResultCombats:
                            break;
                        case ServerCommandEnum.TraiteCombats:
                            _traitement_der.CombatReceived(xvaleur);
                            break;

                        case ServerCommandEnum.ResultRencontres:
                            break;
                        case ServerCommandEnum.TraiteRencontres:
                            _traitement_der.RencontreReceived(xvaleur);
                            break;
                        case ServerCommandEnum.UpdateRencontres:
                            _traitement_der.UpdateRencontreReceived(xvaleur);
                            break;

                        case ServerCommandEnum.ResultTapis:
                            break;
                        case ServerCommandEnum.TraiteTapis:
                            break;

                        case ServerCommandEnum.NonTraite:
                            break;

                            #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }
    }
}
