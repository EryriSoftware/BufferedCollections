namespace Eryri.BufferedCollections
{
    public class WeakObjectPool<T> : WeakReferencePool<T> where T : class, new()
    {
        public static WeakObjectPool<T> Shared { get; } = Create();
        public static WeakObjectPool<T> Create() => new WeakObjectPool<T>();
        protected WeakObjectPool() : base() { }
        protected override T New() => new T();
    }
}
