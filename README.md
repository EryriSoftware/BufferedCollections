# [Eryri.BufferedCollections](https://www.nuget.org/packages/Eryri.BufferedCollections)

[![NuGet](https://img.shields.io/nuget/v/Eryri.BufferedCollections.svg)](https://www.nuget.org/packages/Eryri.BufferedCollections)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Eryri.BufferedCollections.svg)](https://www.nuget.org/packages/Eryri.BufferedCollections)

Allocation-conscious buffered collections and adaptive pooling for bursty .NET workloads.

`Eryri.BufferedCollections` is a performance-focused library for applications that create and discard temporary collections at high frequency. It is designed for game loops, simulation steps, frame-local work, pathfinding, event fan-out, networking, and other burst-heavy workloads where repeated short-lived allocations can increase garbage-collection pressure and introduce latency or frame-time spikes.

The library focuses on two goals:

- Reuse temporary collections aggressively while they are active.
- Avoid retaining unnecessary memory indefinitely after demand subsides.

This makes it useful when a conventional strong-reference pool retains too much memory after a burst, while allocating new collections creates excessive garbage.

## Why this exists

The .NET ecosystem already provides excellent pooling primitives:

- `ArrayPool<T>` for reusable array buffers.
- `Microsoft.Extensions.ObjectPool` for strongly retained pooled objects.

Those are often the right choices. `Eryri.BufferedCollections` targets a different scenario: temporary collection objects created in bursts, reused heavily for a short period, and then allowed to become collectible when demand falls.

In simplified terms:

- `new List<T>()` is simple, but repeated allocation can create avoidable churn.
- A conventional object pool can reduce allocation, but may retain every object created during a burst.
- An adaptive or weakly retained pool can preserve hot reuse while allowing cold instances to be reclaimed.

## When to use it

This library is intended for high-frequency, allocation-sensitive paths such as:

- Game-engine or server-tick processing.
- ECS and simulation batches.
- Per-frame temporary collections.
- AI and pathfinding result assembly.
- Networking and packet aggregation.
- Transient query and result shaping.
- Event buffering and fan-out.
- Low-latency services with bursty workloads.

It is most appropriate when:

- The same collection types are created repeatedly.
- Collections are short-lived.
- Demand arrives in spikes.
- Allocation or GC behaviour is visible in profiling.
- Memory usage after a burst matters.

## Design principles

`Eryri.BufferedCollections` follows these principles:

1. **Fast rent and return paths** for temporary reusable objects.
2. **Explicit reset semantics** so reused collections do not expose previous state.
3. **Adaptive retention** so burst capacity does not automatically become permanent memory usage.
4. **Thread-safe implementations** where cross-thread pooling is required.
5. **Low-friction APIs** suitable for hot paths.
6. **Benchmark-driven adoption** rather than assuming pooling is always beneficial.

## Installation

```bash
dotnet add package Eryri.BufferedCollections
```

Or add the package reference directly:

```xml
<PackageReference Include="Eryri.BufferedCollections" Version="x.y.z" />
```

Replace `x.y.z` with the version you want to use.

## Quick start

A typical pooled-list usage pattern looks like this:

```csharp
using Eryri.BufferedCollections;

var pool = ListPool<int>.Shared;
var list = pool.Rent();

try
{
    list.Add(1);
    list.Add(2);
    list.Add(3);

    Process(list);
}
finally
{
    pool.Return(list);
}
```

Using the lease API:

```csharp
using Eryri.BufferedCollections;

using var lease = ListPool<int>.Shared.Rent(out var list);

list.Add(42);
list.Add(99);

Consume(list);
```

The package also includes exension methods for ArrayPool<T> for disposable lease support.

```csharp
using Eryri.BufferedCollections;

using var lease = ArrayPool<int>.Shared.Rent(10, out var list);

list.Add(42);
list.Add(99);

Consume(list);
```

The lease returns the collection when disposed. Prefer `using` or `try/finally` so the collection is always returned, including when processing throws.

## Choosing the right pool

| Scenario | Recommended option |
|---|---|
| Reusing raw arrays | `ArrayPool<T>` |
| Reusing strongly retained, expensive objects | `Microsoft.Extensions.ObjectPool` |
| Reusing temporary collections during bursty workloads | `Eryri.BufferedCollections` |
| Infrequent or inexpensive allocations | A normal `new` allocation |

Pooling is not automatically faster. Use this package when profiling shows that allocation, garbage collection, or memory retention affects throughput, latency, or frame stability.

## Important behavior

Pooled collections have ownership rules. Treat a rented collection as exclusively owned by the caller until it is returned.

- Do not use a collection after returning it.
- Do not return the same collection more than once.
- Do not retain references to a collection after returning it.
- Ensure collections are reset before reuse.
- Consider trimming or discarding unusually large collections.
- Weakly retained objects may be reclaimed by the garbage collector at any time.
- Pooling may be slower than allocation for small or inexpensive objects.

For list-heavy workloads, the key trade-off is between immediate reuse and long-term memory retention.

## Example: frame-local temporary lists

A common game-style pattern is assembling temporary results for one frame or simulation tick:

```csharp
using Eryri.BufferedCollections;

using var nearbyLease =
    ListPool<Entity>.Shared.Rent(out var nearby);

using var visibleLease =
    ListPool<Entity>.Shared.Rent(out var visible);

FindNearbyEntities(player, nearby);
FilterVisible(camera, nearby, visible);
RenderHighlights(visible);
```

Both collections are returned automatically at the end of the scope.

## Available APIs

The package is intended to provide reusable collection primitives such as:

- `WeakReferencePool<T>`
- `WeakObjectPool<T>`
- `ListPool<T>`
- `CollectionPool<TCollection, TItem>`
- `ConcurrentBagPool<T>`
- `HashSetPool<T>`

## Guidance

### Use this package when

- Profiling shows significant temporary collection allocation.
- The same collection types recur frequently.
- Workloads are bursty.
- GC pressure or frame-time variance matters.
- A conventional strong pool retains more memory than desired after bursts.

### Prefer another approach when

- Allocations are infrequent.
- Objects are cheap to create.
- Simplicity is more important than allocation reduction.
- Collection ownership is unclear.
- A standard `ArrayPool<T>` or `Microsoft.Extensions.ObjectPool` already fits the workload.

## Thread safety

Pool implementations may be safe for concurrent rent and return operations, but rented collections are not automatically safe for concurrent mutation.

Unless an API explicitly states otherwise:

- Treat each rented collection as owned by one logical operation.
- Do not mutate the same rented collection concurrently.
- Synchronize access when ownership must be shared.
- Return the collection only after all users have finished with it.

## Disclaimer

This library is not a universal replacement for `ArrayPool<T>`, `Microsoft.Extensions.ObjectPool`, or ordinary allocations. Its purpose is to provide an adaptive option for workloads where temporary collection allocations are frequent, bursty, and measurable.

Use it where benchmarks demonstrate a practical benefit.
