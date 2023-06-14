
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers.Binary;

namespace BloomFilter.HashAlgorithms.Internal;

/// <summary>
///   Provides an implementation of the CRC-64 algorithm as described in ECMA-182, Annex B.
/// </summary>
/// <remarks>
///   <para>
///     For methods that return byte arrays or that write into spans of bytes,
///     this implementation emits the answer in the Big Endian byte order so that
///     the CRC residue relationship (CRC(message concat CRC(message))) is a fixed value) holds.
///     For CRC-64 this stable output is the byte sequence
///     <c>{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }</c>.
///   </para>
///   <para>
///     There are multiple, incompatible, definitions of a 64-bit cyclic redundancy
///     check (CRC) algorithm. When interoperating with another system, ensure that you
///     are using the same definition. The definition used by this implementation is not
///     compatible with the cyclic redundancy check described in ISO 3309.
///   </para>
/// </remarks>
internal sealed partial class Crc64 : NonCryptoHashAlgorithm
{
    private const ulong InitialState = 0UL;
    private const int Size = sizeof(ulong);

    private ulong _crc = InitialState;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Crc64"/> class.
    /// </summary>
    public Crc64()
        : base(Size)
    {
    }

    /// <summary>
    ///   Appends the contents of <paramref name="source"/> to the data already
    ///   processed for the current hash computation.
    /// </summary>
    /// <param name="source">The data to process.</param>
    public override void Append(ReadOnlySpan<byte> source)
    {
        _crc = Update(_crc, source);
    }

    /// <summary>
    ///   Resets the hash computation to the initial state.
    /// </summary>
    public override void Reset()
    {
        _crc = InitialState;
    }

    /// <summary>
    ///   Writes the computed hash value to <paramref name="destination"/>
    ///   without modifying accumulated state.
    /// </summary>
    /// <param name="destination">The buffer that receives the computed hash value.</param>
    protected override void GetCurrentHashCore(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt64BigEndian(destination, _crc);
    }

    /// <summary>
    ///   Writes the computed hash value to <paramref name="destination"/>
    ///   then clears the accumulated state.
    /// </summary>
    protected override void GetHashAndResetCore(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt64BigEndian(destination, _crc);
        _crc = InitialState;
    }

    /// <summary>Gets the current computed hash value without modifying accumulated state.</summary>
    /// <returns>The hash value for the data already provided.</returns>
    public ulong GetCurrentHashAsUInt64() => _crc;

    public static ulong HashToUInt64(ReadOnlySpan<byte> source) =>
        Update(InitialState, source);

    private static ulong Update(ulong crc, ReadOnlySpan<byte> source)
    {
#if NET7_0_OR_GREATER
        if (CanBeVectorized(source))
        {
            return UpdateVectorized(crc, source);
        }
#endif

        return UpdateScalar(crc, source);
    }

    private static ulong UpdateScalar(ulong crc, ReadOnlySpan<byte> source)
    {
        ReadOnlySpan<ulong> crcLookup = CrcLookup;
        for (int i = 0; i < source.Length; i++)
        {
            ulong idx = (crc >> 56);
            idx ^= source[i];
            crc = crcLookup[(int)idx] ^ (crc << 8);
        }

        return crc;
    }
}
