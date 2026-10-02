using System.Buffers;
using System.Security.Cryptography;
using FluentAssertions;

namespace Eryri.BufferedCollections.Tests;

public class ArrayPoolTests
{
    [Test]
    public void CanLeaseArray()
    {
        // Arrange
        var data = RandomNumberGenerator.GetBytes(10);

        // Act
        var lease1 = ArrayPool<byte>.Shared.Rent(10, out var array);
        data.CopyTo(array.AsSpan(0, 10));
        lease1.Dispose();

        // Assert
        using var lease2 = ArrayPool<byte>.Shared.Rent(10, out array);
        array.AsSpan(0, 10).ToArray().Should().BeEquivalentTo(data, options => options.WithStrictOrdering());
    }
}
