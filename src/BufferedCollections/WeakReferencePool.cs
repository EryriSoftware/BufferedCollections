using System;
using System.Collections.Concurrent;
using Eryri.BufferedCollections.Extensions;

namespace Eryri.BufferedCollections
{
    public abstract class WeakReferencePool<T> where T : class
    {
        protected readonly ConcurrentBag<WeakReference<T>> _pool = new ConcurrentBag<WeakReference<T>>();
        private readonly ConcurrentBag<WeakReference<T>> references = new ConcurrentBag<WeakReference<T>>();
        protected WeakReferencePool() { }

        public IDisposable Rent(out T result)
        {
            var rented = result = Rent();
            return new Disposable(rented, this);
        }

        public virtual T Rent()
        {
            while (_pool.TryTake(out var item))
            {
                if (item.TryGetTarget(out var result))
                {
                    references.Add(item);
                    return result;
                }
            }

            return New();
        }

        public virtual void Return(T item)
        {
            if (references.TryTake(out var weakItem))
            {
                weakItem.SetTarget(item);
            }
            else
            {
                weakItem = new WeakReference<T>(item);
            }

            _pool.Add(weakItem);
        }

        protected abstract T New();
        public virtual void Clear()
        {
            references.Clear();
            _pool.Clear();
        }

        private class Disposable : IDisposable
        {
            private readonly T item;
            private readonly WeakReferencePool<T> pool;
            private bool disposed = false;
            public Disposable(T item, WeakReferencePool<T> pool)
            {
                this.item = item;
                this.pool = pool;
            }

            ~Disposable()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    pool.Return(item);
                    disposed = true;
                }
            }
        }
    }
}
