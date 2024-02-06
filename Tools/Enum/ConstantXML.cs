namespace Tools.Enum
{
    /// <summary>
    /// Enumération des constants pour la construction des fichiers XML
    /// </summary>
    public class ConstantXML
    {
        public const string ServerJudo = "serverjudo";
        public const string Command = "command";
        public const string Valeur = "valeur";
        public const string Tapis = "tapis";
        public const string Competitions = "competitions";
        public const string Transmis = "transmis";
        public const string NonTransmis = "nontransmis";
        public const string Date = "date";
        public const string Classement = "classement";

        public const string Machine = "machine";
        public const string User = "username";
        public const string Address = "address";
        public const string Port = "port";
        public const string AddressSite = "addresssite";
        public const string PortSite = "portsite";
        public const string Image = "image";
        public const string Type = "type";

        public const string JudoTV_Address = "address";
        public const string JudoTV_Port = "port";
        public const string JudoTV_Envoie1 = "judotven1";
        public const string JudoTV_Envoie2 = "judotven2";

        public const string Directory = "directory";
        public const string FileName = "filename";



        public const string Event_ID = "ID";
        public const string Event_Nom = "libelle";
        public const string Event_Date = "date";

        public const string Event_erreur_0 = "erreur_0";
        public const string Event_erreur_1 = "erreur_1";
        public const string Event_erreur_2 = "erreur_2";
        public const string Event_erreur_3 = "erreur_3";

        public const string Logos = "logos";
        public const string LogoFede = "logo_fede";
        public const string LogoLigue = "logo_ligue";
        public const string LogoSponsor = "logo_sponsor";
        public const string Logo_Valeur = "valeur";
        public const string Logo_Nom = "nom";

        public const string publierProchainsCombats = "PublierProchainsCombats";
        public const string publierAffectationTapis = "PublierAffectationTapis";
        public const string delaiActualisationClientSec = "DelaiActualisationClientSec";
        public const string nbProchainsCombats = "NbProchainsCombats";
        public const string msgProchainsCombats = "MsgProchainsCombats";
        public const string DateGeneration = "DateGeneration";
        public const string Logo = "Logo";

        public const string checksums = "Checksums";
        public const string checksumFile = "ChecksumFile";
        public const string checksumFile_fichier = "fichier";
        public const string checksumFile_checksum = "checksum";

        public const string TapisEpreuve = "TapisEpreuve";
        public const string Tapis_No = "no_tapis";

        //Structure 
        public const string Structure_ID = "ID";
        public const string Structure_RemoteID = "remoteID";
        public const string Structure_Nom = "nom";

        //Compétition 
        public const string Competition = "competition";
        public const string Competition_ID = "ID";
        public const string Competition_ID2 = "id";
        public const string Competition_RemoteID = "RemoteID";
        public const string Competition_Date = "date";
        public const string Competition_Titre = "titre";
        public const string Competition_Lieu = "lieu";
        public const string Competition_Information = "information";
        public const string Competition_Type = "type";
        public const string Competition_Type2 = "type2";
        public const string Competition_Discipline = "discipline";
        public const string Competition_Couleur1 = "couleur1";
        public const string Competition_Couleur2 = "couleur2";
        public const string Competition_Niveau = "niveau";
        public const string Competition_Version = "version";
        public const string Competition_Tapis = "tapis";
        public const string Competition_AfficheCSA = "afficheCSA";
        public const string Competition_AfficheKinzas = "kinzas";
        public const string Competition_AfficheAnimationVainqueur = "animationVainqueur";
        public const string Competition_RandomCombat = "randomCombat";
        public const string Competition_TempsPause = "tempsPause";
        public const string Competition_TempsMedical = "tempsMedical";

        //Club 
        public const string Club = "club";
        public const string Clubs = "clubs";
        public const string Club_ID = "ID";
        public const string Club_ID2 = "id";
        public const string Club_NomCourt = "nomCourt";
        public const string Club_Nom = "nom";
        public const string Club_Comite = "comite";
        public const string Club_Ligue = "ligue";

        //Comite 
        public const string Comites = "comites";
        public const string Comite = "comite";
        public const string Comite_ID = "ID";
        public const string Comite_Nom = "nom";
        public const string Comite_Ligue = "ligue";
        public const string Comite_RemoteID = "remoteID";
        public const string Comite_Secteur = "secteur";

        //Secteurs 
        public const string Secteurs = "secteurs";
        public const string Secteur = "secteur";
        public const string Secteur_ID = "ID";
        public const string Secteur_Nom = "nom";
        public const string Secteur_NomCourt = "nomCourt";

        //Ligue 
        public const string Ligues = "ligues";
        public const string Ligue = "ligue";
        public const string Ligue_ID = "ID";
        public const string Ligue_Nom = "nom";
        public const string Ligue_NomCourt = "nomCourt";
        public const string Ligue_RemoteID = "remoteID";

        //Pays 
        public const string Pays1 = "pays1";
        public const string Pays = "pays";
        public const string Pays_ID = "ID";
        public const string Pays_Code = "code";
        public const string Pays_Nom = "nom";
        public const string Pays_Abr2 = "abr2";
        public const string Pays_Abr3 = "abr3";
        public const string Pays_AbrF = "abrF";

        //Combats 
        public const string Combats = "combats";
        public const string Combat = "combat";
        public const string Combat_ID = "id";
        public const string Combat_Numero = "numero";
        public const string Combat_Reference = "reference";
        public const string Combat_Groupe = "groupe";
        public const string Combat_Niveau = "niveau";
        public const string Combat_Temps = "temps";
        public const string Combat_Date_debut = "date_debut";
        public const string Combat_Time_debut = "time_debut";
        public const string Combat_Date_fin = "date_fin";
        public const string Combat_Time_fin = "time_fin";
        public const string Combat_Date_programmation = "date_programmation";
        public const string Combat_Time_programmation = "time_programmation";
        public const string Combat_Vainqueur = "vainqueur";
        public const string Combat_Repechage = "repechage";
        public const string Combat_Score = "score";
        public const string Combat_Kinza = "kinza";
        public const string Combat_Kinza1 = "kinza1";
        public const string Combat_Kinza2 = "kinza2";
        public const string Combat_Judoka = "judoka";
        public const string Combat_Penalite = "penalite";
        public const string Combat_Points = "points";
        public const string Combat_PointsGRCH = "pointsGRCH";
        public const string Combat_PointsGRCH1 = "pointsGRCH1";
        public const string Combat_PointsGRCH2 = "pointsGRCH2";
        public const string Combat_Phase = "phase";
        public const string Combat_Detail = "detail";
        public const string Combat_Challenge1Refused = "challenge1Refused";
        public const string Combat_Challenge2Refused = "challenge2Refused";
        public const string Combat_ScoresJujitsu = "scoresJujitsu";
        public const string Combat_PremiereCategoriePoids = "premiereCategoriePoids";


        public const string Combat_NbVictoire = "nbvictoire";

        public const string Combat_Tapis = "tapis";
        public const string Combat_Etat = "etat";
        public const string Combat_Arbitre1 = "arbitre1";
        public const string Combat_Arbitre2 = "arbitre2";
        public const string Combat_Arbitre3 = "arbitre3";
        public const string Combat_Virtuel = "virtuel";
        public const string Combat_Epreuve = "epreuve";
        public const string Combat_TempsCombat = "tempsCombat";
        public const string Combat_TempsRecuperation = "tempsRecuperation";
        public const string Combat_TempsHippon = "tempsHippon";
        public const string Combat_TempsWazaAri = "tempsWazaAri";
        public const string Combat_TempsYuko = "tempsCombatYuko";
        public const string Combat_TempsGolden = "tempsGolden";
        public const string Combat_IsNewCombat = "isNewCombat";
        public const string Combat_TempsRecupFinal = "tempsRecupFinal";
        public const string Combat_Discipline = "discipline";


        public const string Combat_ScoreVainqueur = "scorevainqueur";
        public const string Combat_PenVainqueur = "penvainqueur";
        public const string Combat_ScorePerdant = "scoreperdant";
        public const string Combat_PenPerdant = "penperdant";
        public const string Combat_FirstRencontre = "firstrencontre";
        public const string Combat_FirstRencontreLib = "firstrencontrelib";

        public const string Combat_IsPlayable = "isPlayable";

        //Epreuves 
        public const string Epreuves = "epreuves";
        public const string Epreuve = "epreuve";
        public const string Epreuve_Categorie_Age = "categorieAge";
        public const string Epreuve_Categorie_Poids = "categoriePoids";
        public const string Epreuve_Nom = "nom";
        public const string Epreuve_Grade_Min = "gradeMin";
        public const string Epreuve_Grade_Max = "gradeMax";
        public const string Epreuve_ID = "ID";
        public const string Epreuve_RemoteID = "idRemoteEpreuve";
        public const string Epreuve_Sexe = "sexe";
        public const string Epreuve_Inscrits = "inscrits";
        public const string Epreuve_Competition = "competition";
        public const string Epreuve_AnneeMin = "anneeMin";
        public const string Epreuve_AnneeMax = "anneeMax";
        public const string Epreuve_PoidsMin = "poidsMin";
        public const string Epreuve_PoidsMax = "poidsMax";
        public const string Epreuve_EquipeEP = "equipeEP";

        public const string Epreuve_CateAge_Nom = "nom_cateage";
        public const string Epreuve_CateAge_RemoteId = "remoteId_cateage";

        public const string Epreuve_CatePoids_Nom = "nom_catepoids";
        public const string Epreuve_CatePoids_RemoteId = "remoteId_catepoids";

        //Epreuves_Equipe

        public const string Epreuve_Equipes = "epreuve_equipes";
        public const string Epreuve_Equipe = "epreuve_equipe";
        public const string Epreuve_Equipe_Categorie_Age = "categorieAge";
        public const string Epreuve_Equipe_Libelle = "libelle";
        public const string Epreuve_Equipe_GradeMin = "gradeMin";
        public const string Epreuve_Equipe_GradeMax = "gradeMax";
        public const string Epreuve_Equipe_ID = "id";
        public const string Epreuve_Equipe_Competition = "competition";
        public const string Epreuve_Equipe_Debut = "debut";
        public const string Epreuve_Equipe_Fin = "fin";
        public const string Epreuve_Equipe_RemoteID = "remoteID";
        public const string Epreuve_Equipe_AnneeMin = "anneeMin";
        public const string Epreuve_Equipe_AnneeMax = "anneeMax";
        public const string Epreuve_Equipe_EpreuveRef = "epreuveRef";
        

        //Groupe PhaseDecoupage 
        public const string PhaseDecoupages = "phasedecoupages";
        public const string PhaseDecoupage = "phasedecoupage";
        public const string PhaseDecoupage_ID = "id";
        public const string PhaseDecoupage_Phase = "phase";
        public const string PhaseDecoupage_Finales = "finales";
        public const string PhaseDecoupage_Tableau = "tableau";
        public const string PhaseDecoupage_Poule = "poule";

        //Groupe Combats 
        public const string Groupes = "groupes";
        public const string Groupe = "groupe";
        public const string Groupe_ID = "id";
        public const string Groupe_Tapis = "tapis";
        public const string Groupe_Libelle = "libelle";
        public const string Groupe_Horaire_Debut_Date = "horaire_debut_date";
        public const string Groupe_Horaire_Debut_Time = "horaire_debut_time";
        public const string Groupe_Horaire_Fin_Date = "horaire_fin_date";
        public const string Groupe_Horaire_Fin_Time = "horaire_fin_time";
        public const string Groupe_Verrouille = "verrouille";
        public const string Groupe_Decoupage = "decoupage";


        //judoka 
        public const string Judokas = "judokas";
        public const string Judoka = "judoka";
        public const string Judoka_ID = "ID";
        public const string Judoka_RemoteID = "remoteID";
        public const string Judoka_Licence = "licence";
        public const string Judoka_Nom = "nom";
        public const string Judoka_Prenom = "prenom";
        public const string Judoka_Sexe = "sexe";
        public const string Judoka_Naissance = "naissance";
        public const string Judoka_Pays = "pays";
        public const string Judoka_Club = "club";
        public const string Judoka_Grade = "grade";
        public const string Judoka_Poids = "poids";
        public const string Judoka_PoidsM = "poidsM";
        public const string Judoka_Categorie = "categorie";
        public const string Judoka_Present = "present";
        public const string Judoka_Etat = "etat";
        public const string Judoka_Modification = "modification";
        public const string Judoka_DatePesee_Date = "date_datePesee";
        public const string Judoka_DatePesee_Time = "time_datePesee";
        public const string Judoka_Passeport = "passeport";
        public const string Judoka_ModeControle = "modeControle";
        public const string Judoka_ModePesee = "modePesee";
        public const string Judoka_Ajoute = "ajoute";
        public const string Judoka_QualifieE0 = "qualifieE0";
        public const string Judoka_QualifieE1 = "qualifieE1";
        public const string Judoka_Equipe = "idEquipe";
        public const string Judoka_Points = "corg";
        public const string Judoka_Serie = "serie";
        public const string Judoka_Serie2 = "serie2";
        public const string Judoka_Observation = "observation";
        public const string Judoka_CatePoids_RemoteId = "remoteId_catepoids";


        //participant 
        public const string Participants = "participants";
        public const string Participant = "participant";
        public const string Participant_ID = "ID";
        public const string Participant_RemoteID = "remoteID";
        public const string Participant_Judoka = "judoka";
        public const string Participant_Licence = "licence";
        public const string Participant_Nom = "nom";
        public const string Participant_Prenom = "prenom";
        public const string Participant_Sexe = "sexe";
        public const string Participant_Naissance = "naissance";
        public const string Participant_Pays = "pays";
        public const string Participant_Club = "club";
        public const string Participant_Grade = "grade";
        public const string Participant_Categorie = "categorie";
        public const string Participant_Poids = "poids";
        public const string Participant_PoidsM = "poidsM";
        public const string Participant_Present = "present";
        public const string Participant_Ranking = "ranking";
        public const string Participant_ClassementAvant = "classementAvant";
        public const string Participant_ClassementFinal = "classementFinal";
        public const string Participant_Qualifie = "qualifie";
        public const string Participant_Position = "position";
        public const string Participant_OrdreTirage = "ordreTirage";
        public const string Participant_NbVictoires = "nbVictoires";
        public const string Participant_NbVictoiresInd = "nbVictoiresInd";
        public const string Participant_CumulPoints = "cumulPoints";
        public const string Participant_CumulPointsGRCH = "cumulPointsGRCH";
        public const string Participant_Poule = "poule";
        public const string Participant_Phase = "phase";
        public const string Participant_PositionOriginal = "positionOriginal";
        public const string Participant_DernierCombat = "dernierCombat";
        public const string Participant_QualifieE1 = "qualifieE1";
        public const string Participant_ScoreProLeague = "scoreProLeague";
        public const string Participant_NbPenalites = "nbPenalites";



        //Phase
        public const string Phases = "phases";
        public const string Phase = "phase";
        public const string Phase_ID = "id";
        public const string Phase_Libelle = "libelle";
        public const string Phase_Etat = "etat";
        public const string Phase_TypePhase = "typePhase";
        public const string Phase_NiveauRepechage = "niveauRepechage";
        public const string Phase_Bresilien = "bresilien";
        public const string Phase_NiveauRepeches = "niveauRepeches";
        public const string Phase_NbPoules = "nbPoules";
        public const string Phase_NbCombatsTotal = "nbCombatsTotal";
        public const string Phase_NbCombatsFinalistes = "nbCombatsFinalistes";
        public const string Phase_NbQualifiesComplet = "nbQualifiesComplet";
        public const string Phase_NbQualifiesIncomplet = "nbQualifiesIncomplet";
        public const string Phase_NbJudokaPoule = "nbJudokaPoule";
        public const string Phase_NbJudoka = "nbJudoka";
        public const string Phase_Suivant = "suivant";
        public const string Phase_Precedent = "precedent";
        public const string Phase_Epreuve = "epreuve";
        public const string Phase_IsEquipe = "isequipe";
        public const string Phase_Barrage3 = "barrage3";
        public const string Phase_Barrage5 = "barrage5";
        public const string Phase_Barrage7 = "barrage7";
        public const string Phase_Ecartement = "ecartement";
        public const string Phase_Date_Tirage = "date_tirage";
        public const string Phase_Time_Tirage = "time_tirage";


        //Feuilles 
        public const string Feuille = "feuille";
        public const string Feuille_ID = "id";
        public const string Feuille_Numero = "numero";
        public const string Feuille_Repechage = "repechage";
        public const string Feuille_Source1 = "source1";
        public const string Feuille_Source2 = "source2";
        public const string Feuille_Reference = "reference";
        public const string Feuille_Ref1 = "ref1";
        public const string Feuille_Ref2 = "ref2";
        public const string Feuille_Ordre = "ordre";
        public const string Feuille_Niveau = "niveau";
        public const string Feuille_Combat = "combat";
        public const string Feuille_TypeSource = "typeSource";
        public const string Feuille_Pere = "pere";
        public const string Feuille_Classement1 = "classement1";
        public const string Feuille_Classement2 = "classement2";
        public const string Feuille_Phase = "phase";

        //Poule
        public const string Poules = "poules";
        public const string Poule = "poule";
        public const string Poule_ID = "id";
        public const string Poule_Numero = "numero";
        public const string Poule_Nom = "nom";
        public const string Poule_Phase = "phase";

        //Vue Epreuves 
        public const string Vue_Epreuve = "epreuve";
        public const string Vue_Epreuve_ID = "id";
        public const string Vue_Epreuve_Nom = "nom";
        public const string Vue_Epreuve_Nom_CategorieAge = "nom_cateage";
        public const string Vue_Epreuve_Nom_CategoriePoids = "nom_catepoids";
        public const string Vue_Epreuve_Nom_Competition = "nom_competition";
        public const string Vue_Epreuve_PoidsMin_CategoriePoids = "poidsmin_catepoids";
        public const string Vue_Epreuve_PoidsMax_CategoriePoids = "poidsmax_catepoids";
        public const string Vue_Epreuve_Categorie_Age = "categorieAge";
        public const string Vue_Epreuve_Categorie_Poids = "categoriePoids";
        public const string Vue_Epreuve_PoidsMin = "poidsMin";
        public const string Vue_Epreuve_PoidsMax = "poidsMax";
        public const string Vue_Epreuve_GradeMin = "ceintureMin";
        public const string Vue_Epreuve_GradeMax = "ceintureMax";
        public const string Vue_Epreuve_AnneeMin = "anneeMin";
        public const string Vue_Epreuve_AnneeMax = "anneeMax";

        //Vue Epreuves 
        public const string EpreuveJudokas = "epreuvejudokas";
        public const string EpreuveJudoka = "epreuvejudoka";
        public const string EpreuveJudoka_ID = "id";
        public const string EpreuveJudoka_Judoka = "judoka";
        public const string EpreuveJudoka_Epreuve = "epreuve";
        public const string EpreuveJudoka_Classement = "classement";
        public const string EpreuveJudoka_Serie = "serie";
        public const string EpreuveJudoka_Serie2 = "serie2";
        public const string EpreuveJudoka_Etat = "etat";
        public const string EpreuveJudoka_Observation = "observation";
        public const string EpreuveJudoka_Points = "points";

        //vue judoka 
        public const string Vue_Judoka = "judoka";
        public const string Vue_Judoka_ID = "id";
        public const string Vue_Judoka_Licence = "licence";
        public const string Vue_Judoka_Nom = "nom";
        public const string Vue_Judoka_Prenom = "prenom";
        public const string Vue_Judoka_Sexe = "lib_sexe";
        public const string Vue_Judoka_Naissance = "naissance";

        public const string Vue_Judoka_CeintureID = "ceinture_id";
        public const string Vue_Judoka_CeintureNom = "ceinture_nom";
        public const string Vue_Judoka_CeintureCouleur1 = "ceinture_couleur1";
        public const string Vue_Judoka_CeintureCouleur2 = "ceinture_couleur2";
        public const string Vue_Judoka_Serie = "serie";
        public const string Vue_Judoka_Serie2 = "serie2";
        public const string Vue_Judoka_Observation = "observation";
        public const string Vue_Judoka_Points = "points";
        public const string Vue_Judoka_PoidsMesure = "poidsMesure";


        public const string Vue_Judoka_Present = "present";
        public const string Vue_Judoka_NomCategorieAge = "nomCategorieAge";

        public const string Vue_Judoka_Club = "club";
        public const string Vue_Judoka_ClubNomCourt = "clubnomcourt";
        public const string Vue_Judoka_ClubNom = "clubnom";

        public const string Vue_Judoka_ComiteNomCourt = "comitenomcourt";
        public const string Vue_Judoka_ComiteNom = "comitenom";

        public const string Vue_Judoka_Ligue = "ligue";
        public const string Vue_Judoka_LigueNomCourt = "liguenomcourt";
        public const string Vue_Judoka_LigueNom = "liguenom";

        public const string Vue_Judoka_IdEpreuve = "idepreuve";
        public const string Vue_Judoka_LibEpreuve = "libepreuve";

        public const string Vue_Judoka_Etat = "etat";
        public const string Vue_Judoka_Qualifie0 = "qualifie0";
        public const string Vue_Judoka_Qualifie1 = "qualifie1";


        //Ceinture
        public const string Ceintures = "ceintures";
        public const string Ceinture = "ceinture";
        public const string Ceinture_id = "id";
        public const string Ceinture_nom = "nom";
        public const string Ceinture_ordre = "ordre";
        public const string Ceinture_remoteId = "remoteId";
        public const string Ceinture_couleur1 = "couleur1";
        public const string Ceinture_couleur2 = "couleur2";

        //CateAge
        public const string CateAges = "categorieAges";
        public const string CateAge = "categorieAge";
        public const string CateAge_id = "id";
        public const string CateAge_nom = "nom";
        public const string CateAge_ordre = "ordre";
        public const string CateAge_remoteId = "remoteId";
        public const string CateAge_anneeMin = "anneeMin";
        public const string CateAge_anneeMax = "anneeMax";

        //CatePoids
        public const string CatesPoids = "categoriesPoids";
        public const string CatePoids = "categoriePoids";
        public const string CatePoids_id = "id";
        public const string CatePoids_nom = "nom";
        public const string CatePoids_ordre = "ordre";
        public const string CatePoids_remoteId = "remoteId";
        public const string CatePoids_poidsMin = "poidsMin";
        public const string CatePoids_poidsMax = "poidsMax";
        public const string CatePoids_sexe = "sexe";
        public const string CatePoids_cateage = "categorieAge";
        public const string CatePoids_equipe = "equipe";
        public const string CatePoids_discipline = "discipline";

        //Gestion temps
        public const string GestionsTemps = "gestionsTemps";
        public const string GestionTemps = "gestionTemps";
        public const string GestionTemps_CateAge = "cateAge";
        public const string GestionTemps_Sexe = "sexe";
        public const string GestionTemps_TempsCombat = "tempsCombats";
        public const string GestionTemps_TempsRecup = "tempsRecup";
        public const string GestionTemps_TempsRecupFinal = "tempsRecupFinal";
        public const string GestionTemps_TempsHippon = "tempsHippon";
        public const string GestionTemps_TempsWaza = "tempsWaza";
        public const string GestionTemps_TempsYuko = "tempsYuko";
        public const string GestionTemps_TempsGolden = "tempsGolden";
        public const string GestionTemps_Discipline = "discipline";

        //Arbitre 
        public const string Arbitres = "arbitres";
        public const string Arbitre = "arbitre";
        public const string Arbitre_ID = "ID";
        public const string Arbitre_Licence = "licence";
        public const string Arbitre_Nom = "nom";
        public const string Arbitre_Prenom = "prenom";
        public const string Arbitre_Naissance = "naissance";
        public const string Arbitre_Sexe = "sexe";
        public const string Arbitre_Modification = "modification";
        public const string Arbitre_RemoteID = "remoteID";
        public const string Arbitre_Club = "club";
        public const string Arbitre_Comite = "comite";
        public const string Arbitre_Ligue = "ligue";
        public const string Arbitre_Niveau = "niveau";
        public const string Arbitre_EstResponsable = "responsable";
        public const string Arbitre_Present = "present";

        //Commissaire 
        public const string Commissaires = "commissaires";
        public const string Commissaire = "commissaire";
        public const string Commissaire_ID = "ID";
        public const string Commissaire_Licence = "licence";
        public const string Commissaire_Nom = "nom";
        public const string Commissaire_Prenom = "prenom";
        public const string Commissaire_Naissance = "naissance";
        public const string Commissaire_Sexe = "sexe";
        public const string Commissaire_Modification = "modification";
        public const string Commissaire_RemoteID = "remoteID";
        public const string Commissaire_Club = "club";
        public const string Commissaire_Comite = "comite";
        public const string Commissaire_Ligue = "ligue";
        public const string Commissaire_Niveau = "niveau";
        public const string Commissaire_EstResponsable = "responsable";
        public const string Commissaire_Present = "present";


        //Recap 
        public const string Recaps = "recaps";
        public const string Recap = "recap";
        public const string Recap_Grade = "grade";

        //Delegues
        public const string Delegues = "delegues";
        public const string Delegue = "delegue";
        public const string Delegue_ID = "id";
        public const string Delegue_Nom = "nom";
        public const string Delegue_Prenom = "prenom";
        public const string Delegue_Telephone = "telephone";
        public const string Delegue_Mail = "mail";
        public const string Delegue_Fonction = "fonction";
        public const string Delegue_Commentaire = "commentaire";


        //Equipe

        public const string Equipes = "equipes";
        public const string Equipe = "equipe";
        public const string Equipe_Id = "id";
        public const string Equipe_Nom = "nom";
        public const string Equipe_Club = "club";
        public const string Equipe_Comite = "comite";
        public const string Equipe_Ligue = "ligue";
        public const string Equipe_Pays = "pays";
        public const string Equipe_Sexe = "sexe";
        public const string Equipe_RemoteId = "remoteIds";
        public const string Equipe_Epreuve = "epreuve";

        //Rencontre 

        public const string Rencontres = "rencontres";
        public const string Rencontre = "rencontre";
        public const string Rencontre_Id = "id";
        public const string Rencontre_Judoka1 = "judoka1";
        public const string Rencontre_Judoka2 = "judoka2";
        public const string Rencontre_Score1 = "score1";
        public const string Rencontre_Score2 = "score2";
        public const string Rencontre_Penalite1 = "penalite1";
        public const string Rencontre_Penalite2 = "penalite2";
        public const string Rencontre_Details = "details";
        public const string Rencontre_Date_Programmation = "date_programmation";
        public const string Rencontre_Date_Debut = "date_debut";
        public const string Rencontre_Date_Fin = "date_fin";
        public const string Rencontre_Time_Programmation = "time_programmation";
        public const string Rencontre_Time_Debut = "time_debut";
        public const string Rencontre_Time_Fin = "time_fin";
        public const string Rencontre_Temps = "temps";
        public const string Rencontre_Etat = "etat";
        public const string Rencontre_EtatJ1 = "etatJ1";
        public const string Rencontre_EtatJ2 = "etatJ2";
        public const string Rencontre_Arbitre1 = "arbitre1";
        public const string Rencontre_Arbitre2 = "arbitre2";
        public const string Rencontre_Arbitre3 = "arbitre3";
        public const string Rencontre_Vainqueur = "vainqueur";
        public const string Rencontre_Combat = "combat";
        public const string Rencontre_CatePoids = "catePoids";
        public const string Rencontre_Ippon1 = "ippon1";
        public const string Rencontre_Ippon2 = "ippon2";
        public const string Rencontre_GoldenScore = "goldenScore";
        public const string Rencontre_IsNewRencontre = "isNewRencontre";
        

        public const string Rencontre_TempsCombat = "tempsCombat";
        public const string Rencontre_TempsRecuperation = "tempsRecuperation";
        public const string Rencontre_TempsHippon = "tempsHippon";
        public const string Rencontre_TempsWazaAri = "tempsWazaAri";
        public const string Rencontre_TempsYuko = "tempsCombatYuko";
        public const string Rencontre_TempsRecupFinal = "tempsRecupFinal";
        public const string Rencontre_Discipline = "discipline";

        //vue groupe 
        public const string Vue_Groupes = "groupes";
        public const string Vue_Groupe = "groupe";
        public const string Vue_Groupe_Id = "groupe_id";
        public const string Vue_Groupe_Tapis = "groupe_tapis";
        public const string Vue_Groupe_Libelle = "groupe_libelle";
        public const string Vue_Groupe_DebutDate = "groupe_debutDate";
        public const string Vue_Groupe_DebutTime = "groupe_debutTime";
        public const string Vue_Groupe_FinDate = "groupe_finDate";
        public const string Vue_Groupe_FinTime = "groupe_finTime";
        public const string Vue_Groupe_Verrouille = "groupe_verrouille";
        public const string Vue_Groupe_Restant = "nb_combats_restant";
        public const string Vue_Groupe_PhaseEtat = "phase_etat";
        public const string Vue_Groupe_PhaseLibelle = "phase_libelle";
        public const string Vue_Groupe_PhaseId = "phase_id";
        public const string Vue_Groupe_PhaseType = "phase_type";
        public const string Vue_Groupe_EpreuveId = "epreuve_id";
        public const string Vue_Groupe_EpreuveNom = "epreuve_nom";
        public const string Vue_Groupe_EpreuvePoidsMin = "epreuve_poidsMin";
        public const string Vue_Groupe_EpreuvePoidsMax = "epreuve_poidsMax";
        public const string Vue_Groupe_EpreuveLibsexe = "epreuve_libsexe";
    }
}
