using System.Collections.Concurrent;

namespace Eryri.BufferedCollections.Extensions
{
    internal static class ConcurrentBagExtensions
    {
        public static void Clear<T>(this ConcurrentBag<T> bag)
        {
            while (bag.TryTake(out _))
            {
            }
        }
    }
}
