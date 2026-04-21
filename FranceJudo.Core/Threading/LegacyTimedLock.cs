using System;
using System.Threading;

namespace FranceJudo.Core.Threading
{
    public readonly struct LegacyTimedLock : IDisposable
    {
        public static LegacyTimedLock Lock(object o)
        {
            return Lock(o, TimeSpan.FromSeconds(10));
        }

        public static LegacyTimedLock Lock(object o, TimeSpan timeout)
        {
            LegacyTimedLock tl = new LegacyTimedLock(o);
            if (!Monitor.TryEnter(o, timeout))
            {
#if DEBUG
                System.GC.SuppressFinalize(tl.leakDetector);
#endif
                throw new LockTimeoutException();
            }

            return tl;
        }

        private LegacyTimedLock(object o)
        {
            target = o;
#if DEBUG
            leakDetector = new Sentinel();
#endif
        }
        private readonly object target;

        public readonly void Dispose()
        {
            Monitor.Exit(target);

            // It's a bad error if someone forgets to call Dispose,
            // so in Debug builds, we put a finalizer in to detect
            // the error. If Dispose is called, we suppress the
            // finalizer.
#if DEBUG
            GC.SuppressFinalize(leakDetector);
#endif
        }

#if DEBUG
        // (In Debug mode, we make it a class so that we can add a finalizer
        // in order to detect when the object is not freed.)
        private class Sentinel
        {
            ~Sentinel()
            {
                // If this finalizer runs, someone somewhere failed to
                // call Dispose, which means we've failed to leave
                // a monitor!
                System.Diagnostics.Debug.Fail("Undisposed lock");
            }
        }
        private readonly Sentinel leakDetector;
#endif

    }
    public class LockTimeoutException : ApplicationException
    {
        public LockTimeoutException() : base("Timeout waiting for lock")
        {
        }
    }
}