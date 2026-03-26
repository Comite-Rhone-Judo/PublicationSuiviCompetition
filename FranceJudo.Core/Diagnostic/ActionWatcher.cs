using System;
using System.Diagnostics;

public static class ActionWatcher
{
    /// <summary>
    /// Exécute une action (void) et retourne le temps d'exécution en millisecondes.
    /// </summary>
    public static long Execute(Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        // Démarrage du chronomètre haute précision
        var sw = Stopwatch.StartNew();

        // Exécution directe sur le MÊME thread
        action();

        sw.Stop();

        // Retourne le temps total en millisecondes pas besoin de precision mieux que la Ms
        return (long) Math.Round(sw.Elapsed.TotalMilliseconds);
    }

    /// <summary>
    /// Exécute une fonction (avec retour) et retourne le résultat + le temps.
    /// </summary>
    public static TimedResult<T> Execute<T>(Func<T> function)
    {
        if (function == null) throw new ArgumentNullException(nameof(function));

        var sw = Stopwatch.StartNew();

        // Exécution sur le thread courant
        T result = function();

        sw.Stop();

        return new TimedResult<T>(result, (long) Math.Round(sw.Elapsed.TotalMilliseconds));
    }
}

// Petite structure helper pour retourner le résultat ET le temps proprement
public readonly struct TimedResult<T>
{
    public T Result { get; }
    public long DurationMs { get; }

    public TimedResult(T result, long durationMs)
    {
        Result = result;
        DurationMs = durationMs;
    }
}