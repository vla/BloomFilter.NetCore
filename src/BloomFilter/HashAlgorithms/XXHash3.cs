using System;

namespace BloomFilter.HashAlgorithms;

public class XXHash3 : HashFunction
{
    public override HashMethod Method => HashMethod.XXHash3;

    public override long[] ComputeHash(ReadOnlySpan<byte> data, long m, int k)
    {
        long[] positions = new long[k];

        var hash = Internal.XxHash3.HashToUInt64(data, 0);

        uint hash1 = (uint)hash;
        uint hash2 = (uint)(hash >> 32);

        for (int i = 0; i < k; i++)
        {
            positions[i] = ((hash1 + i * hash2) % m);
        }
        return positions;
    }
}