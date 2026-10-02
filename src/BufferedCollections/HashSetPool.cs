using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Eryri.BufferedCollections
{
    public class HashSetPool<T> : CollectionPool<HashSet<T>, T>
    {
        public static HashSetPool<T> Share(IEqualityComparer<T> comparer) => pools.GetOrAdd(comparer, _ => new HashSetPool<T>(comparer));

        private static ConcurrentDictionary<IEqualityComparer<T>, HashSetPool<T>> pools = new ConcurrentDictionary<IEqualityComparer<T>, HashSetPool<T>>();
        private IEqualityComparer<T> comparer;
        protected HashSetPool() : base() { }
        protected HashSetPool(IEqualityComparer<T> comparer = null) : this()
        {
            this.comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected override HashSet<T> New() => new HashSet<T>(comparer);
    }
}
