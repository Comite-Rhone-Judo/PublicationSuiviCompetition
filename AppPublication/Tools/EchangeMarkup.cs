// 1. LA CLASSE INTELLIGENTE
// Elle contient les données ET la logique de protection.
using System.Diagnostics;

public class EchangeMarkup
{
    // Données d'état (privées pour garantir l'intégrité)
    private long _debutEchange;
    private bool _demandeEmise;
    private bool _reponseRecue;

    // Objet de verrouillage pour protéger cette instance spécifique
    private readonly object _lock = new object();

    // Méthode appelée par le Thread d'Émission
    public void DemandeEmise()
    {
        lock (_lock)
        {
            _debutEchange = Stopwatch.GetTimestamp();
            _demandeEmise = true;
            _reponseRecue = false;
        }
    }

    // Méthode appelée par le Thread de Réception
    // Retourne la latence en ms si succès, ou null si échec/doublon
    public double? ReponseRecue()
    {
        lock (_lock)
        {
            // Vérifications de cohérence
            if (!_demandeEmise) return null; // Reçu sans avoir été envoyé (impossible logiquement mais sécurisé)
            if (_reponseRecue) return null;  // Déjà traité (doublon réseau)

            // Capture du temps de fin
            long finEchange = Stopwatch.GetTimestamp();

            // Marquer comme terminé pour bloquer les futurs appels
            _reponseRecue = true;

            // Calcul (Compatible .NET Core / Framework)
            return (finEchange - _debutEchange) * 1000.0 / Stopwatch.Frequency;
        }
    }
}