using KernelImpl;
using KernelImpl.Noyau.Arbitrage;
using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using KernelImpl.Noyau.Structures;
using AppPublication.ExtensionNoyau;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;
using AppPublication.ExtensionNoyau.Engagement;

namespace AppPublication.Export
{
    public static class ExportXML
    {
        public static void AddPublicationInfo(ref XmlDocument doc, ConfigurationExportSite config)
        {
            // Recupere le flag de la competition
            XmlNodeList listcomp = doc.GetElementsByTagName(ConstantXML.Competition);

            foreach (XmlNode node in listcomp)
            {
                XmlAttribute attrProchainCombat = doc.CreateAttribute(ConstantXML.publierProchainsCombats);
                attrProchainCombat.Value = config.PublierProchainsCombats.ToString().ToLower();
                XmlAttribute attrAffectationTapis = doc.CreateAttribute(ConstantXML.publierAffectationTapis);
                attrAffectationTapis.Value = config.PublierAffectationTapis.ToString().ToLower();
                XmlAttribute attrEngagements = doc.CreateAttribute(ConstantXML.publierEngagements);
                attrEngagements.Value = config.PublierEngagements.ToString().ToLower();

                XmlAttribute attrEngagementsAbsents = doc.CreateAttribute(ConstantXML.EngagementsAbsents);
                attrEngagementsAbsents.Value = config.EngagementsAbsents.ToString().ToLower();
                XmlAttribute attrEngagementsTousCombats = doc.CreateAttribute(ConstantXML.EngagementsTousCombats);
                attrEngagementsTousCombats.Value = config.EngagementsTousCombats.ToString().ToLower();
                XmlAttribute attrEngagementsScoreGP = doc.CreateAttribute(ConstantXML.EngagementsScoreGP);
                attrEngagementsScoreGP.Value = config.EngagementsScoreGP.ToString().ToLower();
                XmlAttribute attrAfficherPositionCombat = doc.CreateAttribute(ConstantXML.EngagementsPositionCombat);
                attrAfficherPositionCombat.Value = config.AfficherPositionCombat.ToString().ToLower();

                XmlAttribute attrDelaiActualisationClient = doc.CreateAttribute(ConstantXML.delaiActualisationClientSec);
                attrDelaiActualisationClient.Value = config.DelaiActualisationClientSec.ToString();
                XmlAttribute attrNbProchainsCombats = doc.CreateAttribute(ConstantXML.nbProchainsCombats);
                attrNbProchainsCombats.Value = config.NbProchainsCombats.ToString().ToLower();
                XmlAttribute attrMsgProchainsCombats = doc.CreateAttribute(ConstantXML.msgProchainsCombats);
                attrMsgProchainsCombats.Value = config.MsgProchainCombats;
                XmlAttribute attrDateGeneration = doc.CreateAttribute(ConstantXML.DateGeneration);
                attrDateGeneration.Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                XmlAttribute attrAppVersion = doc.CreateAttribute(ConstantXML.AppVersion);
                attrAppVersion.Value = Tools.AppInformation.Instance.AppVersion;
                XmlAttribute attrLogo = doc.CreateAttribute(ConstantXML.Logo);
                attrLogo.Value = config.Logo;
                XmlAttribute attrUseIntituleCommun = doc.CreateAttribute(ConstantXML.useIntituleCommun);
                attrUseIntituleCommun.Value = config.UseIntituleCommun.ToString().ToLower(); 
                XmlAttribute attrIntituleCommun = doc.CreateAttribute(ConstantXML.intituleCommun);
                attrIntituleCommun.Value = config.IntituleCommun;


                node.Attributes.Append(attrProchainCombat);
                node.Attributes.Append(attrAffectationTapis);
                node.Attributes.Append(attrEngagements);
                node.Attributes.Append(attrEngagementsAbsents);
                node.Attributes.Append(attrEngagementsTousCombats);
                node.Attributes.Append(attrEngagementsScoreGP);
                node.Attributes.Append(attrAfficherPositionCombat);
                node.Attributes.Append(attrDelaiActualisationClient);
                node.Attributes.Append(attrNbProchainsCombats);
                node.Attributes.Append(attrDateGeneration);
                node.Attributes.Append(attrMsgProchainsCombats);
                node.Attributes.Append(attrUseIntituleCommun);
                node.Attributes.Append(attrIntituleCommun);
                node.Attributes.Append(attrLogo);
                node.Attributes.Append(attrAppVersion);
            }
        }

