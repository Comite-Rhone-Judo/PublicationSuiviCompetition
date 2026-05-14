using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Reflection;
using System;
using System.IO;

namespace FranceJudo.Core.Export
{
    public static class ResourceExtractor
    {
        /// <summary>
        /// Extrait une ressource du dictionnaire et la sauvegarde sur le disque physique.
        /// </summary>
        /// <param name="dictionary">Le dictionnaire contenant la ressource.</param>
        /// <param name="resourceName">Le nom complet de la ressource à extraire.</param>
        /// <param name="destinationFilePath">Le chemin physique complet où sauvegarder le fichier.</param>
        /// <returns>True si l'extraction a réussi, False sinon.</returns>
        public static bool ExtractToFile(AssemblyResourceDictionary dictionary, string resourceName, string destinationFilePath)
        {
            using (Stream resourceStream = dictionary.GetStream(resourceName))
            {
                if (resourceStream == null)
                    return false;

                // Optionnel mais très utile : s'assurer que le sous-dossier parent existe
                string targetDir = Path.GetDirectoryName(destinationFilePath);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                FileSystemHelper.NeedAccessFile(destinationFilePath);
                try
                {
                    using (FileStream fs = new FileStream(destinationFilePath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fs);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogTools.Logger?.Error(ex, $"Echec d'extraction de la ressource '{resourceName}' vers '{destinationFilePath}'");
                    return false;
                }
                finally
                {
                    FileSystemHelper.ReleaseFile(destinationFilePath);
                }
            }
        }
    }
}