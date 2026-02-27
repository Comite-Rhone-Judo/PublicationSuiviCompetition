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
        public IReadOnlyList<XElement> Clubs { get; private set; }
        public IReadOnlyList<XElement> Comites { get; private set; }
        public IReadOnlyList<XElement> Secteurs { get; private set; }
        public IReadOnlyList<XElement> Ligues { get; private set; }
        public IReadOnlyList<XElement> Pays { get; private set; }

        public IReadOnlyList<XElement> Ceintures { get; private set; }
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
            output.Initialize(DC, EDC);

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
            // Note on ajoute les ceintures tout le temps, l'overhead est faire mais cela simplifie beaucoup le code
            doc?.Root?.Add(Clubs, Comites, Ligues, Secteurs, Pays, Ceintures);
        }
        #endregion

        #region METHODES PRIVEES
        /// <summary>
        /// Initialisation interne
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        protected virtual void Initialize(IJudoData DC, ExtendedJudoData EDC)
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
