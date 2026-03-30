using FranceJudo.Core.Reflection;
using System;
using System.IO;
using System.Linq;
using System.Xml;


namespace FranceJudo.Core.XML
{
    public class InAssemblyUrlResolver : XmlResolver
    {
        // On stocke le dictionnaire de l'assembly cible
        private readonly AssemblyResourceDictionary _dictionary;

        public InAssemblyUrlResolver(AssemblyResourceDictionary dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            // C'est une resource embarquee donc, juste nom suffit
            // return new Uri("Tools.Export.xslt.site." + relativeUri, UriKind.Relative);
            return new Uri(relativeUri, UriKind.Relative);
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            // 1. Remplace les '/' par des '.' pour correspondre au format des ressources
            string resName = absoluteUri.OriginalString.Replace("/", ".");

            // 2. Essaye une recherche directe ciblée
            Stream res = _dictionary.GetStream(resName);

            // 3. Fallback : L'équivalent exact de votre ancien SearchAssemblyResource
            if (res == null)
            {
                // Cherche le premier nom de ressource qui contient la chaîne
                string foundName = _dictionary.AllResources
                    .FirstOrDefault(r => r.Contains(resName) || r.Contains(absoluteUri.OriginalString));

                if (foundName != null)
                {
                    res = _dictionary.GetStream(foundName);
                }
            }

            return res ?? throw new ArgumentOutOfRangeException(
                nameof(absoluteUri),
                $"Impossible de trouver la ressource XSLT liée : {absoluteUri.OriginalString}");
        }
    }
}
