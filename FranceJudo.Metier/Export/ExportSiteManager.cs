using System.Xml.Linq;
using System.Xml.Xsl;
using FranceJudo.Core.Export; // Le métier a le droit de référencer le Core

namespace FranceJudo.Metier.Export
{
    public static class ExportSiteManager // ou vous pouvez l'ajouter dans ExportTools
    {
        /// <summary>
        /// Point d'entrée métier pour générer le site HTML
        /// </summary>
        public static void GenererHtmlSite(XDocument xml, ExportEnum export_type, string fileSave, XsltArgumentList argsList, string fileExtension = "html", bool useCache = true)
        {
            // 1. Logique métier : quel est le template XSLT à utiliser pour cet export ?
            string xslt = ExportTools.GetXsltSite(export_type);

            // 2. Appel à l'outil technique : génère le HTML
            ExportHTML.ToHTML(xml, fileSave, argsList, xslt, fileExtension, useCache);
        }
    }
}