        /// <summary>
        /// Ajout des logos et autres images dans le xml
        /// </summary>
        /// <param name="doc">le document</param>
        /// <param name="DC"></param>
        public static void AddLogo(ref XmlDocument doc, IJudoData DC)
        {
            //Logos logo1 = DC.Logos.FirstOrDefault(o => o.type == 1);
            string logo1 = "";

            foreach (string logo in Directory.GetFiles(ConstantFile.Logo1_dir))
            {
                logo1 = logo;
            }

            if (!String.IsNullOrWhiteSpace(logo1) && File.Exists(logo1))
            {
                //string data1 = @"data:image/png;base64," + Convert.ToBase64String(File.ReadAllBytes(logo1));
                string data1 = @"file:///" + logo1;

                XmlElement image = doc.CreateElement(ConstantXML.Image);
                image.SetAttribute(ConstantXML.Image, data1);
                image.SetAttribute(ConstantXML.Type, "1");
                doc.DocumentElement.AppendChild(image);
            }

            string logo2 = "";

            foreach (string logo in Directory.GetFiles(ConstantFile.Logo2_dir))
            {
                logo2 = logo;
            }

            if (!String.IsNullOrWhiteSpace(logo2) && File.Exists(logo2))
            {
                //string data2 = @"data:image/png;base64," + Convert.ToBase64String(File.ReadAllBytes(logo2));
                string data2 = @"file:///" + logo2;
                XmlElement image = doc.CreateElement(ConstantXML.Image);
                image.SetAttribute(ConstantXML.Image, data2);
                image.SetAttribute(ConstantXML.Type, "2");
                doc.DocumentElement.AppendChild(image);
            }

            foreach (string logo in Directory.GetFiles(ConstantFile.Logo3_dir))
            {
                if (!String.IsNullOrWhiteSpace(logo) && File.Exists(logo))
                {
                    //string data = @"data:image/png;base64," + Convert.ToBase64String(File.ReadAllBytes(logo));
                    string data = @"file:///" + logo;
                    XmlElement image = doc.CreateElement(ConstantXML.Image);
                    image.SetAttribute(ConstantXML.Image, data);
                    image.SetAttribute(ConstantXML.Type, "0");
                    doc.DocumentElement.AppendChild(image);
                }
            }
        }


        /// <summary>
        /// Ajoute les structures dans un document XML
        /// </summary>
        /// <param name="doc">Document XML a modifier</param>
        /// <param name="clubs">Liste des clubs au format XML</param>
        /// <param name="comites">Liste des comites au format XML</param>
        /// <param name="ligues">Liste des ligues au format XML</param>
        /// <param name="secteurs">Liste des secteurs au format XML</param>
        /// <param name="pays">Liste des pays au format XML</param>
        public static void AddStructures(ref XmlDocument doc, List<XElement> clubs, List<XElement> comites, List<XElement> secteurs, List<XElement> ligues,  List<XElement> pays)
        {
            XDocument xdoc = doc.ToXDocument();

            if (clubs != null)
            {
                foreach (XElement club in clubs)
                {
                    xdoc.Root.Add(club);
                }
            }

            if (comites != null)
            {
                foreach (XElement comite in comites)
                {
                    xdoc.Root.Add(comite);
                }
            }

            if (ligues != null)
            {
                foreach (XElement ligue in ligues)
                {
                    xdoc.Root.Add(ligue);
                }
            }

            if (secteurs != null)
            {
                foreach (XElement secteur in secteurs)
                {
                    xdoc.Root.Add(secteur);
                }
            }

            if (pays != null)
            {
                foreach (XElement unPays in pays)
                {
                    xdoc.Root.Add(unPays);
                }
            }

            doc = xdoc.ToXmlDocument();
        }

