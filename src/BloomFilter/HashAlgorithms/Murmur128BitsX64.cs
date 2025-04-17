using System;

namespace BloomFilter.HashAlgorithms;

public class Murmur128BitsX64 : HashFunction
{
    public override HashMethod Method => HashMethod.Murmur128BitsX64;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];

        var uint128 = Internal.Murmur128BitsX64.HashToUInt128Array(data);

        var hash1 = (uint)uint128[0];
        var hash2 = (uint)(uint128[1] >> 32);

        for (int i = 0; i < k; i++)
        {
            positions[i] = (hash1 + i * hash2) % m;
        }
        return positions;
    }
}