// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers.Binary;

// Implemented from the specification at
// https://github.com/Cyan4973/xxHash/blob/f9155bd4c57e2270a4ffbb176485e5d713de1c9b/doc/xxhash_spec.md

namespace BloomFilter.HashAlgorithms.Internal;

/// <summary>
///   Provides an implementation of the XxHash64 algorithm.
/// </summary>
/// <remarks>
/// For methods that persist the computed numerical hash value as bytes,
/// the value is written in the Big Endian byte order.
/// </remarks>
internal sealed partial class XxHash64 : NonCryptoHashAlgorithm
{
    private const int HashSize = sizeof(ulong);
    private const int StripeSize = 4 * sizeof(ulong);

    private readonly ulong _seed;
    private State _state;
    private byte[]? _holdback;
    private long _length;

    /// <summary>
    ///   Initializes a new instance of the <see cref="XxHash64"/> class.
    /// </summary>
    /// <remarks>
    ///   The XxHash64 algorithm supports an optional seed value.
    ///   Instances created with this constructor use the default seed, zero.
    /// </remarks>
    public XxHash64()
        : this(0)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="XxHash64"/> class with
    ///   a specified seed.
    /// </summary>
    /// <param name="seed">
    ///   The hash seed value for computations from this instance.
    /// </param>
    public XxHash64(ulong seed)
        : base(HashSize)
    {
        _seed = seed;
        Reset();
    }

    /// <summary>
    ///   Resets the hash computation to the initial state.
    /// </summary>
    public override void Reset()
    {
        _state = new State(_seed);
        _length = 0;
    }

    /// <summary>
    ///   Appends the contents of <paramref name="source"/> to the data already
    ///   processed for the current hash computation.
    /// </summary>
    /// <param name="source">The data to process.</param>
    public override void Append(ReadOnlySpan<byte> source)
    {
        // Every time we've read 32 bytes, process the stripe.
        // Data that isn't perfectly mod-32 gets stored in a holdback
        // buffer.

        int held = (int)_length & 0x1F;

        if (held != 0)
        {
            int remain = StripeSize - held;

            if (source.Length >= remain)
            {
                source.Slice(0, remain).CopyTo(_holdback.AsSpan(held));
                _state.ProcessStripe(_holdback);

                source = source.Slice(remain);
                _length += remain;
            }
            else
            {
                source.CopyTo(_holdback.AsSpan(held));
                _length += source.Length;
                return;
            }
        }

        while (source.Length >= StripeSize)
        {
            _state.ProcessStripe(source);
            source = source.Slice(StripeSize);
            _length += StripeSize;
        }

        if (source.Length > 0)
        {
            _holdback ??= new byte[StripeSize];
            source.CopyTo(_holdback);
            _length += source.Length;
        }
    }

    /// <summary>
    ///   Writes the computed hash value to <paramref name="destination"/>
    ///   without modifying accumulated state.
    /// </summary>
    protected override void GetCurrentHashCore(Span<byte> destination)
    {
        ulong hash = GetCurrentHashAsUInt64();
        BinaryPrimitives.WriteUInt64BigEndian(destination, hash);
    }

    /// <summary>Gets the current computed hash value without modifying accumulated state.</summary>
    /// <returns>The hash value for the data already provided.</returns>
    public ulong GetCurrentHashAsUInt64()
    {
        int remainingLength = (int)_length & 0x1F;
        ReadOnlySpan<byte> remaining = ReadOnlySpan<byte>.Empty;

        if (remainingLength > 0)
        {
            remaining = new ReadOnlySpan<byte>(_holdback, 0, remainingLength);
        }

        return _state.Complete(_length, remaining);
    }

    /// <summary>Computes the XxHash64 hash of the provided data.</summary>
    /// <param name="source">The data to hash.</param>
    /// <param name="seed">The seed value for this hash computation.</param>
    /// <returns>The computed XxHash64 hash.</returns>
    public static ulong HashToUInt64(ReadOnlySpan<byte> source, ulong seed = 0)
    {
        int totalLength = source.Length;
        State state = new State(seed);

        while (source.Length >= StripeSize)
        {
            state.ProcessStripe(source);
            source = source.Slice(StripeSize);
        }

        return state.Complete((uint)totalLength, source);
    }
}