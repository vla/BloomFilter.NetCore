using System;

namespace BloomFilter.HashAlgorithms;

public class XXHash32 : HashFunction
{
    public override uint[] ComputeHash(ReadOnlySpan<byte> data, uint m, uint k)
    {
        uint[] positions = new uint[k];
        uint hash1 = Internal.XxHash32.HashToUInt32(data, 0);
        uint hash2 = Internal.XxHash32.HashToUInt32(data, hash1);
        for (int i = 0; i < k; i++)
        {
            positions[i] = (uint)((hash1 + i * hash2) % m);
        }
        return positions;
    }
}