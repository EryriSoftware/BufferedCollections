using System.Collections.Generic;

namespace Eryri.BufferedCollections
{
    public class ListPool<T> : CollectionPool<List<T>, T>
    {
        protected ListPool() : base() { }
    }
}
