using System;

namespace BloomFilter.HashAlgorithms;

public class Murmur128BitsX86 : HashFunction
{
    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];

        var uint128 = Internal.Murmur128BitsX86.HashToUInt128Array(data);

        var hash1 = uint128[0];
        var hash2 = uint128[1];

        for (int i = 0; i < k; i++)
        {
            positions[i] = (hash1 + i * hash2) % m;
        }
        return positions;
    }
}