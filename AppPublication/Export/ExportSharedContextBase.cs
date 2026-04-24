using AppPublication.ExtensionNoyau;
using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using System.Linq;
using System.Xml.Linq;

namespace AppPublication.Export
{
    public abstract class ExportSharedContextBase : IReadOnlyExportContext
    {
        #region PROPERTIES
        // Porte la source de donnees (snapshot contextuel)
        public IJudoData DataContext { get; private set; }

        public IExtendedJudoData ExtendedDataContext { get; private set; }

        // Caches pré-construits pour éviter les accès redondants et coûteux lors de l'enrichissement des documents
        public CombatExportCaches Caches { get; private set; }

        // Référentiels de base
        public XElement Clubs { get; private set; }
        public XElement Comites { get; private set; }
        public XElement Secteurs { get; private set; }
        public XElement Ligues { get; private set; }
        public XElement Pays { get; private set; }
        public XElement Ceintures { get; private set; }

        // Configuration spécifique remontée pour factorisation
        public XElement SiteConfiguration { get; protected set; }

        // Document principal généré (Unifie DocCombats et DocEngagements)
        public XDocument ExportDocument { get; protected set; }
        #endregion

        protected ExportSharedContextBase(IJudoData DC, IExtendedJudoData EDC = null)
        {
            DataContext = DC;
            ExtendedDataContext = EDC;
            // On initialise le cache dès la construction de l'objet de base, il pourront etre utilise pour creer les documents de base (combats et engagements) avant l'enrichissement final
            InitCaches();
        }

        #region METHODES PUBLIQUES D'ENRICHISSEMENT (API EXTERNE)

        // À appeler une seule fois au démarrage de la génération
        public void InitCaches()
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
            if (doc?.Root == null) return;

            XElement[] elementsToInject = [Clubs, Comites, Ligues, Secteurs, Pays, Ceintures];

            foreach (XElement element in elementsToInject)
            {
                if (element != null && doc.Root.Element(element.Name) == null)
                {
                    doc.Root.Add(element);
                }
            }
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

        #endregion

        #region PIPELINE D'INITIALISATION
        /// <summary>
        /// Workflow centralisé garantissant l'ordre d'initialisation pour toutes les classes filles.
        /// </summary>
        protected void ExecuteExportPipeline(XElement configXml, XDocument generatedDoc)
        {
            // 1. Chargement des référentiels
            Clubs = ExportXML.GetClubs(this);
            Comites = ExportXML.GetComites(this);
            Secteurs = ExportXML.GetSecteurs(this);
            Ligues = ExportXML.GetLigues(this);
            Pays = ExportXML.GetPays(this);
            Ceintures = ExportXML.GetCeintures(this);

            // 2. Assignation des spécificités transmises par l'enfant
            SiteConfiguration = configXml;
            ExportDocument = generatedDoc;

            // 3. Enrichissement automatique du document généré
            EnrichWithFullContext(ExportDocument);

            // 4. Log
            LogTools.DebugLogData(ExportDocument);
        }
        #endregion
    }
}