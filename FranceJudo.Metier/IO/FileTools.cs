using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace FranceJudo.Metier.IO
{
    public static class FileTools
    {
        // Encapsulation forte et sécurisée
        private static readonly Synchronized<Dictionary<string, XDocument>> _notSave =
            new Synchronized<Dictionary<string, XDocument>>(new Dictionary<string, XDocument>());

        public static void SaveFile(XDocument doc, string fileType)
        {
            Dictionary<string, XDocument> snapshotToProcess = null;

            // 1. VERROUILLAGE ULTRA-COURT (RAM uniquement)
            // On met à jour la mémoire et on prend une photo des données à traiter.
            _notSave.SafeWriteAction(dict =>
            {
                // Remplace ou ajoute le document (plus rapide que Remove + Add)
                dict[fileType] = doc;

                // Création du Snapshot pour travailler hors du verrou
                snapshotToProcess = dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            });

            // 2. TRAITEMENT I/O EN DEHORS DU VERROU
            // Les autres threads peuvent continuer à appeler SaveFile sans être bloqués par le disque.
            foreach (var kvp in snapshotToProcess)
            {
                string file = kvp.Key;
                XDocument document = kvp.Value;
                string filename = Path.Combine(AppDirectoryManager.SaveCOMDir, file + AppDirectoryManager.ExtensionXML);

                bool saveSuccess = false;

                if (!File.Exists(filename) || !FileSystemHelper.IsFileLocked(filename))
                {
                    FileSystemHelper.NeedAccessFile(filename);
                    try
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.Create))
                        {
                            document.Save(fs);
                        }
                        saveSuccess = true; // L'écriture a réussi
                    }
                    catch (Exception ex)
                    {
                        LogTools.Logger?.Error(ex, $"Le fichier '{filename}' est actuellement utilisé par un autre processus et n'a pas pu être accédé dans le délai imparti.");
                    }
                    finally
                    {
                        FileSystemHelper.ReleaseFile(filename);
                    }
                }

                // 3. NETTOYAGE DU CACHE (Micro-verrou)
                // Si la sauvegarde a fonctionné, on retire le fichier de la file d'attente mémoire.
                if (saveSuccess)
                {
                    _notSave.SafeWriteAction(dict =>
                    {
                        // On vérifie si le dictionnaire contient toujours une entrée pour ce fichier
                        // ET si l'objet en mémoire est exactement celui qu'on vient de sauvegarder.
                        if (dict.TryGetValue(file, out XDocument currentInDict) && currentInDict == document)
                        {
                            dict.Remove(file);
                        }
                        // Si currentInDict != document, cela signifie qu'un autre thread a 
                        // déposé une version plus récente. On ne supprime rien !
                    });
                }
            }
        }
    }
}