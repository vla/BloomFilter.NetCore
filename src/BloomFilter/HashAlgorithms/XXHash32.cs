using System;

namespace BloomFilter.HashAlgorithms;

public class XXHash32 : HashFunction
{
    public override HashMethod Method => HashMethod.XXHash32;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];
        uint hash1 = Internal.XxHash32.HashToUInt32(data, 0);
        uint hash2 = Internal.XxHash32.HashToUInt32(data, hash1);
        for (int i = 0; i < k; i++)
        {
            positions[i] = ((hash1 + (i * hash2)) % m);
        }
        return positions;
    }
}