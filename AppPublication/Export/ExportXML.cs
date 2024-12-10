using KernelImpl;
using KernelImpl.Noyau.Arbitrage;
using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using KernelImpl.Noyau.Structures;
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
                attrProchainCombat.Value = config.PublierProchainsCombats.ToString();
                XmlAttribute attrAffectationTapis = doc.CreateAttribute(ConstantXML.publierAffectationTapis);
                attrAffectationTapis.Value = config.PublierAffectationTapis.ToString();
                XmlAttribute attrParticipants = doc.CreateAttribute(ConstantXML.publierParticipants);
                attrParticipants.Value = config.PublierParticpants.ToString();
                XmlAttribute attrDelaiActualisationClient = doc.CreateAttribute(ConstantXML.delaiActualisationClientSec);
                attrDelaiActualisationClient.Value = config.DelaiActualisationClientSec.ToString();
                XmlAttribute attrNbProchainsCombats = doc.CreateAttribute(ConstantXML.nbProchainsCombats);
                attrNbProchainsCombats.Value = config.NbProchainsCombats.ToString();
                XmlAttribute attrMsgProchainsCombats = doc.CreateAttribute(ConstantXML.msgProchainsCombats);
                attrMsgProchainsCombats.Value = config.MsgProchainCombats;
                XmlAttribute attrDateGeneration = doc.CreateAttribute(ConstantXML.DateGeneration);
                attrDateGeneration.Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                XmlAttribute attrAppVersion = doc.CreateAttribute(ConstantXML.AppVersion);
                attrAppVersion.Value = Tools.AppInformation.Instance.AppVersion;
                XmlAttribute attrLogo = doc.CreateAttribute(ConstantXML.Logo);
                attrLogo.Value = config.Logo;

                node.Attributes.Append(attrProchainCombat);
                node.Attributes.Append(attrAffectationTapis);
                node.Attributes.Append(attrParticipants);
                node.Attributes.Append(attrDelaiActualisationClient);
                node.Attributes.Append(attrNbProchainsCombats);
                node.Attributes.Append(attrDateGeneration);
                node.Attributes.Append(attrMsgProchainsCombats);
                node.Attributes.Append(attrLogo);
                node.Attributes.Append(attrAppVersion);
            }
        }

        /// <summary>
        /// Ajout des logos et autres images dans le xml
        /// </summary>
        /// <param name="doc">le document</param>
        /// <param name="DC"></param>
        public static void AddLogo(ref XmlDocument doc, JudoData DC)
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
        /// Ajout des clubs à un document XML
        /// </summary>
        /// <param name="doc">le document</param>
        /// <param name="DC"></param>
        public static void AddClubs(ref XmlDocument doc, JudoData DC)
        {
            XDocument xdoc = doc.ToXDocument();

            ICollection<Club> clubs = null;
            ICollection<Pays> pays = null;
            ICollection<Comite> comites = null;
            ICollection<Secteur> secteurs = null;
            ICollection<Ligue> ligues = null;


            using (TimedLock.Lock((DC.Participants.vjudokas as ICollection).SyncRoot))
            {
                using (TimedLock.Lock((DC.Structures.Clubs as ICollection).SyncRoot))
                {
                    clubs = (from c in DC.Structures.Clubs
                             join j in DC.Participants.vjudokas on c.id equals j.club
                             select c).Distinct().ToList();
                }
            }

            foreach (Club club in clubs)
            {
                xdoc.Root.Add(club.ToXml());
            }

            using (TimedLock.Lock((DC.Participants.Equipes as ICollection).SyncRoot))
            {
                using (TimedLock.Lock((DC.Structures.Clubs as ICollection).SyncRoot))
                {
                    clubs = (from c in DC.Structures.Clubs
                             join eq in DC.Participants.Equipes on c.id equals eq.club
                             select c).Distinct().ToList();
                }
            }

            foreach (Club club in clubs)
            {
                xdoc.Root.Add(club.ToXml());
            }

            using (TimedLock.Lock((DC.Structures.Comites as ICollection).SyncRoot))
            {
                comites = DC.Structures.Comites.ToList();
            }

            foreach (Comite comite in comites)
            {
                xdoc.Root.Add(comite.ToXml());
            }

            using (TimedLock.Lock((DC.Structures.Secteurs as ICollection).SyncRoot))
            {
                secteurs = DC.Structures.Secteurs.ToList();
            }

            foreach (Secteur secteur in secteurs)
            {
                xdoc.Root.Add(secteur.ToXml());
            }

            using (TimedLock.Lock((DC.Structures.Ligues as ICollection).SyncRoot))
            {
                ligues = DC.Structures.Ligues.ToList();
            }

            foreach (Ligue ligue in ligues)
            {
                xdoc.Root.Add(ligue.ToXml());
            }

            using (TimedLock.Lock((DC.Structures.LesPays as ICollection).SyncRoot))
            {
                pays = DC.Structures.LesPays.ToList();
            }

            foreach (Pays pays1 in pays)
            {
                xdoc.Root.Add(pays1.ToXml());
            }

            doc = xdoc.ToXmlDocument();
        }

        /// <summary>
        /// Ajout des grades à un document XML
        /// </summary>
        /// <param name="doc">le document</param>
        /// <param name="DC"></param>
        public static void AddCeintures(ref XmlDocument doc, JudoData DC)
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
        /// Création du menu (pour le site)
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static XmlDocument CreateDocumentMenu(JudoData DC, ExportSiteStructure siteStructure)
        {
            XDocument doc = new XDocument();
            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);

            IList<Competition> competitions = DC.Organisation.Competitions.ToList();
            IList<Epreuve> epreuves1 = DC.Organisation.Epreuves.ToList();
            IList<Epreuve_Equipe> epreuves2 = DC.Organisation.EpreuveEquipes.ToList();
            //IList<i_vue_epreuve_interface> vepreuves = DC.Organisation.vepreuves.ToList();
            IList<Phase> phases = DC.Deroulement.Phases.ToList();
            IList<vue_groupe> groupes = DC.Deroulement.vgroupes.ToList();

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
                    epreuves_compet = DC.Organisation.vepreuves_equipe.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }
                else
                {
                    epreuves_compet = DC.Organisation.vepreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
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
            }

            return doc.ToXmlDocument();
        }

        /// <summary>
        public static XmlDocument CreateDocumentParticipants(JudoData DC, bool groupeClub, ExportSiteStructure siteStructure)
        {
            XDocument doc = new XDocument();
            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);
            IList<Competition> competitions = DC.Organisation.Competitions.ToList();
            IList<Club> clubs = DC.Structures.Clubs.ToList();
            foreach (Competition competition in competitions)
            {
                if (competition.IsShiai() || competition.IsIndividuelle())
                {
                    XElement xcompetition = competition.ToXmlInformations();
                    xcompetitions.Add(xcompetition);
                    XElement xgroupesP = new XElement(ConstantXML.GroupeParticipants_groupes);
                    foreach (int s in Enum.GetValues(typeof(EpreuveSexeEnum)))
                    {
                        IList<Epreuve> epreuves = DC.Organisation.Epreuves.Where(ep => ep.competition == competition.id && ep.sexe == s).ToList();
                        if (groupeClub)
                        {
                            IList<string> clubEp = DC.Participants.vjudokas.Join(epreuves, vj => vj.idepreuve, ep => ep.id, (vj, ep) => vj).Select(o => o.club).Distinct().ToList();
                            foreach (string club in clubEp)
                            {
                                XElement xgroupeP = new XElement(ConstantXML.GroupeParticipants_groupe);
                                xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_competition, competition.id);
                                xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_sexe, s);
                                xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_type, "Club");
                                xgroupeP.SetAttributeValue(ConstantXML.GroupeParticipants_id, club);
                                xgroupesP.Add(xgroupeP);
                            }
                        }
                        else
                        {
                            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                            foreach (char c in alphabet)
                            {
                            }
                        }
                    }
                    xcompetition.Add(xgroupesP);
                }
            }
            return doc.ToXmlDocument();
        }
        /// Document XML contenant les informations pour les generations des affectations de tapis
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        public static XmlDocument CreateDocumentAffectationTapis(JudoData DC)
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
                    epreuves_compet = DC.Organisation.vepreuves_equipe.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }
                else
                {
                    epreuves_compet = DC.Organisation.vepreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>().ToList();
                }


                foreach (i_vue_epreuve_interface ep in epreuves_compet)
                {

                    if (phases.Count(o => o.epreuve == ep.id && o.etat > (int)EtatPhaseEnum.Cree) == 0)
                    {
                        continue;
                    }

                    //i_vue_epreuve ep = vepreuves.FirstOrDefault(o => o.id == epreuve.id);
                    string epreuve_nom = ep != null ? (ep.id + "_" + ep.nom) : null;

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
                        List<int> tapisEpreuve = DC.Deroulement.vcombats.Where(o => o.epreuve_id == ep.id
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

        /// <summary>
        /// Export de la compétition complète
        /// </summary>
        /// <param name="DC"></param>
        /// <returns></returns>
        // TODO Not used
        /*
        public static XmlDocument ExportCompetition(JudoData DC, Competition competition)
        {
            XDocument doc = new XDocument();
            XComment comment = new XComment("compétition de judo");
            doc.Add(comment);
            comment = new XComment("Généré le : " + System.DateTime.Now.ToString());

            XElement xcompetitions = new XElement(ConstantXML.Competitions);
            doc.Add(xcompetitions);

            if (competition != null)
            {
                XElement xcompetition = competition.ToXmlInformations();
                xcompetitions.Add(xcompetition);

                XElement xepreuves = new XElement(ConstantXML.Epreuves);
                xcompetition.Add(xepreuves);

                if (competition.type == (int)CompetitionTypeEnum.Equipe)
                {
                    foreach (vue_epreuve_equipe epreuve in DC.Organisation.vepreuves_equipe.Where(o => o.competition == competition.id))
                    {
                        XElement xepreuve = ExportXML.ExportEpreuve(DC, epreuve);
                        xepreuves.Add(xepreuve);
                    }
                }
                else
                {
                    foreach (vue_epreuve epreuve in DC.Organisation.vepreuves.Where(o => o.competition == competition.id))
                    {
                        XElement xepreuve = ExportXML.ExportEpreuve(DC, epreuve);
                        xepreuves.Add(xepreuve);
                    }
                }
            }
            else
            {
                foreach (Competition compte in DC.Organisation.Competitions)
                {
                    XElement xcompetition = compte.ToXmlInformations();
                    xcompetitions.Add(xcompetition);

                    XElement xepreuves = new XElement(ConstantXML.Epreuves);
                    xcompetition.Add(xepreuves);

                    if (compte.type == (int)CompetitionTypeEnum.Equipe)
                    {
                        foreach (vue_epreuve_equipe epreuve in DC.Organisation.vepreuves_equipe.Where(o => o.competition == compte.id))
                        {
                            XElement xepreuve = ExportXML.ExportEpreuve(DC, epreuve);
                            xepreuves.Add(xepreuve);
                        }
                    }
                    else
                    {
                        foreach (vue_epreuve epreuve in DC.Organisation.vepreuves.Where(o => o.competition == compte.id))
                        {
                            XElement xepreuve = ExportXML.ExportEpreuve(DC, epreuve);
                            xepreuves.Add(xepreuve);
                        }
                    }
                }
            }


            XElement xcommissaires = new XElement(ConstantXML.Commissaires);
            foreach (Commissaire commissaire in DC.Arbitrage.Commissaires.Where(o => o.present))
            {
                xcommissaires.Add(commissaire.ToXml());
            }
            xcompetitions.Add(xcommissaires);

            XElement xarbitres = new XElement(ConstantXML.Arbitres);
            foreach (Arbitre arbitre in DC.Arbitrage.Arbitres.Where(o => o.present))
            {
                xarbitres.Add(arbitre.ToXml());
            }
            xcompetitions.Add(xarbitres);

            XElement xdelegues = new XElement(ConstantXML.Delegues);
            foreach (Delegue delegue in DC.Arbitrage.Delegues)
            {
                xdelegues.Add(delegue.ToXml());
            }
            xcompetitions.Add(xdelegues);

            XmlDocument result = doc.ToXmlDocument();
            ExportXML.AddClubs(ref result, DC);

            return result;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epreuve"></param>
        /// <param name="judokas"></param>
        /// <param name="DC"></param>
        /// <returns></returns>
        // TODO Not used
        /*
        public static XmlDocument CreateDocumentJudokasEpreuve(i_vue_epreuve_interface epreuve, ICollection<vue_judoka> judokas, JudoData DC)
        {
            Competition competition = null;
            if (epreuve != null)
            {
                competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);
                if (competition == null)
                {
                    competition = DC.Organisation.Competitions.FirstOrDefault();
                }
            }
            else
            {
                competition = DC.Organisation.Competitions.FirstOrDefault();
            }

            XDocument doc = new XDocument();
            XElement xcompetition = competition.ToXmlInformations();
            doc.Add(xcompetition);

            if (epreuve != null)
            {
                XElement xepreuve = epreuve.ToXml(DC);
                xcompetition.Add(xepreuve);
            }

            XElement xjudokas = new XElement(ConstantXML.Judokas);
            xcompetition.Add(xjudokas);

            XElement xequipes = new XElement(ConstantXML.Equipes);
            xcompetition.Add(xequipes);

            foreach (int equipeid in judokas.Select(o => o.equipe).Distinct())
            {
                Equipe IEquipe = DC.Participants.Equipes.FirstOrDefault(o => o.id == equipeid);
                XElement xequipe = null;
                if (IEquipe != null)
                {
                    xequipe = IEquipe.ToXml();
                    xequipes.Add(xequipe);
                }

                foreach (vue_judoka judoka in judokas.Where(o => o.equipe == equipeid))
                {
                    if (xequipe == null)
                    {
                        xjudokas.Add(judoka.ToXml());
                    }
                    else
                    {
                        xequipe.Add(judoka.ToXml());
                    }
                }
            }

            //foreach (vue_judoka judoka in judokas)
            //{
            //    xjudokas.Add(judoka.ToXml());
            //}

            return doc.ToXmlDocument();
        }
        */

        public static XmlDocument CreateDocumentEpreuve(JudoData DC, i_vue_epreuve_interface epreuve)
        {
            Competition competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);

            XDocument doc = new XDocument();
            XElement xcompetition = competition.ToXmlInformations();
            doc.Add(xcompetition);
            xcompetition.Add(ExportEpreuve(DC, epreuve));

            return doc.ToXmlDocument();
        }

        public static XmlDocument CreateDocumentPhase(i_vue_epreuve_interface epreuve, Phase phase, JudoData DC)
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

        public static XmlDocument CreateDocumentFeuilleCombat(JudoData DC, Phase _phase, int? tapis)
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
            else
            {
                // Pas de phase specifiee, on va chercher
            }
            if (competition == null)
            {
                // Par defaut, prend la premiere
                competition = DC.Organisation.Competitions.FirstOrDefault();
            }

            ICollection<int> id_compet_combats = new List<int>();   // Pour savoir les IDS de la ou des competitions des combats
            ICollection<XElement> xTapisList = new List<XElement>();

            ICollection<vue_groupe> groupes = DC.Deroulement.vgroupes.ToList();
            ICollection<vue_epreuve> epreuves = DC.Organisation.vepreuves.ToList();
            ICollection<vue_epreuve_equipe> epreuves_eq = DC.Organisation.vepreuves_equipe.ToList();
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
                            switch ((CompetitionTypeEnum)DC.competition.type)
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

        private static XElement ExportEpreuve(JudoData DC, i_vue_epreuve_interface epreuve)
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
            IList<EpreuveJudoka> epj = DC.Participants.EJS.ToList();

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

        private static XElement ExportPhase(JudoData DC, Phase phase)
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
                switch ((CompetitionTypeEnum)DC.competition.type)
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

        private static XElement ExportClassementFinal(JudoData DC, i_vue_epreuve_interface epreuve)
        {
            List<Judoka> judokas = DC.Participants.Judokas.ToList();
            List<Equipe> equipes = DC.Participants.Equipes.ToList();
            Phase phase = DC.Deroulement.Phases.FirstOrDefault(o => o.epreuve == epreuve.id);

            XElement xclassement = new XElement(ConstantXML.Classement);

            ICollection<Participant> participants1 = DC.Deroulement.ListeParticipant2(epreuve.id, DC).ToList();
            ICollection<Participant> participants2 = DC.Deroulement.ListeParticipant1(epreuve.id, DC).ToList();

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
                switch ((CompetitionTypeEnum)DC.competition.type)
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

        // TODO Not used
        /*
        public static XmlDocument CreateDocumentPhaseRepechage(i_vue_epreuve_interface epreuve, Phase phase, JudoData DC)
        {
            Competition competition = DC.Organisation.Competitions.FirstOrDefault();

            XDocument doc = new XDocument();

            XElement xrepechage = new XElement("repechage");
            doc.Add(xrepechage);

            List<Judoka> judokas = DC.Participants.Judokas.ToList();
            List<Equipe> equipes = DC.Participants.Equipes.ToList();

            if (phase.niveauRepeches == 0 && phase.niveauRepechage == 0)
            {
                return doc.ToXmlDocument();
            }

            XElement xphase = phase.ToXml();
            xrepechage.Add(xphase);

            //PARTICIPANTS
            XElement xparticipants = new XElement(ConstantXML.Participants);
            ICollection<Participant> participants = DC.Deroulement.Participants.Where(o => o.phase == phase.id).OrderBy(o => o.position).ToList();
            foreach (Participant p in participants)
            {
                XElement xparticipant = p.ToXml(DC);
                switch ((CompetitionTypeEnum)DC.competition.type)
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
            xphase.Add(xparticipants);



            //COMBATS
            XElement xcombats = new XElement(ConstantXML.Combats);
            //string n = this.numero.ToString();

            List<Combat> combats = DC.Deroulement.Combats.Where(o => o.phase == phase.id).ToList();
            ICollection<Feuille> feuilles = DC.Deroulement.Feuilles.Where(o => o.phase == phase.id).ToList();
            foreach (Combat c in combats)
            {
                Feuille f1 = feuilles.FirstOrDefault(o => o.combat == c.id);
                xcombats.Add(c.ToXml(DC));
            }
            xphase.Add(xcombats);

            feuilles = DC.Deroulement.Feuilles.Where(o => o.phase == phase.id && o.repechage).ToList();

            Feuille F1 = feuilles.FirstOrDefault(o => o.reference == "2.1.1");

            if (F1 == null)
            {
                return doc.ToXmlDocument();
            }


            Feuille F1_1 = feuilles.FirstOrDefault(o => o.reference == F1.ref1);
            Feuille F1_2 = feuilles.FirstOrDefault(o => o.reference == F1.ref2);

            XElement xF1table = new XElement("table");

            xphase.Add(xF1table);

            XElement xF1tr1 = new XElement("tr");
            xF1table.Add(xF1tr1);

            XElement xF1td1_r = new XElement("td");
            xF1td1_r.SetAttributeValue("rowspan", countRowSpan(F1_1, feuilles).ToString());
            xF1td1_r.SetAttributeValue("type", "2");
            xF1tr1.Add(xF1td1_r);

            XElement xF1td1 = new XElement("td");
            xF1td1.SetAttributeValue("rowspan", countRowSpan(F1_1, feuilles).ToString());
            xF1td1.SetAttributeValue("type", "1");
            xF1tr1.Add(xF1td1);
            XElement xF1combat1 = new XElement("combat");
            xF1td1.Add(xF1combat1);
            xF1combat1.SetAttributeValue("reference", F1.reference.ToString());
            xF1combat1.SetAttributeValue("judoka", "1");

            XElement xF1td1_r2 = new XElement("td");
            xF1td1_r2.SetAttributeValue("rowspan", (countRowSpan(F1_1, feuilles) + countRowSpan(F1_2, feuilles) + 1).ToString());
            xF1td1_r2.SetAttributeValue("type", "3");
            xF1tr1.Add(xF1td1_r2);

            // AJOUT
            XElement xF1td_vainq = new XElement("td");
            xF1td_vainq.SetAttributeValue("rowspan", (countRowSpan(F1_1, feuilles) + countRowSpan(F1_2, feuilles) + 1).ToString());
            xF1td_vainq.SetAttributeValue("type", "4");
            xF1td_vainq.Add(xF1combat1);
            xF1tr1.Add(xF1td_vainq);
            //FIN AJOUT


            XElement xF1tr3 = new XElement("tr");
            xF1table.Add(xF1tr3);

            int count_niveau = feuilles.Select(o => o.niveau).Distinct().Count();
            for (int i = 0; i < count_niveau; i++)
            {
                xF1tr3.Add(new XElement("td"));
                xF1tr3.Add(new XElement("td"));
            }

            ConstructFeuille(F1_1, feuilles, xF1table, xF1combat1);


            XElement xF1tr2 = new XElement("tr");
            xF1table.Add(xF1tr2);

            XElement xF1td2_r = new XElement("td");
            xF1td2_r.SetAttributeValue("rowspan", countRowSpan(F1_2, feuilles).ToString());
            if (F1_2 != null)
            {
                xF1td2_r.SetAttributeValue("type", "2");
                xF1tr2.Add(xF1td2_r);
            }

            XElement xF1td2 = new XElement("td");
            xF1td2.SetAttributeValue("rowspan", countRowSpan(F1_2, feuilles).ToString());
            xF1td2.SetAttributeValue("type", "1");
            xF1tr2.Add(xF1td2);
            XElement xF1combat2 = new XElement("combat");
            xF1td2.Add(xF1combat2);
            xF1combat2.SetAttributeValue("reference", F1.reference.ToString());
            xF1combat2.SetAttributeValue("judoka", "2");


            ConstructFeuille(F1_2, feuilles, xF1table, xF1combat2);



            Feuille F2 = feuilles.FirstOrDefault(o => o.reference == "2.1.2");

            Feuille F2_1 = feuilles.FirstOrDefault(o => o.reference == F2.ref1);
            Feuille F2_2 = feuilles.FirstOrDefault(o => o.reference == F2.ref2);

            XElement xF2table = new XElement("table");
            xphase.Add(xF2table);

            XElement xF2tr1 = new XElement("tr");
            xF2table.Add(xF2tr1);

            XElement xF2td1_r = new XElement("td");
            xF2td1_r.SetAttributeValue("rowspan", countRowSpan(F2_1, feuilles).ToString());
            xF2td1_r.SetAttributeValue("type", "2");
            xF2tr1.Add(xF2td1_r);

            XElement xF2td1 = new XElement("td");
            xF2td1.SetAttributeValue("rowspan", countRowSpan(F2_1, feuilles).ToString());
            xF2td1.SetAttributeValue("type", "1");
            xF2tr1.Add(xF2td1);
            XElement xF2combat1 = new XElement("combat");
            xF2td1.Add(xF2combat1);
            xF2combat1.SetAttributeValue("reference", F2.reference.ToString());
            xF2combat1.SetAttributeValue("judoka", "1");

            XElement xF2td1_r2 = new XElement("td");
            xF2td1_r2.SetAttributeValue("rowspan", (countRowSpan(F2_1, feuilles) + countRowSpan(F2_2, feuilles) + 1).ToString());
            xF2td1_r2.SetAttributeValue("type", "3");
            xF2tr1.Add(xF2td1_r2);


            // AJOUT
            XElement xF2td_vainq = new XElement("td");
            xF2td_vainq.SetAttributeValue("rowspan", (countRowSpan(F2_1, feuilles) + countRowSpan(F2_2, feuilles) + 1).ToString());
            xF2td_vainq.SetAttributeValue("type", "4");
            xF2td_vainq.Add(xF2combat1);
            xF2tr1.Add(xF2td_vainq);
            // FIN AJOUT

            XElement xF2tr3 = new XElement("tr");
            xF2table.Add(xF2tr3);

            for (int i = 0; i < count_niveau; i++)
            {
                xF2tr3.Add(new XElement("td"));
                xF2tr3.Add(new XElement("td"));
            }

            ConstructFeuille(F2_1, feuilles, xF2table, xF2combat1);


            XElement xF2tr2 = new XElement("tr");
            xF2table.Add(xF2tr2);

            XElement xF2td2_r = new XElement("td");
            xF2td2_r.SetAttributeValue("rowspan", countRowSpan(F2_2, feuilles).ToString());
            if (F2_2 != null)
            {
                xF2td2_r.SetAttributeValue("type", "2");
                xF2tr2.Add(xF2td2_r);
            }

            XElement xF2td2 = new XElement("td");
            xF2td2.SetAttributeValue("rowspan", countRowSpan(F2_2, feuilles).ToString());
            xF2td2.SetAttributeValue("type", "1");
            xF2tr2.Add(xF2td2);
            XElement xF2combat2 = new XElement("combat");
            xF2td2.Add(xF2combat2);
            xF2combat2.SetAttributeValue("reference", F2.reference.ToString());
            xF2combat2.SetAttributeValue("judoka", "2");


            ConstructFeuille(F2_2, feuilles, xF2table, xF2combat2);

            return doc.ToXmlDocument();
        }
        */

        // TODO Not used
        /*
        private static void ConstructFeuille(Feuille feuille, ICollection<Feuille> feuilles, XElement xtable, XElement xcombat)
        {
            if (feuille == null)
            {
                string reference = xcombat.Attribute("reference").Value;
                Feuille feuille2 = feuilles.FirstOrDefault(o => o.reference == reference);

                int count_niveau = feuilles.Where(o => o.niveau > feuille2.niveau).Select(o => o.niveau).Distinct().Count();
                XElement td = xcombat.Parent;
                for (int i = 0; i < count_niveau; i++)
                {
                    td.AddBeforeSelf(new XElement("td"));
                    td.AddBeforeSelf(new XElement("td"));
                }
                return;
            }

            XElement xtd_bis = xcombat.Parent;

            Feuille F1_2 = feuilles.FirstOrDefault(o => o.reference == feuille.ref2);
            XElement xtr2 = new XElement("tr");
            xtd_bis.Parent.AddAfterSelf(xtr2);

            XElement xtd2_r = new XElement("td");

            xtd2_r.SetAttributeValue("rowspan", countRowSpan(F1_2, feuilles).ToString());
            if (F1_2 != null)
            {
                Feuille F1_2_1 = feuilles.FirstOrDefault(o => o.reference == F1_2.ref1);
                Feuille F1_2_2 = feuilles.FirstOrDefault(o => o.reference == F1_2.ref2);
                if (F1_2_2 == null && F1_2_1 != null)
                {
                    xtd2_r.SetAttributeValue("type", "5");
                }
                else
                {
                    xtd2_r.SetAttributeValue("type", "2");
                }
                xtr2.Add(xtd2_r);
            }

            XElement xtd2 = new XElement("td");
            xtd2.SetAttributeValue("rowspan", countRowSpan(F1_2, feuilles).ToString());
            xtd2.SetAttributeValue("type", "1");
            xtr2.Add(xtd2);
            XElement xcombat2 = new XElement("combat");
            xtd2.Add(xcombat2);
            xcombat2.SetAttributeValue("reference", feuille.reference.ToString());
            xcombat2.SetAttributeValue("judoka", "2");
            ConstructFeuille(F1_2, feuilles, xtable, xcombat2);


            Feuille F1_1 = feuilles.FirstOrDefault(o => o.reference == feuille.ref1);
            XElement xtd1 = new XElement("td");
            xtd1.SetAttributeValue("rowspan", countRowSpan(F1_1, feuilles).ToString());
            xtd1.SetAttributeValue("type", "1");
            xtd_bis.Parent.AddFirst(xtd1);
            XElement xcombat1 = new XElement("combat");
            xtd1.Add(xcombat1);
            xcombat1.SetAttributeValue("reference", feuille.reference.ToString());
            xcombat1.SetAttributeValue("judoka", "1");

            XElement xtd1_r = new XElement("td");
            xtd1_r.SetAttributeValue("rowspan", countRowSpan(F1_1, feuilles).ToString());

            if (F1_1 != null)
            {
                Feuille F1_1_1 = feuilles.FirstOrDefault(o => o.reference == F1_1.ref1);
                Feuille F1_1_2 = feuilles.FirstOrDefault(o => o.reference == F1_1.ref2);
                if (F1_1_2 == null && F1_1_1 != null)
                {
                    xtd1_r.SetAttributeValue("type", "5");
                }
                else
                {
                    xtd1_r.SetAttributeValue("type", "2");
                }
                //xtd1_r.SetAttributeValue("type", "2");
                xtd_bis.Parent.AddFirst(xtd1_r);
            }

            ConstructFeuille(F1_1, feuilles, xtable, xcombat1);



        }
        */

        // TODO Not used
        /*
        private static int countRowSpan(Feuille feuille, ICollection<Feuille> feuilles)
        {

            if (feuille == null)
            {
                return 1;
            }

            Feuille feuille1 = feuilles.FirstOrDefault(o => o.reference == feuille.ref1);
            Feuille feuille2 = feuilles.FirstOrDefault(o => o.reference == feuille.ref2);
            return countRowSpan(feuille1, feuilles) + countRowSpan(feuille2, feuilles);
        }
        */
    }
}
