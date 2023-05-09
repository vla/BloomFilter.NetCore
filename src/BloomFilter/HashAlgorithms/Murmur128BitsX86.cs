using System;

namespace BloomFilter.HashAlgorithms;

public class Murmur128BitsX86 : HashFunction
{
    public override uint[] ComputeHash(ReadOnlySpan<byte> data, uint m, uint k)
    {
        uint[] positions = new uint[k];

        var uint128 = Internal.Murmur128BitsX86.HashToUInt128Array(data);

        uint hash1 = uint128[0];
        uint hash2 = uint128[1];

        for (int i = 0; i < k; i++)
        {
            positions[i] = (uint)((hash1 + i * hash2) % m);
        }
        return positions;
    }
}