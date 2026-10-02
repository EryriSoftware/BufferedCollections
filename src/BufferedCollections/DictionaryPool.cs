using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Eryri.BufferedCollections
{
    public class DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    {
        public static DictionaryPool<TKey, TValue> Share(IEqualityComparer<TKey> comparer) => pools.GetOrAdd(comparer, _ => new DictionaryPool<TKey, TValue>(comparer));

        private static ConcurrentDictionary<IEqualityComparer<TKey>, DictionaryPool<TKey, TValue>> pools = new ConcurrentDictionary<IEqualityComparer<TKey>, DictionaryPool<TKey, TValue>>();
        private IEqualityComparer<TKey> comparer;
        protected DictionaryPool() : base() { }
        protected DictionaryPool(IEqualityComparer<TKey> comparer = null) : this()
        {
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        protected override Dictionary<TKey, TValue> New() => new Dictionary<TKey, TValue>(comparer);
    }
}
