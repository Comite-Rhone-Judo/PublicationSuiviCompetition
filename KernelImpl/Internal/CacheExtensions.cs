using System.Collections.Generic;

namespace KernelImpl.Internal
{
    internal static class CacheExtensions
    {
        // Route vers la méthode optimisée "Full"
        public static void UpdateFullSnapshot<TKey, TValue>(
            this DeduplicatedCachedData<TKey, TValue> cache,
            IEnumerable<TValue> items)
            where TValue : IEntityWithKey<TKey>
        {
            cache.UpdateFullSnapshot(items, x => x.EntityKey);
        }

        // Route vers la méthode optimisée "Diff"
        public static void UpdateDifferentialSnapshot<TKey, TValue>(
            this DeduplicatedCachedData<TKey, TValue> cache,
            IEnumerable<TValue> items)
            where TValue : IEntityWithKey<TKey>
        {
            cache.UpdateDifferentialSnapshot(items, x => x.EntityKey);
        }
    }
}