        /// <summary>
        /// Retourne la liste des comites en XML
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<XElement> GetComites(IJudoData DC)
        {
            ICollection<Comite> comites = null;
            List<XElement> output = new List<XElement>();

            try
            {
                comites = DC.Structures.Comites.ToList();

                foreach (Comite comite in comites)
                {
                    output.Add(comite.ToXml());
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
            }

            return output;
        }

        /// <summary>
        /// Retourne la liste des ligues en XML
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<XElement> GetLigues(IJudoData DC)
        {
            ICollection<Ligue> ligues = null;
            List<XElement> output = new List<XElement>();

            try
            {
                ligues = DC.Structures.Ligues.ToList();

                foreach (Ligue ligue in ligues)
                {
                    output.Add(ligue.ToXml());
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
            }

            return output;
        }



        /// <summary>
        /// Retourne la liste des secteurs en XML
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<XElement> GetSecteurs(IJudoData DC)
        {
            ICollection<Secteur> secteurs = null;
            List<XElement> output = new List<XElement>();

            try
            {
                secteurs = DC.Structures.Secteurs.ToList();

                foreach (Secteur secteur in secteurs)
                {
                   output.Add(secteur.ToXml());
                }

            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
            }

            return output;
        }

        /// <summary>
        /// Retourne la liste des ligues en XML
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<XElement> GetPays(IJudoData DC)
        {
            ICollection<Pays> pays = null;
            List<XElement> output = new List<XElement>();

            try
            {
                pays = DC.Structures.LesPays.ToList();

                foreach (Pays pays1 in pays)
                {
                    output.Add(pays1.ToXml());
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
            }

            return output;
        }


        /// <summary>
        /// Genere la liste des clubs en XML
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static List<XElement> GetClubs(IJudoData DC)
        {
            ICollection<Club> clubs = null;
            List<XElement> output = new List<XElement> ();

            try
            {
                clubs = (from c in DC.Structures.Clubs
                         join j in DC.Participants.Vuejudokas on c.id equals j.club
                         select c).Distinct().ToList();

                foreach (Club club in clubs)
                {
                    output.Add(club.ToXml());
                }

                clubs = (from c in DC.Structures.Clubs
                         join eq in DC.Participants.Equipes on c.id equals eq.club
                         select c).Distinct().ToList();

                foreach (Club club in clubs)
                {
                    output.Add(club.ToXml());
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
            }

            return output;
        }

        /// <summary>
        /// Ajout des grades à un document XML
        /// </summary>
        /// <param name="doc">le document</param>
        /// <param name="DC"></param>
        public static void AddCeintures(ref XmlDocument doc, IJudoData DC)
        {
            XDocument xdoc = doc.ToXDocument();

            foreach (Ceintures ceinture in DC.Categories.Grades.ToList())
            {
                xdoc.Root.Add(ceinture.ToXml());
            }

            doc = xdoc.ToXmlDocument();
        }

        public static XDocument ExportChecksumFichiers(List<FileWithChecksum> listFiles)
        {
            XDocument doc = new XDocument();

            XElement xelemRoot = new XElement(ConstantXML.checksums);
            doc.Add(xelemRoot);
            foreach (FileWithChecksum fc in listFiles)
            {
                xelemRoot.Add(fc.ToXml());
            }

            return doc;
        }

        public static List<FileWithChecksum> ImportChecksumFichiers(XElement rootElem)
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            foreach (XElement xinfo in rootElem.Descendants(ConstantXML.checksumFile))
            {
                FileWithChecksum fc = new FileWithChecksum();
                fc.LoadXml(xinfo);
                output.Add(fc);
            }

            return output;
        }

        /// <summary>
        /// Creation du document pour l'index
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="siteStructure"></param>
        /// <returns></returns>
        public static XmlDocument CreateDocumentIndex(IJudoData DC, ExportSiteStructure siteStructure)
        {
            XDocument doc = new XDocument();
            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);

            IList<Competition> competitions = DC.Organisation.Competitions.ToList();

            foreach (Competition competition in competitions)
            {
                XElement xcompetition = competition.ToXmlInformations();
                xcompetitions.Add(xcompetition);
            }

            return doc.ToXmlDocument();
        }

        /// <summary>
        /// Création du menu (pour le site)
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static XmlDocument CreateDocumentMenu(IJudoData DC, ExtendedJudoData EDC, ExportSiteStructure siteStructure)
        {
            XDocument doc = new XDocument();
            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);

            IList<Competition> competitions = DC.Organisation.Competitions.ToList();
            IList<Epreuve> epreuves1 = DC.Organisation.Epreuves.ToList();
            IList<Epreuve_Equipe> epreuves2 = DC.Organisation.EpreuveEquipes.ToList();
            //IList<i_vue_epreuve_interface> vepreuves = DC.Organisation.vepreuves.ToList();
            IList<Phase> phases = DC.Deroulement.Phases.ToList();
            IList<vue_groupe> groupes = DC.Deroulement.VueGroupes.ToList();

            foreach (Competition competition in competitions)
            {
                XElement xcompetition = competition.ToXmlInformations();
                xcompetitions.Add(xcompetition);

                for (int i = 0; i <= competition.nbTapis; i++)
                {
                    string directory = siteStructure.RepertoireCommon;

                    XElement xtapis = new XElement(ConstantXML.Tapis);
                    xtapis.SetAttributeValue(ConstantXML.Tapis, i);
                    //xtapis.SetAttributeValue(ConstantXML.Directory, directory);

                    xtapis.SetAttributeValue(ConstantXML.Tapis, i);

                    xcompetition.Add(xtapis);
                }

                IList<i_vue_epreuve_interface> epreuves_compet = null;
                if (competition.IsEquipe())
                {
                    epreuves_compet = DC.Organisation.VueEpreuveEquipes.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }
                else
                {
                    epreuves_compet = DC.Organisation.VueEpreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }


                foreach (i_vue_epreuve_interface ep in epreuves_compet)
                {

                    if (phases.Count(o => o.epreuve == ep.id && o.etat > (int)EtatPhaseEnum.Cree) == 0)
                    {
                        continue;
                    }

                    string directory = siteStructure.RepertoireEpreuve(ep.id.ToString(), ep.nom, true);

                    XElement xepreuve = ep.ToXml(DC);
                    xepreuve.SetAttributeValue(ConstantXML.Directory, directory);
                    xcompetition.Add(xepreuve);

                    XElement xphases = new XElement(ConstantXML.Phases);
                    xepreuve.Add(xphases);

                    List<int> phaseEnCours = new List<int>();
                    foreach (Phase phase in DC.Deroulement.Phases.Where(o => o.epreuve == ep.id))
                    {
                        XElement xphase = phase.ToXml();
                        xphases.Add(xphase);

                        if (phase.etat == (int)EtatPhaseEnum.TirageValide)
                        {
                            phaseEnCours.Add(phase.id);
                        }
                    }
                }

                // Ajoute les groupes dans la structure XML
                List<EchelonEnum> typesGroupes =EDC.Engagement.TypesGroupes[competition.id];
                foreach (EchelonEnum typeGroupe in typesGroupes)
                {
                    XElement xgroupesP = new XElement(ConstantXML.GroupeEngagements_groupes);
                    xgroupesP.SetAttributeValue(ConstantXML.GroupeEngagements_type, (int) typeGroupe);

                    // Recupere les groupes d'engagements pour la competition et le type de groupe en cours
                    IList<GroupeEngagements> grpEngages = EDC.Engagement.GroupesEngages.Where(g => g.Competition == competition.id && g.Type == (int)typeGroupe).ToList();

                    // Convertit en XML et ajoute a l'element racine
                    foreach (GroupeEngagements grp in grpEngages)
                    {
                        xgroupesP.Add(grp.ToXml());
                    }

                    // Ajoute les groupes du meme type a la racine
                    xcompetition.Add(xgroupesP);
                }
            }

            return doc.ToXmlDocument();
        }

        /// <summary>
        public static XmlDocument CreateDocumentEngagements(IJudoData DC, ExtendedJudoData EDC)
        {
            XDocument doc = new XDocument();
            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);
            IList<Competition> competitions = DC.Organisation.Competitions.ToList();
            IList<Club> clubs = DC.Structures.Clubs.ToList();
            foreach (Competition competition in competitions)
            {
                // On ne gere les engages que pour les Shiai et les individuelles
                if (competition.IsShiai() || competition.IsIndividuelle())
                {
                    XElement xcompetition = competition.ToXmlInformations();
                    xcompetitions.Add(xcompetition);

                    // Ajoute les groupes dans la structure XML
                    List<EchelonEnum> typesGroupes = EDC.Engagement.TypesGroupes[competition.id];
                    foreach (EchelonEnum typeGroupe in typesGroupes)
                    {
                        XElement xgroupesP = new XElement(ConstantXML.GroupeEngagements_groupes);
                        xgroupesP.SetAttributeValue(ConstantXML.GroupeEngagements_type, (int) typeGroupe);

                        // Recupere les groupes d'engagements pour la competition et le type de groupe en cours
                        IList<GroupeEngagements> groupes = EDC.Engagement.GroupesEngages.Where(g => g.Competition == competition.id && g.Type == (int)typeGroupe).ToList();

                        // Convertit en XML et ajoute a l'element racine
                        foreach (GroupeEngagements groupe in groupes)
                        {
                            xgroupesP.Add(groupe.ToXml());  
                        }

                        // Ajoute les groupes du meme type a la racine
                        xcompetition.Add(xgroupesP);
                    }

                    // Ajoute les judokas de la competition
                    IList<vue_judoka> vjudokas = DC.Participants.Vuejudokas.Where(vj => vj.idcompet == competition.id).ToList();
                    XElement xjudokas = new XElement(ConstantXML.GroupeEngagements_judokas);
                    foreach (vue_judoka vj in vjudokas)
                    {
                        xjudokas.Add(vj.ToXml());
                    }
                    xcompetition.Add(xjudokas);

                    // Ajoute les epreuves de la competition
                    IList<Epreuve> epreuves = DC.Organisation.Epreuves.Where(ep => ep.competition == competition.id).ToList();
                    XElement xepreuves = new XElement(ConstantXML.GroupeEngagements_epreuves);
                    foreach (Epreuve ep in epreuves)
                    {
                        xepreuves.Add(ep.ToXml(DC));
                    }
                    xcompetition.Add(xepreuves);

                    // Ajouter les es de la competition
                    IList<Phase> phases = DC.Deroulement.Phases.Join(epreuves, p => p.epreuve, e => e.id, (p, e) => p).ToList();
                    XElement xphases = new XElement(ConstantXML.Phases);
                    foreach (Phase ph in phases)
                    {
                        xphases.Add(ph.ToXml());
                    }
                    xcompetition.Add(xphases);

                    // Ajoute les combats de la competitions (les combats des phases des epreuves de la competition)
                    IList<Combat> combats = DC.Deroulement.Combats.Join(phases, c => c.phase, p => p.id, (c, p) => c).Distinct(new CombatEqualityComparer()).ToList();
                    XElement xcombats = new XElement(ConstantXML.GroupeEngagements_combats);
                    foreach (Combat c in combats)
                    {
                        xcombats.Add(c.ToXml(DC));
                    }
                    xcompetition.Add(xcombats);

                }
            }
            return doc.ToXmlDocument();
        }
        /// Document XML contenant les informations pour les generations des affectations de tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static XmlDocument CreateDocumentAffectationTapis(IJudoData DC)
        {
            XDocument doc = new XDocument();
            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);

