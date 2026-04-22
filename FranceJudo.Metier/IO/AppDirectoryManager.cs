using FranceJudo.Core.Export;
using FranceJudo.Core.IO;
using FranceJudo.Core.Reflection;
using FranceJudo.Metier.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace FranceJudo.Metier.IO
{
    /// <summary>
    /// Remplace ConstantFile et DirectoryHelper.
    /// Classe statique gérant la définition et la création de l'arborescence de l'application.
    /// </summary>
    public static class AppDirectoryManager
    {
        #region Constantes privées

        private const string kDirSaveCom = @"Save\Com";
        private const string kDirLogoFede = @"Logos\Fédé";
        private const string kDirLogoLigue = @"Logos\Ligue";
        private const string kDirLogoSponsor = @"Logos\sponsor";
        private const string kDirFlags = @"flags";
        private const string kDirRootExport = @"FRANCE-JUDO";

        private const string kDirRessources = @"Ressources";
        private const string kDirRessourcesImg = @"Ressources\Images";

        private const string kFileChecksum = @"checksum_fichiers_site";
        #endregion

        // Liste stricte des répertoires à créer physiquement sur le disque
        private static List<string> _directoriesToCreate;

        #region Propriétés Statiques : Chemins de Répertoires (Remplace ConstantFile)

        public static string RessourcesDir { get; private set; }
        public static string RessoucesImgDir { get; private set; }

        public static string SaveCOMDir { get; private set; }
        public static string SaveDir { get; private set; }
        public static string Logo1Dir { get; private set; }
        public static string Logo2Dir { get; private set; }
        public static string Logo3Dir { get; private set; }

        public static string MediaFlagsDir { get; private set; }

        #endregion

        #region Propriétés Statiques : Chemins de Fichiers Complets

        public static string ChecksumFile { get; private set; }

        #endregion

        #region Propriétés Statiques : Noms de Fichiers Constants

        public static string ExtensionXML { get; } = ".xml";
        public static string ExtensionTXT { get; } = ".txt";

        #endregion

        /// <summary>
        /// Remplace rigoureusement l'ancienne fonction InitDataDirectories.
        /// Initialise les chemins statiques, crée les dossiers physiques et extrait les ressources.
        /// ATTENTION : Doit être appelée une seule fois au démarrage de l'application !
        /// </summary>
        public static void Initialize(string dataPath, string _)
        {
            _directoriesToCreate = new List<string>();
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // 1. Définition et Enregistrement des chemins
            RessourcesDir = RegisterDirectory(dataPath, kDirRessources, createPhysicalFolder: true);
            RessoucesImgDir = RegisterDirectory(dataPath, kDirRessourcesImg, createPhysicalFolder: true);

            SaveDir = RegisterDirectory(dataPath, string.Empty, createPhysicalFolder: true);
            SaveCOMDir = RegisterDirectory(dataPath, kDirSaveCom, createPhysicalFolder: true);

            Logo1Dir = RegisterDirectory(dataPath, kDirLogoFede, createPhysicalFolder: true);
            Logo2Dir = RegisterDirectory(dataPath, kDirLogoLigue, createPhysicalFolder: true);
            Logo3Dir = RegisterDirectory(dataPath, kDirLogoSponsor, createPhysicalFolder: true);

            MediaFlagsDir = RegisterDirectory(dataPath, kDirFlags, createPhysicalFolder: true);

            // Les fichier
            ChecksumFile = kFileChecksum + ExtensionXML;

            // 2. Création stricte des dossiers
            foreach (var directory in _directoriesToCreate)
            {
                FileSystemHelper.CreateDirectory(directory);
            }

            ExtractResources();
        }

        /// <summary>
        /// Extrait les ressources necessaires
        /// </summary>
        private static void ExtractResources()
        {
            // On ne boucle QUE sur les ressources du dossier des images du site
            foreach (string resourceName in MetierResources.Dictionary.FindByFolder(MetierResources.Folders.SiteImg))
            {
                var (targetDir, fileName) = ResolveResourceDestination(resourceName);

                if (string.IsNullOrEmpty(targetDir) || string.IsNullOrEmpty(fileName))
                    continue;

                string fullFilePath = Path.Combine(targetDir, fileName);

                // Appel de l'extracteur général (qui gère les droits d'accès et l'écriture disque)
                ResourceExtractor.ExtractToFile(MetierResources.Dictionary, resourceName, fullFilePath);
            }
        }

        private static (string targetDir, string fileName) ResolveResourceDestination(string resourceName)
        {
            if (resourceName.Contains(MetierResources.Folders.SiteImg))
            {
                return (RessoucesImgDir, ResourcePath.GuessFileName(resourceName));
            }

            // Aucune des resources n'est nécessaire pour la generation (on ne travaille que avec des fichiers embarques)
            return (string.Empty, string.Empty);
        }

        /// <summary>
        /// Enregistre un chemin de répertoire en construisant le chemin complet à partir du chemin de base et du sous-dossier.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="subPath"></param>
        /// <param name="createPhysicalFolder"></param>
        /// <returns></returns>
        private static string RegisterDirectory(string basePath, string subPath, bool createPhysicalFolder = true)
        {
            string fullPath = string.IsNullOrEmpty(subPath)
                ? basePath
                : Path.Combine(basePath, subPath);

            if (!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !fullPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                fullPath += Path.DirectorySeparatorChar;
            }

            if (createPhysicalFolder)
            {
                _directoriesToCreate.Add(fullPath);
            }

            return fullPath;
        }

        /// <summary>
        /// Le repertoire d'export par defaut
        /// </summary>
        /// <param name="racine"></param>
        /// <returns></returns>
        public static string GetExportDir(string racine)
        {
            return Path.Combine(racine, kDirRootExport);
        }
    }
}