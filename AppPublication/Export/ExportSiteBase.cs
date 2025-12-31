using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.Tools.Enum;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;
using AppPublication.Generation;

namespace AppPublication.Export
{
    public abstract class ExportSiteBase
    {
        protected const int kTailleMaxNomCompetition = 30;

        protected static List<XElement> _xClubs = new List<XElement>();          // Instance partagees pour la liste des clubs
        protected static List<XElement> _xComites = new List<XElement>();          // Instance partagees pour la liste des comites
        protected static List<XElement> _xSecteurs = new List<XElement>();          // Instance partagees pour la liste des secteurs
        protected static List<XElement> _xLigues = new List<XElement>();          // Instance partagees pour la liste des ligues
        protected static List<XElement> _xPays = new List<XElement>();          // Instance partagees pour la liste des pays

        /// <summary>
        /// Initialise les structures de donnees partagees pour la generation des documents XML
        /// </summary>
        /// <param name="DC"></param>
        /// <param name="EDC"></param>
        /// <param name="config"></param>
        public static void InitSharedData(IJudoData DC, ExtendedJudoData EDC)
        {
            using (TimedLock.Lock(_xClubs))
            {
                _xClubs = ExportXML.GetClubs(DC);
            }
            using (TimedLock.Lock(_xComites))
            {
                _xComites = ExportXML.GetComites(DC);
            }
            using (TimedLock.Lock(_xSecteurs))
            {
                _xSecteurs = ExportXML.GetSecteurs(DC);
            }
            using (TimedLock.Lock(_xLigues))
            {
                _xLigues = ExportXML.GetLigues(DC);
            }
            using (TimedLock.Lock(_xPays))
            {
                _xPays = ExportXML.GetPays(DC);
            }
        }
                
        /// <summary>
        /// Ajoute les informations de structure en cache
        /// </summary>
        /// <param name="doc"></param>
        protected static void AddStructures(ref XmlDocument doc)
        {
            ExportXML.AddStructures(ref doc, _xClubs, _xComites, _xSecteurs, _xLigues, _xPays);
        }

        /// <summary>
        /// Nettoie le path specifie pour passer de Repertoire Windows à URL
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static string PathForUrl(string path)
        {
            string output = path.Replace('\\', '/');

            return output;
        }

        /// <summary>
        /// Ajoute les arguments de structure du site pour les templates xslt
        /// </summary>
        /// <param name="argsList">La liste d'argument a actualiser</param>
        /// <param name="siteStruct">La structure du site</param>
        /// <param name="targetFile">Le fichier HTML cible</param>
        protected virtual void AddStructureArgument(XsltArgumentList argsList, ExportStructureBase siteStruct, string targetFile)
        {
            siteStruct.TargetPath = targetFile;

            // Ajoute les parametres en relatif par rapport a la position du fichier
            argsList.AddParam("imgPath", "", PathForUrl(siteStruct.RepertoireImg(relatif: true)));
            argsList.AddParam("jsPath", "", PathForUrl(siteStruct.RepertoireJs(relatif: true)));
            argsList.AddParam("cssPath", "", PathForUrl(siteStruct.RepertoireCss(relatif: true)));
            argsList.AddParam("competitionPath", "", PathForUrl(siteStruct.RepertoireCompetition(relatif: true)));
        }
    }
}