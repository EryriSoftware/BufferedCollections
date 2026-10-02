using System;
using System.Buffers;

namespace Eryri.BufferedCollections
{
    public static class ArrayPoolExtensions
    {
        public static IDisposable Rent<T>(this ArrayPool<T> pool, int sizeminimumLength, out T[] result)
        {
            var owner = ArrayOwnerPool<T>.Shared.Rent();
            owner.Disposed = false;
            owner.Pool = pool;
            owner.Value = result = pool.Rent(sizeminimumLength);
            return owner;
        }

        private class ArrayOwnerPool<T> : WeakObjectPool<ArrayOwner<T>>
        {
            public static new ArrayOwnerPool<T> Shared { get; } = new ArrayOwnerPool<T>();
            protected ArrayOwnerPool() : base() { }
        }

        private class ArrayOwner<T> : IDisposable
        {
            internal T[] Value = null;
            internal ArrayPool<T> Pool = null;
            internal bool Disposed;

            ~ArrayOwner()
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
                if (!Disposed)
                {
                    Pool.Return(Value);
                    ArrayOwnerPool<T>.Shared.Return(this);

                    Disposed = true;
                }
            }
        }
    }
}