            IList<Competition> competitions = DC.Organisation.Competitions.ToList();
            IList<Epreuve> epreuves1 = DC.Organisation.Epreuves.ToList();
            IList<Epreuve_Equipe> epreuves2 = DC.Organisation.EpreuveEquipes.ToList();
            //IList<i_vue_epreuve_interface> vepreuves = DC.Organisation.vepreuves.ToList();
            IList<Phase> phases = DC.Deroulement.Phases.ToList();
            // IList<vue_groupe> groupes = DC.Deroulement.vgroupes.ToList();

            foreach (Competition competition in competitions)
            {
                XElement xcompetition = competition.ToXmlInformations();
                xcompetitions.Add(xcompetition);

                IList<i_vue_epreuve_interface> epreuves_compet = null;
                if (competition.IsEquipe())
                {
                    epreuves_compet = DC.Organisation.VueEpreuveEquipes.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }
                else
                {
                    epreuves_compet = DC.Organisation.VueEpreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }


                foreach (i_vue_epreuve_interface ep in epreuves_compet)
                {

                    if (phases.Count(o => o.epreuve == ep.id && o.etat > (int)EtatPhaseEnum.Cree) == 0)
                    {
                        continue;
                    }

                    //i_vue_epreuve ep = vepreuves.FirstOrDefault(o => o.id == epreuve.id);
                    // string epreuve_nom = ep != null ? (ep.id + "_" + ep.nom) : null;

                    XElement xepreuve = ep.ToXml(DC);
                    xcompetition.Add(xepreuve);

                    XElement xphases = new XElement(ConstantXML.Phases);
                    xepreuve.Add(xphases);

                    // List<int> phaseEnCours = new List<int>();
                    foreach (Phase phase in DC.Deroulement.Phases.Where(o => o.epreuve == ep.id))
                    {
                        XElement xphase = phase.ToXml();
                        xphases.Add(xphase);

                        /*
                        if (phase.etat == (int)EtatPhaseEnum.TirageValide)
                        {
                            phaseEnCours.Add(phase.id);
                        }
                        */
                    }

                    // On n'ajoute les affectations de tapis que pour les individuelle (pas besoin en shiai ou equipe)
                    if (competition.IsIndividuelle())
                    {
                        XElement xtapisRoot = new XElement(ConstantXML.TapisEpreuve);
                        xepreuve.Add(xtapisRoot);

                        // Si au moins une phase active
                        // if (phaseEnCours.Count > 0)
                        // {
                        // Cherche tous les numeros de tapis sur les combats de la phase de l'epreuve si la phase n'est pas terminee
                        // List<int?> tapisEpreuve = DC.Deroulement.Combats.Where(o => o.epreuve == ep.id && o.tapis > 0).Join(phaseEnCours, o => o.phase, idx => idx, (o, idx) => o).Select(o => o.tapis).Distinct().ToList();
                        List<int> tapisEpreuve = DC.Deroulement.VueCombats.Where(o => o.epreuve_id == ep.id
                                                                               && o.combat_tapis > 0
                                                                               && o.phase_etat == (int)EtatPhaseEnum.TirageValide
                                                                               && o.combat_vaiqueur == null).Select(o => o.combat_tapis).Distinct().ToList();

                        // Ajoute les no de tapis
                        foreach (int noTapis in tapisEpreuve)
                        {
                            XElement xTapis = new XElement(ConstantXML.Tapis);
                            xTapis.SetAttributeValue(ConstantXML.Tapis_No, noTapis);
                            xtapisRoot.Add(xTapis);
                        }
                        // }
                    }
                }
            }

