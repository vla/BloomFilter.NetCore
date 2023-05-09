using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BloomFilter.HashAlgorithms.Internal;

/// <summary>
/// 表示非加密散列算法。
/// </summary>
internal abstract class NonCryptoHashAlgorithm
{
    /// <summary>
    ///   获取由此散列算法产生的字节数。
    /// </summary>
    /// <value>这个散列算法产生的字节数。</value>
    public int HashLengthInBytes { get; }

    protected NonCryptoHashAlgorithm(int hashLengthInBytes)
    {
        if (hashLengthInBytes < 1)
            throw new ArgumentOutOfRangeException(nameof(hashLengthInBytes));

        HashLengthInBytes = hashLengthInBytes;
    }

    public abstract void Append(ReadOnlySpan<byte> source);

    public abstract void Reset();

    protected abstract void GetCurrentHashCore(Span<byte> destination);

    public void Append(byte[] source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Append(new ReadOnlySpan<byte>(source));
    }

    protected virtual void GetHashAndResetCore(Span<byte> destination)
    {
        Debug.Assert(destination.Length == HashLengthInBytes);

        GetCurrentHashCore(destination);
        Reset();
    }

#if NET6_0_OR_GREATER
    [DoesNotReturn]
#endif

    protected static void ThrowDestinationTooShort() =>
          throw new ArgumentException("Destination is too short.", "destination");
}