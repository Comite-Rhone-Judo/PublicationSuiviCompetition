using AppPublication.ExtensionNoyau;
using KernelImpl;
using KernelImpl.Noyau.Structures;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace AppPublication.Export
{
    public class ExportSharedContextBase
    {
        #region MEMBRES
        // Collections en lecture seule pour garantir le thread-safety lors de la lecture
        public XElement Clubs { get; private set; }
        public XElement Comites { get; private set; }
        public XElement Secteurs { get; private set; }
        public XElement Ligues { get; private set; }
        public XElement Pays { get; private set; }

        public XElement Ceintures { get; private set; }
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Juste pour empecher la creation d'une instance a la volee
        /// </summary>
        protected ExportSharedContextBase()
        {
        }

        /// <summary>
        /// Factory pour creer une instance initialisee
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        /// <returns></returns>
        public static ExportSharedContextBase Instance(IJudoData DC, ExtendedJudoData EDC)
        {
            var output =  new ExportSharedContextBase();
            output.Initialize(DC);

            return output;
        }

        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// Ajoute les informations de structure se trouvant dans le contexte d'export au document XML
        /// </summary>
        /// <param name="doc"></param>
        public virtual void AddFullXmlContext(XDocument doc)
        {
            if (doc?.Root == null) return;

            // On regroupe les éléments dans un tableau pour un traitement propre
            XElement[] structures = { Clubs, Comites, Ligues, Secteurs, Pays, Ceintures };

            foreach (XElement structure in structures)
            {
                // 1. On s'assure que la propriété du contexte n'est pas null
                // 2. On vérifie qu'un élément du même nom n'existe pas déjà à la racine du document
                if (structure != null && doc.Root?.Element(structure.Name) == null)
                {
                    doc.Root?.Add(structure);
                }
            }
        }

        #endregion

        #region METHODES PRIVEES
        /// <summary>
        /// Initialisation interne
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        protected virtual void Initialize(IJudoData DC)
        {
            Clubs = ExportXML.GetClubs(DC);
            Comites = ExportXML.GetComites(DC);
            Secteurs = ExportXML.GetSecteurs(DC);
            Ligues = ExportXML.GetLigues(DC);
            Pays = ExportXML.GetPays(DC);
            Ceintures = ExportXML.GetCeintures(DC);
        }  
        #endregion
    }
}
