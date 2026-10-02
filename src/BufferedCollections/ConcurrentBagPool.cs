using System.Collections.Concurrent;
using Eryri.BufferedCollections.Extensions;

namespace Eryri.BufferedCollections
{
    public class ConcurrentBagPool<T> : WeakObjectPool<ConcurrentBag<T>>
    {
        public static new ConcurrentBagPool<T> Shared { get; } = Create();
        public static new ConcurrentBagPool<T> Create() => new ConcurrentBagPool<T>();
        protected ConcurrentBagPool() : base() { }

        public override void Return(ConcurrentBag<T> collection)
        {
            collection.Clear();
            base.Return(collection);
        }
    }
}
