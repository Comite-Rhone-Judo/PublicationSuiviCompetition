using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Publication;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AppPublication.Export
{
    public static class ExportXML
    {
        #region Constantes
        /// <summary>
        /// Seuil de sécurité pour le flush sur disque des engagements (Judokas + Combats).
        /// </summary>
        private const int kDocumentEngagementsFlushThreshold = 1200;
        
        /// Seuil de sécurité pour le flush sur disque d'une épreuve unique.
        /// </summary>
        private const int kDocumentEpreuveFlushThreshold = 800;

        /// <summary>
        /// Seuil de sécurité pour le flush sur disque d'une phase unique.
        /// </summary>
        private const int kDocumentPhaseFlushThreshold = 500;

        /// <summary>
        /// Seuil de sécurité pour le flush sur disque des feuilles de combat (nombre de combats).
        /// </summary>
        private const int kDocumentFeuilleCombatFlushThreshold = 1000;
        #endregion

        #region Generation XElement
        /// <summary>
        /// Retourne la liste des comites en XML
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static XElement GetComites(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            try
            {
                // On utilise AsEnumerable() pour lire les données en flux continu 
                // et on les transforme à la volée avec Select()
                return new XElement(ConstantXML.Comites,
                        DC.Structures.Comites
                         .AsEnumerable()
                         .Select(comite => comite.ToXml()));
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
                return new XElement(ConstantXML.Comites); // On retourne une liste vide en cas d'erreur, comme votre code d'origine
            }
        }

        /// <summary>
        /// Retourne la liste des ligues en XML
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static XElement GetLigues(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            try
            {
                // Lecture en flux et transformation directe via LINQ
                return new XElement(ConstantXML.Ligues,
                            DC.Structures.Ligues
                         .AsEnumerable()
                         .Select(ligue => ligue.ToXml()));
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
                return new XElement(ConstantXML.Ligues); // Retour sécurisé d'une liste vide en cas de plantage
            }
        }

        /// <summary>
        /// Retourne la liste des secteurs en XML
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static XElement GetSecteurs(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            try
            {
                // On stream la lecture et on convertit à la volée
                return new XElement(ConstantXML.Secteurs,
                        DC.Structures.Secteurs
                         .AsEnumerable()
                         .Select(secteur => secteur.ToXml()));
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
                return new XElement(ConstantXML.Secteurs); // En cas d'erreur, on renvoie une liste vide pour éviter un NullReferenceException plus haut
            }
        }

        /// <summary>
        /// Retourne la liste des pays en XML
        /// </summary>
        /// <param name="ctx">Contexte en lecture seule pour l'export</param>
        /// <returns></returns>
        public static XElement GetPays(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            try
            {
                return new XElement(ConstantXML.LesPays,
                            DC.Structures.LesPays
                                 .AsEnumerable()
                                 .Select(pays => pays.ToXml()));
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
                return new XElement(ConstantXML.LesPays);
            }
        }


        /// <summary>
        /// Genere la liste des clubs en XML
        /// </summary>
        /// <param name="ctx">Contexte en lecture seule pour l'export</param>
        /// <returns></returns>
        public static XElement GetClubs(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            try
            {
                // 1. On prépare la requête pour les clubs ayant des judokas
                var clubsJudokas = (from c in DC.Structures.Clubs
                                    join j in DC.Participants.Vuejudokas on c.id equals j.club
                                    select c).Distinct();

                // 2. On prépare la requête pour les clubs ayant des équipes
                var clubsEquipes = (from c in DC.Structures.Clubs
                                    join eq in DC.Participants.Equipes on c.id equals eq.club
                                    select c).Distinct();

                // 3. On fusionne (Union gère l'élimination des doublons entre les deux groupes), 
                // on exécute la requête, et on transforme en XML à la volée.
                return new XElement(ConstantXML.Clubs,
                    clubsJudokas
                         .Union(clubsEquipes)
                         .AsEnumerable()
                         .Select(club => club.ToXml()));
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
                return new XElement(ConstantXML.Clubs);
            }
        }

        /// <summary>
        /// Ajout des grades à un document XML
        /// </summary>
        /// <param name="ctx">Contexte en lecture seule pour l'export</param>
        public static XElement GetCeintures(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            try
            {
                return new XElement(ConstantXML.Ceintures,
                            DC.Categories.Grades
                            .AsEnumerable()
                            .Select(ceinture => ceinture.ToXml()));
            }
            catch (Exception ex)
            {
                LogTools.Logger.Debug(ex);
                return new XElement(ConstantXML.Ceintures);
            }
        }

        #endregion

        #region Generation Fichiers Checksum
        /// <summary>
        /// Exporte une liste de fichiers avec Checksum en un objet XML
        /// </summary>
        /// <param name="listFiles"></param>
        /// <returns></returns>
        public static XDocument ExportChecksumFichiers(List<FileWithChecksum> listFiles)
        {
            // On construit tout l'arbre en une seule instruction
            return new XDocument(
                new XElement(FileWithChecksum.checksums,
                    // .Select() transforme directement votre liste en séquence de XElement
                    listFiles.Select(fc => fc.ToXml())
                )
            );
        }

        /// <summary>
        /// Importe une liste de fichiers avec Checksum à partir d'un élément XML
        /// </summary>
        /// <param name="rootElem"></param>
        /// <returns></returns>
        public static List<FileWithChecksum> ImportChecksumFichiers(XElement rootElem)
        {
            // On prend les descendants, et on les transforme un par un en objets
            return rootElem.Descendants(FileWithChecksum.checksumFile)
                           .Select(xinfo =>
                           {
                               var fc = new FileWithChecksum();
                               fc.LoadXml(xinfo);
                               return fc;
                           })
                           .ToList(); // On matérialise la liste finale d'un coup
        }

        #endregion

        #region Generation Documents XML pour le site
        /// <summary>
        /// Creation du document pour l'index
        /// </summary>
        /// <param name="ctx">Contexte en lecture seule pour l'export</param>
        /// <returns>Tuple contenant le document XML et un indicateur si le document est volumineux</returns>
        public static (XDocument outDoc, bool isLargeDoc) CreateDocumentIndex(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            // On construit l'arbre entier, la racine et les enfants en une seule passe
            return (
                new XDocument(
                    new XElement(ConstantXML.DocRoot,
                        new XElement(ConstantXML.Competitions,
                            // On stream directement depuis la base vers les éléments XML
                            DC.Organisation.Competitions
                              .AsEnumerable()
                              .Select(competition => competition.ToXml())
                        )   
                    )
                ),
                false
            );
        }

        /// <summary>
        /// Création du menu (pour le site)
        /// </summary>
        /// <param name="ctx">Contexte en lecture seule pour l'export</param>
        /// <param name="siteStructure">Générateur d'URL pour le site</param>
        /// <returns>Tuple contenant le document XML et un indicateur si le document est volumineux</returns>
        public static (XDocument outDoc, bool isLargeDoc) CreateDocumentMenu(IReadOnlyExportContext ctx, SiteUrlGenerator siteStructure)
        {
            IJudoData DC = ctx.DataContext;
            IExtendedJudoData EDC = ctx.ExtendedDataContext;

            // 1. On charge UNIQUEMENT ce qui est nécessaire en mémoire pour éviter le N+1
            var phasesByEpreuve = DC.Deroulement.Phases.ToLookup(p => p.epreuve);

            // 2. Construction fonctionnelle globale de l'arbre
            return (new XDocument(
                new XElement(ConstantXML.DocRoot,
                    new XElement(ConstantXML.Competitions,

                       // Boucle principale transformée en projection LINQ
                       DC.Organisation.Competitions.AsEnumerable().Select(competition =>
                       {
                            // On récupère l'élément racine de la compétition
                            XElement xcompetition = competition.ToXml();

                            // --- A. Ajout des Tapis ---
                            // Enumerable.Range permet de remplacer une boucle "for" de manière élégante
                            xcompetition.Add(
                                Enumerable.Range(0, competition.nbTapis + 1)
                                          .Select(i => new XElement(ConstantXML.Tapis,
                                                           new XAttribute(ConstantXML.Tapis, i)))
                            );

                            // --- B. Ajout des Epreuves ---
                            IEnumerable<i_vue_epreuve_interface> epreuves_compet = competition.IsEquipe()
                                ? DC.Organisation.VueEpreuveEquipes.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>()
                                : DC.Organisation.VueEpreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>();

                            xcompetition.Add(
                                epreuves_compet
                                    // On filtre en utilisant la liste en MÉMOIRE (phasesInMem)
                                    // Utilisation de l'index ILookup (Recherche ultra-rapide)
                                    .Where(ep => phasesByEpreuve[ep.id].Any(o => o.etat > (int)EtatPhaseEnum.Cree))
                                    .Select(ep =>
                                    {
                                        string webPathEpreuve = siteStructure.GetRelativeUrlEpreuveFromCompetition(ep.id.ToString(), ep.nom);
                                        XElement xepreuve = ep.ToXml(DC);
                                        xepreuve.SetAttributeValue(ConstantXML.Directory, webPathEpreuve);

                                        // Ajout de la balise Phases avec les données EN MÉMOIRE
                                        // Utilisation de l'index ILookup pour l'extraction
                                        xepreuve.Add(new XElement(ConstantXML.Phases,
                                            phasesByEpreuve[ep.id].Select(phase => phase.ToXml())
                                            ));

                                        return xepreuve;
                                    })
                            );

                            // --- C. Ajout des Groupes d'engagements ---
                            // Sécurité : On vérifie que la compétition existe dans le dictionnaire avant d'y accéder
                            if (EDC.Engagement.TypesGroupes.TryGetValue(competition.id, out var typesGroupes))
                            {
                                xcompetition.Add(
                                    typesGroupes.Select(typeGroupe =>
                                        new XElement(ConstantXML.GroupeEngagements_groupes,
                                            new XAttribute(ConstantXML.GroupeEngagements_type, (int)typeGroupe),

                                            // On injecte les groupes directement à l'intérieur
                                            EDC.Engagement.GroupesEngages
                                                .Where(g => g.Competition == competition.id && g.Type == (int)typeGroupe)
                                                .Select(grp => grp.ToXml())
                                        )
                                    )
                                );
                            }

                            return xcompetition;
                        })
                    )
            )),
            // Le document structurel d'un menu ne saturera jamais la RAM
            false);
        }


        /// <summary>
        /// Creation du document pour les engagements (pour le site)
        /// </summary>
        /// <param name="ctx">Contexte en lecture seule pour l'export</param>
        /// <returns>Tuple contenant le document XML et un indicateur si le document est volumineux</returns>
        public static (XDocument outDoc, bool isLargeDoc) CreateDocumentEngagements(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;
            IExtendedJudoData EDC = ctx.ExtendedDataContext;

            // 1. ÉVALUATION INSTANTANÉE O(1)
            // On estime le poids total de la compétition (Judokas + Combats)
            int nbJudokas = DC.Participants.Vuejudokas?.Count() ?? 0;
            int nbCombats = DC.Deroulement.Combats?.Count() ?? 0;
            bool isLargeDoc = (nbJudokas + nbCombats) > kDocumentEngagementsFlushThreshold;

            // 2. CRÉATION DES INDEX EN MÉMOIRE (O(1))
            var judokasByCompet = DC.Participants.Vuejudokas.ToLookup(vj => vj.idcompet);
            var epreuvesByCompet = DC.Organisation.Epreuves.ToLookup(ep => ep.competition);
            var phasesByEpreuve = DC.Deroulement.Phases.ToLookup(p => p.epreuve);
            var combatsByPhase = DC.Deroulement.Combats.ToLookup(c => c.phase);
            var groupesByCompet = EDC.Engagement.GroupesEngages.ToLookup(g => g.Competition);

            return (new XDocument(
                new XElement(ConstantXML.DocRoot,
                    new XElement(ConstantXML.Competitions,

                        // 1. On filtre DÈS LE DÉPART (on ne génère le XML que pour les compétitions valides)
                        DC.Organisation.Competitions
                          .Where(c => c.IsShiai() || c.IsIndividuelle())
                          .Select(competition =>
                          {
                              // On récupère la base de la compétition
                              XElement xcompetition = competition.ToXml();

                              // --- A. Ajout des Groupes ---
                              // TryGetValue protège contre une exception si la compétition n'a pas de types de groupes définis
                              if (EDC.Engagement.TypesGroupes.TryGetValue(competition.id, out var typesGroupes))
                              {
                                  xcompetition.Add(
                                      typesGroupes.Select(typeGroupe =>
                                          new XElement(ConstantXML.GroupeEngagements_groupes,
                                              new XAttribute(ConstantXML.GroupeEngagements_type, (int)typeGroupe),

                                              groupesByCompet[competition.id]
                                                  .Where(g => g.Type == (int)typeGroupe)
                                                  .Select(groupe => groupe.ToXml())
                                          )
                                      )
                                  );
                              }

                              // --- B. Ajout des Judokas ---
                              xcompetition.Add(
                                  new XElement(ConstantXML.GroupeEngagements_judokas,
                                        judokasByCompet[competition.id].Select(vj => vj.ToXml())
                                    )
                               );

                              // --- C. Ajout des Epreuves ---
                              // On matérialise la liste (ToList) car on va la réutiliser juste en dessous pour la jointure
                              var epreuves = epreuvesByCompet[competition.id].ToList();
                              xcompetition.Add(
                                  new XElement(ConstantXML.GroupeEngagements_epreuves,
                                        epreuves.Select(ep => ep.ToXml(DC))));

                              // --- D. Ajout des Phases ---
                              // On matérialise la liste des phases car on va la réutiliser pour la jointure des combats
                              var phases = epreuves.SelectMany(ep => phasesByEpreuve[ep.id]).ToList();
                              xcompetition.Add(
                                  new XElement(ConstantXML.Phases,
                                        phases.Select(ph => ph.ToXml())));

                              // --- E. Ajout des Combats ---
                              // On fait la jointure avec la liste en mémoire (phases), on dédoublonne, et on injecte.
                              var combats = phases.SelectMany(ph => combatsByPhase[ph.id]).Distinct(new CombatEqualityComparer());
                              xcompetition.Add(
                                   new XElement(ConstantXML.GroupeEngagements_combats,
                                        combats.Select(c => c.ToXml(DC))));

                              return xcompetition;
                          })
                    )
            )),
            isLargeDoc);
        }

        /// Document XML contenant les informations pour les generations des affectations de tapis
        /// </summary>
        /// <param name="ctx">Contexte en lecture seule pour l'export</param>
        /// <returns>Tuple contenant le document XML et un indicateur si le document est volumineux</returns>
        public static (XDocument outDoc, bool isLargeDoc) CreateDocumentAffectationTapis(IReadOnlyExportContext ctx)
        {
            IJudoData DC = ctx.DataContext;

            // 1. Création de l'index des phases (Fin de la boucle O(N²))
            var phasesByEpreuve = DC.Deroulement.Phases.ToLookup(p => p.epreuve);

            // Indexation globale des Tapis actifs (Fin du N+1)
            // Au lieu de requêter VueCombats pour CHAQUE épreuve dans la boucle, 
            // on extrait tous les tapis actifs de la compétition en UNE SEULE passe.
            var activeTapisByEpreuve = DC.Deroulement.VueCombats
                .Where(o => o.combat_tapis > 0
                         && o.phase_etat == (int)EtatPhaseEnum.TirageValide
                         && o.combat_vaiqueur == null)
                .Select(o => new { o.epreuve_id, o.combat_tapis })
                .Distinct()
                .ToLookup(x => x.epreuve_id, x => x.combat_tapis);

            // 2. Construction fonctionnelle globale
            return (new XDocument(
                new XElement(ConstantXML.DocRoot,
                    new XElement(ConstantXML.Competitions,
                        DC.Organisation.Competitions
                            .AsEnumerable()
                            .Select(competition =>
                            {
                                XElement xcompetition = competition.ToXml();

                                // --- A. Sélection des épreuves ---
                                IEnumerable<i_vue_epreuve_interface> epreuves_compet = competition.IsEquipe()
                                    ? DC.Organisation.VueEpreuveEquipes.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>()
                                    : DC.Organisation.VueEpreuves.Where(o => o.competition == competition.id).Cast<i_vue_epreuve_interface>();

                                // --- B. Ajout des épreuves et de leurs enfants ---
                                xcompetition.Add(
                                    epreuves_compet
                                        // Utilisation de l'index des phases
                                        .Where(ep => phasesByEpreuve[ep.id].Any(o => o.etat > (int)EtatPhaseEnum.Cree))
                                        .Select(ep =>
                                        {
                                            XElement xepreuve = ep.ToXml(DC);

                                            // Ajout des Phases (depuis la mémoire)
                                            xepreuve.Add(
                                                new XElement(ConstantXML.Phases,
                                                    phasesByEpreuve[ep.id].Select(phase => phase.ToXml())
                                                )
                                            );

                                            // Ajout des Tapis (uniquement pour les compétitions individuelles)
                                            if (competition.IsIndividuelle())
                                            {
                                                // Accès immédiat aux tapis via l'index
                                                var tapisEpreuve = activeTapisByEpreuve[ep.id].ToList();

                                                // S'il y a des tapis, on crée la balise et on les injecte
                                                if (tapisEpreuve.Any())
                                                {
                                                    xepreuve.Add(
                                                        new XElement(ConstantXML.TapisEpreuve,
                                                            tapisEpreuve.Select(noTapis =>
                                                                new XElement(ConstantXML.Tapis,
                                                                    new XAttribute(ConstantXML.Tapis_No, noTapis)
                                                                )
                                                            )
                                                        )
                                                    );
                                                }
                                            }

                                            return xepreuve;
                                        })
                                );

                                return xcompetition;
                            })
                    )
                )),
                false);
        }

        /// <summary>
        /// Creation d'un document XML pour une epreuve
        /// </summary>
        /// <param name="ctx">Le contexte d'exportation contenant le DataContext.</param>
        /// <param name="epreuve">L'épreuve à exporter.</param>
        /// <returns>Tuple contenant le document XML et un indicateur si le document est volumineux</returns>
        public static (XDocument outDoc, bool isLargeDoc) CreateDocumentEpreuve(IReadOnlyExportContext ctx, i_vue_epreuve_interface epreuve)
        {
            IJudoData DC = ctx.DataContext;
            ICompetition competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);

            // Sécurité : si la compétition n'existe pas, on renvoie un document vide pour éviter le crash
            if (competition == null) return (new XDocument(), false);

            // On estime la taille en combinant les inscrits et les combats de cette épreuve spécifique
            int nbInscrits = DC.Participants.EpreuveJudokas?.Count(ej => ej.epreuve == epreuve.id) ?? 0;
            int nbCombats = DC.Deroulement.Combats?.Count(c => c.epreuve == epreuve.id) ?? 0;

            // Le flag est levé si la complexité de l'épreuve dépasse le seuil kDocumentEpreuveFlushThreshold
            bool isLarge = (nbInscrits + nbCombats) > kDocumentEpreuveFlushThreshold;

            // Construction directe en une passe
            XElement xcompetition = competition.ToXml();
            xcompetition.Add(ExportEpreuve(DC, epreuve));

            return (new XDocument(new XElement(ConstantXML.DocRoot, xcompetition)), isLarge);
        }

        /// <summary>
        /// Creation d'un document XML pour une phase
        /// </summary>
        /// <param name="ctx">Le contexte d'exportation contenant le DataContext.</param>
        /// <param name="epreuve">L'épreuve à exporter.</param>
        /// <param name="phase">La phase à exporter.</param>
        /// <returns>Tuple contenant le document XML et un indicateur si le document est volumineux</returns>
        public static (XDocument outDoc, bool isLargeDoc)  CreateDocumentPhase(IReadOnlyExportContext ctx, i_vue_epreuve_interface epreuve, IPhase phase)
        {
            IJudoData DC = ctx.DataContext;
            ICompetition competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == epreuve.competition);
            if (competition == null) return (new XDocument(), false); // Sécurité

            // On se base sur les données déjà filtrées par ExportPhase : les participants et combats de cette phase.
            int nbParticipants = DC.Deroulement.Participants.Count(p => p.phase == phase.id);
            int nbCombats = DC.Deroulement.Combats.Count(c => c.phase == phase.id);
            bool isLarge = (nbParticipants + nbCombats) > kDocumentPhaseFlushThreshold;

            XElement xcompetition = competition.ToXml();
            XElement xepreuve = epreuve.ToXml(DC);

            xepreuve.Add(ExportPhase(DC, phase)); // On délègue au sous-boss optimisé
            xcompetition.Add(xepreuve);

            return (new XDocument(new XElement(ConstantXML.DocRoot, xcompetition)), isLarge);
        }

        /// <summary>
        /// Génère l'arbre XML représentant les feuilles de combats pour une compétition, 
        /// filtré optionnellement par phase ou par numéro de tapis.
        /// </summary>
        /// <remarks>
        /// Cette fonction a été optimisée pour réduire l'empreinte mémoire et la complexité temporelle (O(1)).
        /// Elle utilise des index en mémoire (Dictionary, Lookup) pour garantir un accès instantané aux données 
        /// et éviter le problème des requêtes N+1 ou des boucles O(N²) lors de la génération de l'arbre.
        /// </remarks>
        /// <param name="ctx">Le contexte d'exportation contenant le DataContext.</param>
        /// <param name="_phase">La phase spécifique à générer (si null, génère pour toute la compétition).</param>
        /// <param name="tapis">Le numéro du tapis spécifique à générer (si null, boucle sur tous les tapis).</param>
        /// <returns>Tuple contenant le document XML et un indicateur si le document est volumineux</returns>
        public static (XDocument outDoc, bool isLargeDoc) CreateDocumentFeuilleCombat(IReadOnlyExportContext ctx, IPhase _phase, int? tapis)
        {
            IJudoData DC = ctx.DataContext;

            // =========================================================================
            // 1. DÉTERMINATION DE LA COMPÉTITION DE BASE
            // =========================================================================
            int nbtapis = DC.Organisation.Competitions.Max(o => o.nbTapis);
            ICompetition competition = null;

            // Si une phase est fournie, on remonte jusqu'à sa compétition parente
            if (_phase != null)
            {
                var ep = _phase.epreuve.HasValue ? DC.Organisation.Epreuves.FirstOrDefault(o => o.id == _phase.epreuve.Value) : null;
                if (ep != null)
                {
                    competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == ep.competition);
                }
            }

            // Fallback : on prend la première compétition disponible si la résolution a échoué
            competition ??= DC.Organisation.Competitions.FirstOrDefault();
            if (competition == null) return (new XDocument(), false);

            // =========================================================================
            // 2. PRÉCHARGEMENT ET INDEXATION DES DONNÉES (Optimisation O(1))
            // =========================================================================
            // Utilisation de GroupBy().First() pour se prémunir contre d'éventuels doublons d'ID en base.
            var phasesDict = ctx.Caches.PhasesDict;
            var epreuvesDict = ctx.Caches.EpreuvesDict;
            var epreuvesEqDict = ctx.Caches.EpreuvesEqDict;
            var judokasDict = ctx.Caches.JudokasDict;
            var equipesDict = ctx.Caches.EquipesDict;

            // Utilisation de ToLookup pour les relations "Un-vers-Plusieurs" (évite les .Where() dans les boucles)
            var judokasByEquipe = ctx.Caches.JudokasByEquipe;
            var rencontresByCombat = ctx.Caches.RencontresByCombat;
            var poulesByPhase = ctx.Caches.PoulesByPhase;
            var participantsByPhase = ctx.Caches.ParticipantsByPhase;
            var groupesByTapis = ctx.Caches.GroupesByTapis;
            var epreuvesByEquipe = ctx.Caches.EpreuvesByEquipe;

            // Optimisation RAM : On ne rapatrie de la base de données QUE les vrais combats non terminés.
            var allCombats = DC.Deroulement.Combats
                .Where(o => !o.virtuel && (o.vainqueur == null || o.vainqueur == -1))
                .ToList();

            // On lève le drapeau si on génère "tout" (phase et tapis null) ET que le volume de combats dépasse le seuil.
            bool isLarge = (_phase == null && !tapis.HasValue) && (allCombats.Count > kDocumentFeuilleCombatFlushThreshold);

            bool isCSA = (competition.afficheCSA == (int)TypeCSAEnum.Minisite) || (competition.afficheCSA == (int)TypeCSAEnum.Tous);
            int compType = competition.type;

            // Variable pour traquer l'ID réel de la compétition rencontrée lors du traitement des combats
            int? firstCompetIdFound = null;

            var xTapisElements = new List<XElement>();
            int start = (_phase == null && !tapis.HasValue) ? 1 : 0;

            // =========================================================================
            // 3. BOUCLE DE GÉNÉRATION PAR TAPIS
            // =========================================================================
            for (int i = start; i <= nbtapis; i++)
            {
                // Si un tapis spécifique est demandé, on ignore les autres
                if (tapis != null && tapis != i) continue;

                XElement xtapis = new XElement(ConstantXML.Tapis, new XAttribute(ConstantXML.Tapis, i));

                // Initialisation et attachement immédiat des conteneurs
                // (L'ordre est crucial pour respecter la structure attendue par l'ancien parser XML)
                XElement xparticipants = new XElement(ConstantXML.Participants);
                XElement xcombats = new XElement(ConstantXML.Combats);
                XElement xphases = new XElement(ConstantXML.Phases);
                XElement xpoules = new XElement(ConstantXML.Poules);
                xtapis.Add(xparticipants, xcombats, xphases, xpoules);

                // HashSets pour garantir l'unicité des nœuds générés (évite les doublons XML)
                var epreuve_id_ajoute = new HashSet<int>();
                var epreuveeq_id_ajoute = new HashSet<int>();
                var participant_id_ajoute = new HashSet<int>();
                var phase_id_ajoute = new HashSet<int>();

                // Injection des groupes associés à ce tapis (via le Lookup pré-calculé)
                xtapis.Add(groupesByTapis[i].Select(g => g.ToXml()));

                if (_phase != null)
                {
                    AddEpreuveToXml(xtapis, _phase.epreuve, compType, DC, epreuvesDict, epreuvesEqDict, epreuve_id_ajoute, epreuveeq_id_ajoute, epreuvesByEquipe);
                }

                // Filtre des combats spécifiques à ce tapis
                var combatsTapis = allCombats.Where(o => o.tapis == i).ToList();

                // Validation des participants requis (sauf si affichage type CSA où on tolère les combats incomplets)
                var combatsAAfficher = combatsTapis
                    .Where(o => isCSA || (o.participant1.GetValueOrDefault() != 0 && o.participant2.GetValueOrDefault() != 0))
                    .ToList();

                foreach (var combat in combatsAAfficher)
                {
                    // Vérification de l'intégrité et de l'état de la phase liée au combat
                    if (!phasesDict.TryGetValue(combat.phase, out var phase) ||
                        (_phase != null && phase.epreuve != _phase.epreuve) ||
                        phase.etat < (int)EtatPhaseEnum.TirageValide)
                    {
                        continue;
                    }

                    // Ajout de la phase si elle n'a pas encore été traitée pour ce tapis
                    if (phase_id_ajoute.Add(phase.id)) xphases.Add(phase.ToXml());

                    // Enregistrement du premier ID de compétition concrètement rencontré dans les épreuves
                    if (firstCompetIdFound == null && phase.epreuve.HasValue)
                    {
                        if (compType == (int)CompetitionTypeEnum.Equipe)
                        {
                            if (epreuvesEqDict.TryGetValue(phase.epreuve.Value, out var vEq)) firstCompetIdFound = vEq.competition;
                        }
                        else
                        {
                            if (epreuvesDict.TryGetValue(phase.epreuve.Value, out var vEp)) firstCompetIdFound = vEp.competition;
                        }
                    }

                    AddEpreuveToXml(xtapis, phase.epreuve, compType, DC, epreuvesDict, epreuvesEqDict, epreuve_id_ajoute, epreuveeq_id_ajoute, epreuvesByEquipe);

                    // Ajout des participants liés à cette phase
                    // Utilisation du Lookup participantsByPhase pour un accès instantané sans parcourir toute la base
                    foreach (var p in participantsByPhase[phase.id])
                    {
                        if (participant_id_ajoute.Add(p.judoka))
                        {
                            XElement xp = p.ToXml(DC);
                            if (compType == (int)CompetitionTypeEnum.Equipe)
                            {
                                if (equipesDict.TryGetValue(p.judoka, out var eq))
                                {
                                    XElement xeq = eq.ToXml();
                                    xeq.Add(judokasByEquipe[eq.id].Select(j => j.ToXml(DC)));
                                    xp.Add(xeq);
                                }
                            }
                            else
                            {
                                if (judokasDict.TryGetValue(p.judoka, out var judoka)) xp.Add(judoka.ToXml(DC));
                            }
                            xparticipants.Add(xp);
                        }
                    }

                    // Construction du nœud de combat et ajout de ses rencontres associées
                    XElement xcombat = combat.ToXml(DC);
                    xcombat.Add(rencontresByCombat[combat.id].Select(r => r.ToXml()));
                    xcombats.Add(xcombat);
                }

                // =========================================================================
                // 4. LOGIQUE D'AJOUT DES POULES
                // =========================================================================
                foreach (var phaseId in phase_id_ajoute)
                {
                    foreach (var poule in poulesByPhase[phaseId])
                    {
                        string refPoule = poule.numero.ToString();
                        // On n'ajoute la poule que si au moins un combat en attente de ce tapis y fait référence
                        if (combatsTapis.Any(c => c.phase == phaseId && c.reference == refPoule))
                        {
                            xpoules.Add(poule.ToXml());
                        }
                    }
                }

                xTapisElements.Add(xtapis);
            }

            // =========================================================================
            // 5. FINALISATION ET ASSEMBLAGE DU DOCUMENT
            // =========================================================================
            // Si la compétition déduite des combats diffère de la compétition de base, on corrige
            if (firstCompetIdFound.HasValue && competition.id != firstCompetIdFound.Value)
            {
                competition = DC.Organisation.Competitions.FirstOrDefault(o => o.id == firstCompetIdFound.Value) ?? competition;
            }

            // Création de la racine XML et rattachement de la liste des tapis
            XElement xRoot = competition.ToXml();
            xRoot.Add(xTapisElements);

            return (new XDocument(new XElement(ConstantXML.DocRoot, xRoot)), isLarge);
        }
        #endregion

        #region METHODES PRIVEES

        /// <summary>
        /// Fonction utilitaire permettant d'ajouter les nœuds d'épreuves (individuelles ou par équipe) au tapis.
        /// Gère l'unicité via les HashSets fournis et protège contre les références nulles.
        /// </summary>
        private static void AddEpreuveToXml(XElement xtapis, int? epreuveIdNullable, int compType, IJudoData DC,
            Dictionary<int, IVueEpreuve> epreuvesDict, Dictionary<int, IVueEpreuveEquipe> epreuvesEqDict,
            HashSet<int> addedEp, HashSet<int> addedEq,
            ILookup<int?, IVueEpreuve> epreuvesByEquipe)
        {
            // Clause de garde : si l'épreuve est nulle, on ne fait rien
            if (!epreuveIdNullable.HasValue) return;
            int epreuveId = epreuveIdNullable.Value;

            if (compType == (int)CompetitionTypeEnum.Equipe)
            {
                if (epreuvesEqDict.TryGetValue(epreuveId, out var eq) && addedEq.Add(eq.id))
                {
                    xtapis.Add(eq.ToXml(DC));

                    foreach (var ep in epreuvesByEquipe[epreuveId])
                    {
                        if (addedEp.Add(ep.id))
                        {
                            xtapis.Add(ep.ToXml(DC));
                        }
                    }
                }
            }
            else
            {
                if (epreuvesDict.TryGetValue(epreuveId, out var ep) && addedEp.Add(ep.id))
                {
                    xtapis.Add(ep.ToXml(DC));
                }
            }
        }

        /// <summary>
        /// Export la structure XML d'une epreuve specifique
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="epreuve"></param>
        /// <returns></returns>
        private static XElement ExportEpreuve(IJudoData DC, i_vue_epreuve_interface epreuve)
        {
            XElement xepreuve = epreuve.ToXml(DC);

            // 1. Appel de notre méthode optimisée avec le HashSet
            var judokasInscrits = DC.Participants.GetJudokaEpreuve(epreuve.id).ToList();

            // 2. Création d'un index ultra-rapide en mémoire (O(1)) pour les inscriptions
            var inscriptionsDict = DC.Participants.EpreuveJudokas
                                     .Where(ej => ej.epreuve == epreuve.id)
                                     .ToDictionary(ej => ej.judoka);

            // 3. Construction fonctionnelle globale
            xepreuve.Add(

                // --- Les Inscrits ---
                new XElement(ConstantXML.Epreuve_Inscrits,
                    judokasInscrits.SelectMany(judoka =>
                    {
                        var elements = new List<XElement> { judoka.ToXml(DC) };

                        // Recherche instantanée en mémoire via le dictionnaire
                        if (inscriptionsDict.TryGetValue(judoka.id, out var ej))
                        {
                            elements.Add(ej.ToXml());
                        }

                        return elements;
                    })
                ),

                // --- Les Phases ---
                new XElement(ConstantXML.Phases,
                    DC.Deroulement.Phases
                        .Where(o => o.epreuve == epreuve.id)
                        // L'utilisation de .Any() en mémoire est cruciale pour ne pas itérer inutilement
                        .Where(phase => DC.Deroulement.Participants.Any(p => p.phase == phase.id))
                        .Select(phase => ExportXML.ExportPhase(DC, phase))
                ),

                // --- Le Classement Final ---
                ExportXML.ExportClassementFinal(DC, epreuve)
            );

            return xepreuve;
        }

        /// <summary>
        /// Export la structure XML d'un phase specifique
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="phase"></param>
        /// <returns></returns>
        private static XElement ExportPhase(IJudoData DC, IPhase phase)
        {
            XElement xphase = phase.ToXml();

            // --- 1. AFFECTATION DES POULES ---
            if (phase.typePhase == (int)TypePhaseEnum.Poule)
            {
                xphase.Add(
                    new XElement(ConstantXML.Poules,
                        DC.Deroulement.Poules
                            .Where(o => o.phase == phase.id)
                            .Select(poule => poule.ToXml())
                    )
                );
            }

            // --- 2. PREPARATION DES CACHES POUR LES PARTICIPANTS ---
            var participants = DC.Deroulement.Participants.Where(o => o.phase == phase.id).ToList();
            var participantIds = participants.Select(p => p.judoka).ToHashSet();
            var compType = (CompetitionTypeEnum)DC.Organisation.Competition.type;

            // Initialisation stricte des dictionnaires et du Lookup en mémoire
            var judokasDict = new Dictionary<int, IJudoka>();
            var equipesDict = new Dictionary<int, IEquipe>();
            ILookup<int, IJudoka> judokasByEquipe = Enumerable.Empty<IJudoka>().ToLookup(j => j.equipe);

            if (compType == CompetitionTypeEnum.Individuel || compType == CompetitionTypeEnum.Shiai)
            {
                judokasDict = DC.Participants.Judokas
                                .Where(j => participantIds.Contains(j.id))
                                .ToDictionary(j => j.id);
            }
            else if (compType == CompetitionTypeEnum.Equipe)
            {
                equipesDict = DC.Participants.Equipes
                                .Where(e => participantIds.Contains(e.id))
                                .ToDictionary(e => e.id);

                // equipe étant de type int, pas besoin de cast ni de vérification null
                judokasByEquipe = DC.Participants.Judokas
                                    .Where(j => participantIds.Contains(j.equipe))
                                    .ToLookup(j => j.equipe);
            }

            // --- 3. AFFECTATION DES PARTICIPANTS ---
            xphase.Add(
                new XElement(ConstantXML.Participants,
                    participants.Select(p =>
                    {
                        XElement xparticipant = p.ToXml(DC);

                        if (compType == CompetitionTypeEnum.Individuel || compType == CompetitionTypeEnum.Shiai)
                        {
                            if (judokasDict.TryGetValue(p.judoka, out var judoka))
                            {
                                xparticipant.Add(judoka.ToXml(DC));
                            }
                        }
                        else if (compType == CompetitionTypeEnum.Equipe)
                        {
                            if (equipesDict.TryGetValue(p.judoka, out var equipe))
                            {
                                XElement xequipe = equipe.ToXml();

                                // Injection immédiate des judokas de l'équipe (O(1))
                                xequipe.Add(judokasByEquipe[equipe.id].Select(j => j.ToXml(DC)));
                                xparticipant.Add(xequipe);
                            }
                        }

                        return xparticipant;
                    })
                )
            );

            // --- 4. PREPARATION ET AFFECTATION DES COMBATS ---
            var combats = DC.Deroulement.Combats.Where(o => o.phase == phase.id).ToList();

            // Le HashSet utilise l'int standard (Combat.id)
            var combatIds = combats.Select(c => c.id).ToHashSet();

            // Rencontre.combat est un int? : on filtre avec HasValue et on utilise .Value comme clé
            var rencontresByCombat = DC.Deroulement.Rencontres
                                        .Where(r => r.combat.HasValue && combatIds.Contains(r.combat.Value))
                                        .ToLookup(r => r.combat.Value);

            xphase.Add(
                new XElement(ConstantXML.Combats,
                    combats.Select(c =>
                    {
                        XElement xcombat = c.ToXml(DC);

                        // c.id est int, rencontresByCombat attend une clé int : compilation parfaite
                        xcombat.Add(rencontresByCombat[c.id].Select(r => r.ToXml()));

                        return xcombat;
                    })
                )
            );

            return xphase;
        }

        /// <summary>
        /// Export le classement final
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="epreuve"></param>
        /// <returns></returns>
        private static XElement ExportClassementFinal(IJudoData DC, i_vue_epreuve_interface epreuve)
        {
            // 1. RÉCUPÉRATION ET FUSION OPTIMISÉE DES PARTICIPANTS
            var participants1 = DC.Deroulement.ListeParticipant2(epreuve.id);
            var participants2 = DC.Deroulement.ListeParticipant1(epreuve.id);

            // L'astuce magique : Concaténer les deux listes, grouper par judoka, 
            // et ne garder que le premier de chaque groupe. 
            // Cela remplace votre boucle if (Count == 0) en une fraction de seconde !
            var finalParticipants = participants1.Concat(participants2)
                                                 .GroupBy(p => p.judoka)
                                                 .Select(g => g.First())
                                                 .OrderBy(p => p.classementFinal)
                                                 .ToList();

            // S'il n'y a personne, on sort tout de suite pour ne pas travailler pour rien
            if (!finalParticipants.Any()) return new XElement(ConstantXML.Classement);

            // 2. PRÉPARATION DES CACHES (On ne charge QUE les participants de ce classement)
            var participantIds = finalParticipants.Select(p => p.judoka).ToHashSet();
            var compType = (CompetitionTypeEnum)DC.Organisation.Competition.type;

            var judokasDict = new Dictionary<int, IJudoka>();
            var equipesDict = new Dictionary<int, IEquipe>();
            ILookup<int, IJudoka> judokasByEquipe = Enumerable.Empty<IJudoka>().ToLookup(j => j.equipe);

            if (compType == CompetitionTypeEnum.Individuel || compType == CompetitionTypeEnum.Shiai)
            {
                judokasDict = DC.Participants.Judokas
                                .Where(j => participantIds.Contains(j.id))
                                .ToDictionary(j => j.id);
            }
            else if (compType == CompetitionTypeEnum.Equipe)
            {
                equipesDict = DC.Participants.Equipes
                                .Where(e => participantIds.Contains(e.id))
                                .ToDictionary(e => e.id);

                judokasByEquipe = DC.Participants.Judokas
                                    .Where(j => participantIds.Contains(j.equipe))
                                    .ToLookup(j => j.equipe);
            }

            // 3. CONSTRUCTION FONCTIONNELLE
            return new XElement(ConstantXML.Classement,
                finalParticipants.Select(p =>
                {
                    XElement xparticipant = p.ToXml(DC);

                    if (compType == CompetitionTypeEnum.Individuel || compType == CompetitionTypeEnum.Shiai)
                    {
                        if (judokasDict.TryGetValue(p.judoka, out var judoka))
                        {
                            xparticipant.Add(judoka.ToXml(DC));
                        }
                    }
                    else if (compType == CompetitionTypeEnum.Equipe)
                    {
                        if (equipesDict.TryGetValue(p.judoka, out var equipe))
                        {
                            XElement xequipe = equipe.ToXml();
                            xequipe.Add(judokasByEquipe[equipe.id].Select(j => j.ToXml(DC)));
                            xparticipant.Add(xequipe);
                        }
                    }

                    return xparticipant;
                })
            );
        }

        #endregion
    }
}
