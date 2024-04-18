using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tools.Outils;

namespace Tools.Export
{
    public class InAssemblyUrlResolver : XmlResolver
    {
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            // C'est une resource embarquee donc, juste nom suffit
            // return new Uri("Tools.Export.xslt.site." + relativeUri, UriKind.Relative);
            return new Uri(relativeUri, UriKind.Relative);
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            // Recupere l'URL au sein de l'assembly et remplace les '/' par des '.'
            string resName = absoluteUri.OriginalString.Replace("/", ".");

            // Cherche la resource contenant le nom dans l'URI
            // Stream res = ResourcesTools.SearchAssemblyResource(absoluteUri.OriginalString);
            Stream res = ResourcesTools.GetAssembyResource(resName);

            if(res == null)
            {
                // Essaye une recherche complete
                res = ResourcesTools.SearchAssemblyResource(absoluteUri.OriginalString);
                if (res == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(absoluteUri));
                }
            }

            return res;
        }
    }
}