            return doc.ToXmlDocument();
        }

        public static XmlDocument CreateDocumentEpreuve(IJudoData DC, i_vue_epreuve_interface epreuve)
        {
            Competition competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);

            XDocument doc = new XDocument();
            XElement xcompetition = competition.ToXmlInformations();
            doc.Add(xcompetition);
            xcompetition.Add(ExportEpreuve(DC, epreuve));

            return doc.ToXmlDocument();
        }

        public static XmlDocument CreateDocumentPhase(i_vue_epreuve_interface epreuve, Phase phase, IJudoData DC)
        {
            Competition competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);

            XDocument doc = new XDocument();
            XElement xcompetition = competition.ToXmlInformations();
            doc.Add(xcompetition);

            XElement xepreuve = epreuve.ToXml(DC);
            xcompetition.Add(xepreuve);

            xepreuve.Add(ExportPhase(DC, phase));
            return doc.ToXmlDocument();
        }

        public static XmlDocument CreateDocumentFeuilleCombat(IJudoData DC, Phase _phase, int? tapis)
        {
            int nbtapis = DC.Organisation.Competitions.Max(o => o.nbTapis);
            // Competition competition = DC.Organisation.Competitions.FirstOrDefault();
            // Cherche la bonne competition de cette phase
            Competition competition = null;
            if (_phase != null)
            {
                Epreuve ep = DC.Organisation.Epreuves.Where(o => o.id == _phase.epreuve).FirstOrDefault();

                if (ep != null)
                {
                    competition = DC.Organisation.Competitions.Where(o => o.id == ep.competition).FirstOrDefault();
                }
            }

            if (competition == null)
            {
                // Par defaut, prend la premiere
                competition = DC.Organisation.Competitions.FirstOrDefault();
            }

            ICollection<int> id_compet_combats = new List<int>();   // Pour savoir les IDS de la ou des competitions des combats
            ICollection<XElement> xTapisList = new List<XElement>();

            ICollection<vue_groupe> groupes = DC.Deroulement.VueGroupes.ToList();
            ICollection<vue_epreuve> epreuves = DC.Organisation.VueEpreuves.ToList();
            ICollection<vue_epreuve_equipe> epreuves_eq = DC.Organisation.VueEpreuveEquipes.ToList();
            ICollection<Judoka> judokas = DC.Participants.Judokas.ToList();
            ICollection<Equipe> equipes = DC.Participants.Equipes.ToList();
            ICollection<Combat> combats = DC.Deroulement.Combats.ToList();
            ICollection<Rencontre> rencontres = DC.Deroulement.Rencontres.ToList();
            ICollection<Feuille> feuilles = DC.Deroulement.Feuilles.ToList();
            ICollection<Phase> phases = DC.Deroulement.Phases.ToList();
            ICollection<Participant> participants = DC.Deroulement.Participants.ToList();

            int start = 0;
            if (_phase == null && !tapis.HasValue)
            {
                start = 1;
            }

            for (int i = start; i <= nbtapis; i++)
            {
                if (tapis != null && tapis != i)
                {
                    continue;
                }

                XElement xtapis = new XElement(ConstantXML.Tapis);
                xtapis.SetAttributeValue(ConstantXML.Tapis, i);

                XElement xparticipants = new XElement(ConstantXML.Participants);
                xtapis.Add(xparticipants);

                XElement xcombats = new XElement(ConstantXML.Combats);
                xtapis.Add(xcombats);

                XElement xphases = new XElement(ConstantXML.Phases);
                xtapis.Add(xphases);

                XElement xpoules = new XElement(ConstantXML.Poules);
                xtapis.Add(xpoules);


                ICollection<int> epreuve_id_ajoute = new List<int>();
                ICollection<int> epreuveeq_id_ajoute = new List<int>();
                ICollection<int> participant_id_ajoute = new List<int>();
                ICollection<int> phase_id_ajoute = new List<int>();

                foreach (vue_groupe groupe in groupes.Where(o => o.groupe_tapis == i))
                {
                    xtapis.Add(groupe.ToXml());
                }

                if (_phase != null)
                {
                    if (competition.type == (int)CompetitionTypeEnum.Equipe)
                    {
                        vue_epreuve_equipe epreuve_eq = epreuves_eq.FirstOrDefault(o => o.id == _phase.epreuve);
                        if (!epreuveeq_id_ajoute.Contains(epreuve_eq.id))
                        {
                            xtapis.Add(epreuve_eq.ToXml(DC));
                            epreuveeq_id_ajoute.Add(epreuve_eq.id);

                            foreach (vue_epreuve epreuve in epreuves.Where(o => o.id_epreuve_equipe == _phase.epreuve))
                            {
                                xtapis.Add(epreuve.ToXml(DC));
                                epreuve_id_ajoute.Add(epreuve.id);
                            }
                        }
                    }
                    else
                    {
                        vue_epreuve epreuve = epreuves.FirstOrDefault(o => o.id == _phase.epreuve);
                        if (!epreuve_id_ajoute.Contains(epreuve.id))
                        {
                            xtapis.Add(epreuve.ToXml(DC));
                            epreuve_id_ajoute.Add(epreuve.id);
                        }
                    }
                }

                List<Combat> combatList;
                if ((competition.afficheCSA == (int)TypeCSAEnum.Minisite) || (competition.afficheCSA == (int)TypeCSAEnum.Tous))
                {
                    // En cas d'affichage CSA on doit aussi prendre en compte les combats incomplets en attente
                    combatList = combats.Where(o => o.tapis == i
                    && (o.vainqueur == null || o.vainqueur == -1)
                    && o.virtuel == false
                    ).ToList();
                }
                else
                {
                    combatList = combats.Where(o => o.tapis == i
                    && (o.vainqueur == null || o.vainqueur == -1)
                    && o.virtuel == false
                    && o.participant1 != null && o.participant1 != 0
                    && o.participant2 != null && o.participant2 != 0
                    ).ToList();
                }

                foreach (Combat combat in combatList)
                {
                    //EPREUVE
                    Phase phase = phases.FirstOrDefault(o => o.id == combat.phase);
                    if (phase == null || (_phase != null && phase.epreuve != _phase.epreuve) || phase.etat < (int)EtatPhaseEnum.TirageValide)
                    {
                        continue;
                    }
                    else if (!phase_id_ajoute.Contains(phase.id))
                    {
                        xphases.Add(phase.ToXml());
                        phase_id_ajoute.Add(phase.id);
                    }

                    if (competition.type == (int)CompetitionTypeEnum.Equipe)
                    {
                        vue_epreuve_equipe epreuve_eq2 = epreuves_eq.FirstOrDefault(o => o.id == phase.epreuve);

                        // Enregistre l'ID de la competition
                        if (epreuve_eq2 != null && !id_compet_combats.Contains(epreuve_eq2.competition))
                        {
                            id_compet_combats.Add(epreuve_eq2.competition);
                        }

                        if (!epreuveeq_id_ajoute.Contains(epreuve_eq2.id))
                        {
                            xtapis.Add(epreuve_eq2.ToXml(DC));
                            epreuveeq_id_ajoute.Add(epreuve_eq2.id);

                            foreach (vue_epreuve epreuve2 in epreuves.Where(o => o.id_epreuve_equipe == phase.epreuve))
                            {
                                xtapis.Add(epreuve2.ToXml(DC));
                                epreuve_id_ajoute.Add(epreuve2.id);
                            }
                        }
                    }
                    else
                    {
                        vue_epreuve epreuve2 = epreuves.FirstOrDefault(o => o.id == phase.epreuve);

                        // Enregistre l'ID de la competition
                        if (epreuve2 != null && !id_compet_combats.Contains(epreuve2.competition))
                        {
                            id_compet_combats.Add(epreuve2.competition);
                        }

                        if (!epreuve_id_ajoute.Contains(epreuve2.id))
                        {
                            xtapis.Add(epreuve2.ToXml(DC));
                            epreuve_id_ajoute.Add(epreuve2.id);
                        }
                    }

                    //PARTICIPANTS
                    foreach (Participant p in participants.Where(o => o.phase == phase.id))
                    {
                        if (!participant_id_ajoute.Contains(p.judoka))
                        {
                            XElement xparticipant = p.ToXml(DC);
                            switch ((CompetitionTypeEnum)DC.Organisation.Competition.type)
                            {
                                case CompetitionTypeEnum.Individuel:
                                case CompetitionTypeEnum.Shiai:
                                    Judoka judoka1 = judokas.FirstOrDefault(o => o.id == p.judoka);
                                    if (judoka1 != null)
                                    {
                                        xparticipant.Add(judoka1.ToXml(DC));
                                    }
                                    break;
                                case CompetitionTypeEnum.Equipe:
                                    Equipe IEquipe = equipes.FirstOrDefault(o => o.id == p.judoka);
                                    if (IEquipe != null)
                                    {
                                        XElement xequipe = IEquipe.ToXml();
                                        xparticipant.Add(xequipe);
                                        foreach (Judoka judoka in judokas.Where(o => o.equipe == IEquipe.id))
                                        {
                                            xequipe.Add(judoka.ToXml(DC));
                                        }
                                    }
                                    break;
                            }

                            xparticipants.Add(xparticipant);
                            participant_id_ajoute.Add(p.judoka);
                        }
                    }

                    Phase p1 = phases.FirstOrDefault(o => o.id == combat.phase);
                    Feuille f1 = feuilles.FirstOrDefault(o => o.combat == combat.id);
                    List<Combat> cs = combats.Where(o => o.phase == combat.phase).ToList();

                    XElement xcombat = combat.ToXml(DC);
                    xcombats.Add(xcombat);

                    foreach (Rencontre rencontre in rencontres.Where(o => o.combat == combat.id))
                    {
                        xcombat.Add(rencontre.ToXml());
                    }

                    //xcombats.Add(combat.ToXml(DC));
                }

                foreach (Phase phase in phases)
                {
                    if (phase_id_ajoute.Contains(phase.id))
                    {
                        foreach (Poule IPoule in DC.Deroulement.Poules.Where(o => o.phase == phase.id).ToList())
                        {
                            if (combats.Count(o => o.phase == phase.id && o.reference == ("" + IPoule.numero) &&
                                o.tapis == i && (o.vainqueur == null || o.vainqueur == -1) && o.virtuel == false) > 0)
                            {
                                xpoules.Add(IPoule.ToXml());
                            }
                        }
                    }
                }

                xTapisList.Add(xtapis); // Met l'element dans la liste pour l'attacher plus tard quand on aura valide la competition
            }

            

            // Verifie si l'ID de la competition utilise au debut est le bon
            if (id_compet_combats.Count > 0)
            {
                if (competition.id != id_compet_combats.First())
                {
                    // Met la bonne competition a la place
                    competition = DC.Organisation.Competitions.Where(o => o.id == id_compet_combats.First()).FirstOrDefault();
                }
            }

            // Initialise l'arbre XML avec la bonne competition
            XDocument doc = new XDocument();
            XElement xcompetition = competition.ToXmlInformations();
            doc.Add(xcompetition);

            // Attache les tapis
            foreach (XElement xt in xTapisList)
            {
                xcompetition.Add(xt);
            }

            return doc.ToXmlDocument();
        }

        private static XElement ExportEpreuve(IJudoData DC, i_vue_epreuve_interface epreuve)
        {
            XElement xepreuve = epreuve.ToXml(DC);

            XElement xinscrits = new XElement(ConstantXML.Epreuve_Inscrits);
            xepreuve.Add(xinscrits);

            //string query = "";
            ////if(epreuve is vue_epreuve)
            ////{
            //query += "SELECT IJudoka.* FROM IJudoka INNER JOIN EpreuveIJudoka ON IJudoka.id = EpreuveIJudoka.judoka WHERE EpreuveIJudoka.epreuve=" + epreuve.id;

            //IList<IJudoka> judokas_inscrit = DC.Database.SqlQuery<IJudoka>(query).ToList();

            IList<Judoka> judokas_inscrit = DC.Participants.GetJudokaEpreuve(epreuve.id).ToList();
            IList<EpreuveJudoka> epj = DC.Participants.EpreuveJudokas.ToList();

            foreach (Judoka judoka in judokas_inscrit)
            {
                xinscrits.Add(judoka.ToXml(DC));
                EpreuveJudoka EJ = epj.FirstOrDefault(o => o.judoka == judoka.id);
                if (EJ != null)
                {
                    xinscrits.Add(EJ.ToXml());
                }
            }


            //foreach (EpreuveJudoka EJ in epj)
            //{
            //    xinscrits.Add(EJ.ToXml());
            //}
            //}

            XElement xphases = new XElement(ConstantXML.Phases);
            xepreuve.Add(xphases);

            foreach (Phase phase in DC.Deroulement.Phases.Where(o => o.epreuve == epreuve.id))
            {
                if (DC.Deroulement.Participants.Count(o => o.phase == phase.id) > 0)
                {
                    xphases.Add(ExportXML.ExportPhase(DC, phase));
                }
            }

            xepreuve.Add(ExportXML.ExportClassementFinal(DC, epreuve));

            return xepreuve;
        }

        private static XElement ExportPhase(IJudoData DC, Phase phase)
        {
            List<Judoka> judokas = DC.Participants.Judokas.ToList();
            List<Equipe> equipes = DC.Participants.Equipes.ToList();

            XElement xphase = phase.ToXml();

            if (phase.typePhase == (int)TypePhaseEnum.Poule)
            {
                XElement xpoules = new XElement(ConstantXML.Poules);
                xphase.Add(xpoules);

                foreach (Poule IPoule in DC.Deroulement.Poules.Where(o => o.phase == phase.id).ToList())
                {
                    xpoules.Add(IPoule.ToXml());
                }
            }


            XElement xparticipants = new XElement(ConstantXML.Participants);
            xphase.Add(xparticipants);

            foreach (Participant p in DC.Deroulement.Participants.Where(o => o.phase == phase.id).ToList())
            {
                XElement xparticipant = p.ToXml(DC);
                switch ((CompetitionTypeEnum)DC.Organisation.Competition.type)
                {
                    case CompetitionTypeEnum.Individuel:
                    case CompetitionTypeEnum.Shiai:
                        Judoka judoka1 = judokas.FirstOrDefault(o => o.id == p.judoka);
                        if (judoka1 != null)
                        {
                            xparticipant.Add(judoka1.ToXml(DC));
                        }
                        break;
                    case CompetitionTypeEnum.Equipe:
                        Equipe IEquipe = equipes.FirstOrDefault(o => o.id == p.judoka);
                        if (IEquipe != null)
                        {
                            XElement xequipe = IEquipe.ToXml();
                            xparticipant.Add(xequipe);
                            foreach (Judoka judoka in judokas.Where(o => o.equipe == IEquipe.id))
                            {
                                xequipe.Add(judoka.ToXml(DC));
                            }
                        }
                        break;
                }

                xparticipants.Add(xparticipant);
            }

            XElement xcombats = new XElement(ConstantXML.Combats);
            xphase.Add(xcombats);

            List<Combat> combats = DC.Deroulement.Combats.Where(o => o.phase == phase.id).ToList();
            ICollection<Feuille> feuilles = DC.Deroulement.Feuilles.Where(o => o.phase == phase.id).ToList();
            ICollection<Rencontre> rencontres = DC.Deroulement.Rencontres.ToList();

            foreach (Combat c in combats)
            {
                Feuille f1 = feuilles.FirstOrDefault(o => o.combat == c.id);

                XElement xcombat = c.ToXml(DC);
                xcombats.Add(xcombat);

                foreach (Rencontre rencontre in rencontres.Where(o => o.combat == c.id))
                {
                    xcombat.Add(rencontre.ToXml());
                }

                //xcombats.Add(c.ToXml(DC));
            }

            return xphase;
        }

        private static XElement ExportClassementFinal(IJudoData DC, i_vue_epreuve_interface epreuve)
        {
            List<Judoka> judokas = DC.Participants.Judokas.ToList();
            List<Equipe> equipes = DC.Participants.Equipes.ToList();
            Phase phase = DC.Deroulement.Phases.FirstOrDefault(o => o.epreuve == epreuve.id);

            XElement xclassement = new XElement(ConstantXML.Classement);

            ICollection<Participant> participants1 = DC.Deroulement.ListeParticipant2(epreuve.id).ToList();
            ICollection<Participant> participants2 = DC.Deroulement.ListeParticipant1(epreuve.id).ToList();

            //ICollection<IParticipant> participants2 = DC.Database.SqlQuery<IParticipant>(query).ToList();

            foreach (Participant p in participants2)
            {
                if (participants1.Count(u => u.judoka == p.judoka) == 0)
                {
                    participants1.Add(p);
                }
            }

            //participants1 = participants1.Concat(participants2.Where(o => participants1.Count(u => u.judoka == o.judoka) == 0).ToList()).ToList();

            foreach (Participant p in participants1.OrderBy(o => o.classementFinal))
            {
                XElement xparticipant = p.ToXml(DC);
                switch ((CompetitionTypeEnum)DC.Organisation.Competition.type)
                {
                    case CompetitionTypeEnum.Individuel:
                    case CompetitionTypeEnum.Shiai:
                        Judoka judoka1 = judokas.FirstOrDefault(o => o.id == p.judoka);
                        if (judoka1 != null)
                        {
                            xparticipant.Add(judoka1.ToXml(DC));
                        }
                        break;
                    case CompetitionTypeEnum.Equipe:
                        Equipe IEquipe = equipes.FirstOrDefault(o => o.id == p.judoka);
                        if (IEquipe != null)
                        {
                            XElement xequipe = IEquipe.ToXml();
                            xparticipant.Add(xequipe);
                            foreach (Judoka judoka in judokas.Where(o => o.equipe == IEquipe.id))
                            {
                                xequipe.Add(judoka.ToXml(DC));
                            }
                        }
                        break;
                }
                xclassement.Add(xparticipant);
            }

            return xclassement;
        }

    }
}
