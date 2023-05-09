using System;

namespace BloomFilter.HashAlgorithms;

public class XXHash64 : HashFunction
{
    public override uint[] ComputeHash(ReadOnlySpan<byte> data, uint m, uint k)
    {
        uint[] positions = new uint[k];

        var hash = Internal.XxHash64.HashToUInt64(data, 0);

        uint hash1 = (uint)hash;
        uint hash2 = (uint)(hash >> 32);

        for (int i = 0; i < k; i++)
        {
            positions[i] = (uint)((hash1 + i * hash2) % m);
        }
        return positions;
    }
}