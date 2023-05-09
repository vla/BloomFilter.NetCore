// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers.Binary;

namespace BloomFilter.HashAlgorithms.Internal;

/// <summary>
///   Provides an implementation of the CRC-32 algorithm, as used in
///   ITU-T V.42 and IEEE 802.3.
/// </summary>
/// <remarks>
///   <para>
///     For methods that return byte arrays or that write into spans of bytes, this implementation
///     emits the answer in the Little Endian byte order so that the CRC residue relationship
///     (CRC(message concat CRC(message))) is a fixed value) holds.
///     For CRC-32 this stable output is the byte sequence <c>{ 0x1C, 0xDF, 0x44, 0x21 }</c>,
///     the Little Endian representation of <c>0x2144DF1C</c>.
///   </para>
///   <para>
///     There are multiple, incompatible, definitions of a 32-bit cyclic redundancy
///     check (CRC) algorithm. When interoperating with another system, ensure that you
///     are using the same definition. The definition used by this implementation is not
///     compatible with the cyclic redundancy check described in ITU-T I.363.5.
///   </para>
/// </remarks>
internal partial class Crc32 : NonCryptoHashAlgorithm
{
    private const uint InitialState = 0xFFFF_FFFFu;

    private const int Size = sizeof(uint);

    private uint _crc = InitialState;

    public Crc32() : base(Size)
    {
    }

    public override void Append(ReadOnlySpan<byte> source)
    {
        _crc = Update(_crc, source);
    }

    public override void Reset()
    {
        _crc = InitialState;
    }

    protected override void GetCurrentHashCore(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(destination, ~_crc);
    }

    protected override void GetHashAndResetCore(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(destination, ~_crc);
        _crc = InitialState;
    }

    public uint GetCurrentHashAsUInt32() => ~_crc;

    private static uint Update(uint crc, ReadOnlySpan<byte> source)
    {
#if NET7_0_OR_GREATER
        if (CanBeVectorized(source))
        {
            return UpdateVectorized(crc, source);
        }
#endif

        return UpdateScalar(crc, source);
    }

    private static uint UpdateScalar(uint crc, ReadOnlySpan<byte> source)
    {
#if NET6_0_OR_GREATER
        // Use ARM intrinsics for CRC if available. This is used for the trailing bytes on the vectorized path
        // and is the primary method if the vectorized path is unavailable.
        if (System.Runtime.Intrinsics.Arm.Crc32.Arm64.IsSupported)
        {
            return UpdateScalarArm64(crc, source);
        }

        if (System.Runtime.Intrinsics.Arm.Crc32.IsSupported)
        {
            return UpdateScalarArm32(crc, source);
        }
#endif

        ReadOnlySpan<uint> crcLookup = CrcLookup;
        for (int i = 0; i < source.Length; i++)
        {
            crc = crcLookup[(byte)(crc ^ source[i])] ^ (crc >> 8);
        }

        return crc;
    }
}