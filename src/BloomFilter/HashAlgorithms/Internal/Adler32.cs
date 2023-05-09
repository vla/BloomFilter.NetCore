using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

#if NET6_0_OR_GREATER
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace BloomFilter.HashAlgorithms.Internal;

/// <summary>
///   Adler32 algorithm
/// </summary>
internal sealed partial class Adler32 : NonCryptoHashAlgorithm
{
    private const int Size = sizeof(uint);

    private uint _adler;
    private uint _alderInitial;

    // largest prime smaller than 65536
    private const int MOD32 = 65521;

    // NMAX32 is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
    private const int NMAX32 = 5552;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Adler32"/> class.
    /// </summary>
    public Adler32(uint adler = 1)
        : base(Size)
    {
        _alderInitial = adler;
        _adler = adler;
    }

    /// <summary>
    ///   Appends the contents of <paramref name="source"/> to the data already
    ///   processed for the current hash computation.
    /// </summary>
    /// <param name="source">The data to process.</param>
    public override void Append(ReadOnlySpan<byte> source)
    {
        _adler = Update(_adler, source);
    }

    /// <summary>
    ///   Resets the hash computation to the initial state.
    /// </summary>
    public override void Reset()
    {
        _adler = _alderInitial;
    }

    /// <summary>
    ///   Writes the computed hash value to <paramref name="destination"/>
    ///   without modifying accumulated state.
    /// </summary>
    /// <param name="destination">The buffer that receives the computed hash value.</param>
    protected override void GetCurrentHashCore(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(destination, _adler);
    }

    /// <summary>
    ///   Writes the computed hash value to <paramref name="destination"/>
    ///   then clears the accumulated state.
    /// </summary>
    protected override void GetHashAndResetCore(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(destination, _adler);
        _adler = _alderInitial;
    }

    /// <summary>Gets the current computed hash value without modifying accumulated state.</summary>
    /// <returns>The hash value for the data already provided.</returns>
    public uint GetCurrentHashAsUInt32() => _adler;

    private static uint Update(uint adler, ReadOnlySpan<byte> source)
    {
#if NET6_0_OR_GREATER
        if (Ssse3.IsSupported)
        {
            return GetSse(source, adler);
        }
#endif

        return GetSimple(source, adler);
    }

    internal static uint GetSimple(ReadOnlySpan<byte> source, uint adler)
    {
        var count = source.Length;

        uint s1 = adler & 0xFFFF;
        uint s2 = adler >> 16;

        int offset = 0;

        while (count > 0)
        {
            int n = NMAX32;
            if (n > count)
            {
                n = count;
            }
            count -= n;
            while (--n >= 0)
            {
                s1 = s1 + (uint)(source[offset++] & 0xff);
                s2 = s2 + s1;
            }
            s1 %= MOD32;
            s2 %= MOD32;
        }

        adler = (s2 << 16) | s1;

        return adler;
    }

#if NET6_0_OR_GREATER
    internal static unsafe uint GetSse(ReadOnlySpan<byte> buffer, uint adler)
    {
        const int BLOCK_SIZE = 32;

        uint s1 = adler & 0xFFFF;
        uint s2 = adler >> 16;

        uint len = (uint)buffer.Length;

        uint blocks = len / BLOCK_SIZE;
        len = len - blocks * BLOCK_SIZE;

        Vector128<sbyte> tap1 = Vector128.Create(32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17);
        Vector128<sbyte> tap2 = Vector128.Create(16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1);
        Vector128<byte> zero = Vector128<byte>.Zero;
        Vector128<short> ones = Vector128.Create(1, 1, 1, 1, 1, 1, 1, 1);

        fixed (byte* bufPtr = &MemoryMarshal.GetReference(buffer))
        {
            var buf = bufPtr;

            while (blocks != 0)
            {
                uint n = NMAX32 / BLOCK_SIZE;
                if (n > blocks)
                {
                    n = blocks;
                }

                blocks -= n;

                // Process n blocks of data. At most NMAX data bytes can be
                // processed before s2 must be reduced modulo BASE.
                Vector128<uint> v_ps = Vector128.Create(0, 0, 0, s1 * n);
                Vector128<uint> v_s2 = Vector128.Create(0, 0, 0, s2);
                Vector128<uint> v_s1 = Vector128.Create(0u, 0, 0, 0);

                do
                {
                    // Load 32 input bytes.
                    Vector128<byte> bytes1 = Sse2.LoadVector128(&buf[0]);
                    Vector128<byte> bytes2 = Sse2.LoadVector128(&buf[16]);

                    // Add previous block byte sum to v_ps.
                    v_ps = Sse2.Add(v_ps, v_s1);

                    // Horizontally add the bytes for s1, multiply-adds the
                    // bytes by [ 32, 31, 30, ... ] for s2.
                    Vector128<ushort> sad1 = Sse2.SumAbsoluteDifferences(bytes1, zero);
                    v_s1 = Sse2.Add(v_s1, sad1.AsUInt32());
                    Vector128<short> mad11 = Ssse3.MultiplyAddAdjacent(bytes1, tap1);
                    Vector128<int> mad12 = Sse2.MultiplyAddAdjacent(mad11, ones);
                    v_s2 = Sse2.Add(v_s2, mad12.AsUInt32());

                    Vector128<ushort> sad2 = Sse2.SumAbsoluteDifferences(bytes2, zero);
                    v_s1 = Sse2.Add(v_s1, sad2.AsUInt32());
                    Vector128<short> mad21 = Ssse3.MultiplyAddAdjacent(bytes2, tap2);
                    Vector128<int> mad22 = Sse2.MultiplyAddAdjacent(mad21, ones);
                    v_s2 = Sse2.Add(v_s2, mad22.AsUInt32());

                    buf += BLOCK_SIZE;

                    n--;
                } while (n != 0);

                var shift = Sse2.ShiftLeftLogical(v_ps, 5);
                v_s2 = Sse2.Add(v_s2, shift);

                // Sum epi32 ints v_s1(s2) and accumulate in s1(s2).

                // A B C D -> B A D C
                const int S2301 = 2 << 6 | 3 << 4 | 0 << 2 | 1;
                // A B C D -> C D A B
                const int S1032 = 1 << 6 | 0 << 4 | 3 << 2 | 2;

                v_s1 = Sse2.Add(v_s1, Sse2.Shuffle(v_s1, S2301));
                v_s1 = Sse2.Add(v_s1, Sse2.Shuffle(v_s1, S1032));
                s1 += Sse2.ConvertToUInt32(v_s1);
                v_s2 = Sse2.Add(v_s2, Sse2.Shuffle(v_s2, S2301));
                v_s2 = Sse2.Add(v_s2, Sse2.Shuffle(v_s2, S1032));
                s2 = Sse2.ConvertToUInt32(v_s2);

                s1 %= MOD32;
                s2 %= MOD32;
            }

            if (len > 0)
            {
                if (len >= 16)
                {
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    s2 += (s1 += *buf++);
                    len -= 16;
                }

                while (len-- > 0)
                {
                    s2 += (s1 += *buf++);
                }
                if (s1 >= MOD32)
                {
                    s1 -= MOD32;
                }

                s2 %= MOD32;
            }

            return s1 | (s2 << 16);
        }
    }
#endif
}