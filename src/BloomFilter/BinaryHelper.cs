using System;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

#if NET6_0_OR_GREATER
using System.Buffers.Binary;
#endif

namespace BloomFilter;

internal class BinaryHelper
{
    public static int BitToIntOne(BitArray bit, int from, int to)
    {
        const int size = 32;
        int len = to - from;
        int bitCount = bit.Count;
        int result = 0;

        for (int i = 0; i < len && i < bitCount && i < size; i++)
        {
            result = bit[i + from] ? result + (1 << i) : result;
        }

        return result;
    }

    /// <summary>
    /// Perform rejection sampling on a 32-bit,
    /// https://en.wikipedia.org/wiki/Rejection_sampling
    /// </summary>
    /// <param name="random">The random.</param>
    /// <param name="m">integer output range.</param>
    /// <returns></returns>
    public static long Rejection(long random, uint m)
    {
        var intMax = uint.MaxValue;
        random = Math.Abs(random);
        if (random > (intMax - intMax % m) || random == uint.MinValue)
            return -1;
        return random % m;
    }

    public static uint NumberOfTrailingZeros(uint i)
    {
        // HD, Figure 5-14
        uint y;
        if (i == 0) return 32;
        uint n = 31;
        y = i << 16; if (y != 0) { n -= 16; i = y; }
        y = i << 8; if (y != 0) { n -= 8; i = y; }
        y = i << 4; if (y != 0) { n -= 4; i = y; }
        y = i << 2; if (y != 0) { n -= 2; i = y; }
        return n - ((i << 1) >> 31);
    }

    public static uint NumberOfLeadingZeros(uint i)
    {
        // HD, Figure 5-6
        if (i == 0)
            return 32;
        uint n = 1;
        if (i >> 16 == 0) { n += 16; i <<= 16; }
        if (i >> 24 == 0) { n += 8; i <<= 8; }
        if (i >> 28 == 0) { n += 4; i <<= 4; }
        if (i >> 30 == 0) { n += 2; i <<= 2; }
        n -= i >> 31;
        return n;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RotateLeft(int i, int distance)
    {
#if NET6_0_OR_GREATER
        return (int)BitOperations.RotateLeft((uint)i, distance);
#else
        return (i << distance) | (int)((uint)i >> -distance);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateLeft(uint i, int distance)
    {
#if NET6_0_OR_GREATER
        return BitOperations.RotateLeft(i, distance);
#else
        return (i << distance) | (i >> -distance);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RotateRight(int i, int distance)
    {
#if NET6_0_OR_GREATER
        return (int)BitOperations.RotateRight((uint)i, distance);
#else
        return (int)((uint)i >> distance) | (i << -distance);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateRight(uint i, int distance)
    {
#if NET6_0_OR_GREATER
        return BitOperations.RotateRight(i, distance);
#else
        return (i >> distance) | (i << -distance);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RotateLeft(long i, int distance)
    {
#if NET6_0_OR_GREATER
        return (long)BitOperations.RotateLeft((ulong)i, distance);
#else
        return (i << distance) | (long)((ulong)i >> -distance);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateLeft(ulong i, int distance)
    {
#if NET6_0_OR_GREATER
        return BitOperations.RotateLeft(i, distance);
#else
        return (i << distance) | (i >> -distance);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RotateRight(long i, int distance)
    {
#if NET6_0_OR_GREATER
        return (long)BitOperations.RotateRight((ulong)i, distance);
#else
        return (long)((ulong)i >> distance) | (i << -distance);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateRight(ulong i, int distance)
    {
#if NET6_0_OR_GREATER
        return BitOperations.RotateRight(i, distance);
#else
        return (i >> distance) | (i << -distance);
#endif
    }

    public static int RightMove(int value, int pos)
    {
        if (pos != 0)
        {
            var mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }

    public static long RightMove(long value, int pos)
    {
        if (pos != 0)
        {
            var mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }

    public static uint RightMove(uint value, int pos)
    {
        if (pos != 0)
        {
            uint mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }
}