using AppPublication.ExtensionNoyau;
using FranceJudo.Core.Export;
using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.XML;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AppPublication.Export
{
    public abstract class ExportSharedContextBase : IReadOnlyExportContext
    {
        #region PROPERTIES
        // Porte la source de donnees (snapshot contextuel)
        public IJudoData DataContext { get; private set; }

        /// <summary>
        /// Remplace Clubs, Comites, Ligues, etc.
        /// </summary>
        public IExtendedJudoData ExtendedDataContext { get; private set; }

        // Caches pré-construits pour éviter les accès redondants et coûteux lors de l'enrichissement des documents
        public CombatExportCaches Caches { get; private set; }

        public XPathDocument ReferenceData { get; private set; }

        // Configuration spécifique remontée pour factorisation
        public XElement SiteConfiguration { get; protected set; }

        // Document principal généré (Unifie DocCombats et DocEngagements)
        public XmlSource ExportDocument { get; protected set; }

        // Dictionnaire ultra-rapide et lock-free pour suivre l'état de generation des prochains combats (peu y avoir des conflits lors des poules/tableau)
        public ConcurrentDictionary<int, bool> ProchainsCombatsGeneres { get; } = new ConcurrentDictionary<int, bool>();
        #endregion

        protected ExportSharedContextBase(IJudoData DC, IExtendedJudoData EDC = null)
        {
            DataContext = DC;
            ExtendedDataContext = EDC;
            // On initialise le cache dès la construction de l'objet de base, il pourront etre utilise pour creer les documents de base (combats et engagements) avant l'enrichissement final
            InitCaches();

            // Initialisation du référentiel en lecture seule (XPathDocument) à partir du DataContext des la construction de la classe de base,
            // pour éviter de devoir le faire dans chaque classe fille et garantir qu'il est toujours disponible pour les méthodes d'enrichissement.
            InitReferenceData();
        }

        #region METHODES PUBLIQUES D'ENRICHISSEMENT (API EXTERNE)
       
        /// <summary>
        /// Enrichit le document avec toutes les informations (Structure de base + Configuration du site)
        /// </summary>
        public virtual void EnrichWithFullContext(XDocument doc)
        {
            EnrichWithBaseStructure(doc);
            EnrichWithConfiguration(doc);
        }

        /// <summary>
        /// Enrichit le document uniquement avec les référentiels de base (Clubs, Comités, Ligues...)
        /// </summary>
        public void EnrichWithBaseStructure(XDocument doc)
        {
            // Laisser vide pour le moment, le placeholder existe si on doit ajouter des donnees au XML dans le futur qui ne sont pas dans
            // les référentiels de base, mais qui doivent quand meme etre dans la partie "structure" du XML (ex: des types de données statiques, des paramètres globaux, etc.)
        }

        /// <summary>
        /// Enrichit le document uniquement avec la configuration spécifique du site
        /// </summary>
        public void EnrichWithConfiguration(XDocument doc)
        {
            if (doc?.Root == null || SiteConfiguration == null) return;

            if (doc.Root.Element(SiteConfiguration.Name) == null)
            {
                doc.Root.Add(SiteConfiguration);
            }
        }

        public void Dispose()
        {
            // Nettoie le XmlSource (et donc le fichier temporaire si présent)
            ExportDocument?.Dispose();
        }
        #endregion

        #region METHODES PRIVEES
        /// <summary>
        /// Workflow centralisé garantissant l'ordre d'initialisation pour toutes les classes filles.
        /// </summary>
        protected void ExecuteExportPipeline(XElement configXml, XDocument generatedDoc)
        {
            // 2. Assignation des spécificités transmises par l'enfant
            SiteConfiguration = configXml;
            
            // 3. Enrichissement automatique du document généré
            EnrichWithFullContext(generatedDoc);

            // 4. Log
            LogTools.DebugLogData(generatedDoc);

            // on le fait en dernier car sur un gros document, il peut etre flush sur disque
            ExportDocument = new XmlSource(generatedDoc);
        }

        /// <summary>
        /// Initialise les caches pour un accès rapide aux données.
        /// </summary>
        private void InitCaches()
        {
            // Si DataContext est null, Caches vaudra null. S'il ne l'est pas, l'initialisation O(1) s'exécute.
            if (DataContext == null) return;

            this.Caches = new CombatExportCaches
            {
                PhasesDict = DataContext.Deroulement?.Phases?.GroupBy(p => p.id).ToDictionary(g => g.Key, g => g.First()),
                EpreuvesDict = DataContext.Organisation?.VueEpreuves?.GroupBy(e => e.id).ToDictionary(g => g.Key, g => g.First()),
                EpreuvesEqDict = DataContext.Organisation?.VueEpreuveEquipes?.GroupBy(e => e.id).ToDictionary(g => g.Key, g => g.First()),
                JudokasDict = DataContext.Participants?.Judokas?.GroupBy(j => j.id).ToDictionary(g => g.Key, g => g.First()),
                EquipesDict = DataContext.Participants?.Equipes?.GroupBy(e => e.id).ToDictionary(g => g.Key, g => g.First()),

                JudokasByEquipe = DataContext.Participants?.Judokas?.Cast<IJudoka>().ToLookup(j => j.equipe),
                RencontresByCombat = DataContext.Deroulement?.Rencontres?.Where(r => r.combat.HasValue).ToLookup(r => r.combat.Value),
                PoulesByPhase = DataContext.Deroulement?.Poules?.Cast<IPoule>().ToLookup(p => p.phase),
                ParticipantsByPhase = DataContext.Deroulement?.Participants?.Cast<IParticipant>().ToLookup(p => p.phase),
                GroupesByTapis = DataContext.Deroulement?.VueGroupes?.Cast<IVueGroupe>().ToLookup(g => g.groupe_tapis),
                EpreuvesByEquipe = DataContext.Organisation?.VueEpreuves?.Cast<IVueEpreuve>().ToLookup(e => e.id_epreuve_equipe),
                InscriptionsByEpreuve = DataContext.Participants?.EpreuveJudokas?.Cast<IEpreuveJudoka>().ToLookup(ej => ej.epreuve)
            };
        }

        /// <summary>
        /// Initialise les données de référence en construisant un arbre XML et en le convertissant en XPathDocument.
        /// </summary>
        private void InitReferenceData()
        {
            if (DataContext == null) return;

            // On construit l'arbre XML de référence UNE SEULE FOIS.
            // On passe 'this' aux méthodes GetClubs, GetComites, etc.
            XDocument refDoc = new XDocument(
                new XElement(ConstantXML.Structures,
                    ExportXML.GetClubs(this),
                    ExportXML.GetComites(this),
                    ExportXML.GetLigues(this),
                    ExportXML.GetSecteurs(this),
                    ExportXML.GetPays(this),
                    ExportXML.GetCeintures(this)
                )
            );

            // On convertit en XPathDocument : format immuable et compact en RAM
            using (var reader = refDoc.CreateReader())
            {
                this.ReferenceData = new XPathDocument(reader);
            }
        }
        #endregion
    }
}