using System.Collections.Generic;

namespace Eryri.BufferedCollections
{
    public class CollectionPool<TCollection, TItem> : WeakObjectPool<TCollection> where TCollection : class, ICollection<TItem>, new()
    {
        public static new CollectionPool<TCollection, TItem> Shared { get; } = Create();
        public static new CollectionPool<TCollection, TItem> Create() => new CollectionPool<TCollection, TItem>();
        protected CollectionPool() : base() { }

        public override void Return(TCollection collection)
        {
            collection.Clear();
            base.Return(collection);
        }
    }
}
